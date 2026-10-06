using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class ClienteMap : ClassMapBase<Cliente>
    {
        public ClienteMap() : base()
        {
            Table("TB_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE");

            HasMany(x => x.PlanosProdutos).KeyColumn("ID_CLIENTE").Cascade.All().Inverse();
            HasMany(x => x.Descontos).KeyColumn("ID_CLIENTE").Cascade.All().Inverse();
            HasMany(x => x.Formas_Pagamento_Cliente).KeyColumn("ID_CLIENTE").Cascade.All().Inverse();//.Not.LazyLoad();
            //HasMany(x => x.Vendas).KeyColumn("ID_CLIENTE").Cascade.All().Inverse();


            Map(x => x.NomeFantasia, "NOMEFANTASIA").Length(255);
            Map(x => x.Numero_Contrato, "NUMERO_CONTRATO");
            Map(x => x.Data_Nascimento, "DATA_NASC");
            Map(x => x.Sexo, "SEXO").CustomType<Sexo>();
            Map(x => x.CpfCnpj, "CPF").Length(18);
            Map(x => x.Nacionalidade, "NACIONALIDADE").Length(25);
            Map(x => x.Profissao, "PROFISSAO").Length(25);
            Map(x => x.Alterado, "ALTERADO");
            Map(x => x.Cod_Cliente, "COD_CLIENTE_ANIEL");

            Map(x => x.Tipo_Cliente, "TIPO_CLIENTE").CustomType<Tipo_Cliente>();
            Map(x => x.Status, "STATUS").CustomType<Status_Cliente>();
            Map(x => x.Tipo_Pessoa, "TIPOPESSOA").CustomType<Tipo_Pessoa>();
            Map(x => x.Estado_Civil, "ESTADO_CIVIL").CustomType<Estado_Civil>();
            Map(x => x.Grau_Instrucao, "GRAU_INSTRUCAO").CustomType<Grau_Instrucao>();
            Map(x => x.Rg, "RG").Length(20);
            Map(x => x.Data_Emissao_Rg, "DATA_EMISSAO_RG");
            Map(x => x.Orgao_Emissao_Rg, "ORGAO_EMISSAO_RG").Length(15);
            Map(x => x.Filiacao_Pai, "FILIACAO_PAI").Length(50);
            Map(x => x.Filiacao_Mae, "FILIACAO_MAE").Length(50);
            Map(x => x.Outro_Meio, "OUTRO_MEIO").Length(50);

            References(x => x.Organizacao, "ID_ORGANIZACAO");

            Map(x => x.Porte, "PORTE").CustomType<Porte>();
            Map(x => x.Razao_Social, "RAZAO_SOCIAL").Length(200);
            Map(x => x.Data_Abertura_Empresa, "DATA_ABERTURA_EMPRESA");
            Map(x => x.Inscricao_Estadual, "INSCRICAO_ESTADUAL").Length(15);
            Map(x => x.Inscricao_Municipal, "INSCRICAO_MUNICIPAL").Length(15);
            Map(x => x.Nome_Resp_Financeiro, "NOME_RESP_FINANCEIRO").Length(50);
            Map(x => x.Cpf_Responsavel, "CPF_RESPONSAVEL").Length(11);
            Map(x => x.Cargo_Responsavel, "CARGO_RESPONSAVEL").Length(30);
            Map(x => x.Ramo_Atividade, "RAMO_ATIVIDADE").Length(100);

            Map(x => x.Outro_Motivo, "OUTRO_MOTIVO").Length(100);

            Map(x => x.Telefone_Fixo, "TELEFONE_FIXO").Length(18);
            Map(x => x.Telefone_Celular, "TELEFONE_CELULAR").Length(18);
            Map(x => x.Telefone_Recado, "TELEFONE_RECADO").Length(18);
            Map(x => x.Numero_Validado, "NUMERO_VALIDADO").Length(18);
            Map(x => x.Celular_Responsavel, "CELULAR_RESPONSAVEL").Length(18);


            Map(x => x.Pessoa_Tel_Recado, "PESSOA_TEL_RECADO").Length(30);
            Map(x => x.Identificador_Telegram, "IDENTIFICADOR_TELEGRAM");

            Map(x => x.Email_Principal, "EMAIL_PRINCIPAL").Length(150);
            Map(x => x.Email_Auxiliar, "EMAIL_AUXILIAR").Length(150);

            Map(x => x.Informacao_Complementares, "INFORMACAO_COMPLEMENTARES");

            Map(x => x.Login, "LOGIN").Length(25);
            Map(x => x.Senha, "SENHA").Length(25);

            //Map(x => x.Conta_Corrente, "CONTA_CORRENTE").Length(25);
            //Map(x => x.Agencia, "AGENCIA").Length(8);
            //Map(x => x.Digito, "DIGITO").Length(1);
            Map(x => x.End_Cobranca_Igual_Principal, "END_COBRANCA_IGUAL_PRINCIPAL");

            Map(x => x.Id_Integrador, "ID_INTEGRADOR").Length(255);

            Map(x => x.Dt_Ultimo_Contato, "ULTIMO_CONTATO");
            
            Map(x => x.Data_Venda, "DATA_VENDA");
            Map(x => x.Data_Criacao_Cliente, "DATA_CRIACAO_CLIENTE");
            References(x => x.Vendedor, "ID_VENDEDOR").ForeignKey("FK_CLIENTE_VENDEDOR");
            References(x => x.Supervisor, "ID_SUPERVISOR").ForeignKey("FK_CLIENTE_SUPERVISOR");

            //References(x => x.Vencimento, "ID_VENCIMENTO").ForeignKey("FK_CLIENTE_VENCIMENTO");
            //References(x => x.Banco, "ID_BANCO").ForeignKey("FK_CLIENTE_BANCO");
            //References(x => x.Forma_Pagamento, "ID_FORMA_PAGAMENTO").ForeignKey("FK_CLIENTE_FORMA_PAGTO");
            References(x => x.Categoria_Cliente, "ID_CATEGORIA").ForeignKey("FK_CLIENTE_CATEGORIA");
            References(x => x.Qual_Meio, "ID_MEIO_CAPTACAO").ForeignKey("FK_CLIENTE_MEIO_CAPTACAO");
            References(x => x.Quem_Indicou, "ID_QUEM_INDICOU").ForeignKey("FK_CLIENTE_QUEM_INDIC");
            References(x => x.Motivo_Inviabilidade, "ID_MOTIVO_INVIABILIDADE").ForeignKey("FK_CLIENTE_INVIABILIDADE");

            References(x => x.Endereco_Principal, "ID_ENDERECO_PRINCIPAL").Cascade.SaveUpdate().ForeignKey("FK_CLIENTE_END_PRICIPAL");
            References(x => x.Endereco_Prospecto, "ID_ENDERECO_PROSPECTO").Cascade.SaveUpdate().ForeignKey("FK_CLIENTE_END_PROSPECTO");
            //References(x => x.Endereco_Cobranca, "ID_ENDERECO_COBRANCA").Cascade.SaveUpdate().ForeignKey("FK_CLIENTE_END_COBRANCA");

            Map(x => x.End_CadastroCondominioProspecto, "END_PROS_DENTRO_COND");
            Map(x => x.Site, "SITE").Length(100);

            Map(x => x.Telefone_Fixo2, "TELEFONE_FIXO2").Length(100);

            Map(x => x.Email_Aux, "EMAIL_AUX").Length(150);


            References(x => x.Foto_Perfil, "ID_FOTO_PERFIL").ForeignKey("FK_CLIENTE_FOTO_PERFIL");

            References(x => x.Usuario, "ID_IDENTITY").ForeignKey("FK_CLIENTE_USUARIO");
            References(x => x.Departamento, "ID_DEPARTAMENTO").ForeignKey("FK_CLIENTE_DEPARTAMENTO");
            
            Map(x => x.UsuarioPlayhub, "USUARIO_PLAYHUB").Length(12);

            //Map(x => x.Forma_Envio, "FORMA_ENVIO").CustomType<Cliente_FormaEnvio>();

            //Map(x => x.Dia_Vencimento, "DIA_VENCIMENTO");

            Map(x => x.Status_Prospecto, "STATUS_PROSPECTO").CustomType<Status_Prospecto>().Index("IDX_STATUS_PROSPECTO");
            Map(x => x.Ind_Inscricao_Estadual, "IND_INSC_ESTADUAL").CustomType<EnumInd_InscricaoEstadual>();

            Map(x => x.Device_id, "DEVICE_ID");
            Map(x => x.IdCampanha, "ID_CAMPANHA");
            Map(x => x.IntegradoOla, "INTEGRADO_OLA");

            Map(x => x.Origem_Cliente, "ORIGEM_CLIENTE").Length(50);
        }
    }
}
