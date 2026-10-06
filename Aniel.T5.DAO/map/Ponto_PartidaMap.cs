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
    public class Ponto_PartidaMap : ClassMapBase<Ponto_Partida>
    {
        public Ponto_PartidaMap()
        {
            Table("TB_PONTO_PARTIDA");

            //Id(x => x.Matricula, "EQUIPE");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PONTO_PARTIDA");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Cep, "CEP");
            Map(x => x.Logradouro, "LOGRADOURO");
            Map(x => x.Numero, "NUMERO");
            Map(x => x.Bairro, "BAIRRO");
            Map(x => x.Cidade, "CIDADE");
            Map(x => x.Complemento, "COMPLEMENTO");
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONFITUDE");
            Map(x => x.Uf, "UF");
            Map(x => x.Id_Bairro, "ID_BAIRRO");
            Map(x => x.Id_Cidade, "ID_CIDADE");


            HasMany(x => x.Equipes).KeyColumn("ID_PONTO_PARTIDA").Cascade.All().Inverse();

            //Map(x => x.Connect, "CONNECT").CustomSqlType("Char(1)").Default("N");
            
        }
    }
}
