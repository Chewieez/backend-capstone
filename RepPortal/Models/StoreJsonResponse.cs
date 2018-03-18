using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models
{
    public class StoreJsonResponse
    {
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int StoreId { get; set; }
        public string StreetAddress { get; set; }
        public string CityStateZip { get; set; }
        public int StatusId { get; set; }
    }
}
