using CarPool.Clients.Core.Models;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Helpers
{
    public class DriverHelper
    {
        public static IEnumerable<string> GetRiderRequests(Driver driver)
        {
            List<string> result = new List<string>();

            if (driver != null)
            {
                if (!string.IsNullOrEmpty(driver.Rider1) && !driver.Rider1Status)
                {
                    result.Add(driver.Rider1);
                }
                if (!string.IsNullOrEmpty(driver.Rider2) && !driver.Rider2Status)
                {
                    result.Add(driver.Rider2);
                }
                if (!string.IsNullOrEmpty(driver.Rider3) && !driver.Rider3Status)
                {
                    result.Add(driver.Rider3);
                }
                if (!string.IsNullOrEmpty(driver.Rider4) && !driver.Rider4Status)
                {
                    result.Add(driver.Rider4);
                }
            }

            return result;
        }

        public static IEnumerable<string> GetRiders(Driver driver)
        {
            List<string> result = new List<string>();

            if (driver != null)
            {
                if (!string.IsNullOrEmpty(driver.Rider1) && driver.Rider1Status)
                {
                    result.Add(driver.Rider1);
                }
                if (!string.IsNullOrEmpty(driver.Rider2) && driver.Rider2Status)
                {
                    result.Add(driver.Rider2);
                }
                if (!string.IsNullOrEmpty(driver.Rider3) && driver.Rider3Status)
                {
                    result.Add(driver.Rider3);
                }
                if (!string.IsNullOrEmpty(driver.Rider4) && driver.Rider4Status)
                {
                    result.Add(driver.Rider4);
                }
            }

            return result;
        }
    }
}
