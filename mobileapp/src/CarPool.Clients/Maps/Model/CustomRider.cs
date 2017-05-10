using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps.Model
{
    public class CustomRider : CustomPin
    {
        public Color Color { get; set; }

        public string Acronym { get; set; }
    }
}
