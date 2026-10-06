using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class IntegracoesExternasMap : ClassMapBase<IntegracoesExternas>
    {
        public IntegracoesExternasMap() : base()
        {
            Table("TB_INTEGRACOES_EXTERNAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRACOES_EXTERNAS");

            Map(x => x.IntegracaoExterna, "INTEGRACAO_EXTERNA").CustomType<IntegracaoExterna>();
            Map(x => x.TipoIntegracao, "TIPO_INTEGRACAO_EXTERNA").CustomType<EnumTipoIntegracao>();
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Senha, "SENHA");
            Map(x => x.UrlApi, "URL_API");
            Map(x => x.ApiKey, "API_KEY");
            Map(x => x.ApiSecret, "API_SECRET");
            Map(x => x.HeaderIntegracaoExterna1, "HEADER_INTEGRACAO_EXTERNA1");

            Map(x => x.EnviarTodosParametros, "ENVIAR_TODOS_PARAMETROS");
            //Mudança para o tipo Blob
            Map(x => x.TokenAcesso, "TOKEN_ACESSO").Length(8000);
            Map(x => x.refreshToken, "REFRESH_TOKEN").Length(8000);

            Map(x => x.ExpirationRefreshToken, "EXPIRATION_REFRESHTOKEN");
            Map(x => x.ExpirationToken, "EXPIRATION_TOKEN");
        }
    }
}
