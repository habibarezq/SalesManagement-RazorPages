using Microsoft.EntityFrameworkCore;
using SalesManagement.Models;

namespace SalesManagement.Extensions
{
    public static class DatabaseExtensions
    {

        public static void AddSalesManagementDb(this WebApplicationBuilder builder)
        {
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext has a scoped service lifetime because:
            // - DB connections are a limited and expensive resource
            // - DbContext is not thread-safe, so scoped avoids concurrency issues
            //   (one fresh instance per HTTP request)
            // - makes it easier to manage transactions and ensure data consistency
            builder.Services.AddScoped<SalesManagementDbContext>();
            builder.Services.AddSqlServer<SalesManagementDbContext>(
                connString);
        }
    }
}
