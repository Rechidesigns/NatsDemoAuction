using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatsDemo.Core.Model
{
    public class Car
    {
        [Key]
        public string Id { get; set; } 
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }

    public class Auction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CarId { get; set; }
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }

    public class Bid
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AuctionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public Auction Auction { get; set; }
    }


}
