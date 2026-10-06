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
    public class CamposObrigatoriosDocumentoProducaoMap : ClassMapBase<CamposObrigatoriosDocumentoProducao>
    {
        public CamposObrigatoriosDocumentoProducaoMap() : base()
        {
            Table("TB_CAMPOS_OBRIG_DOC_PROD");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPOS_OBRIG_DOC_PROD");

            Map(x => x.Campo, "CAMPO").CustomType<CamposObrigatoriosDocumentoProducaoEnum>();

            Map(x => x.Titulo, "TITULO").Length(100);
            Map(x => x.Bloqueado , "VISIVEL");
            Map(x => x.Obrigatorio, "OBRIGATORIO");
        }
    }
}
