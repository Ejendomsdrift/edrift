using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SyncDataService.Interfaces;

namespace SyncDataService.Implementation
{
    public class DbClient
    {
        private readonly IApplicationConfiguration configuration;

        public DbClient(IApplicationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<T>> Get<T>(string sql, Func<DataRow, T> convertor)
        {
            var dataTable = await GetDataTableFromSql(sql);
            var list = dataTable.Rows
                .Cast<DataRow>()
                .Select(convertor)
                .ToList();
            return list;
        }

        private async Task<DataTable> GetDataTableFromSql(string sql)
        {
            var table = new DataTable();
            using (var connection = new SqlConnection(configuration.ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (var command = new SqlCommand(sql, connection))
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return table;
        }
    }
}