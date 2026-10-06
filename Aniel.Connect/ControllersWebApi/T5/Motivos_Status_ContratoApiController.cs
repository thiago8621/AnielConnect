using Aniel.Connect.Core;
using Aniel.Connect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Aniel.Connect.Models.Tipo_AtendimentoCadastroModel;
using Aniel.Connect.Models;

using Sinapse.DAO.repository;
using NHibernate;
using System.Text;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class Motivos_Status_ContratoApiController : BaseApiController
    {[HttpPost()]
        [Route("api/Motivos_Status_ContratoApi/GetAll")]
      public async Task<DataTableResponse<Tipo_AtendimentoCadastroModel>> GetAll(Tipo_Atendimento_DataTableRequest request)
        {
            List<Tipo_AtendimentoCadastroModel> rertono = new List<Tipo_AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5()) {
                //Filtros 
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                Int64 total_count = 0;

                //Count
                using (var com = session.Connection.CreateCommand())
                {

                  
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_ENCERRAMENTO) AS CO FROM TB_CODIGO_ENCERRAMENTO A ");
                  

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                           total_count = Convert.ToInt32(reader["CO"]);
                    }
                }
                //Consulta
                using (var com = session.Connection.CreateCommand())
                {

                  
                    StringBuilder sql = new StringBuilder("SELECT A.COD_ENCERRAMENTO,A.DESCRICAO " +
                                                          "FROM TB_CODIGO_ENCERRAMENTO A ");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync()) {
                            new Tipo_AtendimentoCadastroModel
                            {
                                Cod_Encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                            };


                        }
                           
                    }
                }


            }



            return new DataTableResponse<Tipo_AtendimentoCadastroModel>
            {
               
                    data = rertono
                };

        }
    }
}