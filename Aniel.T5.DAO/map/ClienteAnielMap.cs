using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ClienteAnielMap : ClassMap<ClienteAniel>
    {
        public ClienteAnielMap()
        {
            Table("TB_CLIENTE");

            Id(u => u.Cod_Cliente, "COD_CLIENTE").GeneratedBy.Assigned();

            Map(x => x.NomeFantasia, "RAZAO_SOCIAL").Length(100).Index("IDX_CLIENTE_RAZAO_CNPJ");
            Map(x => x.CPF, "CNPJ").Length(18).Index("IDX_CLIENTE_RAZAO_CNPJ");
            Map(x => x.Status, "STATUS").CustomType<Status_Cliente>();

            Map(x => x.Telefone_Fixo, "TELEFONE").Length(18);
            Map(x => x.Telefone_Celular, "TELEFONE2").Length(18);

            Map(x => x.Email_Principal, "EMAIL").Length(50);

            Map(x => x.Id_Integrador, "ID_INTEGRADOR").Length(255);
            Map(x => x.Data_Cadastro, "DATA_CADASTRO");

            References(x => x.Categoria_Cliente, "ID_CATEGORIA").ForeignKey("FK_CLIENTE_CATEGORIA");

            Map(x => x.Cep, "CEP").Length(10);

            Map(x => x.Bairro, "BAIRRO").Length(100);

            Map(x => x.UF, "UF").Length(2);

            Map(x => x.Cidade, "COD_MUNICIPIO");

            Map(x => x.Numero, "ENDERECO_NUMERO").Length(10);

            Map(x => x.Logradouro, "ENDERECO").Length(100);

            Map(x => x.Tipo, "TIPO").Length(1);
            
            Map(x => x.Tipo_Pessoa, "TIPOPESSOA").CustomType<Tipo_Pessoa>();
            Map(x => x.Id_Organizacao, "ID_ORGANIZACAO");

            Map(x => x.Tipo_Cliente, "TIPO_CLIENTE").CustomType<Tipo_ClienteAniel>();

            Map(x => x.Status_Prospecto, "STATUS_PROSPECTO").CustomType<Status_ProspectoAniel>();
            Map(x => x.Numero_Contrato, "NUMERO_CONTRATO");
        }
    }
}
