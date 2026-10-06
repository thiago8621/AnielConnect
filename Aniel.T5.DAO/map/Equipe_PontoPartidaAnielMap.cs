using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Equipe_PontoPartidaAnielMap : ClassMapBase<Equipe_PontoPartidaAniel>
    {
        public Equipe_PontoPartidaAnielMap() : base()
        {
            Table("TB_EQUIPE_PONTOPARTIDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_EQUIPE_PONTOPARTIDA");

            Map(x => x.Descricao, "DESCRICAO").Length(40);

            Map(x => x.CEP, "CEP").Length(10);
            Map(x => x.Logradouro, "LOGRADOURO").Length(50);
            Map(x => x.Bairro, "BAIRRO").Length(30);

            Map(x => x.Cidade, "CIDADE").Length(30);
            Map(x => x.Numero, "NUMERO").Length(10);
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");

        }
    }
}
