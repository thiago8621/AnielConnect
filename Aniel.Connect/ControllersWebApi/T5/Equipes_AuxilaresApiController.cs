using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Testing;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerado;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Equipes_AuxilaresApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Equipes_AuxilaresApi/GetEquipes_Auxiliares")]
        public List<EquipeAuxiliaresModel> GetEquipes_Auxiliares(EquipeAuxiliaresModel request)
        {
            using (var session = HibernateUtil.GetSessionT5())
            {
                IEquipeAnielRepository equipeAnielRepository = new EquipeAnielRepository(session);
                return equipeAnielRepository.GetTecnicosAuxiliares(request);
            }
        }

        [HttpPost()]
        [Route("api/Equipes_AuxilaresApi/GetEquipes_AuxiliaresRelatorio")]
        public DataTableResponse<EquipeAuxiliaresModel> GetEquipes_Auxiliares(EquipeAuxiliaresModelRequest request)
        {
            using (var session = HibernateUtil.GetSessionT5())
            {
                IEquipeAnielRepository equipeAnielRepository = new EquipeAnielRepository(session);
                return equipeAnielRepository.GetTecnicosAuxiliaresRelatorio(request);
            }
        }

        [HttpPost()]
        [Route("api/Equipes_AuxilaresApi/GetEquipes_AuxiliaresDataTable")]
        public async Task<DataTableResponse<EquipeAuxiliaresModel>> GetEquipes_AuxiliaresDataTable(EquipeAuxiliaresModelRequest request)
        {
            List<EquipeAuxiliaresModel> retorno = new List<EquipeAuxiliaresModel>();
            IOrderedEnumerable<EquipeAuxiliaresModel> listaOrdenada = null;
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@CODCT", request.CodCt },
                    { "@PROJETO", request.Projeto },
                    { "@NUM_OBRA", request.Num_Obra },
                    { "@NUM_DOC", request.Num_Doc }
                };
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder($@"SELECT A.CODCT, A.PROJETO, C.NUM_OBRA_ORIGINAL AS NUM_OBRA, A.TAREFA, A.EQUIPE, B.NOME, C.NUM_DOC
                                                                FROM TB_MOBILE_PARTICIPANTES_RDO A
                                                               INNER JOIN TB_DOCUMENTO_PRODUCAO C ON C.NUM_OBRA = A.NUM_OBRA 
                                                               LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)
                                                               LEFT JOIN TB_EQUIPE_AUX D ON D.EQUIPE = A.EQUIPE
                                                                WHERE A.CODCT = @CODCT AND A.PROJETO =@PROJETO AND A.NUM_DOC = @NUM_DOC AND A.NUM_OBRA = @NUM_OBRA");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new EquipeAuxiliaresModel
                            {
                                CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                Num_Obra = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty,
                                Tarefa = reader["TAREFA"] != DBNull.Value ? Convert.ToString(reader["TAREFA"]) : String.Empty,
                                Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : String.Empty,
                                Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (int?)null,

                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : String.Empty
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Equipe);

            //Page e retornando DataTable
            return new DataTableResponse<EquipeAuxiliaresModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost]
        [Route("api/Equipes_AuxilaresApi/ExcluirArquivo")]
        public PostApiRetorno<Int64> ExcluirArquivo(EquipeAuxiliaresModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                    var servico = repoDocProd.GetAll().Where(xx => xx.CodCt == request.CodCt &&
                                                                   xx.Projeto == request.Projeto &&
                                                                   xx.Num_Obra == request.Num_Obra &&
                                                                   xx.Num_Doc == request.Num_Doc).FirstOrDefault();

                    if (servico is null)
                    {
                        throw new Exception(RsCadastros.SERVICO_NAO_ENCONTRADO);
                    }


                    IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(session);
                    EquipeAnielBusiness bussEquipe = new EquipeAnielBusiness(repoEquipe, null, null);
                    bussEquipe.RemoverTecnicoAuxiliar(request.Equipe, servico);
                    repoDocProd.InsertLog_Lanc_Servico(null, null, request.CodCt, request.Projeto, request.Num_Obra, request.Num_Doc.Value,
                                                             String.Format("Removido Técnico Auxiliar no serviço"), request.Equipe + "- " + request.Nome, User.Identity.Name, Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
                                                              null,null, null, null);


                    retorno.Ok = true;
                    return retorno;

                }
                catch (Exception e)
                {

                    retorno.Ok = false;
                    retorno.Mensagem = e.Message;
                    return retorno;
                }
            }

        }

        [HttpPost]
        [Route("api/Equipes_AuxilaresApi/InserirRegistro")]
        public PostApiRetorno<Int64> InserirRegistro(EquipeAuxiliaresModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                    var servico = repoDocProd.GetAll().Where(xx => xx.CodCt == request.CodCt &&
                                                                   xx.Projeto == request.Projeto &&
                                                                   xx.Num_Obra == request.Num_Obra &&
                                                                   xx.Num_Doc == request.Num_Doc).FirstOrDefault();

                    if (servico is null)
                    {
                        throw new Exception(RsCadastros.SERVICO_NAO_ENCONTRADO);
                    }
                    if(servico.Equipe==request.Equipe)
                    {
                        throw new Exception("Atenção! Técnico Auxiliar já Adicionado.");
                    }
                    if(!servico.Data_Maxima.HasValue)
                    {
                        throw new Exception("Atenção! Não Adicionado, OS não possui Data de Agendamento.");
                    }
                    if (String.IsNullOrEmpty(servico.Equipe))
                    {
                        throw new Exception("Atenção! Não adicionado, técnico é responsável da OS.");

                    }
                    
                    IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(session);
                    EquipeAnielBusiness bussEquipe = new EquipeAnielBusiness(repoEquipe, null, null);
                    var equipe = GetEquipes_Auxiliares(request).Where(x => x.Equipe == request.Equipe &&
                                                                                  x.Num_Obra == request.Num_Obra &&
                                                                                  x.CodCt == request.CodCt &&
                                                                                  x.Projeto == request.Projeto &&
                                                                                  x.Num_Doc == request.Num_Doc).FirstOrDefault();
                    if (equipe == null)
                    {
                        bussEquipe.AdicionarTecnicoAuxiliar(request.Equipe, servico);
                    }
                    else
                    {
                        throw new Exception("Atenção! Técnico Auxiliar já Adicionado.");
                    }
                    repoDocProd.InsertLog_Lanc_Servico(null, null, request.CodCt, request.Projeto, request.Num_Obra, request.Num_Doc.Value,
                                                             String.Format("Inseriu Técnico Auxiliar  no serviço"), null, User.Identity.Name, Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
                                                              null,  request.Equipe + "- " + request.Nome , null, null);

                    retorno.Ok = true;
                    return retorno;

                }
                catch (Exception e)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = e.Message;
                    return retorno;
                }
            }

        }



    }
}