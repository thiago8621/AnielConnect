using Aniel.Connect.Core;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class AgendamentoCadastroModel
    {
        List<String> lista_Forca_Trabalho;
        List<String> lista_Ja_Agendados;

        public virtual String Data { get; set; }

        public virtual String Hora { get; set; }

        public virtual String Hora_Inicio { get; set; }
        public virtual String Hora_Final { get; set; }
        public virtual Double Deslocamento { get; set; }
        public virtual String Turno { get; set; }
        public virtual bool Flexivel { get; set; }

        public virtual String Equipe_Tmp { get; set; }
        public virtual String Status_Cobranca { get; set; }
        public virtual String Exige_Pagamento_Instalacao { get; set; }

        public virtual Boolean Roteirizar { get; set; }
        public virtual Boolean ConfirmaEndereco { get; set; }

        public virtual String CodMotivoReagendamento { get; set; }
        public virtual String ObsMotivoReagendamento { get; set; }
        public virtual List<String> Lista_Forca_Trabalho
        {
            get
            {
                if (lista_Forca_Trabalho == null)
                    lista_Forca_Trabalho = new List<string>();
                return lista_Forca_Trabalho;
            }
            set
            {
                lista_Forca_Trabalho = value;
            }
        }
        public virtual List<String> Lista_Ja_Agendados
        {
            get
            {
                if (lista_Ja_Agendados == null)
                    lista_Ja_Agendados = new List<string>();
                return lista_Ja_Agendados;
            }
            set
            {
                lista_Ja_Agendados = value;
            }
        }


        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }

        public virtual DocumentoProducaoModel Documento { get; set; }

        public virtual List<ObjBusca<String>> Datas { get; set; }

        public virtual List<ObjBusca<String>> Horas { get; set; }

        public virtual List<AgendamentoDiaModel> Objetos { get; set; }

        public virtual String Pagina { get; set; }

        /// <summary>
        ///     Define se o percentual do limite de agendamento foi ultrapassado, 
        ///     caso esteja configurado para a natureza na área do serviço que
        ///     será agendado
        /// </summary>
        public bool LimiteAgendamentoUltrapassado { get; set; }
        /// <summary>
        ///     Define um tecnico temporario que pode ser usado seguindo a regra do limite de agendamento
        ///     caso a propriedade LimiteAgendamentoUltrapassado, esse campo deve ser usado no lugar do EquipeTmp
        /// </summary>
        public virtual String EquipeTmpComLimite { get; set; }
    }

    public class AgendamentoMobileRequest
    {
        List<String> lista_Forca_Trabalho;
        List<String> lista_Ja_Agendados;

        public virtual Int64? Id_Cliente_PlanoProduto { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual string HoraInicio { get; set; }
        public virtual string HoraFinal { get; set; }

        public virtual String Equipe_Tmp { get; set; }

        public virtual List<String> Lista_Forca_Trabalho
        {
            get
            {
                if (lista_Forca_Trabalho == null)
                    lista_Forca_Trabalho = new List<string>();
                return lista_Forca_Trabalho;
            }
            set
            {
                lista_Forca_Trabalho = value;
            }
        }
        public virtual List<String> Lista_Ja_Agendados
        {
            get
            {
                if (lista_Ja_Agendados == null)
                    lista_Ja_Agendados = new List<string>();
                return lista_Ja_Agendados;
            }
            set
            {
                lista_Ja_Agendados = value;
            }
        }

        public virtual Double Deslocamento { get; set; }
        public virtual String DescricaoProblema { get; set; }
    }

    public class AgendarMobileRequest
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual String Hora_Inicio { get; set; }
        public virtual String Hora_Final { get; set; }
        public virtual Double Deslocamento { get; set; }
        public virtual String Equipe_Tmp { get; set; }

        public virtual Boolean Roteirizar { get; set; }

        List<String> lista_Forca_Trabalho;
        List<String> lista_Ja_Agendados;

        public virtual List<String> Lista_Forca_Trabalho
        {
            get
            {
                if (lista_Forca_Trabalho == null)
                    lista_Forca_Trabalho = new List<string>();
                return lista_Forca_Trabalho;
            }
            set
            {
                lista_Forca_Trabalho = value;
            }
        }
        public virtual List<String> Lista_Ja_Agendados
        {
            get
            {
                if (lista_Ja_Agendados == null)
                    lista_Ja_Agendados = new List<string>();
                return lista_Ja_Agendados;
            }
            set
            {
                lista_Ja_Agendados = value;
            }
        }
        public virtual String Turno { get; set; }
    }

    public class DetalhamentoDisponibilidadeListaModel
    {
        public virtual String NumObra { get; set; } //Número da OS 
        public virtual String DataInicioAgendamento { get; set; } //Data Inicio Agendamento 
        public virtual String DataFimAgendamento { get; set; } //Data Fim Agendamento 
        public virtual String Periodo { get; set; } //Período
        public virtual String OrigemAgendamento { get; set; } //Origem do Agendamento 
        public virtual String UsuarioCriacao { get; set; } //Usuário Criação 
        public virtual String UsuarioAgendamento { get; set; } //Usuario Agendamento
        public virtual String DataHoraCriacao { get; set; } //Data Criação 
        public virtual String TipoServico { get; set; } //Tipo de Serviço 
        public virtual String Natureza { get; set; } //Natureza de Serviço 
        public virtual String GrupoArea { get; set; } //Grupo Área 
        public virtual String Area { get; set; } //Área
        public virtual String Localidade { get; set; } //Localidade
        public virtual String HorasAgendadas { get; set; } //Horas Agendadas
        public virtual String TempoDeslocamento { get; set; } //Tempo de Deslocamento
        public virtual String CapacidadeRestante { get; set; } //Tempo de Deslocamento
        public virtual String Status { get; set; } //Tempo de Deslocamento
        public virtual String Tecnico { get; set; } //Tecnico
        public virtual String TecnicoTemporario { get; set; } //Tecnico
        public virtual String PerfilAgendamento { get; set; } //Perfil de Agendamento


    }

    public class DadosPerfilDisponibilidadeListaModel
    {
        public virtual TimeSpan capacidadeRestanteTimespan { get; set; }
        public virtual TimeSpan capacidadeRestanteTempoComLimiteTimespan { get; set; }
        public virtual TimeSpan capacidadeRestanteCalculoTempoTimespan { get; set; }
        public virtual String capacidadeRestante { get; set; }
        public virtual String capacidadeRestanteServicos { get; set; }
        public virtual String capacidadeHoras { get; set; }
        public virtual String capacidadeUtilHoras { get; set; }
        public virtual TimeSpan capacidadeHorasTimespan { get; set; }
        public virtual String forcaTrabalho { get; set; }
        public virtual String perfilAgendamento { get; set; } //Perfil de Agendamento

    }

    public class DetalhamentoDisponibilidadeDataTableRequest : DataTableRequest
    {
        public Int16? Cod_Tipo_Serv { get; set; }
        public String Cod_Grupo_Area { get; set; }
        public String Cod_Area { get; set; }
        public String LocalidadeStr { get; set; }
        public Int32? Cod_Localidade { get; set; }
        public string Data { get; set; }
        public long? IdPeriodo { get; set; }
        public long? IdTipoImovel { get; set; }
        public long? IdPerfil { get; set; }
    }

    public class DetalhamentoDisponibilidadeDataTableResponse<T>
    {
        public int draw { get; set; }
        //public long? recordsTotal { get; set; }
        public int? recordsFiltered { get; set; }
        public IEnumerable<T> data { get; set; }
        public List<DadosPerfilDisponibilidadeListaModel> dadosPerfil { get; set; }
        public string error { get; set; }
        public string capacidadePlena { get; set; }
        public string capacidadeHoras { get; set; }
        public string capacidadeUtilHoras { get; set; }
        public string qtdeTecnicosEscalados { get; set; }
        public string capacidadeRestante { get; set; }
        public string capacidadeRestanteServicos { get; set; }
        public string tempoExecucaoTipoServico { get; set; }
        public string tempoDeslocamentoPadrao { get; set; }
        public string tempoLacunaTecnico { get; set; }
        public string minutosAgendamentoDentroFaixa { get; set; }
        public string minutosAgendamentoForaFaixa { get; set; }
        public string minutosCapacidadeRestante { get; set; }

    }

    public class RecalcularAgendamentoRequestModel
    {
        public virtual List<Documento_ProducaoChaveModel> documentos { get; set; }
    }

    
}