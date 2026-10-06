using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class RequisicaoAnielMap : ClassMap<Requisicao_SequenciaRmdmAniel>
    {
        public RequisicaoAnielMap()
        {
            Table("TB_SEQUENCIA_RMDM");

            CompositeId()
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.Codct, "CODCT")
            .KeyProperty(x => x.Seqrmdm, "SEQRMDM");

            Map(x => x.Seqtransf, "SEQTRANSF");
            Map(x => x.Num_Obra, "NUM_OBRA");
            Map(x => x.Data, "DATA");

            References(x => x.Equipe, "EQUIPE").ForeignKey("FK_TB_EQUIPE");

            Map(x => x.Cod_CCusto, "COD_CCUSTO");
            Map(x => x.Tipo_Movimento, "TIPO_MOVIMENTO");
            Map(x => x.Observacao, "OBSERVACAO");
            Map(x => x.Baixada, "BAIXADA");
            Map(x => x.Filtro, "FILTRO");
            Map(x => x.Historico, "HISTORICO");
            Map(x => x.Data_Impressao, "DATA_IMPRESSAO");
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Usuario_Autoriz, "USUARIO_AUTORIZ");
            Map(x => x.Dia_Autoriz, "DIA_AUTORIZ");
            Map(x => x.Hora_Autoriz, "HORA_AUTORIZ").CustomType("Time");
            Map(x => x.Seqrmdm_Vinc, "SEQRMDM_VINC");
            Map(x => x.Cod_Regiao, "COD_REGIAO");
            Map(x => x.Id_Delegacia, "ID_DELEGACIA");
            Map(x => x.BO, "BO");
            Map(x => x.Liberar_Impressao, "LIBERAR_IMPRESSAO");
            Map(x => x.Cod_Motivo_N_Atend, "COD_MOTIVO_N_ATEND");
            Map(x => x.Alteracao_Capa, "ALTERACAO_CAPA");
            Map(x => x.Alteracao_Item, "ALTERACAO_ITEM");
            Map(x => x.Recurso_Origem, "RECURSO_ORIGEM");
            Map(x => x.Id_Solicitacao_Material, "ID_SOLICITACAO_MATERIAL");
            Map(x => x.Cod_Superv, "COD_SUPERV");
            Map(x => x.Data_Excluiu_Mat, "DATA_EXCLUIU_MAT");
            Map(x => x.Id_Atendimento_Estoque, "ID_ATENDIMENTO_ESTOQUE");
            Map(x => x.Id_Apropriacao_Baixa, "ID_APROPRIACAO_BAIXA");
            Map(x => x.Id_Moviment_Type, "ID_MOVIMENT_TYPE");
            Map(x => x.Cancelado, "CANCELADO");
            Map(x => x.DataHora_Reportagem, "DATAHORA_REPORTAGEM");
            Map(x => x.Usuario_Reportagem, "USUARIO_REPORTAGEM");
            Map(x => x.Motivo_Reportagem, "MOTIVO_REPORTAGEM");
            Map(x => x.Id_Motivo_Cancelamento, "ID_MOTIVO_CANCELAMENTO");
            Map(x => x.Seqrmdm_Dm_Vinc, "SEQRMDM_DM_VINC");
            Map(x => x.Cod_Local_Entrega, "COD_LOCAL_ENTREGA");
            Map(x => x.Codct_Operacional, "CODCT_OPERACIONAL");
            Map(x => x.Projeto_Operacional, "PROJETO_OPERACIONAL");
            Map(x => x.Assinado, "ASSINADO");
            Map(x => x.Status_Fedex_So, "STATUS_FEDEX_SO");
            Map(x => x.Num_Lanc_Fedex_So, "NUM_LANC_FEDEX_SO");
            Map(x => x.Status_Fedex_Po, "STATUS_FEDEX_PO");
            Map(x => x.Num_Lanc_Fedex_Po, "NUM_LANC_FEDEX_PO");
            Map(x => x.Id_Planejamento_Rm, "ID_PLANEJAMENTO_RM");
            Map(x => x.Id_Nf_Comodato, "ID_NF_COMODATO");
            Map(x => x.Confirm_Comparece, "CONFIRM_COMPARECE");
            Map(x => x.QrCode, "QRCODE");
            Map(x => x.Nome_Locker, "NOME_LOCKER");
            Map(x => x.Id_Locker, "ID_LOCKER");
            Map(x => x.Box, "BOX");
            Map(x => x.Status_Box_Locker, "STATUS_BOX_LOCKER");
            Map(x => x.Seq_Locker, "SEQ_LOCKER");
            Map(x => x.QrCode_Locker_Retirada, "QRCODE_LOCKER_RETIRADA");
            Map(x => x.QrCode_Locker, "QRCODE_LOCKER");
            Map(x => x.Data_Locker_Deposito, "DATA_LOCKER_DEPOSITO");
            Map(x => x.Hora_Locker_Deposito, "HORA_LOCKER_DEPOSITO").CustomType("Time");
            Map(x => x.Data_Locker_Retirada, "DATA_LOCKER_RETIRADA");
            Map(x => x.Hora_Locker_Retirada, "HORA_LOCKER_RETIRADA").CustomType("Time");
            Map(x => x.Data_Locker_Reservou, "DATA_LOCKER_RESERVOU");
            Map(x => x.Hora_Locker_Reservou, "HORA_LOCKER_RESERVOU").CustomType("Time");
            Map(x => x.Usuario_Locker_Reservou, "USUARIO_LOCKER_RESERVOU");
            Map(x => x.Usuario_Locker_Deposito, "USUARIO_LOCKER_DEPOSITO");
            Map(x => x.Usuario_Locker_Retirada, "USUARIO_LOCKER_RETIRADA");
            Map(x => x.Retorno_Retirada, "RETORNO_RETIRADA");
            Map(x => x.URL_Aceite, "URL_ACEITE").Length(2048);
            Map(x => x.URL_Recusa, "URL_RECUSA").Length(2048);
        }
    }
}
