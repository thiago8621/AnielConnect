using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    /// Lista de Campos referentes a Funcionario
    /// </summary>
    /// 
    public class ConsultaMotivoReagendamento
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual int? Cod_Motivo_Reagendamentos { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Descricao { get; set; }
        public virtual int? Status { get; set; }

    }

    public class OsApi
    {
        public virtual String Cpf { get; set; }
        public virtual String Codigo_Categoria { get; set; }
        public virtual Int64 TipoAtendimento { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Assunto { get; set; }
        public virtual Prioridade_Atendimento Prioridade { get; set; }
    }
    public class OsApiConsulta
    {
        public virtual String UsuarioAlteracao { get; set; }
        public virtual String DiaAlteracao { get; set; }
        public virtual String HoraAlteracao { get; set; }
        public virtual String Dia { get; set; }
        public virtual String Hora { get; set; }
        public virtual String CodContrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String NumeroObra { get; set; }
        public virtual String NumeroDocumento { get; set; }
        public virtual String NumeroProtocolo { get; set; }
        public virtual String CodigoCliente { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Cpf { get; set; }
        public virtual String Rg { get; set; }
        public virtual String RgAssinante { get; set; }
        public virtual String EmailAssinante { get; set; }
        public virtual String VinculoCliente { get; set; }
        public virtual String DocumentoCliente { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String EnderecoNumero { get; set; }
        public virtual String EnderecoComplemento { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }
        public virtual String EnderecoTipo { get; set; }
        public virtual String Logitude { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String CodArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String CodLocalidade { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String CodEscritorio { get; set; }
        public virtual String Escritorio { get; set; }
        public virtual String TelCliente { get; set; }
        public virtual String TelTitular { get; set; }
        public virtual String Contato { get; set; }
        public virtual String ContatoTelefone { get; set; }
        public virtual String TelComercial { get; set; }
        public virtual String TelInstalacao { get; set; }
        public virtual String CodPlano { get; set; }
        public virtual String IdPlanoProdutoConnect { get; set; }
        public virtual String Plano { get; set; }
        public virtual String PlanoAssinante { get; set; }
        public virtual String Local { get; set; }
        public virtual String Login { get; set; }
        public virtual String Dslan { get; set; }
        public virtual String Ppoe { get; set; }
        public virtual String Porta { get; set; }
        public virtual String CaixaDe { get; set; }
        public virtual String CaixaPara { get; set; }
        public virtual String CaboDe { get; set; }
        public virtual String CaboPara { get; set; }
        public virtual String Estacao { get; set; }
        public virtual String EstacaoPara { get; set; }
        public virtual String Ssde { get; set; }
        public virtual String SsPara { get; set; }
        public virtual String Eqnde { get; set; }
        public virtual String EqnPara { get; set; }
        public virtual String ParprimPara { get; set; }
        public virtual String StatusProd { get; set; }
        public virtual String Licde { get; set; }
        public virtual String Licpara { get; set; }
        public virtual String EndCaixa { get; set; }
        public virtual String EndArm { get; set; }
        public virtual String Roterizar { get; set; }
        public virtual String DataHoraDeslocamento { get; set; }
        public virtual String DataHoraInicioAtendimento { get; set; }
        public virtual String DataHoraFimAtendimento { get; set; }
        public virtual String UsuarioCancelou { get; set; }
        public virtual String DataHoraCancelou { get; set; }
        public virtual String TempoRealDeslocamento { get; set; }
        public virtual String TempoDeslocamento { get; set; }
        public virtual String GeoSegmento { get; set; }
        public virtual String DataProd { get; set; }
        public virtual String Validado { get; set; }
        public virtual String HoraMaxima { get; set; }
        public virtual String DataMaxima { get; set; }
        public virtual String CodTipoServico { get; set; }
        public virtual String TipoServico { get; set; }
        public virtual String CodSubTipoServico { get; set; }
        public virtual String SubTipoServico { get; set; }
        public virtual String CodEncerramento { get; set; }
        public virtual String HoraEncerramento { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String UsuarioBaixa { get; set; }
        public virtual String UnidadeCusto { get; set; }
        public virtual String NumRVT { get; set; }
        public virtual String Tarefa { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Ard { get; set; }
        public virtual String TipoNota { get; set; }
        public virtual String Comunidade { get; set; }
        public virtual String NumDocEntrega { get; set; }
        public virtual String Posto { get; set; }
        public virtual String TemTesteEletrico { get; set; }
        public virtual String TesteEletrico { get; set; }
        public virtual String ObsTeste { get; set; }
        public virtual String DataIndicador { get; set; }
        public virtual String Insapoioss { get; set; }
        public virtual String ObsGasto { get; set; }
        public virtual String Oxbdpee { get; set; }
        public virtual String SiglaModulo { get; set; }
        public virtual String Origem { get; set; }
        public virtual String Segmento { get; set; }
        public virtual String HoraIndicador { get; set; }
        public virtual String NumeroTarefa { get; set; }
        public virtual String TipoReclamacao { get; set; }
        public virtual String Seq { get; set; }
        public virtual String NumObraOriginal { get; set; }
        public virtual String StatusExecucao { get; set; }
        public virtual String ProtocoloNe { get; set; }
        public virtual String CodTurnoServ { get; set; }
        public virtual String Devolvida { get; set; }
        public virtual String ObsAux { get; set; }
        public virtual String Lnbf { get; set; }
        public virtual String Grupo { get; set; }
        public virtual String ObsDocumento { get; set; }
        public virtual String Tx { get; set; }
        public virtual String Rx { get; set; }
        public virtual String TipoBaixa { get; set; }
        public virtual String RetornoTeclo { get; set; }
        public virtual String SenhaBaixa { get; set; }
        public virtual String Responsabilidade { get; set; }
        public virtual String DataHoraPassou { get; set; }
        public virtual String IdAtendimentoConnect { get; set; }
        public virtual String Adicional1 { get; set; }
        public virtual String Adicional2 { get; set; }
        public virtual String Adicional3 { get; set; }
        public virtual String Adicional4 { get; set; }
        public virtual String Adicional5 { get; set; }
        public virtual String Adicional6 { get; set; }
        public virtual String Adicional7 { get; set; }
        public virtual String Adicional8 { get; set; }
        public virtual String Adicional9 { get; set; }
        public virtual String Adicional10 { get; set; }
        public virtual String ObsPausa { get; set; }
        public virtual String Wo { get; set; }
        public virtual String IdVenda { get; set; }
        public virtual String CodSupervisor { get; set; }
        public virtual String PortaFtth { get; set; }
        public virtual String CaixaFtth { get; set; }
        public virtual String Produto { get; set; }
        public virtual String PontoAcesso { get; set; }
        public virtual String NomeInstalacao { get; set; }
        public virtual String StatusAuditoria { get; set; }
        public virtual String ObsOitv { get; set; }
        public virtual String TemOsDigital { get; set; }
        public virtual String WifiPassword { get; set; }
        public virtual String WifiSsid { get; set; }
    }
    public class OsConsultaApiRequest : ApiRequest
    {

        ///<summary> Tipo de Serviço </summary>
        public virtual String TipoServico { get; set; }
        ///<summary> Projeto da Ordem de Serviço </summary>
        public virtual String Projeto { get; set; }
        ///<summary> Contrato da Ordem de Serviço </summary>
        public virtual String CodCt { get; set; }
        ///<summary> Número da Ordem de Serviço </summary>
        public virtual String NumObra { get; set; }
        public virtual Int64? NumDoc { get; set; }
        public virtual String Equipe { get; set; }
        public string TipoImovel { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }
        public string Escritorio { get; set; }
        public string Bairro { get; set; }
        public string UF { get; set; }
        public string Dia { get; set; }
        public string DataMaxima { get; set; }
        public string Hora { get; set; }
        public string HoraMaxima { get; set; }
        public Int64? IdCliente { get; set; }
        public string Num_Obra_Original { get; set; }
        public string Status_Execucao { get; set; }
        public string Responsabilidade { get; set; }
        public Int64? IdAtendimentoConnect { get; set; }
        public Int64? IdVenda { get; set; }
        public Int64? IdLogin { get; set; }
        public virtual String SubTipoServico { get; set; }

    }

    public class AjustarDataAtendimentoApiRequest : ApiRequest
    {

        /// <summary> Inicio da Busca </summary>         
        public virtual String DataInicio { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String DataFim { get; set; }


    }

    public class AgendamentoApiRequest : ApiRequest
    {
        [Required]
        public virtual String Os { get; set; }
        [Required]
        public virtual String Contrato { get; set; }
        [Required]
        public virtual String Projeto { get; set; }
        [Required]
        public virtual int CodigoCliente { get; set; }
        [Required]
        public virtual String Data { get; set; }
        [Required]
        public virtual String Janela { get; set; }
        public String ObsMotivoCancelamento { get; set; }
        public Int64? CodMotivoReagendamento { get; set; }
    }

    public class AlterarOsApiRequest : ApiRequest
    {
        public  virtual ChavesDocumentoProducao Filtros { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public string Tecnico { get; set; }
        public string Data_HoraAgendamento { get; set; }
        public string Periodo { get; set; }
        public string TipoServico { get; set; }
        public string SubtipoServico { get; set; }
        public string Segmento { get; set; }
        public string Setor { get; set; }
        public string Prioridade { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public string TipoImovel { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DetalhesOS { get; set; }
        public string InformacoesComplementares { get; set; }
        public string ObservacaoTeste { get; set; }
        public ResponsabilidadeOS? Responsabilidade { get; set; }
        public CamposAdicionais CamposAdicionais { get; set; }
    }
   

    public class DocumentoProducaoRequestModels : ApiRequest
    {
        [Required]
        public virtual String Os { get; set; }

        public virtual String Contrato { get; set; }

        public virtual String Projeto { get; set; }

        public virtual int CodigoCliente { get; set; }
    }

    public class DocumentoProducaoParalisarRequestModels : DocumentoProducaoRequestModels
    {
        public virtual String Justificativa { get; set; }
    }

    public class DocumentoProducaoBaixaRequestModels : ApiRequest
    {
        [Required]
        public virtual String Os { get; set; }
        [Required]
        public virtual String Contrato { get; set; }
        [Required]
        public virtual String Projeto { get; set; }
        [Required]
        public virtual int CodigoCliente { get; set; }
        public virtual String Observacao { get; set; }
        [Required]
        public virtual String Codigo_Encerramento { get; set; }
        [Required]
        public virtual String Equipe { get; set; }
        [Required]
        public virtual String Data_Ini_Atend { get; set; }
        [Required]
        public virtual String Data_Fim_Atend { get; set; }
        [Required]
        public virtual String Hora_Ini_Atend { get; set; }
        [Required]
        public virtual String Hora_Fim_Atend { get; set; }
        public virtual String CaixaFTTH { get; set; }
        public virtual String PortaFTTH { get; set; }
    }

    public class DocumentoProducaoLog
    {
        public string codCt { get; set; }
        public string projeto { get; set; }
        public string numObra { get; set; }
        public string numObraOriginal { get; set; }
        public string numDoc { get; set; }
        public string dataMaxima { get; set; }
        public string usuarioCriacao { get; set; }
        public string diaCriacao { get; set; }
        public string horaCriacao { get; set; }
        public string usuarioExclusao { get; set; }
        public string ipMaquina { get; set; }
        public string nomeMaquina { get; set; }
        public string obs { get; set; }
    }

    public class ConsultaDisponibilidadeAgendamentoApiRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String Data { get; set; }
    }

    public class ConsultaDisponibilidadeAgendamentoPeriodoApiRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String DataFim { get; set; }
    }

    public class ConsultaDisponibilidadeAgendamentoBaseRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String Data { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String DataFim { get; set; }

    }
    public class ConsultaMotivoReagendamentoRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual int? Cod_Motivo_Reagendamentos { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Descricao { get; set; }
        public virtual int? Status { get; set; }

    }

    public class ConsultaDisponibilidadeAgendamentoDaysRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String DataFim { get; set; }

    }

    public class ConsultaDisponibilidadeAgendamentoTimesRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String Data { get; set; }

    }

    public class ConsultaDisponibilidadeAgendamentoNearestRequest : ApiRequest
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String GrupoArea { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String TipoServico { get; set; }
        /// <summary> Contrato da Ordem de Serviço </summary>
        public virtual String Contrato { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String DataFim { get; set; }

    }

    public class ConsultaDisponibilidadeAgendamentoApiResponse
    {
        /// <summary> Inicio da Busca </summary>         
        public virtual String Horario { get; set; }
        /// <summary> Fim da Busca </summary>         
        public virtual bool Disponivel { get; set; }
        public virtual AgendamentoHorasDisponiveis AgendamentoModelDetalhes { get; set; }
        public virtual List<String> DiasHoras { get; set; }
        public virtual String dia { get; set; }
    }

    public class TimesApiResponse
    {

        public virtual String description { get; set; }

        public virtual String value { get; set; }
    }

}