using Aniel.Connect.API_Aniel.Models;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Nota_Fiscal_MestreMap : ClassMapBase<Nota_Fiscal_Mestre>
    {
        public Nota_Fiscal_MestreMap() : base()
        {
            Table("TB_NOTA_FISCAL_MESTRE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_NOTA_FISCAL_MESTRE");

            Map(x => x.Razao_Social_Tomador, "RAZAO_SOCIAL_TOMADOR").Length(40);
            Map(x => x.Nome_Fantasia_Tomador, "NOME_FANTASIA_TOMADOR").Length(40);

            Map(x => x.Endereco_Tomador, "ENDERECO_TOMADOR").Length(40);
            Map(x => x.Numero_Tomador, "NUMERO_TOMADOR");
            Map(x => x.Complemento_Tomador, "COMPLEMENTO_TOMADOR").Length(30);
            Map(x => x.Bairro_Tomador, "BAIRRO_TOMADOR").Length(22);
            Map(x => x.Cidade_Tomador, "CIDADE_TOMADOR").Length(22);
            Map(x => x.Estado_Tomador, "ESTADO_TOMADOR").Length(2);
            Map(x => x.Cep_Tomador, "CEP_TOMADOR").Length(8);

            Map(x => x.Cnpj_Tomador, "CNPJ_TOMADOR").Length(18);
            Map(x => x.Insc_Estadual_Tomador, "INSC_ESTADUAL_TOMADOR").Length(17);
            Map(x => x.Insc_Municipal_Tomador, "INSC_MUNICIPAL_TOMADOR").Length(17);
            Map(x => x.Cpf_Tomador, "CPF_TOMADOR").Length(14);
            Map(x => x.Rg_Tomador, "RG_TOMADOR").Length(14);

            Map(x => x.Dia_Vencimento, "DIA_VENCIMENTO");
            Map(x => x.Modelo_Nota_Fiscal, "MODELO_NOTA_FISCAL").Length(2);
            Map(x => x.Cfop, "CFOP").Length(4);

            Map(x => x.DDD_Tomador, "DDD_TOMADOR").Length(5);
            Map(x => x.Telefone_Tomador, "TELEFONE_TOMADOR").Length(13);
            Map(x => x.Email_Tomador, "EMAIL_TOMADOR").Length(40);
            Map(x => x.Cod_Consumidor, "COD_CONSUMIDOR").Length(10);

            Map(x => x.Tipo_Assinante, "TIPO_ASSINANTE").Length(1);
            Map(x => x.Tipo_Utilizacao, "TIPO_UTILIZACAO").Length(1);

            Map(x => x.Data_Emissao, "DATA_EMISSAO");
            Map(x => x.Data_Prestacao, "DATA_PRESTACAO");

            Map(x => x.Num_Nota_Fiscal, "NUM_NOTA_FISCAL").Length(9);

            Map(x => x.Id_Conta_Receber, "ID_CONTA_RECEBER");
            Map(x => x.Instalacao, "INSTALACAO");
            Map(x => x.Observacao, "OBSERVACAO");

            Map(x => x.Optante_Simples_Nacional, "OPTANTE_SIMPLES");
            Map(x => x.Incentivo_Cultural, "INCENTIVO_CULTURAL");
            Map(x => x.Incentivo_Fiscal, "INCENTIVO_FISCAL");
            Map(x => x.Cnae, "CENAE_FISCAL").Length(20);
            Map(x => x.Processo_Suspensao, "NUM_PROCESSO_SUSPENSAO").Length(20);

            Map(x => x.Id_Atividade_NF, "ID_ATIVIDADE_NF");

            Map(x => x.Razao_Social_Prestador, "RAZAO_SOCIAL_PRESTADOR").Length(100);
            Map(x => x.Nome_Fantasia_Prestador, "NOME_FANTASIA_PRESTADOR").Length(100);

            Map(x => x.Endereco_Prestador, "ENDERECO_PRESTADOR").Length(40);
            Map(x => x.Numero_Prestador, "NUMERO_PRESTADOR");
            Map(x => x.Complemento_Prestador, "COMPLEMENTO_PRESTADOR").Length(30);
            Map(x => x.Bairro_Prestador, "BAIRRO_PRESTADOR").Length(22);
            Map(x => x.Cidade_Prestador, "CIDADE_PRESTADOR").Length(22);
            Map(x => x.Estado_Prestador, "ESTADO_PRESTADOR").Length(2);
            Map(x => x.Cep_Prestador, "CEP_PRESTADOR").Length(8);

            Map(x => x.Cnpj_Prestador, "CNPJ_PRESTADOR").Length(18);
            Map(x => x.Insc_Estadual_Prestador, "INSC_ESTADUAL_PRESTADOR").Length(17);
            Map(x => x.Insc_Municipal_Prestador, "INSC_MUNICIPAL_PRESTADOR").Length(17);
            Map(x => x.Cpf_Prestador, "CPF_PRESTADOR").Length(14);

            Map(x => x.DDD_Prestador, "DDD_PRESTADOR").Length(5);
            Map(x => x.Telefone_Prestador, "TELEFONE_PRESTADOR").Length(13);
            Map(x => x.Email_Prestador, "EMAIL_PRESTADOR").Length(40);

            Map(x => x.Id_Api_NF, "ID_API_NF").Length(50);

            Map(x => x.Xml_NF, "XML_NF").Length(5000);
            Map(x => x.Chave_Cancelamento, "CHAVE_CANCELAMENTO").Length(100);
            Map(x => x.Serie_Nota_Fiscal, "SERIE_NF").Length(9);
            Map(x => x.Sequencial_Nota_Fiscal, "SEQUENCIAL_NF");
        }
    }
}
