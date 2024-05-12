using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces.Dapper
{
    public interface IDapperDAO
    {
        List<T0> ExecuteQuery<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Dictionary<string, string> ExecuteQueryList<T>(string sqlQuery, object sqlParam, System.Data.CommandType queryType = System.Data.CommandType.StoredProcedure);
        List<object> ExecuteQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        List<object> ExecuteQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<T0>> ExecuteQueryAsync<T0>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5, T6>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync<T0, T1, T2, T3, T4, T5, T6, T7>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> ExecuteQueryAsync(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<dynamic> ExecuteMultipleQueryAsync(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
    }
}
