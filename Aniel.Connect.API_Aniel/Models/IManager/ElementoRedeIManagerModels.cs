using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models.IManager
{
    public class EstruturaRedeResponse
    {
        public List<PopResponse> pops { get; set; }
    }

    public class PopResponse
    {
        public string nome { get; set; }
        public long codigo { get; set; }
        public List<OLTResponse> olts { get; set; }
    }

    public class OLTResponse
    {
        public string nome { get; set; }
        public long codigo { get; set; }
        public List<SlotResponse> slots { get; set; }
    }

    public class SlotResponse
    {
        public string nome { get; set; }
        public long codigo { get; set; }
        public List<PonResponse> pons { get; set; }
    }

    public class PonResponse
    {
        public string nome { get; set; }
        public long codigo { get; set; }
        public List<CTOResponse> ctos { get; set; }
    }

    public class CTOResponse
    {
        public string nome { get; set; }
        public long idcto { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int? qtdportas { get; set; }
        public int? qtdportasdisponiveis { get; set; }
        /// <summary>
        /// Status da CTO no IManager. Valores: ativada / desativada
        /// </summary>
        public string statuscto { get; set; }
        public List<PortaCTOResponse> portascto { get; set; }
    }

    public class PortaCTOResponse
    {
        public long? idportacto { get; set; }
        /// <summary>
        /// Status de disponiblidade da porta no IManager. Valores: ocupada / desocupada
        /// </summary>
        public string disponibilidade { get; set; }
    }

    public class ReservarPortaRequest
    {
        /// <summary>
        /// Id da Assinatura do Cliente (plano) no IManager. Campo utilizado para realizar uma reserva da porta (originada por Venda).
        /// </summary>
        public long? idAssinatura { get; set; }
        /// <summary>
        /// Id do Contrato do Cliente no IManager. Campo utilizado para realizar a ocupação da porta (realizada pelo técnico).
        /// </summary>
        public long? idContrato { get; set; }
        public long idCto { get; set; }
        public long? numPorta { get; set; }
    }

    public class ReservarPortaResponse
    {
        public bool reservaEfetuada { get; set; }
        public string mensagem { get; set; }
    }

    public class LimparPortaRequest
    {
        public long? idAssinatura { get; set; }
        public long? idContrato { get; set; }
    }

    public class LimparPortaResponse
    {
        public bool cancelamentoEfetuado { get; set; }
        public bool reservaCancelada { get; set; }
        public string mensagem { get; set; }
    }

    public class ConsultaCTOIndividualRequest
    {
        public string chavebase { get; set; }
        public string usuario { get; set; }
        public long? idcontrato { get; set; }
        public long? idassinatura { get; set; }
        public long? idcto { get; set; }
        public long? idportacto { get; set; }
    }

    public class ConsultaCTOIndividualResponse
    {
        public long? idportacto { get; set; }
        /// <summary>
        /// Corresponde ao idContrato/idAssinatura ocupando a porta no momento.
        /// </summary>
        public long? idocupacao { get; set; }
        public long? portafisica { get; set; }
        public long? portalogica { get; set; }
        public long? idcto { get; set; }
        public string nomecto { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string logradouro_cto { get; set; }
        public long? codigocidade { get; set; }
        public string nomecidade { get; set; }
        public long? idpon { get; set; }
        public string nomepon { get; set; }
        public long? idslot { get; set; }
        public string nomeslot { get; set; }
        public long? idolt { get; set; }
        public string nomeolt { get; set; }
        public long? idpop { get; set; }
        public string nomepop { get; set; }
    }



    public class WebHookCTORequest
    {
        public long? idfila { get; set; }
        public long? idposte { get; set; }
        public long? idportacto { get; set; }
        public int portafisica { get; set; }
        public int portalogica { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string logradouro_cto { get; set; }
        public long codigocidade { get; set; }
        public string nomecidade { get; set; }        
        public Contrato_AssinaturaRequest contrato_assinatura { get; set; }
    }

    public class Contrato_AssinaturaRequest
    {
        public string condicao { get; set; }
        public long? idcontrato { get; set; }
        public long? idassinatura { get; set; }
        public string nomecliente { get; set; }
        public string cpfcnpj { get; set; }
        public string logradouro_conexao { get; set; }
        public string cep_conexao { get; set; }
        public string cidade_conexao { get; set; }
    }

}