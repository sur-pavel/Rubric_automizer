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
        private WordList spellDictionary;

        public SpellChecker(SqlHandler sqlHandler)
        {
            this.sqlHandler = sqlHandler;
        }

        internal void CreateDictionary()
        {
            Console.WriteLine("Creation of spellDictionary begins");
            string[] words = sqlHandler.GetAllSubtitles().Split(' ');

            sqlHandler.CreateSpellDictionary(words);

            spellDictionary = WordList.CreateFromWords(words);
            Console.WriteLine("Creation of spellDictionary ended\n");
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

        internal string CleanupStringsInSubObj(string title)
        {
            string wrongTitle = title;

            foreach (string wrongWord in wrongTitle.Split(' '))
            {
                if (!spellDictionary.Check(wrongWord))
                {
                    List<string> suggestList = spellDictionary.Suggest(wrongWord).ToList();
                    if (!suggestList.IsNullOrEmpty())
                    {
                        title = wrongTitle.Replace(wrongWord, ConsoleSuggestDialog(wrongWord, suggestList));
                        sqlHandler.AddWrongTitle(wrongTitle, title);
                    }
                };
            }

            return title;
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
            return subtitleObj;
        }
    }
}