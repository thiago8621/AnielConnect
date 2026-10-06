using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    class Codigo_EncerramentoAnielMap : ClassMap<Codigo_EncerramentoAniel>
    {
        public Codigo_EncerramentoAnielMap()
        {
            Table("TB_CODIGO_ENCERRAMENTO");

            CompositeId()
            .KeyProperty(x => x.Cod_Encerramento, "COD_ENCERRAMENTO")
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");



            Map(x => x.Encerra_OS_Apos_Baixa, "ENCERRA_OS_APOS_BAIXA").Length(4);

            Map(x => x.Descricao, "DESCRICAO").Length(95);
            Map(x => x.Status_Execucao, "STATUS_EXECUCAO");

        }
    }
}
