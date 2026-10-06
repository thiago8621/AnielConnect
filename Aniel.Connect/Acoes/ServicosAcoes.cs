using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Acoes
{
    public class ServicosAcoes : BaseApiController
    {
        public async Task<PostApiRetorno<Int64>> AlterarDataAgendamento(String codigo, DateTime dataAgendamento)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Documento_ProducaoAniel>(session))
            {
                try
                {
                    var obj = await repository.selectFirst(p => p.CodCt.Equals(codigo));

                    if (dataAgendamento == null)
                    {
                        throw new Exception("Data Agendamento não informada");
                    }

                    obj.DiaHora_Agendamento = dataAgendamento;
                    obj.Data_Maxima = dataAgendamento;

                    await repository.SaveOrUpdate(obj);

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

        public async Task<List<DocumentoProducao_GetDocumento>> ConsultarOsAberto(string cpfCliente)
        {
            List<DocumentoProducao_GetDocumento> retorno = new List<DocumentoProducao_GetDocumento>();

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

                        sql.AppendLine("SELECT A.CODCT, A.PROJETO, A.NUM_DOC, A.NUM_OBRA, A.NUM_DOC, A.NUM_OBRA_ORIGINAL, C.CPF ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                        sql.AppendLine("LEFT OUTER JOIN TB_CLIENTE C ");
                        sql.AppendLine("ON(A.CPF_CLIENTE = C.CPF) ");
                        sql.AppendLine("WHERE STATUS_EXECUCAO != (1,2,3,6)");
                        sql.AppendLine("WHERE CPF_CLIENTE = @VALOR)");
                        parametros.Add("@VALOR", $"{cpfCliente}");

                        using var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null);
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new DocumentoProducao_GetDocumento
                            {
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Num_Doc = Convert.ToInt32(reader["NUM_DOC"]),
                                Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            return retorno;
        }

        public async Task<PostApiRetorno<Documento_ProducaoChaveModel>> CriarOrdemServico(Documento_ProducaoCadastroModel value)
        {
            var param = new ParamWebApi<Documento_ProducaoCadastroModel>
            {
                Objeto = value
            };

            var documentoProducaoApi = new Documento_ProducaoApiController();

            return await documentoProducaoApi.Gravar(param);
        }
    }
}