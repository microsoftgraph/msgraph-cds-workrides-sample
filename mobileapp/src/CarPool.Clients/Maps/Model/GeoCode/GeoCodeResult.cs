using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Maps.Model.GeoCode
{
    public class GeoCodeResult
    {
        public IEnumerable<GeoCodeAddress> Results {get;set;}

        public string Status { get; set; }
    }
}
