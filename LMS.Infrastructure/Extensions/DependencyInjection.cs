using LMS.Application.Business.User;
using LMS.Domain.Interfaces.Dapper;
using LMS.Domain.Interfaces.User;
using LMS.Infrastructure.Dapper;
using LMS.Infrastructure.Repository.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureLayer(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #region Dapper Dao
            services.AddTransient<IDapperDAO, DapperDAO>();
            #endregion
            #region User
            services.AddTransient<IUserBusiness, UserBusiness>();
            services.AddTransient<IUserRepository, UserRepository>();
            #endregion

        }
    }
}
