using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class SigaMeMap : ClassMapBase<SigaMe>
    {
        public SigaMeMap()
        {
            Table("TB_SIGAME");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SIGAME");

            Map(x => x.NumeroOrigem, "NUMERO_ORIGEM");
            Map(x => x.NumeroDestino, "NUMERO_DESTINO");
            Map(x => x.DefinirHorario, "DEFINIR_HORARIO");
            Map(x => x.HorarioInicial, "HORARIO_INICIAL");
            Map(x => x.HorarioFinal, "HORARIO_FINAL");
            Map(x => x.Segunda, "SEGUNDA");
            Map(x => x.Terca, "TERCA");
            Map(x => x.Quarta, "QUARTA");
            Map(x => x.Quinta, "QUINTA");
            Map(x => x.Sexta, "SEXTA");
            Map(x => x.Sabado, "SABADO");
            Map(x => x.Domingo, "DOMINGO");

            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_TB_PLANO_SIGAME");
        }
    }
}
