using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.T5.Model.enumerado;
using System;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class NotificacaoBusiness
    {
        public string AplicarVariaveis(string textoNotificacao, Documento_ProducaoAniel documento)
        {
            string retorno = String.Copy(textoNotificacao);
            if (textoNotificacao.Contains("@NUMERO_OS"))
            {
                retorno = retorno.Replace("@NUMERO_OS", documento.Numero_Obra_Original);
            }
            if (textoNotificacao.Contains("@TECNICO"))
            {
                retorno = retorno.Replace("@TECNICO", documento.EquipeAniel?.Nome);
            }
            if (textoNotificacao.Contains("@DATA_AGENDAMENTO"))
            {
                if (documento.Data_Maxima.HasValue && documento.Hora_Maxima.HasValue == false)
                    retorno = retorno.Replace("@DATA_AGENDAMENTO", documento.Data_Maxima.HasValue ? documento.Data_Maxima.Value.ToString("dd/MM/yyyy") : string.Empty);
                if (documento.Data_Maxima.HasValue && documento.Hora_Maxima.HasValue)
                    retorno = retorno.Replace("@DATA_AGENDAMENTO", documento.Data_Maxima.HasValue ? documento.Data_Maxima.Value.Add(documento.Hora_Maxima.Value.TimeOfDay).ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@TIPO_SERVICO"))
            {
                retorno = retorno.Replace("@TIPO_SERVICO", documento.TipoServico != null ? documento.TipoServico.Descricao : string.Empty);
            }
            if (textoNotificacao.Contains("@PRAZO"))
            {
                retorno = retorno.Replace("@PRAZO", documento.Data_Indicador.HasValue && documento.Hora_Indicador.HasValue ? documento.Data_Indicador?.Date.Add(documento.Hora_Indicador.Value.TimeOfDay).ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@STATUS"))
            {
                retorno = retorno.Replace("@STATUS", HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(documento.Status.Value));
            }
            if (textoNotificacao.Contains("@CLIENTE"))
            {
                retorno = retorno.Replace("@CLIENTE", documento.Cliente?.NomeFantasia ?? documento.Nome_Cliente);
            }
            if (textoNotificacao.Contains("@CPF_CLIENTE"))
            {
                retorno = retorno.Replace("@CPF_CLIENTE", documento.Cliente?.CPF ?? documento.CPF_CLIENTE);
            }

            return retorno;
        }
    }
}
