using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class ReincidenciaDocProdAnielMap : ClassMap<ReincidenciaDocProdAniel>
    {
        public ReincidenciaDocProdAnielMap()
        {
            Table("TB_REINCIDENCIA_DOCPROD");

            CompositeId()
                .KeyProperty(x => x.CodCt, "CODCT")
                .KeyProperty(x => x.Projeto, "PROJETO")
                .KeyProperty(x => x.NumObra, "NUM_OBRA")
                .KeyProperty(x => x.NumDoc, "NUM_DOC");

            References(x => x.ServicoReincidente)
                .Columns(
                    "CODCT",
                    "PROJETO",
                    "NUM_OBRA",
                    "NUM_DOC"
                ).ReadOnly();

            Map(x => x.CodCtAnterior, "CODCT_ANTERIOR");
            Map(x => x.ProjetoAnterior, "PROJETO_ANTERIOR");
            Map(x => x.NumObraAnterior, "NUM_OBRA_ANTERIOR");
            Map(x => x.NumDocAnterior, "NUM_DOC_ANTERIOR");

            References(x => x.ServicoAnterior)
                .Columns(
                    "CODCT_ANTERIOR",
                    "PROJETO_ANTERIOR",
                    "NUM_OBRA_ANTERIOR",
                    "NUM_DOC_ANTERIOR"
                ).ReadOnly();

            Map(x => x.Data, "DATA");
        }
    }
}
