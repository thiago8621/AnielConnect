using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Services
{
    public class BaseIntegracaoSaperxService
    {
        public string baseUrl { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string token { get; set; }
        public BaseIntegracaoSaperxService()
        {

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<IntegracoesExternas> repo = new Conexao<IntegracoesExternas>(session))
            {
                IntegracoesExternas confÌntegracao = repo.selectFirst(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.SAPERX).Result;

                if (confÌntegracao != null)
                {
                    this.baseUrl = confÌntegracao.UrlApi;
                    this.senha = confÌntegracao.Senha;
                    this.token = confÌntegracao.TokenAcesso;
                    this.usuario = confÌntegracao.Usuario;
                }
            }

        }
        public string EnviarRequisicaoGet(string endpoint)
        {

            try
            {
                using (var post = new PostHttp(CONSTANTES.log4Net, this.token))
                {
                    return post.ExecuteAsync(endpoint);
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public string EnviarRequisicaoPost(string endpoint, string json)
        {
            try
            {
                using (var post = new PostHttp(CONSTANTES.log4Net, this.token))
                {
                    return post.ExecuteString<PostApiRetorno>(this.baseUrl + endpoint, json);
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public string EnviarRequisicaoPut(string endpoint, string json)
        {
            try
            {
                using (var post = new PostHttp(CONSTANTES.log4Net, this.token))
                {
                    return post.ExecuteStringPut<PostApiRetorno>(this.baseUrl + endpoint, json);
                }
            }
            catch
            {
                return String.Empty;
            }
        }


    }
}