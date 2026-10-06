using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Acompanhamento_EquipeListaModel
    {
        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Cor_StatusEquipe { get; set; }

        public virtual String Icone_Bateria { get; set; }
        public virtual String Cor_Bateria { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String TotalVisitas { get; set; }
        public virtual String BaixadasProdutivas { get; set; }
        public virtual String BaixadasImprodutivas { get; set; }
        public virtual String Restantes { get; set; }
        public virtual String Ultimo_Endereço { get; set; }
        public virtual String Atualizacao { get; set; }
        //public virtual String Equipe_Mapa { get; set; }
        //public virtual String Detalhes_Equipe { get; set; }

        public virtual String Status_App { get; set; }

        public virtual String Data_Agendamento { get; set; }

        public virtual String Pontos_Previstos { get; set; }
        public virtual String Pontos_Realizados { get; set; }




    }

    public class Acompanhamento_Equipes
    {
        public virtual Boolean Tecnico_Ativos { get; set; }
        public virtual Boolean Tecnico_Em_Execucao { get; set; }

        public virtual String DataAbertoIni { get; set; }
        public virtual String DataAbertoFim { get; set; }
        public virtual String DataParalisadosIni { get; set; }
        public virtual String DataParalisadosFim { get; set; }
        public virtual String DataEncerradosIni { get; set; }
        public virtual String DataEncerradosFim { get; set; }
    }

    public class Acompanhamento_EquipeListaItemModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Os { get; set; }
        public virtual String Numero_Os_Original { get; set; }
        public virtual Int32 Numero_Doc { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String SLA_Servicos { get; set; }
        public virtual String DataHora_Indicador { get; set; }
        public virtual String Status_Execucao { get; set; }
        public virtual String Assinante { get; set; }
        public virtual String Recebido_Tecnico { get; set; }
        public virtual String Roterizar { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Endereco_Bairro { get; set; }
        public virtual String Endereco_Tipo { get; set; }
        public virtual String Numero_Atendimento { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Deslocamento { get; set; }
        public virtual String Inicio_Atendimento { get; set; }

        public virtual String Fim_Atendimento { get; set; }
        public virtual String Tempo_Execucao { get; set; }
        public virtual String Tempo_Paralizacoes { get; set; }
        public virtual String Duracao_Total_Visita { get; set; }

        public virtual String Data_Agendamento { get; set; }
        public virtual String Periodo { get; set; }
        public virtual String Motivo_Pausa { get; set; }
        public virtual String Descricao_Encerramento { get; set; }
        public virtual String Pontos_Previstos { get; set; }
        public virtual String Pontos_Realizados { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String Status_Cobranca { get; set; }
    }

    public class Acompanhamento_Equipes_DataTableRequest : DataTableRequest
    {
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual Int64? Id_Natureza { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String Tecnico_Ativos { get; set; }

        public virtual String Tecnico_Em_Execucao { get; set; }
        public virtual Int64? Situacao { get; set; }

        public virtual String DataAbertoIni { get; set; }
        public virtual String DataAbertoFim { get; set; }
        public virtual String DataParalisadosIni { get; set; }
        public virtual String DataParalisadosFim { get; set; }
        public virtual String DataEncerradosIni { get; set; }
        public virtual String DataEncerradosFim { get; set; }
        public virtual String Pesquisa { get; set; } 
        public virtual Int64? MonitorFiltro { get; set; }
         public virtual String DataInicial { get; set; }
         public virtual String DataFinal { get; set; }

    }

    public class DefinirTecnicosRoteirizacaoDataTableRequest : DataTableRequest
    {
        public virtual String Usuario { get; set; }
        public virtual String Empresa { get; set; }
        public virtual String Filial { get; set; }
        public virtual List<String> Departamento { get; set; } = new List<string>();
        public virtual List<String> Supervisor { get; set; } = new List<string>();
        public virtual List<String> Monitor { get; set; } = new List<string>();
        public virtual Int64? Id_Natureza { get; set; }
        public virtual List<Int64?> IdPerfilAgendamentoRoteirizacao { get; set; } = new List<long?>();
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }

        public virtual String Tecnico_Ativos { get; set; }

        public virtual String Tecnico_Em_Execucao { get; set; }
        public virtual Int64? Situacao { get; set; }

        public virtual String DataAbertoIni { get; set; }
        public virtual String DataAbertoFim { get; set; }
        public virtual String DataParalisadosIni { get; set; }
        public virtual String DataParalisadosFim { get; set; }
        public virtual String DataEncerradosIni { get; set; }
        public virtual String DataEncerradosFim { get; set; }
        public virtual String Pesquisa { get; set; }

    }

    public class Acompanhamento_EquipesOss_DataTableRequest : DataTableRequest
    {
        public virtual String Equipe { get; set; }
        public virtual Int64? Situacao { get; set; }
          public virtual Int64? Id_Cliente { get; set; }

        public virtual String DataAbertoIni { get; set; }
        public virtual String DataAbertoFim { get; set; }
        public virtual String DataParalisadosIni { get; set; }
        public virtual String DataParalisadosFim { get; set; }
        public virtual String DataEncerradosIni { get; set; }
        public virtual String DataEncerradosFim { get; set; }
    }

    public class ServicosImprodutivos_DataTableRequest : DataTableRequest
    {
        public virtual String Equipe { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int32? Cod_Tipo_Serv { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Data_Inicio { get; set; }
        public virtual String Data_Fim { get; set; }
        public virtual String Tipo_Data { get; set; }
        public virtual String Tipo_SLA { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String CodCt_Encerramento { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_PlanoProduto { get; set; }
    }

    public class RelatorioAcompanhamento_DataTableRequest : DataTableRequest
    {
        public virtual String Equipe { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual Int32? Cod_Tipo_Serv { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Data_Inicio { get; set; }
        public virtual String Data_Fim { get; set; }
        public virtual String Tipo_Data { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String CodCt_Encerramento { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_PlanoProduto { get; set; }
        public virtual Int64? Status_SLA_Atendimento { get; set; }
    }

    public class Filtros_ProducaoServico_DataTableRequest : DataTableRequest
    {
        public virtual String Data_Inicio { get; set; }
        public virtual String Data_Fim { get; set; }
        public virtual String TipoData { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Natureza_Servico { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual Boolean Producao_Prevista { get; set; }
        public virtual Boolean Servico_Baixado_Mobile { get; set; }
        public virtual Boolean Servico_Doc_Fisico_Devolvido { get; set; }
        public virtual String Agrupamento { get; set; }

    }

    public class EquipeAgendamentoModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Os { get; set; }
        public virtual String Numero_Os_Original { get; set; }
        public virtual Int32 Numero_Doc { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Usuario { get; set; }
    }
}