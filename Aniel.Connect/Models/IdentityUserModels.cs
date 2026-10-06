using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class IdentityUserListaModel
    {
        public virtual String Id { get; set; }
        public virtual String Nome { get; set; }  
        public virtual String Departamento { get; set; }

    }

    public class IdentityUserRequestModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Monitor { get; set; }

    }

    public class IdentityUserMonitorModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual String Desc_User { get; set; }
        public virtual String Id_Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; } 
        public virtual String MonitorFiltro { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }

         public virtual String Lista_Equipes { get; set; }

        private List<EquipeDataTableListaModel> EquipeLista;

        public virtual List<EquipeDataTableListaModel> Equipes

        {
            get
            {
                if (EquipeLista == null)
                    EquipeLista = new List<EquipeDataTableListaModel>();
                return EquipeLista;
            }
            set
            {
                EquipeLista = value;
            }
        }


    }

    public class IdentityUserComboModel : ObjComboResult<String>
    {

    }

}