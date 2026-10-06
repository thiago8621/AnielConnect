using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Checkpoint_VendedorModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Data { get; set; }

        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }

        public virtual String Vendedor { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Atualizado_Em { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }
        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }
        public virtual Boolean Checkpoint { get; set; }

        //public virtual String Servico_Executado { get; set; }
        //public virtual String Servico_Sendo_Executado { get; set; }
        //public virtual String Servico_Em_Andamento { get; set; }
        //public virtual List<Acompanhamento_EquipeListaModel> Cor_StatusEquipe { get; set; }

        //public virtual String Cor_StatusEquipe_Mapa { get; set; }

        //public virtual List<Equipe_MapaModel_Planejado> Rotas_Planejadas { get; set; }

        //public virtual List<Equipe_MapaModel_Realizada> Rotas_Realizada { get; set; }

        public virtual List<Ponto_VendaListaModel> Pontos_Venda { get; set; }

        public virtual List<Ponto_VendaListaModel> Checkpoints_Vendedor { get; set; }







    }

    public class Checkpoint_VendedorMobileModel
    {
        public Int64 idPontoVenda { get; set; }
        public Int64 idVendedor { get; set; }
        public Checkin_Vendedor checkPoint { get; set; }
    }
}