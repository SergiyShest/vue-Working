using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using UBP.Common;
using UBP.DataExport;
using UBP.Collection;

namespace UBP.Reports
{
    /// <summary>
    /// используется при выгрузке в CSV
    /// </summary>
    public static class CsvWorking
    {
        /// <summary>
        /// получает строку коллекции в фомате CSV
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
		 public static string GetCSV_WithoutHeader(IEnumerable<RSQLDataRowWrapper> collection)
        {
            StringBuilder rez = new StringBuilder();

				foreach (RSQLDataRowWrapper DR in collection)
            {

                GetCSVRow(rez, DR, ";");
                rez.Remove(rez.Length - 1, 1);
                rez.Append("\r\n");
            }
            return rez.ToString();
        }

        // <summary>
        /// получает строку коллекции в фомате CSV
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
		 public static string GetCSV_WithHeader(IEnumerable<RSQLDataRowWrapper> collection, List<RCellValue> columnNames)
        {

            StringBuilder rez = new StringBuilder();
            GetCSVHeader(columnNames, rez);
            if (rez.Length > 0)
            { rez.Remove(rez.Length - 1, 1); }
            rez.AppendLine();

				foreach (RSQLDataRowWrapper DR in collection)
            {
                GetCSVRow(rez, DR, ";", columnNames);
                rez.AppendLine();
            }
            if (rez.Length > 0)
            { rez.Remove(rez.Length - 1, 1); }
            return rez.ToString();

        }

        public static void GetCSVHeader(List<RCellValue> columnNames, StringBuilder rez)
        {
            bool first = true;
            foreach (RCellValue coll in columnNames)
            {
                if (!first) rez.Append(';');
                first = false;
                {
                    string colName = string.Empty;
                    if (!String.IsNullOrWhiteSpace(coll.Text))
                    {
                        colName = coll.Text;
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(coll.Name))
                        {
                            colName = coll.Name;
                        }
                    }

                    rez.Append(StringToCSVCell(colName));
                }
            }
            rez.AppendLine();
        }





		  private static void GetCSVRow(StringBuilder rez, RSQLDataRowWrapper DR, string delimiter)
        {
            foreach (object cell in DR.Row.ItemArray)
            {
                if (cell != null)
                {
                    rez.Append(StringToCSVCell(cell.ToString()));
                }
                rez.Append(delimiter);
            }

        }


        /// <summary>
        /// получает строку коллекции в фомате CSV
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
		  public static string GetSimpleText(IEnumerable<RSQLDataRowWrapper> collection, List<RCellValue> columnNames)
        {
            StringBuilder rez = new StringBuilder();

				foreach (RSQLDataRowWrapper DR in collection)
            {
                //if (rez.Length > 0)
                //{
                //    rez.Append(Environment.NewLine);
                //}
                GetCSVRow(rez, DR, "", columnNames);
            }
            return rez.ToString();
        }



		  private static void GetCSVRow(StringBuilder rez, RSQLDataRowWrapper DR, string delimiter, List<RCellValue> ColumnNames)
        {
            foreach (RCellValue cell in ColumnNames)
            {
                if (cell != null)
                {
                    object x = DR[cell.Name];
                    if (x != null)
                    {
                        rez.Append(StringToCSVCell(x.ToString()));
                    }
                }
                rez.Append(delimiter);
            }
            rez.Append(Environment.NewLine);
        }


        /// <summary>
        /// старт и заполнение Ехсел в отдельном потоке
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames"></param>
        public static void CsvSave(List<List<RCellValue>> dt,List<RCellValue>  columnNames)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.RestoreDirectory = true;
            sfd.Filter = "CSV|*.csv";
            if (sfd.ShowDialog() != true) return;
           

          //  var columnNames = WPFHelper.GetColumnsOrdered(dataGrid);
            dt.Insert(0, columnNames);
            try
            {
                File.WriteAllText(sfd.FileName, GetCsv(dt).ToString(), Encoding.GetEncoding(1251));
            }
            catch (Exception ex)
            {
                REnvironment.Current.ShowExceptionMessage(ex.Message);
            }

        }



        /// <summary>
        /// получение 
        /// </summary>
        /// <param name="DGValues"></param>
        /// <returns></returns>
        public static StringBuilder GetCsv(List<List<RCellValue>> DGValues)
        {
            StringBuilder rezult = new StringBuilder();
            int i = 0;

            foreach (List<RCellValue> ctr in DGValues)
            {
                foreach (RCellValue val in ctr)
                {
                    rezult.Append(StringToCSVCell(val.Text) + ';');
                }
                rezult.Remove(rezult.Length - 1, 1);
                rezult.Append(Environment.NewLine);

                i++;
            }
            return rezult.Remove(rezult.Length - 1, 1); ;
        }
       
        /// <summary>
        /// Экранирование  строки для вставки в CSV  если в ней есть символы то строка обрамляестся кавычками
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToCSVCell(string str)
        {
            bool mustQuote = false;
            foreach (char nextChar in str)
            {
                switch (nextChar)
                {
                    case ';':
                    case ',':
                    case '\"':
                    case '\r':
                    case '\n':
                        mustQuote = true;
                        break;
                }
                if (mustQuote) break;//дальше двигаться бессмысленно
            }
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }
}