using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using System.Data.Common;

namespace Aniel.Connect.Repository.Repository.MK
{
    public interface IMKProvisionamentoRepository
    {

        /// <summary>
        ///    Vincula conexão do cliente inserindo informações de Caixa e Porta
        /// </summary>
        /// <param name="nomeCaixa">Nome da caixa onde equipamento está conectado</param>
        ///  <param name="numPorta">Número da Porta onde o equipamento está conectado</param>
        ///   <param name="usuario">Login PPPoE do cliente</param>
        ///    <param name="onuSerial">Serial da ONU</param>
        Task<RetornoMK> VinculaConexao(string nomeCaixa, int? numPorta, string usuario, string onuSerial);

        /// <summary>
        ///    Remove vínculo da conexão do cliente da Caixa e Porta
        /// </summary>
        ///   <param name="usuario">Login PPPoE do cliente</param>

        Task<RetornoMK> DesvinculaConexao(string usuario);
    }
}
