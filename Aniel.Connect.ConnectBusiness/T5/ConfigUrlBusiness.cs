using Aniel.Connect.Repository.ConfigUrl;
using Aniel.Connect.Repository.T5.Natureza;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ConfigUrlBusiness
    {
        private readonly IConfigUrlRepository _repoConfigUrl;

        public ConfigUrlBusiness(IConfigUrlRepository repoConfig)
        {
            this._repoConfigUrl = repoConfig;
        }

        /// <summary>
        /// Busca na tabela de URLS pela url do connect
        /// </summary>
        /// <returns></returns>
        public string GetUrlConnect()
        {
            return _repoConfigUrl.GetUrlConnect();
        }

        /// <summary>
        /// Busca na tabela de URLS pela url do PREMIER
        /// </summary>
        /// <returns></returns>
        public string GetUrlPremier()
        {
            return _repoConfigUrl.GetUrlPremier();
        }

        /// <summary>
        /// Busca na tabela de URLS pela url do API_ANIEL
        /// </summary>
        /// <returns></returns>
        public string GetUrlApiAniel()
        {
            return _repoConfigUrl.GetUrlApiAniel();
        }

        /// <summary>
        /// Busca na tabela de URLS pela url do FINANCEIRO
        /// </summary>
        /// <returns></returns>
        public string GetUrlFinanceiro()
        {
            return _repoConfigUrl.GetUrlApiFinanceiro();
		}

        public string GetUrlIntegra()
        {
            return _repoConfigUrl.GetUrlIntegra();
        }

        /// <summary>
        /// Busca na tabela de URLS pela url do API_ANIEL
        /// </summary>
        /// <returns></returns>
        public string GetUrlApiEVA()
        {
            return _repoConfigUrl.GetUrlEVA();
		}

        /// <summary>
        /// Busca na tabela de URLS pela url do OSRM
        /// </summary>
        /// <returns></returns>
        public string GetUrlOSRM()
        {
            return _repoConfigUrl.GetUrlOSRM();
        }
    }
}
