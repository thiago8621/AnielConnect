using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Configuracao_Tributaria : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual Tipo_Tributacao? Tipo_Tributacao { get; set; }
        public virtual Cfop CFOP { get; set; }
        public virtual SPED SPED { get; set; }
        public virtual String Serie_Nfse { get; set; }
        public virtual Boolean Retencao_Imposto { get; set; }
        public virtual Boolean Deduzir_Material { get; set; }
        public virtual Boolean Deduzir_Valor_Liquido { get; set; }
        public virtual String Informacoes_NF { get; set; }

        private IList<Configuracao_TributariaICMS> _ICMS;
        public virtual IList<Configuracao_TributariaICMS> ICMS
        {
            get
            {
                if (_ICMS == null)
                    _ICMS = new List<Configuracao_TributariaICMS>();
                return _ICMS;
            }
            set
            {
                _ICMS = value;
            }
        }

        public virtual CST_IPI CST_IPI { get; set; }
        public virtual Double Aliquota_IPI { get; set; }
        public virtual Double Reducao_Base_Calculo_IPI { get; set; }

        public virtual CST_PIS CST_PIS { get; set; }
        public virtual Double Aliquota_PIS { get; set; }
        public virtual Double Reducao_Base_Calculo_PIS { get; set; }

        public virtual CST_COFINS CST_COFINS { get; set; }
        public virtual Double Aliquota_COFINS { get; set; }
        public virtual Double Reducao_Base_Calculo_COFINS { get; set; }

        private IList<Configuracao_TributariaISSQN> _ISSQN;
        public virtual IList<Configuracao_TributariaISSQN> ISSQN
        {
            get
            {
                if (_ISSQN == null)
                    _ISSQN = new List<Configuracao_TributariaISSQN>();
                return _ISSQN;
            }
            set
            {
                _ISSQN = value;
            }
        }


        public virtual Double Aliquota_IRRF { get; set; }
        public virtual Double Aliquota_CSLL { get; set; }
        public virtual Double Aliquota_INSS { get; set; }

    }

    public class Configuracao_TributariaICMS : BaseModel
    {
        public virtual string UF { get; set; }
        public virtual Origem_Mercadoria Origem_Mercadoria { get; set; }
        public virtual Regime_TributacaoICMS? Regime_Tributacao { get; set; }
        public virtual Atividade_Nf Atividade_Nf { get; set; }
        public virtual CST_ICMS CST_ICMS { get; set; }
        public virtual CSOSN_ICMS CSOSN_ICMS { get; set; }
        public virtual Cfop CFOP { get; set; }
        public virtual Double Aliquota { get; set; }
        public virtual Double Reducao_Base_Calculo { get; set; }
        public virtual Configuracao_Tributaria Configuracao_Tributaria { get; set; }
    }

    public class CSOSN_ICMS : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class CST_ICMS : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class Origem_Mercadoria : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class Configuracao_TributariaISSQN : BaseModel
    {
        public virtual string UF { get; set; }
        public virtual Cidade Municipio { get; set; }
        public virtual Atividade_Nf Atividade_Nf { get; set; }
        public virtual Cfop CFOP { get; set; }
        public virtual Double Aliquota { get; set; }
        public virtual Configuracao_Tributaria Configuracao_Tributaria { get; set; }
        public virtual Natureza_TributacaoEnum? Natureza_Tributacao { get; set; }
        public virtual TipoTributacaoISS? TipoTributacaoISS { get; set; }
        public virtual String Cod_Tributacao_Municipio { get; set; }
    }

    public class CST_COFINS : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class CST_PIS : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class CST_IPI : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class SPED : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class Cfop : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual Tipo_MovimentoCFOP? TipoMovimento { get; set; }
    }

    public enum Tipo_MovimentoCFOP
    {
        Entrada = 1,
        Saida = 2
    }

    #region ListModel

    public class ConfiguracaoTributariaListModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }

        public virtual string Status { get; set; }

        public virtual string Tipo_Tributacao { get; set; }
        public virtual Int64? CFOP { get; set; }
        public virtual Int64? SPED { get; set; }
        public virtual String Serie_Nfse { get; set; }
        public virtual Boolean Retencao_Imposto { get; set; }
        public virtual Boolean Deduzir_Material { get; set; }
        public virtual Boolean Deduzir_Valor_Liquido { get; set; }
        public virtual String Informacoes_NF { get; set; }

        public virtual Int64? CST_IPI { get; set; }
        public virtual String Aliquota_IPI { get; set; }
        public virtual String Reducao_Base_Calculo_IPI { get; set; }

        public virtual Int64? CST_PIS { get; set; }
        public virtual String Aliquota_PIS { get; set; }
        public virtual String Reducao_Base_Calculo_PIS { get; set; }

        public virtual Int64? CST_COFINS { get; set; }
        public virtual String Aliquota_COFINS { get; set; }
        public virtual String Reducao_Base_Calculo_COFINS { get; set; }

        public virtual String Aliquota_IRRF { get; set; }
        public virtual String Aliquota_CSLL { get; set; }
        public virtual String Aliquota_INSS { get; set; }

        public virtual string TipoTributacaoISS { get; set; }
        public virtual string ICMS_UF { get; set; }
        public virtual Int64? ICMS_Origem_Mercadoria { get; set; }
        public virtual string ICMS_Regime_Tributacao { get; set; }
        public virtual Int64? ICMS_CST_ICMS { get; set; }
        public virtual Int64? ICMS_CSOSN_ICMS { get; set; }
        public virtual Int64? ICMS_CFOP { get; set; }
        public virtual String ICMS_Aliquota { get; set; }
        public virtual String ICMS_Reducao_Base_Calculo { get; set; }

        public virtual string ISSQN_UF { get; set; }
        public virtual Int64? ISSQN_Municipio { get; set; }
        public virtual Int64? ISSQN_Atividade_Nf { get; set; }
        public virtual Int64? ICMS_Atividade_Nf { get; set; }
        public virtual Int64? ISSQN_CFOP { get; set; }
        public virtual String ISSQN_Aliquota { get; set; }
        public virtual string ISSQN_Natureza_Tributacao { get; set; }
        public virtual String ISSQN_Cod_Tributacao_Municipio { get; set; }

        private List<Configuracao_ICMSListModel> _icms;

        public virtual List<Configuracao_ICMSListModel> ICMS
        {
            get
            {
                if (_icms == null)
                    _icms = new List<Configuracao_ICMSListModel>();
                return _icms;
            }
            set
            {
                _icms = value;
            }
        }

        private List<Configuracao_ISSQNListModel> _issqn;

        public virtual List<Configuracao_ISSQNListModel> ISSQN
        {
            get
            {
                if (_issqn == null)
                    _issqn = new List<Configuracao_ISSQNListModel>();
                return _issqn;
            }
            set
            {
                _issqn = value;
            }
        }

    }

    public class ICMS
    {
        public long Atividade { get; set; }
        public virtual string ICMS_UF { get; set; }
        public virtual Int64? ICMS_Origem_Mercadoria { get; set; }
        public virtual string ICMS_Regime_Tributacao { get; set; }
        public virtual Int64? ICMS_CST_ICMS { get; set; }
        public virtual Int64? ICMS_CSOSN_ICMS { get; set; }
        public virtual Int64? ICMS_CFOP { get; set; }
        public virtual String ICMS_Aliquota { get; set; }
        public virtual String ICMS_Reducao_Base_Calculo { get; set; }
    }

    public class ISSQN
    {
        public virtual string TipoTributacaoISS { get; set; }
        public virtual string ISSQN_UF { get; set; }
        public virtual Int64? ISSQN_Municipio { get; set; }
        public virtual Int64? ISSQN_Atividade_Nf { get; set; }
        public virtual Int64? ICMS_Atividade_Nf { get; set; }
        public virtual Int64? ISSQN_CFOP { get; set; }
        public virtual String ISSQN_Aliquota { get; set; }
        public virtual string ISSQN_Natureza_Tributacao { get; set; }
        public virtual String ISSQN_Cod_Tributacao_Municipio { get; set; }
    }

    public class Configuracao_ISSQNListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Aliquota { get; set; }
        public virtual String Estado { get; set; }
        public virtual Int64? Id_Municipio { get; set; }
        public virtual String Descricao_Municipio { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_CFOP { get; set; }
        public virtual String Desc_CFOP { get; set; }
        public virtual Int64? Id_Atividade { get; set; }
        public virtual String Desc_Atividade { get; set; }
        public virtual Natureza_TributacaoEnum? Natureza_Tributacao { get; set; }
        public virtual String Desc_Natureza_Tributacao { get; set; }
        public virtual TipoTributacaoISS? TipoTributacaoISS { get; set; }
        public virtual String Desc_TipoTributacaoISS { get; set; }
        public virtual String Cod_Tributacao_Municipio { get; set; }
    }

    public class Configuracao_ICMSListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Aliquota { get; set; }
        public virtual String ReducaoBaseCalculo { get; set; }
        public virtual String Estado { get; set; }
        public virtual Int64? Id_CST_ICMS { get; set; }
        public virtual Int64? Id_Origem_Mercadoria { get; set; }
        public virtual Int64? Id_Atividade { get; set; }
        public virtual String Desc_Atividade { get; set; }
        public virtual String Desc_Origem_Mercadoria { get; set; }
        public virtual String Desc_CST_ICMS { get; set; }
        public virtual Int64? Id_CST_CSOSN { get; set; }
        public virtual String Desc_CST_CSOSN { get; set; }
        public virtual Int64? Id_CFOP { get; set; }
        public virtual String Desc_CFOP { get; set; }
        public virtual Regime_TributacaoICMS RegimeTributacao { get; set; }
    }

    public enum Tipo_Tributacao
    {
        ICMS = 1,
        ISSQN = 2,
        Isento = 3,
    }

    public enum UF
    {
        AC = 0,
        AL = 1,
        AM = 2,
        AP = 3,
        BA = 4,
        CE = 5,
        DF = 6,
        ES = 7,
        GO = 8,
        MA = 9,
        MG = 10,
        MS = 11,
        MT = 12,
        PA = 13,
        PB = 14,
        PE = 15,
        PI = 16,
        PR = 17,
        RJ = 18,
        RN = 19,
        RO = 20,
        RR = 21,
        RS = 22,
        SC = 23,
        SE = 24,
        SP = 25,
        TO = 26
    }

    public enum Regime_TributacaoICMS
    {
        Tributacao_Normal = 1,
        Simples_Nacional = 2
    }

    public enum TipoTributacaoISS
    {
        Isento = 1,
        Imune = 2,
        Nao_Incidencia_Municipio = 3,
        Nao_Tributavel = 4,
        Retido = 5,
        Tributavel_Dentro_Municipio = 6,
        Tributavel_Fora_Municipio = 7
    }

    public enum Natureza_TributacaoEnum
    {
        Simples_Nacional = 1,
        Fixo = 2,
        Deposito_Juizo = 3,
        Exigibilidade_Suspensa_Decisao_Judicial = 4,
        Exigibilidade_Suspensa_Procedimento_Adm = 5,
        Isencao_Parcial = 6
    }

    #endregion

    public class Configuracao_TributariaAPIMap : ClassMapBase<Configuracao_Tributaria>
    {
        public Configuracao_TributariaAPIMap() : base()
        {
            Table("TB_CONFIGURACAO_TRIBUTARIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIGURACAO_TRIBUTARIA");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Tipo_Tributacao, "TIPO_TRIBUTACAO").CustomType<Tipo_Tributacao>();

            References(x => x.CFOP, "ID_CFOP").ForeignKey("FK_CONFIG_TRIBUT_CFOP");
            References(x => x.SPED, "ID_SPED").ForeignKey("FK_CONFIG_TRIBUT_SPED");
            Map(x => x.Serie_Nfse, "SERIE_NFSE").Length(9);

            Map(x => x.Retencao_Imposto, "RETENCAO_IMPOSTO");
            Map(x => x.Deduzir_Material, "DEDUZIR_MATERIAL");
            Map(x => x.Deduzir_Valor_Liquido, "DEDUZIR_VALOR_LIQUIDO");
            Map(x => x.Informacoes_NF, "INFORMACOES_NF").Length(5000);

            HasMany(x => x.ICMS).KeyColumn("ID_CONFIG_TRIBUTARIA").Cascade.All().Inverse();

            References(x => x.CST_IPI, "ID_CST_IPI").ForeignKey("FK_CONFIG_TRIBUT_CST_IPI");
            Map(x => x.Aliquota_IPI, "ALIQUOTA_IPI");
            Map(x => x.Reducao_Base_Calculo_IPI, "REDUCAO_BASE_CALCULO_IPI");

            References(x => x.CST_PIS, "ID_CST_PIS").ForeignKey("FK_CONFIG_TRIBUT_CST_PIS");
            Map(x => x.Aliquota_PIS, "ALIQUOTA_PIS");
            Map(x => x.Reducao_Base_Calculo_PIS, "REDUCAO_BASE_CALCULO_PIS");

            References(x => x.CST_COFINS, "ID_CST_COFINS").ForeignKey("FK_CONFIG_TRIBUT_CST_COFINS");
            Map(x => x.Aliquota_COFINS, "ALIQUOTA_COFINS");
            Map(x => x.Reducao_Base_Calculo_COFINS, "REDUCAO_BASE_CALCULO_COFINS");

            HasMany(x => x.ISSQN).KeyColumn("ID_CONFIG_TRIBUTARIA").Cascade.All().Inverse();

            Map(x => x.Aliquota_IRRF, "ALIQUOTA_IRRF");
            Map(x => x.Aliquota_CSLL, "ALIQUOTA_CSLL");
            Map(x => x.Aliquota_INSS, "ALIQUOTA_INSS");
        }
    }
}