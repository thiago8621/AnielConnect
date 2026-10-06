using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Horario_ConnectMap : ClassMapBase<Horario_Connect>
    {
        public Horario_ConnectMap() : base()
        {
            Table("TB_HORARIO_CONNECT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_HORARIO_CONNECT");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Padrao, "PADRAO");

            Map(x => x.Codigo, "CODIGO").Length(20);

            Map(x => x.Descricao, "DESCRICAO").Length(50);
            Map(x => x.Tipo, "TIPO").CustomType<Tipo_Hora_Connect>();

            Map(x => x.Opcao_Trabalha_Feriado, "TABALHA_FERIADO");

            Map(x => x.Inicio_Feriado, "INICIO_FERIADO");
            Map(x => x.Termino_Feriado, "TERMINO_FERIADO");
            Map(x => x.Inicio_Intervalo_Feriado, "INICIO_INTERVALO_FERIADO");
            Map(x => x.Termino_Intervalo_Feriado, "TERMINO_INTERVALO_FERIADO");
            Map(x => x.Qtde_HE_Feriado, "QTDE_HE_FERIADO");

            Map(x => x.Inicio_Domingo, "INICIO_DOMINGO");
            Map(x => x.Termino_Domingo, "TERMINO_DOMINGO");
            Map(x => x.Inicio_Intervalo_Domingo, "INICIO_INTERVALO_DOMINGO");
            Map(x => x.Termino_Intervalo_Domingo, "TERMINO_INTERVALO_DOMINGO");
            Map(x => x.Qtde_HE_Domingo, "QTDE_HE_DOMINGO");

            Map(x => x.Inicio_Segunda, "INICIO_SEGUNDA");
            Map(x => x.Termino_Segunda, "TERMINO_SEGUNDA");
            Map(x => x.Inicio_Intervalo_Segunda, "INICIO_INTERVALO_SEGUNDA");
            Map(x => x.Termino_Intervalo_Segunda, "TERMINO_INTERVALO_SEGUNDA");
            Map(x => x.Qtde_HE_Segunda, "QTDE_HE_SEGUNDA");

            Map(x => x.Inicio_Terca, "INICIO_TERCA");
            Map(x => x.Termino_Terca, "TERMINO_TERCA");
            Map(x => x.Inicio_Intervalo_Terca, "INICIO_INTERVALO_TERCA");
            Map(x => x.Termino_Intervalo_Terca, "TERMINO_INTERVALO_TERCA");
            Map(x => x.Qtde_HE_Terca, "QTDE_HE_TERCA");

            Map(x => x.Inicio_Quarta, "INICIO_QUARTA");
            Map(x => x.Termino_Quarta, "TERMINO_QUARTA");
            Map(x => x.Inicio_Intervalo_Quarta, "INICIO_INTERVALO_QUARTA");
            Map(x => x.Termino_Intervalo_Quarta, "TERMINO_INTERVALO_QUARTA");
            Map(x => x.Qtde_HE_Quarta, "QTDE_HE_QUARTA");

            Map(x => x.Inicio_Quinta, "INICIO_QUINTA");
            Map(x => x.Termino_Quinta, "TERMINO_QUINTA");
            Map(x => x.Inicio_Intervalo_Quinta, "INICIO_INTERVALO_QUINTA");
            Map(x => x.Termino_Intervalo_Quinta, "TERMINO_INTERVALO_QUINTA");
            Map(x => x.Qtde_HE_Quinta, "QTDE_HE_QUINTA");

            Map(x => x.Inicio_Sexta, "INICIO_SEXTA");
            Map(x => x.Termino_Sexta, "TERMINO_SEXTA");
            Map(x => x.Inicio_Intervalo_Sexta, "INICIO_INTERVALO_SEXTA");
            Map(x => x.Termino_Intervalo_Sexta, "TERMINO_INTERVALO_SEXTA");
            Map(x => x.Qtde_HE_Sexta, "QTDE_HE_SEXTA");

            Map(x => x.Inicio_Sabado, "INICIO_SABADO");
            Map(x => x.Termino_Sabado, "TERMINO_SABADO");
            Map(x => x.Inicio_Intervalo_Sabado, "INICIO_INTERVALO_SABADO");
            Map(x => x.Termino_Intervalo_Sabado, "TERMINO_INTERVALO_SABADO");
            Map(x => x.Qtde_HE_Sabado, "QTDE_HE_SABADO");

        }
    }
}
