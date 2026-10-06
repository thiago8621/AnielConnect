using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class Configuracao_Servico_OLTApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Configuracao_Servico_OLTapi/GetAll")]
        public async Task<DataTableResponse<Configuracao_Servico_OLTModel>> GetAll(Configuracao_Servico_OLT_DataTableRequest request)
        {
            List<Configuracao_Servico_OLTModel> retorno = new List<Configuracao_Servico_OLTModel>();
            IOrderedEnumerable<Configuracao_Servico_OLTModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_CONFIG_SERVICO_OLT A ");                

                if (request.Status.HasValue)
                    sqlFrom.AppendLine($" WHERE A.STATUS = {request.Status} ");
                else
                    sqlFrom.AppendLine($" WHERE A.STATUS IN (0,1) ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "descricao")
                    {
                        sqlFrom.AppendLine(" AND (A.DESCRICAO LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }                   
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.MARCA_MODELO, A.PERIODICIDADE_EXECUCAO, A.STATUS, A.DATA_ULTIMA_EXECUCAO ");
                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Configuracao_Servico_OLTModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Desc_Marca_Modelo_OLT = HtmlEnumExtensions.GetEnumDescription<Marca_Modelo_OLT>(Convert.ToString(reader["MARCA_MODELO"])),
                                Marca_Modelo_OLT = (Marca_Modelo_OLT)Enum.Parse(typeof(Marca_Modelo_OLT), Convert.ToString(reader["MARCA_MODELO"])),
                                Desc_Periodicidade_Execucao = reader["PERIODICIDADE_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Periodicidade_ExecucaoEnum>(Convert.ToString(reader["PERIODICIDADE_EXECUCAO"])) : null,
                                Periodicidade_Execucao = reader["PERIODICIDADE_EXECUCAO"] != DBNull.Value ?  (Periodicidade_ExecucaoEnum)Enum.Parse(typeof(Periodicidade_ExecucaoEnum), Convert.ToString(reader["PERIODICIDADE_EXECUCAO"])) : (Periodicidade_ExecucaoEnum?)null,
                                Desc_Status = Convert.ToBoolean(reader["STATUS"]) == true ? Convert.ToString(RsCadastros.ATIVO) : Convert.ToString(RsCadastros.INATIVO),       
                                Data_Ultima_Execucao = reader["DATA_ULTIMA_EXECUCAO"] != DBNull.Value ? DateTime.Parse(Convert.ToString(reader["DATA_ULTIMA_EXECUCAO"])).ToString("dd/MM/yyyy HH:mm:ss") : "-"
                            });
                        }
                    }
                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Configuracao_Servico_OLTModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpGet]
        [Route("api/Configuracao_Servico_OLTApi/FindById")]
        public async Task<GetApiRetorno<Configuracao_Servico_OLTModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Configuracao_Servico_OLTModel> retorno = new GetApiRetorno<Configuracao_Servico_OLTModel>();


            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Configuracao_Servico_OLT>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }

                return retorno;
            }
        }

        [HttpPost]
        [Route("api/Configuracao_Servico_OLTApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Configuracao_Servico_OLTModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Configuracao_Servico_OLT modelo = null;
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Configuracao_Servico_OLT>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            if ((await repository.select(p => p.Marca_Modelo_OLT == value.Objeto.Marca_Modelo_OLT)).FirstOrDefault() != null)
                                throw new Exception("Já existe uma configuração cadastrada para o modelo de OLT informado.");

                            modelo = new Configuracao_Servico_OLT
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario,
                                DataHoraCriacao = DateTime.Now,
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.DataHoraAlteracao = DateTime.Now;

                        }

                        BindViewToObj(modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Configuracao_Servico_OLTApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {

            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CONFIG_SERVICO_OLT SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        private void BindViewToObj(Configuracao_Servico_OLT obj, Configuracao_Servico_OLTModel model)
        {
            obj.Id = model.Id;
            obj.Marca_Modelo_OLT = model.Marca_Modelo_OLT;
            obj.Periodicidade_Execucao = model.Periodicidade_Execucao;          
            obj.Hora_Execucao = !String.IsNullOrEmpty(model.Hora_Execucao) ? DateTime.Parse(model.Hora_Execucao) : (DateTime?)null;
            obj.Descricao = model.Descricao;
            obj.Extrair_Diagnostico_Onu = model.Extrair_Diagnostico_Onu;
            obj.Consultar_Status_Onu = model.Consultar_Status_Onu;
            obj.Minutos_Execucao_Status_Onu = model.Minutos_Execucao_Status_Onu;
        }

        private Configuracao_Servico_OLTModel GetViewObj(Configuracao_Servico_OLT obj)
        {
            Configuracao_Servico_OLTModel retorno = new Configuracao_Servico_OLTModel();

            retorno.Id = obj.Id;
            retorno.Descricao = obj.Descricao;
            retorno.Marca_Modelo_OLT = obj.Marca_Modelo_OLT;
            retorno.Status = obj.Status == Status_Default.Ativo;
            retorno.Hora_Execucao = obj.Hora_Execucao?.ToString("HH:mm");
            retorno.Extrair_Diagnostico_Onu = obj.Extrair_Diagnostico_Onu;
            retorno.Consultar_Status_Onu = obj.Consultar_Status_Onu;
            retorno.Minutos_Execucao_Status_Onu = obj.Minutos_Execucao_Status_Onu;
            retorno.Periodicidade_Execucao = obj.Periodicidade_Execucao;

            return retorno;
        }
    }
}