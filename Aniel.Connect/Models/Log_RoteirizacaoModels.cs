using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;

namespace Aniel.Connect.Models
{



    public class Log_RoteirizacaoListaModel
    {
        public virtual Int64? Id { get; set; }

        public virtual String Data_Log { get; set; }

        public virtual String Log { get; set; }

        public virtual String Usuario { get; set; }

        public virtual String Status { get; set; }
        public virtual String Simulacao { get; set; }

    }

    public class Log_RoteirizacaoItemListaModel
    {
        public virtual String Codct { get; set; }

        public virtual String Projeto { get; set; }

        public virtual String Id_Log { get; set; }

        public virtual String Num_Obra { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual String Num_Doc { get; set; }

        public virtual String Motivo { get; set; }

        public virtual Int16? Cod_tipo_serv { get; set; }
        public virtual String Desc_tipo_serv { get; set; }
        public virtual String Cliente { get; set; }
        public virtual Int64? Id_cliente { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Nome_equipe { get; set; }
        public virtual String Data_hora_ini_agendamento { get; set; }

        public virtual String DataInicial { get; set; }
        public virtual String DataFinal { get; set; }

        public virtual String Data_hora_fim_agendamento { get; set; }

        public virtual String Cod_escritorio { get; set; }
        public virtual String Cod_area { get; set; }
        public virtual Int32? Cod_localidade { get; set; }
        public virtual String Desc_escritorio { get; set; }
        public virtual String Desc_area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Id_Supervisor { get; set; }
        public virtual String Id_Monitor { get; set; }
        public virtual String PosicaoFila { get; set; }
        public virtual String SLA { get; set; }
        public virtual LogStatus_SLA_? statussla { get; set; }
        public virtual String Data_Hora_Vencimento_OS { get; set; }
        public virtual String Usuario_Monitor { get; set; }
        public virtual String Numfre_Supervisor { get; set; }
        public virtual String PossuiLatLong { get; set; }
        public virtual String LatitudeOS { get; set; }
        public virtual String LongitudeOS { get; set; }
        public virtual String LatitudeCaixa { get; set; }
        public virtual String LongitudeCaixa { get; set; }
        public virtual String LatitudeLocalidade { get; set; }
        public virtual String LongitudeLocalidade { get; set; }
        public virtual String DescNatureza { get; set; }
        public virtual string OrdemDistribuida { get; set; }
        public virtual string NivelRodadaRoteirizada { get; set; }
        public virtual string AgrupamentoRodadaRoteirizada { get; set; }
        public virtual String PontoPartida { get; set; }
        public virtual String LatitudePontoPartida { get; set; }
        public virtual String LongitudePontoPartida { get; set; }
        public virtual String OrigemAgendamento { get; set; }
        public virtual Boolean UtilizaOsReincidencia { get; set; } = false;
        public virtual String FornecedorAtual { get; set; }
        public virtual String FornecedorAnterior { get; set; }
        public virtual String TecnicoAnterior { get; set; }

    }


    public class Log_Roteirizacao_DataTableRequest : DataTableRequest
    {
        public virtual String DataHoraCriacao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual Boolean Filtro { get; set; }


    }

    public class Log_Roteirizacao_Item_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id { get; set; }
        /*Contrato*/
        
        public virtual String Contrato_filtro  { get; set; }
        public virtual String Projeto_filtro { get; set; }
      
      
        /*Área*/
        public virtual String Grupo_area_filtro { get; set; }
        public virtual String Area_filtro { get; set; }
        public virtual String Localidade_filtro { get; set; }
        public String DataInicial { get; set; }
        public String DataFinal { get; set; }
        /*Sla*/
        public virtual String Sla_filtro { get; set; }

        /*Data_vencimento*/
        public virtual Boolean Vencimentoate_filtro { get; set; }

        /*Equipe*/
        public virtual String Numfre_Supervisor { get; set; }
        public virtual String Usuario_Monitor { get; set; }


        public virtual Somente_LogRoteirizacao? Somente_LogRoteirizacao { get; set; }

    }

    public enum Somente_LogRoteirizacao
    {
        NAO_ROTEIRIZADAS = 0,
        ROTEIRIZADAS = 1
    }



}