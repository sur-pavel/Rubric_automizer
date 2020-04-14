using ManagedClient;
using System;
using System.Collections.Generic;

namespace Rubric_automizer
{
    internal class IrbisHandler
    {
        private ManagedClient64 client;

        public IrbisHandler()
        {
            try
            {
                client = new ManagedClient64();
                client.ParseConnectionString("host=127.0.0.1;port=8888; user=СПА;password=1;");
                client.Connect();
                client.PushDatabase("MPDA");
                Console.WriteLine("Connected to irbis_server successfully\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void Disconnect()
        {
            try
            {
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal List<SubtitleObj> GetSubtitlesObjs(SqlHandler sqlHandler)
        {
            List<SubtitleObj> subtitlesObjs = new List<SubtitleObj>();
            //for (int mfn = 1; mfn <= client.GetMaxMfn(); mfn++)
            //{
            IrbisRecord record = client.ReadRecord(17890);

            foreach (RecordField field606 in record.Fields.GetField("606"))
            {
                string fieldText = field606.ToSortedText();
                string index_MDA = sqlHandler.GetIndexMDA(fieldText.Split('^')[1].Substring(1));
                SubtitleObj subtitleObj = new SubtitleObj(index_MDA, GetSubtitle(fieldText, 1), GetSubtitle(fieldText, 2));
                Console.WriteLine("ИНДЕКС МДА: " + index_MDA + " ЗАГОЛОВОК: " + subtitleObj.Title + " ПОДЗАГОЛОВОК: " + subtitleObj.Subtitle + "\n");
                subtitlesObjs.Add(subtitleObj);
            }
            //}

            return subtitlesObjs;
        }

        private string GetSubtitle(string fieldText, int inc)
        {
            string subtitle = "";
            if (!String.IsNullOrEmpty(fieldText) && !String.IsNullOrWhiteSpace(fieldText))
            {
                int length;
                if (fieldText.Contains(" ; "))
                {
                    length = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None).Length;
                    if (inc > 1)
                    {
                        if (length > 2)
                        {
                            subtitle = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None)[length - inc].Substring(1);
                        }
                        else
                        {
                            length = fieldText.Split('^').Length;
                            subtitle = fieldText.Split('^')[length - 1].Substring(1);
                        }
                    }
                    else
                    {
                        subtitle = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None)[length - inc].Substring(1);
                    }
                }
                else
                {
                    length = fieldText.Split('^').Length;
                    if (inc < length)
                    {
                        subtitle = fieldText.Split('^')[length - inc].Substring(1);
                    }
                }
            }
            return subtitle;
        }
    }
}