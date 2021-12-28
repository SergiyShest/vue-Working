using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;
using UBP.Business.Address;
using UBP.Business.Financial;
using UBP.Collection;
using UBP.Common;

namespace UBP.DataExport
{
    /// <summary>
    /// Запись в справочнике SWIFT
    /// </summary>
    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTRecord", AutoAddErrorParam = false)]
    [RSavedClassProperty("p_ID", "U70SWID", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input,
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.InputOutput)]
    [RSavedClassProperty("U70NAME", "p_nU70NAME", "Name")]
    [RSavedClassProperty("TYPE", "", "TypeID")]
    public class RSWIFTDictionaryRecord : RObject
    {
        public RSWIFTDictionaryRecord() : base()
        { }

        public RSWIFTDictionaryRecord(decimal id)
            : base(id)
        { }

        #region Поля

        private string m_Code;
        private string m_OrganizationStructureType;
        private string m_OrganizationCategory;
        private RAddressCollection m_Addresses;
        
        #endregion

        #region Свойства

        /// <summary>
        /// SWIFT-код
        /// </summary>
        [RSavedProperty("U70CODE", "CODE")]
        public string Code
        {
            get
            {
                this.CheckLoad();
                return this.m_Code;
            }
            set
            {
                this.m_Code = value;
            }
        }

        /// <summary>
        /// Тип с точки зрения структуры подразделений
        /// </summary>
        [RSavedProperty("U70BRNC", "BRNC")]
        public string OrganizationStructureType
        {
            get
            {
                this.CheckLoad();
                return this.m_OrganizationStructureType;
            }
            set
            {
                this.m_OrganizationStructureType = value;
            }
        }

        /// <summary>
        /// Тип с точки зрения рода деятельности организации
        /// </summary>
        [RSavedProperty("U70TYPE", "TYPE")]
        public string OrganizationCategory
        {
            get
            {
                this.CheckLoad();
                return this.m_OrganizationCategory;
            }
            set
            {
                this.m_OrganizationCategory = value;
            }
        }
        
        /// <summary>
        /// Адреса
        /// </summary>
        public RAddressCollection Addresses
        {
            get
            {
                if (this.m_Addresses == null)
                {
                    this.m_Addresses = new RAddressCollection(this);
                    this.m_Addresses.Load();
                }
                return this.m_Addresses;
            }
        }

        #endregion

        /// <summary>
        /// Получение записи из справочника по коду
        /// </summary>
        /// <param name="SWIFTCode"></param>
        /// <returns></returns>
        public static RSWIFTDictionaryRecord GetDictionaryRecordBySWIFTCode(string SWIFTCode)
        {
            RGetRecordIDBySWIFTCodeAdapter ad = new RGetRecordIDBySWIFTCodeAdapter(SWIFTCode);
            ad.Execute();
            return (ad.ID == null)? null : new RSWIFTDictionaryRecord((decimal)ad.ID);
        }
    }

    /// <summary>
    /// Сервис получения параметров счета по его номеру и состоянию
    /// </summary>
    [RServiceName("SWIFT_DATA_EXCHANGE.GetRecordIDBySWIFTCode", AutoAddErrorParam = false)]
    public class RGetRecordIDBySWIFTCodeAdapter : RExecServiceAdapter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public RGetRecordIDBySWIFTCodeAdapter(string swiftCode)
            : base()
        {
            this.m_ID = 0;
            this.m_SWIFTCode = swiftCode;
        }

        private string m_SWIFTCode;

        private decimal? m_ID;

        /// <summary>
        /// SWIFT-Код
        /// </summary>               
        [RServiceProperty("p_sSWIFT")]
        public string SWIFTCode
        {
            get
            {
                return this.m_SWIFTCode;
            }
        }

        /// <summary>
        /// Идентификатор 
        /// </summary>  
        [RServiceProperty("p_nID", OutDbType = DbParameterType.Decimal, Direction = System.Data.ParameterDirection.Output)]
        public decimal? ID
        {
            get
            {
                return this.m_ID;
            }
            set
            {
                this.m_ID = value;
            }
        }
    }


    /// <summary>
    /// Конвертер строковых значений в поле комментария для аудита
    /// </summary>
    [ValueConversion(typeof(object), typeof(IEnumerable<RStandardCollectionItem>))]
    public class SWIFTSubtypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string customStr = value.ToString();
            ObservableCollection<RStandardCollectionItem> coll = new ObservableCollection<RStandardCollectionItem>();
            
            RStandardCollection res = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTFinSubType);
            RStandardCollection res2 = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTBEI);
            RStandardCollection res3 = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTOrganizationDepartmensTypes);
            RStandardCollection res4 = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTPaymentSystems);

            if (res.FirstOrDefault(cur => customStr.Contains(cur.StringID)) != null)
            {
                coll.Add(new RStandardCollectionItem(0, "-------Подтип финансовой организации (SWIFT):", "", ""));
            }
            this.addFromDictionary(customStr, res, coll);
            if (res2.FirstOrDefault(cur => customStr.Contains(cur.StringID)) != null)
            {
                coll.Add(new RStandardCollectionItem(0, "-------Идентификационный код нефинансовой организации (BEI):","", ""));
            }
            this.addFromDictionary(customStr, res2, coll);
            if (res3.FirstOrDefault(cur => customStr.Contains(cur.StringID)) != null)
            {
                coll.Add(new RStandardCollectionItem(0, "-------Квалификатор отделений:", "", ""));
            }
            this.addFromDictionary(customStr, res3, coll);
            if (res4.FirstOrDefault(cur => customStr.Contains(cur.StringID)) != null)
            {
                coll.Add(new RStandardCollectionItem(0, "-------Платёжные системы (SWIFT):", "", ""));
            }
            this.addFromDictionary(customStr, res4, coll);

            return coll;
        }

        public void addFromDictionary(string customStr, RStandardCollection itemsCol, ObservableCollection<RStandardCollectionItem> coll)
        {
            foreach (RStandardCollectionItem item in itemsCol)
            {
                if (customStr.Contains(item.StringID))
                {
                    coll.Add(item);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
