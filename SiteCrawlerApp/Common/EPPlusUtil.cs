using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerApp.Common
{
    public class EPPlusUtil
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelPackage">ExcelPackage class</param>
        /// <param name="sheetPos">position of sheet</param>
        public static List<List<string>> ReadExcelSheet(ExcelPackage excelPackage, int sheetPos)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // check sheetPos ok?
            var workSheet1 = excelPackage.Workbook.Worksheets[sheetPos];

            ////////////////////////////////////////////
            int colCount = workSheet1.Dimension.End.Column;  // get Column Count
            int rowCount = workSheet1.Dimension.End.Row;     // get row count
            int colDataMax = 0;
            int rowDataMax = 0;

            // 1. Get Max Column data
            colDataMax = colCount;
            /**
            for (int col = 1; col <= colCount; col++)
            {
                var value = workSheet1.Cells[1, col].Value?.ToString().Trim();
                if (value == null || value == "")
                {
                    var value1 = workSheet1.Cells[1, col + 1].Value?.ToString().Trim();
                    if ((value1 == null || value1 == "") || (col == colCount - 1))
                    {
                        colDataMax = col - 1;
                        break;
                    }
                }
            }
            */

            // 2. Get Max Row data
            rowDataMax = rowCount;
            for (int row = 1; row <= rowCount; row++)
            {
                var value = workSheet1.Cells[row, 1].Value?.ToString().Trim();
                if (value == null || value == "")
                {
                    var value1 = workSheet1.Cells[row + 1, 1].Value?.ToString().Trim();
                    if ((value1 == null || value1 == "") || (row == rowCount - 1))
                    {
                        rowDataMax = row - 1;
                        break;
                    }
                }
            }


            // Data to read
            List<List<string>> rowDataList = new List<List<string>>();
            List<string> rowData = null;
            for (int row = 1; row <= rowDataMax; row++)
            {
                rowData = new List<string>();
                for (int col = 1; col <= colDataMax; col++)
                {
                    rowData.Add(workSheet1.Cells[row, col].Value?.ToString().Trim());
                }

                rowDataList.Add(rowData);
            }
            return rowDataList;
        }
    }



    // public class EPPlusUtil

}
