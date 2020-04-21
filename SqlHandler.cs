using Npgsql;
using System;
using System.Text;
using System.Text.RegularExpressions;

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

        internal string GetAllSubtitles()
        {
            StringBuilder builder = new StringBuilder();
            Console.WriteLine("Creation of dictionary begins");
            string SELECT_ALL_SUBTITLES = "SELECT subtitle FROM doc_subtitles";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(SELECT_ALL_SUBTITLES, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string subtitle = Regex.Replace(reader[0].ToString(), @"[.,\/#!$%\^&\*;:{}=\-_`~()]", " ");
                        subtitle = subtitle.Replace("  ", " ");
                        builder.Append(subtitle);
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine(ex);
            }
            return builder.ToString();
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

        internal string GetIndexMDA(string subtitle)
        {
            try
            {
                string selectQuery = $"SELECT index_MDA FROM doc_subtitles WHERE LOWER(subtitle) LIKE LOWER('%{subtitle}%')";
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subtitle = reader.GetString(0);
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Can't found index_MDA where subtitle: {subtitle}");
                Console.WriteLine(ex);
            }
            return subtitle;
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

        internal SubtitleObj GetRightSubtitleObj(SubtitleObj subtitleObj)
        {
            //Console.WriteLine(builder.ToString());
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(SelectQuery(subtitleObj), connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subtitleObj.Index_MDA = reader.GetString(0);
                        subtitleObj.Title = reader.GetString(1);
                        subtitleObj.Subtitle = reader.GetString(2);
                    }
                    if (String.IsNullOrEmpty(subtitleObj.Index_MDA) ||
                        String.IsNullOrEmpty(subtitleObj.Title) ||
                        String.IsNullOrEmpty(subtitleObj.Subtitle))
                    {
                        throw new InvalidCastException($"ИНДЕКС МДА: {subtitleObj.Index_MDA} " +
                            $"ЗАГОЛОВОК: {subtitleObj.Title} ПОДЗАГОЛОВОК: {subtitleObj.Subtitle}\n");
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine(ex);
                throw new InvalidCastException($"ИНДЕКС МДА: {subtitleObj.Index_MDA} " +
                    $"ЗАГОЛОВОК: {subtitleObj.Title} ПОДЗАГОЛОВОК: {subtitleObj.Subtitle}\n");
            }
            return subtitleObj;
        }

        private string SelectQuery(SubtitleObj subtitleObj)
        {
            return $"SELECT index_MDA, title, subtitle FROM doc_subtitles " +
                            $"WHERE LOWER (title) LIKE LOWER ('%{subtitleObj.Title}%') " +
                            $"AND LOWER (subtitle) LIKE LOWER ('%{subtitleObj.Subtitle}%')";
        }
    }
}