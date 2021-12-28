using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using UBP.ViewModel.Core;


namespace UBP.Reports
{
    /// <summary>
    /// Класс для представления одного формата вывода.
    /// Унифицированный для разных программ 3CardR,ExcelTemplate,WordTemplate,CrystalTemplate
    /// </summary>
    public class ExportFormatType
    {
        #region  константа которая передается в программу(ExcelTemplate,WordTemplate,CrystalTemplate)для задания типа вывода

        // константа которая передается в программу(ExcelTemplate,WordTemplate,CrystalTemplate)для задания типа вывода
        int value;
        /// <summary>
        /// константа которая передается в программу(ExcelTemplate,WordTemplate,CrystalTemplate)для задания типа вывода
        /// </summary>
        public int Value { get { return value; } }

        #endregion
        #region  полное имя типа вывода

        string name;
        /// <summary>
        /// полное имя типа вывода 
        /// </summary>
        public string Name { get { return name; } }

        #endregion
        #region  Расширение файла

        string extention;
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extention { get { return extention; } }

        #endregion
        #region  сокращенное название формата как оно применяется в документе

        string shortName;

        /// <summary>
        /// сокращенное название формата как оно применяется в документе 
        /// </summary>
        public string ShortName { get { return shortName; } }

        #endregion
        #region строковый Инеднификатор формата в 3CardR

        string treeCardRID;
        /// <summary>
        /// строковый  Идeнификатор формата в 3CardR
        /// </summary>
        public string TreeCardRId
        {
            get { return treeCardRID; }
        }
        #endregion
        #region  Кодировки используемые при сохранении в файл
        List<Encoding> encodingCollection = new List<Encoding>();
        /// <summary>
        /// Кодировки используемые при сохранении в файл
        /// </summary>
        public List<Encoding> EncodingCollection
        {
            get
            {
                return encodingCollection;
            }
            set
            {
                encodingCollection = value;
            }
        }

        #endregion
        #region числовой идентификатор в 3CardR

        private RAvailableExportFormats _3CardId;
        /// <summary>
        /// числовой идентификатор в 3CardR
        /// </summary>
        public RAvailableExportFormats TreeCardId
        {
            get { return _3CardId; }

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Name"></param>
        /// <param name="Extention"></param>
        /// <param name="ShortName"></param>
        /// <param name="treeCardRID"></param>
        /// <param name="EncodingCollection"></param>
        /// <param name="treeCardId"></param>
        public ExportFormatType(int value, string Name, string Extention, string ShortName, string treeCardRID, List<Encoding> EncodingCollection, RAvailableExportFormats treeCardId)
        {
            this.value = value;
            this.name = Name;
            this.extention = Extention;
            this.shortName = ShortName;
            this.treeCardRID = treeCardRID;
            this.encodingCollection = EncodingCollection;
            this._3CardId = treeCardId;
        }

        public override string ToString()
        {
            return Name;
        }

        public string GetRealPath(string savePath)
        {

            return GetRealPath(savePath, this.Extention);
        }

        public static string GetRealPath(string savePath, string extention)
        {
            string realSavePath = string.Empty;
            if (String.IsNullOrWhiteSpace(Path.GetExtension(savePath)))
            //if (!Regex.IsMatch(SavePath, "." + _ExportFormatType.Extention + "$", RegexOptions.IgnoreCase))
            {
                realSavePath = Path.GetFullPath(savePath + "." + extention);
                //получили полный путь 
            }
            else
            {
                realSavePath = Path.GetFullPath(savePath); //получили полный путь 
            }
            return realSavePath;
        }

    }

    /// <summary>
    /// Коллекция типов 
    /// например:   ExcelExportFormatTypes представляет из себя коллекцию \ExportFormatType\
    /// c  набором форматов допустимых для вывода с помощью  ExcelTemplate
    /// </summary>
    public class ExportFormatTypeCollection : ObservableCollection<ExportFormatType>
    {   //пустой список поддерживаемых кодировок
        internal List<Encoding> EmptyList = new List<Encoding>();

        /// <summary>
        /// поиск по идентификатору 
        /// </summary>
        /// <param name="TreeCardRId"></param>
        /// <returns></returns>
        public ExportFormatType FindBy3CardRId(string TreeCardRId)
        {
            ExportFormatType exportFormatType = null;
            try
            {
                exportFormatType = this.First(p => p.TreeCardRId.ToUpper() == TreeCardRId.ToUpper());
            }
            catch { }
            return exportFormatType;
        }

        /// <summary>
        /// поиск по свойству ShortName
        /// </summary>
        /// <param typeName="TreeCardRId"></param>
        /// <returns></returns>
        public ExportFormatType FindByShortName(string shortName)
        {
            shortName = shortName.ToLower();
            ExportFormatType exportFormatType = null;
            try
            {
                exportFormatType = this.First(p => p.ShortName.ToLower() == shortName);
            }
            catch { }
            return exportFormatType;
        }
    }

    /// <summary>
    /// Коллекция  форматов вывода 3CardR которые можно вывести в через механизм отчетов
    /// </summary>
    public class ThreeCardRExportFormatTypes : ExportFormatTypeCollection
    {
        public ThreeCardRExportFormatTypes()
        {
            List<Encoding> EncodingList = new List<Encoding>();
            EncodingList.Add(Encoding.GetEncoding(1251));
            EncodingList.Add(Encoding.GetEncoding("KOI8-R"));
            EncodingList.Add(Encoding.GetEncoding(866));
            EncodingList.Add(Encoding.ASCII);
            EncodingList.Add(Encoding.UTF8);

            
           this.Add(new ExportFormatType(0, "Excel xlsx", "xlsx", "Excel", "Excel", EmptyList, RAvailableExportFormats.Xlsx));
            this.Add(new ExportFormatType(5, "Excel xls", "xls", "Excel7", "Excel7", EmptyList, RAvailableExportFormats.Xls));
            this.Add(new ExportFormatType(1, "Excel csv", "csv", "ExcelRecord", "CSV", EncodingList, RAvailableExportFormats.Csv));
            this.Add(new ExportFormatType(2, "DataBaseFormat III dbf", "dbf", "dbf", "DBF", EmptyList, RAvailableExportFormats.Dbf3));
            this.Add(new ExportFormatType(3, "SimpleText ", "txt", "txt", "TXT", EncodingList, RAvailableExportFormats.TxtWindows));
            this.Add(new ExportFormatType(4, "Excel csv(dataOnly без заголовков) ", "csv", "ExcelRecord", "CsvDO", EncodingList, RAvailableExportFormats.CsvData));
        }
    }

    /// <summary>
    /// Коллекция  форматов вывода WordTemplate которые можно вывести в через механизм отчетов
    /// </summary>
    public class CrystalExportFormatTypes : ExportFormatTypeCollection
    {
        public CrystalExportFormatTypes()
        {
            //NoFormat = 0,
            this.Add(new ExportFormatType(1, "CrystalReport rpt", "rpt", "CrystalReport", "Crystal", EmptyList, RAvailableExportFormats.Rpt));
            //CrystalReport = 1,

            this.Add(new ExportFormatType(2, "Текст в формате RTF", "rtf", "RichText", "RTF", EmptyList, RAvailableExportFormats.Rtf));
            //RichText = 2,
            this.Add(new ExportFormatType(3, "Word  doc", "doc", "WordForWindows", "Word", EmptyList, RAvailableExportFormats.Doc));
            //WordForWindows = 3,

            ////     Export format of the report is a Microsoft ExcelTemplate file.
            this.Add(new ExportFormatType(4, "Excel XLS", "xls", "Excel", "Excel", EmptyList, RAvailableExportFormats.Xls));
            //ExcelTemplate = 4,
            ////
            //// Summary:
            ////     Export format of the report is a PDF file.
            this.Add(new ExportFormatType(5, "PDF документ  pdf", "pdf", "PortableDocFormat", "PDF", EmptyList, RAvailableExportFormats.Pdf));
            //PortableDocFormat = 5,
            ////
            //// Summary:
            ////     Export format of the report is an HTML 3.2 file.
            //this.Add(new FormatType(6, " WebPage HTML", "HTML", "HTML32"));
            //HTML32 = 6,
            ////
            //// Summary:
            ////     Export format of the report is an HTML 4.0 file.
            this.Add(new ExportFormatType(7, "Web страница  htm", "htm", "HTML40", "HTM", EmptyList, RAvailableExportFormats.Html));
            //HTML40 = 7,
            ////
            //// Summary:
            //   this.Add(new ExportFormatType(8, "Microcoft Excel csv", "csv", "ExcelRecord", "CSV", EmptyList, RAvailableExportFormats.Csv));

            this.Add(new ExportFormatType(9, "txt", "txt", "Text", "txt", EmptyList, RAvailableExportFormats.TxtWindows));
            // this.Add(new ExportFormatType(10, "txt(doc)", "txt(doc)", "Text doc", "txt", EmptyList, RAvailableExportFormats.TxtDOS));
            //  this.Add(new ExportFormatType(9, "ToASCII_File", "ToASCIIFile", "Text", "txt", EmptyList, RAvailableExportFormats.TxtDOS));


        }

        static public CrystalDecisions.Shared.ExportFormatType GetFormat(int Value)
        {
            try
            {
                return (CrystalDecisions.Shared.ExportFormatType)Value;// Enum.Parse(typeof(CrystalDecisions.Shared.ExportFormatType), Value);
            }
            catch { }
            return CrystalDecisions.Shared.ExportFormatType.NoFormat;
        }
    }

    /// <summary>
    /// Коллекция  форматов вывода WordTemplate которые можно вывести в через механизм отчетов
    /// </summary>
    public class GyptaExportFormatTypes : ExportFormatTypeCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public GyptaExportFormatTypes()
        {
            //Number: REPORT_OUTPUTFORMAT_TXT = 1
            //Number: REPORT_OUTPUTFORMAT_RTF = 2
            //Number: REPORT_OUTPUTFORMAT_DOC = 3
            //Number: REPORT_OUTPUTFORMAT_HTML = 4
            //Number: REPORT_OUTPUTFORMAT_XLS = 5
            //Number: REPORT_OUTPUTFORMAT_PDF = 6
            //Number: REPORT_OUTPUTFORMAT_CSV = 7
            //Number: REPORT_OUTPUTFORMAT_DBF = 8
            //Number: REPORT_OUTPUTFORMAT_STAROFFICE = 9

            this.Add(new ExportFormatType(2, "Текст в формате RTF", "rtf", "RichText", "RTF", EmptyList, RAvailableExportFormats.Rtf));
            // this.Add(new ExportFormatType(8, "DataBaseFormat III dbf"   , "dbf", "dbf"          , "DBF", EmptyList, RAvailableExportFormats.Dbf3));
            // this.Add(new ExportFormatType(3, "Microcoft Word  doc", "doc", "WordForWindows", "Word", EmptyList, RAvailableExportFormats.Doc));
            this.Add(new ExportFormatType(1, "TEXT DOC  txt", "TXT", "TXT", "TXT", EmptyList, RAvailableExportFormats.TxtDOS));
            //   this.Add(new ExportFormatType(10, "ToASCIIFile", "ToASCIIFile", "Text", "txt", EmptyList, RAvailableExportFormats,));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        static public CrystalDecisions.Shared.ExportFormatType GetFormat(int val)
        {
            try
            {
                return (CrystalDecisions.Shared.ExportFormatType)val;// Enum.Parse(typeof(CrystalDecisions.Shared.ExportFormatType), Value);
            }
            catch { }
            return CrystalDecisions.Shared.ExportFormatType.NoFormat;
        }
    }

    /// <summary>
    /// Коллекция  форматов вывода WordTemplate которые можно вывести в через механизм отчетов
    /// </summary>
    public class WordExportFormatTypes : ExportFormatTypeCollection
    {
        // Summary:
        ////     Microsoft Word 97 document format.
        //wdFormatDocument97 = 0,
        ////
        //// Summary:
        ////     Microsoft Word format.
        //wdFormatDocument = 0,
        ////
        //// Summary:
        ////     Word 97 template format.
        //wdFormatTemplate97 = 1,
        ////
        //// Summary:
        ////     Microsoft Word template format.
        //wdFormatTemplate = 1,
        ////
        //// Summary:
        ////     Microsoft Windows text format.
        //wdFormatText = 2,
        ////
        //// Summary:
        ////     Microsoft Windows text format with line breaks preserved.
        //wdFormatTextLineBreaks = 3,
        ////
        //// Summary:
        ////     Microsoft DOS text format.
        //wdFormatDOSText = 4,
        ////
        //// Summary:
        ////     Microsoft DOS text with line breaks preserved.
        //wdFormatDOSTextLineBreaks = 5,
        ////
        //// Summary:
        ////     Rich text format (RTF).
        //wdFormatRTF = 6,
        ////
        //// Summary:
        ////     Unicode text format.
        //wdFormatUnicodeText = 7,
        ////
        //// Summary:
        ////     Encoded text format.
        //wdFormatEncodedText = 7,
        ////
        //// Summary:
        ////     Standard HTML format.
        //wdFormatHTML = 8,
        ////
        //// Summary:
        ////     Web archive format.
        //wdFormatWebArchive = 9,
        ////
        //// Summary:
        ////     Filtered HTML format.
        //wdFormatFilteredHTML = 10,
        ////
        //// Summary:
        ////     Extensible Markup Language (XML) format.
        //wdFormatXML = 11,
        ////
        //// Summary:
        ////     XML document format.
        //wdFormatXMLDocument = 12,
        ////
        //// Summary:
        ////     XML template format with macros enabled.
        //wdFormatXMLDocumentMacroEnabled = 13,
        ////
        //// Summary:
        ////     XML template format.
        //wdFormatXMLTemplate = 14,
        ////
        //// Summary:
        ////     XML template format with macros enabled.
        //wdFormatXMLTemplateMacroEnabled = 15,
        ////
        //// Summary:
        ////     Word default document file format. For Microsoft Office Word 2007, this is
        ////     the DOCX format.
        //wdFormatDocumentDefault = 16,
        ////
        //// Summary:
        ////     PDF format.
        //wdFormatPDF = 17,
        ////
        //// Summary:
        ////     XPS format.
        //wdFormatXPS = 18,
        ////
        //wdFormatFlatXML = 19,
        ////
        //wdFormatFlatXMLMacroEnabled = 20,
        ////
        //wdFormatFlatXMLTemplate = 21,
        ////
        //wdFormatFlatXMLTemplateMacroEnabled = 22,
        ////
        //wdFormatOpenDocumentText = 23,


        public WordExportFormatTypes()
        {

            this.Add(new ExportFormatType(0, "Word  doc", "doc", "wdFormatDocument", "WORD", EmptyList, RAvailableExportFormats.Doc));
            this.Add(new ExportFormatType(2, "Текстовый документ(Windows)  txt", "txt", "wdFormatText", "Txt", EmptyList, RAvailableExportFormats.TxtWindows));
            this.Add(new ExportFormatType(4, "Текстовый документ(DOS)  txt", "txt", "wdFormatDOSText", "Txt(DOS)", EmptyList, RAvailableExportFormats.TxtDOS));
            this.Add(new ExportFormatType(6, "Текст в формате RTF  rtf", "rtf", "wdFormatRTF", "RTF", EmptyList, RAvailableExportFormats.Rtf));
            this.Add(new ExportFormatType(8, "Web страница  html", "html", "wdFormatHTML", "HTML", EmptyList, RAvailableExportFormats.Html));
            this.Add(new ExportFormatType(11, "XML документ  xml", "xml", "wdFormatXMLDocument", "XML", EmptyList, RAvailableExportFormats.Xml));
            this.Add(new ExportFormatType(17, "PDF format", "PDF", "wdFormatPDF", "PDF", EmptyList, RAvailableExportFormats.Pdf));
            this.Add(new ExportFormatType(18, "XPS format", "XPS", "wdFormatXPS", "XPS", EmptyList, RAvailableExportFormats.Xps));

        }

        public static WdSaveFormat GetFormat(int Value)
        {
            return (WdSaveFormat)Value;

        }


    }


    /// <summary>
    /// Коллекция  форматов вывода WordTemplate которые можно вывести в через механизм отчетов
    /// </summary>
    public class OpenOfficeExportFormatTypes : ExportFormatTypeCollection
    {


        /// <summary>
        /// типы которые поо
        /// </summary>
        public OpenOfficeExportFormatTypes()
        {

            this.Add(new ExportFormatType(0, "OpenOffice odt", "odt", "odt", "odt", EmptyList, RAvailableExportFormats.OpenOffice));
            this.Add(new ExportFormatType(1, "Word  doc", "doc", "wdFormatDocument", "WORD", EmptyList, RAvailableExportFormats.Doc));
            this.Add(new ExportFormatType(2, "Текстовый документ(Windows)  txt", "txt", "wdFormatText", "Txt", EmptyList, RAvailableExportFormats.TxtWindows));
            this.Add(new ExportFormatType(4, "Текстовый документ(DOS)  txt", "txt", "wdFormatDOSText", "Txt(DOS)", EmptyList, RAvailableExportFormats.TxtDOS));
            this.Add(new ExportFormatType(6, "Текст в формате RTF  rtf", "rtf", "wdFormatRTF", "RTF", EmptyList, RAvailableExportFormats.Rtf));
            this.Add(new ExportFormatType(8, "Web страница  html", "html", "wdFormatHTML", "HTML", EmptyList, RAvailableExportFormats.Html));
            this.Add(new ExportFormatType(11, "XML документ  xml", "xml", "wdFormatXMLDocument", "XML", EmptyList, RAvailableExportFormats.Xml));
            this.Add(new ExportFormatType(17, "PDF format", "PDF", "wdFormatPDF", "PDF", EmptyList, RAvailableExportFormats.Pdf));

        }

        //public static WdSaveFormat GetFormat(int Value)
        //{
        //    return (WdSaveFormat)Value;
        //}

    }

    /// <summary>
    /// Коллекция  форматов вывода ExcelTemplate которые можно вывести в через механизм отчетов
    /// 1 - стандартный вывод в Ехсел
    /// 2 - CSV 
    /// 3 - II  DBF
    /// 4 - III  DBF
    /// 5 - IV  DBF
    /// 6 - Html
    /// </summary>
    public class ExcelExportFormatTypes : ExportFormatTypeCollection
    {
        public List<Encoding> EncodingList = new List<Encoding>();

        public ExcelExportFormatTypes()
        {
           this.Add(new ExportFormatType(51, "Excel ", "xlsx", "xlWorkbookNormal", "Excel", EmptyList, RAvailableExportFormats.Xls));
           // this.Add(new ExportFormatType(-4143, "Excel XLS", "XLS", "xlWorkbookNormal", "Excel", EmptyList, RAvailableExportFormats.Xls));
            // if (WB.Application.Version.Contains("11.")) //старые версии не работают со строками более 65 тысяч
            {
                this.Add(new ExportFormatType((int)Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, "Excel 7",
                    "xls", "xlExcel7", "Excel", EmptyList, RAvailableExportFormats.Xls));
            }
            this.Add(new ExportFormatType(6, "Формат CSV", "CSV", "xlCSV", "CSV", EncodingList, RAvailableExportFormats.Csv));
            this.Add(new ExportFormatType(24, "Формат CSV(DOC)", "CSV", "xlCSVMSDOS", "CSV(DOC)", EncodingList, RAvailableExportFormats.CsvDoc));
            //  this.Add(new ExportFormatType(7, "Dbase II  DBF", "DBF", "xlDBF3", "DBFIII", EmptyList, RAvailableExportFormats.Dbf3));
            //  this.Add(new ExportFormatType(8, "Dbase IV  DBF", "DBF", "xlDBF4", "DBFIV", EmptyList, RAvailableExportFormats.Dbf4));
            this.Add(new ExportFormatType(44, "Web Страница  Html", "Html", "xlHtml", "HTML", EmptyList, RAvailableExportFormats.Html));

        }

        public static Microsoft.Office.Interop.Excel.XlFileFormat GetFormat(int Value)
        {
            return (Microsoft.Office.Interop.Excel.XlFileFormat)Value;
        }
        /// <summary>
        /// родные форматы ExcelTemplate  как они выгрузились из метаданных (для справки)
        /// </summary>
        // enum XlFileFormat
        //{
        //    xlCurrentPlatformText = -4158,
        //    xlWorkbookNormal = -4143,
        //    xlSYLK = 2,
        //    xlWKS = 4,
        //    xlWK1 = 5,
        //    xlCSV = 6,
        //    xlDBF2 = 7,
        //    xlDBF3 = 8,
        //    xlDIF = 9,
        //    xlDBF4 = 11,
        //    xlWJ2WD1 = 14,
        //    xlWK3 = 15,
        //    xlExcel2 = 16,
        //    xlTemplate = 17,
        //    xlAddIn = 18,
        //    xlTextMac = 19,
        //    xlTextWindows = 20,
        //    xlTextMSDOS = 21,
        //    xlCSVMac = 22,
        //    xlCSVWindows = 23,
        //    xlCSVMSDOS = 24,
        //    xlIntlMacro = 25,
        //    xlIntlAddIn = 26,
        //    xlExcel2FarEast = 27,
        //    xlWorks2FarEast = 28,
        //    xlExcel3 = 29,
        //    xlWK1FMT = 30,
        //    xlWK1ALL = 31,
        //    xlWK3FM3 = 32,
        //    xlExcel4 = 33,
        //    xlWQ1 = 34,
        //    xlExcel4Workbook = 35,
        //    xlTextPrinter = 36,
        //    xlWK4 = 38,
        //    xlExcel7 = 39,
        //    xlExcel5 = 39,
        //    xlWJ3 = 40,
        //    xlWJ3FJ3 = 41,
        //    xlUnicodeText = 42,
        //    xlExcel9795 = 43,
        //    xlHtml = 44,
        //    xlWebArchive = 45,
        //    xlXMLSpreadsheet = 46,
        //}

    }

}
