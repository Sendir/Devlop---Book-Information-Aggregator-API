using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bookapi.Query
{
    public class PaginationOptions
    {
        [Range(1, int.MaxValue)]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue)]
        [DefaultValue(5)]
        public int PageSize { get; set; } = 5;
    }
}