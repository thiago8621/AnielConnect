using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    class FornecedorMap : ClassMap<Fornecedor>
    {
        public FornecedorMap()
        {
            Table("TB_FORNECEDOR");

            Id(x => x.Cod_Fornecedor, "COD_FORNECEDOR").Length(6);
            Map(x => x.Bairro, "BAIRRO");
            Map(x => x.fornecedor, "FORNECEDOR");
            Map(x => x.Cep, "CEP");
            Map(x => x.Cnpj, "CNPJ");
            Map(x => x.Complemento, "COMPLEMENTO");
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Email, "EMAIL");
            Map(x => x.Endereco, "ENDERECO");
            Map(x => x.Insc_Estadual, "INSC_ESTADUAL");
            Map(x => x.Municipio, "MUNICIPIO");
            Map(x => x.Numero, "NUMERO");
            Map(x => x.Pais, "PAIS");
            Map(x => x.RazaoSocial, "RAZAO_SOCIAL");
            Map(x => x.Telefone, "TELEFONE");
            Map(x => x.Uf, "UF");





        }
    }
}
