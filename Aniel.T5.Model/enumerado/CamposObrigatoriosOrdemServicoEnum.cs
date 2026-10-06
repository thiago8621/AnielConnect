using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum CamposObrigatoriosOrdemServicoEnum
    {
        [Display(Name = "BLUNDE", ResourceType = typeof(Resources.RsCadastros))]
        BLUNDE = 0,
        [Display(Name = "CAMINHO_FIBRA", ResourceType = typeof(Resources.RsCadastros))]
        CAMINHO_FIBRA = 1,
        [Display(Name = "DGO", ResourceType = typeof(Resources.RsCadastros))]
        DGO = 2,
        [Display(Name = "ESTACAO", ResourceType = typeof(Resources.RsCadastros))]
        ESTACAO = 3,
        [Display(Name = "ICX", ResourceType = typeof(Resources.RsCadastros))]
        ICX = 4,
        [Display(Name = "IPTV", ResourceType = typeof(Resources.RsCadastros))]
        IPTV = 5,
        [Display(Name = "NODE", ResourceType = typeof(Resources.RsCadastros))]
        NODE = 6,
        [Display(Name = "OBSERVACAO_CRIACAO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        OBSERVACAO_CRIACAO_SERVICO = 7,
        [Display(Name = "OLT", ResourceType = typeof(Resources.RsCadastros))]
        OLT = 8,
        [Display(Name = "ORIGEM", ResourceType = typeof(Resources.RsCadastros))]
        ORIGEM = 9,
        [Display(Name = "PPPOE", ResourceType = typeof(Resources.RsCadastros))]
        PPPOE = 10,
        [Display(Name = "QTDE_IPTV", ResourceType = typeof(Resources.RsCadastros))]
        QTDE_IPTV = 11,
        [Display(Name = "QTDE_RFTV", ResourceType = typeof(Resources.RsCadastros))]
        QTDE_RFTV = 12,
        [Display(Name = "RESPONSAVEL_OS", ResourceType = typeof(Resources.RsCadastros))]
        RESPONSAVEL_OS = 13,
        [Display(Name = "SEGMENTO", ResourceType = typeof(Resources.RsCadastros))]
        SEGMENTO = 14,
        [Display(Name = "SENHA_BAIXA", ResourceType = typeof(Resources.RsCadastros))]
        SENHA_BAIXA = 15,
        [Display(Name = "SINAL_RX", ResourceType = typeof(Resources.RsCadastros))]
        SINAL_RX = 16,
        [Display(Name = "SINAL_TX", ResourceType = typeof(Resources.RsCadastros))]
        SINAL_TX = 17,
        [Display(Name = "SUB_TIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        SUB_TIPO_SERVICO = 18
    }
}
