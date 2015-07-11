using System;
using System.Data;
using Dapper;

namespace Hinata.Data
{
    public class DapperConfig
    {
        public static void Initialize()
        {
            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
            SqlMapper.AddTypeMap(typeof(DateTime?), DbType.DateTime2);
        }
    }
}
