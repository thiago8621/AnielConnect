using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class RequisicaoMovimentacaoAnielMap : ClassMap<Requisicao_MovimentoRmdm>
    {
        public RequisicaoMovimentacaoAnielMap()
        {
            Table("TB_MOVIMENTO_RMDM");

            CompositeId()
            .KeyProperty(x => x.CodMat, "CODMAT")
            .KeyProperty(x => x.CodCpl, "CODCPL")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.SeqRmdm, "SEQRMDM");

            Map(x => x.Qtde_Solicitada, "QTDE_SOLICITADA");

            Map(x => x.Rir, "RIR");
            Map(x => x.Qtde_Entregue, "QTDE_ENTREGUE");
            Map(x => x.Qtde_Descontada, "QTDE_DESCONTADA");
            Map(x => x.Valor_Descontar, "VALOR_DESCONTAR");
            Map(x => x.Qtde_Devolver, "QTDE_DEVOLVER");
            Map(x => x.Data_Dm_Prog, "DATA_DM_PROG");
            Map(x => x.Saldo_Dia, "SALDO_DIA");
            Map(x => x.Classificacao_Rmdm, "CLASSIFICACAO_RMDM");
            Map(x => x.Observacao, "OBSERVACAO");
            Map(x => x.Automatico, "AUTOMATICO");
            Map(x => x.Sequencial, "SEQUENCIAL");
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Id_Delegacia, "ID_DELEGACIA");
            Map(x => x.BO, "BO");
            Map(x => x.CodCt_Regulador, "CODCT_REGULADOR");
            Map(x => x.Xct, "XCT");
            Map(x => x.Dia_Alterou, "DIA_ALTEROU");
            Map(x => x.Hora_Alterou, "HORA_ALTEROU").CustomType("Time");
            Map(x => x.XRegulador, "XREGULADOR");
            Map(x => x.Devolver_Regul, "DEVOLVER_REGUL");
            Map(x => x.Projeto_Regulador, "PROJETO_REGULADOR");
            Map(x => x.XProj, "XPROJ");
            Map(x => x.Usuario_Alterou, "USUARIO_ALTEROU");
            Map(x => x.Tipo_Saldo, "TIPOSALDO");
            Map(x => x.Tipo_Saldo_Aplic, "TIPOSALDOAPLIC");
            Map(x => x.Grupo_Projeto, "GRUPO_PROJETO");
            Map(x => x.Prog_Rm, "PROG_RM");
            Map(x => x.Prog_Dm, "PROG_DM");
            Map(x => x.Prog_Transf, "PROG_TRANSF");
            Map(x => x.Prog_Transf_Regul, "PROG_TRANSF_REGUL");
            Map(x => x.Prog_Rm_Regul, "PROG_RM_REGUL");
            Map(x => x.Prog_Transf_Regul_Auto, "PROG_TRANSF_REGUL_AUTO");
            Map(x => x.Prog_Transf_Auto, "PROG_TRANSF_AUTO");
            Map(x => x.Saldo_Fisico, "SALDO_FISICO");
            Map(x => x.Saldo_Fisico_Regul, "SALDO_FISICO_REGUL");
            Map(x => x.Aplic, "APLIC");
            Map(x => x.Remov, "REMOV");
            Map(x => x.Ent, "ENT");
            Map(x => x.Sai, "SAI");
            Map(x => x.Limite_Kit_Equipe, "LIMITE_KIT_EQUIPE");
            Map(x => x.Id_Solicitacao_Material, "ID_SOLICITACAO_MATERIAL");
            Map(x => x.Localizacao_Real, "LOCALIZACAO_REAL");
            Map(x => x.Id_Devolucao_Os_Mat, "ID_DEVOLUCAO_OS_MAT");
            Map(x => x.Peso, "PESO");
            Map(x => x.Store_Location, "STORE_LOCATION");
            Map(x => x.Cancelado, "CANCELADO");
            Map(x => x.Terminal_Retirado, "TERMINAL_RETIRADO");
        }
    }
}
