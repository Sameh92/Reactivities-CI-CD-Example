using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    public class PagingParams
    {
        private const int MaxPageSize = 50;

        // auto generated property 
        public int PageNumber { get; set; } = 1; // the initial value is 1


        //full property with a backing filed  
        private int _pageSize = 10;


        //public property
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}