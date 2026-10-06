using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Connect.Models
{

    public class Timeline_ServicosModels
    {
        public virtual Int32 TipoFiltro { get; set; }
        public virtual Tipo_Prioridade? Tipo_Prioridade { get; set; }
        public virtual Int32 TipoVisualizacao { get; set; }
        public virtual Int32 FormatoTimeline { get; set; }
        public virtual Int32? Prazo { get; set; }

        public virtual String Num_Fre_Supervisor { get; set; }
        public virtual String Id_Monitor { get; set; }
        public virtual String SubTipoServico { get; set; }
        public virtual String TipoServico { get; set; }

        public virtual String Tecnico { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Cod_Escritorio { get; set; }

        public virtual String Cod_Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }

        public virtual Int64? Id_Natureza { get; set; }

        public virtual String Data { get; set; }
        public virtual String DataCriacao { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual String tempoStatusProxVencer { get; set; }
        public virtual String? Classe { get; set; }

        public virtual List<Timeline_ServicosEquipeModels> Equipes { get; set; }

    }


    public class PostApiRetornoVerificaTimeline<K> : PostApiRetorno
    {
        public K IdObjeto { get; set; }
        public string DataFim { get; set; }
    }
    public class Timeline_ServicosEquipeModels
    {
        public virtual String Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }
        public virtual String Detalhe_Equipe { get; set; }
        public virtual String Justificativa { get; set; }
        public virtual String Cor_StatusEquipe { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String PerfilAgendamento { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String JornadaTrabalho {get;set;}

        public virtual List<Timeline_ServicosEquipeServicosModels> Servicos { get; set; }


        public virtual String HtmlTitle
        {
            get
            {

                StringBuilder legenda = new StringBuilder($"<div style='font-size:14px'>");
                legenda.AppendLine($"<b>{Resources.RsCadastros.SUPERVISOR}: </b> <span>{this.Supervisor}</span><br/>");
                legenda.AppendLine($"<b>{Resources.RsCadastros.PERFIL_AGENDAMENTO_ROTEIRIZACAO}: </b> <span>{this.PerfilAgendamento}</span><br/>");
                legenda.AppendLine($"<b>{Resources.RsCadastros.BATERIA}: </b> <span>{this.Bateria}</span><br/>");
                legenda.AppendLine($"<b>{Resources.RsCadastros.JORNADA}: </b> <span>{this.JornadaTrabalho}</span>");
                legenda.AppendLine("</div>");
                return legenda.ToString();


            }
            set
            {

            }
        }
    }

    public enum Timeline_ServicosEquipeServicosEnum
    {
        Nenhum = 0,
        Serviço_Previsto = 1,
        Tecnico_em_Deslocamento = 2,
        Serviço_Iniciado = 3,
        Serviço_Baixado_no_Prazo_Produtivo = 4,
        Serviço_Baixado_no_Prazo_Improdutivo = 5,
        Serviço_Fora_do_Prazo_Produtivo = 6,
        Serviço_Fora_do_Prazo_Improdutivo = 7,
        Serviço_Atrasado = 8,
        Serviço_Paralisado = 9,
        Servico_Com_Atendimento_Finalizado = 10,
        Fora_Agendamento = 11,
        Tecnico_Ausente = 12,
        Hora_Extra = 13
    }

    public class Tineline_Documentos
    {
        public virtual String NUM_OBRA { get; set; }
        public virtual String NUM_OBRA_ORIGINAL { get; set; }
        public virtual DateTime? DATAHORA_PROD { get; set; }
        public virtual DateTime? DATAHORA_CRIACAO { get; set; }
        public virtual DateTime? DATAHORA_INDICADOR { get; set; }
        public virtual String PROJETO { get; set; }
        public virtual String EQUIPE { get; set; }
        public virtual String NUM_DOC { get; set; }
        public virtual String CODCT { get; set; }
        public virtual StatusExecucaoAniel STATUS_EXECUCAO { get; set; }
        public virtual Boolean PRODUTIVA { get; set; }
        public virtual Tipo_PrioridadeT5? TIPO_PRIORIDADE { get; set; }
        public virtual String DESC_TIPO { get; set; }
        public virtual String COD_TIPO_SERV { get; set; }
        public virtual String DESC_SUBTIPO { get; set; }
        public virtual String TITULAR { get; set; }
        public virtual DateTime? DATAHORA_DESLOCAMENTO { get; set; }
        public virtual DateTime? DATAHORA_INI_ATEND { get; set; }
        public virtual DateTime? DATAHORA_FIM_ATEND { get; set; }
        public virtual DateTime? DATAHORA_PAUSOU { get; set; }
        public virtual DateTime? DATAHORA_REINICIO { get; set; }
        public virtual DateTime? DATAHORA_INICIO { get; set; }
        public virtual DateTime? DATAHORA_FIM { get; set; }
        public virtual DateTime? DATAHORA_INTERROMPER_DESLOC { get; set; }
        public virtual Double? DESLOCAMENTO { get; set; }
        public virtual bool? FLEXIVEL { get; set; }
        public virtual string ESCRITORIO { get; set; }
        public virtual string AREA { get; set; }
        public virtual string LOCALIDADE { get; set; }
        public virtual double TEMPO_EXECUCAO { get; set; }
        public virtual ResponsabilidadeOS? RESPONSABILIDADE { get; set; }
        public virtual string SEGMENTO { get; set; }
        public virtual long? ID_CATEGORIA_CLIENTE { get; set; }
        public virtual String isNaturezaReincidencia { get; set; }
        
    }

    public class Timeline_ServicosEquipeServicosModels
    {
        public virtual DateTime DataHoraIni { get; set; }
        public virtual DateTime DataHoraFim { get; set; }
        public virtual String DataHoraIni_String { get; set; }
        public virtual String DataHoraFim_String { get; set; }
        public virtual DateTime? DataHoraIndicador { get; set; }
        public virtual Double? Deslocamento { get; set; }
        public virtual String DataHoraIniAgendamento { get; set; }
        public virtual String DataHoraFimAgendamento { get; set; }
        public virtual String DescricaoSla { get; set; }
        public virtual String ClasseSla { get; set; }
        public virtual String HoraIni { get; set; }
        public virtual String HoraFim { get; set; }
        public virtual String Equipe { get; set; }
        public virtual double TempoExecucao { get; set; }
        public virtual String Pixels { get; set; }

        public virtual String DescricaoTempos { get; set; }

        public virtual String Num_OS { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String NumDoc { get; set; }

        public virtual String Nome_Cliente { get; set; }
        public virtual Tipo_PrioridadeT5? Prioridade { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String Cod_Tipo_Servico { get; set; }
        public virtual String Sub_Tipo_Servico { get; set; }
        public virtual bool? Flexivel { get; set; }
        public virtual String Escritorio { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Responsabilidade { get; set; }
        public virtual String Prioridade_Natureza { get; set; }
        public virtual String Segmento { get; set; }
        public virtual String Prioridade_Cliente { get; set; }
        public virtual String Prioridade_Legenda { get; set; }

        public virtual DateTime? DATAHORA_INI_ATEND { get; set; }
        public virtual DateTime? DATAHORA_CRIACAO { get; set; }

        public virtual DateTime? DATAHORA_FIM_ATEND { get; set; }

        public virtual DateTime? DATAHORA_PROD { get; set; }
        public virtual String FornecedorAtual { get; set; }
        public virtual String TecnicoAnterior { get; set; }
        public virtual String FornecedorAnterior { get; set; }
        public virtual bool? IsReincidencia { get; set; }


        public virtual Timeline_ServicosEquipeServicosEnum TipoOS { get; set; }

        private IEnumerable<Classe_ListModel> _classecorservico { get; set; }
        public virtual IEnumerable<Classe_ListModel> ClasseCorServico
        {
            get
            {
                if (_classecorservico == null)
                    _classecorservico = new List<Classe_ListModel>();
                return _classecorservico;
            }
            set
            {
                _classecorservico = value;
            }
        }


        public virtual String Classe { get; set; }

        //public virtual String ClasseCorServico
        //{
        //    get
        //    {


        //      switch (TipoOS)
        //        {
        //            case Timeline_ServicosEquipeServicosEnum.Nenhum: return "bg-transparent";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Previsto: return "bg-serv-prev";
        //            case Timeline_ServicosEquipeServicosEnum.Tecnico_em_Deslocamento: return "bg-tec-desloc";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Iniciado: return "bg-serv-inic";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Produtivo: return "bg-serv-baixo-prod";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Improdutivo: return "bg-serv-baixa-improd";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Produtivo: return "bg-serv-baixa-fora-prod";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Improdutivo: return "bg-serv-baixa-fora-improd";
        //            case Timeline_ServicosEquipeServicosEnum.Servico_Com_Atendimento_Finalizado: return "bg-serv-atend_finalizado";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Atrasado: return "bg-serv-atrasado";
        //            case Timeline_ServicosEquipeServicosEnum.Serviço_Paralisado: return "bg-serv-paralisado";
        //            case Timeline_ServicosEquipeServicosEnum.Fora_Agendamento: return "bg-fora-agendamento";
        //            default: return "bg-transparent";
        //        }
        //    }
        //    set
        //    {

        //    }
        //}

        public virtual String HtmlTitle
        {
            get
            {
                if (this.TipoOS == Timeline_ServicosEquipeServicosEnum.Nenhum)
                    return "";
                else if (this.TipoOS == Timeline_ServicosEquipeServicosEnum.Fora_Agendamento)
                    return "";

                else if (this.TipoOS != Timeline_ServicosEquipeServicosEnum.Tecnico_Ausente)
                {
                    StringBuilder legenda = new StringBuilder($"<div style='font-size:14px'><b>OS: </b> <span>{this.Num_OS}</span> <br/> <b>Cliente: </b> <span>{this.Nome_Cliente}</span><br/><b>Tipo de Serviço: </b> {this.Tipo_Servico}<br/>");


                    if (String.IsNullOrEmpty(this.HoraIni) == false)
                        legenda.AppendLine($"<b><span>Início Deslocamento Previsto:</b>{this.HoraIni}</span><br/><b><span>Hora Agendada com Cliente: </b>{Convert.ToDateTime(this.HoraIni).AddMinutes(this.Deslocamento.Value).ToString("HH:mm")}</span><br/><b>Tempo Execução Previsto: </b> <span>{Convert.ToDateTime(this.HoraIni).AddMinutes(this.Deslocamento.Value).ToString("HH:mm")} - {this.HoraFim}</span><br/>");

                    if (this.DATAHORA_INI_ATEND.HasValue)
                        legenda.AppendLine($"<b>Início Atendimento: </b> <span>{this.DATAHORA_INI_ATEND.Value.ToString("dd/MM/yyyy HH:mm")}</span><br/>");
                    else
                        legenda.AppendLine($"<b>Início Atendimento: </b> <span></span><br>");

                    if (this.DATAHORA_FIM_ATEND.HasValue)
                        legenda.AppendLine($"<b>Término Atendimento: </b> <span>{this.DATAHORA_FIM_ATEND.Value.ToString("dd/MM/yyyy HH:mm")}</span><br/>");
                    else
                        legenda.AppendLine($"<b>Término Atendimento: </b> <span></span><br>");

                    if (this.DATAHORA_PROD.HasValue)
                        legenda.AppendLine($"<b>Baixa do Serviço: </b> <span>{this.DATAHORA_PROD.Value.ToString("dd/MM/yyyy HH:mm")}</span><br/>");
                    else
                        legenda.AppendLine($"<b>Baixa do Serviço: </b> <span></span><br/>");

                    if (this.DataHoraIndicador.HasValue)
                        legenda.AppendLine($"<b>Vencimento do Serviço: </b> <span>{this.DataHoraIndicador.Value.ToString("dd/MM/yyyy HH:mm")}</span><br/>");
                    else
                        legenda.AppendLine($"<b>Vencimento do Serviço: </b> <span></span><br/>");

                    if (this.Flexivel.HasValue)
                        legenda.AppendLine($"<b>Flexível: </b> <span>{(this.Flexivel.Value ? Resources.RsBase.SIM : Resources.RsBase.NAO)}</span><br/>");
                    else
                        legenda.AppendLine($"<b>Flexível: </b> <span></span><br/>");


                    var prioridade_Cliente = "0/Inexistente";
                    var prioridade_Natureza = "0/Inexistente";

                    //Prioridade Cliente
                    if (this.Prioridade_Cliente == "1")
                    {
                        prioridade_Cliente = "1/Alta";
                    }
                    else if (this.Prioridade_Cliente == "2")
                    {
                        prioridade_Cliente = "2/Média";
                    }
                    else if (this.Prioridade_Cliente == "3")
                    {
                        prioridade_Cliente = "3/Baixa";
                    }

                    //Prioriade OS
                    if (this.Prioridade_Natureza == "1")
                    {
                        prioridade_Natureza = "1/Alta";
                    }
                    else if (this.Prioridade_Natureza == "2")
                    {
                        prioridade_Natureza = "2/Média";
                    }
                    else if (this.Prioridade_Natureza == "3")
                    {
                        prioridade_Natureza = "3/Baixa";
                    }

                    legenda.AppendLine($"<b>{Resources.RsCadastros.GRUPO_AREA}: </b> <span>{this.Escritorio}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.AREA}: </b> <span>{this.Area}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.LOCALIDADE}: </b> <span>{this.Localidade}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.RESPONSABILIDADE}: </b> <span>{this.Responsabilidade}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.PRIORIDADE_CLIENTE}: </b> <span>{prioridade_Cliente}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.PRIORIDADE} {Resources.RsCadastros.OS}: </b> <span>{prioridade_Natureza}</span><br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.SEGMENTO}: </b> <span>{this.Segmento}</span>");

                    legenda.AppendLine($"<b>{Resources.RsCadastros.FORNECEDOR} Atual: </b> <span>{this.FornecedorAtual} </span> <br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.TECNICO} Anterior: </b> <span>{this.TecnicoAnterior} </span> <br/>");
                    legenda.AppendLine($"<b>{Resources.RsCadastros.FORNECEDOR} Anterior: </b> <span>{this.FornecedorAnterior} </span> <br/>");





                    legenda.AppendLine("</div>");

                    return legenda.ToString();
                }
                else
                {
                    StringBuilder legenda = new StringBuilder($"<div style='font-size:14px'><b>Justificativa: </b> <span>{this.Prioridade_Legenda}</span><br/>");

                    legenda.AppendLine($"<b>Data Saída: </b> <span>{this.DataHoraIni.ToString("dd/MM/yyyy HH:mm")}</span><br/>");
                    legenda.AppendLine($"<b>Data Retorno: </b> <span>{this.DataHoraFim.ToString("dd/MM/yyyy HH:mm")}</span><br/>");

                    legenda.AppendLine("</div>");

                    return legenda.ToString();
                }

            }
            set
            {

            }
        }

    }


    public class Classe_ListModel
    {
        public virtual string Num_OS { get; set; }

        public virtual string Classe { get; set; }

        public virtual string TipoClasse { get; set; }
    }

    public class TimelineParametros
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string Grupo { get; set; }

        public string CodCt { get; set; }
        public string Projeto { get; set; }
        public int NumDoc { get; set; }
        public string Num_OS { get; set; }

        public string FormatoTimeline { get; set; }
    }

    public class EquipeUsuario
    {
        public virtual String Num_Fre { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Foto { get; set; }
    }

    public class OsReprocessadas
    {
        public  String OsReprocessada { get; set; }
        public  String OsNaoReprocessada { get; set; }
        public String TotalProcessada { get; set; }
        public String TotalNaoReprocessadaOuComErro { get; set; }

    }

    public class OsReprocessaRequestModel
    {
        public String DataInicio { get; set; }
        public String DataFim { get; set; }

    }

}