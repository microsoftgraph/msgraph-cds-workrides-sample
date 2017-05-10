using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Pushpins;
using CarPool.Clients.Core.Maps.Model;

namespace CarPool.Clients.Core.Maps.Routes
{
    public abstract class AbstractRouteManager
    {
        private readonly RoutesUpdater _routeUpdater;
        protected readonly CustomMap FormsMap;
        protected readonly AbstractPushpinManager PushpinManager;

        protected AbstractRouteManager(CustomMap formsMap, AbstractPushpinManager pushpinManager)
        {
            FormsMap = formsMap;
            PushpinManager = pushpinManager;
            _routeUpdater = new RoutesUpdater()
            {
                UpdateInterval = TimeSpan.FromMilliseconds(10),
                UserSpeed = 200
            };
        }

        public RoutesUpdater RouteUpdater
        {
            get
            {
                return _routeUpdater;
            }
        }


        public abstract void ClearAllUserRoutes();

        public async Task DrawRouteInMap(RouteModel route)
        {
            if (route.From == null || route.To == null)
            {
                return;
            }

            if (route.RoutePoints == null || !route.RoutePoints.Any())
            {
                route.RoutePoints = await CalculateRoute(route);
                route.RouteCalculated();
            }

            DrawCalculatedRouteInMap(route);
        }

        public abstract IEnumerable<Position> GetCurrentUserRoute();

        public abstract Task<IEnumerable<Position>> CalculateRoute(RouteModel route);
        
        protected abstract void DrawCalculatedRouteInMap(RouteModel route);
    }
}