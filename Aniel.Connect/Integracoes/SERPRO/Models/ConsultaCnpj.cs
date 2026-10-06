using System.Collections.Generic;

namespace Aniel.Connect.Integracoes.SERPRO.Models
{
    public class ConsultaCnpj
    {
        public string ni { get; set; }
        public string tipoEstabelecimento { get; set; }
        public string nomeEmpresarial { get; set; }
        public string nomeFantasia { get; set; }
        public Situacao situacaoCadastral { get; set; }
        public NaturezaJuridicaConsulta naturezaJuridica { get; set; }
        public string dataAbertura { get; set; }
        public CnaePrincipalConsulta cnaePrincipal { get; set; }
        public List<CnaeSecundariaConsulta> cnaeSecundarias { get; set; }
        public EnderecoConsulta endereco { get; set; }
        public MunicipioJurisdicaoConsulta municipioJurisdicao { get; set; }
        public List<TelefoneConsulta> telefones { get; set; }
        public string correioEletronico { get; set; }
        public int capitalSocial { get; set; }
        public string porte { get; set; }
        public string situacaoEspecial { get; set; }
        public string dataSituacaoEspecial { get; set; }
    }
}