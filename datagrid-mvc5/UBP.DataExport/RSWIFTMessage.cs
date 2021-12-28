using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using UBP.Business.BankTopology;
using UBP.BusinessFactory;
using UBP.Collection;
using UBP.Common;
using UBP.Core;
using UBP.Core.OperationEngine;
using UBP.ViewModel.Core;
using unoidl.com.sun.star.animations;
using unoidl.com.sun.star.awt;
using unoidl.com.sun.star.bridge.oleautomation;
using UBP.Business.Core;
using UBP.Business.Money;
using UBP.Business.Payment;

namespace UBP.DataExport
{
    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTMessageMT", AutoAddErrorParam = false)]
    [RSavedProcedure("SWIFT_DATA_EXCHANGE.SaveSWIFTMessageMT", AutoAddErrorParam = true)]
    [RDeleteProcedure("SWIFT_DATA_EXCHANGE.DeleteSWIFTMessageMT", AutoAddErrorParam = true)]
    [RSavedClassProperty("B98RND2", "", "ID")]
    [RSavedClassProperty("p_ID", "p_ID", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input,
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.InputOutput)]
    [RSavedClassProperty("B98RCID", "RCID", "Name")]
    [RSavedClassProperty("", "bNew", "New",
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.Input)]
    public class RSWIFTMessageMT : RObject
    {
        public RSWIFTMessageMT() : base()
        { }

        public RSWIFTMessageMT(decimal id)
            : base(id)
        { }

        #region Поля

        private decimal m_DocumentID;
        private decimal m_CorrespondentAccountID;
        private decimal m_SendFileID;
        private decimal m_ReceiveFileID;
        private decimal m_ParentFileID;
        private decimal m_NumberOnDate;
        private DateTime m_CreateDate;
        private decimal m_FileEntryID;
        private decimal m_StateID;
        private decimal? m_ExportOPID;
        private string m_ErrorsListString;
        private string m_TypeSymbol;

        private IUser m_ExportUser;
        private RSWIFTMessageFieldsCollection m_FieldsCollection;

        private RSWIFTMessagesMTCollection m_ChildMessages;

        protected DateTime m_OperDayDate;

        protected RSWIFTMessageMT m_ParentMessage;
        protected IBaseRemittance m_Document;

        #endregion

        #region Свойства

        /// <summary>
        /// Ссылка на первичный документ
        /// </summary>
        [RSavedProperty("B98RNID", "RNID")]
        public decimal DocumentID
        {
            get
            {
                this.CheckLoad();
                return this.m_DocumentID;
            }
            set
            {
                this.m_DocumentID = value;
            }
        }

        /// <summary>
        /// Ссылка на корреспондентский счет
        /// </summary>
        [RSavedProperty("B98IIDC", "IIDC")]
        public decimal CorrespondentAccountID
        {
            get
            {
                this.CheckLoad();
                return this.m_CorrespondentAccountID;
            }
            set
            {
                this.m_CorrespondentAccountID = value;
            }
        }

        /// <summary>
        /// Ссылка на сформированный файл
        /// </summary>
        [RSavedProperty("B98FSID", "FSID")]
        public decimal SendFileID
        {
            get
            {
                this.CheckLoad();
                return this.m_SendFileID;
            }
            set
            {
                this.m_SendFileID = value;
            }
        }

        /// <summary>
        /// Ссылка на принятый файл
        /// </summary>
        [RSavedProperty("B98FRID", "FRID")]
        public decimal ReceiveFileID
        {
            get
            {
                this.CheckLoad();
                return this.m_ReceiveFileID;
            }
            set
            {
                this.m_ReceiveFileID = value;
            }
        }

        /// <summary>
        /// Родительский файл
        /// </summary>
        [RSavedProperty("B98ROID", "ROID")]
        public decimal ParentFileID
        {
            get
            {
                this.CheckLoad();
                return this.m_ParentFileID;
            }
            set
            {
                this.m_ParentFileID = value;
            }
        }

        /// <summary>
        /// Порядковый номер сообщения за день
        /// </summary>
        [RSavedProperty("B98P011", "")]
        public decimal NumberOnDate
        {
            get
            {
                this.CheckLoad();
                return this.m_NumberOnDate;
            }
            set
            {
                this.m_NumberOnDate = value;
            }
        }

        /// <summary>
        /// Дата создания сообщения
        /// </summary>
        [RSavedProperty("B98P012", "")]
        public DateTime CreateDate
        {
            get
            {
                this.CheckLoad();
                return this.m_CreateDate;
            }
            set
            {
                this.m_CreateDate = value;
            }
        }

        /// <summary>
        /// Запись с содержимым файла
        /// </summary>
        [RSavedProperty("B98RND3", "RND3")]
        public decimal FileEntryID
        {
            get
            {
                this.CheckLoad();
                return this.m_FileEntryID;
            }
            set
            {
                this.m_FileEntryID = value;
            }
        }

        /// <summary>
        /// Состояние сообщения
        /// </summary>
        [RSavedProperty("B98SCDE", "SCDE")]
        private decimal StateID
        {
            get
            {
                this.CheckLoad();
                return this.m_StateID;
            }
            set
            {
                this.m_StateID = value;
            }
        }

        /// <summary>
        /// Состояние сообщения
        /// </summary>
        public SWIFTMTStatuses Status
        {
            get
            {
                return (SWIFTMTStatuses)this.StateID;
            }
            set
            {
                this.StateID = (decimal)value;
            }
        }

        /// <summary>
        /// Идентификатор операциониста, выгрузившего сообщение
        /// </summary>
        [RSavedProperty("B98OPRI", "OPRI")]
        public decimal? ExportOPID
        {
            get
            {
                this.CheckLoad();
                return this.m_ExportOPID;
            }
            set
            {
                this.m_ExportOPID = value;
            }
        }

        /// <summary>
        /// Список полей-ошибок по результатам проверки правилами
        /// </summary>
        [RSavedProperty("B98INFO", "INFO")]
        public string ErrorsListString
        {
            get
            {
                this.CheckLoad();
                return this.m_ErrorsListString;
            }
            set
            {
                this.m_ErrorsListString = value;
            }
        }

        /// <summary>
        /// Тип файла
        /// </summary>
        [RSavedProperty("B98FLTP", "FLTP")]
        public string TypeSymbol
        {
            get
            {
                this.CheckLoad();
                return this.m_TypeSymbol;
            }
            set
            {
                this.m_TypeSymbol = value;
            }
        }

        ///// <summary>
        ///// Пользователь выгрузивший сообщение по SWIFT
        ///// </summary>
        //public IUser ExportUser
        //{
        //    get
        //    {
        //        this.CheckLoad();
        //        if (this.m_ExportUser == null && this.ExportEMID != null)
        //        {
        //            this.m_ExportUser = RBusinessObjectFactory.Current.GetObject(this.ExportEMID, typeof(IUser)) as IUser;
        //        }
        //        return this.m_ExportUser;
        //    }
        //    set
        //    {
        //        this.m_ExportUser = value;
        //        this.ExportEMID = this.m_ExportUser != null ? this.m_ExportUser.ID : (decimal?)null;
        //        this.RaisePropertyChanged(() => ExportUser);
        //    }
        //}

        /// <summary>
        /// Коллекция полей сообщения
        /// </summary>
        [RLinkCollectionProperty("ID", "ID")]
        public RSWIFTMessageFieldsCollection FieldsCollection
        {
            get
            {
                if (this.m_FieldsCollection == null)
                {
                    this.CheckLoad();
                    this.m_FieldsCollection =
                          (RSWIFTMessageFieldsCollection)this.StdGet(this.m_FieldsCollection, "FieldsCollection");
                }
                return this.m_FieldsCollection;
            }
            set
            {
                this.m_FieldsCollection = value;
            }
        }

        /// <summary>
        /// Коллекция дочерних сообщений
        /// </summary>
        [RLinkCollectionProperty("ParentID", "ID")]
        public RSWIFTMessagesMTCollection ChildMessages
        {
            get
            {
                if (this.m_ChildMessages == null)
                {
                    this.CheckLoad();
                    this.m_ChildMessages =
                          (RSWIFTMessagesMTCollection)this.StdGet(this.m_ChildMessages, "ChildMessages");
                }
                return this.m_ChildMessages;
            }
            set
            {
                this.m_ChildMessages = value;
            }
        }

        /// <summary>
        /// Родительское сообщение
        /// </summary>
        [RSavedProperty("B98ROID", "", "ID")]
        public RSWIFTMessageMT ParentMessage
        {
            get
            {
                this.CheckLoad();
                this.m_ParentMessage =
                    (RSWIFTMessageMT)this.StdGet(m_ParentMessage, "ParentMessage");

                return this.m_ParentMessage;
            }
            set
            {
                this.m_ParentMessage = value;
                
            }
        }

        /// <summary>
        /// Референс
        /// </summary>
        public string Reference
        {
            get
            {
                RSWIFTMessageField fieldref = this.FieldsCollection.FirstOrDefault(cur => (cur.TypicalFieldType == 0 ||
                    cur.TypicalFieldType == (decimal)RObjectStandardTypes.RSWIFTMessageTypicalField) && cur.TypicalField.Code == "20");
                return (fieldref != null) ? fieldref.Value : String.Empty;
            }
        }

        /// <summary>
        /// Связанный референс
        /// </summary>
        public string LinkedReference
        {
            get
            {
                RSWIFTMessageField fieldref = this.FieldsCollection.FirstOrDefault(cur => (cur.TypicalFieldType == 0 ||
                    cur.TypicalFieldType == (decimal)RObjectStandardTypes.RSWIFTMessageTypicalField) && cur.TypicalField.Code == "21");
                return (fieldref != null) ? fieldref.Value : String.Empty;
            }
        }

        /// <summary>
        /// Платежный документ
        /// </summary>
        public IBaseRemittance Document
        {
            get
            {
                if (this.m_Document == null)
                {
                    this.m_Document = RBusinessObjectFactory.Current.GetObject(this.DocumentID, RObjectStandardTypes.RSWIFTOperation) as IBaseRemittance;
                }
                return this.m_Document;
            }
        }

        #endregion

        public static RSWIFTMessageMT GetSWIFTMessageBySourceDocument(decimal seid)
        {
            GetSWIFTMT103ByRemittanceAdapter ad = new GetSWIFTMT103ByRemittanceAdapter(seid);
            RSWIFTMessageMT message = new RSWIFTMessageMT();
            message.Load(ad.Read(REnvironment.Current.SaveProviderPool.GetProvider()));
            return message;
        }

        /// <summary>
        /// Запуск проверки правил SWIFT
        /// </summary>
        public static void checkSWIFTRules(RSWIFTMessageMT mes, IRSaveProvider prov, IScriptState scriptState)
        {
            RCheckRulesForMessageInTypicalPhaseAdapter ad2 = new RCheckRulesForMessageInTypicalPhaseAdapter(scriptState.PHTP, mes.ID);
            try
            {
                ad2.Execute(prov);
            }
            catch (RGeneralException gex)
            {
                throw new RGeneralException("Ошибки при проверке правил SWIFT", gex);
            }
            if (ad2.RulesErrorsCollection.Count > 0)
            {
                RErrorCollection errColl = new RErrorCollection();
                for (int i = 0; i < ad2.RulesErrorsCollection.Count; i++)
                {
                    RStandardCollectionItem item = ad2.RulesErrorsCollection[i];
                    RSWIFTRule rule = ad2.RulesCollection[i];
                    string fieldsStr = String.Empty;
                    foreach (RSWIFTMessageTypicalField field in rule.TypicalFields)
                    {
                        fieldsStr += field.Name + " - " + field.Code + "(" + field.AvailableOptions + "). ";
                    }
                    errColl.Add(new RError("Правило: " + rule.Name + ", Ошибка: " + item.StringID + ", Поля: " + fieldsStr,
                        (item.FullName != String.Empty) ? item.FullName : rule.Description));
                }
                throw new RGeneralException("Ошибки при проверке правил SWIFT", errColl);
            }
        }

        public static void checkSWIFTFieldsOnPhaseOnExistence(RSWIFTMessageMT mes, IRSaveProvider prov, IScriptState scriptState, string modeStringMask)
        {
            // проверка на заполненность всех полей и на наличие обязательных полей
            try
            {
                // режим 1111 - полная проверка
                RCheckAllSWIFTMessageFieldsOnPhaseOnExistence ad2 = 
                    new RCheckAllSWIFTMessageFieldsOnPhaseOnExistence(scriptState.PHID, mes.ID, modeStringMask);
                ad2.Execute(prov);
            }
            catch (RGeneralException gex)
            {
                throw new RGeneralException("Ошибка проверки сформированных полей сообщения.", gex);
            }
        }
    }

    [RReadCollectionAttribute("SWIFT_DATA_EXCHANGE.GetSWIFTMessagesByParent", ColID = "B98RND2", ThisOnly = true)]
    public class RSWIFTMessagesMTCollection : RObjectCollection<RSWIFTMessageMT>
    {
        public RSWIFTMessagesMTCollection():
            base()
        {}

        public RSWIFTMessagesMTCollection(decimal id) :
            base()
        {
            this.m_ParentID = id;

        }

        private decimal m_ParentID;

        [RServiceProperty("p_ParentID", Direction = ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal ParentID
        {
            get
            {
                return this.m_ParentID;
            }
            set
            {
                this.m_ParentID = value;
            }
        }
    }

    /// <summary>
    /// Сообщение Mt103 - Однократный клиентский перевод
    /// </summary>
    public class RSWIFTMessageMT103 : RSWIFTMessageMT
    {
        public RSWIFTMessageMT103()
            : base()
        {

        }
    }

    /// <summary>
    /// Сообщение MT202 - ОБщий межбанковский перевод
    /// </summary>
    public class RSWIFTMessageMT202 : RSWIFTMessageMT
    {
        public RSWIFTMessageMT202() : base()
        {
            
        }
    }

    [RServiceName("SWIFT_DATA_EXCHANGE.CreateFieldByMesAndOption", AutoAddErrorParam = true)]
    public class RCreateFieldByMessageAndOptionAdapter : RExecServiceAdapter
    {
        ///// <summary>
        ///// Конструктор
        ///// </summary>
        //public RCreateFieldByMessageAndOptionAdapter(decimal messageID,string typicalFieldCode, string option)
        //    : this(messageID)
        //{
        //    this.m_TypicalFieldCode = typicalFieldCode;
        //    this.m_Option = option;
        //}

        /// <summary>
        /// Конструктор
        /// </summary>
        public RCreateFieldByMessageAndOptionAdapter(decimal messageID, decimal? phid, decimal? seid, decimal? opid)
            : base()
        {
            this.m_MessageID = messageID;
            this.m_TypicalFieldCode = String.Empty;
            this.m_Option = String.Empty;
            this.m_NewFieldID = 0;
            this.m_PHID = phid;
            this.m_SEID = seid;
            this.m_OPID = opid;
        }

        #region Поля

        private decimal m_MessageID;
        private string m_Option;
        private string m_TypicalFieldCode;

        private decimal m_NewFieldID;
        private decimal? m_PHID;
        private decimal? m_SEID;
        private decimal? m_OPID;

        private decimal? m_SequenceNumber;
        private decimal m_State;

        #endregion

        #region Свойства

        /// <summary>
        /// Идентификатор поля сообщения 
        /// </summary>  
        [RServiceProperty("p_nNewFieldID", OutDbType = DbParameterType.Decimal, Direction = System.Data.ParameterDirection.Output)]
        public decimal NewFieldID
        {
            get
            {
                return this.m_NewFieldID;
            }
            set
            {
                this.m_NewFieldID = value;
            }
        }

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        [RServiceProperty("p_MessageID", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal MessageID
        {
            get
            {
                return this.m_MessageID;
            }
            set
            {
                this.m_MessageID = value;
            }
        }

        /// <summary>
        /// Опция
        /// </summary>
        [RServiceProperty("p_Option", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Varchar2)]
        public string Option
        {
            get
            {
                return this.m_Option;
            }
            set
            {
                this.m_Option = value;
            }
        }

        /// <summary>
        /// Код поля сообщения
        /// </summary>
        [RServiceProperty("p_TypicalFieldCode", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Varchar2)]
        public string TypicalFieldCode
        {
            get
            {
                return this.m_TypicalFieldCode;
            }
            set
            {
                this.m_TypicalFieldCode = value;
            }
        }

        /// <summary>
        /// Порядковый номер этапа
        /// </summary>
        [RServiceProperty("p_PHID", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        private decimal? PHID
        {
            get
            {
                return this.m_PHID;
            }
            set
            {
                this.m_PHID = value;
            }
        }

        /// <summary>
        /// Идентификатор операционного документа
        /// </summary>
        [RServiceProperty("p_SEID", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        private decimal? SEID
        {
            get
            {
                return this.m_SEID;
            }
            set
            {
                this.m_SEID = value;
            }
        }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        [RServiceProperty("p_OPID", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        private decimal? OPID
        {
            get
            {
                return this.m_OPID;
            }
            set
            {
                this.m_OPID = value;
            }
        }

        [RServiceProperty("p_SequenceNumber", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        private decimal? SequenceNumber
        {
            get
            {
                return this.m_SequenceNumber;
            }
            set
            {
                this.m_SequenceNumber = value;
            }
        }

        [RServiceProperty("p_State", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        private decimal State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
            }
        }

        

        #endregion

        #region Методы

        /// <summary>
        /// Создать поле на основе типового и указания опции
        /// </summary>
        /// <param name="typicalFieldCode"></param>
        /// <param name="option"></param>
        /// <param name="prov"></param>
        /// <returns></returns>
        public decimal CreateField(string typicalFieldCode, RSWIFTOptions option, IRSaveProvider prov)
        {
            return this.CreateField(typicalFieldCode, option.ToStr(), prov);
        }

        public decimal CreateField(string typicalFieldCode, string option, IRSaveProvider prov)
        {
            this.m_TypicalFieldCode = typicalFieldCode;
            this.m_Option = option;
            this.Execute(prov);
            return this.m_NewFieldID;
        }

        public decimal CreateField(string typicalFieldCode, string option, decimal? sequenceNumber, IRSaveProvider prov)
        {
            this.m_SequenceNumber = sequenceNumber;
            return this.CreateField(typicalFieldCode, option, prov);
        }

        public decimal CreateField(string typicalFieldCode, string option, decimal? sequenceNumber, decimal state, IRSaveProvider prov)
        {
            this.m_State = state;
            this.m_SequenceNumber = sequenceNumber;
            return this.CreateField(typicalFieldCode, option, prov);
        }

        #endregion
    }

    /// <summary>
    /// Сервис-адаптер для альтернативной начитки объекта юр. лицо по ИНН
    /// </summary>
    [RServiceName("SWIFT_DATA_EXCHANGE.GetSWIFTMT103ByRemittance", AutoAddErrorParam = false)]
    [Author(Developer.Ostanin)]
    internal class GetSWIFTMT103ByRemittanceAdapter : RReadServiceAdapter
    {
        public GetSWIFTMT103ByRemittanceAdapter(decimal seid)
        {
            this.SEID = seid;
        }

        /// <summary>
        /// значение инн
        /// </summary>
        [RServiceProperty("p_nSEID", Direction = ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal SEID { get; set; }
    }

    [Author(Developer.Ostanin)]
    public interface ISWIFTMessageTypicalField : IObject
    {
        string Code { get; set; }

        bool IsRequired { get; }

        string FormatString { get; set; }

        string ExecuteProcedure { get; set; }
    }

    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTSysTypicalFieldMesMT", AutoAddErrorParam = false)]
    [RSavedClassProperty("p_ID", "", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input)]
    [RSavedClassProperty("U25NAME", "", "Name")]
    [Author(Developer.Ostanin)]
    public class RSWIFTMessageSystemTypicalField : RObject, ISWIFTMessageTypicalField
    {
        public RSWIFTMessageSystemTypicalField() : base() { }
        
        public RSWIFTMessageSystemTypicalField(decimal id) : base(id) { }

        #region Поля

        private string m_Code;
        private string m_Flags;
        private string m_FormatString;
        private string m_ExecuteProcedure;

        private string m_StringID;

        #endregion

        #region Свойства

        /// <summary>
        /// Код типового поля
        /// </summary>
        [RSavedProperty("U25CODE")]
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
        /// Флаги типового поля
        /// </summary>
        [RSavedProperty("U25FLAG")]
        public string Flags
        {
            get
            {
                this.CheckLoad();
                return this.m_Flags;
            }
            set
            {
                this.m_Flags = value;
            }
        }

        /// <summary>
        /// Флаг обязательности поля
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return !String.IsNullOrEmpty(this.Flags) && this.Flags[0] == '1';
            }
        }

        /// <summary>
        /// Формат типового поля
        /// </summary>
        [RSavedProperty("U25TPCD")]
        public string FormatString
        {
            get
            {
                this.CheckLoad();
                return this.m_FormatString;
            }
            set
            {
                this.m_FormatString = value;
            }
        }

        /// <summary>
        /// Процедура для формирования значения поля
        /// </summary>
        [RSavedProperty("U25PROC")]
        public string ExecuteProcedure
        {
            get
            {
                this.CheckLoad();
                return this.m_ExecuteProcedure;
            }
            set
            {
                this.m_ExecuteProcedure = value;
            }
        }

        /// <summary>
        /// Кодовый идентификатор
        /// </summary>
        [RSavedProperty("U25SCOD")]
        public string StringID
        {
            get
            {
                this.CheckLoad();
                return this.m_StringID;
            }
            set
            {
                this.m_StringID = value;
            }
        }

        #endregion

        public static ISWIFTMessageTypicalField GetSWIFTTypicalField(string fieldCode, string fieldStringID)
        {
            RGetSWIFTMessageSysTypicalFieldAdapter ad = new RGetSWIFTMessageSysTypicalFieldAdapter(fieldCode, fieldStringID);
            ad.Execute();
            return RBusinessObjectFactory.Current.GetObject(ad.TypicalFieldID, RObjectStandardTypes.RSWIFTMessageSystemTypicalField) as RSWIFTMessageSystemTypicalField;
        }
    }


    [RServiceName("SWIFT_DATA_EXCHANGE.GetSysTypFieldIDByCodeStrID", AutoAddErrorParam = false)]
    public class RGetSWIFTMessageSysTypicalFieldAdapter : RExecServiceAdapter
    {
        public RGetSWIFTMessageSysTypicalFieldAdapter(string fieldCode, string fieldStringID)
            : base()
        {
            this.m_FieldCode = fieldCode;
            this.m_FieldStringID = fieldStringID;
            this.m_TypicalFieldID = -1;
        }

        #region Поля

        private string m_FieldCode;
        private string m_FieldStringID;

        private decimal m_TypicalFieldID;

        #endregion

        #region Свойства

        /// <summary>
        /// Код типового сообщения
        /// </summary>
        [RServiceProperty("p_FieldCode", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Input)]
        public string FieldCode
        {
            get
            {
                return this.m_FieldCode;
            }
            set
            {
                this.m_FieldCode = value;
            }
        }

        /// <summary>
        /// Наименование типового поля
        /// </summary>
        [RServiceProperty("p_FieldStringID", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Input)]
        public string FieldStringID
        {
            get
            {
                return this.m_FieldStringID;
            }
            set
            {
                this.m_FieldStringID = value;
            }
        }

        /// <summary>
        /// Идентификатор типового поля
        /// </summary>
        [RServiceProperty("p_TypicalFieldID", OutDbType = DbParameterType.Decimal, Direction = System.Data.ParameterDirection.Output)]
        public decimal TypicalFieldID
        {
            get
            {
                return this.m_TypicalFieldID;
            }
            set
            {
                this.m_TypicalFieldID = value;
            }
        }

        #endregion
    }

    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTTypicalFieldMesMT", AutoAddErrorParam = false)]
    [RSavedClassProperty("p_ID", "", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input)]
    [RSavedClassProperty("U23NAME", "", "Name")]
    [Author(Developer.Ostanin)]
    public class RSWIFTMessageTypicalField : RObject, ISWIFTMessageTypicalField
    {
        public RSWIFTMessageTypicalField(): base(){}

        public RSWIFTMessageTypicalField(decimal id): base(id){}

        #region Поля

        private string m_TypicalMessageCode;
        private string m_Code;
        private string m_AvailableOptions;
        private string m_Flags;
        private string m_FormatString;
        private string m_ExecuteProcedure;
        private decimal m_SequenceID;

        private decimal m_OrderInSequence;

        private RStandardCollectionItem m_Sequence;

        #endregion

        #region Свойства

        /// <summary>
        /// Код типового сообщения
        /// </summary>
        [RSavedProperty("U23TPMS")]
        public string TypicalMessageCode
        {
            get
            {
                this.CheckLoad();
                return this.m_TypicalMessageCode;
            }
            set
            {
                this.m_TypicalMessageCode = value;
            }
        }

        /// <summary>
        /// Код типового поля
        /// </summary>
        [RSavedProperty("U23CODE")]
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
        /// Доступные опции
        /// </summary>
        [RSavedProperty("U23OPTN")]
        public string AvailableOptions
        {
            get
            {
                this.CheckLoad();
                return this.m_AvailableOptions;
            }
            set
            {
                this.m_AvailableOptions = value;
            }
        }

        /// <summary>
        /// Флаги типового поля
        /// </summary>
        [RSavedProperty("U23FLAG")]
        public string Flags
        {
            get
            {
                this.CheckLoad();
                return this.m_Flags;
            }
            set
            {
                this.m_Flags = value;
            }
        }

        /// <summary>
        /// Флаг обязательности поля
        /// </summary>
        public bool IsRequired
        {
            get
            {
                return !String.IsNullOrEmpty(this.Flags) && this.Flags[0] == '1';
            }
        }

        /// <summary>
        /// Формат типового поля
        /// </summary>
        [RSavedProperty("U23TPCD")]
        public string FormatString
        {
            get
            {
                this.CheckLoad();
                return this.m_FormatString;
            }
            set
            {
                this.m_FormatString = value;
            }
        }

        /// <summary>
        /// Процедура для формирования значения поля
        /// </summary>
        [RSavedProperty("U23PROC")]
        public string ExecuteProcedure
        {
            get
            {
                this.CheckLoad();
                return this.m_ExecuteProcedure;
            }
            set
            {
                this.m_ExecuteProcedure = value;
            }
        }

        /// <summary>
        /// Идентификатор последовательности в типовом сообщении
        /// </summary>
        [RSavedProperty("U23SQID")]
        public decimal SequenceID
        {
            get
            {
                this.CheckLoad();
                return this.m_SequenceID;
            }
            set
            {
                this.m_SequenceID = value;
            }
        }

        [RSavedProperty("U23ORDR")]
        public decimal OrderInSequence
        {
            get
            {
                this.CheckLoad();
                return this.m_OrderInSequence;
            }
            set
            {
                this.m_OrderInSequence = value;
            }
        }

        public RStandardCollectionItem Sequence
        {
            get
            {
                if (this.m_Sequence == null)
                {
                    this.m_Sequence = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTTypicalMessagesSequences).
                        FirstOrDefault(cur => cur.ID == this.SequenceID) as RStandardCollectionItem;
                }
                return this.m_Sequence;
            }
            set
            {
                this.m_Sequence = value;
                this.SequenceID = (value == null) ? -1 : value.ID;
            }
        }

        #endregion

        /// <summary>
        /// Получить типовое поле по описанию
        /// </summary>
        /// <param name="typicalMessageCode"></param>
        /// <param name="fieldName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static RSWIFTMessageTypicalField GetSWIFTTypicalField(string typicalMessageCode, string fieldName, string option, decimal? sequenceNumber = null)
        {
            RGetSWIFTMessageTypicalFieldAdapter ad = new RGetSWIFTMessageTypicalFieldAdapter(typicalMessageCode, fieldName, option, sequenceNumber);
            ad.Execute();
            return RBusinessObjectFactory.Current.GetObject(ad.TypicalFieldID, RObjectStandardTypes.RSWIFTMessageTypicalField) as RSWIFTMessageTypicalField;
        }
    }


    /// <summary>
    /// Получить идентификатор типового поля по наименованию сообщения и по наименованию поля
    /// </summary>
    [RServiceName("SWIFT_DATA_EXCHANGE.GetTypFieldByNameOpMes", AutoAddErrorParam = false)]
    [Author(Developer.Ostanin)]
    public class RGetSWIFTMessageTypicalFieldAdapter : RExecServiceAdapter
    {
        public RGetSWIFTMessageTypicalFieldAdapter(string typicalMessageCode, string fieldName, string option, decimal? sequenceNumber = null)
            : base()
        {
            this.m_TypicalMessageCode = typicalMessageCode;
            this.m_FieldName = fieldName;
            this.m_Option = option;
            this.m_TypicalFieldID = -1;
            this.m_SequenceNumber = sequenceNumber;
        }

        #region Поля

        private string m_TypicalMessageCode;
        private string m_FieldName;
        private string m_Option;

        private decimal m_TypicalFieldID;

        private decimal? m_SequenceNumber;

        #endregion

        #region Свойства

        /// <summary>
        /// Код типового сообщения
        /// </summary>
        [RServiceProperty("p_TypicalMessageCode", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Input)]
        public string TypicalMessageCode
        {
            get
            {
                return this.m_TypicalMessageCode;
            }
            set
            {
                this.m_TypicalMessageCode = value;
            }
        }

        /// <summary>
        /// Наименование типового поля
        /// </summary>
        [RServiceProperty("p_FieldName", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Input)]
        public string FieldName
        {
            get
            {
                return this.m_FieldName;
            }
            set
            {
                this.m_FieldName = value;
            }
        }

        /// <summary>
        /// Опция типового поля
        /// </summary>
        [RServiceProperty("p_Option", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Input)]
        public string Option
        {
            get
            {
                return this.m_Option;
            }
            set
            {
                this.m_Option = value;
            }
        }

        /// <summary>
        /// Идентификатор типового поля
        /// </summary>
        [RServiceProperty("p_TypicalFieldID", OutDbType = DbParameterType.Decimal, Direction = System.Data.ParameterDirection.Output)]
        public decimal TypicalFieldID
        {
            get
            {
                return this.m_TypicalFieldID;
            }
            set
            {
                this.m_TypicalFieldID = value;
            }
        }

        /// <summary>
        /// Последовательность в сообщении
        /// </summary>
        [RServiceProperty("p_SequenceNumber", OutDbType = DbParameterType.Decimal, Direction = System.Data.ParameterDirection.Input)]
        public decimal? SequenceNumber
        {
            get
            {
                return this.m_SequenceNumber;
            }
            set
            {
                this.m_SequenceNumber = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Поле сообщения SWIFT
    /// </summary>
    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTMessageField", AutoAddErrorParam = false)]
    [RSavedProcedure("SWIFT_DATA_EXCHANGE.SaveSWIFTMessageField", AutoAddErrorParam = true)]
    [RDeleteProcedure("SWIFT_DATA_EXCHANGE.DeleteSWIFTMessageField", AutoAddErrorParam = true)]
    [RSavedClassProperty("p_ID", "p_ID", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input)]
    [RSavedClassProperty("", "bNew", "New",
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.Input)]
    [Author(Developer.Ostanin)]
    public class RSWIFTMessageField : RObject
    {
        public RSWIFTMessageField() : base()
        {
            this.m_TypicalFieldType = (decimal)RObjectStandardTypes.RSWIFTMessageTypicalField;
        }

        public RSWIFTMessageField(decimal id) : base(id) { }

        #region Поля

        private decimal m_TypicalFieldID;
        private string  m_Value;
        private string  m_Option;
        private decimal m_PhaseOrderNumber;
        private decimal m_OperationDocumentID;
        private decimal m_Order;
        private decimal m_State;
        private decimal m_MessageID;
        private decimal m_OperationID;

        private ISWIFTMessageTypicalField m_TypicalField;

        private decimal m_TypicalFieldType;

        private decimal m_BlockID;

        #endregion

        #region Свойства

        /// <summary>
        /// Ссылка на типовое поле сообщения SWIFT
        /// </summary>
        [RSavedProperty("U20TPFD", "p_U20TPFD")]
        public decimal TypicalFieldID
        {
            get
            {
                this.CheckLoad();
                return this.m_TypicalFieldID;
            }
            set
            {
                this.m_TypicalFieldID = value;
            }
        }

        /// <summary>
        /// Значение поля сообщения SWIFT
        /// </summary>
        [RSavedProperty("U20MVAL", "p_U20MVAL")]
        public string Value
        {
            get
            {
                this.CheckLoad();
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        /// <summary>
        /// Опция
        /// </summary>
        [RSavedProperty("U20OPTN", "p_U20OPTN")]
        public string Option
        {
            get
            {
                this.CheckLoad();
                return this.m_Option;
            }
            set
            {
                this.m_Option = value;
            }
        }

        /// <summary>
        /// Порядковый номер этапа
        /// </summary>
        [RSavedProperty("U20PHID", "p_U20PHID")]
        public decimal PhaseOrderNumber
        {
            get
            {
                this.CheckLoad();
                return this.m_PhaseOrderNumber;
            }
            set
            {
                this.m_PhaseOrderNumber = value;
            }
        }

        /// <summary>
        /// Идентификатор операционного документа
        /// </summary>
        [RSavedProperty("U20SEID", "p_U20SEID")]
        public decimal OperationDocumentID
        {
            get
            {
                this.CheckLoad();
                return this.m_OperationDocumentID;
            }
            set
            {
                this.m_OperationDocumentID = value;
            }
        }

        /// <summary>
        /// Порядковый номер поля в рамках сообщения
        /// </summary>
        [RSavedProperty("U20ORDR", "p_U20ORDR")]
        public decimal Order
        {
            get
            {
                this.CheckLoad();
                return this.m_Order;
            }
            set
            {
                this.m_Order = value;
            }
        }

        /// <summary>
        /// Состояние
        /// </summary>
        [RSavedProperty("U20STAT", "p_U20STAT")]
        public decimal State
        {
            get
            {
                this.CheckLoad();
                return this.m_State;
            }
            set
            {
                this.m_State = value;
            }
        }

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        [RSavedProperty("U20RND2", "p_U20RND2")]
        public decimal MessageID
        {
            get
            {
                this.CheckLoad();
                return this.m_MessageID;
            }
            set
            {
                this.m_MessageID = value;
            }
        }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        [RSavedProperty("U20OPID", "p_U20OPID")]
        public decimal OperationID
        {
            get
            {
                this.CheckLoad();
                return this.m_OperationID;
            }
            set
            {
                this.m_OperationID = value;
            }
        }

        /// <summary>
        /// Тип поля
        /// </summary>
        [RSavedProperty("U20TYPE", "p_U20TYPE")]
        public decimal TypicalFieldType
        {
            get
            {
                this.CheckLoad();
                return this.m_TypicalFieldType;
            }
            set
            {
                this.m_TypicalFieldType = value;
            }
        }

        /// <summary>
        /// Идентификатор блока
        /// </summary>
        [RSavedProperty("U20BLID", "p_U20BLID")]
        public decimal BlockID
        {
            get
            {
                this.CheckLoad();
                return this.m_BlockID;
            }
            set
            {
                this.m_BlockID = value;
            }
        }

        /// <summary>
        /// Типовое поле сообщения SWIFT
        /// </summary>
        public ISWIFTMessageTypicalField TypicalField
        {
            get
            {
                if (this.m_TypicalField == null)
                {
                    this.m_TypicalField = RBusinessObjectFactory.Current.GetObject(this.TypicalFieldID, 
                            (this.TypicalFieldType == 0) ? (decimal)RObjectStandardTypes.RSWIFTMessageTypicalField: this.TypicalFieldType
                        ) as ISWIFTMessageTypicalField;
                    //RObjectStandardTypes.RSWIFTMessageTypicalField)
                }
                return this.m_TypicalField;
            }
            set
            {
                this.m_TypicalField = value;
                this.TypicalFieldID = (value != null) ? value.ID : 0;
            }
        }

        /// <summary>
        /// Наименование и опция
        /// </summary>
        public string NameAndOption
        {
            get
            {
                return this.TypicalField.Code + this.Option;
            }
        }

        #endregion

        /// <summary>
        /// Создать новое системное поле
        /// </summary>
        /// <param name="code"></param>
        /// <param name="stringID"></param>
        /// <param name="messageType"></param>
        /// <param name="value"></param>
        /// <param name="messageType"></param>
        /// <param name="messageID"></param>
        /// <param name="scriptState"></param>
        /// <returns></returns>
        public static RSWIFTMessageField CreateNewSWIFTMessageSystemField(string code, string stringID, string value, string messageType,
            decimal messageID, IScriptState scriptState, decimal blockID)
        {
            RSWIFTMessageField field = new RSWIFTMessageField();
            field.New = true;
            field.Value = value;

            field.PhaseOrderNumber = scriptState.PHID;
            field.OperationDocumentID = (scriptState.SEID == null) ? -1 : (decimal)scriptState.SEID;
            field.State = 1; // создано
            field.MessageID = messageID;
            field.OperationID = scriptState.OPID;
            field.TypicalFieldType = (decimal)RObjectStandardTypes.RSWIFTMessageSystemTypicalField;
            field.TypicalField = RSWIFTMessageSystemTypicalField.GetSWIFTTypicalField(code, stringID);
            field.BlockID = blockID;
            return field;
        }
        /// <summary>
        /// Создать новое поле
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <param name="messageType"></param>
        /// <param name="messageID"></param>
        /// <param name="scriptState"></param>
        /// <returns></returns>
        public static RSWIFTMessageField CreateNewSWIFTMessageField(string name, string option, string value, string messageType,
            decimal messageID, IScriptState scriptState, decimal? sequenceNumber = null)
        {
            RSWIFTMessageField field = new RSWIFTMessageField();
            field.New = true;
            field.Value = value;

            field.Option = option;
            field.PhaseOrderNumber = scriptState.PHID;
            field.OperationDocumentID = (scriptState.SEID == null) ? -1 : (decimal)scriptState.SEID;
            field.State = 1; // создано
            field.MessageID = messageID;
            field.OperationID = scriptState.OPID;
            field.TypicalFieldType = (decimal)RObjectStandardTypes.RSWIFTMessageTypicalField;
            field.TypicalField = RSWIFTMessageTypicalField.GetSWIFTTypicalField(messageType, name, option, sequenceNumber);
            return field;
        }

        public static RSWIFTMessageField CreateNewSWIFTMessageField(string name, RSWIFTOptions option, string value, string messageType,
            decimal messageID, IScriptState scriptState, decimal? sequenceNumber = null)
        {
            return CreateNewSWIFTMessageField(name, option.ToStr(), value, messageType, messageID, scriptState, sequenceNumber);
        }

        public override string ToString()
        {
            return ":" + this.NameAndOption + ":" + Value;
        }
    }

    /// <summary>
    /// Коллекция полей сообщения SWIFT
    /// </summary>
    [RReadCollectionAttribute("SWIFT_DATA_EXCHANGE.GetSWIFTMessageFields", ColID = "U20MSID", ThisOnly = true)]
    [Author(Developer.Ostanin)]
    public class RSWIFTMessageFieldsCollection : RObjectCollection<RSWIFTMessageField>
    {
        public RSWIFTMessageFieldsCollection()
            : base()
        {
        }

        public RSWIFTMessageFieldsCollection(decimal id) :
            this()
        {
            this.m_ID = id;
        }

        private decimal m_ID;

        [RServiceProperty("p_ID", Direction = ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal ID
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

        private void OrderFields()
        {
            IOrderedEnumerable<RSWIFTMessageField> coll =
                this.Where(cur => cur.TypicalField is RSWIFTMessageTypicalField).
                    OrderBy(cur => (cur.TypicalField as RSWIFTMessageTypicalField).Sequence.StringID).
                        ThenBy(cur => (cur.TypicalField as RSWIFTMessageTypicalField).OrderInSequence).
                            ThenBy(cur => this.IndexOf(cur));     //cur => cur.TypicalField.Code).ThenBy(cur => cur.Option).
            IOrderedEnumerable<RSWIFTMessageField> coll2 =
                this.Where(cur => !(cur.TypicalField is RSWIFTMessageTypicalField)).OrderBy(cur => this.IndexOf(cur));
            decimal i = 1;
            if (coll != null)
            {
                foreach (RSWIFTMessageField fld in coll)
                {
                    fld.Order = i++;
                }
            }
            if (coll2 != null)
            {
                foreach (RSWIFTMessageField fld in coll2)
                {
                    fld.Order = i++;
                }
            }
        }

        public void AddWithOrder(RSWIFTMessageField field)
        {
            this.Add(field);
            this.OrderFields();
        }

        public void RemoveWithOrder(RSWIFTMessageField field)
        {
            this.Remove(field);
            this.OrderFields();
        }

        public override void Save(IRSaveProvider provider)
        {
            foreach (RSWIFTMessageField fld in this)
            {
                if (fld.TypicalField.IsRequired || !string.IsNullOrEmpty(fld.Value))
                {
                    fld.Save(provider);
                }
            }
            //base.Save(provider);
        }
    }

    /// <summary>
    /// Статусы сообщения MT103
    /// </summary>
    public enum SWIFTMTStatuses
    {
        [DisplayString(" ")]
        Empty = -1,
        [DisplayString("Новое")]
        New = 0,
        [DisplayString("Готово к отправке")]
        ReadyToSend = 1,
        [DisplayString("Отправлено")]
        Sended = 2,
        [DisplayString("Подтверждено")]
        Confirmed = 3,
        [DisplayString("Некорректно сформировано по системе SWIFT")]
        NotCorrectForSWIFT = 4,
        [DisplayString("Корректно сформировано по системе SWIFT")]
        CorrectForSWIFT = 5,
        [DisplayString("Данные сообщения готовы")]
        DataIsReady = 6,
        [DisplayString("Помечено на отзыв")]
        ReadyToCancel = 7,
        [DisplayString("Отозвано")]
        Cancelled = 8
    }

    //public static class SWIFTMTStatusesExtensions
    //{
    //    public static SWIFTMTStatuses status = SWIFTMTStatuses.Empty;

    //    public static string ToStr(this RSWIFTOptions opt)
    //    {
    //        string str = String.Empty;
    //        switch (opt)
    //        {
    //            case SWIFTMTStatuses.Empty: str = String.Empty; break;
    //            case SWIFTMTStatuses.New: str = String.Empty; break;
    //            case SWIFTMTStatuses.ReadyToSend: str = "A"; break;
    //            case SWIFTMTStatuses.Sended: str = "B"; break;
    //            case SWIFTMTStatuses.Confirmed: str = "C"; break;
    //            case SWIFTMTStatuses.NotCorrectForSWIFT: str = "D"; break;
    //            case SWIFTMTStatuses.CorrectForSWIFT: str = "F"; break;
    //            case SWIFTMTStatuses.DataIsReady: str = "K"; break;
    //            case SWIFTMTStatuses.ReadyToCancel: str = "K"; break;
    //            case SWIFTMTStatuses.Cancelled: str = "K"; break;
    //            default: break;
    //        }
    //        return str;
    //    }
    //}

    /// <summary>
    /// Описание опций SWIFT
    /// </summary>
    public enum RSWIFTOptions
    {
        [DisplayString(" ")]
        Empty = -1,
        [DisplayString("Без опции")]
        WithoutOptions = 0,
        [DisplayString("Опция A. SWIFT(BEI) и номер счета")]
        OptionA = 1,
        [DisplayString("Опция B. Номер счета")]
        OptionB = 2,
        [DisplayString("Опция С. Номер счета или код клиринговой системы")]
        OptionC = 3,
        [DisplayString("Опция D. Наименование, адрес")]
        OptionD = 4,
        [DisplayString("Опция F. Счет, наименование, страна и город, адрес, дата и место рождения клиента и др.")]
        OptionF = 5,
        [DisplayString("Опция K. Счет, наименование, адрес")]
        OptionK = 6
    }

    public static class RSWIFTOptionsExtensions
    {
        public static RSWIFTOptions option = RSWIFTOptions.Empty;

        public static string ToStr(this RSWIFTOptions opt)
        {
            string str = String.Empty;
            switch (opt)
            {
                case RSWIFTOptions.Empty: str = String.Empty; break;
                case RSWIFTOptions.WithoutOptions: str = String.Empty; break;
                case RSWIFTOptions.OptionA: str = "A"; break;
                case RSWIFTOptions.OptionB: str = "B"; break;
                case RSWIFTOptions.OptionC: str = "C"; break;
                case RSWIFTOptions.OptionD: str = "D"; break;
                case RSWIFTOptions.OptionF: str = "F"; break;
                case RSWIFTOptions.OptionK: str = "K"; break;
                default: break;
            }
            return str;
        }

        public static RSWIFTOptions GetEnum(this RSWIFTOptions en, string opt)
        {
            switch (opt)
            {
                case "": en = RSWIFTOptions.WithoutOptions; break;
                case "A": en = RSWIFTOptions.OptionA; break;
                case "B": en = RSWIFTOptions.OptionB; break;
                case "C": en = RSWIFTOptions.OptionC; break;
                case "D": en = RSWIFTOptions.OptionD; break;
                case "F": en = RSWIFTOptions.OptionF; break;
                case "K": en = RSWIFTOptions.OptionK; break;
            }
            return en;
        }
    }

    /// <summary>
    /// Типы информации о платеже для юр. лиц
    /// </summary>
    public enum RSWIFTLegalPaymentInfoTypes
    {
        [DisplayString("Инвойс")]
        Invoice = 1,
        [DisplayString("Операция с использованием службы Trade Services Utility")]
        TradeServiceUtilityInvoiceNumber = 2,
        [DisplayString("Референс для клиента-бенефициара")]
        ReferenceForBeneficiary = 3,
        [DisplayString("Референс клиента-заказчика")]
        ReferenceOfClient = 4,
        [DisplayString("Уникальный референс, определяющий международные платежные инструкции")]
        UniqueReferenceWithInternationalRegulatios = 5
    }

    /// <summary>
    /// Участник, которому предназначаются дополнительные инструкции по переводу
    /// </summary>
    public enum RSWIFTAdditionalInstructionsPurposes
    {
        [DisplayString(" ")]
        None = 0,
        [DisplayString("ACC - Инструкции для банка бенефициара")]
        ForBankBeneficiary = 1,
        [DisplayString("INS - Инструкции от организации-приказодателя банку-отправителя об исполнении операции")]
        ForBankSenderFromOrganizationSender = 2,
        [DisplayString("INT - Инструкции для банка-посредника")]
        ForBankIntermediary = 3,
        [DisplayString("REC - Инструкции для бенефициара")]
        ForBeneficiary = 4
    }

    public static class RSWIFTAdditionalInstructionsPurposesExtensions
    {
        public static RSWIFTAdditionalInstructionsPurposes instruction = RSWIFTAdditionalInstructionsPurposes.None;

        public static string ToStr(this RSWIFTAdditionalInstructionsPurposes instr)
        {
            string str = String.Empty;
            switch (instr)
            {
                case RSWIFTAdditionalInstructionsPurposes.None: str = String.Empty; break;
                case RSWIFTAdditionalInstructionsPurposes.ForBankBeneficiary: str = "ACC"; break;
                case RSWIFTAdditionalInstructionsPurposes.ForBankSenderFromOrganizationSender: str = "INS"; break;
                case RSWIFTAdditionalInstructionsPurposes.ForBankIntermediary: str = "INT"; break;
                case RSWIFTAdditionalInstructionsPurposes.ForBeneficiary: str = "REC"; break;
                default: break;
            }
            return str;
        }
    }

    /// <summary>
    /// Сторона, несущая расходы
    /// </summary>
    public enum RSWIFTCommisionWay
    {
        [DisplayString("BEN")]
        Beneficiary = 1,
        [DisplayString("OUR")]
        Sender = 2,
        [DisplayString("SHA")]
        Separetely = 3
    }

    public static class RSWIFTCommisionWayExtensions
    {
        public static RSWIFTCommisionWay instruction = RSWIFTCommisionWay.Sender;

        public static string ToStr(this RSWIFTCommisionWay instr)
        {
            string str = String.Empty;
            switch (instr)
            {
                case RSWIFTCommisionWay.Beneficiary: str = "BEN"; break;
                case RSWIFTCommisionWay.Sender: str = "OUR"; break;
                case RSWIFTCommisionWay.Separetely: str = "SHA"; break;
                default: break;
            }
            return str;
        }

        public static RSWIFTCommisionWay GetEnum(this RSWIFTCommisionWay en, string instr)
        {
            switch (instr)
            {
                case "BEN": en = RSWIFTCommisionWay.Beneficiary; break;
                case "OUR": en = RSWIFTCommisionWay.Sender; break;
                case "SHA": en = RSWIFTCommisionWay.Separetely; break;
                default: en = RSWIFTCommisionWay.Sender; break;
            }
            return en;
        }
    }

    /// <summary>
    /// Получить коллекцию непривязанных сообщений отзыва
    /// </summary>
    [RReadCollectionAttribute("SWIFT_DATA_EXCHANGE.GetSWIFTNotLnkCancelMessages", ColID = "B98RND2", ThisOnly = true)]
    [Author(Developer.Ostanin)]
    public class RSWIFTCancelMessageMTCollection : RObjectCollection<RSWIFTMessageMT>
    {
        public RSWIFTCancelMessageMTCollection(): base()
        {
            
        }
    }

    /// <summary>
    /// Получить коллекцию непривязанных сообщений ответа на отзыв или произвольных сообщений
    /// </summary>
    [RReadCollectionAttribute("SWIFT_DATA_EXCHANGE.GetSWIFTNotLnkCAnsMessages", ColID = "B98RND2", ThisOnly = true)]
    [Author(Developer.Ostanin)]
    public class RSWIFTCancelMessageAnswersMTCollection : RObjectCollection<RSWIFTMessageMT>
    {
        public RSWIFTCancelMessageAnswersMTCollection()
            : base()
        {

        }
    }
}
