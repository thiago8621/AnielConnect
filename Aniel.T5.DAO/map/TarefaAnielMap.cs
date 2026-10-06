using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using NHibernate.AdoNet;

namespace Aniel.T5.DAO.map
{
    public class TarefaAnielMap : ClassMap<TarefaAniel>
    {
        public TarefaAnielMap() : base()
        {
            Table("TB_TAREFA");

            CompositeId()
            .KeyProperty(x => x.CodCt, column => column
                                                       .ColumnName("CODCT")
                                                       .Length(4))

            .KeyProperty(x => x.Projeto, column => column
                                                         .ColumnName("PROJETO")
                                                         .Length(20))

            .KeyProperty(x => x.Num_Obra, column => column
                                                          .ColumnName("NUM_OBRA")
                                                          .Length(50))

            .KeyProperty(x => x.Tarefa, column => column
                                                        .ColumnName("TAREFA")
                                                        .Length(15));

            Map(x => x.Equipe_Prevista, "EQUIPE_PREVISTA").Length(10)
                                                          .Index("TB_TAREFA_IDX2");

            Map(x => x.CodOs, "OS").Length(13);

            Map(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");
            Map(x => x.Cod_Sub_Tipo_Serv, "COD_SUB_TIPO_SERV").Length(6);

            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.Usuario_Alteracao, "USUARIO_ALTERACAO").Length(10);

            //Map(x => x.Uf, "UF").Length(2);
            Map(x => x.Mobile, "MOBILE").Length(1);

            Map(x => x.Dia, "DIA").Index("IDX_TAREFA_DIA_HORA");
            Map(x => x.Hora, "HORA").CustomType("Time")
                                    .Index("IDX_TAREFA_DIA_HORA");
            
        }
    }
}
