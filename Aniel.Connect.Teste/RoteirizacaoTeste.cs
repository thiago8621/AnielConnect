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
    public class RoteirizacaoTeste
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

        public List<DocumentoRoteirizar> GetDocumentosLatitudeLongitude()
        {
            var documentos = new List<DocumentoRoteirizar>();


            documentos.Add(new DocumentoRoteirizar
            {
                Num_Obra = "Rua 24 de Outubro",
                Equipe = "Caroline",
                Area = "1",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(1, 30, 0),
                DeslocamentoMinutos = 0,
                Latitude_Str = "-22.7087289",
                Longitude_Str = "-43.3108274",
                Num_Doc = 1
            });
            documentos.Add(new DocumentoRoteirizar
            {
                Num_Obra = "Igreja do Pilar",
                Area = "1",
                Equipe = "Caroline",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(1, 30, 0),
                Latitude_Str = "-22.7049845",
                Longitude_Str = "-43.295475",
                Num_Doc = 2
            });
            documentos.Add(new DocumentoRoteirizar
            {
                Num_Obra = "Posto de Saude Pilar",
                Area = "1",
                Equipe = "Caroline",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(1, 30, 0),
                Latitude_Str = "-22.7054836",
                Longitude_Str = "-43.3062246",
                Num_Doc = 3

            });
            documentos.Add(new DocumentoRoteirizar
            {

                Num_Obra = "Rua Tescana - Pilar",
                Area = "1",
                Equipe = "Caroline",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(1, 30, 0),
                Latitude_Str = "-22.7029946",
                Longitude_Str = "-43.2967272",
                Num_Doc = 4

            });

            documentos.Add(new DocumentoRoteirizar
            {

                Num_Obra = "Fabrica Patricia Moraes",
                Area = "1",
                Equipe = "Caroline",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(1, 30, 0),
                Latitude_Str = "-22.7099925",
                Longitude_Str = "-43.3167985",
                Num_Doc = 5
            });

            return documentos;
        }


        [TestMethod]
        public void Passo01_VerificarTecnicoInCapazPorArea()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };
            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1
            };
            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(false, capaz);
        }
        [TestMethod]
        public void Passo02_VerificarTecnicoCapazPorArea()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };
            tecnico.Localidades.Add(new LocalidadeRoteirizacao
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1
            });
            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1
            };
            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(true, capaz);
        }

        [TestMethod]
        public void Passo03_VerificarTecnicoIncapazPorNatureza()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Habilidade = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };

            tecnico.Naturezas.Add(2);

            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Natureza = 1
            };
            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(false, capaz);
        }

        [TestMethod]
        public void Passo04_VerificarTecnicoCapazPorNatureza()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Habilidade = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };

            tecnico.Naturezas.Add(2);

            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Natureza = 2
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(true, capaz);
        }

        [TestMethod]
        public void Passo05_VerificarTecnicoCapazPorAreaTodasLocalidades()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao
            {
                Cod_Escritorio = "1",
                Cod_Area = "1",
                Cod_Localidade = 3,
                Todas_Localidades = true
            });

            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(true, capaz);
        }

        [TestMethod]
        public void Passo06_VerificarTecnicoCapazPorAreaHablidade()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };
            var tecnico = new TecnicoRoteirizar
            {
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao
            {
                Cod_Escritorio = "1",
                Cod_Area = "1",
                Cod_Localidade = 3,
                Todas_Localidades = true
            });

            tecnico.Naturezas.Add(1);

            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 1,
                Cod_Natureza = 1
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var capaz = roteirizacao.VerificarTecnicoCapaz(config, tecnico, doc_Agrupado);

            Assert.AreEqual(true, capaz);
        }

        [TestMethod]
        public void Passo07_FiltrarOrdensTecnicoCapaz()
        {

            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };
            var tecnico = new TecnicoRoteirizar { Localidades = new List<LocalidadeRoteirizacao>(), Naturezas = new List<long?>() };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 3, Todas_Localidades = true });

            tecnico.Naturezas.Add(1);

            var doc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
            {
                Cod_Area = "1",
                Cod_Escritorio = "1",
                Cod_Localidade = 3,
                Cod_Natureza = 1
            };

            var documentos = new List<DocumentoRoteirizar>();
            documentos.Add(new DocumentoRoteirizar { Area = "2", Grupo_Area = "2", Cod_Localidade = 3, Id_Natureza = 5 });
            documentos.Add(new DocumentoRoteirizar { Area = "1", Grupo_Area = "1", Cod_Localidade = 3, Id_Natureza = 1 });

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var documentos_capaz = roteirizacao.FiltrarOrdensTecnicoCapaz(config, doc_Agrupado, tecnico, documentos);

            Assert.AreEqual(0, documentos_capaz.FindAll(p => p.Id_Natureza == 5).Count);
        }

        [TestMethod]
        public void Passo08_Numero_Max_Os_TecnicoDistribuirEquipe()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 3,
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);

            var documentos = new List<DocumentoRoteirizar>();
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1,  Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 2, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 3, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 4, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 5, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 6, Tempo_Execucao = new TimeSpan(0, 30, 0) });

            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var roteirizacao = new RoteirizarDistribuirEquipe(config, diaConf, new List<Turno_ServicoAniel>());
            var resultado = roteirizacao.Run(documentos, fila_tecnicos);
            Assert.AreEqual(true, resultado.documentosRoteirizados.Where(p => p.Equipe == "thaluan").Count() == 3);
        }

        [TestMethod]
        public void Passo09_Numero_Max_Os_TecnicoDistribuirIgual()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 3,
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);

            var documentos = new List<DocumentoRoteirizar>();
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 2, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 3, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 4, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 5, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 6, Tempo_Execucao = new TimeSpan(0, 30, 0) });

            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var roteirizacao = new RoteirizarDistribuirIgual(config, diaConf, new List<Turno_ServicoAniel>());
            var resultado = roteirizacao.Run(documentos, fila_tecnicos);
            Assert.AreEqual(true, resultado.documentosRoteirizados.Where(p => p.Equipe == "thaluan").Count() == 3);
        }

        [TestMethod]
        public void Passo10_PriorizarAgendamentosDistribuirIgual()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 3,
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);

            var documentos = new List<DocumentoRoteirizar>();
            documentos.Add(new DocumentoRoteirizar
            {
                Num_Obra = "123",
                Equipe = "Caroline",
                Area = "1",
                Grupo_Area = "1",
                Cod_Localidade = 1,
                Id_Natureza = 1,
                Tempo_Execucao = new TimeSpan(2, 30, 0),
                Data_hora_ini_agendamento = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy 14:30")),
                Data_hora_fim_agendamento = Convert.ToDateTime(GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy 17:30")),
                DeslocamentoMinutos = 0
            });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Tempo_Execucao = new TimeSpan(2, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Tempo_Execucao = new TimeSpan(2, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Tempo_Execucao = new TimeSpan(2, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Tempo_Execucao = new TimeSpan(2, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Tempo_Execucao = new TimeSpan(2, 30, 0) });

            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var roteirizacao = new RoteirizarDistribuirIgual(config, diaConf, new List<Turno_ServicoAniel>());
            var resultado = roteirizacao.Run(documentos, fila_tecnicos);
            Assert.AreEqual(true, resultado.documentosRoteirizados.Where(p => p.Num_Obra == "123").Count() > 0);
        }

        [TestMethod]
        public void Passo11_ValidaDistribuirEquipeDistanciaUsandoPrimeiroServicoComoReferencia()
        {
            var configComAgrupamento = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true,
                Utiliza_AgrupamentoDistancia = true
            };

            var configSemAgrupamento = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true,
                Utiliza_AgrupamentoDistancia = false
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 3,
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock(),
                Latitude = Convert.ToDouble("-22.7083142".Replace(".", ",")),
                Longitude = Convert.ToDouble("-43.3010689".Replace(".", ",")),

            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);



            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var fila2_tecnicos = new Queue<TecnicoRoteirizar>();
            fila2_tecnicos.Enqueue(tecnico);

            var roteirizacaoComAgrupamento = new RoteirizarDistribuirEquipe(configComAgrupamento, diaConf, new List<Turno_ServicoAniel>());
            var resultadoComAgrupamento = roteirizacaoComAgrupamento.Run(GetDocumentosLatitudeLongitude(), fila_tecnicos);

            var roteirizacaoSemAgrupamento = new RoteirizarDistribuirEquipe(configSemAgrupamento, diaConf, new List<Turno_ServicoAniel>());
            var resultadoSemAgrupamento = roteirizacaoSemAgrupamento.Run(GetDocumentosLatitudeLongitude(), fila2_tecnicos);

            Assert.AreEqual(("Rua Tescana - Pilar", "Fabrica Patricia Moraes"), (resultadoComAgrupamento.documentosRoteirizados.ElementAt(2).Num_Obra, resultadoSemAgrupamento.documentosRoteirizados.ElementAt(2).Num_Obra));
        }

        [TestMethod]
        public void Passo12_ValidaDistribuirEquipeFeriado()
        {
            var configComAgrupamento = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true,
                Utiliza_AgrupamentoDistancia = true
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 3,
                feriado = true
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock(),
                Latitude = Convert.ToDouble("-22.7083142".Replace(".", ",")),
                Longitude = Convert.ToDouble("-43.3010689".Replace(".", ",")),

            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);

            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var roteirizacao = new RoteirizarDistribuirEquipe(configComAgrupamento, diaConf, new List<Turno_ServicoAniel>());
            var resultado = roteirizacao.Run(GetDocumentosLatitudeLongitude(), fila_tecnicos);
            Assert.AreEqual(1, resultado.documentosRoteirizados.Count());
        }

        [TestMethod]
        public void Passo13_ValidaDistanciaMinimaParaDeslocamento()
        {
            var config = new Configuracoes_Gerais
            {
                DistanciaMinimaDeslocamento = 1000,
                TempoBaseDeslocamentoPorKm = Convert.ToDateTime($"{GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy")} 00:02")
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var tempo = roteirizacao.GetTempoDeslocamento(config, 0.5, ReferenciaDistancia.UltimoServico);

            Assert.AreEqual(tempo.TotalMinutes, 0);
        }

        [TestMethod]
        public void Passo14_ValidaTempoDeslocamentoPorKm()
        {
            var config = new Configuracoes_Gerais
            {
                DistanciaMinimaDeslocamento = 1000,
                TempoBaseDeslocamentoPorKm = Convert.ToDateTime($"{GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy")} 00:02")
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var tempo = roteirizacao.GetTempoDeslocamento(config, 2, ReferenciaDistancia.UltimoServico);

            Assert.AreEqual(tempo.TotalMinutes, 4);
        }

        /// <summary>
        /// OBSERVAR QUE OS ATENDIMENTOS SÃO CONTABILIZADOS PELO NUM_DOC DISTINTOS
        /// </summary>
        [TestMethod]
        public void Passo15_Numero_Max_Atendimentos_TecnicoDistribuirEquipe()
        {
            var config = new Configuracoes_Gerais
            {
                Criterio_Area = true,
                Criterio_Habilidade = true
            };

            var diaConf = new RoteirizacaoDiaConf()
            {
                Data = GetNextDayofTheWeek(DayOfWeek.Monday),
                NumeroMaxAtendimentoTecnico = 2,
            };

            var tecnico = new TecnicoRoteirizar
            {
                Equipe = "thaluan",
                Localidades = new List<LocalidadeRoteirizacao>(),
                Naturezas = new List<long?>(),
                JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>(),
                HorarioConnect = GetHorarioMock()
            };

            tecnico.Localidades.Add(new LocalidadeRoteirizacao { Cod_Escritorio = "1", Cod_Area = "1", Cod_Localidade = 1, Todas_Localidades = true });
            tecnico.Naturezas.Add(1);

            var documentos = new List<DocumentoRoteirizar>();
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 1, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 2, Tempo_Execucao = new TimeSpan(0, 30, 0) });
            documentos.Add(new DocumentoRoteirizar { Equipe = "Caroline", Area = "1", Grupo_Area = "1", Cod_Localidade = 1, Id_Natureza = 1, Num_Doc = 3, Tempo_Execucao = new TimeSpan(0, 30, 0) });

            var fila_tecnicos = new Queue<TecnicoRoteirizar>();
            fila_tecnicos.Enqueue(tecnico);

            var roteirizacao = new RoteirizarDistribuirEquipe(config, diaConf, new List<Turno_ServicoAniel>());
            var resultado = roteirizacao.Run(documentos, fila_tecnicos);
            Assert.AreEqual(true, resultado.documentosRoteirizados.Where(p => p.Equipe == "thaluan").Count() == 5);
        }

        [TestMethod]
        public void Passo16_ValidaDistanciaMinimaParaDeslocamentoValorMinimo()
        {
            var config = new Configuracoes_Gerais
            {
                DistanciaMinimaDeslocamento = 1000,
                TempoBaseDeslocamentoPorKm = Convert.ToDateTime($"{GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy")} 00:02")
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var tempo = roteirizacao.GetTempoDeslocamento(config, 1, ReferenciaDistancia.UltimoServico);

            Assert.AreEqual(tempo.TotalMinutes, 2);
        }

        [TestMethod]
        public void Passo17_ValidaDistanciaMinimaParaDeslocamentoAgrupamentoPrimeiroServico()
        {
            var config = new Configuracoes_Gerais
            {
                DistanciaMinimaDeslocamento = 0,
                TempoBaseDeslocamentoPorKm = Convert.ToDateTime($"{GetNextDayofTheWeek(DayOfWeek.Thursday).ToString("dd/MM/yyyy")} 00:02"),
                Utiliza_AgrupamentoDistancia = true,
            };

            var roteirizacao = new RoteirizarDistribuirEquipe(config, new RoteirizacaoDiaConf(), new List<Turno_ServicoAniel>());
            var tempo = roteirizacao.GetTempoDeslocamento(config, 5, ReferenciaDistancia.PrimeiroServico);

            Assert.AreEqual(tempo.TotalMinutes, 0);
        }

    }
}
