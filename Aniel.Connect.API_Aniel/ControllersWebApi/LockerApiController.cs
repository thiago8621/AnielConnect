using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class LockerApiController : BaseApiController
    {
        private object reader;

        [HttpPost]
        [Route("api/LockerApi/AtualizaLockerRetiradaAniel")]

        public async Task<GetApiRetorno<Boolean>> AtualizaLockerRetiradaAniel (Locker_Retirada_Models lockerRetirada)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    bool _existeAlgumaBox = false;
                    bool _existeBox = false;
                    string _box = null;
                    Int32 _idLocker = 0;
                    string _equipe = "-";
                    string _seqRmDm = "-";
                    string _codCt = "-";
                    string _projeto = "-";

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT ID_LOCKER,STATUS_BOX_LOCKER,BOX,EQUIPE,SEQRMDM,CODCT,PROJETO ");
                    sql.AppendLine("FROM TB_SEQUENCIA_RMDM ");
                    sql.AppendLine("WHERE SEQ_LOCKER = @SEQ_LOCKER ");
                    sql.AppendLine("AND ID_LOCKER IS NOT NULL ");
                    parametros.Add("@SEQ_LOCKER", lockerRetirada.Numero);
                    using (var com = sessionT5.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            _box = Convert.ToString(reader["BOX"]); 
                            _idLocker = Convert.ToInt32(reader["ID_LOCKER"]);
                            _equipe = Convert.ToString(reader["EQUIPE"]);
                            _seqRmDm = Convert.ToString(reader["SEQRMDM"]);
                            _codCt = Convert.ToString(reader["CODCT"]);
                            _projeto = Convert.ToString(reader["PROJETO"]);
                        }
                    }
                    var lista_boxes = lockerRetirada.Boxretirados.Split(',');
                    
                    foreach (var _boxUsado in lista_boxes)
                    {
                        if (await ExisteBoxCadastro(_idLocker, _boxUsado) == true)
                        {
                            _existeAlgumaBox = true;
                            AtualizaBoxLocker(_idLocker, _boxUsado, 0);
                            RegistraLogLocker(_idLocker, _boxUsado, 3, _seqRmDm, _equipe, lockerRetirada.observacao, "-", "-");
                        }
                        else
                        {
                            RegistraLogLocker(_idLocker, _boxUsado, 3, _seqRmDm, _equipe, "Box não cadastrado para esse Locker.", "-", "-");
                        }

                    }
                    if (_existeAlgumaBox==true)
                    {
                        AtualizaBoxLockerRmDm(lockerRetirada.Numero, "-", 3, lockerRetirada.dataretirada, lockerRetirada.horaRetirada);
                        retorno.Mensagem = "Sucesso ao retirar o Locker";
                    }
                    else
                    {
                        retorno.Mensagem = "Não houve o depósito, pois nenhuma das box enviadas estão cadastradas no Aniel para esse Locker.";
                    }
                      

                    retorno.Ok = true;
                    retorno.Objeto = true;
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    retorno.Ok = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = String.Format("Erro: {0} \n StackTrace: {1]", ex.Message, ex.StackTrace);
                }
                
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/LockerApi/AtualizaLockerDepositoAniel")]
        public async Task<GetApiRetorno<Boolean>> AtualizaLockerDepositoAniel(Locker_Deposito_Models lockerDeposito)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    bool _existeAlgumaBox = false;
                    bool _existeBox = false;
                    String _box = "000";
                    String _seqRmDm = "-";
                    String _equipe = "-";
                    String _codCt = "-";
                    String _projeto = "-";
                    Int32 _statusBox = 0;
                    Int32 _idLocker = 0;
                    String _qrCodeRetirada = $"R{lockerDeposito.IdEntrega}{lockerDeposito.codRetirada}";

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.CODCT,A.PROJETO,A.ID_LOCKER,A.STATUS_BOX_LOCKER,A.BOX,A.SEQRMDM,A.EQUIPE ");
                    sql.AppendLine("FROM TB_SEQUENCIA_RMDM A ");
                    sql.AppendLine("WHERE A.SEQ_LOCKER = @SEQ_LOCKER ");
                    parametros.Add("@SEQ_LOCKER", lockerDeposito.Numero);

                    using (var com = sessionT5.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            _box = Convert.ToString(reader["BOX"]);
                            _idLocker = Convert.ToInt32(reader["ID_LOCKER"]);
                            _statusBox = Convert.ToInt32(reader["STATUS_BOX_LOCKER"]);
                            _seqRmDm = Convert.ToString(reader["SEQRMDM"]);
                            _equipe = Convert.ToString(reader["EQUIPE"]);
                            _codCt = Convert.ToString(reader["CODCT"]);
                            _projeto = Convert.ToString(reader["PROJETO"]);
                        }
                    }


                    var lista_boxes = lockerDeposito.Boxusados.Split(',');
                    foreach (var _boxUsado in lista_boxes)
                    {
                        if (await ExisteBox(_idLocker,_seqRmDm,_codCt,_projeto, _boxUsado)!=true  )
                        {
                            if (await ExisteBoxCadastro(_idLocker, _boxUsado) == true)
                            {
                                _existeBox = true;
                                _existeAlgumaBox = true;
                                VinculaBoxLocker(_codCt, _projeto, _seqRmDm, _idLocker, _boxUsado, Convert.ToInt32(lockerDeposito.Numero));
                            }
                            else
                            {
                                _existeBox = false;
                            }
                            
                        }
                        else
                        {
                            if (await ExisteBoxCadastro(_idLocker, _boxUsado) == true)
                            {
                                _existeBox = true;
                                _existeAlgumaBox = true;
                            }
                            else
                            {
                                _existeBox = false;
                            }
                        }

                        if (_existeBox == true)
                        {
                            AtualizaBoxLocker(_idLocker, _boxUsado, 2);
                            RegistraLogLocker(_idLocker, _boxUsado, 2, _seqRmDm, _equipe, lockerDeposito.observacao, lockerDeposito.IdEntrega, lockerDeposito.codRetirada);
                        }
                        else
                        {
                            RegistraLogLocker(_idLocker, _boxUsado, 2, _seqRmDm, _equipe, "Esse Box não está cadastrado para esse Locker no Aniel.", lockerDeposito.IdEntrega, lockerDeposito.codRetirada);
                        }
                    }

                    if (_existeAlgumaBox == true)
                    {
                        AtualizaBoxLockerRmDm(lockerDeposito.Numero, _qrCodeRetirada, 2, lockerDeposito.datadeposito, lockerDeposito.horadeposito);
                        DesvinculaBoxLocker(_codCt, _projeto, _seqRmDm, Convert.ToInt32(_idLocker), lockerDeposito.Boxusados, Convert.ToInt32(lockerDeposito.Numero));
                        retorno.Mensagem = "Sucesso ao depositar o Locker";
                    }
                    else
                    {
                        retorno.Mensagem = "Não houve o depósito, pois nenhuma das box enviadas estão cadastradas no Aniel para esse Locker.";
                    }
                    retorno.Ok = true;
                    retorno.Objeto = true;
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    retorno.Ok = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = String.Format("Erro: {0} \n StackTrace: {1]", ex.Message, ex.StackTrace);
                }

            }
            return retorno;
        }

        private async Task<Boolean?> ExisteBox(int pIdLocker,string pSeqRmDm,string pCodCt,string pProjeto,string pBox)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
               // return false;

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COUNT(BOX),0)AS CO FROM TB_VINCULO_LOCKER_RMDM ");
                sql.AppendLine(" WHERE CODCT = @pCODCT AND PROJETO = @pPROJETO ");
                sql.AppendLine(" AND BOX = @pBOX AND SEQRMDM = @pSEQRMDM ");
                sql.AppendLine(" AND ID_LOCKER = @pID_LOCKER ");
                parametros.Add("@pCODCT", pCodCt);
                parametros.Add("@pPROJETO", pProjeto);
                parametros.Add("@pBOX", pBox);
                parametros.Add("@pSEQRMDM", pSeqRmDm);
                parametros.Add("@pID_LOCKER", Convert.ToString(pIdLocker));

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt64(reader["CO"]) > 0 ;
                    else
                        return false;
                }
            }
        }

        private async Task<Boolean?> ExisteBoxCadastro(int pIdLocker,string pBox)
        {
            using (var _session = HibernateUtil.GetSessionT5())
            using (var _com = _session.Connection.CreateCommand())
            {
                //return false;

                Dictionary<string, object> _parametros = new Dictionary<string, object>();

                StringBuilder _sql = new StringBuilder();
                _parametros.Clear();
                _sql.AppendLine(" SELECT COALESCE(COUNT(ID),0)AS CO FROM TB_LOCKER_PORTAS ");
                _sql.AppendLine(" WHERE ID = @pBOX ");
                _sql.AppendLine(" AND ID_LOCKER = @pID_LOCKER ");
                _parametros.Add("@pBOX", pBox);
                _parametros.Add("@pID_LOCKER", Convert.ToString(pIdLocker));

                using (var reader = await CommandHelper.ExecuteReader(_com, _sql, _parametros, _session.Connection, null))
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt64(reader["CO"]) > 0;
                    else
                        return false;
                }
            }
        }

        private async Task VinculaBoxLocker(string pCodCt,string pProjeto,string pSeqRmdm,int pidLocker,string pBox, int pSeqLocker)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            sql.Clear();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.Clear();
                parametros.Remove("@BOX");
                sql.AppendLine(" INSERT INTO TB_VINCULO_LOCKER_RMDM (CODCT,PROJETO,SEQRMDM,ID_LOCKER,BOX,SEQ_LOCKER) ");
                sql.AppendLine(" VALUES (@pCODCT,@pPROJETO,@pSEQRMDM,@pID_LOCKER,@pBOX,@pSEQ_LOCKER) ");
                parametros.Add("@pCODCT", pCodCt);
                parametros.Add("@pPROJETO", pProjeto);
                parametros.Add("@pSEQRMDM", pSeqRmdm);
                parametros.Add("@pID_LOCKER", Convert.ToString(pidLocker));
                parametros.Add("@pBOX", pBox);
                parametros.Add("@pSEQ_LOCKER", Convert.ToString(pSeqLocker));
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
        }
        private async Task DesvinculaBoxLocker(string pCodCt, string pProjeto, string pSeqRmdm, int pidLocker, string pBoxes, int pSeqLocker)
        {
            var lista_temp = pBoxes.Split(',');
            string listaBoxes = string.Join(", ", lista_temp.Select(c => string.Format("'{0}'", c)));
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            sql.Clear();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.Clear();
                sql.AppendLine(" DELETE FROM TB_VINCULO_LOCKER_RMDM ");
                sql.AppendLine(" WHERE CODCT = @pCODCT ");
                sql.AppendLine(" AND PROJETO = @pPROJETO ");
                sql.AppendLine(" AND SEQRMDM = @pSEQRMDM ");
                sql.AppendLine(" AND ID_LOCKER = @pID_LOCKER ");
                sql.AppendLine($" AND BOX NOT IN ({listaBoxes}) ");
                parametros.Add("@pCODCT", pCodCt);
                parametros.Add("@pPROJETO", pProjeto);
                parametros.Add("@pSEQRMDM", pSeqRmdm);
                parametros.Add("@pID_LOCKER", Convert.ToString(pidLocker));
                parametros.Add("@pSEQ_LOCKER", Convert.ToString(pSeqLocker));
                parametros.Add("@pBOX", Convert.ToString(listaBoxes));
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
        }

        private async Task AtualizaBoxLocker(Int32 pidLocker, String pBox, Int32 pStatus)
        {                             
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            sql.Clear();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.Clear();
                parametros.Remove("@BOX");
                sql.AppendLine("UPDATE TB_LOCKER_PORTAS SET STATUS = @STATUS ");
                sql.AppendLine("WHERE ID = @BOX ");
                sql.AppendLine("AND ID_LOCKER = @ID_LOCKER ");
                parametros.Add("@BOX", pBox);
                parametros.Add("@ID_LOCKER", pidLocker);
                parametros.Add("@STATUS", pStatus);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
        }
        private async Task RegistraLogLocker(Int32 pidLocker, String pBox, Int32 pStatus, String pSeqRmdm, String pEquipe, String pObs, String pIdEntrega, String pCodRetirada)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            sql.Clear();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                parametros.Remove("@pBOX");
                sql.Clear();
                sql.AppendLine("INSERT INTO TB_LOG_LOCKER(ID_LOCKER,BOX,SEQRMDM,EQUIPE, ");
                sql.AppendLine("                          STATUS,USUARIO,DIA,HORA,OBS,ID_ENTREGA,COD_RETIRADA) ");
                sql.AppendLine("            VALUES (@pID_LOCKER,@pBOX,@pSEQRMDM,@pEQUIPE, ");
                sql.AppendLine("                    @pSTATUS,@pUSUARIO,CURRENT_DATE,CURRENT_TIME, ");
                sql.AppendLine("                    @pOBS,@pID_ENTREGA,@pCOD_RETIRADA) ");

                parametros.Add("@pID_LOCKER", pidLocker);
                parametros.Add("@pBOX", pBox);
                parametros.Add("@pSTATUS", pStatus);
                parametros.Add("@pSEQRMDM", pSeqRmdm);
                parametros.Add("@pID_ENTREGA", pIdEntrega);
                parametros.Add("@pCOD_RETIRADA", pCodRetirada);
                parametros.Add("@pEQUIPE", pEquipe);
                parametros.Add("@pUSUARIO", "API-LOCKER");
                parametros.Add("@pOBS", pObs);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
        }

        private async Task AtualizaBoxLockerRmDm(String pNumero, String pQrCode, Int32 pStatus, String pData, String pHora)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            sql.Clear();
            parametros.Clear();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.AppendLine("UPDATE TB_SEQUENCIA_RMDM SET STATUS_BOX_LOCKER = STATUS_BOX_LOCKER ");

                if (pQrCode != "-")
                {
                    sql.AppendLine(", QRCODE_LOCKER_RETIRADA = @QRCODE_LOCKER_RETIRADA ");
                    sql.AppendLine(", DATA_LOCKER_DEPOSITO = @DATA_LOCKER_DEPOSITO ");
                    sql.AppendLine(", HORA_LOCKER_DEPOSITO = @HORA_LOCKER_DEPOSITO ");

                    parametros.Add("@DATA_LOCKER_DEPOSITO", pData.Replace('/', '.'));
                    parametros.Add("@HORA_LOCKER_DEPOSITO", pHora);
                }
                else
                {
                    sql.AppendLine(", DATA_LOCKER_RETIRADA = @DATA_LOCKER_RETIRADA ");
                    sql.AppendLine(", HORA_LOCKER_RETIRADA = @HORA_LOCKER_RETIRADA ");
                    sql.AppendLine(", RETORNO_RETIRADA = 1 ");

                    parametros.Add("@DATA_LOCKER_RETIRADA", pData.Replace('/', '.'));
                    parametros.Add("@HORA_LOCKER_RETIRADA", pHora);
                }
                               
                sql.AppendLine("WHERE SEQ_LOCKER = @SEQ_LOCKER ");
                parametros.Add("@SEQ_LOCKER", pNumero);
               
                
                //parametros.Add("@STATUS", pStatus);
                if (pQrCode != "-")
                {
                    parametros.Add("@QRCODE_LOCKER_RETIRADA", pQrCode);
                }
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
        }
    }

}