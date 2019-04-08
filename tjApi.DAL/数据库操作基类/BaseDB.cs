using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace tjApi.DAL
{
    public class BaseDB
    {
        private IConfiguration _config;

        public BaseDB(IConfiguration config)
        {
            _config = config;
        }

        //SqlSugarClient db = new SqlSugarClient(new ConnectionConfig {
        //    ConnectionString = _config.GetConnectionString("")
        //});
    }
}
