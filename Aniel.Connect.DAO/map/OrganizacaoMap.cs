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
    public class OrganizacaoMap : ClassMapBase<Organizacao>
    {
        public OrganizacaoMap() : base()
        {
            Table("TB_ORGANIZACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ORGANIZACAO");

            Map(x => x.Razao_Social, "RAZAO_SOCIAL");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Classificacao_Organizacao, "CLASSIFICACAO_ORGANIZACAO").CustomType<Classificacao_Organizacao>();
            Map(x => x.Nome_Fantasia, "NOME_FANTASIA");
            Map(x => x.CNPJ, "CNPJ");
            Map(x => x.Cep, "CEP");
            Map(x => x.Complemento, "COMPLEMENTO");
            Map(x => x.Id_Bairro, "ID_BAIRRO");
            Map(x => x.Id_Cidade, "ID_CIDADE");
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Numero, "NUMERO");
            Map(x => x.Logradouro, "LOGRADOURO");
            Map(x => x.Uf, "UF");
            HasMany(x => x.Dominios).KeyColumn("ID_ORGANIZACAO").Cascade.All().Inverse();
            HasMany(x => x.Contatos).KeyColumn("ID_ORGANIZACAO").Cascade.All().Inverse();
        }
    }
}
