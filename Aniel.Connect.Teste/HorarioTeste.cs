using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Functions;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Teste
{
    [TestClass]
    public class HorarioTeste
    {

        public DateTime GetNextDayofTheWeek(DayOfWeek dia_semana) {

            var dia = DateTime.Today.AddDays(1);
            while (dia.DayOfWeek != dia_semana) {
                dia = dia.AddDays(1);
            }

            return dia;

        }

        public Horario_Connect GetHorarioMock()
        {
            return new Horario_Connect
            {
                Inicio_Domingo = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Domingo = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Domingo = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Domingo = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Segunda = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Segunda = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Segunda = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Segunda = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Terca = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Terca = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Terca = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Terca = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Quarta = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Quarta = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Quarta = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Quarta = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Quinta = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Quinta = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Quinta = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Quinta = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Sexta = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Sexta = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Sexta = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Sexta = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Sabado = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Sabado = Convert.ToDateTime("01/10/2020 12:00"),
                Termino_Intervalo_Sabado = Convert.ToDateTime("01/10/2020 13:00"),
                Termino_Sabado = Convert.ToDateTime("01/10/2020 18:00"),

                Inicio_Feriado = Convert.ToDateTime("01/10/2020 08:00"),
                Inicio_Intervalo_Feriado = null,
                Termino_Intervalo_Feriado = null,
                Termino_Feriado = Convert.ToDateTime("01/10/2020 09:30"),

            };
        }

        [TestMethod]
        public void Passo01_VerificarHoraAptaPorHorario()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday),false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("08:30"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("11:00"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(true, resultado);
        }

        [TestMethod]
        public void Passo02_VerificarHoraNaoAptaPorHorarioTermino()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday), false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("16:00"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("19:00"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(false, resultado);
        }

        [TestMethod]
        public void Passo03_VerificarHoraNaoAptaPorHorarioInicio()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday), false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("06:00"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("09:00"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(false, resultado);
        }

        [TestMethod]
        public void Passo04_VerificarHoraNaoAptaPorHorarioInicioIntervalo()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday), false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("12:30"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("13:30"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(false, resultado);
        }

        [TestMethod]
        public void Passo05_VerificarHoraNaoAptaPorHorarioTerminoIntervalo()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday), false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("11:30"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("12:02"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(false, resultado);
        }

        [TestMethod]
        public void Passo06_VerificarHoraNaoAptaPorHorarioEntreIntervalo()
        {
            var horario_api = new HorarioApiController();
            var horario = GetHorarioMock();
            var horario_Dia = horario_api.RetornaHorarioDia(horario, GetNextDayofTheWeek(DayOfWeek.Monday), false);
            var horario_requisicao_inicio = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("11:40"));
            var horario_requisicao_fim = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Monday).ToString("13:30"));


            var resultado = horario_api.AptoHorario(horario_Dia.inicio.Value, horario_Dia.inicio_intervalo, horario_Dia.termino_intervalo, horario_Dia.termino.Value,
                                    horario_requisicao_inicio, horario_requisicao_fim);



            Assert.AreEqual(false, resultado);
        }

        [TestMethod]
        public void Passo07_VerificarHorarioBetween()
        {
            DateTime dataInicioServico = new DateTime(2022, 05, 18, 9, 30,0);
            DateTime dataFinalServico = new DateTime(2022, 05, 18,11, 30, 0);

            DateTime dataAgendamentoInicio = new DateTime(2022, 05, 18, 10, 00, 0);
            DateTime dataAgendamentoFim = new DateTime(2022, 05, 18, 11, 00, 0);

            List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)> jaAgendadosDia = new List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>();
            jaAgendadosDia.Add((equipe: "renato", dataHoraInicio: dataInicioServico, dataHoraFim: dataFinalServico));

            List<String> JaAgendados = new List<String>();
            JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => HorarioApiController.BetweenIgual(p.dataHoraInicio, dataAgendamentoInicio, dataAgendamentoFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(p.dataHoraFim, dataAgendamentoInicio.AddSeconds(1), dataAgendamentoFim)
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoInicio, p.dataHoraInicio, p.dataHoraFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoFim, p.dataHoraInicio.AddSeconds(1), p.dataHoraFim)).Select(p => p.equipe).ToList()).ToList();





            Assert.AreEqual(true, JaAgendados.Count > 0);
        }

        [TestMethod]
        public void Passo08_VerificarHorarioBetween()
        {
            DateTime dataInicioServico = new DateTime(2022, 05, 18, 9, 30, 0);
            DateTime dataFinalServico = new DateTime(2022, 05, 18, 11, 30, 0);

            DateTime dataAgendamentoInicio = new DateTime(2022, 05, 18, 13, 00, 0);
            DateTime dataAgendamentoFim = new DateTime(2022, 05, 18, 14, 00, 0);

            List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)> jaAgendadosDia = new List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>();
            jaAgendadosDia.Add((equipe: "renato", dataHoraInicio: dataInicioServico, dataHoraFim: dataFinalServico));

            List<String> JaAgendados = new List<String>();
            JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => HorarioApiController.BetweenIgual(p.dataHoraInicio, dataAgendamentoInicio, dataAgendamentoFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(p.dataHoraFim, dataAgendamentoInicio.AddSeconds(1), dataAgendamentoFim)
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoInicio, p.dataHoraInicio, p.dataHoraFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoFim, p.dataHoraInicio.AddSeconds(1), p.dataHoraFim)).Select(p => p.equipe).ToList()).ToList();





            Assert.AreEqual(false, JaAgendados.Count > 0);
        }

        [TestMethod]
        public void Passo09_VerificarHorarioBetween()
        {
            DateTime dataInicioServico = new DateTime(2022, 05, 18, 9, 30, 0);
            DateTime dataFinalServico = new DateTime(2022, 05, 18, 11, 30, 0);

            DateTime dataAgendamentoInicio = new DateTime(2022, 05, 18, 11, 30, 0);
            DateTime dataAgendamentoFim = new DateTime(2022, 05, 18, 13, 00, 0);

            List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)> jaAgendadosDia = new List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>();
            jaAgendadosDia.Add((equipe: "renato", dataHoraInicio: dataInicioServico, dataHoraFim: dataFinalServico));

            List<String> JaAgendados = new List<String>();
            JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => HorarioApiController.BetweenIgual(p.dataHoraInicio, dataAgendamentoInicio, dataAgendamentoFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(p.dataHoraFim, dataAgendamentoInicio.AddSeconds(1), dataAgendamentoFim)
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoInicio, p.dataHoraInicio, p.dataHoraFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoFim, p.dataHoraInicio.AddSeconds(1), p.dataHoraFim)).Select(p => p.equipe).ToList()).ToList();





            Assert.AreEqual(false, JaAgendados.Count > 0);
        }

        [TestMethod]
        public void Passo10_VerificarHorarioBetween()
        {
            DateTime dataInicioServico = new DateTime(2022, 05, 18, 9, 30, 0);
            DateTime dataFinalServico = new DateTime(2022, 05, 18, 11, 30, 0);

            DateTime dataAgendamentoInicio = new DateTime(2022, 05, 18, 8, 0, 0);
            DateTime dataAgendamentoFim = new DateTime(2022, 05, 18, 9, 30, 0);

            List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)> jaAgendadosDia = new List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>();
            jaAgendadosDia.Add((equipe: "renato", dataHoraInicio: dataInicioServico, dataHoraFim: dataFinalServico));

            List<String> JaAgendados = new List<String>();
            JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => HorarioApiController.BetweenIgual(p.dataHoraInicio, dataAgendamentoInicio, dataAgendamentoFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(p.dataHoraFim, dataAgendamentoInicio.AddSeconds(1), dataAgendamentoFim)
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoInicio, p.dataHoraInicio, p.dataHoraFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(dataAgendamentoFim, p.dataHoraInicio.AddSeconds(1), p.dataHoraFim)).Select(p => p.equipe).ToList()).ToList();





            Assert.AreEqual(false, JaAgendados.Count > 0);
        }



    }
}
