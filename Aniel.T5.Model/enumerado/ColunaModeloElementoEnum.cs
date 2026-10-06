using System.ComponentModel.DataAnnotations;


namespace Aniel.T5.Model.enumerado
{
    public enum ColunaModeloElementoEnum
    {
        [Display(Name = "CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        Contrato = 0,
        [Display(Name = "PROJETO", ResourceType = typeof(Resources.RsCadastros))]
        Projeto = 1,
        [Display(Name = "POP", ResourceType = typeof(Resources.RsCadastros))]
        POP = 2,
        [Display(Name = "OLT", ResourceType = typeof(Resources.RsCadastros))]
        OLT = 3,
        [Display(Name = "SLOT", ResourceType = typeof(Resources.RsCadastros))]
        SLOT = 4,
        [Display(Name = "PON", ResourceType = typeof(Resources.RsCadastros))]
        PON = 5,
        [Display(Name = "CAPACIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Capacidade = 6,
        [Display(Name = "FABRICANTE", ResourceType = typeof(Resources.RsCadastros))]
        Fabricante = 7,
        [Display(Name = "MODELO", ResourceType = typeof(Resources.RsCadastros))]
        Modelo = 8,
        [Display(Name = "OBSERVACAO", ResourceType = typeof(Resources.RsCadastros))]
        Observacao = 9,
        [Display(Name = "LATITUDE", ResourceType = typeof(Resources.RsCadastros))]
        Latitude = 10,
        [Display(Name = "LONGITUDE", ResourceType = typeof(Resources.RsCadastros))]
        Longitude = 11,
        [Display(Name = "CEP", ResourceType = typeof(Resources.RsCadastros))]
        CEP = 12,
        [Display(Name = "ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        Endereco = 13,
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        Numero = 14,
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Complemento = 15,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 16,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 17,
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        Bairro = 18,
        [Display(Name = "POSTE", ResourceType = typeof(Resources.RsCadastros))]
        Poste = 19,
        [Display(Name = "CABO", ResourceType = typeof(Resources.RsCadastros))]
        Cabo = 20,
        [Display(Name = "METRAGEM", ResourceType = typeof(Resources.RsCadastros))]
        Metragem = 21,
        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsCadastros))]
        Pais = 22,

    }
}
