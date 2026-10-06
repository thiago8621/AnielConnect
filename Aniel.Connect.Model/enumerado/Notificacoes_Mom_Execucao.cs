using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Notificacoes_Mom_Execucao
    {
        [Display(Name = "EV_VALIDA_CPF", ResourceType = typeof(Resources.RsCadastros))]
        EV_VALIDA_CPF = 0,

        [Display(Name = "EV_VALIDA_TIPO_PESSOA", ResourceType = typeof(Resources.RsCadastros))]
        EV_VALIDA_TIPO_PESSOA = 1,

        [Display(Name = "EV_CONSULTA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        EV_CONSULTA_VENDA = 2,

        [Display(Name = "EV_ENVIA_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        EV_ENVIA_CONTRATO = 3,

        [Display(Name = "EV_CANCELA_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        EV_CANCELA_PLANO = 4,

        [Display(Name = "EV_CONSULTA_BLOQUEIO", ResourceType = typeof(Resources.RsCadastros))]
        EV_CONSULTA_BLOQUEIO = 5,

        //[Display(Name = "EV_SEGUNDA_VIA", ResourceType = typeof(Resources.RsCadastros))]
        //EV_SEGUNDA_VIA = 6,

        [Display(Name = "EV_FATURAS_ABERTO", ResourceType = typeof(Resources.RsCadastros))]
        EV_FATURAS_ABERTO = 7,


        [Display(Name = "EV_ENVIA_PESQUISA", ResourceType = typeof(Resources.RsCadastros))]
        EV_ENVIA_PESQUISA = 8,

        [Display(Name = "EV_FINALIZA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        EV_FINALIZA_ATENDIMENTO = 9,

        [Display(Name = "EV_CONSULTA_OS", ResourceType = typeof(Resources.RsCadastros))]
        EV_CONSULTA_OS = 10,

        [Display(Name = "EV_CONSULTA_MAN_REDE", ResourceType = typeof(Resources.RsCadastros))]
        EV_CONSULTA_MAN_REDE = 11,

        [Display(Name = "EV_TESTE_CONEXAO", ResourceType = typeof(Resources.RsCadastros))]
        EV_TESTE_CONEXAO = 12,
                
        
        [Display(Name = "EV_REINICIA_SINAL", ResourceType = typeof(Resources.RsCadastros))]
        EV_REINICIA_SINAL = 13,

        //[Display(Name = "EV_RESET_ONU", ResourceType = typeof(Resources.RsCadastros))]
        //EV_RESET_ONU = 14,

        [Display(Name = "EV_DESBL_CONFIANCA", ResourceType = typeof(Resources.RsCadastros))]
        EV_DESBL_CONFIANCA = 15,

        [Display(Name = "EV_EXISTE_CPF", ResourceType = typeof(Resources.RsCadastros))]
        EV_EXISTE_CPF = 16,

        [Display(Name = "EV_TRANSFERE_CHAT", ResourceType = typeof(Resources.RsCadastros))]
        EV_TRANSFERE_CHAT = 17,

        [Display(Name = "EV_CONSULTA_PLANOS", ResourceType = typeof(Resources.RsCadastros))]
        EV_CONSULTA_PLANOS = 18,

        [Display(Name = "EV_ARMAZENA", ResourceType = typeof(Resources.RsCadastros))]
        EV_ARMAZENA = 19,

        [Display(Name = "EV_LISTA_BOLETOS", ResourceType = typeof(Resources.RsCadastros))]
        EV_LISTA_BOLETOS = 20

    }
}
