
using NetTools;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pool_Ip : BaseModel
    {
        private IList<Pool_Ip_Endereco> _ips;
        public virtual String Descricao { get; set; }
        public virtual string Nome_Pool_Concentradora { get; set; }
        public virtual String Ip_Inicio_Range { get; set; }
        public virtual String Ip_Fim_Range { get; set; }
        //public virtual String Ip_Mascara { get; set; }

        public virtual Boolean PoolPadraoConcentradora { get; set; }
        public virtual Boolean IsPoolBloqueio { get; set; }
        public virtual Boolean IsPoolConcentradora { get; set; }
        public virtual Boolean IsDistribuicaoBlocoIp { get; set; }
        public virtual Boolean IsPoolIpFixo { get; set; }
        public virtual Boolean IsPoolReducaoVelocidade { get; set; }

        public virtual String Mascara_CIDR { get; set; }
        public virtual Status_Default? Status { get; set; }

        public virtual Tipo_Ip Padrao_Ip {get; set;}

        public virtual Radius Radius { get; set; }

        public virtual Concentradora Concentradora { get; set; }
        
        public virtual IList<Pool_Ip_Endereco> Pool_Ip_Enderecos
        {
            get
            {
                if (_ips == null)
                    _ips = new List<Pool_Ip_Endereco>();
                return _ips;
            }
            set
            {
                _ips = value;
            }
        }

        public virtual IPAddressRange RangeIp { 
            get
            {
                if (!String.IsNullOrEmpty(Mascara_CIDR)) //Cálculo do range por máscara
                {
                    var network = IPNetwork.Parse(String.Format("{0}/{1}", Ip_Inicio_Range, Mascara_CIDR));
                    return new IPAddressRange(network.FirstUsable, network.LastUsable);
                }
                else
                {
                    return new IPAddressRange(IPAddress.Parse(Ip_Inicio_Range), IPAddress.Parse(Ip_Fim_Range));
                }
            }
            set
            {
                this.RangeIp = RangeIp;
            }
        }
    }
}
