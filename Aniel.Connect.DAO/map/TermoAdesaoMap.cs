using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class TermoAdesaoMap : ClassMapBase<TermoAdesao>
    {
        public TermoAdesaoMap() : base()
        {
            Table("TB_TERMO_ADESAO");

            Id(x => x.Id,"ID");

            Map(x => x.NomeArquivo, "NOME_ARQUIVO");
            Map(x => x.CaminhoArquivo, "CAMINHO_ARQUIVO");
            Map(x => x.MensagemConcluido, "MENSAGEM_CONCLUIDO").Length(500);
        }
    }
}
