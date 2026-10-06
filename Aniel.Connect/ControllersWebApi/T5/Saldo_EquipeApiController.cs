using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Saldo_EquipeApiController : BaseApiController
    {
        public async Task<SaldoEquipeModel> LocalizaSaldoEquipePer(ISession session, String TipoSaldoAplic, String CodCt, String Projeto, String Equipe, DateTime Data_Inicial, DateTime Data_Final, String CodMat, String CodCpl)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            SaldoEquipeModel retorno = new SaldoEquipeModel();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"SELECT rEQUIPE, rCODMAT, rCODCPL, rTOTENT, rTOTSAI, rTOTAPLIC, rTOTREMOV, rSALDO
                                 FROM SP_CALCULA_SALDO_EQUIPE_PER (@TIPOSALDO, @CODCT, @PROJETO, @EQUIPE, @DT_INICIAL, @DT_FINAL, @CODMAT, @CODCPL)");

                parametros.Add("@TIPOSALDO", TipoSaldoAplic);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);
                parametros.Add("@DT_INICIAL", Data_Inicial);
                parametros.Add("@DT_FINAL", Data_Final);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return new SaldoEquipeModel
                        {
                            EQUIPE = Convert.ToString(reader["rEQUIPE"]),
                            CODMAT = Convert.ToString(reader["rCODMAT"]),
                            CODCPL = Convert.ToString(reader["rCODCPL"]),
                            TOTENT = Convert.ToDouble(reader["rTOTENT"]),
                            TOTSAI = Convert.ToDouble(reader["rTOTSAI"]),
                            TOTAPLIC = Convert.ToDouble(reader["rTOTAPLIC"]),
                            TOTREMOV = Convert.ToDouble(reader["rTOTREMOV"]),
                            SALDO = Convert.ToDouble(reader["rSALDO"])
                        };
                    }
                    return retorno;
                }
            }
        }

        public async Task<SaldoEquipeModel> LocalizaSaldoEquipe(ISession session, String TipoSaldoAplic, String CodCt, String Projeto, String Equipe, String CodMat, String CodCpl)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            SaldoEquipeModel retorno = new SaldoEquipeModel();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"SELECT rEQUIPE, rCODMAT, rCODCPL, rTOTENT, rTOTSAI, rTOTAPLIC, rTOTREMOV, rSALDO
                                 FROM SP_CALCULA_SALDO_EQUIPE (@TIPOSALDO, @CODCT, @PROJETO, @EQUIPE, @CODMAT, @CODCPL)");

                parametros.Add("@TIPOSALDO", TipoSaldoAplic);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return new SaldoEquipeModel
                        {
                            EQUIPE = Convert.ToString(reader["rEQUIPE"]),
                            CODMAT = Convert.ToString(reader["rCODMAT"]),
                            CODCPL = Convert.ToString(reader["rCODCPL"]),
                            TOTENT = Convert.ToDouble(reader["rTOTENT"]),
                            TOTSAI = Convert.ToDouble(reader["rTOTSAI"]),
                            TOTAPLIC = Convert.ToDouble(reader["rTOTAPLIC"]),
                            TOTREMOV = Convert.ToDouble(reader["rTOTREMOV"]),
                            SALDO = Convert.ToDouble(reader["rSALDO"])
                        };
                    }
                    return retorno;
                }
            }
        }

        public async Task<SaldoEquipeModel> LocalizaSaldoEquipeSaldo(ISession session, String TipoSaldoAplic, String CodCt, String Projeto, String Equipe, String CodMat, String CodCpl,double Saldo)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            SaldoEquipeModel retorno = new SaldoEquipeModel();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"SELECT rEQUIPE, rCODMAT, rCODCPL, rTOTENT, rTOTSAI, rTOTAPLIC, rTOTREMOV, rSALDO
                                 FROM SP_CALCULA_SALDO_EQUIPE (@TIPOSALDO, @CODCT, @PROJETO, @EQUIPE, @CODMAT, @CODCPL)
                                 WHERE rSALDO >= @SALDO");

                parametros.Add("@TIPOSALDO", TipoSaldoAplic);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);
                parametros.Add("@SALDO", Saldo);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return new SaldoEquipeModel
                        {
                            EQUIPE = Convert.ToString(reader["rEQUIPE"]),
                            CODMAT = Convert.ToString(reader["rCODMAT"]),
                            CODCPL = Convert.ToString(reader["rCODCPL"]),
                            TOTENT = Convert.ToDouble(reader["rTOTENT"]),
                            TOTSAI = Convert.ToDouble(reader["rTOTSAI"]),
                            TOTAPLIC = Convert.ToDouble(reader["rTOTAPLIC"]),
                            TOTREMOV = Convert.ToDouble(reader["rTOTREMOV"]),
                            SALDO = Convert.ToDouble(reader["rSALDO"])
                        };
                    }
                    return retorno;
                }
            }
        }


        public async Task<Double> CalcularProgramacaoRmDm(ISession session, String TipoSaldoAplic, String TipoMovto, String CodCt, String Projeto, String Equipe, String CodMat, String CodCpl, String SeqRmDm = "XXXXXX")
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@"Select REQUIPE, RCODMAT,  RCODCPL, RTOTPROG
                                   FROM SP_CALCULA_MATERIAL_PROG_EQUIPE(@TIPOSALDO , @TIPO_MOVTO, @SEQRMDM, @CODCT, @PROJETO, @EQUIPE, @CODMAT, @CODCPL)");

                parametros.Add("@TIPOSALDO", TipoSaldoAplic);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);
                parametros.Add("@SEQRMDM", SeqRmDm);
                parametros.Add("@TIPO_MOVTO", TipoMovto); //R OU D

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["RTOTPROG"] != DBNull.Value ? Convert.ToDouble(reader["RTOTPROG"]) : 0;
                    }
                    return 0;
                }
            }
        }

    }
}