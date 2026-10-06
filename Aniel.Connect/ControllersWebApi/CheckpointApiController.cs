using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class CheckpointApiController : BaseApiController
    {
        
        [HttpGet()]
        [Route("api/CheckpointApi/GetCheckpoint")]
        public async Task<GetApiRetorno<List<Checkpoint_Vendedor_Mobile>>> GetCheckpoint(Int64 id_vendedor, string data_hora)
        {
            GetApiRetorno<List<Checkpoint_Vendedor_Mobile>> retorno = new GetApiRetorno<List<Checkpoint_Vendedor_Mobile>>();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT C.ID,C.ID_VENDEDOR, C.ID_TIPO, ");
                        sql.Append("C.USUARIOCRIACAO,C.DATA_HORA,C.LONGITUDE,C.LATITUDE, ");
                        sql.Append("C.ENDERECO,C.OBSERVACAO,C.PRECISAO, C.BATERIA, C.STATUS_GPS,C.DISPOSITIVO, ");
                        sql.Append("C.VERSAO_ANDROID,C.VERSAO_APP ");
                        sql.Append("FROM TB_CHECKPOINT_VENDEDOR AS C ");
                        sql.Append("WHERE ID_VENDEDOR = @ID_VENDEDOR AND CAST(DATA_HORA as DATE) = @DATA_HORA;");
                        parametros.Add("@ID_VENDEDOR", id_vendedor);
                        parametros.Add("@DATA_HORA", DateTime.Parse(data_hora, new CultureInfo("pt-BR")).ToString("yyyy-MM-dd"));
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            var tempRetorno = new List<Checkpoint_Vendedor_Mobile>();
                            while (await reader.ReadAsync())
                            {

                                tempRetorno.Add(new Checkpoint_Vendedor_Mobile()
                                {
                                    id = Convert.ToInt64(reader["ID"]),
                                    id_vendedor = Convert.ToInt64(reader["ID_VENDEDOR"]),
                                    id_tipo = reader["ID_TIPO"] != null ? Convert.ToInt64(reader["ID_TIPO"]) : (Int64?)null,
                                    usuario_criacao = reader["USUARIOCRIACAO"].ToString(),
                                    data_hora = reader["DATA_HORA"] != null ? Convert.ToDateTime(reader["DATA_HORA"]) : (DateTime?)null,
                                    longitude = reader["LONGITUDE"] != null ? Convert.ToDouble(reader["LONGITUDE"]) : (Double?)null,
                                    latitude = reader["LATITUDE"] != null ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null,
                                    endereco = reader["ENDERECO"].ToString(),
                                    observacao = reader["OBSERVACAO"].ToString(),
                                    precisao = reader["PRECISAO"] != null ? Convert.ToDouble(reader["PRECISAO"]) : (Double?)null,
                                    bateria = reader["BATERIA"] != null ? Convert.ToInt64(reader["BATERIA"]) : (Int64?)null,
                                    status_gps = reader["STATUS_GPS"].ToString(),
                                    dispositivo = reader["DISPOSITIVO"].ToString(),
                                    versao_android = reader["VERSAO_ANDROID"].ToString(),
                                    versao_app = reader["VERSAO_APP"].ToString()
                                });
                            }



                            retorno.Ok = true;
                            retorno.Mensagem = "Carregamento feito com sucesso!";
                            retorno.Objeto = tempRetorno;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.StackTrace;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/CheckpointApi/GetTipoCheckpointAll")]
        public async Task<GetApiRetorno<List<Tipo_Checkpoint_Vendedor>>> GetTipoCheckpointAll()
        {
            GetApiRetorno<List<Tipo_Checkpoint_Vendedor>> retorno = new GetApiRetorno<List<Tipo_Checkpoint_Vendedor>>();
            using (var session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        //Filtros
                        Dictionary<String, object> parametros = new Dictionary<String, object>();
                        //Consulta

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ID,DESCRICAO FROM TB_TIPO_CHECKPOINT_VENDEDOR");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            var tempRetorno = new List<Tipo_Checkpoint_Vendedor>();
                            while (await reader.ReadAsync())
                            {
                                tempRetorno.Add(new Tipo_Checkpoint_Vendedor()
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = reader["DESCRICAO"].ToString()
                                });
                            }

                            retorno.Objeto = tempRetorno;
                        }
                    }
                    retorno.Ok = true;
                    retorno.Mensagem = "Carregamento feito com sucesso!";
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.StackTrace;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/CheckpointApi/GetAllAtivos")]
        public async Task<GetApiRetorno<List<Tipo_Checkpoint_Vendedor>>> GetAllAtivos()
        {
            GetApiRetorno<List<Tipo_Checkpoint_Vendedor>> retorno = new GetApiRetorno<List<Tipo_Checkpoint_Vendedor>>();
            using (var session = HibernateUtil.GetSession())
            {
                try
                {
                    retorno.Objeto = (await new Conexao<Tipo_Checkpoint_Vendedor>(session).select(s => s.Status == Status_Default.Ativo)).Select(s => new Tipo_Checkpoint_Vendedor()
                    {
                        Id = s.Id,
                        Descricao = s.Descricao,
                        Status = s.Status
                    }).ToList();
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.StackTrace;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/CheckpointApi/SalvarCheckpoint")]
        public async Task<PostApiRetorno> SalvarCheckpoint(Checkpoint_Vendedor_Mobile value)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            Checkpoint_Vendedor modelo = null;
            Funcionario funcionario = null;
            Tipo_Checkpoint_Vendedor tipo_Checkpoint = null;
            using (var session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var repository_checkpoint = new Conexao<Checkpoint_Vendedor>(session))
                    {
                        using (var repositorio_funcionario = new Conexao<Funcionario>(session))
                        {
                            using (var repositorio_tipo_checkpoint = new Conexao<Tipo_Checkpoint_Vendedor>(session))
                            {
                                funcionario = (await repositorio_funcionario.select(p => p.Id == value.id_vendedor)).FirstOrDefault();
                                if (funcionario != null)
                                {
                                    tipo_Checkpoint = (await repositorio_tipo_checkpoint.select(p => p.Id == value.id_tipo)).FirstOrDefault();
                                    if (tipo_Checkpoint != null)
                                    {
                                        modelo = new Checkpoint_Vendedor()
                                        {
                                            Atualizado_Em = DateTime.Now,
                                            Bateria = value.bateria,
                                            DataHoraAlteracao = DateTime.Now,
                                            DataHoraCriacao = DateTime.Now,
                                            Data_Hora = value.data_hora,
                                            Dispositivo = value.dispositivo,
                                            Endereco = value.endereco,
                                            Latitude = value.latitude,
                                            Longitude = value.longitude,
                                            Observacao = value.observacao,
                                            Precisao = value.precisao,
                                            Status_Gps = value.status_gps,
                                            Tipo = tipo_Checkpoint,
                                            UsuarioAlteracao = funcionario.Nome,
                                            UsuarioCriacao = funcionario.Nome,
                                            Vendedor = funcionario,
                                            Versao_Android = value.versao_android,
                                            Versao_App = value.versao_app,

                                        };

                                        await repository_checkpoint.Save(modelo);

                                        retorno.Ok = true;
                                        retorno.Mensagem = "Gravado com sucesso !";
                                    }
                                    else
                                    {
                                        retorno.Ok = false;
                                        retorno.Mensagem = "Tipo Checkpoint não informado";
                                    }
                                }
                                else
                                {
                                    retorno.Ok = false;
                                    retorno.Mensagem = "Funcionario não Encontrado";
                                }
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.StackTrace;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }




    }
}