using Aniel.Connect.Data.Model;
using Aniel.Connect.Repository.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aniel.Connect.Data
{
    /// <summary>
    ///     Classe para acessar dados locais, armazenados na pasta "Data" do próprio projeto do Connect
    /// </summary>
    public class LocalData
    {
        /// <summary>
        ///     Verifica um JSON local para trazer o código do IBGE da cidade informada
        /// </summary>
        /// <param name="uf">Sigla da UF onde a cidade se encontra</param>
        /// <param name="cidade">Nome da cidade</param>
        /// <returns></returns>
        public static string GetCodigoIBGECidade(string uf, string cidade)
        {
            string codigoIbge = null;
            List<DadosCodigoIbgeCidade> dadosCodigoIbge = null;

            using (var streamReader = new StreamReader(CONSTANTES.CaminhoFisico + "/Data/ListaCodigoIBGECidades.json"))
            {
                string fileContent = streamReader.ReadToEnd();
                dadosCodigoIbge = JsonService.DeserializeObject<List<DadosCodigoIbgeCidade>>(fileContent);
            }

            if (dadosCodigoIbge != null)
            {
                uf = uf.ToUpperInvariant();
                cidade = cidade.ToUpperInvariant();

                DadosCodigoIbgeCidade dadosCidadeIbge =
                    dadosCodigoIbge
                        .Where(d => d.uf.ToUpperInvariant() == uf &&
                                    d.cidade.ToUpperInvariant() == cidade
                        ).FirstOrDefault();

                if (dadosCidadeIbge != null)
                    codigoIbge = Convert.ToString(dadosCidadeIbge.codigoIBGE);
            }

            return codigoIbge;
        }

        /// <summary>
        ///     Lê as informações constantes para a integração com o MK de um JSON local
        /// </summary>
        public static ConstantesIntegracaoMk GetConstantesIntegracaoMk()
        {
            ConstantesIntegracaoMk dadosIntegracaoMk = null;

            using (var streamReader = new StreamReader(CONSTANTES.CaminhoFisico + "/DadosIntegracaoMk.json"))
            {
                string fileContent = streamReader.ReadToEnd();
                dadosIntegracaoMk = JsonService.DeserializeObject<ConstantesIntegracaoMk>(fileContent);
            }

            return dadosIntegracaoMk;
        }

        /// <summary>
        ///     Lê as informações constantes para a integração com o IXC de um JSON local
        /// </summary>
        public static ConstantesIntegracaoIXC GetConstantesIntegracaoIXC()
        {
            ConstantesIntegracaoIXC dadosIntegracaoMk = null;

            using (var streamReader = new StreamReader(CONSTANTES.CaminhoFisico + "/DadosIntegracaoIXC.json"))
            {
                string fileContent = streamReader.ReadToEnd();
                dadosIntegracaoMk = JsonService.DeserializeObject<ConstantesIntegracaoIXC>(fileContent);
            }

            return dadosIntegracaoMk;
        }
    }
}