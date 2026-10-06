using System;

namespace Aniel.T5.Model.nhibernate
{
    public class SenhaAniel 
    {
        public virtual String Usuario { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Administrador { get; set; }

        public virtual String Senha { get; set; }
        
        /// <summary>
        ///     Objeto apenas para leitura utilizado em consultas com o NHibernate
        /// </summary>
        public virtual FuncionarioAniel Funcionario { get; set; }
        public virtual String NumFre { get; set; }

        public virtual Int64? Perfil_Dash_Cherry { get; set; }
        public virtual String Desc_Perfil_Dash_Cherry { get; set; }
    }
}
