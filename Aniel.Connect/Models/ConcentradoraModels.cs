using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class ConcentradoraListModel
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
        public String Ip { get; set; }
        public String Tipo { get; set; }
        public String POP { get; set; }
        public String Radius { get; set; }
    }

    public class ConcentradoraCadastroModel
    {
        public Int64 Id { get; set; }

        public String Nome { get; set; }
        public Marca_Modelo_Concentradora? Tipo { get; set; }

        public Int64 IdPop { get; set; }
        public String Ip { get; set; }

        public String Login { get; set; }
        public String Senha { get; set; }

        public Int64 IdRadius { get; set; }

        public String PortaTelnet { get; set; }
        public String PortaAPI { get; set; }
        public String PortaHTTP { get; set; }
        public String PortaSSH { get; set; }
        public String PortaIncoming { get; set; }

        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public String Cod_Localidade { get; set; }

        private List<Concentradora_Grupo_AreaListModel> GrupoAreaLista;
        public virtual List<Concentradora_Grupo_AreaListModel> GruposArea
        {
            get
            {
                if (GrupoAreaLista == null)
                    GrupoAreaLista = new List<Concentradora_Grupo_AreaListModel>();
                return GrupoAreaLista;
            }
            set
            {
                GrupoAreaLista = value;
            }
        }
    }

    public class ConcentradoraDatatableRequest : DataTableRequest
    {
        public Int32? Tipo { get; set; }
        public Int64? IdPop { get; set; }
        public Int64? IdRadius { get; set; }
    }
}