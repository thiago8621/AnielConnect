using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class MaterialAnielMap : ClassMap<MaterialAniel>
    {
        public MaterialAnielMap() : base()
        {
            Table("TB_MATERIAL");

            Id(x => x.CodMat, "CODMAT");
            
            Map(x => x.CodMatIntegra, "CODMAT_INTEGRA");
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Unid, "UNID");
            Map(x => x.Valor, "VALOR");
            Map(x => x.Classificacao, "CLASSIFICACAO");
            Map(x => x.Familia, "FAMILIA");
            Map(x => x.Grupo, "COD_SUBGRUPO_MAT");
            Map(x => x.SubCod, "SUBCOD");
            Map(x => x.SubCod_Exclusivo, "SUBCOD_EXCLUSIVO");
            Map(x => x.Tipo, "TIPO");
            Map(x => x.ENVIA_RESERV, "ENVIA_RESERV");
            Map(x => x.Gerar_Saldo_Auto, "GERAR_SALDO_AUTO_APLIC");
            Map(x => x.Data_Criacao, "DATA_CRIACAO");
            Map(x => x.Hora_Criacao, "HORA_CRIACAO").CustomType("Time");
            Map(x => x.Usuario_Criacao, "USUARIO_CRIACAO");
            Map(x => x.Desc_Auxiliar, "DESC_AUXILIAR").Length(200);
            Map(x => x.TipoEquipamento, "TIPO_EQUIPAMENTO").CustomType<TipoEquipamento>();
            Map(x => x.AtivacaoSinal, "ATIVACAO_SINAL");

            Map(x => x.CodMat_Ret, "CODMAT_RET");
            Map(x => x.CodMat_Det, "CODMAT_DET");
            Map(x => x.CodMat_Reap, "CODMAT_REAP");
            Map(x => x.CodMat_Defeito, "CODMAT_DEFEITO");
            Map(x => x.CodMat_Sucata, "CODMAT_SUCATA");
        }
    }
}
