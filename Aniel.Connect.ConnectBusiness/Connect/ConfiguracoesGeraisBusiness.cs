using Aniel.Connect.Core;
//using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ConfiguracoesGeraisBusiness
    {
        private readonly IConfiguracoesGeraisRepository _repoConfig;

        public ConfiguracoesGeraisBusiness(IConfiguracoesGeraisRepository repoConfig)
        {
            this._repoConfig = repoConfig;
        }

        public Configuracoes_Gerais GetConfiguracoesGerais()
        {
            return this._repoConfig.GetConfiguracoesGerais();
        }

        public string GetUrlOla()
        {
            return this._repoConfig.GetUrlOla();
        }

        /// <summary>
        ///     Retorna as consultas de restrição habilitadas para exibição no cadastro de lead
        /// </summary>
        public IEnumerable<EnumEtapaVenda> GetConsultasRestricaoHabilitadasLead()
        {
            var consultas = new List<EnumEtapaVenda>();
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

            if (config.ConsultaViabilidadeExibeLead)
            {
                consultas.Add(EnumEtapaVenda.ConsultaViabilidade);
            }

            if (config.ConsultaPendenciaFinanceiraExibeLead)
            {
                consultas.Add(EnumEtapaVenda.ConsultaPendenciaFinanceira);
            }

            if (config.ConsultaPendenciaEnderecoExibeLead)
            {
                consultas.Add(EnumEtapaVenda.ConsultaPendenciaEndereco);
            }

            if (config.ConsultaCreditoExibeLead)
            {
                consultas.Add(EnumEtapaVenda.ConsultaCredito);
            }

            return consultas;
        }

        public void AtualizarProvedor(string senhaOla, Configuracoes_Gerais cg)
        {
            string url = this._repoConfig.GetUrlOla();

            Response resposta = new BaseController("", "").GetOla(url, $"api/Param/GetToken/{senhaOla}");

            if (resposta.result)
            {

                ProvedorSetRequest psr = new ProvedorSetRequest();
                psr.caminhoLogo = cg.OLA_URL_Logo;
                psr.cnpj = cg.CNPJ;
                psr.colorAccents = cg.OLA_Cor_Assests;
                psr.colorAccentsDark = cg.OLA_Cor_Assets_Dark;
                psr.colorFont = cg.OLA_Cor_Fonte;
                psr.colorPrinc = cg.OLA_Cor_Principal;
                psr.colorPrincDark = cg.OLA_Cor_Principal_Dark;
                psr.nome = cg.Nome_Cliente_Sinapse;
                psr.token = cg.TokenOla;
                psr.tokenOperacao = resposta.message;
                psr.status = "A";
                psr.uuidProvedor = cg.Uuid_Provedor_Ola;
                psr.ambienteTeste = cg.Ambiente_Teste;
                psr.utilizarEsquemaCoresPadraoOla = cg.Utilizar_Esquema_Cores_Padrao_Ola;

                try
                {
                    psr.urlConnect = cg.Url_Sistemas.ToList().Find(u => u.Sistema == Model.enumerado.Sistema.Connect).URL;
                }
                catch { psr.urlConnect = ""; }

                try
                {
                    psr.urlEva = cg.Url_Sistemas.ToList().Find(u => u.Sistema == Model.enumerado.Sistema.EVA).URL;
                }
                catch { psr.urlEva = ""; }

                try
                {
                    psr.urlInnovation = cg.Url_Sistemas.ToList().Find(u => u.Sistema == Model.enumerado.Sistema.Innovation).URL;
                }
                catch { psr.urlInnovation = ""; }

                try
                {
                    psr.urlTouch = cg.Url_Sistemas.ToList().Find(u => u.Sistema == Model.enumerado.Sistema.TOUCH).URL;
                }
                catch { psr.urlTouch = ""; }

                Response provedorResponse = new BaseController("", "").PostOla<ProvedorSetRequest>(url, "api/Provedor/InsertOrUpdate", psr);



            }
            else
            {
                throw new Exception(resposta.message);
            }
        }
    }
}
