using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class BaseClass
    {
        private List<PropertyInfo> pks
        {
            get
            {
                List<PropertyInfo> retorno = new List<PropertyInfo>();
                List<PropertyInfo> props = this.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Column))).ToList();
                foreach (PropertyInfo prop in props)
                {
                    Column coluna = (Column)this.GetType().GetProperty(prop.Name).GetCustomAttributes(typeof(Column), false).FirstOrDefault();
                    if (coluna.getTarget().Equals(ColumnTargets.Key)) retorno.Add(prop);
                }

                return retorno;
            }
        }

        [Column(ColumnTargets.Key)]
        public virtual Int64 Id { get; set; }

        public virtual void CopyTo(object dest)
        {
            List<PropertyInfo> ps = this.GetType().GetProperties().ToList();
            foreach (PropertyInfo p in ps)
                p.SetValue(dest, p.GetValue(this, null), null);
        }

        public override bool Equals(object obj)
        {
            bool retorno = obj != null;
            if (retorno)
            {
                if (this.GetType() != obj.GetType())
                    return false;

                retorno = true;
                foreach (PropertyInfo campo in pks)
                {
                    if (campo.GetValue(this) != campo.GetValue(obj))
                    {
                        retorno = false;
                        break;
                    }

                }

            }
            return retorno;
        }

        public override int GetHashCode()
        {
            string retorno = string.Empty;
            foreach (PropertyInfo campos in pks)
            {
                if (!string.IsNullOrEmpty(retorno)) retorno += "|";
                retorno += campos.GetHashCode();
            }

            return retorno.GetHashCode();
        }
    }
}
