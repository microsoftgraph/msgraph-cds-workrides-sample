using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.iOS.Services.Sql;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteClient))]
namespace CarPool.Clients.iOS.Services.Sql
{
    public class SQLiteClient : ISQLite
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var sqliteFilename = AppSettings.SQLiteDatabaseName;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFilename);

            var platform = new SQLitePlatformIOS();

            var connectionWithLock = new SQLiteConnectionWithLock(
                                          platform,
                                          new SQLiteConnectionString(path, true));

            var connection = new SQLiteAsyncConnection(() => connectionWithLock);

            return connection;
        }
    }
}