using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class IntegracoesExternas : BaseModel
    {
        /// <summary>
        /// Url da Api Base para requisições 
        /// </summary>
        public virtual string UrlApi { get; set; }
        /// <summary>
        /// Usuario de Autenticação da Api
        /// </summary>
        public virtual string Usuario { get; set; }
        /// <summary>
        /// Senha de Autenticação da Api
        /// </summary>
        public virtual string Senha { get; set; }
        /// <summary>
        /// Token de Acesso da Api
        /// </summary>
        public virtual string TokenAcesso { get; set; }

        public virtual string ApiKey { get; set; }
        public virtual string ApiSecret { get; set; }
        public virtual string HeaderIntegracaoExterna1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool EnviarTodosParametros { get; set; }
        /// <summary>
        /// Tipo de integração Selecionada
        /// </summary>
        public virtual IntegracaoExterna IntegracaoExterna { get; set; } 
        /// <summary>
        /// Tipo de integração 
        /// </summary>
        public virtual EnumTipoIntegracao TipoIntegracao { get; set; }
        /// <summary>
        /// Refresh Token (especial utilizado na integração Geosite e da Voalle)
        /// </summary>
        public virtual string refreshToken { get; set; }
        /// <summary>
        /// Data de Expiração do Token
        /// </summary>        
        public virtual DateTime? ExpirationToken { get; set; }
        /// <summary>
        /// Data de Expiração RefreshToken
        /// </summary>
        public virtual DateTime? ExpirationRefreshToken { get; set; }

    }
}





