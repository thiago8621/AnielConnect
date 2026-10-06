using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    //elementos carregados na pagina index
    public class CampanhaListaModel
    {
        public virtual Int64? Id { get; set; }
        public virtual String Titulo { get; set; }
        public virtual String Data_Inicio { get; set; }
        public virtual String Data_Final { get; set; }
        public virtual Int64? Meta { get; set; }

        public virtual String Definir_Campanha_Regioes { get; set; }
        public virtual String Definir_Area_Cobertura { get; set; }
        public virtual String Camp_Tempo_Determinado { get; set; }


        public virtual String Status { get; set; }

    }

    public class CampanhaListaModelEquipeVendas
    {
        public virtual long Id { get; set; }
        public virtual String Setor_Departamento { get; set; }
        public virtual String Setor_Departamento_Desc { get; set; }
        public virtual String Id_Funcionario { get; set; }
        public virtual String Id_Supervisor { get; set; }
        public virtual String Id_Funcionario_Desc { get; set; }
        public virtual String Id_Supervisor_Desc { get; set; }
        public virtual String Meta { get; set; }

    }

    public class CampanhaListaModelRegioes
    {
        public virtual Int64 Id { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Estado { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Pais_Desc { get; set; }
        public virtual String Estado_Desc { get; set; }
        public virtual String Cidade_Desc { get; set; }
        public virtual Boolean CampanhaRegioes { get; set; }
    }

    public class CampanhaListaModelArea_Cobertura
    {
        public virtual Int64 Id { get; set; }
        public virtual Boolean Definir_Area_Cobertura { get; set; }
        public virtual Int64? Area_Cobertura { get; set; }
        public virtual String Area_Cobertura_Desc { get; set; }

    }
    public class CampanhaByVendendor_Mobile
    {
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public Int64? Id_Vendedor { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }



    //elementos carregados na pagina cadastro / alteracao
    public class CampanhaCadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual String Titulo { get; set; }


        [Display(Name = "DATAINICIAL", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual DateTime? Data_Inicio { get; set; }


        [Display(Name = "DATAFINAL", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual DateTime? Data_Final { get; set; }

        public virtual String Meta { get; set; }

        public virtual Boolean Status { get; set; }



        public virtual String Setor_Departamento { get; set; }
        public virtual Int64? Id_Depto { get; set; }
        public virtual Int64? Id_Funcionario { get; set; }
        public virtual Int64? Id_Supervisor { get; set; }
        public virtual Int64? Id_Plano_Produto { get; set; }
        public virtual Int64? Id_Meio_Captacao { get; set; }

        public virtual bool Camp_Tempo_Determinado { get; set; }
        public CampanhaUtilizadaPor? CampanhaUtilizadaPor { get; set; }
        public string PrefixoCupom {get;set;}
        public virtual Boolean Definir_CampanhaRegioes { get; set; }
        public virtual Boolean Definir_CampanhaEquipeVenda { get; set; }
        public virtual Boolean Definir_Area_Cobertura { get; set; }
        public virtual String Area_Cobertura { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Estado { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Novo_Nome_Plano_Produto { get; set; }
        public virtual String Valor_Plano_Produto { get; set; }
        public virtual Double Valor_Instalacao { get; set; }
        public virtual Atividade_Decisoria Validade_Plano_Produto { get; set; }
        public virtual DateTime? Data_Validade { get; set; }
        public virtual String Parcela_Instalacao { get; set; }


        private List<Campanha_Plano_ProdutoListModel> PlanoProdutoLista;

        public virtual List<Campanha_Plano_ProdutoListModel> PlanosProdutos

        {
            get
            {
                if (PlanoProdutoLista == null)
                    PlanoProdutoLista = new List<Campanha_Plano_ProdutoListModel>();
                return PlanoProdutoLista;
            }
            set
            {
                PlanoProdutoLista = value;
            }
        }
        private List<CampanhaListaModelEquipeVendas> EquipeVendasLista;

        public virtual List<CampanhaListaModelEquipeVendas> EquipeVendas

        {
            get
            {
                if (EquipeVendasLista == null)
                    EquipeVendasLista = new List<CampanhaListaModelEquipeVendas>();
                return EquipeVendasLista;
            }
            set
            {
                EquipeVendasLista = value;
            }
        }

        private List<CampanhaListaModelRegioes> RegioesLista;

        public virtual List<CampanhaListaModelRegioes> Regioes

        {
            get
            {
                if (RegioesLista == null)
                    RegioesLista = new List<CampanhaListaModelRegioes>();
                return RegioesLista;
            }
            set
            {
                RegioesLista = value;
            }
        }

        private List<CampanhaListaModelArea_Cobertura> AreaCoberturaLista;

        public virtual List<CampanhaListaModelArea_Cobertura> AreaCobertura

        {
            get
            {
                if (AreaCoberturaLista == null)
                    AreaCoberturaLista = new List<CampanhaListaModelArea_Cobertura>();
                return AreaCoberturaLista;
            }
            set
            {
                AreaCoberturaLista = value;
            }
        }
    }

    public class Campanha_Plano_ProdutoListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual long? Plano { get; set; }

        public virtual long? Id_Plano_Produto { get; set; }
        public virtual String Id_Plano_Produto_Desc { get; set; }
        public virtual String Validade_Plano_Produto { get; set; }
        public virtual String Validade_Plano_Produto_Desc { get; set; }
        public virtual String Data_Validade { get; set; }
        public virtual String Plano_Desc { get; set; }
        public virtual PlanoProdutoEnum? IsPlanProd { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Campanha Campanha { get; set; }
        public virtual String Novo_Nome_Plano_Produto { get; set; }
        public virtual String Valor_Plano_Produto { get; set; }
        public virtual String Valor_Instalacao { get; set; }
        public virtual String Parcela_Instalacao { get; set; }



    }
    public class Campanha_DataTableRequest : DataTableRequest
    {
        public bool? CampanhaTempoDeterminado { get; set; }
        public bool? EquipeVendas { get; set; }
        public bool? Regioes { get; set; }
        public bool? AreaCobertura { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
    }

    public class Campanha_Request_ObjComboVenda
    {
        public virtual bool Acesso_EquipeVendas { get; set; }
        public virtual Camp_Equipe_VendasRequestModel ObjEquipeVendas { get; set; } = new Camp_Equipe_VendasRequestModel();
        public virtual bool Acesso_Regiao { get; set; }
        public virtual Camp_RegioesRequestModel ObjRegioes { get; set; } = new Camp_RegioesRequestModel();
        public virtual bool Acesso_Area_Cobertura { get; set; }
        public virtual Camp_Area_CoberturaRequestModel ObjArea_Cobertura { get; set; } = new Camp_Area_CoberturaRequestModel();
        public virtual bool Campanha_com_Prazo { get; set; }
        public virtual string idPlanoProduto { get; set; }
    }
    public class Camp_Equipe_VendasRequestModel
    {
        public virtual string Id_Vendedor { get; set; }
        public virtual string Id_Supervisor { get; set; }
        public virtual string Id_Setor_Departamento { get; set; }
    }
    public class Camp_Area_CoberturaRequestModel
    {
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
    }
    public class Camp_RegioesRequestModel
    {
        public virtual string Cidade { get; set; }
        public virtual string Pais { get; set; }
        public virtual string Uf { get; set; }
    }

    public class ObjCampanhaResultado : ObjComboResult<Int64>
    {
        public virtual List<String> Pais { get; set; } = new List<string>();
        public virtual List<String> UF { get; set; } = new List<string>();
        public virtual List<String> Cidade { get; set; } = new List<string>();
        public virtual List<String> Id_Supervisor { get; set; } = new List<string>();
        public virtual List<String> Id_Vendedor { get; set; } = new List<string>();
        public virtual List<String> Id_Departamento { get; set; } = new List<string>();
        public virtual List<long> Id_Plano_Produto { get; set; } = new List<long>();
        public virtual List<long> Id_Campanha_Plano { get; set; } = new List<long>();
        public virtual List<String> Data_Validade { get; set; } = new List<string>();
        public virtual List<String> Novo_Nome_Plano_Produto { get; set; } = new List<string>();
        public virtual List<String> Valor_Plano_Produto { get; set; } = new List<string>();
        public virtual List<String> Valor_Instalacao { get; set; } = new List<string>();
        public virtual List<long?> Id_AreaCobertura { get; set; } = new List<long?>();
        //public virtual bool PointInPolygon { get; set; }
    }

}