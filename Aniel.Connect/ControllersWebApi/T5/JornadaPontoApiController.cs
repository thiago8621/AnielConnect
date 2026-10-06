using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class JornadaPontoApiController : BaseApiController
    {

        public async Task<TimeSpan> GetHorasPrevistasDia(ISession session, Tb_Pt_Jornada_Funcionario jornada, (DateTime Data, Int64? Id_Evento) folha_ponto, Boolean tem_feriado)
        {
            TimeSpan horasPrevistasRetorno = TimeSpan.Zero;

            switch (folha_ponto.Data.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        if (jornada.Opcao_Trabalha_Domingo)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            string duracaoIntervaloDomingo = StringService.HoraParaString(jornada.Duracao_Intervalo_Domingo.GetValueOrDefault(0));
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Domingo.Value,
                                                                          jornada.Termino_Domingo.Value,
                                                                          jornada.Duracao_Intervalo_Domingo.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Domingo.Value,
                                                                          jornada.Termino_Domingo.Value,
                                                                          jornada.Inicio_Intervalo_Domingo.Value,
                                                                          jornada.Termino_Intervalo_Domingo.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Domingo.Value, jornada.Termino_Domingo.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Domingo.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Domingo.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Monday:
                    {
                        if (jornada.Opcao_Trabalha_Segunda)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;

                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Segunda.Value,
                                                                          jornada.Termino_Segunda.Value,
                                                                          jornada.Duracao_Intervalo_Segunda.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Segunda.Value,
                                                                          jornada.Termino_Segunda.Value,
                                                                          jornada.Inicio_Intervalo_Segunda.Value,
                                                                          jornada.Termino_Intervalo_Segunda.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Segunda.Value, jornada.Termino_Segunda.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Segunda.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Segunda.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Tuesday:
                    {
                        if (jornada.Opcao_Trabalha_Terca)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;

                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Terca.Value,
                                                                          jornada.Termino_Terca.Value,
                                                                          jornada.Duracao_Intervalo_Terca.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Terca.Value,
                                                                          jornada.Termino_Terca.Value,
                                                                          jornada.Inicio_Intervalo_Terca.Value,
                                                                          jornada.Termino_Intervalo_Terca.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Terca.Value, jornada.Termino_Terca.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Terca.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Terca.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;


                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Wednesday:
                    {
                        if (jornada.Opcao_Trabalha_Quarta)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;

                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Quarta.Value,
                                                                          jornada.Termino_Quarta.Value,
                                                                          jornada.Duracao_Intervalo_Quarta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Quarta.Value,
                                                                          jornada.Termino_Quarta.Value,
                                                                          jornada.Inicio_Intervalo_Quarta.Value,
                                                                          jornada.Termino_Intervalo_Quarta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Quarta.Value, jornada.Termino_Quarta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Quarta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Quarta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Thursday:
                    {
                        if (jornada.Opcao_Trabalha_Quinta)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Quinta.Value,
                                                                          jornada.Termino_Quinta.Value,
                                                                          jornada.Duracao_Intervalo_Quinta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Quinta.Value,
                                                                          jornada.Termino_Quinta.Value,
                                                                          jornada.Inicio_Intervalo_Quinta.Value,
                                                                          jornada.Termino_Intervalo_Quinta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Quinta.Value, jornada.Termino_Quinta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Quinta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Quinta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Friday:
                    {
                        if (jornada.Opcao_Trabalha_Sexta)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Sexta.Value,
                                                                          jornada.Termino_Sexta.Value,
                                                                          jornada.Duracao_Intervalo_Sexta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Sexta.Value,
                                                                          jornada.Termino_Sexta.Value,
                                                                          jornada.Inicio_Intervalo_Sexta.Value,
                                                                          jornada.Termino_Intervalo_Sexta.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Sexta.Value, jornada.Termino_Sexta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Sexta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Quinta.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
                case DayOfWeek.Saturday:
                    {
                        if (jornada.Opcao_Trabalha_Sabado)
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Duracao_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                            else
                                            {
                                                TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                              jornada.Inicio_Feriado.Value,
                                                                              jornada.Termino_Feriado.Value,
                                                                              jornada.Inicio_Intervalo_Feriado.Value,
                                                                              jornada.Termino_Intervalo_Feriado.Value
                                                                          );

                                                horasPrevistasRetorno = horasPrevistas;
                                            }
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Sabado.Value,
                                                                          jornada.Termino_Sabado.Value,
                                                                          jornada.Duracao_Intervalo_Sabado.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(
                                                                          jornada.Inicio_Sabado.Value,
                                                                          jornada.Termino_Sabado.Value,
                                                                          jornada.Inicio_Intervalo_Sabado.Value,
                                                                          jornada.Termino_Intervalo_Sabado.Value
                                                                      );

                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Inicio_Sabado.Value, jornada.Termino_Sabado.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                            else
                            {
                                if (tem_feriado)
                                {
                                    if (jornada.Opcao_Trabalha_Feriado)
                                    {
                                        if (jornada.Intevalo_Almoco)
                                        {
                                            TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                        else
                                        {
                                            TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Feriado.Value);
                                            horasPrevistasRetorno = horasPrevistas;
                                        }
                                    }
                                }
                                else
                                {
                                    if (jornada.Intevalo_Almoco)
                                    {
                                        TimeSpan horasPrevistas = GetHorasPrevistas(jornada.Duracao_Sabado.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                    else
                                    {
                                        TimeSpan horasPrevistas = TimeSpan.FromHours(jornada.Duracao_Sabado.Value);
                                        horasPrevistasRetorno = horasPrevistas;
                                    }
                                }
                            }
                        }
                        else
                        {
                            horasPrevistasRetorno = TimeSpan.Zero;
                        }

                        if (folha_ponto.Id_Evento.HasValue)
                        {
                            using (var repoEvento = new Conexao<Tb_Pt_Classificacao_Escala>(session))
                            {
                                Tb_Pt_Classificacao_Escala evento = await repoEvento.selectFirst(p => p.Id == folha_ponto.Id_Evento.Value);

                                Boolean gera_movimento = Convert.ToBoolean(evento.Gera_Movimento);
                                var permite_marcar = evento.Permitir_Marcar.HasValue ? evento.Permitir_Marcar.Value : false;

                                if (!(permite_marcar) && !(gera_movimento))
                                {
                                    horasPrevistasRetorno = TimeSpan.Zero;
                                }
                            }
                        }

                        break;
                    }
            }

            return horasPrevistasRetorno;
        }

        public TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino, DateTime inicioIntervalo, DateTime terminoIntervalo)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            if (terminoIntervalo < inicioIntervalo)
                terminoIntervalo = terminoIntervalo.AddDays(1);

            TimeSpan retorno = (termino - inicio) - (terminoIntervalo - inicioIntervalo);

            return retorno;
        }

        public TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino, double duracaoIntervalo)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            TimeSpan retorno = (termino - inicio) - TimeSpan.FromHours(duracaoIntervalo);

            return retorno;
        }

        public TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            return termino - inicio;
        }

        public TimeSpan GetHorasPrevistas(double duracao)
        {
            TimeSpan retorno = TimeSpan.FromHours(duracao);
            return retorno;
        }

        public async Task<Boolean> GetFeriado(String numFre, DateTime data)
        {
            Boolean temFeriado = false;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFeriado = new Conexao<FeriadoAniel>(session))
            using (var repoFuncionario = new Conexao<FuncionarioAniel>(session))
            {
                var funcionario = await repoFuncionario.selectFirst(p => p.Num_Fre == numFre);
                var listFeriado = await repoFeriado.select(p => p.Mes == data.Month && p.Dia == data.Day && p.Status == Status_Default.Ativo);

                if (listFeriado.Count != 0)
                {
                    foreach (var feriado in listFeriado)
                    {
                        if (feriado.Abrangencia == Abrangencia.Estadual)
                        {
                            if (funcionario.Uf_Trabalho == feriado.UF)
                            {
                                temFeriado = true;
                            }
                        }
                        else if (feriado.Abrangencia == Abrangencia.Municipal)
                        {
                            if (funcionario.Uf_Trabalho == feriado.UF &&
                                funcionario.Id_Municipio_Trabalho == feriado.Id_Municipio)
                            {
                                temFeriado = true;
                            }
                        }
                        else
                        {
                            temFeriado = true;
                        }

                        if (temFeriado) break;
                    }
                }
            }

            return temFeriado;
        }
    }
}