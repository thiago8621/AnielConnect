using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ConsultaSerasaDadosMap : ClassMapBase<ConsultaSerasaDados>
    {
        public ConsultaSerasaDadosMap() : base()
        {
            Table("TB_CONSULTA_SERASA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONSULTA_SERASAPF");
            Map(u => u.consulta, "CONSULTA").Length(8000);
            Map(u => u.cpf, "CPF").Length(15);
            Map(u => u.cnpj, "CNPJ").Length(15);
        }
    }
}
