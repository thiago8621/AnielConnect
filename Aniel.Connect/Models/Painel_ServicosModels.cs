using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class Painel_ServicosListaFiltroModel
    {
        public Boolean ExibirFuncionariosDemitidos { get; set; }
        public Boolean ExibirSomenteOSComVenda { get; set; }
        public Int64? Id_Venda { get; set; }
        public Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }

        public String Natureza_OS { get; set; }

        public String DataInicial { get; set; }
        public String DataFinal { get; set; }


        public String Tipo_Servico { get; set; }
        public String SubTipo_Servico { get; set; }

        public String Endereco_Cidade { get; set; }
        public String Endereco_Bairro { get; set; }
        public String Endereco_UF { get; set; }

        public StatusExecucaoAniel? Status_OS { get; set; }
        public EquipeStatusTipo? Status_Equipe { get; set; }
        public Status_AuditoriaAniel? Status_Auditoria { get; set; }
        public String IdPeriodo { get; set; }

        public String Id_Equipe { get; set; }
        public String Nome_Equipe { get; set; }

        public String Id_Supervisor { get; set; }
        public String Nome_Supervisor { get; set; }


        public String Id_Monitor { get; set; }
        public String Nome_Monitor { get; set; }

        public String Grupo_Area { get; set; }
        public String Area { get; set; }
        public String Localidade { get; set; }
        public Status_SLA_Horas_Servico? StatusHorasSLA { get; set; }

        public Status_SLA_Servico? StatusSLA { get; set; }
        public ResponsabilidadeOS? Responsabilidade { get; set; }
        public Status_CobrancaAniel? Status_Cobranca { get; set; }

        public Status_Cobranca? Status{ get; set; }

        public Status_Cobranca? StatusAtividadeAprovada { get; set; }
        public virtual Atividade_Decisoria? Atividade_Decisoria { get; set; }

        public virtual Boolean UtilizaHistoricoReabertura { get; set; }

    }

    public class Painel_ServicosListaModel
    {

        public String Numero_Cliente { get; set; }
        public String Equipe_Matricula { get; set; }
        public String Equipe_Auxiliares { get; set; }

        public String Cpf_Cnpj_Cliente { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Numero_Obra { get; set; }
        public int QuantidadeReabertura { get; set; }
        public String Numero_Obra_Original { get; set; }
        public Int32 Num_Doc { get; set; }

        public String Id_Venda { get; set; }
        public String Id_Atendimento { get; set; }

        //Codigo do cliente gerado no cadastro do Connect
        public String Codigo_Cliente { get; set; }
        public String Nome_Cliente { get; set; }
        public String Endereco_Cliente { get; set; }
        public String Endereco_Cidade { get; set; }
        public String Endereco_Bairro { get; set; }
        public String Endereco_UF { get; set; }
        public String Endereco_Pais { get; set; }
        public String Endereco_Complemento { get; set; }
        public String Endereco_Numero { get; set; }

        public String Atendimento_Finalizado { get; set; }
        public String Status_Responsavel { get; set; }
        public String Telefone_Principal_Cliente { get; set; }
        public String Celular_Cliente { get; set; }

        //Aniel
        public String Tipo_Servico { get; set; }
        public String Cod_Tipo_Servico { get; set; }
        public String SubTipo_Servico { get; set; }

        //Status_OS é o mesmo do MSG
        public String Status_OS { get; set; }

        //Responsavel (Equipe Aniel)
        public String Responsavel { get; set; }

        //Tipo de Imovel (Referente ao Cliente)
        public String Tipo_Imovel { get; set; }

        //Supervisor (funcionario vinculado como supervisor (Equipe_Aniel)
        public String Supervisor { get; set; }

        //Aniel - Grupo_Area (Campo escritorio Aniel)
        public String Grupo_Area { get; set; }
        public String Cod_Grupo_Area { get; set; }
        public String Area { get; set; }
        public String Cod_Area { get; set; }
        public String Cod_Localidade { get; set; }
        public String Localidade { get; set; }

        //Novo Conceito
        public String Codigo_Encerramento { get; set; }
        public String Descricao_Encerramento { get; set; }

        //Se for pelo OLA deverá aparecer "OLA CLIENTE"
        public String Usuario_Abertura_OS { get; set; }
        public String DataHora_Abertura_OS { get; set; }
        public String DataHora_Abertura_OS_Original { get; set; }

        //Se for pelo OLA deverá aparecer "OLA CLIENTE"
        public String Usuario_Agendamento_OS { get; set; }
        public String Data_Hora_Agendamento_OS { get; set; }
        public String Data_Hora_Inicio { get; set; }
        public String Data_Hora_Final { get; set; }
        public String Data_Hora_Deslocamento { get; set; }

        public String DataHora_Encerramento_OS { get; set; }
        public String DataHora_Vencimento_OS { get; set; }
        public String DataHora_Deslocamento_OS { get; set; }
        public String DataHora_IniAtendimento_OS { get; set; }
        public String DataHora_Pausou_OS { get; set; }
        public String DataHora_Reinicio_OS { get; set; }
        public String DataHora_FimAtendimento_OS { get; set; }
        public String Status_Execucao { get; set; }
        public String Responsabilidade { get; set; }
        public String Roterizar { get; set; }

        public String Usuario_Encerramento { get; set; }

        public String Contrato { get; set; }
        public String CategoriaCliente { get; set; }
        public String Monitor { get; set; }
        public String Natureza { get; set; }
        public String PlanoProduto { get; set; }
        public String Sla { get; set; }
        public String Pontos_Previstos { get; set; }
        public String Pontos_Realizados { get; set; }
        public String Observacao { get; set; }
        public String ObservacaoTeste { get; set; }
        public String Periodo { get; set; }
        public String Status_Cobranca { get; set; }
        public String Status_Auditoria { get; set; }

        public Status_Cobranca? Status { get; set; }

        public Status_Cobranca? StatusAtividadeAprovada { get; set; }
        public virtual Atividade_Decisoria? Atividade_Decisoria { get; set; }

        public String Observacao_Pausa { get; set; }
        public String LatitudeLongitude { get; set; }
        public String Seq { get; set; }
        public String Wo { get; set; }
        public String TemOsDigital { get; set; }
        public String Tipo_Prioridade { get; set; }
        public String Departamento { get; set; }
        public String Segmento { get; set; }

    }

    public class Painel_ServicosCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
    }

    public class Servico_ModeloAttribute : Attribute
    {
        public Filtro_PainelServicoEnum Recurso { get; set; }
        public Servico_ModeloAttribute(Filtro_PainelServicoEnum value)
        {
            Recurso = value;
        }
    }


    public class VincularOsViewModel
    {
        public List<String> ids { get; set; }
        public String idOsManutencao { get; set; }
        public String usuario { get; set; }
        public String Projeto_Manut { get; set; }
        public String CodCt_Manut { get; set; }
        public String Num_Obra_Manut { get; set; }
        public Int32 numDoc_Manut { get; set; }

    }


    public class AlterarResponsavelViewModel
    {
        public List<String> ids { get; set; }
        public String equipe { get; set; }
        public String usuario { get; set; }
    }


    public class AlterarResponsabilidadeModel
    {
        public List<String> ids { get; set; }
        public virtual ResponsabilidadeOS responsabilidade { get; set; }
    }

    public class AutorizarBaixaModel
    {
        public List<String> ids { get; set; }        
    }

    public class EncaixarRoteirizacaoOsModel
    {
        public List<String> ids { get; set; }
        public virtual ResponsabilidadeOS responsabilidade { get; set; }
        public String data { get; set; }
    }

    public class AdicionarInformacaoComplementarModel
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; } 
        public String Informacao { get; set; }
        public Int32 numDoc { get; set; }
        
    }

    public class AdicionarInformacaoComplementarVendaModel
    {
        public long IdVenda { get; set; }
        public long IdClientePlanoProduto { get; set; }
        public long IdPlanoProduto_Troca { get; set; }
        public String Informacao { get; set; }
    }

    public class AlterarStatusModel
    {
        public List<String> ids { get; set; }
        public string justificativa { get; set; }
    }
    public class AlterarVinculoManutencao

    {
        public String Projeto_Manut { get; set; }
        public String CodCt_Manut { get; set; }
        public String Num_Obra_Manut { get; set; }
        public Int32 numDoc_Manut { get; set; }

        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public String Informacao { get; set; }
        public Int32 numDoc { get; set; }
    }

    public class DocumentoProducao_AtribuirEquipe
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public Int32? Num_Obra_Original { get; set; }
        public String Equipe { get; set; }
        public String Nome_Equipe { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public StatusExecucaoAniel statusExecucaoAniel { get; set; }
    }

    public class Painel_Servicos_DataTableRequest : DataTableRequest
    {
        public Boolean ExibirFuncionariosDemitidos { get; set; }
        public Boolean ExibirSomenteOSComVenda { get; set; }
        public Int64? Id_Venda { get; set; }

        public Int64? Id_Modelo { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Natureza_Producao)]
        public virtual String Id_Natureza { get; set; }
        public virtual String Natureza_OS { get; set; }

        public virtual Int32? TipoData { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual String DataVencimento { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Contrato)]
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Tipo_Servico)]
        public virtual String Id_Tipo_Servico { get; set; }
        public virtual String Tipo_Servico { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Sub_Tipo_Servico)]
        public virtual String Id_Sub_Tipo_Servico { get; set; }
        public virtual String SubTipo_Servico { get; set; }


        public virtual String Estado { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Cidade)]
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.UF)]
        public virtual String Endereco_UF { get; set; }
        [Servico_Modelo(Filtro_PainelServicoEnum.Status)]
        public virtual String StatusOS { get; set; }

        public virtual String StatusEquipe { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Status_Cobranca)]
        public virtual String Status_Cobranca { get; set; }
        public virtual String Status_Auditoria { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Responsavel)]
        public virtual String Matricula_Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Supervisor_Tecnico)]
        public virtual String Id_Supervisor { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Monitor_Tecnico)]
        public virtual String Id_Monitor { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.GrupoArea)]
        public virtual String Grupo_Area { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Area)]
        public virtual String Area { get; set; }

        [Servico_Modelo(Filtro_PainelServicoEnum.Localidade)]

        public virtual String Localidade { get; set; }
        public virtual String SituacaoOS { get; set; }
        [Servico_Modelo(Filtro_PainelServicoEnum.SLA)]
        public String Sla { get; set; }
        public String Responsabilidade { get; set; }
        public String ConfigDemanda { get; set; }
        public String ConfigRoteirizados { get; set; }
        public String HabilidadeNatureza { get; set; }
        public String DataInicioDemanda { get; set; }
        [Servico_Modelo(Filtro_PainelServicoEnum.Periodo)]
        public String IdPeriodo { get; set; }
        public String TemOsDigital { get; set; }
        public String TipoPrioridade { get; set; }
        [Servico_Modelo(Filtro_PainelServicoEnum.SLA)]
        public String StatusSla { get; set; }

        public Int64? Id_Pop { get; set; }
        public Int64? Id_Olt { get; set; }
        public Int64? Id_ElementoRede { get; set; }

        public String CategoriaCliente { get; set; }
        public String CodDepto { get; set; }
        [Servico_Modelo(Filtro_PainelServicoEnum.UsuarioAbertura)]
        public String UsuarioCriacao { get; set; }
        public Boolean UtilizaHistoricoReabertura { get; set; }
        public string Tipo { get; set; }
        public long? Id_Organizacao { get; set; }
        public Comparativo_QuantidadeEnum? OperadorFiltro { get; set; }
        public int? ValorInicialReabertura{ get; set; }
        public int? ValorFinalReabertura{ get; set; }
    }

    public class LogAlteracaoOS_DataTableRequest : DataTableRequest
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        // Os request.model devem ter o mesmo nome no data do datatable.
        //data: function(data)
        //{
        //    data.CodCt = '@Request.Params["codCt"]';
        //    data.Projeto = '@Request.Params["projeto"]';
        //    data.Numero_Obra = '@Request.Params["num_Obra"]';
        //    data.Num_Doc = '@Request.Params["num_Doc"]';
        //},
    }

    public class LogAlteracaoOSListaModel
    {
        //public virtual String Num_Obra { get; set; }

        public virtual String Num_Obra { get; set; }

        public virtual String Usuario { get; set; }

        public virtual String Dia { get; set; }

        public virtual String Hora { get; set; }

        public virtual String Tipo { get; set; }

        public virtual String Valor_Ant { get; set; }

        public virtual String Valor_Atual { get; set; }
    }
}