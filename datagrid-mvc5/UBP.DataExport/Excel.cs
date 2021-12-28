/*
Version information:
$Revision: 1913 $
$Author: Shestakov $
$Date: 2010-05-13 10:59:54 +0400 (Чт, 13 май 2010) $
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Office.Interop.Excel;
using UBP.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Threading;
using System.Globalization;
using System.Linq;
using System.Data;
using UBP.Collection;
using System.Reflection;
using UBP.Common;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using UBP.Protocol.Core;
using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;


namespace UBP.DataExport
{
    public class RSaveExeption : Exception
    {
        public RSaveExeption(String message) : base(message) { }
    }



    public static class ExcelWorking
    {

        /// <summary>
        /// получить запущенный  Excel или создать новый
        /// </summary>
        /// <returns></returns>
        public static Excel.Application GetExcelApplication()
        {
            Excel.Application xl = null;
            try
            {
                try
                {
                    xl = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application") as Excel.Application;
                }
                catch
                {
                    xl = new Excel.Application();
                }
            }
            catch (Exception ex)
            {
                REnvironment.Current.Protocol.PutExceptionMessage( new Exception("Не удалось запустить EXCEL\nВозможно он не установлен на Вашем компьютере",ex));
            }
            return xl;
        }

        /// <summary>
        /// закрыть Excel если есть 
        /// </summary>
        public static void CloseExcelApplication()
        {
                try
                {
                  var  xl = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application") as Excel.Application;
                    xl.Quit();
                }
                catch
                {
                    
                }
        }

        public static Excel.Application OpenExcel(string fileName)
        {
            Excel.Application XL=new Excel.Application();
          // Excel.Application XL = GetExcelApplication();
            if (XL == null)
            {
                return null;
            }
            object templName = fileName;
            bool ok = false;
            String nam = Path.GetFileName(fileName);

            for (int x = 1; x <= XL.Workbooks.Count; x++)
            {
                Workbook wb = XL.Workbooks[x];
                if (wb.Name == nam)
                {
                    ok = true;
                    wb.Activate();
                    break;
                }
            }
            if (!ok)
            {
                XL.Workbooks.Add(templName);
                string n = XL.Workbooks[1].FullName;

            }
            return XL;
        }

        /// <summary>
        /// если такая книга существует то она будет возвращена иначе она будет создана (если есть путь)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Workbook GetWorkbook(string fileName)
        {
            Excel.Application XL = GetExcelApplication();
            if (XL == null)
            {
                return null;
            }
            String nam = Path.GetFileName(fileName);
            Workbook wb = null;

            wb = XL.GetWorkbook(nam);
            if (wb == null)
            {
                if (File.Exists(fileName))
                {
                    wb = XL.Workbooks.Add(fileName);

                }
                else //нет такого файла добавим
                {
                    wb = XL.Workbooks.Add();
                    wb.SaveAs(fileName);
                }


            }
            return wb;
        }

        /// <summary>
        /// создает новую книгу
        /// </summary>
        /// <returns></returns>
        public static Workbook GetWorkbook()
        {
            Excel.Application XL = GetExcelApplication();
            return XL.Workbooks.Add();
        }

        public static void MakeVisible(bool visible)
        {
            Excel.Application XL = GetExcelApplication(); if (XL == null)
            {
                return;
            }

            XL.Visible = visible;

        }


        public static Workbook GetWorkbook(this Excel.Application XL, string nam)
        {
            for (int x = 1; x <= XL.Workbooks.Count; x++)
            {
                Workbook wb = XL.Workbooks[x];
                if (wb.Name == nam)
                {
                    return wb;

                }
            }
            return null;
        }


        public static Worksheet getList(this  Workbook WB, int nr)
        {
            Worksheet WS = null;
            try
            {
                WS = (Worksheet)WB.Worksheets[nr];
            }
            catch { }
            return WS;
        }

        public static void SaveAs(this Workbook WB, string name)
        {
            Excel.XlFileFormat ff = Excel.XlFileFormat.xlWorkbookDefault;

            SaveAs(WB, name, ff);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WB"></param>
        /// <param name="name"></param>
        /// <param name="fileFormat"></param>
        public static void SaveAs(Excel._Workbook WB, string name, Excel.XlFileFormat fileFormat)
        {

            try
            {

                if (File.Exists(name))
                {
                    File.Delete(name);
                }
                //name = Path.GetFileNameWithoutExtension(name);

                WB.SaveAs(name, fileFormat, Type.Missing,  Type.Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("0x800A03EC") > 0)
                {
                    throw new RSaveExeption(name + " Сохранение отмененно пользователем");
                }
                else
                {
                    throw ex;
                }
            }
        }


       /// <summary>
        /// 
        /// </summary>
        /// <param name="WB"></param>
        /// <param name="name"></param>
        /// <param name="fileFormat"></param>
        public static void Save(Excel._Workbook WB, string name)
        {

            try
            {

                if (File.Exists(name))
                {
                    File.Delete(name);
                }
                //name = Path.GetFileNameWithoutExtension(name);

                WB.SaveAs(name, Type.Missing, Type.Missing,  Type.Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("0x800A03EC") > 0)
                {
                    throw new RSaveExeption(name + " Сохранение отмененно пользователем");
                }
                else
                {
                    throw ex;
                }
            }
        }



        /// <summary>
        /// преобразование числового адреса колонки в строку
        /// </summary>
        /// <param name="columnNumberOneBased"></param>
        /// <returns></returns>
        public static string LetterAddres(int columnNumberOneBased)
        {
            int baseValue = Convert.ToInt32('A');
            int columnNumberZeroBased = columnNumberOneBased - 1;

            string ret = "";

            if (columnNumberOneBased > 26)
            {
                ret = ExcelWorking.LetterAddres(columnNumberZeroBased / 26);
            }

            return ret + Convert.ToChar(baseValue + (columnNumberZeroBased % 26));
        }

        public static void Save_Grid_in_Excel(DataGrid DT, List<string> BindingPathes, bool selectedRows)
        {
            if ((DT.SelectedItems.Count == 0) && (selectedRows)) { return; }
            try
            {
                int c = 0, Otb = 0;

                Workbook WB = null;
                Excel._Worksheet WS = null;
                Excel.Range range = null;

                WB = GetWorkbook();
                WS = getList(WB, 1);//Получили ссылку на первый лист

                Dictionary<int, int> orderCompare = new Dictionary<int, int>();
                for (int col = 0; col < DT.Columns.Count; col++)//пребор колонок
                {
                    orderCompare.Add(DT.Columns[col].DisplayIndex, col);//заполнили коллекцию соответствия номеров колонок их порядку в отображении
                }

                for (c = 0; c < DT.Columns.Count; c++)//построение заголовка
                {
                    if (DT.Columns[orderCompare[c]].Visibility == Visibility.Visible)
                    {
                        range = (Excel.Range)WS.get_Range(LetterAddres(Otb + 1) + "1", Type.Missing);
                        range.Value2 = DT.Columns[orderCompare[c]].Header.ToString().Replace("\r", "");
                        range.ColumnWidth = (int)DT.Columns[orderCompare[c]].Width.DisplayValue / 5;
                        //range.Columns.AutoFit();
                        range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1);//сделали рамку
                        range.Interior.ColorIndex = 15;//закрасили внутри заголовка серым
                        Otb++;
                    }
                }

                int RowsCount;
                if (selectedRows) { RowsCount = DT.SelectedItems.Count; }
                else { RowsCount = DT.Items.Count; }
                if (RowsCount == 0) return;

                object[,] dArr = new object[RowsCount, Otb];//создали пустой двухмерный массив

                for (int r = 0; r < RowsCount; r++)
                {
                    for (c = 0, Otb = 0; c < DT.Columns.Count; c++)//ряда
                    {
                        if (DT.Columns[orderCompare[c]].Visibility == Visibility.Visible)
                        {
                            string x = String.Empty;
                            try
                            {
                                if (selectedRows)
                                {
                                    //x = ((RSQLDataRowWrapper)dt.SelectedItems[r - 2]).Row[BindingPathes[OrderCompare[c]]].ToString();
                                    int ordC = orderCompare[c];
                                    try
                                    {
                                        x = GetSubItem(DT.SelectedItems[r], BindingPathes[ordC]).ToString();
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        x = GetSubItem(DT.Items[r], BindingPathes[orderCompare[c]]).ToString();
                                    }
                                    catch { }
                                }
                            }
                            catch (Exception ex) { MessageBox.Show("   \n" + ex.Message); }

                            dArr[r, Otb] = x;
                            Otb++;
                        }
                    }
                }
                string A = LetterAddres(Otb) + (RowsCount).ToString();
                range = (Excel.Range)WS.get_Range("A2", A);//получили диапазон в который должны лечь значения
                //range.NumberFormat = "General";// "@";
                range.Value2 = dArr;//присвоили значение диапазона 
                range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = 2;
                range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = 2;

                range.BorderAround(1, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, 1);
            }
            catch (Exception ex) { MessageBox.Show("   \n" + ex.Message); }
        }

        /// <summary>
        /// старт и заполнение Ехсел в отдельном потоке
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnNames"></param>
        public static void Show_Data_in_Excel(List<List<RCellValue>> dt, List<RCellValue> columnNames)
        {
           // List<RCellValue> columnNames = WPFHelper.GetColumnsOrdered(dataGrid);

          
            Thread thread = new Thread
                (delegate()
                {
                    ExcelWorking.Show_Data_In_Excel(dt, columnNames);
                }
                );
            thread.Start();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="Report"></param>
        /// <param name="Collection"></param>
        /// <param name="SavePath"></param>
        public static void Save_Data_In_Excel(RSQLCollection collection, string SavePath)
        {
            int c = 0, Otb = 0;
            Excel.Application XL = null;
            Workbook WB = null;
            Excel._Worksheet WS = null;
            Excel.Range range = null;
            List<RCellValue> columnNames = new List<RCellValue>();
            foreach (DataColumn reportColumn in collection.Columns)
            {
                if (collection.Report != null)
                {
                   // collection.Report.
                }
                int width = (int)reportColumn.MaxLength;
                if (width == 0) { width = 100; }
                if (width > 1250) { width = 1250; }
                columnNames.Add(new RCellValue(reportColumn.Caption, reportColumn.Caption, width,String.Empty));

            }
            try
            {

                WB = GetWorkbook();
                WS = getList(WB, 1);//Получили ссылку на первый лист
                for (c = 0; c < columnNames.Count; c++)//построение заголовка
                {
                    range = (Excel.Range)WS.get_Range(LetterAddres(Otb + 1) + "1", Type.Missing);
                    range.Value2 = columnNames[c].Text.Replace("\r", "");
                    if (columnNames[c].Wigth > 0)
                    {
                        range.ColumnWidth = columnNames[c].Wigth / 5;
                    }
                    range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1);//сделали рамку
                    range.Interior.ColorIndex = 15;//закрасили внутри заголовка серым
                    Otb++;//

                }

                int rowsCount;
                { rowsCount = collection.Count; }

                if (rowsCount != 0)
                {
                    object[,] dArr = new object[rowsCount, Otb];//создали пустой двухмерный массив

                    for (int r = 0; r < rowsCount; r++)
                    {
                        for (c = 0; c < columnNames.Count; c++)//ряда
                        {
                            string x = collection[r][columnNames[c].Name].ToString();
                            dArr[r, c] = x;
                        }
                    }
                    string cell2 = LetterAddres(Otb) + (rowsCount).ToString();
                    range = (Excel.Range)WS.get_Range("A2", cell2);//получили диапазон в который должны лечь значения
                    range.Value2 = dArr;//присвоили значение диапазона 
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = 2;
                    range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = 2;

                    range.BorderAround(1, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, 1);
                }
            }
            catch (Exception ex) { MessageBox.Show("   \n" + ex.ToString()); }

            try
            {
                SaveAs(WB, SavePath);
            }
            finally
            {
                Excel._Application apl = WB.Parent as Excel._Application;
                object False = false;
                apl.ActiveWorkbook.Close(False, Type.Missing, Type.Missing);
                int i = apl.Workbooks.Count;
                apl.Quit();
                GC.SuppressFinalize(apl);
                //apl.Workbooks.Add(Type.Missing);
                //apl.Visible = true; 
                //    int ii = apl.Workbooks.Count;   
            }
        }
        // сохранение в Exсel 

           
        public static void SaveFileInExcel(Excel.Application XL, List<List<RCellValue>> DT, List<RCellValue> columnNames, string savePath, XlFileFormat ff = XlFileFormat.xlWorkbookDefault)
        {

            Workbook WB = XL.Workbooks.Add(Type.Missing);
            Show_Data_In_Excel(DT, columnNames, XL, WB);
            ExcelWorking.SaveAs(WB, savePath,ff);
            object FALSE = false;
            WB.Close(FALSE, Type.Missing, Type.Missing);
        }

        // сохранение в Exсel 
        public static void SaveFileInExcel_old(Excel.Application XL, ObservableCollection<RSQLDataRowWrapper> collection, List<RCellValue> columnNames, string savePath)
        {
            int c = 0, Otb = 0;

            Workbook WB = null;
            Worksheet WS = null;
            Excel.Range range = null;
            try
            {
                XL.Workbooks.Add(Type.Missing);
                WB = XL.ActiveWorkbook;
                WS = ExcelWorking.getList(WB, 1);//Получили ссылку на первый лист
                for (c = 0; c < columnNames.Count; c++)//построение заголовка
                {
                    range = (Excel.Range)WS.get_Range(ExcelWorking.LetterAddres(Otb + 1) + "1", Type.Missing);
                    range.Value2 = columnNames[c].Text.Replace("\r", "");
                    range.ColumnWidth = columnNames[c].Wigth / 5;
                    range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1);//сделали рамку
                    range.Interior.ColorIndex = 15;//закрасили внутри заголовка серым
                    Otb++;//
                }
            }
            catch (Exception ex)
            {
                REnvironment.Current.Protocol.PutMessage(RProtocolMessageTypes.Debug, "Oшибка добавления книги  Excel \n" + ex.ToString());
            }

            int RowsCount;
            { RowsCount = collection.Count; }
            if (RowsCount == 0) return;

            object[,] dArr = new object[RowsCount, Otb];//создали пустой двухмерный массив

            for (int r = 0; r < RowsCount; r++)
            {
                for (c = 0; c < columnNames.Count; c++)//ряда
                {
                    try
                    {
                        string x = collection[r][columnNames[c].Name].ToString();
                        dArr[r, c] = x;
                    }
                    catch (Exception)
                    {


                    }

                }
            }
            string A = ExcelWorking.LetterAddres(Otb) + (RowsCount + 1).ToString();
            range = (Excel.Range)WS.get_Range("A2", A);//получили диапазон в который должны лечь значения
            range.Value2 = dArr;//присвоили значение диапазона 
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = 2;
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = 2;
            range.BorderAround(1, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, 1);
            ExcelWorking.SaveAs(WB, savePath);
            object FALSE = false;
            WB.Close(FALSE, Type.Missing, Type.Missing);
        }


        public static Workbook Show_Data_In_Excel_adv(List<List<RCellValue>> dt, List<RCellValue> columnNames,string name= null,string header=null)
        {

            Workbook wb = null;
            Excel.Application xl = GetExcelApplication();

            if (xl == null)
            {
                return null;
            }
           

            try
            {

                wb = xl.Workbooks.Add();

            }
            catch (Exception)
            {

                xl = new Excel.Application();
                wb = xl.Workbooks.Add();
            }
            var otb = 0;
    var x=   (Worksheet) wb.Worksheets.get_Item(1) ;
            if (name != null)
            {
                x.Name =  name.Substring(0,30);
            }
            if (header != null)
            {
              Worksheet ws=  wb.getList(1);
                Excel.Range r = ws.get_Range("A1");
                r.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
              r.Value2 = "GOBBLYDEEGOOK";
                otb = 1;

            }
            Show_Data_In_Excel(dt, columnNames, xl, wb,otb);
          xl.Visible = true;
          
            return wb;
        }

       
        public static void Show_Data_In_Excel(List<List<RCellValue>> dt, List<RCellValue> columnNames,string name= null,string header=null)
        {
         
           Workbook WB =  Show_Data_In_Excel_adv(dt, columnNames,null,null);
          
        }


  /// <summary>
  /// 
  /// </summary>
  /// <param name="dt"></param>
  /// <param name="columnNames"></param>
  /// <param name="xl"></param>
  /// <param name="wb"></param>
  /// <param name="otb"></param>
        public static void Show_Data_In_Excel(List<List<RCellValue>> dt, List<RCellValue> columnNames, Excel.Application xl, Workbook wb,int otb = 0)
        {

            int MaxRowOnList = dt.Count+1;
            if (xl.Version.Contains("11."))//старые версии не работают со строками более 65 тысяч
            {
                MaxRowOnList = 60000;
            }


            int RowsCount = dt.Count;
            int List = 1;
            try
            {

                #region инициализация Excel



                #endregion
                int shift = 0; //сдвиг в записях

                for (int i = 1; shift < RowsCount; i++)//Заранее добавим нужное количество листов
                {
                    if (wb.Worksheets.Count < i)
                    {
                        wb.Worksheets.Add();
                    }
                    shift = shift + MaxRowOnList;
                }

                shift = 0;

                for (; shift < RowsCount; List++)
                {
                    FullExcelList(dt, columnNames, List, wb, otb, MaxRowOnList, shift);
                    shift = shift + MaxRowOnList;
                }
                wb.Saved = true;

            }
            catch (Exception ex) { MessageBox.Show("   \n" + ex.ToString()); }


        }





        private static void FullExcelList(List<List<RCellValue>> dataList, List<RCellValue> columnNames, int exelListNumber, Workbook workbook, int otb, int max, int shift)
        {
            int rowsCount = Math.Min(max, dataList.Count - shift);//реальное количество рядов это сдвиг 
            Range range;
           
            Worksheet worksheet = getList(workbook, exelListNumber);
            int columnCount;
            int[] fractPart = new int[Math.Max(columnNames.Count, dataList[0].Count)];

            #region               построение заголовка

            for (int i = 0; i < columnNames.Count; i++) //
            {
                range = (Excel.Range)worksheet.get_Range(LetterAddres(otb + 1) + "1", Type.Missing);
                range.Value2 = columnNames[i].Text.Replace("\r", "");
                range.ColumnWidth = Math.Min(columnNames[i].Wigth / 5, 255);
                range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1); //сделали рамку
                range.Interior.ColorIndex = 15; //закрасили внутри заголовка серым
                otb++;
            }
            if (otb == 0) 
            {
                REnvironment.Current.ShowExceptionMessage("В запросе нет ни одной колокни из определенных в отчете");
                return;
            }
            #endregion

         
            if (rowsCount == 0) return;
            columnCount = Math.Min(columnNames.Count, dataList[0].Count);
            Dictionary<int, int> accordance = new Dictionary<int, int>();
           for (int i = 0; i < columnNames.Count; i++) 
            {
                for (int dataListCounter = 0; dataListCounter < dataList[0].Count; dataListCounter++)
                {
                   
                    if (   dataList[0][dataListCounter].Name == columnNames[i].Name)
                    {
                        if (dataList[0][dataListCounter].Name != null)
                        { 
                            accordance.Add(i,dataListCounter); 
                        }
                        else
                        { 
                            accordance.Add(i, i); 
                        }
                        break;
                    }
                }
            }
            
            object[,] arrValues = new object[rowsCount + 1, columnCount];
            Type[] arrColumnTypes = new Type[dataList[0].Count]; //создадим массив для типов колонок
           
            Type t = typeof(string);
            string TextVal = "";
            int rr = shift;
            int realRow = 0;
           int c = 0;
            try
            {
                for (; rr < rowsCount + shift; rr++, realRow++) //перебор строк таблицы
                {
                   
                    for (int i=0; i < columnCount; i++) //проход по колонкам строки
                    {
                        if(!accordance.ContainsKey(i))
                            continue;
                        c = accordance[i];

                        TextVal = dataList[rr][c].Text;

                        t = dataList[rr][c].Type;

                        #region определение типа колонок для установки типа ячейки

                        if (t != null && t != typeof(DBNull) && arrColumnTypes[c] == null)
                        {
                            arrColumnTypes[c] = t;//первичная установка типа 

                        }
                        if (arrColumnTypes[c] != t && t == typeof(string))//если хоть одна ячейка строка то тип желательно строка то же
                        {
                            arrColumnTypes[c] = t;

                        }
                        
                        #endregion

                        if (t != null && t.IsNumericType())
                        {
                            int cou = 0;
                            foreach (char cha in TextVal)
                            {
                                if (cha == '.' || cha == ',')
                                {

                                    if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                                    {
                                        TextVal = TextVal.Replace(".", ",");
                                    }
                                    else
                                    {
                                        TextVal = TextVal.Replace(",", ".");
                                    }
                                    int fp = TextVal.Length - cou - 1;
                                    fractPart[c] = Math.Max(fractPart[c], fp);

                                    break;
                                }
                                cou++;
                            }



                        }
                        if (t != null && t != typeof(DBNull))
                        {
                            try
                            {
                                arrValues[realRow, i] = Convert.ChangeType(TextVal, t);
                            }
                            catch
                            {
                                if (t == typeof(DateTime))
                                {
                                    arrValues[realRow, i] = TextVal;
                                }
                                else
                                { arrValues[realRow, i] = TextVal; }
                            }

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("  row= " + rr + "   col= " + c + " type=" + t.Name + " val=" + TextVal +
                                "   \n" + Ex);
            }

            for (int ix = 0; ix < columnNames.Count; ix++) //ряда
            {
                if (!accordance.ContainsKey(ix))
                    continue;
                c = accordance[ix];
                if(c>=arrColumnTypes.Length)
                    break;
                string A = LetterAddres(ix + 1) + (0 + 2).ToString();
                string B = LetterAddres(ix + 1) + (rowsCount + 2).ToString();
                range = (Excel.Range)worksheet.get_Range(A, B);
                if ( arrColumnTypes[c]!=null &&  arrColumnTypes[c].IsNumericType())
                {
                    if (fractPart[c] == 0)
                    {
                        range.NumberFormat = "0";
                    }
                    else
                    {
                        string x = string.Empty;
                        for (int i = 0; i < fractPart[c]; i++)
                        {
                            x += '0';
                        }
                        range.NumberFormat = "#." + x;
                    }
                }
                else
                    if (arrColumnTypes[c] !=null && arrColumnTypes[c] == typeof(DateTime))
                    {

                        range.NumberFormat = "dd/mm/yyyy hh:mm:ss";
                    }
                    else
                    {
                        range.NumberFormat = "@";
                    }
            }

            string AA = LetterAddres(otb) + (rowsCount + 1).ToString();
            range = (Excel.Range)worksheet.get_Range("A2", AA);
            range.Value2 = arrValues;
            try
            {
                range = (Excel.Range)worksheet.get_Range("A2", AA); //получили диапазон в который должны лечь значения
                range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = 2;
                range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = 2;

                range.BorderAround(1, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, 1);
            }
            catch
            {
            }

        }

        public static void FullExcelList(System.Data.DataTable DT)
        {

            var wb = GetWorkbook();
            Worksheet WS = getList(wb, 1);
            FullExcelList(DT, 0, 0, WS);

        }

        public static void FullExcelList(List<INamedValue> namedValues, int offsetX, int offsetY, Worksheet WS)
        {

            Range range;


            int ColumnCount = namedValues.Count;



            for (int c = 0; c < ColumnCount; c++) //
            {

                INamedValue nv = namedValues[c];

                if (nv != null)
                {

                    string val = string.Empty;
                    IEnumerable enumeravle = nv.Value as IEnumerable;
                    if (enumeravle != null)
                    {

                        int x = 0;
                        foreach (var VARIABLE in enumeravle)
                        {
                            if (x != 0) val += ",";
                            val += VARIABLE.ToString();
                            x++;
                        }

                    }
                    else
                    {
                        val = nv.Value.ToString();
                    }




                    range = WS.get_Range(LetterAddres(c + 1 + offsetX) + offsetY, Type.Missing);
                    range.Value2 = nv.Name + "=" + val;
                    range.BorderAround(1, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, 1); //сделали рамку
                    range.Interior.ColorIndex = 15; //закрасили внутри заголовка серым
                }
            }



        }


        public static int FullExcelList_old(List<INamedValue> namedValues, int offsetX, int offsetY, Worksheet WS)
        {
            int RowsCount = 2;
            Range range;
            int c;

            int ColumnCount = namedValues.Count;


            #region               построение заголовка
            for (c = 0; c < ColumnCount; c++) //
            {
                range = WS.get_Range(LetterAddres(c + 1 + offsetX) + offsetY, Type.Missing);
                range.Value2 = namedValues[c].Name.Replace("\r", "");
                range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1); //сделали рамку
                range.Interior.ColorIndex = 15; //закрасили внутри заголовка серым

            }



            #endregion


            #region               построение заголовка

            for (c = 0; c < ColumnCount; c++) //
            {
                INamedValue nv = namedValues[c];
                if (nv != null)
                {
                    IEnumerable enumeravle = nv.Value as IEnumerable;
                    if (enumeravle != null)
                    {
                        int x = 0;
                        foreach (var VARIABLE in enumeravle)
                        {
                            WS.FullCell(c + 1 + offsetX, offsetY + 1 + x, VARIABLE);
                            x++;

                        }
                        if (x > RowsCount)
                        { RowsCount = x; }
                    }
                    else
                    {
                        WS.FullCell(c + 1 + offsetX, offsetY + 1, namedValues[c].Value);
                    }
                }
            }

            #endregion

            return RowsCount + offsetY + 2;
        }

        public static void FullExcelList(System.Data.DataTable DT, int offsetX, int offsetY, Worksheet WS)
        {
            int RowsCount = DT.Rows.Count;
            Range range;
            int c;

            int ColumnCount = DT.Columns.Count;


            #region               построение заголовка

            for (c = 0; c < ColumnCount; c++) //
            {
                range = WS.get_Range(LetterAddres(c + 1 + offsetX) + offsetY, Type.Missing);
                range.Value2 = DT.Columns[c].ColumnName.Replace("\r", "");
                range.BorderAround(1, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, 1); //сделали рамку
                range.Interior.ColorIndex = 15; //закрасили внутри заголовка серым

            }

            #endregion

            if (RowsCount == 0) return;
            ColumnCount = DT.Columns.Count;
            object[,] arrValues = new object[RowsCount + 1, ColumnCount];

            Type t = typeof(string);
            string TextVal = "";

            int rr = 0;
            try
            {
                for (rr = 0; rr < RowsCount; rr++) //перебор строк таблицы
                {
                    c = 0;
                    for (; c < ColumnCount; c++) //проход по колонкам строки
                    {
                        TextVal = DT.Rows[rr][c].ToString();
                        t = DT.Columns[c].DataType;



                        if (t.IsNumericType())
                        {
                            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                            {
                                TextVal = TextVal.Replace(".", ",");
                            }
                            else
                            {
                                TextVal = TextVal.Replace(",", ".");
                            }

                        }
                        if (t != null && t != typeof(DBNull))
                        {


                            try
                            {
                                arrValues[rr, c] = Convert.ChangeType(TextVal, t);
                            }
                            catch
                            {
                                if (t == typeof(DateTime))
                                {
                                    arrValues[rr, c] = TextVal;
                                }
                                else
                                { arrValues[rr, c] = TextVal; }
                            }

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("  row= " + rr + "   col= " + c + " type=" + t.Name + " val=" + TextVal +
                                "   \n" + Ex);
            }

            for (c = 0; c < ColumnCount; c++) //ряда
            {
                string A = LetterAddres(c + 1 + offsetX) + (offsetY + 2).ToString();
                string B = LetterAddres(c + 1 + offsetX) + (offsetY + RowsCount + 2).ToString();
                range = WS.get_Range(A, B);
                if (DT.Columns[c].DataType.IsNumericType())
                {

                    range.NumberFormat = "#";
                }
                else
                    if (DT.Columns[c].DataType == typeof(DateTime))
                    {

                        range.NumberFormat = "dd/mm/yyyy hh:mm:ss";
                    }
                    else
                    {
                        range.NumberFormat = "@";
                    }
            }
            string aa = LetterAddres(1 + offsetX) + (offsetY + 1);
            string BB = LetterAddres(ColumnCount + offsetX) + (offsetY + RowsCount + 1);

            range = WS.get_Range(aa, BB);
            range.Value2 = arrValues;
            //try
            //{

            //    range.Borders[XlBordersIndex.xlInsideHorizontal].Weight = 2;
            //    range.Borders[XlBordersIndex.xlInsideVertical].Weight = 2;

            //    range.BorderAround(1, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, 1);
            //}
            //catch
            //{
            //}

        }

        public static void FullCellsHorizontal(this Worksheet WS, int x, int y, params object[] val)
        {
            int offset = 0;
            foreach (var o in val)
            {
                WS.FullCell(x + offset, y, o);
                offset++;
            }

        }
        public static void FullCell(this Worksheet WS, int x, int y, object val)
        {
            string adr = LetterAddres(x + 1) + y;
            Range range = WS.get_Range(adr, Type.Missing);
            range.Value2 = val;

        }

        public static object GetCell(this Worksheet WS, int x, int y)
        {
            Range range = WS.get_Range(LetterAddres(x + 1) + y, Type.Missing);
            return range.Value2;

        }

        public static String GetCellS(this Worksheet WS, int x, int y)
        {
            Range range = WS.get_Range(LetterAddres(x + 1) + y, Type.Missing);
            if (range.Value2 != null)
            {
                object sx = range.NumberFormat;
                return range.Value.ToString();

            }
            return string.Empty;
        }
        /// <summary>
        /// один ряд 
        /// </summary>
        /// <param name="WS"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        public static List<String> GetCellSRow(this Worksheet WS, int x, int y, int x2)
        {
            List<String> ls = new List<string>();
            Range range = WS.get_Range(LetterAddres(x + 1) + y, LetterAddres(x2 + 1) + y);
            object[,] cells = (object[,])range.get_Value(XlRangeValueDataType.xlRangeValueDefault);
            for (int r = cells.GetLowerBound(1); r <= cells.GetUpperBound(1); r++)
            {
                object cell = cells[1, r];
                if (cell != null)
                {
                    ls.Add(cell.ToString());
                }
            }

            return ls;
        }
        /// <summary>
        ///  получение из листа двухмерного массива значений
        /// </summary>
        /// <param name="WS"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static object[,] GetArray(this Worksheet WS, int x, int y, int x2, int y2)
        {
            string a1 = LetterAddres(x + 1) + y;
            string a2 = LetterAddres(x2 + 1) + y2;
            Range range = WS.get_Range(a1, a2);
            return (object[,])range.get_Value(XlRangeValueDataType.xlRangeValueDefault);
        }


        public static object[,] GetArray(string fileName)
        {
            var workbook = ExcelWorking.GetWorkbook(fileName);
            var list = workbook.getList(1);
           int   lastRow = list.LastRow();
           int   lastColumn = list.LastColumn();
            var array = list.GetArray(0,1,lastColumn,lastRow);
            return array;
        }

        public static DateTime? GetCellD(this Worksheet WS, int x, int y)
        {
            Range range = WS.get_Range(LetterAddres(x + 1) + y, Type.Missing);
            object value = range.Value2;
            if (value != null)
            {
                DateTime reTime;
                if (value is double)
                {
                    reTime = DateTime.FromOADate((double)value);
                }
                else
                {
                    DateTime.TryParse((string)value, out reTime);
                }
                return reTime;
            }
            return null;
        }
        #region служебные методы

        /// <summary>
        /// получение двухмерного массива
        /// </summary>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public static object[,] GetArray(this Excel._Worksheet Sheet)
        {
            object[,] srcArr = (object[,])Sheet.UsedRange.get_Value(Excel.XlRangeValueDataType.xlRangeValueDefault);
            return srcArr;
        }

        /// <summary>
        /// последний используемый ряд
        /// </summary>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public static int LastRow(this Excel._Worksheet Sheet)
        {
            return Sheet.UsedRange.Row - 1 + Sheet.UsedRange.Rows.Count;
        }

        /// <summary>
        /// последняя используемая колонка
        /// </summary>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public static int LastColumn(this Excel._Worksheet Sheet)
        {
            return Sheet.UsedRange.Column - 1 + Sheet.UsedRange.Columns.Count;
        }


        #region методы перенесенные из core

        public static object GetSubItem(object obj, string Name)
        {
            RSQLDataRowWrapper RSQLDataRowWrapper = obj as RSQLDataRowWrapper;
            if (RSQLDataRowWrapper != null)
            {
                try { return RSQLDataRowWrapper[Name]; }
                catch { return null; }
            }
            else //возможно тип обьект 
            {
                return GetObjectSubItem(obj, Name);
            }
        }

        public static object GetSubItem(object obj, int place)
        {
            RSQLDataRowWrapper RSQLDataRowWrapper = obj as RSQLDataRowWrapper;
            if (RSQLDataRowWrapper != null)
            {
                try { return RSQLDataRowWrapper[place]; }
                catch { return null; }
            }
            else //только нул
            {
                return null;
            }
        }
        /// <summary>
        /// получение свойств  любого объекта по имени свойства
        /// возможно использование вложенных свойств
        /// </summary>
        /// <param name="obj">объект свойство котрого нужно получтиь</param>
        /// <param name="Name">имя (путь) к свойсту</param>
        /// <returns></returns>
        public static object GetObjectSubItem(object obj, string Name)
        {
            if (obj == null) { return null; }
            Type type = obj.GetType();
            string patch = string.Empty;

            int DotePlace = Name.IndexOf('.');

            if (DotePlace > 0)
            {
                string basePath = Name.Substring(0, DotePlace);
                object BaseItem = ExcelWorking.GetObjectSubItem(obj, basePath);//получили объект верхнего уровня
                patch = Name.Substring(DotePlace + 1);
                return ExcelWorking.GetObjectSubItem(BaseItem, patch);
            }

            Name = Name.Trim();
            Name = Name.Trim('.');
            PropertyInfo PropertyInfo = type.GetProperty(Name);//базовая обработка
            if (PropertyInfo != null)
            {
                if (PropertyInfo.CanRead)
                {
                    MethodInfo am = PropertyInfo.GetGetMethod();
                    object x = am.Invoke(obj, null);//получили значения
                    return x;
                }
            }
            return null;
        }

        #endregion
        #endregion
    }




    /// <summary>
    /// при открытии книги возможны варианты 
    /// (она была уже открыта и /или были открыты другие книги)
    /// класс оставляет после выполнения метода MadeAsWos все как было
    /// </summary>
    public class ExcelWrapper
    {
        private Excel.Application _application;

        public Excel.Application ExcelApplication
        {
            get { return _application; }
            set { _application = value; }
        }

        List<string> _namesList = new List<string>();

        public Workbook GetWorkbook(string fileName)
        {
            _application = ExcelWorking.GetExcelApplication();
            if (_application == null)
            {
                return null;
            }
            string nam = Path.GetFileName(fileName);
            Workbook wb = _application.GetWorkbook(nam);
            if (wb == null)
            {
                wb = _application.Workbooks.Add(fileName);
                _namesList.Add(nam);
            }
            return wb;
        }

        public void MadeAsWos()
        {
            if (_application != null)
            {
                foreach (var nam in _namesList)
                {
                    Workbook wb = _application.GetWorkbook(nam);
                    if (wb != null)
                    {
                        wb.Close();
                    }
                }

                if (_application.Workbooks.Count == 0)
                    _application.Quit();

            }
        }
    }






    ///// <summary>
    ///// методы необходимые для получения текста из заголовка колонки грида (аналогичные методы есть в UBP.Controls) что не хорошо однако тут надо советоватся как все построить 
    ///// </summary>
    // static class WPFHelper
    //{
    //    internal static List<RCellValue> GetColumnsOrdered(DataGrid dataGrid)
    //    {
    //        var orderedColumns =
    //            from p in dataGrid.Columns
    //            orderby p.DisplayIndex
    //            select p;
    //        List<RCellValue> columnNames = new List<RCellValue>();
    //        foreach (DataGridColumn coll in orderedColumns)
    //        {
    //            if (coll.Visibility == Visibility.Visible)
    //            {
    //                var columnHeaderText = GetColumnHeaderText(coll);
    //                columnNames.Add(new RCellValue(columnHeaderText, Convert.ToInt32(coll.ActualWidth)));
    //            }
    //        }
    //        return columnNames;
    //    }

    //    internal static string GetColumnHeaderText(DataGridColumn coll)
    //    {
    //        string N = String.Empty;

    //        if (coll.Header != null)
    //        {
    //            N = coll.Header.ToString();
    //        }
    //        if (String.IsNullOrEmpty(N))
    //        {
    //            try
    //            {
    //                FrameworkElement templEl = coll.HeaderTemplate.LoadContent() as FrameworkElement;
    //                N = GetText_ch(templEl);
    //            }
    //            catch
    //            {
    //            }
    //        }
    //        if (String.IsNullOrEmpty(N))
    //        {
    //            N = coll.SortMemberPath;
    //        }
    //        return N;
    //    }

    //    public static string GetText_ch(FrameworkElement obj)
    //    {

    //        string text = String.Empty;
    //        List<FrameworkElement> DependencyObjects = GetAllVisualChildren(obj);
    //        List<FrameworkElement> f = DependencyObjects.FindAll(x => (x as TextBlock) != null || (x as TextBox) != null);
    //        foreach (FrameworkElement elem in f)
    //        {
    //            CheckBox check = elem as CheckBox;
    //            if (check != null)
    //            {
    //                try
    //                {
    //                    if ((bool)check.IsChecked) { text += " Истина"; }
    //                    else { text += " Ложь"; }
    //                }
    //                catch { text += " Не определенно"; }
    //            }
    //            text += " " + GetPlainText(elem);
    //        }
    //        return text.Trim();
    //    }

    //    /// <summary>
    //    /// Получение текстового значения из поля любого элемента
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <param name="Name"></param>
    //    /// <returns></returns>
    //    internal static string GetPlainText(FrameworkElement obj)
    //    {
    //        MethodInfo Metod = typeof(FrameworkElement).GetMethod("GetPlainText", BindingFlags.NonPublic | BindingFlags.Instance);
    //        object Objtext = Metod.Invoke(obj, null);
    //        if (Objtext != null) return Objtext.ToString();
    //        return String.Empty;
    //    }

    //    internal static List<FrameworkElement> GetAllVisualChildren(FrameworkElement parent)
    //    {
    //        List<FrameworkElement> result = new List<FrameworkElement>();
    //        if (parent != null)
    //        {
    //            if (parent.Visibility == Visibility.Visible)
    //            {
    //                result.Add(parent);

    //                int chCount = VisualTreeHelper.GetChildrenCount(parent);
    //                for (int i = 0; i < chCount; i++)
    //                {
    //                    FrameworkElement depObj = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
    //                    result.AddRange(GetAllVisualChildren(depObj));
    //                }
    //            }
    //        }
    //        return result;
    //    }
    //}
}
