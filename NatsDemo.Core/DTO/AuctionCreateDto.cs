

namespace NatsDemo.Core.DTO
{
    public class AuctionCreateDto
    {
        public string CarId { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class BidDto
    {
        public string AuctionId { get; set; }
        public decimal BidAmount { get; set; }
    }

}
