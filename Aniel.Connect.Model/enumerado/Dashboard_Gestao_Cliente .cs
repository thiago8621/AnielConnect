using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Gestao_Cliente
    {
        [Display(Name = "EVOLUCAO_CARTEIRA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Evolucao_Carteira_Cliente = 0,
        [Display(Name = "CARTEIRA_CLIENTE_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Carteira_Cliente_Localidade = 1,
        [Display(Name = "CARTEIRA_ATUAL_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        Carteira_Atual_Plano = 2,
        [Display(Name = "ATIVACAO_PERDA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Ativacao_Perda_Cliente = 3,
        [Display(Name = "CARTEIRA_CLIENTE_CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        Carteira_Cliente_Categoria = 4,
        [Display(Name = "CARTEIRA_CLIENTE_FAIXA_VALOR", ResourceType = typeof(Resources.RsCadastros))]
        Carteira_Cliente_Faixa_Valor = 5,
        [Display(Name = "CARTEIRA_CLIENTE_TEMPO_PERMANENCIA", ResourceType = typeof(Resources.RsCadastros))]
        Carteira_Cliente_Tempo_Permacencia = 6,
        [Display(Name = "CHAT", ResourceType = typeof(Resources.RsCadastros))]
        Chat = 7,
        [Display(Name = "NOVIDADES_SINAPSE", ResourceType = typeof(Resources.RsCadastros))]
        Novidades_Sinapse = 8
    }
}