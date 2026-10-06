using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Equipe_MapaModel
    {
        private List<Equipe_MapaItemModel> _itens;

        #region Filtros
        public virtual Int32 TipoFiltro { get; set; }
        public virtual String Num_Fre_Supervisor { get; set; }
        public virtual String Id_Monitor { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Int64? Id_Natureza { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Endereco { get; set; }
        #endregion

        public virtual List<Equipe_MapaItemModel> Itens
        {
            get
            {
                if (_itens == null)
                    _itens = new List<Equipe_MapaItemModel>();
                return _itens;
            }
            set
            {
                _itens = value;
            }
        }
    }

    public class Equipe_MapaItemModel
    {
        public virtual Int64 Id { get; set; }

        // vem da tabela de funcionario do aniel
        public virtual String Email { get; set; }
        public virtual String Telefone { get; set; }

        public virtual String Data_Busca { get; set; }

        public virtual String Equipe_Busca { get; set; }

        public virtual String Data { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String OrigemLocalizacao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Status_Tecnico { get; set; }
        public virtual String Num_Fre { get; set; }
        public virtual String Equipe_Nome { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Atualizado_Em { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String Logado { get; set; }
        public virtual String DataHora_Login { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }

        public virtual String Justificativa_Ausencia { get; set; }


        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }
        public virtual Boolean Checkpoint { get; set; }

        public virtual String Status_App { get; set; }

        public virtual String InternetMovel { get; set; }
        public virtual String Wifi { get; set; }
        public virtual String IMEI { get; set; }

        public virtual String Servico_Executado { get; set; }
        public virtual String Servico_Sendo_Executado { get; set; }
        public virtual String Servico_Em_Andamento { get; set; }
        public virtual List<Acompanhamento_EquipeListaModel> Cor_StatusEquipe { get; set; }

        public virtual String Cor_StatusEquipe_Mapa { get; set; }
        public virtual String Numero_Os { get; set; }
        public virtual String TipoServico { get; set; }

        public virtual List<Equipe_MapaModel_Planejado> Rotas_Planejadas { get; set; }

        public virtual List<Equipe_MapaModel_Realizada> Rotas_Realizada { get; set; }
        public virtual List<Equipe_MapaModel_Realizada> Rotas_Integracao { get; set; } = new List<Equipe_MapaModel_Realizada>();
        public virtual string DistanciaTotalIntegrada { get; set; }
        public virtual List<Equipe_MapaModel_Realizada> Rotas_Paradas { get; set; } = new List<Equipe_MapaModel_Realizada>();
        public virtual List<Equipe_MapaModel_Realizada> BaixasForaLocal { get; set; } = new List<Equipe_MapaModel_Realizada>();

        public virtual String Status_Execucao { get; set; }

        //public virtual Funcionario Vendedor { get; set; }
        //public virtual String Data { get; set; }

        //public virtual String Endereco_Checkin { get; set; }
        //public virtual String Latitude_Checkin { get; set; }
        //public virtual String Longitude_Checkin { get; set; }
        //public virtual String Longitude_Checkout { get; set; }
        //public virtual String Latitude_Checkout { get; set; }
        //public virtual String Endereco_Checkout { get; set; }

        //public virtual Ponto_Venda Ponto_Venda { get; set; }
    }

    public class Equipe_MapaLogItemModel
    {
        public virtual Int64 Id { get; set; }

        // vem da tabela de funcionario do aniel
        public virtual String Email { get; set; }
        public virtual String Log { get; set; }
        public virtual String Telefone { get; set; }

        public virtual String Data_Busca { get; set; }

        public virtual String Equipe_Busca { get; set; }

        public virtual String Data { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Status_Tecnico { get; set; }
        public virtual String Num_Fre { get; set; }
        public virtual String Equipe_Nome { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Atualizado_Em { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String Logado { get; set; }
        public virtual String DataHora_Login { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }

        public virtual String Justificativa_Ausencia { get; set; }


        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }
        public virtual Boolean Checkpoint { get; set; }

        public virtual String Status_App { get; set; }

        public virtual String InternetMovel { get; set; }
        public virtual String Wifi { get; set; }
        public virtual String IMEI { get; set; }

        public virtual String Servico_Executado { get; set; }
        public virtual String Servico_Sendo_Executado { get; set; }
        public virtual String Servico_Em_Andamento { get; set; }
        public virtual List<Acompanhamento_EquipeListaModel> Cor_StatusEquipe { get; set; }

        public virtual String Cor_StatusEquipe_Mapa { get; set; }
        public virtual String Numero_Os { get; set; }
        public virtual String TipoServico { get; set; }

        public virtual List<Equipe_MapaModel_Planejado> Rotas_Planejadas { get; set; }

        public virtual List<Equipe_MapaModel_Realizada> Rotas_Realizada { get; set; }

        public virtual String Status_Execucao { get; set; }

        //public virtual Funcionario Vendedor { get; set; }
        //public virtual String Data { get; set; }

        //public virtual String Endereco_Checkin { get; set; }
        //public virtual String Latitude_Checkin { get; set; }
        //public virtual String Longitude_Checkin { get; set; }
        //public virtual String Longitude_Checkout { get; set; }
        //public virtual String Latitude_Checkout { get; set; }
        //public virtual String Endereco_Checkout { get; set; }

        //public virtual Ponto_Venda Ponto_Venda { get; set; }
    }

    public class Equipe_MapaModel_Planejado
    {
        public virtual String Ponto_Partida { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String DataHoraStr { get; set; }
        public virtual DateTime DataHora { get; set; }
        public virtual String Data_Ini_Atendimento { get; set; }
        public virtual String Data_Fim_Atendimento { get; set; }

        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual StatusExecucaoAniel? StatusExecucaoAniel { get; set; }
        public virtual String StatusExecucaoAniel_Str { get; set; }
        public virtual String Sla_Servico { get; set; }
        public virtual String TipoLocalizacao { get; set; }
        public virtual String BaixaNoLocal { get; set; }
        public virtual Double? LatitudeBaixa { get; set; }
        public virtual Double? LongitudeBaixa { get; set; }
        public virtual string DataHoraBaixa { get; set; }
    }

    public class Equipe_MapaModel_Realizada
    {
        public virtual String Ponto_Partida { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String DataHoraStr { get; set; }
        public virtual DateTime DataHora { get; set; }
        public String Cor { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Status { get; set; }
        public virtual String Geofence { get; set; }
        public virtual String DataHoraFimStr { get; set; }
        public virtual DateTime DataHoraFim { get; set; }
    }

    public class Equipe_Status_Mobile
    {
        public virtual String Nome { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Foto { get; set; }
    }
}