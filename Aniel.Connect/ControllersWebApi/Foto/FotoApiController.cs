using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class FotoApiController : BaseApiController
    {


        //[HttpGet()]
        //[Route("api/Fotoapi/GetList")]
        //public async Task<GetApiRetorno<List<ObjBusca<String>>>> GetList(String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc)
        //{
        //    GetApiRetorno<List<ObjBusca<String>>> retorno = new GetApiRetorno<List<ObjBusca<String>>>();
        //    retorno.Objeto = new List<ObjBusca<String>>();

        //    using (ISession session = HibernateUtil.GetSessionFoto())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder();

        //                sql.AppendLine("SELECT ID, CHAVE, DESCRICAO, TIPO, ARQUIVO, USUARIO, DATA, HORA ");
        //                sql.AppendLine("FROM TB_ARQUIVO_DOCPROD ");
        //                sql.AppendLine("WHERE A.CHAVE = @VALOR");

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        retorno.Objeto.Add(new ObjBusca<String>
        //                        {
        //                            Value = Convert.ToString(reader["COD_AREA"]),
        //                            Label = Convert.ToString(reader["DESCRICAO"])
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retorno;

        //}



    }
}