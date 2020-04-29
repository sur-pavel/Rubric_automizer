using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;

namespace Rubric_automizer
{
    public class WordHandler
    {
        private Document objDoc;
        private Application wordApp;

        public WordHandler()
        {
            try
            {
                wordApp = new Application();
                objDoc = wordApp.Documents.Open(@"c:\Users\sur-p\Downloads\Default_Rubrics.doc");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                objDoc.Close();
                wordApp.Quit();
            }
        }

        internal List<SubtitleObj> GetSubtitles()
        {
            List<SubtitleObj> subtitleObjs = new List<SubtitleObj>();
            var numberOfPages = objDoc.ComputeStatistics(WdStatistic.wdStatisticPages, false);

            for (int i = 1; i <= numberOfPages; i++)
            {
                var pageRange = objDoc.Range()
                    .GoTo(WdGoToItem.wdGoToPage, WdGoToDirection.wdGoToAbsolute, i);
                foreach (Table tb in pageRange.Tables)
                {
                    foreach (Row row in tb.Rows)
                    {
                        foreach (Cell cell in row.Cells)
                        {
                            string pageText = pageRange.Text;
                            string cellText = cell.Range.Text;
                            Console.WriteLine("Text in cell: {0}\n" +
                            "Text On Page: {1}\n\n", cellText, pageText);
                            if (cellText.Contains("подзаголовок"))
                            {
                                continue;
                            }
                            else
                            {
                                SubtitleObj subObj = new SubtitleObj("", "", "");
                                subObj.Title = pageText;
                                subObj.Subtitle = cellText;
                                subtitleObjs.Add(subObj);
                            }
                        }
                    }
                }
            }

            return subtitleObjs;
        }
    }
}