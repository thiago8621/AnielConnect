using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class HorarioBusiness
    {
        public static bool AptoHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino,
                                DateTime hora_inicio, DateTime? hora_fim)
        {
            if (!horario_inicio.HasValue || !horario_termino.HasValue)
                return false;

            if (hora_inicio.TimeOfDay < horario_inicio?.TimeOfDay || hora_fim?.TimeOfDay > horario_termino?.TimeOfDay)
            {
                return false;
            }

            if (horario_inicio_intervalo.HasValue && horario_termino_intervalo.HasValue)
            {

                if (Between(hora_inicio, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (Between(hora_fim, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (hora_inicio.TimeOfDay < horario_inicio_intervalo?.TimeOfDay && hora_fim?.TimeOfDay > horario_inicio_intervalo?.TimeOfDay)
                    return false;

            }



            return true;
        }
        public static bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            date1 = date1.Value.AddMinutes(1);
            date2 = date2.Value.AddMinutes(-1);
            return (input?.TimeOfDay > date1.Value.TimeOfDay && input.Value.TimeOfDay < date2.Value.TimeOfDay);
        }
        public static bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }
        public static bool IgualDatas(DateTime? date1, DateTime? date2)
        {
            return (date1.Value.TimeOfDay == date2.Value.TimeOfDay);
        }
    }
}
