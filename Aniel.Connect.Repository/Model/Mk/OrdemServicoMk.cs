using System;

namespace Aniel.Connect.Repository.Model.Mk
{
    /// <summary>
    ///     Dados de uma Ordem de Serviço consultada diretamente na base de dados do MK Solutions
    /// </summary>
    public class OrdemServicoMkResponse
    {
        public string NumOS { get; set; }
        public string CodCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneFixo { get; set; }
        public string Email { get; set; }
        public string Contato { get; set; }
        public string DescricaoCondominio { get; set; }
        public string UsuarioSac { get; set; }
        public string SenhaSac { get; set; }
        public string CodContrato { get; set; }
        public DateTime? DataAbertura { get; set; }
        public TimeSpan? HoraAbertura { get; set; }
        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string UF { get; set; }
        public string CodConexao { get; set; }
        public string UsuarioPPPoE { get; set; }
        public string SenhaPPPoE { get; set; }
        public string PontoAcesso { get; set; }
        public string PlanoAcesso { get; set; }
        public string PlanoCrm { get; set; }
        public string CodPlano { get; set; }
        public string Mac { get; set; }
        public string UsuarioWpa { get; set; }
        public string SenhaWpa { get; set; }
        public string TipoOs { get; set; }
        public string TipoOsDescricao { get; set; }
        public string DefeitoDescricao { get; set; }
        public string DefeitoReclamado { get; set; }
        public string SlaTipo { get; set; }
        public string Observacoes { get; set; }
        public string Indicacoes { get; set; }
        public string Status { get; set; }
        public string Encerrado { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CdAtendimento { get; set; }
        public string Operador { get; set; }
        public string PortCode { get; set; }
        public string CtoDescription { get; set; }
    }
}
