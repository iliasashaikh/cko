using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Cko.PaymentGateway.Repository
{
    public class DapperRepository<T> : IRepository<T> where T : class
    {
        private readonly IConfiguration _settings;
        private readonly ILogger<DapperRepository<T>> _logger;
        private readonly string _connString;

        public DapperRepository(IConfiguration settings, ILogger<DapperRepository<T>> logger)
        {
            this._settings = settings;
            this._logger = logger;

            if (settings!=null)
                this._connString = settings.GetConnectionString("CkoDb");
        }


        public async Task<IEnumerable<T>> Get(string query, object? parameters = null)
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    return await conn.QueryAsync<T>(query, parameters);
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }
        }

        public async Task Run(string query, object? parameters = null)
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    await conn.ExecuteAsync(query,parameters);
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }
        }

        public async Task<int> Insert(T entity)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    int id = await conn.InsertAsync(entity);
                    return id;
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }
        }

        public async Task<bool> Update(T entity)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();
                    bool updated = await conn.UpdateAsync(entity);
                    return updated;
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                throw;
            }
        }


    }
}
