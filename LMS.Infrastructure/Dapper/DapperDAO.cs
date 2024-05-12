using Dapper;
using LMS.Domain.Entities.Domain;
using LMS.Domain.Interfaces.Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Dapper
{
    public class DapperDAO : IDapperDAO
    {
        private readonly ILogger _log = Log.ForContext<DapperDAO>();
        IConfiguration _configuration;
        public DapperDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<T0> ExecuteQuery<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType);
                    var res = result.Read<T0>().ToList();
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public Dictionary<string, string> ExecuteQueryList<T>(string sqlQuery, object sqlParam, CommandType queryType = System.Data.CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.Query<KeyValuePair<string, string>>(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType).ToDictionary(pair => pair.Key, pair => pair.Value);
                    return result;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public List<object> ExecuteQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                sqlConnection.Open();
                var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                var res = new List<object>();
                res.Add(result.Read<T0>().ToList());
                res.Add(result.Read<T1>().ToList());
                sqlConnection.Close();
                return res;
            }
        }
        public List<object> ExecuteQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(result.Read<T0>().ToList());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(result.Read<T1>().ToList());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(result.Read<T2>().ToList());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<T0>> ExecuteQueryAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    try
                    {
                        var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000,
                        commandType: queryType);
                        var res = await result.ReadAsync<T0>();
                        return res.ToList();
                    }
                    catch (SqlException sex)
                    {
                        if (typeof(T0) == typeof(BaseResponseModel))
                        {
                            List<BaseResponseModel> res = new();
                            res.Add(new BaseResponseModel() { ErrorCode = 1, Message = sex.Message });
                            return (List<T0>)Convert.ChangeType(res, typeof(List<T0>));
                        }
                        throw;
                    }
                    catch (Exception eex)
                    {
                        if (typeof(T0) == typeof(BaseResponseModel))
                        {
                            List<BaseResponseModel> res = new();
                            res.Add(new BaseResponseModel() { ErrorCode = 1, Message = eex.Message });
                            return (List<T0>)Convert.ChangeType(res, typeof(List<T0>));
                        }
                        return null;
                        //throw;
                    }


                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }
                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T3>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T3>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T4>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T5>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    while (!result.IsConsumed)
                    {
                        res.Add(await result.ReadAsync<SelectListItem>());
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<dynamic> ExecuteMultipleQueryAsync(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = await sqlConnection.QueryMultipleAsync(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    while (!result.IsConsumed)
                    {
                        res.Add(result.Read<object>());
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public string GetDataTableToString(DataTable dt)
        {
            string xml = null;
            using (TextWriter writer = new StringWriter())
            {
                dt.WriteXml(writer);
                xml = writer.ToString();
            }
            return xml;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable("dt");

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        private string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("DBConnection");
            return connectionString ?? "";
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T3>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T4>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5, T6>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T3>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T4>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T5>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T6>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        public async Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5, T6, T7>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using (var sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    var result = sqlConnection.QueryMultiple(sqlQuery, sqlParam, commandTimeout: 30000, commandType: queryType);
                    var res = new List<object>();
                    res.Add(await result.ReadAsync<T0>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T1>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T2>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T3>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T4>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T5>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T6>());
                    if (result.IsConsumed)
                    {
                        return res;
                    }

                    res.Add(await result.ReadAsync<T7>());
                    return res;
                }
                catch (Exception ex)
                {
                    _log.Error("Error on Database Operation. Error Details As: {0}", ex.StackTrace);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}
