using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Model.enumerado;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CalendarioMap : ClassMapBase<Calendario>
    {
        public CalendarioMap() : base()
        {
            Table("TB_CALENDARIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CALENDARIO");

            Map(x => x.Domingo_Entrada, "DOMINGO_ENTRADA");
            Map(x => x.Domingo_Saida, "DOMINGO_SAIDA");

            Map(x => x.Segunda_Entrada, "SEGUNDA_ENTRADA");
            Map(x => x.Segunda_Saida, "SEGUNDA_SAIDA");

            Map(x => x.Terca_Entrada, "TERCA_ENTRADA");
            Map(x => x.Terca_Saida, "TERCA_SAIDA");

            Map(x => x.Quarta_Entrada, "QUARTA_ENTRADA");
            Map(x => x.Quarta_Saida, "QUARTA_SAIDA");

            Map(x => x.Quinta_Entrada, "QUINTA_ENTRADA");
            Map(x => x.Quinta_Saida, "QUINTA_SAIDA");

            Map(x => x.Sexta_Entrada, "SEXTA_ENTRADA");
            Map(x => x.Sexta_Saida, "SEXTA_SAIDA");

            Map(x => x.Sabado_Entrada, "SABADO_ENTRADA");
            Map(x => x.Sabado_Saida, "SABADO_SAIDA");


            Map(x => x.Tipo_Calendario, "TIPO_CALENDARIO").CustomType<Tipo_Calendario>();
        }
    }
}
