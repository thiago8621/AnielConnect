using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Pool_IpMap : ClassMapBase<Pool_Ip>
    {

        public Pool_IpMap() : base()
        {
            Table("TB_POOL_IP");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_POOL_IP");

            Map(x => x.Descricao , "DESCRICAO");
            Map(x => x.Nome_Pool_Concentradora, "NOME_POOL_CONCENTRADORA");
            Map(x => x.Ip_Inicio_Range, "IP_INICIO_RANGE");
            Map(x => x.Ip_Fim_Range, "IP_FIM_RANGE");
            //Map(x => x.Ip_Mascara, "IP_MASCARA");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.PoolPadraoConcentradora, "POOL_PADRAO_CONCENTRADORA");
            Map(x => x.IsPoolBloqueio, "POOL_BLOQUEIO_SINAL");
            Map(x => x.IsPoolConcentradora, "CONTROLE_POOL_CONCENTRADORA");
            Map(x => x.IsPoolIpFixo, "POOL_IP_FIXO");
            Map(x => x.IsDistribuicaoBlocoIp, "DISTRIBUICAO_BLOCO_IP");
            Map(x => x.IsPoolReducaoVelocidade, "POOL_REDUCAO_VELOCIDADE");
            Map(x => x.Mascara_CIDR, "MASCARA_IP_CIDR");
            Map(x => x.Padrao_Ip, "PADRAO_IP").CustomType<Tipo_Ip>();
            HasMany(x => x.Pool_Ip_Enderecos).KeyColumn("ID_POOL_IP").Cascade.All().Inverse();

            References(p => p.Radius, "ID_RADIUS");
            References(p => p.Concentradora, "ID_CONCENTRADORA");

        }

    }
}
