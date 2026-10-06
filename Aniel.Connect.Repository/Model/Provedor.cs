using System;
namespace Aniel.Connect.Repository.Model
{
    public class ProvedorSetRequest
    {
        public String cnpj { get; set; }
        public String nome { get; set; }
        public String status { get; set; }
        public String tokenOperacao { get; set; }
        public String token { get; set; }
        public String caminhoLogo { get; set; }
        public String urlConnect { get; set; }
        public String urlEva { get; set; }
        public String urlTouch { get; set; }
        public String urlInnovation { get; set; }
        public String colorPrinc { get; set; }
        public String colorPrincDark { get; set; }
        public String colorAccents { get; set; }
        public String colorAccentsDark { get; set; }
        public String colorFont { get; set; }

        public string uuidProvedor { get; set; }

        public bool ambienteTeste { get; set; }
        public bool utilizarEsquemaCoresPadraoOla { get; set; }

    }
    public class RadGruposIXCResponse
    {
        public string id { get; set; }
        public string vd_contratos_produtos_id_vd_contrato { get; set; }
        public string grupo { get; set; }
        public string download { get; set; }
        public string upload { get; set; }
        public string tipo { get; set; }
        public string cliente_contrato_id { get; set; }
    }

    public class VdContratosProdutosIXCResponse
    {
        public string descricao_plano_valor_1 { get; set; }
        public string descricao_plano_valor_2 { get; set; }
        public string fixar_ip { get; set; }
        public string tipo_desconto { get; set; }
        public string desconto_percentual { get; set; }
        public string repetir { get; set; }
        public string id { get; set; }
        public string tipo { get; set; }
        public string id_plano { get; set; }
        public string id_vd_contrato { get; set; }
        public string id_produto { get; set; }
        public string qtde { get; set; }
        public string valor_unit { get; set; }
        public string descricao { get; set; }
        public string id_contrato { get; set; }
        public string obs { get; set; }
        public string valor_ate_vencimento { get; set; }
        public string qtde_repeticoes_desconto_produto { get; set; }
        public string valor_desconto_produto { get; set; }
        public string ultima_atualizacao { get; set; }
        public string id_tipo_documento { get; set; }
    }
}
