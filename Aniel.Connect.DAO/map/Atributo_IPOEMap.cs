using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Atributo_IPOEMap : ClassMapBase<Atributo_IPOE>
    {
        public Atributo_IPOEMap() : base()
        {
            Table("TB_ATRIBUTO_IPOE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATRIBUTO_IPOE");
            Map(u => u.Nome, "NOME");
            Map(u => u.Atributo_Autenticacao, "ATRIBUTO_AUTENTICACAO").Length(999);
            Map(u => u.Atributo_Bloqueio, "ATRIBUTO_BLOQUEIO").Length(999);
            Map(u => u.Atributo_ReducaoVelocidade, "ATRIBUTO_REDUCAO_VELOCIDADE").Length(999);
            Map(u => u.Atributo_VelocidadeNormal, "ATRIBUTO_VELOCIDADE_NORMAL").Length(999);

        }
    }
}
