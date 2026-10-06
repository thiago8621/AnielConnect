using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.T5.Model.enumerado;
using System;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class NotificacaoBusiness
    {
        public string AplicarVariaveis(string textoNotificacao, Documento_ProducaoAniel documento, string parametro = null, Atendimento atendimento = null)
        {
            string retorno = String.Copy(textoNotificacao);
            if (textoNotificacao.Contains("@NUMERO_OS"))
            {
                retorno = retorno.Replace("@NUMERO_OS", documento.Numero_Obra_Original);
            }
            if (textoNotificacao.Contains("@NUM_OS"))
            {
                retorno = retorno.Replace("@NUM_OS", documento.Numero_Obra_Original);
            }
            if (textoNotificacao.Contains("@TECNICO"))
            {
                retorno = retorno.Replace("@TECNICO", documento.EquipeAniel?.Nome);
            }
            if (textoNotificacao.Contains("@NOME_TECNICO"))
            {
                retorno = retorno.Replace("@NOME_TECNICO", documento.EquipeAniel?.Nome);
            }
            if (textoNotificacao.Contains("@NOME_MONITOR"))
            {
                retorno = retorno.Replace("@NOME_MONITOR", documento.EquipeAniel?.Monitor);
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
            if (textoNotificacao.Contains("@DATA_HORA_DESLOCAMENTO"))
            {
                retorno = retorno.Replace("@DATA_HORA_DESLOCAMENTO", documento.DataHora_Deslocamento.HasValue ? documento.DataHora_Deslocamento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@DATA_HORA_INTERROMPEU"))
            {
                retorno = retorno.Replace("@DATA_HORA_INTERROMPEU", documento.DataHora_Interromper_Deslocamento.HasValue ? documento.DataHora_Interromper_Deslocamento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@DATA_HORA_PARALISACAO"))
            {
                retorno = retorno.Replace("@DATA_HORA_PARALISACAO", documento.DataHora_Pausou.HasValue ? documento.DataHora_Pausou.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@DATA_HORA_ROTERIZOU"))
            {
                retorno = retorno.Replace("@DATA_HORA_ROTERIZOU", documento.Hora.HasValue ? documento.Hora.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty);
            }
            if (textoNotificacao.Contains("@QTDE_SERVICOS"))
            {
                retorno = retorno.Replace("@QTDE_SERVICOS", parametro);
            }
            if (textoNotificacao.Contains("@PROTOCOLO")) 
            {
                retorno = retorno.Replace("@PROTOCOLO", !String.IsNullOrEmpty(atendimento?.Protocolo) ? atendimento?.Protocolo: string.Empty);
            }
            return retorno;
        }

        public string AplicarVariaveisAtendimento(string textoNotificacao, Atendimento atendimento, string parametro)
        {
            string retorno = String.Copy(textoNotificacao);
            if (textoNotificacao.Contains("@NOME_CLIENTE"))
            {
                retorno = retorno.Replace("@NOME_CLIENTE", atendimento.Cliente.NomeFantasia);
            }
            if (textoNotificacao.Contains("@CPF_CLIENTE"))
            {
                retorno = retorno.Replace("@CPF_CLIENTE", atendimento.Cliente?.CpfCnpj);
            }
            if (textoNotificacao.Contains("@PROTOCOLO"))
            {
                retorno = retorno.Replace("@PROTOCOLO", atendimento.Protocolo);
            }
            if (textoNotificacao.Contains("@SETOR_ENVOLVIDO"))
            {
                retorno = retorno.Replace("@SETOR_ENVOLVIDO", atendimento.Setor.Descricao);
            }
            if (textoNotificacao.Contains("@DATA_HORA"))
            {
                retorno = retorno.Replace("@DATA_HORA", atendimento.DataHoraAlteracao.HasValue ? atendimento.DataHoraAlteracao.Value.ToString("dd/MM/yyyy") : string.Empty);
            }
            if (textoNotificacao.Contains("@NUM_OS"))
            {
                retorno = retorno.Replace("@NUM_OS", parametro);
            }
            if (textoNotificacao.Contains("@NOME_USUARIO_ACAO"))
            {
                retorno = retorno.Replace("@NOME_USUARIO_ACAO", atendimento.Responsavel?.Funcionario?.Nome != null ? atendimento.Responsavel?.Funcionario?.Nome : string.Empty);
            }
            

            return retorno;
        }


        public string AplicarVariaveisComercial(string textoNotificacao, Venda venda, string parametro)
        {
            string retorno = String.Copy(textoNotificacao);
            if (textoNotificacao.Contains("@NOME_VENDEDOR"))
            {
                retorno = retorno.Replace("@NOME_VENDEDOR", venda.Vendedor.Nome);
            }
            if (textoNotificacao.Contains("@QTDE_VENDAS_CANCELADAS"))
            {
                retorno = retorno.Replace("@QTDE_VENDAS_CANCELADAS", parametro);
            }
            if (textoNotificacao.Contains("@QTDE_VENDAS_APROVADAS"))
            {
                retorno = retorno.Replace("@QTDE_VENDAS_APROVADAS", parametro);
            }
            if (textoNotificacao.Contains("@QTDE_NOVOS_PROSPECTOS"))
            {
                retorno = retorno.Replace("@QTDE_NOVOS_PROSPECTOS", parametro);
            }
            if (textoNotificacao.Contains("@DATA_DIA_ANTERIOR"))
            {
                retorno = retorno.Replace("@DATA_DIA_ANTERIOR", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            }
            if (textoNotificacao.Contains("@TITULO_CAMPANHA"))
            {
                retorno = retorno.Replace("@TITULO_CAMPANHA", parametro);
            }
            if (textoNotificacao.Contains("@NOME_PROSPECTO"))
            {
                retorno = retorno.Replace("@NOME_PROSPECTO", parametro);
            }
            if (textoNotificacao.Contains("@NUMERO_VENDA"))
            {
                retorno = retorno.Replace("@NUMERO_VENDA", venda.Id.ToString());
            }



            return retorno;
        }


    }
}
