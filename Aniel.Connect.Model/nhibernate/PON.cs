using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class PON : BaseModel
    {
        public virtual Int64 PONID { get; set; }
        public virtual Int64? Num_PON { get; set; }
        
        public virtual String Interface { get; set; }

        public virtual String Descricao { get; set; }
        public virtual OLT OLT { get; set; }
        public virtual Slot Slot { get; set; }
        public virtual Status_Default Status { get; set; }

        public virtual Int32? Potencia_Saida { get; set; }
        public virtual double? LimitePotenciaONUBaixa { get; set; }

        public virtual Int16? VLAN_Uplink { get; set; }
        public virtual Int16? VLAN_PPPOE { get; set; }
        public virtual Int16? VLAN_DHCP { get; set; }
        public virtual Int16? VLAN_IPTV { get; set; }
        public virtual Int16? VLAN_VOIP { get; set; }
        public virtual Int16? VLAN_TR69 { get; set; }
        public virtual Int16? VLAN_Outros { get; set; }
        public virtual string VLAN_Profile { get; set; }
        public virtual string CodigoAuxiliar { get; set; }

        #region Campos Antigos
        public virtual Int16? Id_VLAN_Voz { get; set; }
        public virtual Int16? Id_VLAN_Dados { get; set; }
        public virtual Int16? Id_VLAN_Multicast { get; set; }

        #endregion

        public virtual String GetPonID()
        {   
            return $"{this.Slot?.OLT?.Filial?.Empresa?.CodEmpresa}-{this.Slot?.OLT?.Filial?.CodFilial}-{this.Slot?.OLT?.Descricao}-{this.Slot?.SlotID}-{this.PONID}";
        }
        public virtual String GetPonIDFiberhome() 
        {
            return $"NA-{this.OLT?.Subrack}-{this.Slot?.SlotID}-{this.PONID}";
        }
    }
}
