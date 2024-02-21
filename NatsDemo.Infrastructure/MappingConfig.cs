using AutoMapper;
using NatsDemo.Core.DTO;
using NatsDemo.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatsDemo.Infrastructure
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Auction, AuctionCreateDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
