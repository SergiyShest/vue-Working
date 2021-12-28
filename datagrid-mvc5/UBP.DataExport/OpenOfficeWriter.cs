using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using uno;
using unoidl.com.sun.star.awt;
using unoidl.com.sun.star.beans;
using unoidl.com.sun.star.container;
using unoidl.com.sun.star.document;
using unoidl.com.sun.star.frame;
using unoidl.com.sun.star.lang;
using unoidl.com.sun.star.style;
using unoidl.com.sun.star.text;
using unoidl.com.sun.star.uno;
using unoidl.com.sun.star.view;
using Exception = System.Exception;

namespace UBP.DataExport
{
    public static class OpenOfficeWriter
    {

        public static XComponent NewFile(bool visible)
        {
            return OpenFileInner(@"private:factory/swriter", visible);

        }

        public static XComponent OpenFile(string fileName, bool visible)
        {
            return OpenFileInner(PathConverter(fileName), visible);
        }
        private static XComponent OpenFileInner(string fileName, bool visible)
        {

            XMultiServiceFactory multiServiceFactory = GetServiceFactory();
            XComponentLoader componentLoader = (XComponentLoader)multiServiceFactory.createInstance("com.sun.star.frame.Desktop");


            var pv = new PropertyValue();
            pv.Name = "Hidden";
            pv.Value = new uno.Any(!visible);
            PropertyValue[] pvArr = new PropertyValue[1];
            pvArr[0] = pv;
            XComponent xComponent = componentLoader.loadComponentFromURL(fileName, "_blank", 0,
                pvArr);

            XDocumentInsertable ins = xComponent as XDocumentInsertable;
            // ins.insertDocumentFromURL(PathConverter(fileName), new PropertyValue[0]); 
            return xComponent;
        }

        private static XMultiServiceFactory GetServiceFactory()
        {
            //Call the bootstrap method to get a new ComponentContext  
            //object. If OpenOffice isn't already started this will
            //start it and then return the ComponentContext.
            XComponentContext localContext = uno.util.Bootstrap.bootstrap();

            //Get a new service manager of the MultiServiceFactory type  
            //we need this to get a desktop object and create new CLI  
            //objects.  
            XMultiServiceFactory multiServiceFactory = (XMultiServiceFactory)localContext.getServiceManager();


            return multiServiceFactory;

        }

        ///  
        /// Convert into OO file format  
        ///  
        private static string PathConverter(string file)
        {
            try
            {
                file = file.Replace(@"\", "/");
                return "file:///" + file;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsOOoInstalled()
        {
            try
            {
            GetServiceFactory();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
            //string baseKey;
            //// Для 64 битной версии
            //if (Marshal.SizeOf(typeof(IntPtr)) == 8) baseKey = @"SOFTWARE\Wow6432Node\OpenOffice.org\";
            //else
            //    baseKey = @"SOFTWARE\OpenOffice.org\";
            //string key = baseKey + @"Layers\URE\1";
            //RegistryKey reg = Registry.CurrentUser.OpenSubKey(key);
            //if (reg == null)
            //{
            //    reg = Registry.LocalMachine.OpenSubKey(key);
            //}
            //string urePath = reg.GetValue("UREINSTALLLOCATION") as string;
            //reg.Close();
            //if (urePath != null) return true;
            //else
            //    return false;
        }

        public static void AddDocument(this XComponent xComponent, string fileName)
        {
            var xTextDocucment = ((XTextDocument)xComponent);
            var xText = xTextDocucment.getText();
            var xTextCursor = xText.createTextCursor();

            xTextCursor.gotoEnd(false);

            var xPropSet = (XPropertySet)xTextCursor;
            xPropSet.setPropertyValue("BreakType", new Any((int)BreakType.PAGE_BEFORE));

            ((XDocumentInsertable)xTextCursor).insertDocumentFromURL(PathConverter(fileName), new PropertyValue[0]);

        }


        //public static void Hide()
        //{
        //    XMultiServiceFactory multiServiceFactory = GetServiceFactory();
        //    var oFrame = multiServiceFactory.
        //    //  XWindow oWindow = oFrame.getContainerWindow();
        //    // oWindow.setVisible(false);

        //}


        public static void SaveAsAndClose(this XComponent xComponent, string fileName, int typeContent, string encoding)
        {
            //0, "Microcoft Word  doc", "doc", "wdFormatDocument", "WORD", EmptyList, RAvailableExportFormats.Doc));
            //1, "Текстовый документ(Windows)  txt", "txt", "wdFormatText", "Txt", EmptyList, RAvailableExportFormats.TxtWindows));
            //4, "Текстовый документ(DOS)  txt", "txt", "wdFormatDOSText", "Txt(DOS)", EmptyList, RAvailableExportFormats.TxtDOS));
            //6, "Текст в формате RTF  rtf", "rtf", "wdFormatRTF", "RTF", EmptyList, RAvailableExportFormats.Rtf));
            //8, "Web страница  html", "html", "wdFormatHTML", "HTML", EmptyList, RAvailableExportFormats.Html));
            //11, "XML документ  xml", "xml", "wdFormatXMLDocument", "XML", EmptyList, RAvailableExportFormats.Xml));

            string typ = null;

            switch (typeContent)
            {

                case 0: typ = "MS Word 97";
                    break;
                case 1: typ = "Text";
                    break;
                case 4: typ = "Text";
                    encoding = "866";
                    break;
                case 8: typ = "HTML";
                 break;
                 case 17:
                     typ = "writer_pdf_Export";
                    break;
                default: throw new NotImplementedException("пока не поддерживается");

            }
            SaveAsAndClose(xComponent, fileName, typ, encoding);
        }

        public static void SaveAsAndClose(this XComponent xComponent, string fileName)
        {
            string typ = null;

            SaveAsAndClose(xComponent, fileName, typ, null);
        }

        public static void SaveAsAndClose(this XComponent xComponent, string fileName, string typ, string encoding)
        {
            List<PropertyValue> prop = new List<PropertyValue>();
            if (!String.IsNullOrWhiteSpace(typ))
            {
                prop.Add(new PropertyValue("FilterName", 0, new Any(typ), PropertyState.DIRECT_VALUE));
            }
            if (!String.IsNullOrWhiteSpace(encoding))
            {
                prop.Add(new PropertyValue() { Name = "CharacterSet", Value = new Any(encoding) }); ;
            }
            if (typ == "writer_pdf_Export")
            {
                 ((XStorable)xComponent).storeToURL(PathConverter(fileName), prop.ToArray());
            }
            else
            {
                 ((XStorable)xComponent).storeAsURL(PathConverter(fileName), prop.ToArray());
            }
           
            xComponent.dispose();

        }
        public static void PrintDocumentOnTray(this  XComponent xComp, int copyCount)
        {
            PrinterSettings settings = new PrinterSettings();
            PrintDocumentOnTray(xComp, settings.PrinterName, null, null, copyCount, null, null);
        }

        public static void PrintDocumentOnTray(this XComponent xComp, String printerName, int copyCount)
        {
            PrintDocumentOnTray(xComp, printerName, null, null, copyCount, null, null);
        }


        public static void PrintDocumentOnTray(this XComponent xComp, String printerName, String paperTray, String pageRange, int copyCount, String collate, String sort)
        {

            PropertyValue[] printerProp = new PropertyValue[1];
            printerProp[0] = new PropertyValue();
            printerProp[0].Name = "Name";
            printerProp[0].Value = new Any(printerName);

            XPrintable xPrintable = (XPrintable)xComp;

            //set the printer driver and return the printer settings
            xPrintable.setPrinter(printerProp);
            //printerProp = xPrintable.getPrinter();
            //XPrintJobBroadcaster selection = (XPrintJobBroadcaster)xPrintable;
            //MyXPrintJobListener xPrintJobListener = new MyXPrintJobListener();
            //selection.addPrintJobListener(xPrintJobListener);

            XTextDocument xTextDocument = (XTextDocument)xComp;

            XText xText = xTextDocument.getText();
            /* Manipulate the text .. basically add the text */

            // create a text cursor from the cells XText interface
            XTextCursor xTextCursor = xText.createTextCursor();

            // Get the property set of the cell's TextCursor
            XPropertySet xTextCursorProps = (XPropertySet)xTextCursor;

            uno.Any pageStyleName = xTextCursorProps.getPropertyValue("PageStyleName");

            // Get the StyleFamiliesSupplier interface of the document
            XStyleFamiliesSupplier xSupplier = (XStyleFamiliesSupplier)xTextDocument;

            // Use the StyleFamiliesSupplier interface to get the XNameAccess interface of the
            // actual style families
            XNameAccess xFamilies = xSupplier.getStyleFamilies();

            // Access the 'PageStyles' Family
            XNameContainer xFamily = (XNameContainer)xFamilies.getByName("PageStyles").Value;

            XStyle xStyle = (XStyle)xFamily.getByName((String)pageStyleName.Value).Value;

            // Get the property set of the cell's TextCursor
            // XPropertySet xStyleProps = (XPropertySet)xStyle;

            // Just set Tray into PageStyleSetting ...
            // xStyleProps.setPropertyValue("PrinterPaperTray", new uno.Any(paperTray));

            List<PropertyValue> printOps = new List<PropertyValue>();

            if (pageRange != null)
            {

                PropertyValue pv1 = new PropertyValue();
                pv1.Name = "Pages";
                pv1.Value = new uno.Any(pageRange);
                printOps.Add(pv1);
            }
            if (copyCount != null)
            {

                PropertyValue pv2 = new PropertyValue();
                pv2.Name = "CopyCount";
                pv2.Value = new uno.Any(copyCount);
                printOps.Add(pv2);
            }
            if (collate != null)
            {
                PropertyValue pv3 = new PropertyValue();
                pv3.Name = "Collate";
                pv3.Value = new uno.Any(collate);
                printOps.Add(pv3);
            }
            if (sort != null)
            {

                PropertyValue pv4 = new PropertyValue();
                pv4.Name = "Sort";
                pv4.Value = new uno.Any(sort);
                printOps.Add(pv4);
            }
            if (sort != null)
            {

                PropertyValue pv4 = new PropertyValue();
                pv4.Name = "Sort";
                pv4.Value = new uno.Any(sort);
                printOps.Add(pv4);
            }

            PropertyValue pv5 = new PropertyValue();
            pv5.Name = "Wait";
            pv5.Value = new uno.Any(true);
            printOps.Add(pv5);

            xPrintable.print(printOps.ToArray());
            //  return xPrintJobListener;

            //while (xPrintJobListener.getStatus() != PrintableState.JOB_COMPLETED)
            //{
            //    Thread.Sleep(1000);
            //}
            xComp.dispose();
            // selection.removePrintJobListener(xPrintJobListener);
        }




    }


    ///// <summary>
    ///// при попытеке печатать через сетевой принтер вылетает ошибка
    ///// </summary>
    //public class MyXPrintJobListener : XPrintJobListener
    //{
    //    public EventHandler JobEvent;


    //    private PrintableState status;

    //    public PrintableState getStatus()
    //    {
    //        return status;
    //    }

    //    public void setStatus(PrintableState status)
    //    {
    //        this.status = status;
    //    }

    //    /**
    //    * The print job event: has to be called when the action is triggered.
    //    */
    //    public void printJobEvent(PrintJobEvent printJobEvent)
    //    {
    //        if (printJobEvent.State == PrintableState.JOB_COMPLETED)
    //        {
    //            this.setStatus(PrintableState.JOB_COMPLETED);
    //            return;
    //        }
    //        if (printJobEvent.State == PrintableState.JOB_ABORTED)
    //        {
    //            this.setStatus(PrintableState.JOB_ABORTED);
    //            return;
    //        }
    //        if (printJobEvent.State == PrintableState.JOB_FAILED)
    //        {
    //            this.setStatus(PrintableState.JOB_FAILED);
    //            return;
    //        }
    //        if (printJobEvent.State == PrintableState.JOB_SPOOLED)
    //        {
    //            this.setStatus(PrintableState.JOB_SPOOLED);
    //            return;
    //        }
    //        if (printJobEvent.State == PrintableState.JOB_SPOOLING_FAILED)
    //        {
    //            this.setStatus(PrintableState.JOB_SPOOLING_FAILED);
    //            return;
    //        }
    //        if (printJobEvent.State == PrintableState.JOB_STARTED)
    //        {
    //            this.setStatus(PrintableState.JOB_STARTED);
    //            return;
    //        }

    //        if (JobEvent != null)
    //        {
    //            JobEvent(this, EventArgs.Empty);
    //        }
    //    }

    //    /**
    //    * Disposing event: ignore.
    //    */

    //    public void disposing(unoidl.com.sun.star.lang.EventObject eventObject)
    //    {

    //    }

    //    #region XPrintJobListener Members

    //    void XPrintJobListener.printJobEvent(PrintJobEvent Event)
    //    {

    //    }

    //    #endregion


    //}
}
