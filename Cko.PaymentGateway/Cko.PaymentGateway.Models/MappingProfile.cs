using AutoMapper;
using Cko.PaymentGateway.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PaymentRequest, Payment>();
        }
    }
}
