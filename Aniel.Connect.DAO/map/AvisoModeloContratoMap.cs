using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class AvisoModeloContratoMap : ClassMapBase<AvisoModeloContrato>
    {
        public AvisoModeloContratoMap() : base()
        {
            Table("AVISO_MODELO_CONTRATO");

            Id(x => x.Id);

            Map(x => x.QuantidadeDias, "QUANTIDADE_DIAS");

            Map(x => x.IdModeloContrato, "ID_MODELO_CONTRATO");
            Map(x => x.IdEmail, "ID_EMAIL");
            Map(x => x.IdSMS, "ID_SMS");
            Map(x => x.IdNotificacao, "ID_NOTIFICACAO");

            Map(x => x.TipoAviso, "TIPO_AVISO").CustomType<TipoAvisoModeloContratoEnum>();
        }
    }
}