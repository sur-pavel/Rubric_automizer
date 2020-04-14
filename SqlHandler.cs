using Npgsql;
using System;
using System.Text;

namespace Rubric_automizer
{
    public class SqlHandler
    {
        private const string CONNECTION_PARAMETERS = "Host = localhost; Username = postgres; Password = s$cret; Database = irbis_db";
        private NpgsqlConnection connection;

        public SqlHandler()
        {
            connection = new NpgsqlConnection(CONNECTION_PARAMETERS);
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT version()", connection))
            {
                var version = command.ExecuteScalar().ToString();
                Console.WriteLine($"PostgreSQL version: {version}");
                command.ExecuteNonQuery();
            }
        }

        internal void DisConnect()
        {
            connection.Close();
        }

        internal void DeleteDataFromDocsDB()

        {
            using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM doc_subtitles;", connection))
            {
                command.ExecuteNonQuery();
            }
        }

        internal void InsertDataDB(string tableName, SubtitleObj subtitleObj)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"INSERT INTO doc_subtitles (index_MDA, title, subtitle)
            VALUES ('{0}', '{1}', '{2}')", subtitleObj.Index_MDA, subtitleObj.Title, subtitleObj.Subtitle);

            Console.WriteLine(builder.ToString());
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(builder.ToString(), connection))
                {
                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        internal void InsertDataIntoDB(string tableName, string searchTherm, string data)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"INSERT INTO {0} ({1})
            VALUES ('{3}')", tableName, searchTherm, data);

            Console.WriteLine(builder.ToString());
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(builder.ToString(), connection))
                {
                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        internal bool ExistInDB(string dbName, string searchinTerm, string lastSubtitle)
        {
            bool exist = false;
            return exist;
        }

        internal string GetIndexMDA(string title)
        {
            string index_MDA = "";
            var builder = new StringBuilder();
            builder.AppendFormat(@"SELECT index_MDA FROM doc_subtitles WHERE title = '{0}'", title);
            Console.WriteLine(builder.ToString());
            try

            {
                using (NpgsqlCommand command = new NpgsqlCommand(builder.ToString(), connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        index_MDA = reader.GetString(0);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                index_MDA = "";
            }
            return index_MDA;
        }
    }
}