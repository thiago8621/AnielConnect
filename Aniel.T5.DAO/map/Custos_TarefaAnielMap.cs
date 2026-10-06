using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Custos_TarefaAnielMap : ClassMap<Custos_TarefaAniel>
    {
        public Custos_TarefaAnielMap() : base()
        {
            Table("TB_CUSTOS_TAREFA");

            CompositeId()
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.Num_Obra, "NUM_OBRA")
            .KeyProperty(x => x.Num_Doc, "NUM_DOC")
            .KeyProperty(x => x.Tarefa, "TAREFA")
            .KeyProperty(x => x.Unidade_Custo, "UNIDADE_CUSTO")
            .KeyProperty(x => x.Codigo, "CODIGO")
            .KeyProperty(x => x.Tipo_Producao, "TIPO_PRODUCAO")
            .KeyProperty(x => x.Equipe, "EQUIPE")
            .KeyProperty(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV")
            .KeyProperty(x => x.Cod_Sub_Tipo_Serv, "COD_SUB_TIPO_SERV");

            Map(x => x.Data_Prod, "DATA_PROD");

            Map(x => x.Cod_Preco, "COD_PRECO");

            Map(x => x.Quant_Prod, "QUANT_PROD");
            Map(x => x.Quant_Catal, "QUANT_CATAL");


            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA").Index("IDX_DOCPROD_DIA");
            Map(x => x.Hora, "HORA").CustomType("Time"); 
        }
    }
}
