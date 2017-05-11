using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.UWP.Services.Sql;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteClient))]
namespace CarPool.Clients.UWP.Services.Sql
{
    public class SQLiteClient : ISQLite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = AppSettings.SQLiteDatabaseName;
            var localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            var path = Path.Combine(localPath, sqliteFilename);

            var platform = new SQLitePlatformWinRT();

            var connectionWithLock = new SQLiteConnectionWithLock(
                                          platform,
                                          new SQLiteConnectionString(path, true));

            var connection = new SQLiteAsyncConnection(() => connectionWithLock);

            return connection;
        }
    }
}