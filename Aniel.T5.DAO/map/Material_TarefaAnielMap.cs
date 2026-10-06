using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Material_TarefaAnielMap : ClassMap<Material_TarefaAniel>
    {
        public Material_TarefaAnielMap() : base()
        {
            Table("TB_MATERIAL_TAREFA");

            CompositeId()
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.Num_Obra, "NUM_OBRA")
            .KeyProperty(x => x.Num_Doc, "NUM_DOC")
            .KeyProperty(x => x.Tarefa, "TAREFA")
            .KeyProperty(x => x.CodMat, "CODMAT")
            .KeyProperty(x => x.CodCpl, "CODCPL")
            .KeyProperty(x => x.Equipe, "EQUIPE")
            .KeyProperty(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV")
            .KeyProperty(x => x.Cod_Sub_Tipo_Serv, "COD_SUB_TIPO_SERV");

            Map(x => x.Data_Aplic, "DATA_APLIC");
            Map(x => x.Id_Cliente, "ID_CLIENTE");
            Map(x => x.Quantidade, "QUANTIDADE");
            Map(x => x.Quant_Remov, "QUANT_REMOV");            
            Map(x => x.Quant_Catal, "QUANT_CATAL");
            Map(x => x.CodMat_Inst, "CODMAT_INST");

            Map(x => x.ID_DEVOLUCAO_OS_MAT, "ID_DEVOLUCAO_OS_MAT");

            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA").Index("IDX_DOCPROD_DIA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Integrado, "INTEGRADO");
        }
    }
}
