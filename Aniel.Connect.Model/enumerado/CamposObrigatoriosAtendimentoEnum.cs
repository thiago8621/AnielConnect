using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum CamposObrigatoriosAtendimentoEnum
    {

        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "ASSUNTO", ResourceType = typeof(Resources.RsCadastros))]
        ASSUNTO = 0,
        [Display(Name = "CATEGORIA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        CATEGORIA_ATENDIMENTO = 1,
        [Display(Name = "ORIGEM", ResourceType = typeof(Resources.RsCadastros))]
        ORIGEM = 2,
        [Display(Name = "PESSOA_QUE_ENTROU_EM_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        PESSOA_QUE_ENTROU_EM_CONTATO = 3,
        [Display(Name = "PRIORIDADE", ResourceType = typeof(Resources.RsCadastros))]
        PRIORIDADE = 4,

        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "PROBLEMA_RELATADO", ResourceType = typeof(Resources.RsCadastros))]
        PROBLEMA_RELATADO = 5,

        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "SETOR_ENVOLVIDO", ResourceType = typeof(Resources.RsCadastros))]
        SETOR_ENVOLVIDO = 6,
        [Display(Name = "ORGANIZACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        ORGANIZACAO_CLIENTE = 7
    }
}
