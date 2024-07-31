using ggst_api.config;
using ggst_api.entity;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace ggst_api.Services
{

    public class LoginUserService
    {
        private readonly SqlServerConnectDbcontext _dbcontext;
        public LoginUserService(SqlServerConnectDbcontext dbcontext) { 
            _dbcontext = dbcontext;
        }

        
    }
}
