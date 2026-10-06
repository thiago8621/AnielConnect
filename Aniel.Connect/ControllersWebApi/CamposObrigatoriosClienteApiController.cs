using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
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
    public class CamposObrigatoriosClienteApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/CamposObrigatoriosClienteApi/GetAll")]
        public async Task<DataTableResponse<CamposObrigatoriosClienteModels>> GetAll(CamposObrigatoriosDataTableRequest request)
        {

            List<CamposObrigatoriosClienteModels> retorno = new List<CamposObrigatoriosClienteModels>();
            IOrderedEnumerable<CamposObrigatoriosClienteModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CAMPO, A.VISIVEL, ")
                    .AppendLine("A.OBRIGATORIO ")
                    .AppendLine(" FROM TB_CAMPOS_OBRIG_CLIENTE A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        //sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        //parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new CamposObrigatoriosClienteModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Campo = (CamposObrigatoriosClienteEnum)Enum.Parse(typeof(CamposObrigatoriosClienteEnum), Convert.ToString(reader["CAMPO"])),
                                Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                            };

                            obj.CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosClienteEnum>(obj.Campo.ToString());

                            retorno.Add(obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.Where(p => !String.IsNullOrEmpty(p.CampoString)).OrderBy(p => p.CampoString);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<CamposObrigatoriosClienteModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/CamposObrigatoriosClienteApi/busca")]
        public async Task<List<CamposObrigatoriosClienteModels>> GetCampos()
        {
            List<CamposObrigatoriosClienteModels> retorno = new List<CamposObrigatoriosClienteModels>();

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

                        sql.AppendLine("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CAMPO, A.VISIVEL,")
                           .AppendLine("A.OBRIGATORIO")
                           .AppendLine("FROM TB_CAMPOS_OBRIG_CLIENTE A ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new CamposObrigatoriosClienteModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Campo = (CamposObrigatoriosClienteEnum)Enum.Parse(typeof(CamposObrigatoriosClienteEnum), Convert.ToString(reader["CAMPO"])),
                                    CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosClienteEnum>(Convert.ToString(reader["CAMPO"])),
                                    Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                    Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                                });
                            }
                        }
                    }


                    //Inserir Enums Novos
                    foreach (String enumerado in Enum.GetNames(typeof(CamposObrigatoriosClienteEnum)))
                    {
                        var tmp = (CamposObrigatoriosClienteEnum)Enum.Parse(typeof(CamposObrigatoriosClienteEnum), enumerado);
                        if (retorno.Find(p => p.Campo == tmp) == null)
                        {
                            retorno.Add(new CamposObrigatoriosClienteModels
                            {
                                Campo = tmp,
                                Obrigatorio = false,
                                Bloqueado = false,
                                CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosClienteEnum>(tmp)
                            });
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        
        [HttpGet()]
        [Route("api/CamposObrigatoriosClienteApi/GetCamposObrigatoriosInvisivel")]
        public List<CamposObrigatoriosClienteModels> GetCamposObrigatoriosInvisivel()
        {
            List<CamposObrigatoriosClienteModels> retorno = new List<CamposObrigatoriosClienteModels>();

            foreach (var item in GetCampos().Result)
            {
                if (item.Bloqueado == true || item.Obrigatorio == true)
                {
                    retorno.Add(item);
                }
            }

            return retorno;
        }


        public static Boolean CampoBloqueado(List<CamposObrigatoriosClienteModels> campos, CamposObrigatoriosClienteEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo == null)
                return false;
            else
                return _campo.Bloqueado;
        }

        public static Boolean CampoObrigatorio(List<CamposObrigatoriosClienteModels> campos, CamposObrigatoriosClienteEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo != null)
            {
                if (_campo.Obrigatorio == false)
                    return false;
                else
                    return true;
            }
            return false;
        }

        [HttpPost()]
        [Route("api/CamposObrigatoriosClienteApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(CamposObrigatoriosListModel value, ISession session)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (var repository = new Conexao<CamposObrigatoriosCliente>(session))
                {
                    var listaCliente = (await repository.select()).ToList();

                    foreach (var obj in listaCliente)
                    {
                        var objView = value.Campos_Cliente.Find(p => p.Id == obj.Id);
                        if (objView != null)
                        {
                            obj.Obrigatorio = objView.Obrigatorio;
                            obj.Bloqueado = objView.Bloqueado;
                        }
                    }

                    await repository.SaveOrUpdate(listaCliente);
                }

                retorno.IdObjeto = 1;
                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

    }
}