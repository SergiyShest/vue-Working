/*
 Version information:
    $Revision: 36792 $
    $Author: Shestakov $
    $Date: 2015-06-18 14:02:24 +0400 (Чт, 18 июн 2015) $
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Office.Interop.Word;
using UBP.Business.Core;
using UBP.Business.Money;
using UBP.Business.TypeManager.Report;
using UBP.BusinessFactory;
using UBP.Collection;
using UBP.Collection.Builder;
using UBP.Common;

using UBP.Core;
using UBP.Core.OperationEngine;
using UBP.Reports;
using UBP.TypeManager.Common;
using UBP.Unity.Core;

namespace UBP.DataExport
{
    /// <summary>
    /// выгрузка в файл средствами 3сard
    /// </summary>
    public class RSystemFileExport
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="report"></param>
        /// <param name="usinfColumnNames"></param>
        /// <param name="par"></param>
        /// <param name="mode">
        ///   0, "Microcoft Excel xls"  
        ///   1, "Microcoft Excel csv"  
        ///   2, "DataBaseFormat III dbf"   
        ///   3, "SimpleText "</param>
        /// <param name="fileEncoding"></param>
        /// <param name="fileDivision"></param>
        /// <param name="fileName"></param>
        public RSystemFileExport(IReport report, List<RCellValue> usingColumnNames, IList<ICheckedNamedValue> par, int mode,
            Encoding fileEncoding, bool fileDivision, string fileName, IScriptState scriptState)
        {
            if (!(mode == 1 || mode == 2 || mode == 3 || mode == 4))
            {
                throw new Exception("Не поддерживается режим " + mode); //исключили не неужные значения
            }
            if (mode == 2)
            {
                foreach (var usingColumn in usingColumnNames)
                {
                    this.UsingColumns.Add(usingColumn.Name);
                }
            }
            else
            {
                this._fileStreamDict = new Dictionary<string, FileStream>(StringComparer.InvariantCultureIgnoreCase);
            }
            string fileNameColumn = null;
            string realFileName = string.Empty;
            if (fileDivision)
            {
                fileNameColumn = report.FileDivisionColumnName; //колонка деления на файлы
                if (fileNameColumn == null)
                {
                    throw new Exception(string.Format("В отчете {0} не найдена колока для деления на файлы", report.ShortName));
                }
            }
            else
            {
                realFileName = FileHelper.SetExtentionAndCreateDirectory(fileName, (SystemFileExt)mode);
                //установили расшинение , создали директорию и удалили старые файлы
                // this._generatedFilesList.Add(fileName, realFileName);
            }
            DBFx dbfx = null;
            try
            {
                List<IBDObjectReference> wholeList = new List<IBDObjectReference>();
                IAuditManager auditManager = ServiceLocator.Instance.Resolve<IAuditManager>();
                bool isAuditPlaceEnabled = auditManager.IsAuditInReportEnabled(report.ShortName);

                this._generatedFilesList = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                using (IRSaveProvider provider = REnvironment.Current.SaveProviderPool.GetProvider())
                {
                    using (IRDataReader reader = (report.SqlExpression as RSqlQueryExpression).GetQueryReader(par, provider)) //получили ридер
                    {
                        if (!fileDivision)
                        {
                            this.MadeHeader(usingColumnNames, mode, fileEncoding, fileName, realFileName, reader);
                        }
                        int lineCount = 0;
                        while (reader.Read()) //фетч
                        {
                            if (this.Break)
                                break;
                            if (fileDivision)
                            {
                                fileName = reader[fileNameColumn].ToString(); //jk
                                if (!this._generatedFilesList.ContainsKey(fileName)) //нет имени файла 
                                {
                                    realFileName = FileHelper.SetExtentionAndCreateDirectory(fileName, (SystemFileExt)mode);
                                    this.MadeHeader(usingColumnNames, mode, fileEncoding, fileName, realFileName, reader);
                                }
                            }
                            if (mode == 2) //dbf
                            {
                                dbfx = this._dbfxDictionary[fileName]; //нашел нужный в массиве
                                dbfx.WriteDbfRow(reader); //
                            }
                            else
                            {
                                this.WriteTextRow(usingColumnNames, fileEncoding, fileName, ref lineCount, reader, mode);
                            }
                            if (isAuditPlaceEnabled)
                            {
                                // формирование списка аудируемых объектов
                                wholeList.AddRange(this.createRowPairCollection(report.PairsNamesColumnsCollection, reader));
                            }
                        }
                    }
                }
                if (isAuditPlaceEnabled)
                {
                    // запись списка параметров в строку
                    string pars = String.Empty;
                    foreach (INamedValue val in par)
                    {
                        pars += val.Name + "=" + val.Value + ";";
                    }
                    // выполнение записи объектов аудита по окончании 
                    auditManager.AuditObjectLinksCollection(wholeList, report, scriptState, report.SqlExpression.Text, pars);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
                this.CloseAll();
                throw;
            }
            finally
            {
                this.CloseAll();
            }
        }

        #region Поля

        /// <summary>
        /// прервать выполнение
        /// </summary>
        public bool Break;

        Dictionary<string, DBFx> _dbfxDictionary = new Dictionary<string, DBFx>(StringComparer.InvariantCultureIgnoreCase);
        private List<string> _usingColumns;
        object _obj;

        private Dictionary<string, FileStream> _fileStreamDict;
        // = new FileStream(folder + "\\" + tblname + ".DBF", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 500000);

        #endregion

        #region Свойства

        #region  Список сфрормированных файлов

        private Dictionary<string, string> _generatedFilesList;
        
        /// <summary>
        /// список сфрормированных файлов
        /// </summary>
        public Dictionary<string, string> GeneratedFilesList
        {
            get
            {
                return this._generatedFilesList;
            }
        }

        #endregion

        private List<string> UsingColumns
        {
            get
            {
                if (this._usingColumns == null)
                {
                    this._usingColumns = new List<string>();
                }
                return this._usingColumns;
            }
        }

        // int MaxFileStreem { get; set; }

        #endregion

        #region Методы

        /// <summary>
        /// Получить набор пар <Идентификатор, Тип>
        /// </summary>
        /// <param name="pairsDictionary"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<IBDObjectReference> createRowPairCollection(Dictionary<string, string> pairsDictionary, IRDataReader reader)
        {
            List<IBDObjectReference> wholeList = new List<IBDObjectReference>();
            foreach (KeyValuePair<string, string> pair in pairsDictionary)
            {
                object tp = reader[pair.Value];
                object id = reader[pair.Key];
                Int64 resTp;
                Int64 resId;
                if (Int64.TryParse(tp.ToString(), out resTp) && Int64.TryParse(id.ToString(), out resId))
                {
                    wholeList.Add(new RLightWeightObjectRefernce(resTp, (long)resId));
                }
            }
            return wholeList;
        }

        /// <summary>
        /// Создать и открыть файл для записи
        /// </summary>
        /// <param name="usingColumnNames"></param>
        /// <param name="mode"></param>
        /// <param name="fileEncoding"></param>
        /// <param name="fileName"></param>
        /// <param name="realFileName"></param>
        /// <param name="reader"></param>
        private void MadeHeader(List<RCellValue> usingColumnNames, int mode, Encoding fileEncoding, string fileName,
            string realFileName, IRDataReader reader)
        {
            if (File.Exists(realFileName))
            {
                File.Delete(realFileName);
            }
            REnvironment.Current.Protocol.PutDebugMessage("Начали запись в файл " + realFileName);
            this._generatedFilesList.Add(fileName, realFileName);
            //csv или текст надо создать FileStream и добавить ее в массив
            if (mode == 1 || mode == 3 || mode == 4) 
            {
                //создали новый FileStream
                var fileStreem = new FileStream(realFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                // добавили его в массив
                this._fileStreamDict.Add(fileName, fileStreem);
                //csv надо записать заголовок
                if (mode == 1) 
                {
                    StringBuilder sb = new StringBuilder();
                    CsvWorking.GetCSVHeader(usingColumnNames, sb);
                    fileStreem.Write(fileEncoding.GetBytes(sb.ToString()), 0, sb.Length);
                }
            }
            //dbf
            if (mode == 2) 
            {
                this._dbfxDictionary.Add(fileName, new DBFx(reader.EtalonDataTable, reader.GetSchemaTable(), this.UsingColumns, realFileName));
            }
        }

        /// <summary>
        /// Завершить запись в файл(ы)
        /// </summary>
        private void CloseAll()
        {
            foreach (var ddf in this._dbfxDictionary)
            {
                ddf.Value.Close();
            }
            if (_fileStreamDict != null)
            {
                foreach (var fileStream in _fileStreamDict.Values)
                {
                    fileStream.Close();
                    REnvironment.Current.Protocol.PutDebugMessage("Окончили  запись в файл " + fileStream.Name);
                }
            }
        }

        /// <summary>
        /// Запись строки файла
        /// </summary>
        /// <param name="usingColumnNames"></param>
        /// <param name="fileEncoding"></param>
        /// <param name="filename"></param>
        /// <param name="lineCount"></param>
        /// <param name="reader"></param>
        /// <param name="mode"></param>
        private void WriteTextRow(List<RCellValue> usingColumnNames, Encoding fileEncoding, string filename, 
            ref int lineCount, IRDataReader reader, int mode)
        {
            try
            {
                StringBuilder row = new StringBuilder();
                bool first = true;
                foreach (var collName in usingColumnNames)
                {
                    if(collName.HasError)continue;
                    try
                    {
                        _obj = reader[collName.Name]; //получили значение поля
                    }
                    catch (Exception ex)
                    {
                        REnvironment.Current.Protocol.PutExceptionMessage(
                            new Exception(String.Format("Ошибка чтения   строки из базы {0}   ", collName.Name),ex));
                        collName.HasError = true;
                        continue;
                    }
                    switch (mode)
                    {
                        case 1://CSV добавим значение и ';'
                        case 4:
                            if (!first)
                            {
                                row.Append(';');
                            }
                            if (this._obj != DBNull.Value)
                            {

                                row.Append(CsvWorking.StringToCSVCell(string.Format("{0:"+collName.Format+"}",this._obj) ));
                            }; 
                            break;
                        case 3://SimpleText просто добавим значение
                            if (this._obj != DBNull.Value) 
                                row.Append(_obj); 
                            break;
                    }
                    first = false;
                }
                // получили строку для записи в файл
                if (mode != 2)
                {
                    row.Append(Environment.NewLine);
                    var bytes = fileEncoding.GetBytes(row.ToString());
                    _fileStreamDict[filename].Write(bytes, 0, bytes.Length);//собственно запись
                }
                lineCount++;
            }
            catch (ThreadAbortException eax)
            {
                throw eax;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка записи  строки в  файл{0} {1} ", filename, ex));
            }
        }

        #endregion
    }

    /// <summary>
    /// Расширения файла 
    /// </summary>
    public enum SystemFileExt
    {
        xls = 0,
        csv = 1,
        dbf = 2,
        CSV = 4,
        txt = 3
    }
}




