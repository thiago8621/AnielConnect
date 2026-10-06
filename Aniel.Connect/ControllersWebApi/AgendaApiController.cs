using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AgendaApiController : BaseApiController
    {
        
        [HttpPost()]
        [Route("api/Agendaapi/SalvarCompromisso")]
        public async Task<GetApiRetorno<List<Compromisso>>> SalvarCompromisso(List<Compromisso> compromissos)
        {
            GetApiRetorno<List<Compromisso>> retorno = new GetApiRetorno<List<Compromisso>>
            {
                Objeto = new List<Compromisso>()
            };

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Compromisso>(session))
                    {
                        foreach (var compromisso in compromissos.FindAll(p => p.Id == 0))
                            await repository.SaveOrUpdate(compromisso);

                        foreach (var compromisso in compromissos.FindAll(p => p.Id != 0 && p.Deletado == false))
                        {
                            var modelo = (await repository.select(p => p.Id == compromisso.Id)).FirstOrDefault();

                            if (modelo == null)
                            {
                                modelo = new Compromisso();
                            }

                            modelo.UsuarioCriacao = compromisso.UsuarioCriacao;
                            modelo.DataHoraCriacao = compromisso.DataHoraCriacao;
                            modelo.Assunto = compromisso.Assunto;
                            modelo.DataInicioDefault = compromisso.DataInicioDefault;
                            modelo.DataFimDefault = compromisso.DataFimDefault;
                            modelo.HoraInicioDefault = compromisso.HoraInicioDefault;
                            modelo.HoraFimDefault = compromisso.HoraFimDefault;
                            modelo.TipoRecorrencia = compromisso.TipoRecorrencia;
                            modelo.QtdTempoEvento = compromisso.QtdTempoEvento;
                            modelo.IsConcluido = compromisso.IsConcluido;
                            modelo.Id_funcionario = compromisso.Id_funcionario;
                            modelo.Id_Evento = compromisso.Id_Evento;
                            modelo.Local = compromisso.Local;
                            modelo.Observacoes = compromisso.Observacoes;

                            await repository.SaveOrUpdate(modelo);
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime data = compromissos.First().DataInicioDefault;
                        DateTime dataFim = DateTimeUtil.GetLastDayOfMonth(data);
                        DateTime dataInicio = new DateTime(data.Year, data.Month, 1);

                        //StringBuilder sql = new StringBuilder($"SELECT ID, USUARIOCRIACAO, DATAHORACRIACAO, ASSUNTO, DATA_INICIO_DEFAULT, " +
                        //    $"DATA_FIM_DEFAULT, HORA_INICIO_DEFAULT, HORA_FIM_DEFAULT, TIPO_RECORRENCIA, QTD_TEMPO_EVENTO, IS_CONCLUIDO,  " +
                        //    $"ID_FUNCIONARIO, ID_EVENTO, LOCAL, OBSERVACOES FROM TB_COMPROMISSO WHERE DATA_INICIO_DEFAULT BETWEEN '{dataInicio.ToString("yyyy-MM-dd")}' AND '{dataFim.ToString("yyyy-MM-dd")}'  ORDER BY DATA_INICIO_DEFAULT ");

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT ");
                        sql.AppendLine("    ID, USUARIOCRIACAO, DATAHORACRIACAO, ASSUNTO, DATA_INICIO_DEFAULT, ");
                        sql.AppendLine("    DATA_FIM_DEFAULT, HORA_INICIO_DEFAULT, HORA_FIM_DEFAULT, ");
                        sql.AppendLine("    TIPO_RECORRENCIA, QTD_TEMPO_EVENTO, IS_CONCLUIDO,  ID_FUNCIONARIO, ");
                        sql.AppendLine("    ID_EVENTO, LOCAL, OBSERVACOES ");
                        sql.AppendLine(" FROM ");
                        sql.AppendLine("    TB_COMPROMISSO ");
                        sql.AppendLine(" WHERE ");
                        sql.AppendLine($"    DATA_INICIO_DEFAULT BETWEEN '{dataInicio.ToString("yyyy-MM-dd")}' AND '{dataFim.ToString("yyyy-MM-dd")}' ");
                        sql.AppendLine($"    AND ID_FUNCIONARIO = '{compromissos.First().Id_funcionario}' ");
                        sql.AppendLine(" ORDER BY ");
                        sql.AppendLine("    DATA_INICIO_DEFAULT ");


                        List <Compromisso> retornoCompromissos = new List<Compromisso>();

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Compromisso
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    UsuarioCriacao = reader["USUARIOCRIACAO"] == DBNull.Value ? "" : Convert.ToString(reader["USUARIOCRIACAO"]),
                                    DataHoraCriacao = reader["DATAHORACRIACAO"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DATAHORACRIACAO"]),
                                    Assunto = Convert.ToString(reader["ASSUNTO"]),
                                    DataInicioDefault = Convert.ToDateTime(reader["DATA_INICIO_DEFAULT"]),
                                    DataFimDefault = Convert.ToDateTime(reader["DATA_FIM_DEFAULT"]),
                                    HoraInicioDefault = Convert.ToDateTime(reader["HORA_INICIO_DEFAULT"]),
                                    HoraFimDefault = Convert.ToDateTime(reader["HORA_FIM_DEFAULT"]),
                                    TipoRecorrencia = Convert.ToInt16(reader["TIPO_RECORRENCIA"]),
                                    QtdTempoEvento = Convert.ToInt32(reader["QTD_TEMPO_EVENTO"]),
                                    IsConcluido = Convert.ToBoolean(reader["IS_CONCLUIDO"]),
                                    Id_funcionario = Convert.ToInt64(reader["ID_FUNCIONARIO"]),
                                    Id_Evento = Convert.ToInt64(reader["ID_EVENTO"]),
                                    Local = reader["LOCAL"] == DBNull.Value ? "" : Convert.ToString(reader["LOCAL"]),
                                    Observacoes = reader["OBSERVACOES"] == DBNull.Value ? "" : Convert.ToString(reader["OBSERVACOES"])
                                });
                            }
                        }

                        retorno.Ok = true;
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        
        [HttpPost()]
        [Route("api/Agendaapi/ConcluiRestauraCompromisso")]
        public async Task<GetApiRetorno<Boolean>> ConcluiRestauraCompromisso(Compromisso compromisso)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Compromisso>(session))
                    {
                        var modelo = (await repository.select(p => p.Id == compromisso.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Compromisso();
                        }

                        modelo.UsuarioCriacao = compromisso.UsuarioCriacao;
                        modelo.DataHoraCriacao = compromisso.DataHoraCriacao;
                        modelo.Assunto = compromisso.Assunto;
                        modelo.DataInicioDefault = compromisso.DataInicioDefault;
                        modelo.DataFimDefault = compromisso.DataFimDefault;
                        modelo.HoraInicioDefault = compromisso.HoraInicioDefault;
                        modelo.HoraFimDefault = compromisso.HoraFimDefault;
                        modelo.TipoRecorrencia = compromisso.TipoRecorrencia;
                        modelo.QtdTempoEvento = compromisso.QtdTempoEvento;
                        modelo.IsConcluido = compromisso.IsConcluido;
                        modelo.Id_funcionario = compromisso.Id_funcionario;
                        modelo.Id_Evento = compromisso.Id_Evento;
                        modelo.Local = compromisso.Local;
                        modelo.Observacoes = compromisso.Observacoes;

                        await repository.SaveOrUpdate(modelo);

                        retorno.Objeto = true;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        
        [HttpGet()]
        [Route("api/Agendaapi/DeletarCompromisso")]
        public async Task<GetApiRetorno<Boolean>> DeletarCompromisso(Int64? id, Int64 idEvento)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {

                        if (id != null)
                        {
                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_COMPROMISSO WHERE ID = {id} AND ID_EVENTO = {idEvento}");
                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }
                        else
                        {
                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_COMPROMISSO WHERE ID_EVENTO = {idEvento}");
                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }
                        retorno.Ok = true;
                        retorno.Objeto = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Objeto = false;
            }

            return retorno;
        }
        
        [HttpPost()]
        [Route("api/Agendaapi/GetCompromissos")]
        public async Task<GetApiRetorno<List<Compromisso>>> GetCompromissos(RequestCompromissosModel value)
        {
            GetApiRetorno<List<Compromisso>> retorno = new GetApiRetorno<List<Compromisso>>
            {
                Objeto = new List<Compromisso>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();                    
                    using (var conexao = new Conexao<Compromisso>(session))
                    {
                        List<Compromisso> compromissosTmp = new List<Compromisso>();

                        foreach (var compromisso in value.compromissos.FindAll(p => p.Id != 0 && p.Deletado == true))
                        {
                            var modelo = (await conexao.select(p => p.Id == compromisso.Id)).FirstOrDefault();

                            if (modelo != null)
                                await conexao.Remove(await conexao.selectFirst(p => p.Id == compromisso.Id));
                        }                           

                        foreach (var compromisso in value.compromissos.FindAll(p => p.Id == 0))
                            await conexao.SaveOrUpdate(compromisso);
                                                
                        foreach (var compromisso in value.compromissos.FindAll(p => p.Id != 0 && p.Deletado == false))
                        {
                            var modelo = (await conexao.select(p => p.Id == compromisso.Id)).FirstOrDefault();

                            if (modelo == null)
                            {
                                modelo = new Compromisso();
                            }

                            modelo.UsuarioCriacao = compromisso.UsuarioCriacao;
                            modelo.DataHoraCriacao = compromisso.DataHoraCriacao;
                            modelo.Assunto = compromisso.Assunto;
                            modelo.DataInicioDefault = compromisso.DataInicioDefault;
                            modelo.DataFimDefault = compromisso.DataFimDefault;
                            modelo.HoraInicioDefault = compromisso.HoraInicioDefault;
                            modelo.HoraFimDefault = compromisso.HoraFimDefault;
                            modelo.TipoRecorrencia = compromisso.TipoRecorrencia;
                            modelo.QtdTempoEvento = compromisso.QtdTempoEvento;
                            modelo.IsConcluido = compromisso.IsConcluido;
                            modelo.Id_funcionario = compromisso.Id_funcionario;
                            modelo.Id_Evento = compromisso.Id_Evento;
                            modelo.Local = compromisso.Local;
                            modelo.Observacoes = compromisso.Observacoes;

                            await conexao.SaveOrUpdate(modelo);
                        }

                        retorno.Objeto = (await conexao.select(
                                            s => s.Id_funcionario == value.IdFuncionario && 
                                            s.Deletado == false && 
                                            s.DataInicioDefault >= value.dataInicio &&
                                            s.DataInicioDefault <= value.dataFim)
                                            ).ToList();
                    }
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;

                }
            }
            return retorno;
        }

        
        [HttpGet()]
        [Route("api/Agendaapi/BuscarCompromissos")]
        public async Task<GetApiRetorno<List<Compromisso>>> BuscarCompromissos(Int64 id_Funcionario, String assunto)
        {
            GetApiRetorno<List<Compromisso>> retorno = new GetApiRetorno<List<Compromisso>>
            {
                Objeto = new List<Compromisso>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ID, USUARIOCRIACAO, DATAHORACRIACAO, ASSUNTO, DATA_INICIO_DEFAULT, ");
                        sql.AppendLine("DATA_FIM_DEFAULT, HORA_INICIO_DEFAULT, HORA_FIM_DEFAULT, TIPO_RECORRENCIA, QTD_TEMPO_EVENTO, IS_CONCLUIDO,  ");
                        sql.AppendLine("ID_FUNCIONARIO, ID_EVENTO, LOCAL, OBSERVACOES FROM TB_COMPROMISSO WHERE ");
                        sql.AppendLine("ID_FUNCIONARIO = @ID_FUNCIONARIO AND UPPER(ASSUNTO) LIKE UPPER(@ASSUNTO) ");
                        parametros.Add("@ID_FUNCIONARIO",  id_Funcionario);
                        parametros.Add("@ASSUNTO", String.Format("%{0}%", assunto));


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Compromisso
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    DataHoraCriacao = Convert.ToDateTime(reader["DATAHORACRIACAO"]),
                                    Assunto = Convert.ToString(reader["ASSUNTO"]),
                                    DataInicioDefault = Convert.ToDateTime(reader["DATA_INICIO_DEFAULT"]),
                                    DataFimDefault = Convert.ToDateTime(reader["DATA_FIM_DEFAULT"]),
                                    HoraInicioDefault = Convert.ToDateTime(reader["HORA_INICIO_DEFAULT"]),
                                    HoraFimDefault = Convert.ToDateTime(reader["HORA_FIM_DEFAULT"]),
                                    TipoRecorrencia = Convert.ToInt16(reader["TIPO_RECORRENCIA"]),
                                    QtdTempoEvento = Convert.ToInt32(reader["QTD_TEMPO_EVENTO"]),
                                    IsConcluido = Convert.ToBoolean(reader["IS_CONCLUIDO"]),
                                    Id_funcionario = Convert.ToInt64(reader["ID_FUNCIONARIO"]),
                                    Id_Evento = Convert.ToInt64(reader["ID_EVENTO"]),
                                    Local = Convert.ToString(reader["LOCAL"]),
                                    Observacoes = Convert.ToString(reader["OBSERVACOES"])
                                });
                            }
                        }
                    }
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;

                }
            }

            return retorno;
        }
    }
}


