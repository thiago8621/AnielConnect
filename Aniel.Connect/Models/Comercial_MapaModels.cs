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
    public class Comercial_MapaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Data { get; set; }
        //public virtual String Latitude { get; set; }
        //public virtual String Longitude { get; set; }

        public virtual Double? Latitude_CheckIn { get; set; }
        public virtual Double? Longitude_CheckIn { get; set; }

        public virtual Double? Latitude_CheckOut { get; set; }
        public virtual Double? Longitude_CheckOut { get; set; }

        public virtual String Vendedor { get; set; }
        public virtual String Num_Fre { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Supervisor { get; set; }
      
        public virtual String Email { get; set; }
        public virtual String Logado { get; set; }
        public virtual String DataHora_Login { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Atualizado_Em { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }
        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }
        public virtual String Data_Busca { get; set; }
        public virtual String Vendedor_Busca { get; set; }
        public virtual Boolean Checkpoint { get; set; }

        public virtual String Servico_Executado { get; set; }
        public virtual String Servico_Sendo_Executado { get; set; }
        public virtual String Servico_Em_Andamento { get; set; }
        public virtual List<Acompanhamento_EquipeListaModel> Cor_StatusEquipe { get; set; }

        public virtual String Cor_StatusEquipe_Mapa { get; set; }

        public virtual List<Equipe_MapaModel_Planejado> Rotas_Planejadas { get; set; }

        public virtual List<Equipe_MapaModel_Realizada> Rotas_Realizada { get; set; }

        public virtual List<Comercial_MapaModel> Pontos_Venda { get; set; }

        public virtual List<Checkpoint_VendedorModel> Checkpoints_Vendedor { get; set; }


        public virtual List<Comercial_Mapa_PontoVenda_CheckPointModel> PontoVenda_CheckPoint_Mapa { get; set; }




    }

    public class Comercial_Mapa_PontoVenda_CheckPointModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Ponto_Venda { get; set; }

        public virtual String Desc_Nome_Venda { get; set; }

        public virtual String Data { get; set; }

        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }

        public virtual Double? Latitude_CheckIn { get; set; }
        public virtual Double? Longitude_CheckIn { get; set; }

        public virtual Double? Latitude_CheckOut { get; set; }
        public virtual Double? Longitude_CheckOut { get; set; }
        public virtual String Vendedor { get; set; }
       
        public virtual String Tipo { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Atualizado_Em { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }
        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }

        public virtual List<Comercial_MapaModel> Pontos_Venda { get; set; }

        public virtual List<Checkpoint_VendedorModel> Checkpoints_Vendedor { get; set; }
        

        public virtual Boolean Checkpoint { get; set; }
        public virtual String Obs_Checkpoint { get; set; }

        public virtual Boolean Ponto_Venda { get; set; }

        public virtual Boolean CheckIn { get; set; }
        public virtual DateTime? DataHora_CheckIn { get; set; }

        public virtual Boolean CheckOut { get; set; }


        public virtual Int64 Count_CheckIn { get; set; }
    }

}