﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rubric_automizer
{
    internal class Program
    {
        private static ExcelHandler excelHandler;
        private static SqlHandler sqlHandler;
        private static IrbisHandler irbisHandler;
        private static SpellChecker spellChecker;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    CancellationToken cancellationToken = tokenSource.Token;
                    Task.Factory.StartNew(() => ExitDBs());
                    Task.WaitAll();
                    Environment.Exit(0);
                    return false;
            }
        }

        private static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3);
            Console.WriteLine("Application started");

            sqlHandler = new SqlHandler();
            excelHandler = new ExcelHandler();
            spellChecker = new SpellChecker(sqlHandler);
            irbisHandler = new IrbisHandler(spellChecker, sqlHandler);

            //SaveRubricsToSqlFromExcel();
            //spellChecker.CreateDictionary();

            SaveRubricsToSqlFromIrbis();

            try
            {
                ExitDBs();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }

        private static void ExitDBs()
        {
            irbisHandler.Disconnect();

            Console.WriteLine("Irbis disconnected");
            sqlHandler.DisConnect();
            Console.WriteLine("Postgresql disconnected");
            Console.ReadKey();
        }

        private static void SaveRubricsToSqlFromIrbis()
        {
            foreach (SubtitleObj subtitleObj in irbisHandler.GetSubtitlesObjs())
            {
                //sqlHandler.InsertDataDB("doc_subtitles", subtitleObj);
            }
        }

        private static void SaveRubricsToSqlFromExcel()
        {
            foreach (SubtitleObj subtitleObj in excelHandler.GetSubtitlesObjs())
            {
                sqlHandler.InsertSubtitleObjDB("doc_subtitles", subtitleObj);
            }
        }

        private static string GetSubtitle(string rubric, int inc)
        {
            int splitLength;
            string subtitle;
            if (rubric.Contains(" ; "))
            {
                splitLength = rubric.Split(new string[] { "xx" }, StringSplitOptions.None).Length;
                subtitle = rubric.Split(new string[] { "xx" }, StringSplitOptions.None)[splitLength - inc];
            }
            else
            {
                splitLength = rubric.Split('^').Length;
                subtitle = rubric.Split('^')[splitLength - inc].Substring(1);
            }
            return subtitle;
        }

        private static bool ConsoleDialog(SubtitleObj subObj)
        {
            bool exit = false;
            StringBuilder builder = new StringBuilder();
            bool readCh = true;
            do
            {
                Console.WriteLine("Add Подзаголовок\n{0}\n from Заголовка{1}\n", subObj.Subtitle, subObj.Title);
                Console.Write("(");
                ColorConsole("y");
                Console.Write("es /");
                ColorConsole("n");
                Console.Write("o / ");
                ColorConsole("e");
                Console.Write("dit / e");
                ColorConsole("x");
                Console.Write("it) ?");
                char c = Console.ReadKey().KeyChar;
                switch (c)
                {
                    case 'e':
                        Console.Write("\nEdit name:\n");
                        SendKeys.SendWait(subObj.Subtitle);
                        subObj.Subtitle = Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine("New subtitle name:\n{0}", subObj.Subtitle);
                        Console.ReadKey();

                        readCh = false;
                        break;

                    case 'y':

                        readCh = false;
                        break;

                    case 'n':
                        readCh = false;
                        break;

                    case 'x':
                        readCh = false;
                        exit = true;
                        break;

                    default:
                        Console.Clear();
                        continue;
                }
            } while (readCh);
            Console.Clear();
            return exit;
        }

        private static void ColorConsole(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(s);
            Console.ResetColor();
        }
    }
}