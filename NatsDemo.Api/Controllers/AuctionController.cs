using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NatsDemo.Core.DTO;
using NatsDemo.Infrastructure.Interface;

namespace NatsDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] AuctionCreateDto auctionCreateDto)
        {
            var auction = await _auctionService.CreateAuctionAsync(auctionCreateDto);
            return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction(string id, [FromBody] AuctionCreateDto auctionCreateDto)
        {
            var updatedAuction = await _auctionService.UpdateAuctionAsync(id, auctionCreateDto);
            if (updatedAuction == null) return NotFound("Auction not found.");
            return Ok(updatedAuction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(string id)
        {
            var message = await _auctionService.DeleteAuctionAsync(id);
            if (message == "Auction deleted successfully") return Ok(message);
            return NotFound(message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuction(string id)
        {
            var auction = await _auctionService.GetAuctionAsync(id);
            if (auction == null) return NotFound("Auction not found.");
            return Ok(auction);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuctions()
        {
            var auctions = await _auctionService.GetAllAuctionsAsync();
            return Ok(auctions);
        }

        [HttpPost("bid")]
        public async Task<IActionResult> PlaceBid([FromBody] BidDto bidDto)
        {
            var result = await _auctionService.PlaceBidAsync(bidDto);
            if (result == "Bid placed successfully.") return Ok(result);
            return BadRequest(result);
        }
    }
}
