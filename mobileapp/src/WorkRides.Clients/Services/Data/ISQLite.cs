using SQLite.Net.Async;

namespace CarPool.Clients.Core.Services.Data
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetConnection();
    }
}