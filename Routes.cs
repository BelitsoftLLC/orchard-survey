using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Belitsoft.Orchard.Survey
{
    public class RoutesIRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
                       {
                           new RouteDescriptor
                               {
                                   Priority = 5,
                                   Route = new Route( "Survey",
                                                     new RouteValueDictionary
                                                         {
                                                             {"area", "Belitsoft.Orchard.Survey" },
                                                             {"controller", "Survey"},
                                                             {"action", "Add" }
                                                         },
                                                     new RouteValueDictionary(),
                                                     new RouteValueDictionary {{"area", "Belitsoft.Orchard.Survey" }},
                                                     new MvcRouteHandler())
                               }
                       };
        }
    }


}