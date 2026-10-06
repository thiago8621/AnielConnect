using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using Resources;
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
    public class CamposObrigatoriosLeadApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/CamposObrigatoriosLeadApi/GetAll")]
        public async Task<DataTableResponse<CamposObrigatoriosLeadModels>> GetAll(CamposObrigatoriosDataTableRequest request)
        {

            List<CamposObrigatoriosLeadModels> retorno = new List<CamposObrigatoriosLeadModels>();
            IOrderedEnumerable<CamposObrigatoriosLeadModels> listaOrdenada = null;
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
                            var obj = new CamposObrigatoriosLeadModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Campo = (CamposObrigatoriosLeadEnum)Enum.Parse(typeof(CamposObrigatoriosLeadEnum), Convert.ToString(reader["CAMPO"])),
                                Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                            };

                            obj.CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosLeadEnum>(obj.Campo.ToString());

                            retorno.Add(obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.Where(p => !String.IsNullOrEmpty(p.CampoString)).OrderBy(p => p.CampoString);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<CamposObrigatoriosLeadModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/CamposObrigatoriosLeadApi/busca")]
        public async Task<List<CamposObrigatoriosLeadModels>> GetCampos()
        {
            List<CamposObrigatoriosLeadModels> retorno = new List<CamposObrigatoriosLeadModels>();

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
                           .AppendLine("FROM TB_CAMPOS_OBRIG_LEAD A ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                string CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosLeadEnum>(Convert.ToString(reader["CAMPO"]));
                                if (CampoString != RsCadastros.STRING_VAZIA)
                                {
                                    retorno.Add(new CamposObrigatoriosLeadModels
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Campo = (CamposObrigatoriosLeadEnum)Enum.Parse(typeof(CamposObrigatoriosLeadEnum), Convert.ToString(reader["CAMPO"])),
                                        CampoString = CampoString,
                                        Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                        Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1
                                    });
                                }

                            }
                        }
                    }


                    //Inserir Enums Novos
                    foreach (String enumerado in Enum.GetNames(typeof(CamposObrigatoriosLeadEnum)))
                    {
                        var tmp = (CamposObrigatoriosLeadEnum)Enum.Parse(typeof(CamposObrigatoriosLeadEnum), enumerado);
                        if (retorno.Find(p => p.Campo == tmp) == null)
                        {
                            retorno.Add(new CamposObrigatoriosLeadModels
                            {
                                Campo = tmp,
                                Obrigatorio = false,
                                Bloqueado = false,
                                CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosLeadEnum>(tmp)
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
        [Route("api/CamposObrigatoriosLeadApi/GetCamposObrigatoriosInvisivel")]
        public List<CamposObrigatoriosLeadModels> GetCamposObrigatoriosInvisivel()
        {
            List<CamposObrigatoriosLeadModels> retorno = new List<CamposObrigatoriosLeadModels>();

            foreach (var item in GetCampos().Result)
            {
                if (item.Bloqueado == true || item.Obrigatorio == true)
                {
                    retorno.Add(item);
                }
            }

            return retorno;
        }


        public static Boolean CampoBloqueado(List<CamposObrigatoriosLeadModels> campos, CamposObrigatoriosLeadEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo == null)
                return false;
            else
                return _campo.Bloqueado;
        }

        public static Boolean CampoObrigatorio(List<CamposObrigatoriosLeadModels> campos, CamposObrigatoriosLeadEnum campo)
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
        [Route("api/CamposObrigatoriosLeadApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(CamposObrigatoriosListModel value, ISession session)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (var repository = new Conexao<CamposObrigatoriosLead>(session))
                {
                    var listaLead = (await repository.select()).ToList();

                    foreach (var objView in value.Campos_Lead)
                    {
                        var obj = listaLead.Find(p => p.Campo == objView.Campo);
                        if (obj != null)
                        {
                            obj.Obrigatorio = objView.Obrigatorio;
                            obj.Bloqueado = objView.Bloqueado;
                        }
                        else
                        {
                            obj = new CamposObrigatoriosLead
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