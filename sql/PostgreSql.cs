using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace RubricAutimatization
{

    public class PostgreSql
    {
        public PostgreSql()
        {
            var cs = "Host=localhost;Username=postgres;Password=s$cret;Database=irbis_db";

            using var con = new NpgsqlConnection(cs);
            con.Open();

            var sql = "SELECT version()";

            using var cmd = new NpgsqlCommand(sql, con);

            var version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"PostgreSQL version: {version}");


            cmd.ExecuteNonQuery();

            const string INSERT_QUERY = @"INSERT INTO students (group_id, first_name, last_name) 
            VALUES	(1,'JAMES', 'SMITH')
";
            cmd.CommandText = INSERT_QUERY;
            cmd.ExecuteNonQuery();

        }
    }


}