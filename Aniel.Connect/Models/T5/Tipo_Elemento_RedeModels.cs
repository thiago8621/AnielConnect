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
using Padrao_Cor = Sinapse.Model.enumerados.Padrao_Cor;

namespace Aniel.Connect.Models.T5
{
    public class Tipo_Elemento_RedeListaModel : BaseModel
    {
        #region Tipo_Elemento_RedeListaModel
        public virtual String Descricao { get; set; }
        public virtual String Icone { get; set; }
        public virtual String Cor { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String Fabricante { get; set; }
        public virtual String E_Cabo { get; set; }
        public virtual String Tipo_Cabo { get; set; }
        public virtual String Espessura { get; set; }
        public virtual Int32? Num_Pares_Fibra { get; set; }
        public virtual Int32? Num_Tubos_Loose { get; set; }
        public virtual String Espessura_linha { get; set; }
        public virtual String Cor_Linha_Ativa { get; set; }
        public virtual String Cor_Linha_Inativa { get; set; }
        public virtual String Estilo_Linha_Ativa { get; set; }
        public virtual String Estilo_Linha_Inativa { get; set; }
        #endregion
    }
    public class Tipo_Elemento_RedeRequest : DataTableRequest
    {
        #region Tipo_Elemento_RedeRequest
        public virtual long? Icone { get; set; }
        public virtual Boolean E_Cabo { get; set; }
        public virtual long? Id_Tipo_Cabo { get; set; }
        public virtual long? Id_Espessura_linha { get; set; }
        public virtual long? Id_Modelo { get; set; }
        public virtual Status_Default? Situacao { get; set; }
        #endregion
    }
    public class Tipo_Elemento_RedeCadastroModel
    {
        #region Tipo_Elemento_RedeCadastroModel
        public virtual String Codigo_Auxiliar { get; set; }

        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int64? Id_Modelo { get; set; }
        public virtual Int64? Id_Icone { get; set; }
        public virtual Int64? Id_Tipo_Cabo { get; set; }
        public virtual Tipo_CaboEnum? TipoCabo { get; set; }
        public virtual String Cor { get; set; }
        public virtual Boolean Situacao { get; set; }
        public virtual String Fabricante { get; set; }
        public virtual Boolean E_Cabo { get; set; }
        public virtual Boolean E_Caixa { get; set; }
        public virtual String Capacidade { get; set; }
        public virtual Padrao_Cor? Padrao_Cor { get; set; }
        public virtual String Espessura { get; set; }
        public virtual String Espessura_linha { get; set; }
        public virtual Int32? Num_Pares_Fibra { get; set; }
        public virtual Int32? Num_Tubos_Loose { get; set; }
        public virtual Int32? Total_Tubos_Loose { get; set; }
        public virtual String Cor_Linha_Ativa { get; set; }
        public virtual String Cor_Linha_Inativa { get; set; }
        public virtual Estilo_LinhaEnum? Estilo_Linha_Ativa { get; set; }
        public virtual Estilo_LinhaEnum? Estilo_Linha_Inativa { get; set; }
        public virtual String Estilo_Linha_Ativa_Demonstracao { get; set; }
        public virtual String Estilo_Linha_Inativa_Demonstracao { get; set; }

        private IEnumerable<TipoElementoMaterial> materiais;

        public virtual IEnumerable<TipoElementoMaterial> Materiais
        {
            get
            {
                if (materiais == null)
                    materiais = new List<TipoElementoMaterial>();
                return materiais;
            }
            set
            {
                materiais = value;
            }
        }

        private IEnumerable<TipoElementoServico> servicos;

        public virtual IEnumerable<TipoElementoServico> Servicos
        {
            get
            {
                if (servicos == null)
                    servicos = new List<TipoElementoServico>();
                return servicos;
            }
            set
            {
                servicos = value;
            }
        }



        #endregion
    }

    public class TipoElementoRedeComboResult : IObjComboResult<Int64>
    {
        #region TipoElementoRedeComboResult
        public Int64 Id { get; set; }
        public String Text { get; set; }
        public String Modelo_Vinculo { get; set; }
        public String capacidade { get; set; }
        public List<ColunaModeloRedeListaModel> Colunas { get; set; }
        #endregion
    }


    public class TipoElementoMaterial
    {
        #region TipoElementoMaterial
        public virtual Int64 Id { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String Descricao { get; set; }
        #endregion
    }

    public class TipoElementoServico
    {
        #region TipoElementoServico
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String UnidadeCusto { get; set; }
        public virtual String Descricao { get; set; }
        #endregion
    }
    public class Tipo_Elemento_RedeTreeViewModel
    {
        #region Tipo_Elemento_RedeTreeViewModel

        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Contrato { get; set; }
        #endregion
    }

    public class Tipo_ElementoIconeViewModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Caminho { get; set; }
        public virtual String Arquivo_Str { get; set; }
        public virtual String Cor { get; set; }
    }
    public class Tipo_ElementoIconesViewModel
    {
        public virtual IEnumerable<Tipo_ElementoIconeViewModel> Files { get; set; }
        public virtual String File_Json { get; set; }
        public String Posted_Arquivo_JSON { get; set; }
        public List<HttpPostedFile> Arquivo { get; set; }


    }
    public class ShowIconesElemento
    {
        public virtual String Dados { get; set; }

    }
    public class ObjComboResultTipoElemento<T> : ObjComboResult<T>
    {
        public virtual String IconePath { get; set; }
        public virtual String IconeCor { get; set; }
        public virtual Boolean E_Cabo { get; set; }
        public virtual String IdModelo { get; set; }
        public String EstiloLinhaAtiva { get; set; }
        public String EstiloLinhaInativa { get; set; }
        public int Espessura { get; set; }
    }



}