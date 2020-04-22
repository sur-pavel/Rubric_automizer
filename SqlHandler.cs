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

        internal void DisConnect()
        {
            connection.Close();
        }

        internal string GetAllSubtitles()
        {
            StringBuilder builder = new StringBuilder();

            string SELECT_ALL_SUBTITLES = "SELECT subtitle FROM doc_subtitles";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(SELECT_ALL_SUBTITLES, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string subtitle = Regex.Replace(reader[0].ToString(), @"[.,\/#!$%\^&\*;:{}=\-_`~–()]", " ");
                        subtitle = subtitle.Replace("  ", " ");
                        if (!String.IsNullOrEmpty(subtitle))
                        {
                            builder.Append(subtitle + " ");
                        }
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine(ex);
            }
            return builder.ToString();
        }

        internal string GetRightTitle(string wrongTitle)
        {
            string rightTitle = "";
            string selectQuery = $"SELECT subtitle FROM doc_subtitles" +
                $"WHERE doc_subtitle_id =" +
                $"(SELECT doc_subtitle_id FROM doc_subtitles_wrong_subtitles" +
                $"  WHERE wrong_subtitle_id =" +
                $"(SELECT wrong_subtitle_id FROM wrong_subtitles" +
                $"WHERE subtitle = '{wrongTitle}'));";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rightTitle = reader.GetString(0);
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Can't found rightTitle");
                Console.WriteLine(ex);
            }
            return rightTitle;
        }

        private string PerfromSelectQuery(string searchTherm, string tableName, string whereParam, string likeParam)
        {
            string result = "";
            string selectQuery = $"SELECT {searchTherm} FROM {tableName} WHERE LOWER({whereParam}) LIKE LOWER('%{likeParam}%')";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }
            }
            catch (PostgresException ex)
            {
                Console.WriteLine($"Can't found {searchTherm}");
                Console.WriteLine(ex);
            }
            return result;
        }

        internal void AddWrongTitle(string wrongTitle, string title)
        {
            string doc_subtitle_id = PerfromSelectQuery("doc_subtitle_id", "doc_subtitles", "title", title);
            if (!String.IsNullOrEmpty(doc_subtitle_id))
            {
                InsertDataIntoDB("wrong_subtitles", "subtitle", wrongTitle);
                string wrong_subtitle_id = PerfromSelectQuery("wrong_subtitle_id", "wrong_subtitles", "subtitle", title);
                InsertDataIntoDB("doc_subtitles_wrong_subtitles",
                    "doc_subtitle_id, wrong_subtitle_id", $"{doc_subtitle_id}, {wrong_subtitle_id}");
            }
        }

        internal void DeleteDataFromDocsDB()

        {
            using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM doc_subtitles;", connection))
            {
                command.ExecuteNonQuery();
            }
        }

        internal void InsertSubtitleObjDB(string tableName, SubtitleObj subtitleObj)
        {
            string insertQuery = $"INSERT INTO doc_subtitles (index_MDA, title, subtitle)" +
            $"VALUES ('{subtitleObj.Index_MDA}', '{subtitleObj.Title}', '{subtitleObj.Subtitle}')";

            Console.WriteLine($"INSERT INTO doc_subtitles " +
                $"INDEX: {subtitleObj.Index_MDA}, TITLE: {subtitleObj.Title}, SUBTITLE: {subtitleObj.Subtitle}");
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        internal void InsertDataIntoDB(string tableName, string insertParam, string value, params string[] oddParams)
        {
            string insertQuery = $"INSERT INTO {tableName} ({insertParam}) VALUES ('{value}')";
            foreach (string param in oddParams)
            {
                insertQuery = insertQuery + param;
            }

            Console.WriteLine(insertQuery);
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        internal SubtitleObj GetRightSubtitleObj(SubtitleObj subtitleObj)
        {
            //Console.WriteLine(builder.ToString());
            string selectQuery = $"SELECT index_MDA, title, subtitle FROM doc_subtitles " +
                            $"WHERE LOWER (title) LIKE LOWER ('%{subtitleObj.Title}%') " +
                            $"AND LOWER (subtitle) LIKE LOWER ('%{subtitleObj.Subtitle}%')";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection))
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

        internal void CreateSpellDictionary(string[] words)
        {
            foreach (string word in words)
            {
                InsertDataIntoDB("spell_dictionary", "word", word, " ON CONFLICT (word) DO NOTHING;");
            }
        }

        internal string GetIndexMDA(string subtitle)
        {
            return PerfromSelectQuery("index_MDA", "doc_subtitles", "subtitle", subtitle);
        }
    }
}