using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class VincularUsuarioPerfilMap : ClassMapBase<VincularUsuarioPerfil>
    {
        public VincularUsuarioPerfilMap() : base() {
            Table("TB_USUARIO_VINCULADO_PERFIL");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_Tipo_Categoria_Vendedor");
            Map(x => x.Perfil_Vinculado, "PERFIL_VINCULADO"); 
            Map(x => x.Secao, "SECAO"); 
            Map(x => x.Tipo_Usuario, "TIPO_USUARIO"); 
            References(m => m.IdentityUser, "ID_USUARIO").ForeignKey("FK_USUARIO");
            References(m => m.Perfil_DashBoard_Connect, "ID_PEFILDASHBOARD").ForeignKey("FK_PEFILDASHBOARD");


        }
    }
}
