using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Models.T5
{

    public class Area_CoberturaListModel
    {
        #region Area_CoberturaListModel
        public virtual Int64? Id { get; set; }
        public virtual String Status { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int64? Id_Plano { get; set; }
        public virtual String Codigo_PlanoProduto { get; set; }
        public virtual String Descricao_Plano { get; set; } 
        #endregion
    }

    public class Area_CoberturaCadastroModel
    {
        #region Area_CoberturaCadastroModel
        public virtual Int64 Id { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Cor { get; set; } 
       


        private IEnumerable<Area_Cobertura_Plano_ProdutoModel> produtos { get; set; }
        private IEnumerable<Area_Cobertura_ComboModel> combos { get; set; }

        public virtual IEnumerable<Area_Cobertura_Plano_ProdutoModel> Produtos
        {
            get
            {
                if (produtos == null)
                {
                    produtos = new List<Area_Cobertura_Plano_ProdutoModel>();

                }
                return produtos;
            }
            set { produtos = value; }
        }
        public virtual IEnumerable<Area_Cobertura_ComboModel> Combos
        {
            get
            {
                if (combos == null)
                {
                    combos = new List<Area_Cobertura_ComboModel>();

                }
                return combos;
            }
            set { combos = value; }
        }

        #endregion
    }

    /// <summary>
    /// Classe base para virar Lista
    /// </summary>
    public class Area_Cobertura_Plano_ProdutoModel
    {
        #region Area_Cobertura_Plano_ProdutoModel
        public virtual Int64? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Plano_produto { get; set; } 
        #endregion
    }
    public class Area_Cobertura_ComboModel
    {
        #region Area_Cobertura_ComboModel
        public virtual Int64? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Combo { get; set; } 
        #endregion
    }


    public class Area_CoberturaDataTableRequest : DataTableRequest
    {
        public virtual Status_Default? Status { get; set; }
    }

    public class Mapa_Area_CoberturaCadastroModel
    {
        public long Id { get; set; }

        //[Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo { get; set; }

        //[Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao { get; set; }

        public string Tipo { get; set; }

        public string Situacao { get; set; }

        public string Detalhes { get; set; }


    }

    public class Mapa_Area_CoberturaModel
    {
        #region Mapa_Area_CoberturaModel

        public virtual Int64 Id { get; set; }
        public virtual String Cor { get; set; }
        public virtual String Coordenadas { get; set; }
        public virtual Tipo_Poligono Tipo { get; set; }

        /*public virtual List<Double?> CoordenadasDouble
        {
            get
            {
                return Json.Decode<List<Double?>>(Coordenadas);
            }
            set
            {
                Coordenadas = Json.Encode(value);
            }
        }*/

        public virtual List<List<Double?>> ListCoordenadas
        {
            get
            {
                List<List<Double?>> CoordenadasDouble2 = new List<List<Double?>>();
                //foreach (String coordenada in Coordenadas)
                //CoordenadasDouble2.Add(Json.Decode<List<Double?>>(coordenada));
                CoordenadasDouble2 = Json.Decode<List<List<Double?>>>(Coordenadas);
                return CoordenadasDouble2;
            }
            set
            {
                Coordenadas = Json.Encode(value);
                //foreach (List<Double?> coordenada in value)
                    //Coordenadas.Add(Json.Encode(coordenada));
            }
        }
        //public virtual Double[,] CoordenadasDouble
        //{
        //    get
        //    {
        //        return Json.Decode<Double[,]>(Coordenadas);
        //    }
        //    set
        //    {
        //        Coordenadas = Json.Encode(value);
        //    }
        //}
        public virtual Int64? Id_Plano { get; set; }
        public virtual String Codigo_PlanoProduto { get; set; }
        public virtual String Descricao_Plano { get; set; } 
        #endregion
    }
    public class Area_CoberturaTreeViewModel
    {
        #region Area_CoberturaTreeViewModel
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; } 
        #endregion
    }
    public class AreaCoberturaRequestIds 
    {
        private IList<long> _Ids;
        public IList<long> Ids { 
            get 
            { 
                if (_Ids == null) 
                { 
                    return new List<long>();
                } 
                else 
                {
                    return _Ids;
                }
            } 
            set { _Ids = value; }
        }
    }
}