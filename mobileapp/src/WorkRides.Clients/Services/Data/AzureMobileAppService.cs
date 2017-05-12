using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarPool.Clients.Core.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.Sync;
// using Plugin.Connectivity;
// using Newtonsoft.Json.Linq;
// using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace CarPool.Clients.Core.Services.Data
{
    public class AzureMobileAppService : IDataService
    {
        private MobileServiceClient _client;
        private IMobileServiceTable<Driver> _driverTable;
        private IMobileServiceTable<Employee> _employeeTable;
        private IMobileServiceTable<RideDetails> _rideDetailsTable;
        // private IMobileServiceSyncTable<Driver> _driverSyncTable;

        public AzureMobileAppService()
        {
            if (_client != null)
            {
                return;
            }

            // Create our client
            _client = new MobileServiceClient("http://<INSERT YOUR EASY TABLES>/");

            _employeeTable = _client.GetTable<Employee>();
            _rideDetailsTable = _client.GetTable<RideDetails>();
            _driverTable = _client.GetTable<Driver>();
        }

        public async Task InitializeAsync(string token = "")
        {
            await Task.Delay(0);

            Debug.WriteLine(token);
                       
            /*
            // Setup our local SQLite store and initialize our table
            var store = new MobileServiceSQLiteStore(AppSettings.OfflineDatabaseName);
            store.DefineTable<Driver>();

            // Get our sync table that will call out to azure
            _driverSyncTable = _client.GetSyncTable<Driver>();

            await _client.SyncContext.InitializeAsync(store,
                new MobileServiceSyncHandler());
            */
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            try
            {
                await InitializeAsync();
                // await SynchronizeAsync();
                var drivers = await _driverTable.ReadAsync();
                return drivers;
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine(String.Format("Cannot read from remote table: {0}", ex.Message));

                return new List<Driver>();
            }
            catch(Exception ex)
            {
                Debug.WriteLine( ex.Message);

                return new List<Driver>();
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                await InitializeAsync();
                return await _employeeTable.ReadAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine(String.Format("Cannot read from remote table: {0}", ex.Message));

                return new List<Employee>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new List<Employee>();
            }
        }

        public async Task<IEnumerable<RideDetails>> GetAllRideDetailsAsync()
        {
            try
            {
                await InitializeAsync();
                return await _rideDetailsTable.ReadAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                Debug.WriteLine(String.Format("Cannot read from remote table: {0}", ex.Message));

                return new List<RideDetails>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new List<RideDetails>();
            }
        }

        public async Task InsertAsync<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                await InitializeAsync();
                await _driverTable.InsertAsync(item as Driver);
                // await SynchronizeAsync();
            }
            if (itemType == typeof(Employee))
            {
                await _employeeTable.InsertAsync(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                await _rideDetailsTable.InsertAsync(item as RideDetails);
            }
        }

        public async Task InsertOrUpdateRideDetailsAsync(RideDetails rideDetail)
        {
            if (string.IsNullOrEmpty(rideDetail.Id))
            {
                await _rideDetailsTable.InsertAsync(rideDetail);
            }
            else
            {
                await _rideDetailsTable.UpdateAsync(rideDetail);
            }
        }

        public async Task InsertOrUpdateDriverAsync(Driver item)
        {
            try
            {
                await InitializeAsync();

                if (string.IsNullOrEmpty(item.Id))
                {
                    await _driverTable.InsertAsync(item);
                }
                else
                {
                    await _driverTable.UpdateAsync(item);
                }

                // await SynchronizeAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task InsertOrUpdateEmployeeAsync(Employee item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await _employeeTable.InsertAsync(item);
            }
            else
            {
                await _employeeTable.UpdateAsync(item);
            }
        }

        public async Task Remove<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                await InitializeAsync();
                await _driverTable.DeleteAsync(item as Driver);
                // await SynchronizeAsync();
            }
            if (itemType == typeof(Employee))
            {
                await _employeeTable.DeleteAsync(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                await _rideDetailsTable.DeleteAsync(item as RideDetails);
            }
        }

        public async Task AddRiderAsync(Driver driver, string rider)
        {
            var existingItem = (await _driverTable.ReadAsync())
                .Where(x => x.Name == driver.Name)
                .FirstOrDefault();

            if (existingItem != null)
            {
                if (string.IsNullOrEmpty(existingItem.Rider1))
                {
                    existingItem.Rider1 = rider;
                    existingItem.Rider1Status = false;
                }
                else if (string.IsNullOrEmpty(existingItem.Rider2))
                {
                    existingItem.Rider2 = rider;
                    existingItem.Rider2Status = false;
                }
                else if (string.IsNullOrEmpty(existingItem.Rider3))
                {
                    existingItem.Rider3 = rider;
                    existingItem.Rider3Status = false;
                }
                else
                {
                    existingItem.Rider4 = rider;
                    existingItem.Rider4Status = false;
                }

                await _driverTable.UpdateAsync(existingItem).ConfigureAwait(false);
            }
        }

        public async Task UpdateRiderStatusAsync(Driver driver, string rider)
        {
            var existingItem = (await _driverTable.ReadAsync())
                 .Where(x => x.Name == driver.Name)
                 .FirstOrDefault();

            if (existingItem != null)
            {
                if (rider.Equals(existingItem.Rider1))
                {
                    existingItem.Rider1Status = true;
                }
                if (rider.Equals(existingItem.Rider2))
                {
                    existingItem.Rider2Status = true;
                }
                if (rider.Equals(existingItem.Rider3))
                {
                    existingItem.Rider3Status = true;
                }
                if (rider.Equals(existingItem.Rider4))
                {
                    existingItem.Rider4Status = true;
                }

                await _driverTable.UpdateAsync(existingItem).ConfigureAwait(false);
            }
        }

        public async Task RejectRiderAsync(Driver driver, string rider)
        {
            var existingItem = (await _driverTable.ReadAsync())
                 .Where(x => x.Name == driver.Name)
                 .FirstOrDefault();

            if (existingItem != null)
            {
                if (rider.Equals(existingItem.Rider1))
                {
                    existingItem.Rider1 = null;
                    existingItem.Rider1Status = false;
                }
                if (rider.Equals(existingItem.Rider2))
                {
                    existingItem.Rider2 = null;
                    existingItem.Rider2Status = false;
                }
                if (rider.Equals(existingItem.Rider3))
                {
                    existingItem.Rider3 = null;
                    existingItem.Rider3Status = false;
                }
                if (rider.Equals(existingItem.Rider4))
                {
                    existingItem.Rider4 = null;
                    existingItem.Rider4Status = false;
                }

                await _driverTable.UpdateAsync(existingItem).ConfigureAwait(false);
            }
        }

        /*
        private async Task SynchronizeAsync()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                   return;
                       
                await _driverTable.PullAsync($"all{nameof(Driver)}", _driverTable.CreateQuery());

                await _client.SyncContext.PushAsync();
            }
            catch (MobileServicePushFailedException ex)
            {
                if (ex.PushResult.Status == MobileServicePushStatus.CancelledByAuthenticationError)
                {
                    await SynchronizeAsync();
                    return;
                }

                if (ex.PushResult != null)
                    foreach (var result in ex.PushResult.Errors)
                        await ResolveErrorAsync(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Sync Failed: {0}", ex.Message);
            }
        }

        private async Task ResolveErrorAsync(MobileServiceTableOperationError result)
        {
            if (result.Result == null || result.Item == null)
                return;

            var serverItem = result.Result.ToObject<Driver>();
            var localItem = result.Item.ToObject<Driver>();

            if (serverItem.Name == localItem.Name
                && serverItem.Id == localItem.Id)
            {
                await result.CancelAndDiscardItemAsync();
            }
            else
            {
                localItem.AzureVersion = serverItem.AzureVersion;
                await result.UpdateOperationAsync(JObject.FromObject(localItem));
            }
        }
        */
    }
}