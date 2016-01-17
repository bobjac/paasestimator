using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn
{
    public static class Extensions
    {
        public static string GetResourceType(this IOrderedEnumerable<LoadTestSnapShot> snapshots)
        {
            string resourceType = string.Empty;

            var first = snapshots.FirstOrDefault();
            if (first != null )
            {
                resourceType = first.ResourceType;
            }

            return resourceType;
        }

        public static IEnumerable<IOrderedEnumerable<LoadTestSnapShot>> GetOrderedWebsites(this IEnumerable<IOrderedEnumerable<LoadTestSnapShot>> orderedSnapshots)
        {
            foreach (var current in orderedSnapshots)
            {
                if (current.GetResourceType() == "WebSite")
                {
                    yield return current;
                }
            }
        }
    }
}
