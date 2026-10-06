using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Util;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class LocalidadeApiController : BaseApiController
    {


        [HttpPost]
        [Route("api/LocalidadeApiController/GetLocalidade")]

        public async Task<GetApiRetorno<List<LocalidadeResponseModel>>> GetLocalidade(LocalidadeRequestModel request)
        {
            GetApiRetorno<List<LocalidadeResponseModel>> retorno = new GetApiRetorno<List<LocalidadeResponseModel>>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {

                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    StringBuilder sql = new StringBuilder();
                    var areaBusca = request.descricaoArea != null ? StringService.RemoverAcentos(request.descricaoArea).ToUpperInvariant() : "";
                    var localidadeBusca = request.descricaoLocalidade != null ? StringService.RemoverAcentos(request.descricaoLocalidade).ToUpperInvariant() : "";
                    var grupoAreaBusca = request.descricaoGrupoArea != null ? StringService.RemoverAcentos(request.descricaoGrupoArea).ToUpperInvariant() : "";
                    List<LocalidadeResponseModel> localidades = new List<LocalidadeResponseModel>();

                    sql.AppendLine("SELECT A.COD_CLIENTE, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE, A.LOCALIDADE, A.CODCT, A.PROJETO, B.DESCRICAO AS DESCRICAO_AREA, C.DESCRICAO AS DESCRICAO_ESCRITORIO ")
                       .AppendLine("FROM TB_LOCALIDADE A ")
                       .AppendLine("INNER JOIN TB_AREA B ")
                       .AppendLine("ON (A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA) ")
                       .AppendLine("INNER JOIN TB_ESCRITORIO C ")
                       .AppendLine("ON (A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");

                    sql.AppendLine(" WHERE A.STATUS = 1");
                    sql.AppendLine(" AND B.STATUS = 1");
                    sql.AppendLine(" AND C.STATUS = 1");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, com.Transaction).Result)
                    {
                        while (_reader.Read())
                        {
                            localidades.Add(new LocalidadeResponseModel()
                            {
                                descricaoLocalidade = Convert.ToString(_reader["LOCALIDADE"]),
                                descricaoArea = Convert.ToString(_reader["DESCRICAO_AREA"]),
                                descricaoGrupoArea = Convert.ToString(_reader["DESCRICAO_ESCRITORIO"]),
                                codArea = Convert.ToString(_reader["COD_AREA"]),
                                codEscritorio = Convert.ToString(_reader["COD_ESCRITORIO"]),
                                codLocalidade = Convert.ToInt32(_reader["COD_LOCALIDADE"]),
                            });
                        }
                    }

                    if (grupoAreaBusca != "")
                        localidades = localidades.FindAll(p => StringService.RemoverAcentos(p.descricaoGrupoArea.ToUpperInvariant().Trim()) == grupoAreaBusca.Trim());
                    if (areaBusca != "")
                        localidades = localidades.FindAll(p => StringService.RemoverAcentos(p.descricaoArea.ToUpperInvariant().Trim()) == areaBusca.Trim());
                    if (localidadeBusca != "")
                        localidades = localidades.FindAll(p => StringService.RemoverAcentos(p.descricaoLocalidade.ToUpperInvariant().Trim()) == localidadeBusca.Trim());

                    retorno.Objeto = localidades;
                    retorno.Ok = true;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = new List<LocalidadeResponseModel>();
                    retorno.Mensagem = String.Format("Erro: {0} \n StackTrace: {1]", ex.Message, ex.StackTrace);
                }


                return retorno;
            }
        }

    }
}