using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using NHibernate.Criterion;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class MotivoAusenciaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/MotivoAusenciaapi/GetAll")]
        public async Task<DataTableResponse<MotivoAusencia_ListModel>> GetAll(MotivoAusencia_DataTableRequest request)
        {

            List<MotivoAusencia_ListModel> retorno = new List<MotivoAusencia_ListModel>();
            IOrderedEnumerable<MotivoAusencia_ListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MOTIVO_AUSENCIA"); //WHERE STATUS = 1  

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

                    sql.AppendLine("SELECT A.ID, A.MOTIVO, A.STATUS");
                    sql.AppendLine(" FROM TB_MOTIVO_AUSENCIA A");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sql.AppendLine(" WHERE A.STATUS = @STATUS ");

                        parametros.Add("@STATUS", request.Status);

                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        if (!String.IsNullOrEmpty(request.Status)) {
                            sql.AppendLine(" AND ((UPPER(A.MOTIVO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));

                        }
                        else
                        {
                            sql.AppendLine(" WHERE ((UPPER(A.MOTIVO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        }

                    }
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new MotivoAusencia_ListModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<MotivoAusencia_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        private MotivoAusencia_CadastroModel GetViewObj(Motivo_Ausencia obj)
        {
            return new MotivoAusencia_CadastroModel
            {
                Id = obj.Id,
                Motivo = obj.Motivo,
                Status  = obj.Status == Status_Default.Ativo ? true : false,

            };

        }


        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>> retorno = new GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Motivo_Ausencia>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = (lista.Select(p => new MotivoAusencia_ListModel
                    {
                        Id = p.Id,
                        Motivo = $"{p.Motivo}"

                    }));

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

       

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>>> GetListMotivos()
        {
            GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>> retorno = new GetApiRetorno<IEnumerable<MotivoAusencia_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Motivo_Ausencia>(session))
            {
                try
                {
                    var lista = await repository.select();

                    retorno.Objeto = (lista.Select(p => new MotivoAusencia_ListModel
                    {
                        Id = p.Id,
                        Motivo = $"{p.Motivo}"
                        
                    }));

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

      
        [HttpPost()]
        [Route("api/MotivoAusenciaapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_AUSENCIA SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpGet()]
        [Route("api/MotivoAusenciaapi/FindById")]
        public async Task<GetApiRetorno<MotivoAusencia_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<MotivoAusencia_CadastroModel> retorno = new GetApiRetorno<MotivoAusencia_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Motivo_Ausencia>(session))
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
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/MotivoAusenciaapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MOTIVO_AUSENCIA WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/MotivoAusenciaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<MotivoAusencia_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Ausencia modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Motivo_Ausencia>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        //bool padrao = value.Objeto.Padrao == "Sim" ? true : false;

                        if (string.IsNullOrEmpty(value.Objeto.Motivo))
                        {
                            if (string.IsNullOrEmpty(value.Objeto.Motivo) == true || value.Objeto.Id == 0)
                            {
                                var ExisteMotivoIgual = await repository.select(p => p.Motivo == value.Objeto.Motivo && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo);

                                if (ExisteMotivoIgual != null)
                                {
                                    if (ExisteMotivoIgual.Count() > 0)
                                        throw new Exception("Motivo já cadastrado.");
                                }
                            }
                        }
                     
                        if (modelo == null)
                        {
                            modelo = new Motivo_Ausencia
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario,
                                Motivo = value.Objeto.Motivo
                            };
                        }
                        else
                        {
                            var Statusreal = Status_Default.Ativo;
                            if (!value.Objeto.Status)
                            {
                                Statusreal = Status_Default.Inativo;
                            }
                            modelo.Status = Statusreal;
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Motivo = value.Objeto.Motivo;
                        }

                        await repository.SaveOrUpdate(modelo);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
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