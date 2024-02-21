using NatsDemo.Core.DTO;
using NatsDemo.Core.Model;


namespace NatsDemo.Infrastructure.Interface
{
    public interface IAuctionService
    {
        Task<Auction> CreateAuctionAsync(AuctionCreateDto auctionCreateDto);
        Task<Auction> UpdateAuctionAsync(string Id, AuctionCreateDto auctionCreateDto);
        Task<string> DeleteAuctionAsync(string Id);
        Task<Auction> GetAuctionAsync(string Id);
        Task<IEnumerable<Auction>> GetAllAuctionsAsync();
        Task<string> PlaceBidAsync(BidDto bidDto);
    }
}
