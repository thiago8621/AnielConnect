namespace Aniel.Connect.API_Aniel.Models
{
    public class ConsultaCreditoByCpfApiRequest : ApiRequest
    {
        public string Cpf { get; set; }
        public string IdVendedor { get; set; }
        public long? IdVenda { get; set; }
    }

    public class ConsultaCreditoByCnpjApiRequest : ApiRequest
    {
        public string Cnpj { get; set; }
        public string IdVendedor { get; set; }
        public long? IdVenda { get; set; }
    }

    public class ConsultaCreditoApiResponse
    {
        /// <summary>
        ///     Status da consulta de crédito com base na pontuação retornada
        ///     (Aprovado com Instalação, Aprovado sem Instalação, Aguardando Gestor ou Reprovado)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///     Pontuação da consulta de crédito
        /// </summary>
        public int Pontos { get; set; }

        /// <summary>
        ///     String retornada pela API do Serasa, de onde a pontuação é extraída
        /// </summary>
        public string Retorno_Consulta { get; set; }
        /// <summary>
        ///     String passada no parametro Loja do PreQual
        /// </summary>
        public string ParamPreQual { get; set; }

        public long Id { get; set; }

        public Model.enumerado.IntegracaoExterna Tipo_Integracao { get; set; }
    }
}