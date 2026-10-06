using System;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class ComboListModel
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
    }

    public class ComboCadastroModel
    {
        private IEnumerable<ComboPlanoProdutoCadastroModel> _listPlanoProduto = null;

        public long Id { get; set; }
        public string Descricao { get; set; }
        public string DetalhesCombo { get; set; }
        public string CodigoAuxiliar { get; set; }
        public string CodigoIntegracaoAuxiliar { get; set; }
        public string CodigoTipoOS { get; set; }
        public bool Status { get; set; }
        public bool EmitirContrato { get; set; }
        public long? IdModeloContrato { get; set; }
        public IEnumerable<ComboPlanoProdutoCadastroModel> ListPlanoProduto
        {
            get
            {
                if (this._listPlanoProduto == null)
                    this._listPlanoProduto = new List<ComboPlanoProdutoCadastroModel>();

                return this._listPlanoProduto;
            }
            set
            {
                this._listPlanoProduto = value;
            }
        }
    }

    public class ComboPlanoProdutoCadastroModel
    {
        public long Id { get; set; }

        /// <summary>
        ///     ID do registro da TB_PLANO associado ao combo caso o registro
        ///     seja referente a um plano
        /// </summary>
        public long? IdPlano { get; set; }
        /// <summary>
        ///     ID do registro da TB_PRODUTO associado ao combo caso o registro
        ///     seja referente a um plano
        /// </summary>
        public long? IdProduto { get; set; }

        public string DescricaoPlanoProduto { get; set; }
        public string Tipo { get; set; }

        public string ValorReal { get; set; }
        public string ValorCombo { get; set; }

        public bool CobraInstalacao { get; set; }

        public string ValorRealInstalacao { get; set; }
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

    public class RemoverPlanoComboRequest
    {
        public long IdComboPlanoProduto { get; set; }
    }

    public class PlanoVinculadoC_ComboModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Int64? Id_Modelo_Contrato { get; set; }
        public virtual Boolean Emite_Contrato { get; set; }
        public virtual Double? Valor { get; set; }
        public virtual Boolean Cobrar_Instalacao { get; set; }
        public virtual Double? Valor_Instalacao { get; set; }
        public virtual Int64? Id_Plano { get; set; }
    }

    public class AreaCoberturaCombo : ObjComboResult<long>
    {
        public string Id_area_cobertura { get; set; }       

    }
}