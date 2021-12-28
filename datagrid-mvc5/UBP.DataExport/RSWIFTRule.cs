using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBP.Business.TypeManager;
using UBP.BusinessFactory;
using UBP.Collection;
using UBP.Common;
using UBP.Core;
using UBP.ViewModel.Core;

namespace UBP.DataExport
{
    /// <summary>
    /// Правило проверки SWIFT
    /// </summary>
    [RLoadProcedure("SWIFT_DATA_EXCHANGE.GetSWIFTRule", AutoAddErrorParam = false)]
    [RSavedProcedure("SWIFT_DATA_EXCHANGE.SaveSWIFTRule", AutoAddErrorParam = true)]
    [RSavedClassProperty("p_ID", "p_ID", "ID",
        LoadOraType = DbParameterType.Decimal,
        LoadDirection = System.Data.ParameterDirection.Input,
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.InputOutput)]
    [RSavedClassProperty("U21NAME", "NAME", "Name")]
    [RSavedClassProperty("U21TPID", "TPID", "TypeID")]
    [RSavedClassProperty("", "bNew", "New",
        SavedOraType = DbParameterType.Decimal,
        SavedDirection = System.Data.ParameterDirection.Input)]
    public class RSWIFTRule : RObject
    {
        public RSWIFTRule() : base()
        { }

        public RSWIFTRule(decimal id):
            base(id)
        { }

        #region Поля

        private string m_ErrorCodesStringCollection;
        private string m_CheckLogicBlock;
        private string m_Description;
        private string m_TypicalMessageName;

        private string m_TypicalPhasesForRule;
        private decimal m_State;
        private string m_SWIFTMessageFieldsString;

        private List<RSWIFTMessageTypicalField> m_TypicalFields;
        private List<ITypicalPhase> m_TypicalPhases;
        private List<RStandardCollectionItem> m_SWIFTErrorsCollection;

        private RStandardCollectionItem m_TypicalMessage;

        #endregion

        #region Свойства

        /// <summary>
        /// Список кодов ошибок 
        /// </summary>
        [RSavedProperty("U21ECOD", "ECOD")]
        private string ErrorCodesStringCollection
        {
            get
            {
                this.CheckLoad();
                return this.m_ErrorCodesStringCollection;
            }
            set
            {
                this.m_ErrorCodesStringCollection = value;
            }
        }

        /// <summary>
        /// Список используемых в правиле типовых полей сообщения
        /// </summary>
        [RSavedProperty("U21TFLT", "TFLT")]
        private string SWIFTMessageFieldsString
        {
            get
            {
                this.CheckLoad();
                return this.m_SWIFTMessageFieldsString;
            }
            set
            {
                this.m_SWIFTMessageFieldsString = value;
            }
        }

        /// <summary>
        /// Логика проверки правила
        /// </summary>
        [RSavedProperty("U21RSQL", "RSQL")]
        public string CheckLogicBlock
        {
            get
            {
                this.CheckLoad();
                return this.m_CheckLogicBlock;
            }
            set
            {
                this.m_CheckLogicBlock = value;
            }
        }

        /// <summary>
        /// Описание правила
        /// </summary>
        [RSavedProperty("U21DESC", "DSC")]
        public string Description
        {
            get
            {
                this.CheckLoad();
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
            }
        }

        /// <summary>
        /// Название типового сообщения
        /// </summary>
        [RSavedProperty("U21TPMS", "TPMS")]
        public string TypicalMessageName
        {
            get
            {
                this.CheckLoad();
                return this.m_TypicalMessageName;
            }
            set
            {
                this.m_TypicalMessageName = value;
            }
        }

        /// <summary>
        /// Список типовых этапов через черту, в которых можно пускать правило
        /// </summary>
        [RSavedProperty("U21PHTP", "PHTP")]
        private string TypicalPhasesForRule
        {
            get
            {
                this.CheckLoad();
                return this.m_TypicalPhasesForRule;
            }
            set
            {
                this.m_TypicalPhasesForRule = value;
            }
        }

        /// <summary>
        /// Состояние
        /// </summary>
        [RSavedProperty("U21STAT", "STAT")]
        private decimal State
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
        public RSWIFTRuleStates Status
        {
            get
            {
                return (RSWIFTRuleStates) this.State;
            }
            set
            {
                this.State = (decimal) value;
            }
        }

        /// <summary>
        ///  Типовое сообщение
        /// </summary>
        public RStandardCollectionItem TypicalMessage
        {
            get
            {
                if (this.m_TypicalMessage == null && this.TypicalMessageName != null && this.TypicalMessageName != String.Empty)
                {
                    this.m_TypicalMessage = RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTTypicalMessages).
                        FindByID(this.TypicalMessageName);
                }
                return this.m_TypicalMessage;
            }
            set
            {
                this.m_TypicalMessage = value;
                this.TypicalMessageName = (this.m_TypicalMessage == null)? String.Empty: this.m_TypicalMessage.StringID;
            }
        }

        /// <summary>
        /// Тип правила SWIFT
        /// </summary>
        public RSWIFTRuleTypes SWIFTRuleType
        {
            get
            {
                return (RSWIFTRuleTypes)this.TypeID;
            }
            set
            {
                this.SetTypeID((long)value);
            }
        }


        /// <summary>
        /// Коллекция ошибок SWIFT
        /// </summary>
        public List<RStandardCollectionItem> SWIFTErrorsCollection
        {
            get
            {
                if (this.m_SWIFTErrorsCollection == null)
                {
                    this.m_SWIFTErrorsCollection = new List<RStandardCollectionItem>();
                    if (this.ErrorCodesStringCollection != null && this.ErrorCodesStringCollection != String.Empty)
                    {
                        string[] masCodes = this.ErrorCodesStringCollection.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                        if (masCodes.Count() > 0)
                        {
                            foreach (string code in masCodes)
                            {
                                RStandardCollectionItem it = RStandardCollectionPull.GetStandardCollection(
                                    RStandardCollectionTypes.SWIFTErrorCodes).FindByID(int.Parse(code));
                                this.m_SWIFTErrorsCollection.Add(it);
                            }
                        }
                    }
                }
                return this.m_SWIFTErrorsCollection;
            }
            set
            {
                this.m_SWIFTErrorsCollection = value;
            }
        }

        /// <summary>
        /// Список типовых полей
        /// </summary>
        public List<RSWIFTMessageTypicalField> TypicalFields
        {
            get
            {
                if (this.m_TypicalFields == null)
                {
                    this.m_TypicalFields = new List<RSWIFTMessageTypicalField>();
                    if (this.SWIFTMessageFieldsString != String.Empty && this.SWIFTMessageFieldsString != null)
                    {
                        string[] mas = this.SWIFTMessageFieldsString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string str in mas)
                        {
                            RSWIFTMessageTypicalField fld = new RSWIFTMessageTypicalField(Decimal.Parse(str));
                            fld.Load();
                            this.m_TypicalFields.Add(fld);
                        }
                    }
                }
                return this.m_TypicalFields;
            }
            set
            {
                this.m_TypicalFields = value;
                //if (this.TypicalFields.Count > 0)
                //{
                //    this.m_SWIFTMessageFieldsString =  String.Join("|", value.Select(cur => cur.ID));
                //}
            }
        }

        /// <summary>
        /// Коллекция типовых этапов, в которых срабатывает правило
        /// </summary>
        public List<ITypicalPhase> TypicalPhases
        {
            get
            {
                if (this.m_TypicalPhases == null)
                {
                    this.m_TypicalPhases = new List<ITypicalPhase>();
                    if (this.TypicalPhasesForRule != String.Empty && this.TypicalPhasesForRule != null)
                    {
                        string[] mas = this.TypicalPhasesForRule.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string str in mas)
                        {
                            ITypicalPhase phase = 
                                RBusinessObjectFactory.Current.GetObject(Decimal.Parse(str),RObjectStandardTypes.TypicalPhase) as ITypicalPhase;
                            this.m_TypicalPhases.Add(phase);
                        }
                    }
                    
                }
                return this.m_TypicalPhases;
            }
            set
            {
                this.m_TypicalPhases = value;
            }
        }

        #endregion

        protected override void VirtSave(IRSaveProvider pConnection)
        {
            this.ErrorCodesStringCollection = String.Join("|", this.SWIFTErrorsCollection.Select(cur => cur.ID));
            this.SWIFTMessageFieldsString = String.Join("|", this.TypicalFields.Select(cur => cur.ID));
            this.TypicalPhasesForRule = String.Join("|", this.TypicalPhases.Select(cur => cur.ID));
            base.VirtSave(pConnection);
        }
    }

    [RServiceName("SWIFT_DATA_EXCHANGE.CheckRulesForMes", AutoAddErrorParam = true)]
    public class RCheckRulesForMessageInTypicalPhaseAdapter : RExecServiceAdapter
    {
        public RCheckRulesForMessageInTypicalPhaseAdapter(decimal PHTP, decimal messageID)
        {
            this.m_TypicalPhaseID = PHTP;
            this.m_MessageID = messageID;
            this.m_ErrorsCodesStringCollection = String.Empty;
            this.m_ErrorsCRulesStringCollection = String.Empty;
        }

        #region Поля

        private decimal m_TypicalPhaseID;
        private decimal m_MessageID;
        private string m_ErrorsCodesStringCollection;
        private string m_ErrorsCRulesStringCollection;

        private List<RSWIFTRule> m_RulesCollection;

        #endregion

        #region Свойства

        /// <summary>
        /// Список кодов ошибок
        /// </summary>  
        [RServiceProperty("p_ErrorCodes", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Output,Size = 3000)]
        public string ErrorsCodesStringCollection
        {
            get
            {
                return this.m_ErrorsCodesStringCollection;
            }
            set
            {
                this.m_ErrorsCodesStringCollection = value;
            }
        }

        /// <summary>
        /// Список идентификаторов правил, на которых произошла ошибка
        /// </summary>
        [RServiceProperty("p_ErrorRules", OutDbType = DbParameterType.Varchar2, Direction = System.Data.ParameterDirection.Output, Size = 3000)]
        public string ErrorsCRulesStringCollection
        {
            get
            {
                return this.m_ErrorsCRulesStringCollection;
            }
            set
            {
                this.m_ErrorsCRulesStringCollection = value;
            }
        }

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        [RServiceProperty("p_B98RND2", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
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
        /// Идентификатор типового этапа
        /// </summary>
        [RServiceProperty("p_PHTP", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal TypicalPhaseID
        {
            get
            {
                return this.m_TypicalPhaseID;
            }
            set
            {
                this.m_TypicalPhaseID = value;
            }
        }

        [RNonServiceProperty]
        public List<RStandardCollectionItem> RulesErrorsCollection
        {
            get
            {
                List<RStandardCollectionItem> coll = new List<RStandardCollectionItem>();
                string[] masCodes = this.ErrorsCodesStringCollection.Split(new char[] { '¦' }, StringSplitOptions.RemoveEmptyEntries);

                if (masCodes.Count() > 0)
                {
                    foreach (string code in masCodes)
                    {
                        RStandardCollectionItem it = 
                            RStandardCollectionPull.GetStandardCollection(RStandardCollectionTypes.SWIFTErrorCodes).FindByID(int.Parse(code));
                        coll.Add(it);
                    }
                }
                return coll;
            }
        }

        /// <summary>
        /// Коллекция правил, в которых случилась ошибка SWIFT
        /// </summary>
        [RNonServiceProperty]
        public List<RSWIFTRule> RulesCollection
        {
            get
            {
                if (this.m_RulesCollection == null)
                {
                    this.m_RulesCollection = new List<RSWIFTRule>();
                    string[] masCodes = this.ErrorsCRulesStringCollection.Split(new char[] { '¦' }, StringSplitOptions.RemoveEmptyEntries);
                    if (masCodes.Count() > 0)
                    {
                        foreach (string code in masCodes)
                        {
                            RSWIFTRule it = new RSWIFTRule(Decimal.Parse(code));
                            it.Load();
                            this.m_RulesCollection.Add(it);
                        }
                    }
                }
                return this.m_RulesCollection;
            }
        }

        #endregion
    }

    /// <summary>
    /// Адаптер для проверки форматов полей, наличия полей и данных в полях
    /// </summary>
    [RServiceName("SWIFT_DATA_EXCHANGE.CheckFMesOnExistsAndFormats", AutoAddErrorParam = true)]
    public class RCheckAllSWIFTMessageFieldsOnPhaseOnExistence : RExecServiceAdapter
    {
        public RCheckAllSWIFTMessageFieldsOnPhaseOnExistence(decimal PHID, decimal messageID, string modeStringMask):
            base()
        {
            this.m_PhaseID = PHID;
            this.m_MessageID = messageID;
            this.m_ModeStringMask = modeStringMask;
        }

        private decimal m_PhaseID;
        private decimal m_MessageID;
        private string m_ModeStringMask;

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        [RServiceProperty("p_B98RND2", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
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
        /// Идентификатор типового этапа
        /// </summary>
        [RServiceProperty("p_PHID", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Decimal)]
        public decimal PhaseID
        {
            get
            {
                return this.m_PhaseID;
            }
            set
            {
                this.m_PhaseID = value;
            }
        }

        /// <summary>
        /// Маска с режимами работы проверок
        /// </summary>
        [RServiceProperty("p_ModeStringMask", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Varchar2)]
        public string ModeStringMask
        {
            get
            {
                return this.m_ModeStringMask;
            }
            set
            {
                this.m_ModeStringMask = value;
            }
        }
    }

    [RServiceName("SWIFT_DATA_EXCHANGE.TransformSWIFTRegExpToPOSIX", AutoAddErrorParam = false)]
    public class RTransformSWIFTRegularExpressionToPOSIXAdapter : RExecServiceAdapter
    {
        public RTransformSWIFTRegularExpressionToPOSIXAdapter(string swiftRegExpString) :
            base()
        {
            this.m_POSIXRegExpString = String.Empty;
            this.m_SWIFTRegExpString = swiftRegExpString;
        }

        private string m_POSIXRegExpString;
        private string m_SWIFTRegExpString;

        /// <summary>
        /// Регулярное выражение в формате SWIFT
        /// </summary>
        [RServiceProperty("p_SWIFTRegExp", Direction = System.Data.ParameterDirection.Input, OutDbType = DbParameterType.Varchar2)]
        public string SWIFTRegExpString
        {
            get
            {
                return this.m_SWIFTRegExpString;
            }
            set
            {
                this.m_SWIFTRegExpString = value;
            }
        }

        /// <summary>
        /// Регулярное выражение в формате POSIX
        /// </summary>
        [RServiceProperty("p_POSIXRegExp", Direction = System.Data.ParameterDirection.Output, OutDbType = DbParameterType.Varchar2,Size = 5000)]
        public string POSIXRegExpString
        {
            get
            {
                return this.m_POSIXRegExpString;
            }
            set
            {
                this.m_POSIXRegExpString = value;
            }
        }
    }

    /// <summary>
    /// Состояния правила проверки SWIFT
    /// </summary>
    public enum RSWIFTRuleStates
    {
        [DisplayString("Включено")]
        Enabled = 1,
        [DisplayString("Отключено")]
        Disabled = 0
    }

    /// <summary>
    /// Типы правил проверки SWIFT
    /// </summary>
    public enum RSWIFTRuleTypes
    {
        [DisplayString("Общее правило сообщения SWIFT")]
        CommonRule = 1063,
        [DisplayString("Правило заполнения полей SWIFT")]
        FieldRule = 1064,
        [DisplayString("Пользовательское правило SWIFT")]
        UserRule = 1065
    }
}
