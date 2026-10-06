using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Pool_IpListaModel
    {
        public virtual Int64  Id { get; set; }
        public virtual String Descricao { get; set; }
        
        public virtual String Ip_Inicio_Range { get; set; }
        public virtual String Ip_Fim_Range { get; set; }
        public virtual String Ip_Mascara { get; set; }     
        public virtual Tipo_Ip? Tipo_Ip { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual String Status { get; set; }

    }


    public class Pool_IpCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
        public virtual string Nome_Pool_Concentradora { get; set; }
        public virtual bool IsPoolBloqueio { get; set; }
        public virtual String Ip_Inicio_Range { get; set; }
        public virtual String Ip_Fim_Range { get; set; }
        public virtual String CIDR { get; set; }

        //public virtual String Ip_Mascara { get; set; }
        public virtual Int64 IdRadius { get; set; }
        public virtual long? IdConcentradora { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Tipo_Ip PadraoIp { get; set; }
        public virtual Boolean PoolPadraoConcentradora { get; set; }
        public virtual Boolean IsPoolConcentradora { get; set; }
        public virtual Boolean IsDistribuicaoBlocoIp { get; set; }
        public virtual Boolean IsPoolIpFixo { get; set; }
        public virtual Boolean IsPoolReducaoVelocidade { get; set; }

        private List<Pool_Ip_EnderecoModels> Pool_Ip_EnderecosLista;

        public virtual List<Pool_Ip_EnderecoModels> Pool_Ip_Enderecos
        {
            get
            {
                if (Pool_Ip_EnderecosLista == null)
                    Pool_Ip_EnderecosLista = new List<Pool_Ip_EnderecoModels>();
                return Pool_Ip_EnderecosLista;
            }
            set
            {
                Pool_Ip_EnderecosLista = value;
            }
        }


    }

    public class Pool_IpEdicaoModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao_Antiga { get; set; }
        public virtual String Descricao_Nova { get; set; }
        public virtual String Ip_Inicio_Range { get; set; }
        public virtual String Ip_Fim_Range { get; set; }
        
        public virtual Int64 IdRadius { get; set; }
        public virtual Boolean Status { get; set; }
    }

    public class Pool_Ip_DataTableRequest : DataTableRequest
    {
        public virtual String Radius { get; set; }
        public virtual String Concentradora { get; set; }
        public virtual String Tipo_Ip { get; set; }
        public virtual String Status { get; set; }
    }

    public class RequestComboIpPoolDisponiveis<Y,T> : RequestObjComboPai<Y,T>
    {
        public ComboIpPoolDisponiveisParams Config { get; set; }
    }

    public class ComboIpPoolDisponiveisParams
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int64 Cod_Localidade { get; set; }
    }

    public class ObjComboIpPool<T> : ObjComboResult<T>
    {
        public virtual bool Is_Pool_Radius { get; set; }
        public virtual bool IsPoolIpFixo { get; set; }
    }
}