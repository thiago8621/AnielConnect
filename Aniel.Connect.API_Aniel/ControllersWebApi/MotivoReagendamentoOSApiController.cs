using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Sinapse.Model.Identity;
using Sinapse.Model;
using Microsoft.AspNet.Identity;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.Repository;
using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using OpenXmlPowerTools;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core.Service;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MySqlX.XDevAPI;
using Aniel.Connect.Repository.Model;
using Resources;
using System.Diagnostics.Contracts;
using System.Net;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Repository.PesquisaSatisfacaoRepository;
using System.Web.WebSockets;
using Aniel.Connect.Repository.ConfigUrl;
using Swashbuckle.Swagger;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class MotivoReagendamentoOSApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/MotivoReagendamentoOS/ConsultarMotivo")]
        public async Task<DataTableResponse<ConsultaMotivoReagendamento>> ConsultarMotivo(ConsultaMotivoReagendamentoRequest request)
        {
            List<ConsultaMotivoReagendamento> retorno = new List<ConsultaMotivoReagendamento>();

            try
            {
                Int32 recordsTotal = 0;
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }


                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MOTIVOS_REAGENDAMENTOS A ");

                    ////Filtrando
                    List<String> condicoes = new List<String>();


                    if (!String.IsNullOrEmpty(request.Descricao))
                    {
                        condicoes.Add("( A.Descricao = @Descricao)");
                        parametros.Add("@Descricao", Convert.ToString(request.Descricao));
                    }

                    if (request.Status!=0 && request.Status != null)
                    {
                        condicoes.Add("( A.Status = @Status)");
                        parametros.Add("@Status", Convert.ToString(request.Status));
                    }

                    if (request.Cod_Motivo_Reagendamentos!= null)
                    {
                        condicoes.Add("( A.Cod_Motivo_Reagendamentos = @Cod_Motivo_Reagendamentos)");
                        parametros.Add("@Cod_Motivo_Reagendamentos", Convert.ToString(request.Cod_Motivo_Reagendamentos));
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.Descricao,A.Cod_Motivo_Reagendamentos,A.Status");
                        sql.AppendLine(sqlFrom.ToString());



                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ConsultaMotivoReagendamento
                                {
                                    Descricao = Convert.ToString(reader["Descricao"]),
                                    Cod_Motivo_Reagendamentos = Convert.ToInt32(reader["Cod_Motivo_Reagendamentos"]),
                                    Status = Convert.ToInt32(reader["Status"]),

                                });
                            }
                        }

                    }
                }

                //Page e retornando DataTable
                return new DataTableResponse<ConsultaMotivoReagendamento>
                {
                    //draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new DataTableResponse<ConsultaMotivoReagendamento>
                {
                    error = ex.Message
                };
            }
        }


    }
}
