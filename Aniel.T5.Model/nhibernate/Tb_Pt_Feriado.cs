using Aniel.T5.Model.enumerado;
using Sinapse.Core;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    [Serializable]
    [TokenLess]
    public class Tb_Pt_Feriado : BaseModel
    {
        public virtual String Descricao { get; set; }

        public virtual Tipo_Feriado Tipo_Feriado { get; set; }

        public virtual Int16 Ano { get; set; }

        public virtual Int16 Mes { get; set; }
        public virtual Int16 Dia { get; set; }

        public virtual DateTime? Data { get; set; }

        public virtual String UF { get; set; }

        public virtual String Municipio { get; set; }

        public virtual Enum_Abrangencia Abrangencia { get; set; }

        public virtual String Pais { get; set; }

        public virtual Int64? Id_Municipio { get; set; }

        public virtual Status_Default Status { get; set; }

        public override string ToString()
        {
            return Descricao;
        }

        public override int GetHashCode()
        {
            return Descricao.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Descricao.Equals(((Tb_Pt_Feriado)obj).Descricao);
        }
    }
}
