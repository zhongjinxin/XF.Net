using System;
using System.Collections.Generic;
using System.Data;
using XF.Core.Configuration;
using XF.Core.Const;
using XF.Core.Enums;
using System.Data.SqlClient;
using Npgsql;
using MySqlConnector;

namespace XF.Core.DBManager
{
    public class DBServerProvider
    {
        private static Dictionary<string, string> ConnectionPool = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly string DefaultConnName = "defalut";

        static DBServerProvider()
        {
            SetConnection(DefaultConnName, AppSetting.DbConnectionString);
        }

        public static void SetConnection(string key, string val)
        {
            if (ConnectionPool.ContainsKey(key))
            {
                ConnectionPool[key] = val;
                return;
            }
            ConnectionPool.Add(key, val);
        }

        public static void SetDefaultConnection(string val)
        {
            SetConnection(DefaultConnName, val);
        }

        public static string GetConnectionString(string key)
        {
            key = key ?? DefaultConnName;
            if (ConnectionPool.ContainsKey(key))
            {
                return ConnectionPool[key];
            }
            return key;
        }

        public static string GetConnectionString()
        {
            return GetConnectionString(DefaultConnName);
        }

        public static IDbConnection GetDbConnection(string connString = null)
        {
            if (connString == null)
            {
                connString = ConnectionPool[DefaultConnName];
            }
            if (DBType.Name == DbCurrentType.MySql.ToString())
            {
                return new MySqlConnection(connString);
            }
            if (DBType.Name == DbCurrentType.PgSql.ToString())
            {
                return new NpgsqlConnection(connString);
            }
            return new SqlConnection(connString);
        }


    }
}
