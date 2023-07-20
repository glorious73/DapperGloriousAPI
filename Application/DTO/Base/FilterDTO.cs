using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Base
{
    public class FilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DateTime? CreatedStart { get; set; }
        public DateTime? CreatedEnd { get; set; }
        public string? Search { get; set; }
    }
}
