using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class EstoqueAnielMap : ClassMap<EstoqueAniel>
    {
        public EstoqueAnielMap() : base()
        {
            Table("TB_ESTOQUE");

            CompositeId()
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.CodMat, "CODMAT")
            .KeyProperty(x => x.CodCpl, "CODCPL")
            .KeyProperty(x => x.Data, "DATA")
            .KeyProperty(x => x.Entrada, "ENTRADA")
            .KeyProperty(x => x.NotaFiscal, "NOTAFISCAL")
            .KeyProperty(x => x.Pedido, "PEDIDO")
            .KeyProperty(x => x.ProjDestino, "PROJDESTINO")
            .KeyProperty(x => x.ProjOrigem, "PROJORIGEM")
            .KeyProperty(x => x.Saida, "SAIDA")
            .KeyProperty(x => x.SeqLivre, "SEQLIVRE")
            .KeyProperty(x => x.SeqRmdm, "SEQRMDM")
            .KeyProperty(x => x.SeqTransf, "SEQTRANSF");

            Map(x => x.CodOs, "CODOS");
            Map(x => x.CodLib, "CODLIB");
            Map(x => x.SeqDevolucao, "SEQDEVOLUCAO");
            Map(x => x.Rir, "RIR");
            Map(x => x.CtOrigem, "CTORIGEM");
            Map(x => x.Empresa_Origem, "EMPRESA_ORIGEM");
            Map(x => x.CtDestino, "CTDESTINO");
            Map(x => x.Empresa_Destino, "EMPRESA_DESTINO");
            Map(x => x.Equipe, "EQUIPE");
            Map(x => x.Cod_CCusto, "COD_CCUSTO");
            Map(x => x.Cod_Fornecedor, "COD_FORNECEDOR");
            Map(x => x.Transferencia, "TRANSFERENCIA");
            Map(x => x.Num_Obra, "NUM_OBRA");
            Map(x => x.Observacao, "OBSERVACAO");
            Map(x => x.Valor_Compra, "VALOR_COMPRA");
            Map(x => x.Limite_Saida, "LIMITE_SAIDA");
            Map(x => x.Qtde_Tarefa, "QTDE_TAREFA");
            Map(x => x.Saldo_Dia, "SALDO_DIA");
            Map(x => x.Tipo_Atividade, "TIPO_ATIVIDADE");
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA").CustomType("Date");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Integracao, "INTEGRACAO");
            Map(x => x.Status, "STATUS");
            Map(x => x.Cod_Local_Entrega, "COD_LOCAL_ENTREGA");
            Map(x => x.Sequencial, "SEQUENCIAL");
            Map(x => x.Cod_Tipo_Movto, "COD_TIPO_MOVTO");
            Map(x => x.Seq_Contabil, "SEQ_CONTABIL");
            Map(x => x.OP, "OP");
            Map(x => x.CodEmpresa, "CODEMPRESA");
            Map(x => x.Locado, "LOCADO");
            Map(x => x.Lancado_Contabil, "LANCADO_CONTABIL");
            Map(x => x.Valor_Manual, "VALOR_MANUAL");
            Map(x => x.Cod_Receb_Fisico, "COD_RECEB_FISICO");
            Map(x => x.Hora_Atend, "HORA_ATEND").CustomType("Time");
            Map(x => x.Consolida_NF, "CONSOLIDA_NF");
            Map(x => x.Valor_Total, "VALOR_TOTAL");
            Map(x => x.Tipo_Transferencia, "TIPO_TRANSFERENCIA");
            Map(x => x.Selecionar, "SELECIONAR");
            Map(x => x.Num_Doc, "NUM_DOC");
            Map(x => x.Id_Receb_Item, "ID_RECEB_ITEM");
            Map(x => x.Id_Fechamento_Fornecedor, "ID_FECHAMENTO_FORNECEDOR");
            Map(x => x.Calcula_Precisao, "CALCULA_PRECISAO");
            Map(x => x.Id_Inv_Fisico, "ID_INV_FISICO");
            Map(x => x.Id_Inv_Equipe, "ID_INV_EQUIPE");
            Map(x => x.Diferenca, "DIFERENCA");
            Map(x => x.Processado_Portal, "PROCESSADO_PORTAL");
            Map(x => x.INS, "INS");
            Map(x => x.AC_IE, "AC_IE");
            Map(x => x.Id_Lei_Icms, "ID_LEI_ICMS");
            Map(x => x.Id_Lei_Ipi, "ID_LEI_IPI");
            Map(x => x.Id_Lei_Confins, "ID_LEI_CONFINS");
            Map(x => x.Id_Lei_Pis, "ID_LEI_PIS");
            Map(x => x.Id_Nf_Type, "ID_NF_TYPE");
            Map(x => x.Retem_Icms_Iss, "RETEM_ICMS_ISS");
            Map(x => x.Data_Emissao_Nf, "DATA_EMISSAO_NF").CustomType("Date");
            Map(x => x.Qtde_Volumes_Recebidos, "QTDE_VOLUMES_RECEBIDOS");
            Map(x => x.Peso_Bruto_Volumes, "PESO_BRUTO_VOLUMES");
            Map(x => x.Peso_Liquido_Volumes, "PESO_LIQUIDO_VOLUMES");
            Map(x => x.CFOP, "CFOP");
            Map(x => x.Tipo_Item, "TIPO_ITEM");
            Map(x => x.Serie_Nf, "SERIE_NF");
            Map(x => x.Origem_Material, "ORIGEM_MATERIAL");
            Map(x => x.Valor_Descontar_Impostos, "VALOR_DESCONTAR_IMPOSTOS");
            Map(x => x.Saldo_Revisado, "SALDO_REVISADO");
            Map(x => x.Audita_Sucata, "AUDITA_SUCATA");
            Map(x => x.Recuperado, "RECUPERADO");
            Map(x => x.Seq_Insert, "SEQ_INSERT");
            Map(x => x.Observacao_Bkp, "OBSERVACAO_BKP");
            Map(x => x.Id_Atendimento_Estoque, "ID_ATENDIMENTO_ESTOQUE");
            Map(x => x.Id_Saldo_Auto, "ID_SALDO_AUTO");
            Map(x => x.Num_Contrato_Cliente, "NUM_CONTRATO_CLIENTE");
            Map(x => x.Chave_Acesso, "CHAVE_ACESSO");
            Map(x => x.Id_Conta_Financeira, "ID_CONTA_FINANCEIRA");
            Map(x => x.Conta_Financeira, "CONTA_FINANCEIRA");
            Map(x => x.Id_Contas_Pagar, "ID_CONTAS_PAGAR");
        }
    }
}
