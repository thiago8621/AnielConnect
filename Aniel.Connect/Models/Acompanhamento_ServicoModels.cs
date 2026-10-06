using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Padrao = Sinapse.Model.enumerados.Padrao;

namespace Aniel.Connect.Models
{
    public class DadosGerais_CadastroModel : IViewModel<String>
    {
        public virtual Int64 Id_Cliente { get; set; }
        public virtual Int64? Id_Venda { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual String Cpf_Cnpj { get; set; }
        public virtual String Identificacao_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        [AllowHtml]
        public virtual String Observacao_Aux { get; set; }
        public virtual String InformacoesComplementares { get; set; }
        public virtual bool ConfirmaEnderecoAgendamento { get; set; }

        public virtual String Observacao_Pausa { get; set; }

        public virtual String Tempo_Cliente { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Email { get; set; }

        public virtual Int64? Id_PlanoProduto { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual Int32? Cod_Tipo_Imovel { get; set; }
        public virtual String Tipo_Imovel { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }

        public virtual String Ponto_Referencia { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String Cod_Localidade { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Grupo_Area { get; set; }
        public virtual String Cod_Escritorio { get; set; }

        public virtual String Tipo_Servico { get; set; }
        public virtual Int32? Id_Tipo_Servico { get; set; }
        public virtual String TipoServico { get; set; }
        public virtual String DescTipoManutencao { get; set; }
        public virtual String Subtipo_Servico { get; set; }
        public virtual String Id_Subtipo_Servico { get; set; }
        public virtual String Segmento { get; set; }
        public virtual Origem_Documento_Producao Origem_Ordem_Servico { get; set; }
        public virtual String Id_Origem_Ordem_Servico { get; set; }
        public virtual String Natureza { get; set; }
        [AllowHtml]
        public virtual String Obs_Criacao_Servico { get; set; }


        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Num_Doc { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual Status_AuditoriaAniel? Status_Auditoria { get; set; }

        public virtual StatusExecucaoAniel? Status_Execucao { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual ResponsabilidadeOS? ResponsabilidadeOS { get; set; }
        public virtual String Status_OS { get; set; }
        public virtual String Numero_Atendimento { get; set; }

        public virtual String SLA_Atual { get; set; }
        public virtual String Hora_Indicador { get; set; }
        public virtual String Data_Indicador { get; set; }

        public virtual String Exige_Pagamento_Instalacao { get; set; }

        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual String ErroApiMaps { get; set; }

        public virtual String Id { get; set; }
        public virtual int? Id_Turno { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Desc_Turno { get; set; }
        public virtual String Departamento { get; set; }

        public virtual String Longitude_Str { get; set; }
        public virtual String Latitude_Str { get; set; }

        public virtual String Adcional_1 { get; set; }

        public virtual String Adcional_2 { get; set; }

        public virtual String Adcional_3 { get; set; }

        public virtual String Adcional_4 { get; set; }

        public virtual String Adcional_5 { get; set; }

        public virtual String Adcional_6 { get; set; }

        public virtual String Adcional_7 { get; set; }

        public virtual String Adcional_8 { get; set; }

        public virtual String Adcional_9 { get; set; }

        public virtual String Adcional_10 { get; set; }
        public virtual String NomeInstalacao { get; set; }
        public virtual String CpfInstalacao { get; set; }
        public virtual String TelefoneInstalacao { get; set; }
        public virtual String Nome_Tecnologia { get; set; }
        public virtual Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }
        public virtual String Tipo { get; set; }
        public virtual Int64? Id_Pop { get; set; }
        public virtual Int64? Id_Olt { get; set; }
        public virtual Int64? Id_Elemento_Rede { get; set; }
        public virtual Tipo_PrioridadeT5? Prioridade_Cliente { get; set; }
        public virtual String IdDepartamento { get; set; }
        public virtual String DescricaoContrato { get; set; }
        public virtual String Redirecionamento { get; set; }
        public virtual String CodCt_Manut { get; set; }
        public virtual String Num_Obra_Manut { get; set; }
        public virtual Int32 numDoc_Manut { get; set; }

        public virtual String Projeto_Manut { get; set; }
        public virtual String Codigo_Motivo_Reagendamento { get; set; }
        public virtual String Observacao_Motivo_Reagendamento { get; set; }
        public virtual long? IdCondominioPredioEndereco { get; set; }
        public virtual String Id_Tipo_Imovel { get; set; }
        public virtual String Bloco { get; set; }
        public virtual String Apartamento { get; set; }
        public virtual Boolean UtilizaEndeCondo { get; set; }
        public virtual int? BuscarEnderecoPorId { get; set; }
        public virtual Int64? Id_Troca_Plano { get; set; }
        public virtual String Descricao_Plano_Anterior { get; set; }

        #region [Mudança de Endereço]
        public virtual Boolean Endereco_Mudanca { get; set; }
        public virtual String Cod_Localidade_Antigo { get; set; }
        public virtual String Cod_Area_Antigo { get; set; }
        public virtual String Cod_Escritorio_Antigo { get; set; }
        public virtual String Nome_Tecnologia_Antigo { get; set; }
        public virtual String CodCt_Antigo { get; set; }
        public virtual String Projeto_Antigo { get; set; }
        public virtual String DescricaoContrato_Antigo { get; set; }
        public virtual String Ponto_Referencia_Antigo { get; set; }
        public virtual String Longitude_Str_Antigo { get; set; }
        public virtual String Latitude_Str_Antigo { get; set; }
        public virtual String Tipo_Imovel_Antigo { get; set; }
        public virtual Int32? Cod_Tipo_Imovel_Antigo { get; set; }
        public virtual String Complemento_Antigo { get; set; }
        public virtual String Bloco_Antigo { get; set; }
        public virtual String Apartamento_Antigo { get; set; }
        public virtual String Pais_Antigo { get; set; }
        public virtual String Uf_Antigo { get; set; }
        public virtual String Cidade_Antigo { get; set; }
        public virtual String Bairro_Antigo { get; set; }
        public virtual String Numero_Antigo { get; set; }
        public virtual String Cep_Antigo { get; set; }
        public virtual String Logradouro_Antigo { get; set; }
        public virtual long? IdCondominioPredioEndereco_Antigo { get; set; }
        public virtual Int32? BuscarEnderecoPorId_Antigo { get; set; }
        #endregion
    }

    public class AcompanhamentoListaModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Id_Status_Cobranca { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Num_OS { get; set; }
        public virtual TipoManutencaoOs? TipoManutencaoOs { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Num_OS_Original { get; set; }
        public virtual String Status_OS { get; set; }
        public virtual String SLA_Atual { get; set; }
        public virtual String Agendamento { get; set; }
        public virtual String Periodo { get; set; }
        public virtual String Data_Prod { get; set; }
        public virtual String Unidade_Custo { get; set; }
        public virtual String Observacao_Aux { get; set; }
        public virtual String TipoServico { get; set; }
        public virtual StatusExecucaoAniel? Status_Execucao { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual Status_AuditoriaAniel? Status_Auditoria { get; set; }
        public virtual Padrao? Tipo_Servico_Equipe { get; set; }
        public virtual long? IdCliente { get; set; }
        public virtual long? IdPlanoProduto { get; set; }
        public virtual int? CodTipoServ { get; set; }
        public virtual string Tipo { get; set; }
        public virtual long? Id_Troca_Plano { get; set; }
        public virtual string Descricao_Plano_Anterior { get; set; }
        public virtual Boolean Endereco_Mudanca { get; set; }
        public virtual String Id_Caixa_FTTH { get; set; }
        public virtual String Id_Porta_FTTH { get; set; }
        public virtual String IdAtendimento { get; set; }
        public virtual int? CodNatureza { get; set; }
        public virtual String DataHora_TipoManutencaoPreventiva { get; set; }
        public virtual String DataHora_TipoManutencaoCorretiva { get; set; }
    }

    public class CheckListRequest
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumObra { get; set; }
        public string NumDoc { get; set; }
    }


    public class CancelamentoVisita
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Descricao_Cancelamento { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Token { get; set; }
    }

    public class TecnicoCadastroModel : IViewModel<String>
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Id { get; set; }
        public virtual String Matricula { get; set; }
        public virtual String Matricula_Nome { get; set; }
        public virtual String Matricula_Id { get; set; }
        public virtual Padrao? Exige_Pagamento_Instalcao { get; set; }
        public virtual String Matricula_Aux { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Tipo_Serv { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Email { get; set; }
        public virtual String Centro_Custo { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Tipo_Baixa { get; set; }
        public virtual String Data_Hora_Encerramento { get; set; }
        public virtual String Cod_Desc_Encerramento { get; set; }
        public virtual String Qualidade_Servico { get; set; }
        public virtual String Sinal_RX { get; set; }
        public virtual Padrao? Exige_Pagamento_Instalacao { get; set; }
        public virtual String Sinal_TX { get; set; }
        public virtual String Custo_Servico { get; set; }
        public virtual String CaixaFTTH { get; set; }
        public virtual String PortaFTTH { get; set; }
        public virtual String NovaPorta { get; set; }
        public virtual String NovaCaixa { get; set; }
        [AllowHtml]
        public virtual String Observacao_Tecnico { get; set; }
        public virtual StatusExecucaoAniel? Status_Execucao { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual Int64 Id_Atendimento { get; set; }
        public virtual String Foto { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual ResponsabilidadeOS? ResponsabilidadeOS { get; set; }
        public virtual String Redirecionamento { get; set; }
        public virtual String Projeto_Manut { get; set; }
    }

    //public class Materiais_AplicadosListaModel
    //{

    //}

    //public class Materiais_RetiradosListaModel
    //{

    //}

    //public class Prod_LancadaListaModel
    //{

    //}
    public class Historico_ServicoCadastroModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }

        public virtual String Num_Obra { get; set; }
        public virtual Int32? Numero_Doc { get; set; }
        public virtual String TempoRealDeslocamento { get; set; }
        public virtual String TempoAtendimento { get; set; }
        public virtual String TempoTotalParalisacoes { get; set; }
        public virtual String TempoTotalAtendimento { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual String CustoTotalServico { get; set; }

        private List<Historico_ServicoListaModel> historico_servico;

        public virtual List<Historico_ServicoListaModel> Historico_Servico
        {
            get
            {
                if (historico_servico == null)
                    historico_servico = new List<Historico_ServicoListaModel>();
                return historico_servico;
            }
            set
            {
                historico_servico = value;
            }
        }

    }


    public class Historico_ServicoListaModel
    {
        public virtual Int32? Id { get; set; }
        public virtual String Status_OS { get; set; }
        public virtual String DataHora { get; set; }
        public virtual String DataHora_Servidor { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String NumObra { get; set; }
        public virtual String TipoMudanca { get; set; }
        public virtual String Valor_Atual { get; set; }
        public virtual String Valor_Alterado { get; set; }
        public virtual String Obs_Cobranca_Status { get; set; }
        public virtual StatusExecucaoAniel? Status_Documento { get; set; }
        public virtual String IndexStatus { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual Int64? CodMotivoReagendamento { get; set; }
        public virtual String ObsMotivoReagendamento { get; set; }
    }

    public class ChecklistListaModel
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Perguntas { get; set; }
        public string Respondidas { get; set; }
        public string UsuarioCriacao { get; set; }
        public string DataHoraCriacao { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public string DataHoraUltimaAlteracao { get; set; }
    }


    public class Fotos_ArquivosCadastroModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Numero_Doc { get; set; }
        public virtual String Arquivo { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class Fotos_ArquivosListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tabela { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Cod_ct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_doc { get; set; }
    }
    public class ImagemQuizRequest
    {
        public virtual string IdQuestionAnswer { get; set; }
        public virtual string IdQuestionAnswerCapa { get; set; }
        public virtual string IdQuestionAnswerItem { get; set; }
        public virtual string IdQuestion { get; set; }

    }

    public class Caminho_Foto_Arquivo
    {
        public virtual String Tipo { get; set; }
        public virtual String Foto { get; set; }
    }

    public class Fotos_ArquivosDataTableRequest : DataTableRequest
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
    }

    public class AlterarObservacaoTecnico
    {
        public virtual String codCt { get; set; }
        public virtual String projeto { get; set; }
        public virtual String num_obra { get; set; }
        public virtual Int32? num_doc { get; set; }
        public virtual String observacao_teste { get; set; }
    }

    public class AlterarObservacaoPausa
    {
        public virtual String codct { get; set; }
        public virtual String projeto { get; set; }
        public virtual String num_obra { get; set; }
        public virtual Int32? num_doc { get; set; }
        public virtual String observacao_pausa { get; set; }
    }

    public class Historico_ServicoDataTableRequest : DataTableRequest
    {
        public virtual String Id { get; set; }
        public virtual String Num_OS { get; set; }
        public virtual String Status_OS { get; set; }
        public virtual String SLA_Atual { get; set; }
        public virtual String Agendamento { get; set; }

        public virtual String Equipe { get; set; }
        public virtual DateTime? Data_Criacao { get; set; }
        public virtual DateTime? Data_Agendamento { get; set; }
        public virtual DateTime? Data_Roteirizacao { get; set; }
        public virtual DateTime? Data_Execuçao { get; set; }
        public virtual String Usuario { get; set; }
    }    

    public class VincularAtendimento_DataTableRequest : DataTableRequest
    {
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual StatusExecucaoAniel? Status { get; set; }
        public virtual Status_SLA_Atendimento? Sla { get; set; }

        public String DataInicial { get; set; }
        public String DataFinal { get; set; }

    }

    public class VincularAtendimentoResponse
    {
        public virtual Int64? id { get; set; }
        public virtual Int64? venda { get; set; }

        public virtual String protocolo { get; set; }
        public virtual String vendedor { get; set; }
        public virtual String supervisor { get; set; }
        public virtual String status_venda { get; set; }
        public virtual String termo_assinado { get; set; }

        public virtual String status { get; set; }
        public virtual String motivoStatus { get; set; }
        public virtual String tipoAtendimento { get; set; }
        public virtual String responsavel { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String assunto { get; set; }
        public virtual String dataSla { get; set; }
        public virtual String numero { get; set; }
        public virtual String cliente { get; set; }


    }
    public class HistoricoVincularAtendimentoResponse
    {
        public virtual Int64? id { get; set; }
        public virtual Int64? venda { get; set; }

        public virtual String protocolo { get; set; }
        public virtual String vendedor { get; set; }
        public virtual String supervisor { get; set; }
        public virtual String status_venda { get; set; }
        public virtual String termo_assinado { get; set; }

        public virtual String status { get; set; }
        public virtual String motivoStatus { get; set; }
        public virtual String tipoAtendimento { get; set; }
        public virtual String responsavel { get; set; }
        public virtual String dataHoraCriacao { get; set; }
        public virtual String assunto { get; set; }
        public virtual String dataSla { get; set; }
        public virtual String numero { get; set; }
        public virtual String cliente { get; set; }


    }

    public class HistoricoVincularOSResponse_DataTableRequest : DataTableRequest
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Num_Obra { get; set; }

    }

    
}