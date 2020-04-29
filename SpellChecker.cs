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
            List<string> words = sqlHandler.GetSpellDictionary();
            if (words.Any())
            {
                spellDictionary = WordList.CreateFromWords(words);
            }
            else
            {
                CreateDictionaryFromSubtitles();
            }
        }

        internal void CreateDictionaryFromSubtitles()
        {
            Console.WriteLine("Creation of spellDictionary from subtitles begins");
            string[] words = sqlHandler.GetAllSubtitles().Split(' ');
            words = words.Where(val => val != "").ToArray();
            words = words.Where(val => val != " ").ToArray();

            sqlHandler.CreateSpellDictionary(words);

            spellDictionary = WordList.CreateFromWords(words);
            Console.WriteLine("Creation of spellDictionary from subtitles ended\n");
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

        internal string CleanupTitle(string title)
        {
            string wrongTitle = title;
            string rightTitle = sqlHandler.GetRightTitleFromWrongDB(wrongTitle);
            if (string.IsNullOrEmpty(rightTitle))
            {
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
            }
            else
            {
                title = rightTitle;
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
            subtitleObj.Title = CleanupTitle(subtitleObj.Title);
            subtitleObj.Subtitle = CleanupTitle(subtitleObj.Subtitle);
            return subtitleObj;
        }
    }
}