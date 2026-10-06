using Aniel.Connect.Repository.Model;
using Sinapse.Model;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models.T5
{
    public class ExecutarConsultaViabilidadeRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long? IdVenda { get; set; }
        public long? IdClientePlanoProduto { get; set; }
        public long? IdCliente { get; set; }
        public long? IdEnderecoLogradouro { get; set; }
        public long? IdCondominio { get; set; }
        public string Numero { get; set; }
        public string Logradouro { get; set; }
        public long? IdCidade { get; set; }
        public long? IdBloco { get; set; }
        public long? IdApto_Numero { get; set; }
    }

    public class Consulta_ViabilidadeModel
    {
        private IEnumerable<Consulta_ViabilidadeCaixa> caixas { get; set; }
        public virtual IEnumerable<Consulta_ViabilidadeCaixa> Caixas
        {
            get
            {
                if (caixas == null)
                {
                    caixas = new List<Consulta_ViabilidadeCaixa>();

                }
                return caixas;
            }
            set
            {
                caixas = value;
            }
        }

        private IEnumerable<Consulta_ViabilidadePlanoProduto> planosProdutos { get; set; }
        public virtual IEnumerable<Consulta_ViabilidadePlanoProduto> PlanosProdutos
        {
            get
            {
                if (planosProdutos == null)
                {
                    planosProdutos = new List<Consulta_ViabilidadePlanoProduto>();

                }
                return planosProdutos;
            }
            set
            {
                planosProdutos = value;
            }
        }

    }
    public class Consulta_ViabilidadeRequestModel
    {
        #region Consulta_ViabilidadeRequestModel
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        public Int64? IdVenda { get; set; }
        public Int64? IdCombo { get; set; }
        public Int64? IdCampanha { get; set; }
        public long? Cliente_PlanoProduto { get; set; }
        #endregion
    }
    public class Consulta_ViabilidadeResponseModel
    {
        private List<Consulta_ViabilidadeCaixa> caixas { get; set; }
        public List<Consulta_ViabilidadeCaixa> Caixas_Viaveis
        {
            get
            {
                return caixas!=null? caixas : new List<Consulta_ViabilidadeCaixa>();
            }
            set
            {
                caixas = value;
            }
        }
        public Motivo_InviabilidadeCadastroModel Inviabilidade { get; set; }       
    }

    public class Consulta_ViabilidadePlanoProduto
    {
        public virtual Int64? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Valor { get; set; }
    }
    public class JsonObj
    {
        public string ObjJson { get; set; }
        public string GeoCliente { get; set; }
        public string Plano { get; set; }
        public string PoligonoBanco { get; set; }
    }
    public class ViabilidadeViewModel
    {
        private List<Consulta_ViabilidadeCaixa> _CaixasViabilidade { get; set; }
        public List<Consulta_ViabilidadeCaixa> CaixasViabilidade 
        {
            get
            {
                if (_CaixasViabilidade == null)
                {
                    return new List<Consulta_ViabilidadeCaixa>();
                }
                else
                {
                    return _CaixasViabilidade;
                }
            }
            set
            {
                _CaixasViabilidade = value;
            }
        }
        private List<Double> _PosicaoCliente { get; set; }
        public List<Double> PosicaoCliente 
        {
            get 
            {
                if (_PosicaoCliente == null) { 
                    return new List<Double>();
                }
                else 
                {
                    return _PosicaoCliente;
                } 
            }
            set 
            { 
                _PosicaoCliente = value;
            }
        }
    }
    public class ConsultaViabilidadeDadosViewModel 
    {
        private IList<ConsultaViabilidadePortaDisponivel> _portasDisponiveis;

        public virtual ConsultaViabilidade Consulta { get; set; }
        public virtual long? Id_Elemento_Rede { get; set; }
        public virtual bool PortaReservada { get; set; }
        /// <summary>
        ///     Distância em quilômetros da latitude e longitude do endereço de instalação para
        ///     a latitude e longitude da caixa
        /// </summary>
        public virtual double Distancia { get; set; }
        public virtual IList<ConsultaViabilidadePortaDisponivel> PortasDisponiveis
        {
            get
            {
                if (this._portasDisponiveis == null)
                    this._portasDisponiveis = new List<ConsultaViabilidadePortaDisponivel>();

                return this._portasDisponiveis;
            }

            set
            {
                this._portasDisponiveis = value;
            }
        }
    }

    public class GravarMotivoAprovacaoManualRequest
    {
        public long IdVenda { get; set; }
        public long IdEtapaVenda { get; set; }

        public string MotivoAprovacao { get; set; }
        public string MotivoAprovManualESC { get; set; }

    }

    public class RegrasExibicaoCTOModel
    {
        public bool ExibirApenasCaixasDentroRaio { get; set; }
        public int? LimiteDistanciaCaixa { get; set; }
    }
}