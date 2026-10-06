using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Painel_RoteirizacaoListaFiltroModel
    {
        public virtual String Ordem_Servico { get; set; }

        public virtual String Cod_Escritorio { get; set; }
        public virtual String Grupo_Area { get; set; }


        public virtual String Cod_Area { get; set; }
        public virtual String Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String DataInicial_Abertura_OS { get; set; }
        public virtual String DataFinal_Abertura_OS { get; set; }

        public virtual String DataInicial_Agendamento_OS { get; set; }
        public virtual String DataFinal_Agendamento_OS { get; set; }

        public virtual String SLA { get; set; }

        public virtual String Categoria_Cliente { get; set; }
        public virtual String Codigo_Cliente { get; set; }
        public virtual String Nome_Cliente { get; set; }

        public virtual String Tipo_Imovel { get; set; }

        public virtual String Status { get; set; }

        public virtual String Id_Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }

    }

    public class Painel_RoteirizacaoModel
    {
        public virtual String Data_SemAgendamentoIni { get; set; }
        public virtual String Data_SemAgendamentoFim { get; set; }
        public virtual String Data_Roteirizar { get; set; }
        public virtual String Numero_Maximo_Servicos { get; set; }
        public virtual String Numero_Ordem_Servico { get; set; }

        public virtual Boolean Servico_SemAgendamento { get; set; }
        public virtual Boolean Servico_Agendados { get; set; }
        public virtual Boolean Servico_Paralisados { get; set; }
        public virtual Boolean Servico_Nao_Paralisados { get; set; }
        public virtual Boolean Servico_Nao_Executados { get; set; }
        public virtual Boolean Servico_Roteirizados { get; set; }
        public virtual String Data_Agendamento { get; set; }
        public virtual String Data_ParalisadoIni { get; set; }
        public virtual String Data_ParalisadoFim { get; set; }
        public virtual String Data_Nao_ExecutadoIni { get; set; }
        public virtual String Data_Nao_ExecutadoFim { get; set; }
        public virtual String Data_RoteirizadosIni { get; set; }
        public virtual String Data_RoteirizadosFim { get; set; }

        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual Int64? Id_Natureza_Servico { get; set; }

        public virtual Int16 Cod_Filial { get; set; }
        public virtual String Filial { get; set; }

        public virtual String Cod_Superv { get; set; }
        public virtual String Supervisor { get; set; }

        public virtual String Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }

        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Localidade { get; set; }

        public virtual String Data_Log { get; set; }
        public virtual String Usuario { get; set; }
    }

    public class Painel_RoteirizacaoListaModel
    {
        public virtual String Cnpj { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        public virtual String Hora_Agendamento { get; set; }
        public virtual String Deslocamento { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Grupo_Area { get; set; }


        public virtual String Cod_Area { get; set; }
        public virtual String Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String Pesquisa_Data { get; set; }

        public virtual String Data_Criacao { get; set; }


        public virtual String Data_Agendamento { get; set; }
        public virtual String Cliente { get; set; }


        public virtual String SLA { get; set; }

        public virtual String Categoria_Cliente { get; set; }



        public virtual String Tipo_Imovel { get; set; }

        public virtual String Status { get; set; }

        public virtual String Id_Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }
        public virtual String Filtro_Agendamento { get; set; }

    }

    public class RelatorioKmModel
    {
        public virtual Int64 Quantidade_OS { get; set; }
        public virtual String Matricula { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual double DistanciaPlanejada { get; set; }
        public virtual double DistanciaRealizada { get; set; }
        public virtual String PerfilAgendamento { get; set; }
    }

    public class Painel_RotasListaModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        public virtual String Hora_Inicio_Agendamento { get; set; }
        public virtual String Hora_Fim_Agendamento { get; set; }

        public virtual String Tipo_Servico { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Grupo_Area { get; set; }


        public virtual String Cod_Area { get; set; }
        public virtual String Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String Pesquisa_Data { get; set; }

        public virtual String Data_Criacao { get; set; }


        public virtual String Data_Agendamento { get; set; }


        public virtual String SLA { get; set; }

        public virtual String Categoria_Cliente { get; set; }



        public virtual String Tipo_Imovel { get; set; }

        public virtual String Status { get; set; }

        public virtual String Id_Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }

    }


    public class Painel_RoteirizacaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
    }

    public class RoteirizarServicosModel
    {
        public List<String> ids { get; set; } = new List<string>();
        public Int16? numero_max_atendimento { get; set; }
        public String data { get; set; }
        public String Equipe { get; set; }
        public bool EquipesConfiguradas { get; set; }
        public bool? alterar_DataHora_SemAgendamentoAniel { get; set; }
        public bool simulacao { get; set; }
        public bool encaixe { get; set; }
        public bool encaixeAutomatico { get; set; }
    }

    public class Painel_Rotas_DataTableRequest : DataTableRequest
    {
        public virtual String Equipe { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String Data_Agendamento { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
    }

    public class Painel_Roteirizacao_DataTableRequest : DataTableRequest
    {

        public virtual String Numero_Ordem_Servico { get; set; }
        public virtual Boolean Servico_Agendado { get; set; }
        public virtual Boolean Servico_Paralisado { get; set; }
        public virtual Boolean Servico_Nao_Paralisado { get; set; }
        public virtual Boolean Servico_Sem_Agendamento { get; set; }
        public virtual Boolean Servico_Nao_Executado { get; set; }
        public virtual Boolean Servico_Roteirizados { get; set; }
        public virtual Boolean Servico_Sem_Roteirizacao { get; set; }

        public List<Int64?> Id_NaturezaServico { get; set; } = new List<Int64?>();
        public virtual Int64? Id_CategoriaCliente { get; set; }
        public virtual String TipoPrioridade { get; set; }

        public virtual String DataAgendados { get; set; }
        public virtual string NaoAgendadosIni { get; set; }
        public virtual string NaoAgendadosFim { get; set; }
        public virtual String DataNaoRoteirizados { get; set; }
        public virtual String Data_ParalisadoIni { get; set; }
        public virtual String Data_ParalisadoFim { get; set; }
        public virtual String Data_Nao_ExecutadoIni { get; set; }
        public virtual String Data_Nao_ExecutadoFim { get; set; }
        public virtual String Data_RoteirizadosIni { get; set; }
        public virtual String Data_RoteirizadosFim { get; set; }

        public virtual String IdFilial { get; set; }
        public virtual String IdSupervisor { get; set; }
        public virtual String Matricula_Id { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual Int32 Localidade { get; set; }
    }

    public class RelatorioKmDataTableRequest : DataTableRequest
    {
        public virtual String Data { get; set; }
        public virtual String Equipe { get; set; }
        public List<String> Supervisor { get; set; } = new List<String>();
        public virtual String Monitor { get; set; }
        public virtual bool ApenasTecnicosComOs { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
    }

    public class CompactarRotaModel
    {
        public bool SalvarRota { get; set; }
        public String NumObra { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public Int32? NumDoc { get; set; }
    }

    public class CompactarRotaEncaixeModel
    {
        public bool SalvarRota { get; set; }
        public string DataHoraFimAntigoAgendamento { get; set; }
        public string DataHoraFimEncaixe { get; set; }
        public string NumObraEncaixe { get; set; }
        public string Equipe { get; set; }
    }

    public class CompactarRotaRetorno
    {
        public int QtdOsCompactar { get; set; }
        public int QtdOsRoteirizada { get; set; }
        public bool Ok { get; set; }
    }

    public class SalvarUsuarioTecnicoRoteirizacaoRequestModel
    {
        public virtual List<String> Equipes { get; set; } = new List<string>();
        public virtual String Usuario { get; set; }
        
    }


}