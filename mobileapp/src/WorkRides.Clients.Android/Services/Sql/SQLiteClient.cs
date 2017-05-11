using System.IO;
using SQLite.Net.Async;
using Xamarin.Forms;
using CarPool.Clients.Droid.Services.Sql;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using CarPool.Clients.Core.Services.Data;

[assembly: Dependency(typeof(SQLiteClient))]
namespace CarPool.Clients.Droid.Services.Sql
{
    public class SQLiteClient : ISQLite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = AppSettings.SQLiteDatabaseName;
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var path = Path.Combine(documentsPath, sqliteFilename);

            var platform = new SQLitePlatformAndroid();

            var connectionWithLock = new SQLiteConnectionWithLock(
                                         platform,
                                         new SQLiteConnectionString(path, true));

            var connection = new SQLiteAsyncConnection(() => connectionWithLock);

            return connection;
        }
    }
}