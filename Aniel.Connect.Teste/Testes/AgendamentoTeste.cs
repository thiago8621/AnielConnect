using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Teste.Testes
{
    [TestClass]
    public class AgendamentoTeste
    {
        [TestMethod]
        public void NaturezaReparo()
        {
            DateTime dia = Convert.ToDateTime("22/08/2022");
            bool feriado = false;
            DadosServicoAgendamento servico = new DadosServicoAgendamento
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1,
                Cod_Natureza = 2,
                LocalidadeStr = "bangu",
                TempoExecucaoMinutos = dia.AddMinutes(60)
            };
            Horario_Connect horario1 = new Horario_Connect
            {
                Id = 1,
                Codigo = "1",
                Descricao = "horario1",
                Tipo = T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico,
                Inicio_Segunda = dia.AddHours(8),
                Inicio_Intervalo_Segunda = dia.AddHours(12),
                Termino_Intervalo_Segunda = dia.AddHours(13),
                Termino_Segunda = dia.AddHours(18),
            };

            List<Horario_Connect> horariosServico = new List<Horario_Connect> { horario1 };

            Horario_Connect horarioAgendamento = new Horario_Connect
            {
                Id = 1000,
                Codigo = "1000",
                Descricao = "horarioAgendamento",
                Tipo = T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento,
                Inicio_Segunda = dia.AddHours(8),
                Termino_Segunda = dia.AddHours(20),
            };

            Configuracoes_Gerais config = new Configuracoes_Gerais
            {
                Tempo_Execucao_Padrao = dia.AddMinutes(40),
                Criterio_Area = true,
                Criterio_Habilidade = true,
                Tempo_Medio_Deslocamento = dia.AddMinutes(15)
            };

            AgendamentoEquipeHorarioModel dadosTecnicoLeandro = new AgendamentoEquipeHorarioModel
            {
                Id_Horario_Connect = 1,
                Nome = "leandro",
                Equipe = "312218",
                Naturezas = new List<long?> { 1 },
                Localidades = new List<LocalidadeRoteirizacao> {
                    new LocalidadeRoteirizacao{
                        Cod_Escritorio = "1",
                        Cod_Area = "1",
                        Cod_Localidade = 1,
                        Descricao_Area = "rj",
                        Descricao_Escritorio = "rj",
                        LocalidadeStr = "bangu",
                        Nivel = 1
                    }
                }
            };

            List<AgendamentoEquipeHorarioModel> forcaTrabalho = new List<AgendamentoEquipeHorarioModel> { dadosTecnicoLeandro };
            List<AgendamentoDadosModel> jaAgendados = new List<AgendamentoDadosModel>();

            Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController();

            var resultadamentoAgendamento = agendamentoApi.BuscarAgendamentoDia(dia,
                                                                                jaAgendados,
                                                                                forcaTrabalho,
                                                                                horariosServico,
                                                                                horarioAgendamento,
                                                                                config,
                                                                                feriado,
                                                                                servico,
                                                                                new List<Turno_ServicoAniel>(),
                                                                                null);



            Assert.AreEqual(expected: 0, actual: resultadamentoAgendamento?.Horas?.Count ?? 0);
        }

        [TestMethod]
        public void NaturezaInstalacao()
        {
            DateTime dia = Convert.ToDateTime("22/08/2022");
            bool feriado = false;
            DadosServicoAgendamento servico = new DadosServicoAgendamento
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1,
                Cod_Natureza = 1,
                LocalidadeStr = "bangu",
                TempoExecucaoMinutos = dia.AddMinutes(60)
            };
            Horario_Connect horario1 = new Horario_Connect
            {
                Id = 1,
                Codigo = "1",
                Descricao = "horario1",
                Tipo = T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico,
                Inicio_Segunda = dia.AddHours(8),
                Inicio_Intervalo_Segunda = dia.AddHours(12),
                Termino_Intervalo_Segunda = dia.AddHours(13),
                Termino_Segunda = dia.AddHours(18),
            };

            List<Horario_Connect> horariosServico = new List<Horario_Connect> { horario1 };

            Horario_Connect horarioAgendamento = new Horario_Connect
            {
                Id = 1000,
                Codigo = "1000",
                Descricao = "horarioAgendamento",
                Tipo = T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento,
                Inicio_Segunda = dia.AddHours(8),
                Termino_Segunda = dia.AddHours(20),
            };

            Configuracoes_Gerais config = new Configuracoes_Gerais
            {
                Tempo_Execucao_Padrao = dia.AddMinutes(40),
                Criterio_Area = true,
                Criterio_Habilidade = true,
                Tempo_Medio_Deslocamento = dia.AddMinutes(15)
            };

            AgendamentoEquipeHorarioModel dadosTecnicoLeandro = new AgendamentoEquipeHorarioModel
            {
                Id_Horario_Connect = 1,
                Nome = "leandro",
                Equipe = "312218",
                Naturezas = new List<long?> { 1 },
                Localidades = new List<LocalidadeRoteirizacao> {
                    new LocalidadeRoteirizacao{
                        Cod_Escritorio = "1",
                        Cod_Area = "1",
                        Cod_Localidade = 1,
                        Descricao_Area = "rj",
                        Descricao_Escritorio = "rj",
                        LocalidadeStr = "bangu",
                        Nivel = 1
                    }
                }
            };

            List<AgendamentoEquipeHorarioModel> forcaTrabalho = new List<AgendamentoEquipeHorarioModel> { dadosTecnicoLeandro };
            List<AgendamentoDadosModel> jaAgendados = new List<AgendamentoDadosModel>();

            Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController();

            var resultadamentoAgendamento = agendamentoApi.BuscarAgendamentoDia(dia,
                                                                                jaAgendados,
                                                                                forcaTrabalho,
                                                                                horariosServico,
                                                                                horarioAgendamento,
                                                                                config,
                                                                                feriado,
                                                                                servico,
                                                                                new List<Turno_ServicoAniel>(),
                                                                                null);



            Assert.AreEqual(expected: 7, actual: resultadamentoAgendamento?.Horas?.Count ?? 0);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 14, 00, 49);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 16, 00, 0);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreNotEqual(resultado, null);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado2()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 14, 00, 0);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 16, 00, 0);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreNotEqual(resultado, null);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado3()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 14, 00, 49);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 16, 00, 53);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreNotEqual(resultado, null);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado4()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 16, 00, 53);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreNotEqual(resultado, null);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado5()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 13, 00, 00);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 13, 59, 59);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreEqual(resultado, null);
        }

        [TestMethod]
        public void VerificaServicoJaAgendado6()
        {
            DateTime dataInicioServicoAgendado = new DateTime(2022, 10, 14, 16, 00, 00);
            DateTime dataFinalServicoAgendado = new DateTime(2022, 10, 14, 16, 59, 59);

            DateTime dataInicioServico = new DateTime(2022, 10, 14, 14, 00, 00);
            DateTime dataFinalServico = new DateTime(2022, 10, 14, 16, 00, 00);


            List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos = new List<Painel_Servicos_AgendamentosEquipe>();
            ja_agendadosTecnicos.Add(new Painel_Servicos_AgendamentosEquipe
            {
                Data_Inicial = dataInicioServicoAgendado,
                Data_Final = dataFinalServicoAgendado
            });

            RoteirizacaoBase roteirizacaoBase = new RoteirizacaoBase();
            var resultado = roteirizacaoBase.jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, new AgendamentoHorasDisponiveis
            {
                Hora_Inicio = dataInicioServico,
                Hora_Final = dataFinalServico
            });

            Assert.AreEqual(resultado, null);
        }

    }
}
