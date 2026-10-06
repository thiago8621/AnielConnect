using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aniel.T5.Model.enumerado;

namespace Aniel.Connect.Models.T5
{
    public class Modelo_Elemento_RedeListaModels : BaseModel
    {
        #region Modelo_Elemento_RedeListaModels

        public virtual String Descricao { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Poste { get; set; }
        public virtual String Cabo { get; set; }
        public virtual String Fabricante { get; set; }
        public virtual String Modelo { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Status { get; set; }
        public virtual Int64? IdPON { get; set; }
        public virtual Int64? IdOLT { get; set; }
        public virtual Int64? IdSlot { get; set; }
        public virtual Int64? IdPOP { get; set; }
        public virtual Int32? Capacidade { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Double? Metragem { get; set; }

        #endregion
    }
    public class Modelo_Elemento_RedeRequest : DataTableRequest
    {
        #region Modelo_Elemento_RedeRequest

        public virtual Boolean? Status { get; set; }
        public virtual String Bairro { get; set; }
        public virtual Int64? IdPON { get; set; }
        public virtual Int64? IdOLT { get; set; }
        public virtual Int64? IdSlot { get; set; }
        public virtual Int64? IdPOP { get; set; }
        public virtual Int32? Capacidade { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Double? Metragem { get; set; }
        #endregion
    }

    public class ModeloElementoRedeComboResult : IObjComboResult<Int64>
    {
        #region ModeloElementoRedeComboResult
        public Int64 Id { get; set; }
        public String Text { get; set; }
        public List<ColunaModeloRedeListaModel> Colunas { get; set; }
        #endregion
    }

    public class ColunaModeloRedeListaModel
    {
        #region ColunaModeloRedeListaModel
        public virtual ColunaModeloElementoEnum? Coluna { get; set; }
        public virtual bool Obrigatorio { get; set; }
        public virtual Int64? Id_Dinamico { get; set; }
        #endregion
    }
    public class Modelo_CampoRequest 
    {
        public virtual Int64? Id_ { get; set; }
    }

    public class Modelo_Elemento_RedeCadastroModel
    {
        #region Modelo_Elemento_RedeCadastroModel
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }

        private IEnumerable<Coluna_ElementoRedeModel> colunas;

        public virtual IEnumerable<Coluna_ElementoRedeModel> Colunas
        {
            get
            {
                if (colunas == null)
                    colunas = new List<Coluna_ElementoRedeModel>();
                return colunas;
            }
            set
            {
                colunas = value;
            }
        }

        #endregion
    }

    public class Coluna_ElementoRedeModel
    {
        #region Coluna_ElementoRedeModel
        public virtual Int64 Id { get; set; }
        //public virtual Int64? Tipo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual ColunaModeloElementoEnum? Coluna { get; set; }
        public virtual Int64? IdColunaDinamica { get; set; }
        public virtual bool Obrigatorio { get; set; }
        #endregion
    }
    public class Modelo_Elemento_RedeCampoDinamicoViewModel
    {
        #region Modelo_Elemento_RedeCampoDinamicoViewModel
        public virtual Int64 Id { get; set; }
        public virtual Tipo_CampoModelo Tipo { get; set; }
        private IEnumerable<Modelo_DinamicoCampo> _Lista;
        public virtual IEnumerable<Modelo_DinamicoCampo> Lista
        {
            get
            {
                if (_Lista == null)
                {
                    _Lista = new List<Modelo_DinamicoCampo>();
                }
                return _Lista;
            }
            set
            {
                _Lista = value;
            }
        }
        private IEnumerable<Opcoes_CampoDinamicoView> opcao { get; set; }
        public virtual IEnumerable<Opcoes_CampoDinamicoView> Opcao
        {
            get
            {
                if (opcao == null)
                {
                    opcao = new List<Opcoes_CampoDinamicoView>();
                }
                return opcao;
            }
            set
            {
                opcao = value;
            }
        }
      
        #endregion
    }
    public class Modelo_DinamicoCampo
    {
        #region Modelo_DinamicoCampo
        public virtual Int64 Id { get; set; }
        public virtual Tipo_CampoModelo Tipo { get; set; }
        public virtual String TipoStr { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Boolean Gravado { get; set; }
        private IEnumerable<Opcoes_CampoDinamicoView> opcoes { get; set; }
        public virtual IEnumerable<Opcoes_CampoDinamicoView> Opcoes {
            get
            {
                if (opcoes == null) 
                {
                    opcoes = new List<Opcoes_CampoDinamicoView>();
                }
                return opcoes;
            }
            set 
            {
                opcoes = value;
            }
        }
        public virtual String OpcoesStr { get; set; }
        
        #endregion
    }
    public class Opcoes_CampoDinamicoView
    {
        #region Opcoes_CampoDinamicoView

        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Campo { get; set; }        
        public virtual String Resposta { get; set; }
        #endregion
    }  
    public class CampoDinamicoView
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        //public virtual String Tipo_Campo { get; set; }
    }
}