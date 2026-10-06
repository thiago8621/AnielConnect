using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class OLTCadastroModel
    {
        public Int64? Delay { get; set; }
        public Int64 Id { get; set; }
        public String CodigoAuxiliar { get; set; }
        public String NumOLT { get; set; }
        public String OLTID { get; set; }

        public Int64 IdPOP { get; set; }

        public Int32? PortaTelnet { get; set; }
        public Int32? PortaAPI { get; set; }
        public Int32? PortaHTTP { get; set; }
        public Int32? PortaSSH { get; set; }

        public String Descricao { get; set; }

        public Marca_Modelo_OLT Marca_Modelo { get; set; }
        public String Ip { get; set; }

        public String Login { get; set; }

        public String Senha { get; set; }
        public string LimitePotenciaONUBaixa { get; set; }

        public Boolean Status { get; set; }
        public Boolean ConverteOlt { get; set; }
        public Int16? VLAN_Uplink { get; set; }
        public bool Config_PPPOE_Manual { get; set; }

        #region TL1

        public String IpTL1 { get; set; }
        public String LoginTL1 { get; set; }
        public String SenhaTL1 { get; set; }
        public Int32? PortaTL1 { get; set; }
        public String Gabinete { get; set; }
        public String Subrack { get; set; }

        #endregion

        public ONU_Tipo_Identificacao? Tipo_Identificacao { get; set; }
        public virtual string Cod_Escritorio { get; set; }
        public virtual string Cod_Area { get; set; }
        private List<OLT_Grupo_AreaListModel> GrupoAreaLista;
        public virtual List<OLT_Grupo_AreaListModel> GruposArea
        {
            get
            {
                if (GrupoAreaLista == null)
                    GrupoAreaLista = new List<OLT_Grupo_AreaListModel>();
                return GrupoAreaLista;
            }
            set
            {
                GrupoAreaLista = value;
            }
        }
    }
    public class OLT_Grupo_AreaListModel
    {
        public long Id { get; set; }
        public string Cod_Area { get; set; }
        public string Desc_Area { get; set; }

        public string Cod_Escritorio { get; set; }
        public string Desc_Escritorio { get; set; }
    }
}
