using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PesquisaSatisfacaoRepository;
using Resources;
using Sinapse.DAO;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class PesquisaSatisfacaoBusiness
    {
        private readonly IPesquisaSatisfacaoRepository _repository;
        

        public PesquisaSatisfacaoBusiness(IPesquisaSatisfacaoRepository repository)
        {
            this._repository = repository;
        }

        public PostApiRetorno EnviarEmail(Cliente cliente,string tituloPesquisa,string urlLogo,string urlPesquisa, EmailUtil emailService,string caminhoFisicoConnect)
        {
			PostApiRetorno retorno = new PostApiRetorno();
            using (StreamReader stream = File.OpenText($@"{caminhoFisicoConnect}\Arquivos\email_pesquisa_satisfacao.html"))
            {
                String arquivo_email = stream.ReadToEnd();
                String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                         .Replace("{{TITULO_PESQUISA}}", tituloPesquisa)
                                                         .Replace("{{URL_PESQUISA}}", urlPesquisa)
                                                         .Replace("{{URL_LOGO}}", urlLogo);

                if (!String.IsNullOrEmpty(cliente.Email_Principal))
                {
                    //parametros.Add("@OBS", String.Format(RsCadastros.PESQUISA_SATISFACAO, pesquisa_satisfacao.Titulo_Da_Pesquisa, cliente.NomeFantasia));
                    emailService.EnviarMensagem(cliente.Email_Principal, "Pesquisa de Satisfação", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                }
                else
                {
                    retorno.Mensagens.Add(new PostApiMensagem
                    {
                        Mensagem = String.Format(RsCadastros.PESQUISA_SATISFACAO_CLIENTE_SEM_EMAIL, tituloPesquisa, cliente.NomeFantasia),
                        Tipo = PostApiMensagemType.Info
                    });
                }
            }

            return retorno;

        }

        /// <summary>
        /// Inserir dados da pesquisa
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="usuario"></param>
        /// <param name="idPesquisa"></param>
        public void InsertPesquisaResposta(Cliente cliente, string usuario, long idPesquisa, long? idAtendimento =  null)
        {
            this._repository.InsertPesquisaResposta(cliente, usuario, idPesquisa, idAtendimento);
        }
    }
}
