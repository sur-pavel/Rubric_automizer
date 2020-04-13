using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace Rubric_automizer
{
    internal class ExcelHandler
    {
        private const string EXCEL_FILE_PATH = @"c:\Users\sur-p\Downloads\Default_Rubrics.xlsx";
        internal List<SubtitleObj> subtitlesObjs;
        internal bool exit = false;

        internal List<SubtitleObj> GetSubtitlesObjs()
        {
            subtitlesObjs = new List<SubtitleObj>();
            UsingInteropExcel();
            return subtitlesObjs;
        }

        private void UsingInteropExcel()
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Open(EXCEL_FILE_PATH);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];
            Excel.Range lastCell = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            int lastColumn = (int)lastCell.Column;
            int lastRow = (int)lastCell.Row;
            string text = "";
            string firstTitle = "";
            string title = "";
            string subtitle = "";
            string index_MDA = "";

            for (int rowIndex = 1; rowIndex <= lastRow; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex <= lastColumn; columnIndex++)
                {
                    text = worksheet.Cells[rowIndex, columnIndex].Text.ToString();
                    if (String.IsNullOrEmpty(text))
                    {
                        continue;
                    }

                    if (columnIndex > 1)
                    {
                        title = worksheet.Cells[rowIndex, columnIndex - 1].Text.ToString();
                        int inc = 1;
                        while (String.IsNullOrEmpty(title))
                        {
                            title = worksheet.Cells[rowIndex - inc, columnIndex - 1].Text.ToString();
                            inc++;
                        }
                    }
                    else
                    {
                        title = firstTitle;
                    }

                    if (text.Contains("Предметный заголовок"))
                    {
                        int splitLength = text.Split('|').Length;
                        firstTitle = text.Split('|')[splitLength - 1];
                        index_MDA = text.Split('|')[1];
                        Console.WriteLine("\n\nдобавлен Предметный заголовок: " + index_MDA + " " + firstTitle);
                        continue;
                    }
                    subtitle = text;

                    SubtitleObj subtitleObj = new SubtitleObj(index_MDA, title, subtitle);
                    Console.WriteLine("ИНДЕКС МДА: " + index_MDA + " ЗАГОЛОВОК: " + subtitleObj.Title + " ПОДЗАГОЛОВОК: " + subtitleObj.Subtitle);

                    subtitlesObjs.Add(subtitleObj);
                }
            }
            workbook.Close(false);
            app.Quit();
        }
    }
}