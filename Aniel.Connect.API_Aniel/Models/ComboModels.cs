using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ComboListModel
    {
        public Int64 Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public bool EmiteContrato { get; set; }
        public string ModeloContrato { get; set; }
        public bool ModeloTemCarencia { get; set; }
        public string PeriodoCarencia { get; set; }
        public List<PlanoProdutoComboList> ComboPlanoProduto { get; set; }
    }  
    public class RequestCombo : ApiRequest
    {
        public Int64? Id { get; set; }
        public String CodigoIntegracao { get; set; }
    }

    public class FindByIdComboApiRequest : IApiValidate
    {
        public Int64 id { get; set; }
        /// <summary> Usuário da API </summary>
        [Required]
        public string user { get; set; }
        /// <summary> Senha do Usuário </summary>
        [Required]
        public string password { get; set; }
        /// <summary> Token da API </summary>
        [Required]
        public string token { get; set; }
        
        public string codigoIntegracao { get; set; }
    }

    public class PlanoProdutoComboList
    {
        public long Id { get; set; }
        public string PlanoProduto { get; set; }
        public string Tipo { get; set; }
        public string ValorReal { get; set; }
        public string ValorNoCombo { get; set; }
        public bool CobrarInstalacao { get; set; }
        public string ValorIntalacaoReal { get; set; }
        public string ValorInstalacaoCombo { get; set; }
       
    }


    public class ComboDetalheModel
    {
        private IEnumerable<ComboDetalhePlanoProdutoModel> _listPlanos = null;

        public string Valor_Combo { get; set; }
        public string Valor_Instalacao { get; set; }
        public string Detalhes { get; set; }
        public long? Numero_Parcelas { get; set; }
        public string Tem_Carencia { get; set; }
        public long? Periodo_Carencia { get; set; }
        public string Multa_Carencia_Reais { get; set; }
        public string Cancelar_Contrato_Automatico_Fim_Carencia { get; set; }
        public IEnumerable<ComboDetalhePlanoProdutoModel> ListaModelos
        {
            get
            {
                if (this._listPlanos == null)
                    this._listPlanos = new List<ComboDetalhePlanoProdutoModel>();

                return this._listPlanos;
            }
            set
            {
                this._listPlanos = value;
            }
        }
    }


    public class ComboDetalhePlanoProdutoModel
    {
        public string Descricao { get; set; }
        public string PlanoProduto { get; set; }
        public string Franquia { get; set; }
        public string Ponto { get; set; }
        public string Velocidade { get; set; }
        public string Frequencia_Cobranca { get; set; }
        public string Formato_Cobranca { get; set; }
        public string Detalhe { get; set; }
    }

}