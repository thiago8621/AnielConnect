using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Codigo_ComplementarAnielMap : ClassMap<Codigo_ComplementarAniel>
    {
        public Codigo_ComplementarAnielMap()
        {
            Table("TB_CODIGO_COMPLEMENTAR");

            CompositeId()
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.CodMat, "CODMAT")
            .KeyProperty(x => x.CodCpl, "CODCPL");

            Map(x => x.Localizacao, "LOCALIZACAO");
            Map(x => x.Sinal_Ativo, "SINAL_ATIVO");
            Map(x => x.Mensagem_Erro, "MENSAGEM_ERRO_ATIVACAO");
        }
    }
}
