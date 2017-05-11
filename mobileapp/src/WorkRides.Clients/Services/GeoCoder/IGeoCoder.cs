using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Services.GeoCoder
{
    interface IGeoCoder
    {
        Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address);
    }
}
