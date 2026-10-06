using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.MGTEC.Models
{
    
    public class PerfilAcessoUsuarioListViewModel : IViewModel<Int64>
    {
        List<PerfilAcessoUsuarioListItensViewModel> _usuarios;

        public Int64 Id { get; set; }

        public virtual Int32 Codigo { get; set; }

        public virtual String Descricao { get; set; }

        public UsuarioListagemViewModel UsuarioSelecionado { get; set; }

        public List<PerfilAcessoUsuarioListItensViewModel> Usuarios
        {
            get
            {
                if (_usuarios == null)
                    _usuarios = new List<PerfilAcessoUsuarioListItensViewModel>();
                return _usuarios;
            }
            set
            {
                _usuarios = value;
            }
        }
    }


    public class PerfilAcessoUsuarioListItensViewModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        public Int64 IdPerfil { get; set; }

        public String IdUsuario { get; set; }

        public String Login { get; set; }

        public String Nome { get; set; }

        public String Email { get; set; }
    }

    public class PerfilAcessoUsuarioInsertViewModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        public Int64 IdPerfil { get; set; }

        public String IdUsuario { get; set; }
    }

}