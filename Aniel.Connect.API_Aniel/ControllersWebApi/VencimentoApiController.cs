using Aniel.Connect.API_Aniel.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class VencimentoApiController : ApiController
    {
        [HttpPost]
        [Route("api/FormaPagamentoApi/GetVencimentos")]
        public async Task<List<Forma_PagamentoVencimentoCadastroModel>> GetVencimentos(FormaPagamentoVencimentoApiRequest request)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();


            var retorno = new List<Forma_PagamentoVencimentoCadastroModel>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT A.ID, A.ID_VENCIMENTO, B.DIA_VENCIMENTO ")
                        .AppendLine("FROM TB_FORMA_PAGAMENTO_VENCIMENTO A ")
                        .AppendLine("INNER JOIN TB_VENCIMENTO B ")
                        .AppendLine("ON (A.ID_VENCIMENTO = B.ID) ")
                        .AppendLine("WHERE A.ID_FORMA_PAGAMENTO = @ID_FORMA_PAGAMENTO ");
                    parametros.Add("@ID_FORMA_PAGAMENTO", request.FormaPagamento);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Forma_PagamentoVencimentoCadastroModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                IdVencimento = reader["ID_VENCIMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENCIMENTO"]) : 0,
                                DiaVencimento = reader["DIA_VENCIMENTO"] != DBNull.Value ? Convert.ToInt16(reader["DIA_VENCIMENTO"]) : Convert.ToInt16(0)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }
            return retorno;
        }

    }
    public class FormaPagamentoVencimentoApiRequest : ApiRequest
    {
        [Required]
        public long FormaPagamento { get; set; }
    }
}