using System;
using System.Reflection;
using System.Windows.Xps.Packaging;
using Word_ = Microsoft.Office.Interop.Word;
//using UBP.Collection;
using System.Data;
//using System.Windows.Forms;
using System.IO;
using System.Windows;
//using UBP.Common;
//using UBP.Protocol.Core;
//using UBP.Business.TypeManager.Report;

namespace UBP.DataExport
{
    /// <summary>
    /// Набор статических функций для работы с Вордом
    /// </summary>
    public static class WorkingWitchWord
    {
        static Object missing = Type.Missing;


        #region  Проверка установлен ли ворд на компе

        /// <summary>
        /// Проверка установлен ли ворд на компе
        /// </summary>
        /// <returns></returns>
        public static bool ExistWord()
        {
            Type word = Type.GetTypeFromProgID("Word.Application");
            return word != null;
        }

        private static bool? _existWordXP;

        /// <summary>
        /// Проверка установлен ли ворд xp на компе
        /// </summary>
        /// <returns></returns>
        public static bool ExistWordXP()
        {
            if (_existWordXP == null)
            {
                Type word = Type.GetTypeFromProgID("Word.Application.10");
                _existWordXP = word != null;
            }
            return (bool)_existWordXP;
        }

        private static bool? _existWord2003;

        /// <summary>
        /// Проверка установлен ли ворд 2003 на компе
        /// </summary>
        /// <returns></returns>
        public static bool ExistWord2003()
        {
            if (_existWord2003 == null)
            {
                Type word = Type.GetTypeFromProgID("Word.Application.11");
                _existWord2003 = word != null;
            }
            return (bool)_existWord2003;

        }

        private static bool? _existWord2007;

        /// <summary>
        /// Проверка установлен ли ворд 2007 на компе
        /// </summary>
        /// <returns></returns>
        public static bool ExistWord2007()
        {
            if (_existWord2007 == null)
            {
                Type word = Type.GetTypeFromProgID("Word.Application.12");
                _existWord2007 = word != null;
            }
            return (bool)_existWord2007;
        }

        private static bool? _existWord2010;

        /// <summary>
        /// Проверка установлен ли ворд 2010 на компе
        /// </summary>
        /// <returns></returns>
        public static bool ExistWord2010()
        {
            if (_existWord2010 == null)
            {
                Type word = Type.GetTypeFromProgID("Word.Application.14");
                _existWord2010 = (word != null);
            }
            return (bool)_existWord2010;
        }

        #endregion

        private static object fals = false;
        private static object empty = Type.Missing;

        /// <summary>
        /// Открывает существующий документ
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Word_.Document Open(string path)
        {
            Word_.Application wordAppl = new Word_.Application();

            Object filename = path;
            Object confirmConversions = Type.Missing;
            Object readOnly = Type.Missing;
            Object addToRecentFiles = Type.Missing;
            Object passwordDocument = Type.Missing;
            Object passwordTemplate = Type.Missing;
            Object revert = Type.Missing;
            Object writePasswordDocument = Type.Missing;
            Object writePasswordTemplate = Type.Missing;
            Object format = Type.Missing;
            Object encoding = Type.Missing;
            Object visible = true;
            Object openConflictDocument = Type.Missing;
            Object openAndRepair = Type.Missing;
            Object documentDirection = Type.Missing;
            Object noEncodingDialog = Type.Missing;
            if (wordAppl == null)
            {
                wordAppl = new Microsoft.Office.Interop.Word.Application();
            }

            wordAppl.Documents.Open(ref filename,
                ref confirmConversions,
                ref readOnly,
                ref addToRecentFiles,
                ref passwordDocument,
                ref passwordTemplate,
                ref revert,
                ref writePasswordDocument,
                ref writePasswordTemplate,
                ref format,
                ref encoding,
                ref visible,
                ref openConflictDocument,
                ref openAndRepair,
                ref documentDirection,
                ref noEncodingDialog);
            object x = wordAppl.Documents.Count;
            wordAppl.Visible = true;
            return wordAppl.Documents.get_Item(ref x);
        }

        public static Word_.Application TryGetRunningWordApplcation()
        {

            try
            {
                return System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application") as Word_.Application;
            }
            catch
            {
                return new Word_.Application();
            }


        }


        public static Word_.Application GetRunningWordApplcation()
        {
            try
            {
                return System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application") as Word_.Application;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// закрыть Word если есть 
        /// </summary>
        public static void CloseWordApplication(Word_.Application wordAppl)
        {
            try
            {
                wordAppl.Visible = true;

                foreach (Word_.Document doc in wordAppl.Documents)
                {
                    doc.Saved = true;
                }

                wordAppl.Quit();

            }
            catch
            {

            }
        }


        public static void CloseAllWordApplication()
        {


            try
            {
                int x = 0;
                while (true)
                {
                    x++; if (x > 100) return;
                    var wordAppl = GetRunningWordApplcation();

                    if (wordAppl == null) return;
                    CloseWordApplication(wordAppl);

                }

            }
            catch
            {

            }
        }


        /// <summary>
        /// Открывает существующий документ и оставляет ссылку на Application
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Word_.Document Open(string path, ref Word_.Application wordAppl)
        {
            Object filename = path;
            Object confirmConversions = Type.Missing;
            Object readOnly = Type.Missing;
            Object addToRecentFiles = Type.Missing;
            Object passwordDocument = Type.Missing;
            Object passwordTemplate = Type.Missing;
            Object revert = Type.Missing;
            Object writePasswordDocument = Type.Missing;
            Object writePasswordTemplate = Type.Missing;
            Object format = Type.Missing;
            Object encoding = Type.Missing;
            Object visible = Type.Missing;
            Object openConflictDocument = Type.Missing;
            Object openAndRepair = Type.Missing;
            Object documentDirection = Type.Missing;
            Object noEncodingDialog = Type.Missing;

            if (wordAppl == null)
            {
                wordAppl = new Word_.Application();
            }

            wordAppl.Documents.Open(ref filename,
                ref confirmConversions,
                ref readOnly,
                ref addToRecentFiles,
                ref passwordDocument,
                ref passwordTemplate,
                ref revert,
                ref writePasswordDocument,
                ref writePasswordTemplate,
                ref format,
                ref encoding,
                ref visible,
                ref openConflictDocument,
                ref openAndRepair,
                ref documentDirection,
                ref noEncodingDialog);
            object x = wordAppl.Documents.Count;

            return wordAppl.Documents.get_Item(ref x);
        }

        /// <summary>
        /// Добавляет пустой документ
        /// </summary>
        /// <param name="wordAppl">если null то будет созданна</param>
        /// <returns></returns>
        public static Word_.Document AddEmpty(ref Word_.Application wordAppl)
        {
            try
            {
                Object filename = Type.Missing;
                Object visible = true;


                if (wordAppl != null)
                {
                    wordAppl = new Word_.Application();
                }

                wordAppl.Documents.Add(ref filename, ref missing, ref visible, ref missing);
                object x = wordAppl.Documents.Count;
                return wordAppl.Documents.get_Item(ref x);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// добавляет в ворд новый документ на основе  шаблона
        /// </summary>
        /// <param name="templatePath">путь к шаблону документа</param>
        /// <param name="wordAppl">если null то будет созданна</param>
        /// <returns>ссылка на добавленный документ</returns>
        public static Word_.Document AddTemplated(string templatePath, ref Word_.Application wordAppl,
            bool madeNewApplication)
        {
            Object filename = templatePath;

            if (wordAppl == null)
            {
                if (madeNewApplication)
                {
                    wordAppl = new Word_.Application();
                }
                else
                {
                    wordAppl = TryGetRunningWordApplcation();
                }

                wordAppl.DisplayAlerts = Word_.WdAlertLevel.wdAlertsNone;
            }

            return wordAppl.Documents.Add(ref filename, ref fals, ref missing, ref fals);

        }

        /// <summary>
        /// чтение документа в текстовом виде
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path)
        {
            Word_.Application WordAppl = null;
            Word_.Document Doc = Open(path, ref WordAppl);
            object start = 0;
            object stop = Doc.Characters.Count;
            Word_.Range Rng = Doc.Range(ref start, ref stop);
            string Result = Rng.Text;
            object sch = Type.Missing;
            object aq = Type.Missing;
            object ab = Type.Missing;
            WordAppl.Quit(ref sch, ref aq, ref ab);
            return Result;
        }

        /// <summary>
        /// Сохранение  и закрытие документа
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Doc"></param>
        public static void SaveAsAndCloce(string path, Word_.Document Doc)
        {

            Object Path = path;

            Doc.SaveAs(ref Path, ref missing, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing);
            Close(Doc);
        }


        /// <summary>
        /// получение конца документа
        /// </summary>
        /// <param name="finalWordDoc"></param>
        /// <returns></returns>
        public static Word_.Range GetEndRange(Word_.Document finalWordDoc)
        {
            object missing = Missing.Value;


            object what = Word_.WdGoToItem.wdGoToLine;


            object which = Word_.WdGoToDirection.wdGoToLast;


            Word_.Range endRange = finalWordDoc.GoTo(ref what, ref which, ref missing, ref missing);
            return endRange;
        }



        /// <summary>
        /// Сохранение  и з документа
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Doc"></param>
        public static void SaveAs(string path, Word_.Document Doc, Word_.WdSaveFormat SaveFormat)
        {
            Object FileFormat = SaveFormat;

            Object Path = path;
            Object oFalse = false;
            Doc.SaveAs(ref Path, ref FileFormat, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing
                , ref missing, ref missing, ref missing, ref missing);
        }

        public static void Close(Word_.Document Doc)
        {
            object oFalse = false;
            Doc.Close(ref oFalse, ref missing, ref missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(Doc);
            Doc = null;
        }



        ///// <summary>
        /// Пример от дяюшки Билла
        /// </summary>
        /// <param name="sourceTemplatePath"></param>
        /// <param name="outputDocPath"></param>
        /// <param name="sourceData"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public static bool MergeDataWithWordTemplate(string sourceTemplatePath, string outputDocPath, DataSet sourceData,
            out string errorString)
        {
            #region Declares

            errorString = "";
            //          Object missing = System.Reflection.Missing.Value; //null value
            Object oTrue = true;
            Object oFalse = false;
            Object oTemplatePath = sourceTemplatePath;
            Object oOutputPath = outputDocPath;
            Object oOutputPathTemp = outputDocPath.Substring(0, outputDocPath.IndexOf(".doc")) + "_temp.doc";
            Object sectionStart = (Object)Microsoft.Office.Interop.Word.WdSectionStart.wdSectionNewPage;

            Word_.Application oWord = null;
            Word_.Document oWordDoc = null; //the document to load into word application
            Word_.Document oFinalWordDoc = null; //the document to load into word application

            #endregion

            try
            {
                oWord = new Word_.Application(); //starts an instance of word
                oWord.Visible = false; //don't show the UI

                //create an empty document that we will insert all the merge docs into
                oFinalWordDoc = oWord.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //for each record in the dataset
                int count = 1;
                foreach (DataRow dr in sourceData.Tables[0].Rows)
                {
                    //Log.Out("MailMerge:CreateMergeDoc() adding a document for this record");

                    //insert a document for each record
                    oWordDoc = oWord.Documents.Add(ref oTemplatePath, ref missing, ref missing, ref missing);
                    if (oWordDoc.Fields.Count == 0)
                    {
                        //Log.Out("MailMerge:CreateMergeDoc() No template fields found in document:" + sourceTemplatePath);
                        return false;
                    }
                    oWordDoc.Activate(); //make current

                    // Perform mail merge field
                    foreach (Word_.Field myMergeField in oWordDoc.Fields)
                    {
                        Word_.Range rngFieldCode = myMergeField.Code;
                        String fieldText = rngFieldCode.Text;

                        // ONLY GET THE MAILMERGE FIELDS
                        if (fieldText.StartsWith(" MERGEFIELD"))
                        {
                            // THE TEXT COMES IN THE FORMAT OF
                            // MERGEFIELD MyFieldName \\* MERGEFORMAT
                            // THIS HAS TO BE EDITED TO GET ONLY THE FIELDNAME "MyFieldName"
                            Int32 endMerge = fieldText.IndexOf(@"\");
                            Int32 fieldNameLength = fieldText.Length - endMerge;
                            String fieldName = fieldText.Substring(11, endMerge - 11);

                            // GIVES THE FIELDNAMES AS THE USER HAD ENTERED IN .dot FILE
                            //field names with spaces in them have quotes on either end, so strip those
                            fieldName = fieldName.Trim().Replace("\"", "");

                            //Log.Out(Log.LOGLEVEL_STANDARD, "DeviceLabelPrinter:SendAndReceive() found word template field: " + fieldName);

                            //find a matching dataset column
                            foreach (DataColumn col in sourceData.Tables[0].Columns)
                            {
                                string key = col.ColumnName;
                                string value = dr[key].ToString();

                                // **** FIELD REPLACEMENT IMPLEMENTATION GOES HERE ****//
                                // THE PROGRAMMER CAN HAVE HIS OWN IMPLEMENTATIONS HERE
                                if (fieldName == key)
                                {
                                    //Log.Out(Log.LOGLEVEL_STANDARD, "DeviceLabelPrinter:SendAndReceive() setting value: " + value);

                                    myMergeField.Select();
                                    oWord.Selection.TypeText(value);
                                }
                            }
                        }
                    }

                    //Log.Out(Log.LOGLEVEL_STANDARD, "DeviceLabelPrinter:SendAndReceive() saving the doc");

                    //SAVE THE DOCUMENT as temp
                    oWordDoc.SaveAs(ref oOutputPathTemp, ref missing, ref missing, ref missing
                        , ref missing, ref missing, ref missing, ref missing
                        , ref missing, ref missing, ref missing, ref missing
                        , ref missing, ref missing, ref missing, ref missing);

                    //Log.Out(Log.LOGLEVEL_STANDARD, "DeviceLabelPrinter:SendAndReceive() closing the doc");

                    //CLOSE THE DOCUMENT
                    oWordDoc.Close(ref oFalse, ref missing, ref missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oWordDoc);
                    oWordDoc = null;

                    //NOW ADD THE NEW DOC TO THE MAIN DOC
                    oFinalWordDoc.Activate(); //make current
                    oWord.Selection.InsertFile(oOutputPathTemp.ToString(), ref missing, ref missing, ref missing,
                        ref missing);

                    if (count < sourceData.Tables[0].Rows.Count)
                        oWord.Selection.InsertBreak(ref sectionStart);

                    count++;
                }

                //SAVE THE FINAL DOC
                oFinalWordDoc.SaveAs(ref oOutputPath, ref missing, ref missing, ref missing
                    , ref missing, ref missing, ref missing, ref missing
                    , ref missing, ref missing, ref missing, ref missing
                    , ref missing, ref missing, ref missing, ref missing);

                //CLOSE THE FINAL DOC
                oFinalWordDoc.Close(ref oFalse, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oFinalWordDoc);
                oFinalWordDoc = null;

                //now delete the temp file
                File.Delete(oOutputPathTemp.ToString());

                //Log.Out(Log.LOGLEVEL_STANDARD, "DeviceLabelPrinter:SendAndReceive() Merge complete, printing document");

                return true;
            }
            catch (System.Exception ex)
            {
                errorString = ex.Message;
            }
            finally
            {
                //RELEASE WORD ITSELF
                oWord.Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;

                GC.Collect();
            }

            return false;
        }



        /// <summary>
        /// установка текска в закладки
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="bookmarkName"></param>
        /// <param name="val"></param>
        public static void TrySetBookmarkByFieldName(this Word_.Document doc, string bookmarkName, string val)
        {

            foreach (Word_.Bookmark bookmark in doc.Bookmarks)
            {
                if (bookmark.Name.ToUpper() == bookmarkName.ToUpper())
                {
                    bookmark.Range.Text = val;
                }
            }
        }




        /// <summary>
        /// Преобразование из формата в формат
        /// </summary>
        /// <param name="wordDocName"></param>
        /// <param name="xpsDocName"></param>
        /// <returns></returns>
        public static XpsDocument ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {

            if (!ExistWord2007() && !ExistWord2010())
            {
                throw new Exception("Для работы необходим Word 2007 или выше");
            }
            Microsoft.Office.Interop.Word.Application
                wordApplication = new Microsoft.Office.Interop.Word.Application();

            wordApplication.Documents.Add(wordDocName);
            Word_.Document doc = wordApplication.ActiveDocument;
            doc.SaveAs(xpsDocName, Word_.WdSaveFormat.wdFormatXPS);
            wordApplication.Quit();
            XpsDocument xpsDoc = new XpsDocument(xpsDocName, FileAccess.Read);
            return xpsDoc;
        }





        public static void SetFirstPageOrientation(Word_.Document currernWord, Word_.WdOrientation orientation)
        {


            Word_.WdStatistic stat = Word_.WdStatistic.wdStatisticPages;
            int num = currernWord.ComputeStatistics(stat, ref missing); // Get number of pages

            // for (int i = 0; i < num; i++)
            {

                var endRange = GetPageRange(currernWord, 0);
                endRange.PageSetup.Orientation = orientation;


            }


        }

        public static Word_.Range GetPageRange(this Word_.Document currernWord, int pageNum)
        {
            object missing = Type.Missing;
            object count = pageNum;
            object what = Word_.WdGoToItem.wdGoToPage;
            object which = Word_.WdGoToDirection.wdGoToAbsolute;
            Word_.Range startRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count,
                ref missing);
            object count2 = (int)count + 1;
            Word_.Range endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2,
                ref missing);

            //if you want to select last page
            if (endRange.Start == startRange.Start)
            {
                which = Word_.WdGoToDirection.wdGoToLast;
                what = Word_.WdGoToItem.wdGoToLine;
                endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2, ref missing);
            }

            endRange.SetRange(startRange.Start, endRange.End);
            var or = currernWord.PageSetup.Orientation;
            return endRange;
        }

        public static Word_.WdOrientation GetFirstPageOrientation(Word_.Document currernWord)
        {
            object missing = Type.Missing;
            object count = 0;
            object what = Word_.WdGoToItem.wdGoToPage;
            object which = Word_.WdGoToDirection.wdGoToAbsolute;

            Word_.Range startRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count,
                ref missing);
            object count2 = (int)count + 1;
            Word_.Range endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2,
                ref missing);

            //if you want to select last page
            if (endRange.Start == startRange.Start)
            {
                which = Word_.WdGoToDirection.wdGoToLast;
                what = Word_.WdGoToItem.wdGoToLine;
                endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2, ref missing);
            }

            endRange.SetRange(startRange.Start, endRange.End);

            return endRange.PageSetup.Orientation;
        }


    }
}


//пример хождения по страницам

//    object missing = System.Reflection.Missing.Value;

//                                    Word_.WdStatistic stat = Word_.WdStatistic.wdStatisticPages;
//                                   int num = currernWord.ComputeStatistics(stat, ref missing);    // Get number of pages

//                                   // for (int i = 0; i < num; i++)
//                                   // {
//                                   //     currernWord           // Go to page "i"
//                                   //             .GoTo(Word_.WdGoToItem.wdGoToPage, missing, missing, i+1.ToString());
//                                   //     currernWord           // Select whole page
//                                   //             .GoTo(Word_.WdGoToItem.wdGoToBookmark, missing, missing, "\\page");
//                                   //    
//                                   //     // Do whatever you want with the selection
//                                   // }
//                                   //----------------------------------------------------------------------

//                                    object what = Word_.WdGoToItem.wdGoToPage;
//                                    object which = Word_.WdGoToDirection.wdGoToAbsolute;
//                                    for (int i = 0; i < num; i++)
//                                    {

//                                        object count = i;




//                                    Word_.Range startRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count, ref missing);
//                                    object count2 = (int)count + 1;
//                                    Word_.Range endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2, ref missing);

//                                    //if you want to select last page
//                                    if (endRange.Start == startRange.Start)
//                                    {
//                                        which = Word_.WdGoToDirection.wdGoToLast;
//                                        what = Word_.WdGoToItem.wdGoToLine;
//                                        endRange = currernWord.ActiveWindow.Selection.GoTo(ref what, ref which, ref count2, ref missing);
//                                    }

//                                    endRange.SetRange(startRange.Start, endRange.End);

//                                    var or = endRange.PageSetup.Orientation;
//                                    //------------------------------
//                                   // string tmpPath = Path.GetTempFileName() + Guid.NewGuid().ToString().Substring(0, 5);
//                                   // WorkingWitchWord.SaveAsAndCloce(tmpPath, currernWord);
//                                    finalWordDoc.Activate();

//                                  //  Word_.WdStatistic stat = Word_.WdStatistic.wdStatisticPages;

//                                   // int pagecount = currernWord.ComputeStatistics(stat, Type.Missing);


//                                    var endRangeFinalDoc = WorkingWitchWord.GetEndRange(finalWordDoc);
//                                    endRange.Copy();
//                                    endRangeFinalDoc.Paste();
//}
