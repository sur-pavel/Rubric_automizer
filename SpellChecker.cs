using AM;
using AM.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeCantSpell.Hunspell;

namespace Rubric_automizer
{
    internal class SpellChecker

    {
        private SqlHandler sqlHandler;

        public SpellChecker(SqlHandler sqlHandler)
        {
            this.sqlHandler = sqlHandler;
        }

        internal bool CheckByMatch(string subtitle)
        {
            bool negative = false;
            Match fullPersonName = Regex.Match(subtitle, @"_[\d]+[\,с]");
            if (fullPersonName.Success)
            {
            }
            return negative;
        }

        internal string CleanupStringsInSubObj(string data)
        {
            string str = data;
            if (CheckByMatch(str))
            {
                data = "ODD_SUB";
            }
            string[] words = sqlHandler.GetAllSubtitles().Split(' ');
            WordList dictionary = WordList.CreateFromWords(words);
            Console.WriteLine("Creation of dictionary ended\n");
            foreach (string word in str.Split(' '))
            {
                if (!dictionary.Check(word))
                {
                    List<string> suggestList = dictionary.Suggest(word).ToList();
                    if (!suggestList.IsNullOrEmpty())
                    {
                        data = ConsoleSuggestDialog(word, suggestList);
                    }
                };
            }

            return data;
        }

        private string ConsoleSuggestDialog(string word, List<string> suggestList)
        {
            Console.WriteLine($"Wrong word: {word}");
            Console.WriteLine("Choose suggest (print number): ");
            Console.WriteLine("Else you can 'E'dit or e'X'it)");
            for (int inc = 0; inc < suggestList.Count; inc++)
            {
                Console.WriteLine($"{inc} - {suggestList.GetItem(inc)}");
            }
            int choosedItem = -1;
            bool exit = false;
            while (choosedItem == -1 || exit)
            {
                ConsoleKeyInfo UserInput = Console.ReadKey();
                char key = UserInput.KeyChar;
                if (char.IsDigit(key))
                {
                    choosedItem = int.Parse(key.ToString());
                    word = suggestList.GetItem(choosedItem);
                }
                else if (key.OneOf(new char[] { 'e', 'x' }))
                {
                    switch (key)
                    {
                        case 'E':
                        case 'e':
                            Console.Write("\nEdit name:\n");
                            SendKeys.SendWait(word);
                            word = Console.ReadLine();
                            Console.WriteLine($"New string:\n{word}");
                            Console.ReadKey();
                            exit = true;
                            break;

                        case 'X':
                        case 'x':
                            exit = true;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    choosedItem = -1;
                }
            }
            return word;
        }

        internal SubtitleObj CheckSubtitleObj(SubtitleObj subtitleObj)
        {
            subtitleObj.Title = CleanupStringsInSubObj(subtitleObj.Title);
            subtitleObj.Subtitle = CleanupStringsInSubObj(subtitleObj.Subtitle);
            if (subtitleObj.Subtitle.Equals("ODD_SUB"))
            {
                subtitleObj.Subtitle = subtitleObj.Title;
                subtitleObj.Title = "ODD_SUB";
            }

            return subtitleObj;
        }
    }
}