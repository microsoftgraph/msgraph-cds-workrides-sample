using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using SQLite.Net.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Services.Data
{
    public class SQLiteService : IDataService
    {
        private static readonly AsyncLock Mutex = new AsyncLock();
        private SQLiteAsyncConnection _sqlCon;

        public SQLiteService()
        {
            _sqlCon = DependencyService.Get<ISQLite>().GetConnection();

            CreateDatabaseAsync();
        }

        public async Task InitializeAsync(string token = "")
        {
            await Task.Delay(500);

            Debug.WriteLine(token);
        }

        public async void CreateDatabaseAsync()
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                await _sqlCon.CreateTableAsync<Driver>().ConfigureAwait(false);
                await _sqlCon.CreateTableAsync<Employee>().ConfigureAwait(false);
                await _sqlCon.CreateTableAsync<RideDetails>().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            var items = new List<Driver>();
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                items = await _sqlCon.Table<Driver>().ToListAsync().ConfigureAwait(false);
            }

            return items;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var items = new List<Employee>();
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                items = await _sqlCon.Table<Employee>().ToListAsync().ConfigureAwait(false);
            }

            return items;
        }

        public async Task<IEnumerable<RideDetails>> GetAllRideDetailsAsync()
        {
            var items = new List<RideDetails>();
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                items = await _sqlCon.Table<RideDetails>().ToListAsync().ConfigureAwait(false);
            }

            return items;
        }

        public async Task InsertOrUpdateDriverAsync(Driver item)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<Driver>()
                    .Where(x => x.Name == item.Name)
                    .FirstOrDefaultAsync();

                if (existingItem == null)
                {
                    await _sqlCon.InsertAsync(item).ConfigureAwait(false);
                }
                else
                {
                    item.Name = existingItem.Name;
                    await _sqlCon.UpdateAsync(item).ConfigureAwait(false);
                }
            }
        }

        public async Task InsertOrUpdateRideDetailsAsync(RideDetails rideDetail)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<RideDetails>()
                        .Where(x => x.Driver == rideDetail.Driver)
                        .FirstOrDefaultAsync();

                if (existingItem == null)
                {
                    await _sqlCon.InsertAsync(rideDetail).ConfigureAwait(false);
                }
                else
                {
                    // TODO:
                    // rideDetail.Driver = existingItem.Driver;
                    // await _sqlCon.UpdateAsync(rideDetail).ConfigureAwait(false);
                }
            }
        }

        public async Task InsertOrUpdateEmployeeAsync(Employee item)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<Employee>()
                        .Where(x => x.Name == item.Name)
                        .FirstOrDefaultAsync();

                if (existingItem == null)
                {
                    await _sqlCon.InsertAsync(item).ConfigureAwait(false);
                }
                else
                {
                    item.Name = existingItem.Name;
                    await _sqlCon.UpdateAsync(item).ConfigureAwait(false);
                }
            }
        }

        public async Task InsertAsync<T>(T item)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                await _sqlCon.InsertAsync(item).ConfigureAwait(false);
            }
        }

        public async Task AddRiderAsync(Driver driver, string rider)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<Driver>()
                    .Where(x => x.Name == driver.Name)
                    .FirstOrDefaultAsync();

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

                    await _sqlCon.UpdateAsync(existingItem).ConfigureAwait(false);
                }
            }
        }

        public async Task UpdateRiderStatusAsync(Driver driver, string rider)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<Driver>()
                    .Where(x => x.Name == driver.Name)
                    .FirstOrDefaultAsync();

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

                    await _sqlCon.UpdateAsync(existingItem).ConfigureAwait(false);
                }
            }
        }

        public async Task RejectRiderAsync(Driver driver, string rider)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var existingItem = await _sqlCon.Table<Driver>()
                    .Where(x => x.Name == driver.Name)
                    .FirstOrDefaultAsync();

                if (existingItem != null)
                {
                    if (rider.Equals(existingItem.Rider1))
                    {
                        existingItem.Rider1 = null;
                    }
                    if (rider.Equals(existingItem.Rider2))
                    {
                        existingItem.Rider2 = null;
                    }
                    if (rider.Equals(existingItem.Rider3))
                    {
                        existingItem.Rider3 = null;
                    }
                    if (rider.Equals(existingItem.Rider4))
                    {
                        existingItem.Rider4 = null;
                    }

                    await _sqlCon.UpdateAsync(existingItem).ConfigureAwait(false);
                }
            }
        }
        
        public async Task Remove<T>(T item)
        {
            await _sqlCon.DeleteAsync(item);
        }
    }
}
