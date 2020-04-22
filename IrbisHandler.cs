using ManagedClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rubric_automizer
{
    internal class IrbisHandler
    {
        private ManagedClient64 client;
        private SpellChecker spellChecker;
        private SqlHandler sqlHandler;
        internal bool notConnected;

        public IrbisHandler(SpellChecker spellChecker, SqlHandler sqlHandler)
        {
            this.sqlHandler = sqlHandler;
            this.spellChecker = spellChecker;
            try
            {
                client = new ManagedClient64();
                client.ParseConnectionString("host=127.0.0.1;port=8888; user=a;password=1;");
                client.Connect();
                client.PushDatabase("MPDA");
                Console.WriteLine($"Irbis version: {client.GetVersion().Version}\n");
            }
            catch (Exception ex)
            {
                notConnected = true;
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

        internal List<SubtitleObj> GetSubtitlesObjs()
        {
            List<SubtitleObj> subtitlesObjs = new List<SubtitleObj>();
            for (int mfn = 1; mfn <= client.GetMaxMfn(); mfn++)
            {
                if (mfn > 10) break;
                IrbisRecord record = client.ReadRecord(mfn);
                Console.WriteLine($"\nMFN: {mfn}");

                foreach (RecordField field606 in record.Fields.GetField("606"))
                {
                    string fieldText = field606.ToSortedText();
                    fieldText = Regex.Replace(fieldText, @";\s([А-Я])\s; ", "");
                    fieldText = DeleteNotesOfBK(fieldText);
                    SubtitleObj subtitleObj = new SubtitleObj("", GetSubtitle(fieldText, 2), GetSubtitle(fieldText, 1));
                    try
                    {
                        subtitleObj = sqlHandler.GetRightSubtitleObj(subtitleObj);
                    }
                    catch (InvalidCastException ex)
                    {
                        try
                        {
                            Console.WriteLine($"\nMFN: {mfn}\n {ex.Message}\nTry to check subtitles.\n");
                            subtitleObj = spellChecker.CheckSubtitleObj(subtitleObj);
                        }
                        catch (InvalidCastException except)
                        {
                            Console.WriteLine($"\nMFN: {mfn}\n {except.Message}");
                        }
                    }
                    subtitlesObjs.Add(subtitleObj);
                }
            }

            return subtitlesObjs;
        }

        private string DeleteNotesOfBK(string fieldText)
        {
            foreach (string substr in fieldText.Split('^'))
            {
                //Console.WriteLine($"Substr: {substr}");
                string[] tags = { "P", "H", "3", "1" };
                foreach (string tag in tags)
                {
                    if (substr.StartsWith(tag))
                    {
                        fieldText = fieldText.Replace($"^{substr}", "");
                    }
                }
            }
            return fieldText;
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
                            subtitle = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None)[length - inc];
                        }
                        else
                        {
                            subtitle = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None)[0];
                            length = fieldText.Split('^').Length;
                            subtitle = subtitle.Split('^')[length - 1].Substring(1);
                        }
                    }
                    else
                    {
                        subtitle = fieldText.Split(new string[] { " ; " }, StringSplitOptions.None)[length - inc];
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