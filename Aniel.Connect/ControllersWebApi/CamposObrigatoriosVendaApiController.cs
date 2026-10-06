using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
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
    public class CamposObrigatoriosVendaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/CamposObrigatoriosVendaApi/GetAll")]
        public async Task<DataTableResponse<CamposObrigatoriosVendaModels>> GetAll(CamposObrigatoriosDataTableRequest request)
        {

            List<CamposObrigatoriosVendaModels> retorno = new List<CamposObrigatoriosVendaModels>();
            IOrderedEnumerable<CamposObrigatoriosVendaModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CAMPO, A.VISIVEL, ")
                    .AppendLine("A.OBRIGATORIO ")
                    .AppendLine(" FROM TB_CAMPOS_OBRIG_VENDA A");

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
                            var obj = new CamposObrigatoriosVendaModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Campo = (CamposObrigatoriosVendaEnum)Enum.Parse(typeof(CamposObrigatoriosVendaEnum), Convert.ToString(reader["CAMPO"])),
                                Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                            };

                            obj.CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosVendaEnum>(obj.Campo.ToString());

                            retorno.Add(obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.Where(p => !String.IsNullOrEmpty(p.CampoString)).OrderBy(p => p.CampoString);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<CamposObrigatoriosVendaModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/CamposObrigatoriosVendaApi/busca")]
        public async Task<List<CamposObrigatoriosVendaModels>> GetCampos()
        {
            List<CamposObrigatoriosVendaModels> retorno = new List<CamposObrigatoriosVendaModels>();

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
                           .AppendLine("FROM TB_CAMPOS_OBRIG_VENDA A ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new CamposObrigatoriosVendaModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Campo = (CamposObrigatoriosVendaEnum)Enum.Parse(typeof(CamposObrigatoriosVendaEnum), Convert.ToString(reader["CAMPO"])),
                                    CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosVendaEnum>(Convert.ToString(reader["CAMPO"])),
                                    Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                    Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                                });
                            }
                        }
                    }


                    //Inserir Enums Novos
                    foreach (String enumerado in Enum.GetNames(typeof(CamposObrigatoriosVendaEnum)))
                    {
                        var tmp = (CamposObrigatoriosVendaEnum)Enum.Parse(typeof(CamposObrigatoriosVendaEnum), enumerado);
                        if (retorno.Find(p => p.Campo == tmp) == null)
                        {
                            retorno.Add(new CamposObrigatoriosVendaModels
                            {
                                Campo = tmp,
                                Obrigatorio = false,
                                Bloqueado = false,
                                CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosVendaEnum>(tmp)
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
        [Route("api/CamposObrigatoriosVendaApi/GetCamposObrigatoriosInvisivel")]
        public List<CamposObrigatoriosVendaModels> GetCamposObrigatoriosInvisivel()
        {
            List<CamposObrigatoriosVendaModels> retorno = new List<CamposObrigatoriosVendaModels>();

            foreach (var item in GetCampos().Result)
            {
                if (item.Bloqueado == true || item.Obrigatorio == true)
                {
                    retorno.Add(item);
                }
            }

            return retorno;
        }


        public static Boolean CampoBloqueado(List<CamposObrigatoriosVendaModels> campos, CamposObrigatoriosVendaEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo == null)
                return false;
            else
                return _campo.Bloqueado;
        }

        public static Boolean CampoObrigatorio(List<CamposObrigatoriosVendaModels> campos, CamposObrigatoriosVendaEnum campo)
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
        [Route("api/CamposObrigatoriosVendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(CamposObrigatoriosListModel value, ISession session)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (var repository = new Conexao<CamposObrigatoriosVenda>(session))
                {
                    var listaVenda = (await repository.select()).ToList();

                    foreach (var objView in value.Campos_Venda)
                    {
                        var obj = listaVenda.Find(p => p.Campo == objView.Campo);
                        if (obj != null)
                        {
                            obj.Obrigatorio = objView.Obrigatorio;
                            obj.Bloqueado = objView.Bloqueado;
                        }
                        else
                        {
                            obj = new CamposObrigatoriosVenda
                            {
                                Campo = objView.Campo,
                                Bloqueado = objView.Bloqueado,
                                Obrigatorio = objView.Obrigatorio,
                                UsuarioCriacao = User.Identity.GetUserId()
                            };
                        }

                        await repository.SaveOrUpdate(obj);
                    }
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