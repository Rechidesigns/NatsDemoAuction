using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NatsDemo.Core.Data;
using NatsDemo.Core.DTO;
using NatsDemo.Core.Model;
using NatsDemo.Infrastructure.Interface;
using Newtonsoft.Json;
using NATS.Client.JetStream;
using System.Text;
using NATS.Client;

namespace NatsDemo.Infrastructure.Implementation
{
    public class AuctionService : IAuctionService
    {
        private readonly NatsDemoDbContext _context;
        private readonly IConnection _natsConnection;
        private IMapper _mapper;

        public AuctionService(NatsDemoDbContext context, IConnection natsConnection, IMapper mapper)
        {
            _context = context;
            _natsConnection = natsConnection;
            _mapper = mapper;
        }
        private bool isMessagePublished = false;
        public async Task<Auction> CreateAuctionAsync(AuctionCreateDto auctionCreateDto)
        {
            var carExists = await _context.Carss.AnyAsync(c => c.Id == auctionCreateDto.CarId.ToString());
            if (!carExists)
            {
                throw new ArgumentException("CarId does not exist.");
            }

            var auction = new Auction
            {
                CarId = auctionCreateDto.CarId,
                StartingPrice = auctionCreateDto.StartingPrice,
                StartTime = auctionCreateDto.StartTime,
                EndTime = auctionCreateDto.EndTime
            };

            _context.Auctions.Add(auction);
            await _context.SaveChangesAsync();

            auction = await _context.Auctions.Include(a => a.Car)
                    .FirstOrDefaultAsync(a => a.Id == auction.Id);

            var jetStream = _natsConnection.CreateJetStreamContext();

            // Serializing auction data including related Car entity details
            var auctionData = JsonConvert.SerializeObject(new
            {
                auction.Id,
                auction.CarId,
                auction.StartingPrice,
                auction.StartTime,
                auction.EndTime,
                Car = new { auction.Car.Make, auction.Car.Model } // Assuming Car entity has Make and Model properties
            });
            var messageData = Encoding.UTF8.GetBytes(auctionData);

            // Preparing and publishing the message using JetStream
            if (!isMessagePublished)
            {
                // Publish the message
                await jetStream.PublishAsync("auction.created", messageData);
                isMessagePublished = true; // Set the flag to indicate that the message has been published
            }

            return auction;
        }

        public async Task<string> DeleteAuctionAsync(string Id)
        {
            var auction = await _context.Auctions.FindAsync(Id);
            if (auction != null)
            {
                _context.Auctions.Remove(auction);
                await _context.SaveChangesAsync();
                return "Auction deleted successfully";
            }
            return "can not find auction";
        }

        public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
        {
            var auctions = await _context.Auctions
                .Include(a => a.Car).ToListAsync();
            return auctions;
        }

        public async Task<Auction> GetAuctionAsync(string Id)
        {
            var auction = await _context.Auctions
                .Include(a => a.Car).FirstOrDefaultAsync(a => a.Id == Id);
            return auction;
        }

        public async Task<string> PlaceBidAsync(BidDto bidDto)
        {
            var auction = await _context.Auctions
                                        .Include(a => a.Bids)
                                        .FirstOrDefaultAsync(a => a.Id == bidDto.AuctionId);

            if (auction == null)
            {
                return "Auction not found.";
            }

            if (DateTime.UtcNow > auction.EndTime)
            {
                return "Bidding period has ended.";
            }

            var highestBid = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : 0;
            if (bidDto.BidAmount <= highestBid)
            {
                return $"Bid must be higher than the current highest bid of {highestBid}.";
            }

            var bid = new Bid
            {
                AuctionId = bidDto.AuctionId,
                Amount = bidDto.BidAmount,
                BidTime = DateTime.UtcNow
                // Add other necessary fields like BidderId
            };
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            // Publish an event to NATS
            var message = $"New bid of {bidDto.BidAmount} placed on Auction {bidDto.AuctionId}.";
            var messageData = Encoding.UTF8.GetBytes(message);
            _natsConnection.Publish("auction.bidPlaced", messageData);

            return "Bid placed successfully.";
        }


        public async Task<Auction> UpdateAuctionAsync(string Id, AuctionCreateDto auctionCreateDto)
        {
            var auction = await _context.Auctions.FindAsync(Id);
            if (auction == null)
            {
                return null;
            }
            _mapper.Map(auctionCreateDto, auction);

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            var message = $"Auction {auction.Id} updated successfully";
            var messageData = Encoding.UTF8.GetBytes(message);
            _natsConnection.Publish("auction.updated", messageData);

            return auction;
        }
    }
}
