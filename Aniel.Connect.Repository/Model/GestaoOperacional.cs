using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using MongoDB.Bson.IO;

namespace Aniel.Connect.Repository.Model
{
    public class GestaoCapacidadeRoteirizacaoRequest : DataTableRequest
    {
        public virtual string Data { get; set; }
        public virtual string DataInicioDemanda { get; set; }
        public virtual string BuscarServicosSemAgendamento { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public virtual string Equipe { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string Monitor { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string TipoServico { get; set; }
        public virtual string Natureza { get; set; }
        public virtual string NaturezaServico { get; set; }
        public virtual string Periodo { get; set; }

    }
    public class DocumentoPlanejamento
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Equipe_Anterior { get; set; }
        //public virtual String Monitor { get; set; }
        //public virtual String Supervisor { get; set; }
        public virtual TimeSpan? Tempo_Execucao { get; set; }
        public virtual double TempoExecucaoMinutos { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual String Descricao_Grupo_Area { get; set; }
        public virtual String Descricao_Area { get; set; }
        public virtual Int32? Cod_Tipo_Imovel { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Int64? Id_Natureza { get; set; }
        public virtual DateTime? Data_Maxima { get; set; }
        public virtual DateTime? Hora_Maxima { get; set; }
        public virtual string DataMaximaStr { get; set; }
        public virtual string HoraMaximaStr { get; set; }
        public virtual DateTime? SLA { get; set; }
        public virtual TecnicoRoteirizar Tecnico { get; set; }
        public virtual TecnicoRoteirizar Tecnico_Anterior { get; set; }
        public virtual Int64? Id_Agendamento { get; set; }
        public virtual DateTime? Data_hora_ini_agendamento { get; set; }
        public virtual DateTime? Data_hora_Criacao { get; set; }
        public virtual DateTime? Data_hora_fim_agendamento { get; set; }
        public virtual DateTime? Data_hora_Encerramento { get; set; }
        public virtual DateTime? Data_Hora_Vencimento_OS { get; set; }
        public virtual String DataHoraAberturaOriginal { get; set; }
        public virtual String OXBDPEE { get; set; }
        public virtual Int16 DeslocamentoMinutos { get; set; }
        public virtual String Latitude_Str { get; set; }
        public virtual String Longitude_Str { get; set; }
        public virtual String Longitude_Cx { get; set; }
        public virtual String Latitude_Cx { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Descricao_Tipo_Serv { get; set; }
        public virtual String Log { get; set; }
        public virtual String NumFre_Supervisor { get; set; }
        public virtual String Usuario_Monitor { get; set; }
        public virtual String NomeSupervisor { get; set; }
        public virtual String NomeMonitor { get; set; }
        public virtual int? CodTurnoServ { get; set; }
        public virtual DateTime? TurnoHoraIni { get; set; }
        public virtual DateTime? TurnoHoraFim { get; set; }
        public virtual TipoHabilidade? TipoHabilidade { get; set; }
        public virtual StatusExecucaoAniel? Status { get; set; }
        public virtual String Periodo { get; set; }
        public virtual bool? DentroPrazo { get; set; }
        public virtual String CodEncerramento { get; set; }
        public virtual String DescricaoEncerramento { get; set; }
        public virtual String Descricao_Natureza { get; set; }
        public virtual String UsuarioBaixa { get; set; }
        public virtual Status_AuditoriaAniel? StatusAuditoria { get; set; }
        public virtual string NomeTecnico { get; set; }
        public virtual Double? GetLatitude()
        {
            if (!String.IsNullOrEmpty(Latitude_Str))
            {
                return Convert.ToDouble(Latitude_Str.Replace(".", ","));
            }
            else
                return null;
        }
        public virtual Double? GetLongitude()
        {
            if (!String.IsNullOrEmpty(Longitude_Str))
                return Convert.ToDouble(Longitude_Str.Replace(".", ","));
            else
                return null;
        }
        public Int16 GetPrioridade_Nivel(List<LocalidadeRoteirizacao> localidades, DocumentoRoteirizar documento)
        {

            foreach (var localidade in localidades)
            {

                if (localidade.Todas_Localidades)
                {
                    if ((localidade.Cod_Escritorio, localidade.Cod_Area) == (documento.Grupo_Area, documento.Area))
                    {
                        return localidade.Nivel.GetValueOrDefault(0);
                    }
                }

                if ((localidade.Cod_Escritorio, localidade.Cod_Area, localidade.Cod_Localidade) == (documento.Grupo_Area, documento.Area, documento.Cod_Localidade))
                {
                    return localidade.Nivel.GetValueOrDefault(0);
                }

            }

            return 0;

        }
    }

    public class DocumentoRoteirizar : ICloneable
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Num_Obra_Original { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Equipe_Anterior { get; set; }
        //public virtual String Monitor { get; set; }
        //public virtual String Supervisor { get; set; }
        public virtual TimeSpan? Tempo_Execucao { get; set; }
        public virtual double TempoExecucaoMinutos { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Descricao_Grupo_Area { get; set; }
        public virtual StatusExecucaoAniel? StatusOld { get; set; }
        public virtual String Descricao_Area { get; set; }
        public virtual Int32? Cod_Tipo_Imovel { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Int64? Id_Natureza { get; set; }
        public virtual String DescNatureza { get; set; }
        public virtual DateTime? Data_Maxima { get; set; }
        public virtual DateTime? Hora_Maxima { get; set; }
        public virtual string DataMaximaStr { get; set; }
        public virtual string HoraMaximaStr { get; set; }
        public virtual DateTime? SLA { get; set; }
        public virtual TecnicoRoteirizar Tecnico { get; set; }
        public virtual TecnicoRoteirizar Tecnico_Anterior { get; set; }
        public virtual Int64? Id_Agendamento { get; set; }
        public virtual string OrigemAgendamento { get; set; }
        public virtual DateTime? Data_hora_ini_agendamento { get; set; }
        public virtual DateTime? Data_hora_Criacao { get; set; }
        public virtual DateTime? Data_hora_fim_agendamento { get; set; }
        public virtual DateTime? Data_hora_Encerramento { get; set; }
        public virtual DateTime? Data_Hora_Vencimento_OS { get; set; }
        public virtual int? OrdemDistribuida { get; set; }
        public virtual int? NivelRodadaRoteirizada { get; set; }
        public virtual int? AgrupamentoRodadaRoteirizada { get; set; }
        public virtual Int16 DeslocamentoMinutos { get; set; }
        public virtual String Latitude_Str { get; set; }
        public virtual String Longitude_Str { get; set; }
        public virtual String Longitude_Cx { get; set; }
        public virtual String Latitude_Cx { get; set; }
        public virtual Double? Longitude_Localidade { get; set; }
        public virtual Double? Latitude_Localidade { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Descricao_Tipo_Serv { get; set; }
        public virtual String Log { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String NomeSupervisor { get; set; }
        public virtual String NomeMonitor { get; set; }
        public virtual int? CodTurnoServ { get; set; }
        public virtual DateTime? TurnoHoraIni { get; set; }
        public virtual DateTime? TurnoHoraFim { get; set; }
        public virtual TipoHabilidade? TipoHabilidade { get; set; }
        public virtual bool IgnoradaPorConfiguracaoDistanciaMaxima { get; set; }
        public virtual String CodFornecedorAnterior { get; set; }
        public virtual bool IsDocReincidencia { get; set; }

        public virtual Double? GetLatitude()
        {
            if (!String.IsNullOrEmpty(Latitude_Str))
            {
                return Convert.ToDouble(Latitude_Str.Replace(".", ","));
            }
            else
                return null;
        }
        public virtual Double? GetLongitude()
        {
            if (!String.IsNullOrEmpty(Longitude_Str))
                return Convert.ToDouble(Longitude_Str.Replace(".", ","));
            else
                return null;
        }
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        public Int16 GetPrioridade_Nivel(List<LocalidadeRoteirizacao> localidades, DocumentoRoteirizar documento)
        {

            foreach (var localidade in localidades)
            {

                if (localidade.Todas_Localidades)
                {
                    if ((localidade.Cod_Escritorio, localidade.Cod_Area) == (documento.Grupo_Area, documento.Area))
                    {
                        return localidade.Nivel.GetValueOrDefault(0);
                    }
                }

                if ((localidade.Cod_Escritorio, localidade.Cod_Area, localidade.Cod_Localidade) == (documento.Grupo_Area, documento.Area, documento.Cod_Localidade))
                {
                    return localidade.Nivel.GetValueOrDefault(0);
                }

            }

            return 0;

        }
    }

    public class GestaoOperacionalServicosResponse
    {
        private IEnumerable<DocumentoPlanejamento> _data;

        public int Total { get; set; }
        public IEnumerable<DocumentoPlanejamento> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<DocumentoPlanejamento>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }
}
