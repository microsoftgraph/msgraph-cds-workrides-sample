using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps.Routes
{
    public abstract class Route
    {
        private string _id;
        private int _lastRoutePositionIndex;
        private List<Position> _routePositions;
        private Position _currentPosition;

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public int LastRoutePositionIndex
        {
            get
            {
                return _lastRoutePositionIndex;
            }
        }

        public Position CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
        }

        public Position NextPosition
        {
            get
            {
                return _lastRoutePositionIndex < TotalPositions - 2
                    ? _routePositions[_lastRoutePositionIndex + 1]
                    : _routePositions[TotalPositions - 1];
            }
        }

        public List<Position> RoutePositions
        {
            get
            {
                return _routePositions;
            }
        }

        public int TotalPositions
        {
            get
            {
                return _routePositions.Count;
            }
        }

        public bool ArrivedToDestination
        {
            get
            {
                return _lastRoutePositionIndex == TotalPositions - 1;
            }
        }

        public Route(Position[] routePositions)
        {
            if (!routePositions.Any())
            {
                throw new ArgumentException("Error creating Route: positions array length is zero");
            }

            _id = Guid.NewGuid().ToString();

            _routePositions = routePositions.ToList();
        }

        public void Init()
        {
            _lastRoutePositionIndex = 0;
            _currentPosition = RoutePositions[0];
        }

        public void AddStartPoint(Position start)
        {
            _routePositions.Insert(0, start);
        }

        public void AddRouteToStartPoint(IEnumerable<Position> startRoute)
        {
            _routePositions.InsertRange(0, startRoute);
        }

        public void MoveToNextPosition()
        {
            if (!ArrivedToDestination)
            {
                _currentPosition = NextPosition;

                if (_lastRoutePositionIndex < TotalPositions - 1)
                {
                    _lastRoutePositionIndex++;
                }
            }
        }

        public void MoveToPosition(Position position)
        {
            _currentPosition = position;
        }
    }

    public class Route<T> : Route
    {
        public T Element { get; set; }

        public Route(Position[] routePositions) : base(routePositions)
        {
        }
    }
}