using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class ConstantesIntegracaoMk
    {
        public string CodigoEmpresa { get; set; }
        public string CodigoTipoPlano { get; set; }
        public string CodigoPlanoAcesso { get; set; }
        public string IgnorarRenovacaoAutomatica { get; set; }
        public string CodigoRegraBloqueio { get; set; }
        public string CodigoFormaPagamento { get; set; }
        public string CodigoProfilePagamento { get; set; }
        public string CodigoMetodoFaturamento { get; set; }
        public string CodigoMetodoFaturamentoCortesia { get; set; }
        public string CodigoPlanoContas { get; set; }
        public string AguardaAtivacao { get; set; }
        public IEnumerable<CodigoRegraVencimento> CodigosRegraVencimento { get; set; }
        public string CodigoPontoAcesso { get; set; }
        public string CodigoServidor { get; set; }
        public string CodigoTipoIP { get; set; }
        public string Tecnologia { get; set; }
        public string AutoDesbloqueio { get; set; }
        public string MacAddress { get; set; }
        public string EnderecoCadastro { get; set; }
        public string TipoConexao { get; set; }
        public string PoolMKCentral { get; set; }
        public string SlaResidencial { get; set; }
        public string SlaCorporativo { get; set; }
        public string DataConnection { get; set; }
        public string TpAutenticacao { get; set; }
        public int? TempoDelayConexao { get; set; }
        public string UtilizaCodigoPlanoIntegracaoAuxiliar { get; set; }
        public string CodigoTecnico { get; set; }
        public string CodigoGrupoServico { get; set; }
        public string ProcessoPessoaFisica { get; set; }
        public string ProcessoPJ { get; set; }
        public string CodigoClassificacaoAtendimento { get; set; }
    }

    public class CodigoRegraVencimento
    {
        /// <summary>
        ///     Código da regra de vencimento no MK
        /// </summary>
        public string Codigo { get; set; }
        
        /// <summary>
        ///     Dia de vencimento correspondente
        /// </summary>
        public string DiaVencimento { get; set; }
    }
}
