using Sinapse.Model.nhibernate;
using System;


namespace Aniel.Connect.ModelLog.nhibernate
{
    public class Log_RoteirizacaoItemAniel : BaseModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }


        public virtual Log_RoteirizacaoAniel Log { get; set; }

        public virtual String Motivo { get; set; }


        public virtual String Num_Obra_Original { get; set; }
        public virtual Int16? Cod_tipo_serv { get; set; }
        public virtual String Desc_tipo_serv { get; set; }
        public virtual String Cliente { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Nome_equipe { get; set; }
        public virtual String Usuario_Monitor { get; set; }
        public virtual String Nome_Monitor { get; set; }
        public virtual String NumFre_Supervisor { get; set; }
        public virtual String Nome_Supervisor { get; set; }
        public virtual DateTime? Data_hora_ini_agendamento { get; set; }
        public virtual Double? Deslocamento { get; set; }
        public virtual DateTime? Data_hora_fim_agendamento { get; set; }
        public virtual String SLA { get; set; }
        public virtual DateTime? Data_Vencimento { get; set; }
        
        public virtual String Cod_escritorio { get; set; }
        public virtual String Cod_area { get; set; }
        public virtual Int32? Cod_localidade { get; set; }
        public virtual String Desc_escritorio { get; set; }
        public virtual String Desc_area { get; set; }
        public virtual String Desc_natureza { get; set; }
        public virtual String Localidade { get; set; }
        public virtual Int16? Posicao_Fila { get; set; }
        public virtual Boolean PossuiLatLong { get; set; }
        public virtual String LatitudeOS { get; set; }
        public virtual String LongitudeOS { get; set; }
        public virtual String LatitudeCaixa { get; set; }
        public virtual String LongitudeCaixa { get; set; }
        public virtual String LatitudeLocalidade { get; set; }
        public virtual String LongitudeLocalidade { get; set; }
        public virtual int? OrdemDistribuida { get; set; }
        public virtual int? NivelRodadaRoteirizada { get; set; }
        public virtual int? AgrupamentoRodadaRoteirizada { get; set; }
        public virtual String PontoPartida { get; set; }
        public virtual String LatitudePontoPartida { get; set; }
        public virtual String LongitudePontoPartida { get; set; }
        public virtual String OrigemAgendamento { get; set; }


    }
}
