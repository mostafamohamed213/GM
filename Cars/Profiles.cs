using AutoMapper;
using Cars.Models;
using Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Models.OrderDetails, ViewModels.LaborOrdersViewModel>().ReverseMap();
            CreateMap<PagingViewModel<OrderDetails>, PagingViewModel<LaborOrdersViewModel>>().ReverseMap();

        }
    }
}
