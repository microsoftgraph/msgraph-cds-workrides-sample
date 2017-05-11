using CarPool.Clients.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Data
{
    public interface IDataService
    {
        /// <summary>
        /// Method used to include the authentication token neccesary to connect with backend.
        /// </summary>
        /// <param name="token">The authentication Token</param>
        Task InitializeAsync(string token = "");

        /// <summary>
        /// Method used to retrieve all the employees what are offered as drivers. 
        /// Each driver record <see cref="Driver" /> contains the information related 
        /// to the particular ride (e.g. interested riders and their approval status etc.) 
        /// </summary>
        /// <returns>
        /// The list of drivers who have offered rides at this time.
        /// </returns>
        Task<IEnumerable<Driver>> GetAllDriversAsync();

        /// <summary>
        /// Method used to retrieve all the employees who have signed in the app (as rider, driver or both).
        /// </summary>
        /// <returns>
        /// The list of employees <see cref="Employee" /> who have signed in to the app. 
        /// </returns>
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        /// <summary>
        /// ethod used to retrieve all the ride details who have been scheduled at this time.
        /// </summary>
        /// <returns>The list of ride details.</returns>
        Task<IEnumerable<RideDetails>> GetAllRideDetailsAsync();

        /// <summary>
        /// Method used to persist the driver profile information.
        /// 
        /// If the driver already exists (primary key by Name) in the data service, 
        /// will be updated with the given by the parameter.
        /// </summary>
        /// <param name="item">The driver item to be insert/update in the data service</param>
        /// <returns>void (awaitable task)</returns>
        Task InsertOrUpdateDriverAsync(Driver item);

        /// <summary>
        /// Method used to persist the employee profile information.
        /// The initial user profile is coming from MS Graph during the first sign in to the app. 
        /// 
        /// If the employee already exists (primary key by Name) in the data service,  
        /// will be updated with the given by the parameter.
        /// </summary>
        /// <param name="item">The employee to be insert/update in the data service.</param>
        /// <returns>void (awaitable task)</returns>
        Task InsertOrUpdateEmployeeAsync(Employee item);

        /// <summary>
        /// Method used to persist the ride details information.
        /// The ride detail information is populated when the driver accept the rider request to join the carpool.
        /// 
        /// If the RideDetail already exists (primary key by Driver) in the data service,  
        /// will be updated with the given by the parameter.
        /// </summary>
        /// <param name="rideDetails">The RideDetails to be insert/update in the data service.</param>
        /// <returns>void (awaitable task)</returns>
        Task InsertOrUpdateRideDetailsAsync(RideDetails rideDetails);

        /// <summary>
        /// Method used to propose an employee as a rider for the given carpool.
        /// 
        /// Each rider get the first position that its empty of all the possible seats. <see cref="Driver.Rider1" />
        /// If one seat is full, the rider should be inserted into the next seat (Rider2, Rider3, Rider4)
        /// 
        /// The rider must be confirmed by the driver. To represent that state, the 
        /// status column for the rider must be set to 'False' <see cref="Driver.Rider1Status" />
        /// on its seat (Rider1Status, Rider2Status, Rider3Status, Rider4Status)
        /// </summary>
        /// <param name="driver">the Driver instance</param>
        /// <param name="rider">the name of the employee</param>
        /// <returns>void (awaitable task)</returns>
        Task AddRiderAsync(Driver driver, string rider);

        /// <summary>
        /// Method used to confirm an employee as a rider for the given carpool.
        /// 
        /// Confirm a rider means to change the status column to 'True' <see cref="Driver.Rider1Status" /> 
        /// on its seat (Rider1Status, Rider2Status, Rider3Status, Rider4Status)
        /// </summary>
        /// <param name="driver">the Driver instance</param>
        /// <param name="rider">the name of the employee</param>
        /// <returns>void (awaitable task)</returns>
        Task UpdateRiderStatusAsync(Driver driver, string rider);

        /// <summary>
        /// Method used to reject an employee as a rider for the given carpool.
        /// 
        /// When the driver reject to include the rider into him carpool, the rider must be removed from the
        /// seat (Rider1, Rider2, Rider3, Rider4) column.
        /// </summary>
        /// <param name="driver">the Driver instance</param>
        /// <param name="rider">the name of the employee</param>
        /// <returns>void (awaitable task)</returns>
        Task RejectRiderAsync(Driver driver, string rider);

        /// <summary>
        /// Remove an instance object from the data service. The instance object passed as parameter
        /// should contains the primary key field.
        /// </summary>
        /// <typeparam name="T">Type of the object model to be removed (Employee, Driver, RideDetails)</typeparam>
        /// <param name="item">Instance of the object model to be removed </param>
        /// <returns>void (awaitable task)</returns>
        Task Remove<T>(T item);

        /// <summary>
        /// Insert an instance object to the data service. 
        /// </summary>
        /// <typeparam name="T">Type of the object model to be inserted (Employee, Driver, RideDetails)</typeparam>
        /// <param name="item">Instance of the object model to be inserted</param>
        /// <returns>void (awaitable task)</returns>
        Task InsertAsync<T>(T item);
    }
}