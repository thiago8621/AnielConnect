using Aniel.ConnectAPI.Dal.Helper;
using Aniel.ConnectAPI.Model.Except;
using Aniel.ConnectAPI.Model.Params;
using FirebirdSql.Data.FirebirdClient;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        public delegate Boolean MensagemCustomizadaErro();

        protected FbConnection CreateConnection()
        {
            return ConnectionHelper.CreateConnection();
        }

        protected FbTransaction GetTransaction(FbConnection Conn)
        {
            if (Conn.State == System.Data.ConnectionState.Closed)
                Conn.Open();
            return ConnectionHelper.BeginTransaction(Conn);
        }

        protected void ValidaParamWebApi(ParamWebApi param, String metodo)
        {
            if (param == null || param.Usuario == null || param.Token == null)
            {
                throw new Exception(String.Format("Serviço {0} - executado com valores inválidos.", metodo));
            }

            /*if (param.Token.ToUpper() != "DEBUG")
            {
                if (CryptSimple.Encrypt(param.Usuario, true) != param.Token)
                {
                    throw new Exception(String.Format("Token informado para o Serviço {0} Inválido.", metodo));
                }
            }*/

            try
            {
                //if (Convert.ToInt32(param.Token) > Convert.ToInt32(CONSTANTES.Versao))
                //    throw new EAnielException("Aguarde alguns minutos a atualização do servidor e tente novamente.");

                if ((param.Token.ToUpper() == "DEBUG") || (Convert.ToInt32(param.Token) < Convert.ToInt32(CONSTANTES.Versao)))
                    throw new EAnielException("O aplicativo está desatualizado. Por favor acesse a Play Store para atualizar.");
            }
            catch (EAnielException e)
            {
                throw e;
            }
            catch { }
        }

        protected void ValidaParamWebApi<T>(ParamWebApi<T> param, String metodo)
        {
            if (param == null || param.Usuario == null || param.Token == null)
            {
                throw new EAnielException(String.Format("Serviço {0} - executado com valores inválidos.", metodo));
            }

            /*
            if (param.Token.ToUpper() != "DEBUG")
            {
                if (CryptSimple.Encrypt(param.Usuario, true) != param.Token)
                {
                    throw new EAnielException(String.Format("Token informado para o Serviço {0} Inválido.", metodo));
                }
            }
            */

            if (param.Objeto == null)
            {
                throw new EAnielException(String.Format("Serviço {0} executado com valores inválidos.", metodo));
            }

            try
            {
                //if (Convert.ToInt32(param.Token) > Convert.ToInt32(CONSTANTES.Versao))
                //    throw new EAnielException("Aguarde alguns minutos a atualização do servidor e tente novamente.");

                if ((param.Token.ToUpper() == "DEBUG") || (Convert.ToInt32(param.Token) < Convert.ToInt32(CONSTANTES.Versao)))
                    throw new EAnielException("O aplicativo está desatualizado. Por favor acesse a Play Store para atualizar.");
            }
            catch (EAnielException e)
            {
                throw e;
            }
            catch { }
        }

        protected void TrataRetornoErro<T>(Assembly assembly, out Response<T> response, Exception ex, MensagemCustomizadaErro mensagemCustomizada)
        {
            TrataRetornoErro<T>(assembly, out response, ex, mensagemCustomizada, "");
        }

        protected void TrataRetornoErro<T>(Assembly assembly, out Response<T> response, Exception ex, MensagemCustomizadaErro mensagemCustomizada, String jsonValue)
        {
            response = new Response<T>
            {
                result = false
            };

            if (ex.GetType().IsSubclassOf(typeof(EAnielException)) || (ex.GetType() == typeof(EAnielException)))
            {
                response.message = ex.Message; //String.Concat(GetMensException(ex), ex.StackTrace);
                //LogManager.GetLogger(assembly.GetName().Name).Error(ex.Message);
            }
            else if (ex.GetType() == typeof(AggregateException))
            {
                foreach (Exception exinner in ((AggregateException)ex).InnerExceptions)
                {
                    response.exception += String.Format("{0}\n", TrataErro(ex));
                    response.stacktrace += String.Format("{0}\n", ex.StackTrace);
                }
            }
            else if (mensagemCustomizada != null)
            {
                if (!mensagemCustomizada())
                {
                    response.exception = TrataErro(ex);
                    response.stacktrace += String.Format("{0}\n", ex.StackTrace);
                }
            }
            else
            {
                response.exception = TrataErro(ex);
                response.stacktrace += String.Format("{0}\n", ex.StackTrace);
            }

            if (!(ex.GetType().IsSubclassOf(typeof(EAnielException)) || (ex.GetType() == typeof(EAnielException))))
                LogManager.GetLogger(assembly.GetType()).Error(String.Format("ERROR:{0}{1}STACKTRACE:{2}{3}OBJETO:{4}{5}{6}",
                    ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, jsonValue, Environment.NewLine, Environment.NewLine));
        }

        protected void TrataRetornoErro(Assembly assembly, out Response response, Exception ex, MensagemCustomizadaErro mensagemCustomizada)
        {
            TrataRetornoErro(assembly, out response, ex, mensagemCustomizada, "");
        }

        protected void TrataRetornoErro(Assembly assembly, out Response response, Exception ex, MensagemCustomizadaErro mensagemCustomizada, String valueJSON)
        {
            response = new Response
            {
                result = false
            };

            if (ex.GetType() == typeof(EAnielException))
            {
                response.message = String.Format("{0}\n{1}", ex.Message, ex.InnerException?.Message); //String.Concat(GetMensException(ex), ex.StackTrace);
                //LogManager.GetLogger(assembly.GetName().Name).Error(String.Format("ERROR:{0}\n\rSTACKTRACE:{1}\n\rOBJETO:{2}", ex.Message,ex.StackTrace, valueJSON));
            }
            else if (ex.GetType() == typeof(AggregateException))
            {
                foreach (Exception exinner in ((AggregateException)ex).InnerExceptions)
                {
                    response.exception += String.Format("{0}\n{1}", TrataErro(ex), ex.InnerException?.Message);
                    response.stacktrace += String.Format("{0}\n", ex.StackTrace);
                }
            }
            else if (mensagemCustomizada != null)
            {
                if (!mensagemCustomizada())
                {
                    response.exception = String.Format("{0}\n{1}", TrataErro(ex), ex.InnerException?.Message);
                    response.stacktrace += String.Format("{0}\n{1}", ex.StackTrace);
                }
            }
            else
            {
                response.exception = String.Format("{0}\n{1}", TrataErro(ex), ex.InnerException?.Message);
                response.stacktrace += String.Format("{0}\n", ex.StackTrace);
            }

            if (!(ex.GetType() == typeof(EAnielException)))
                LogManager.GetLogger(assembly.GetType()).Error(String.Format("ERROR:{0}{1}STACKTRACE:{2}{3}OBJETO:{4}{5}{6}",
                    ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, valueJSON, Environment.NewLine, Environment.NewLine));
        }

        private String GetMensException(Exception ex)
        {
            return String.Concat(ex.Message, " ", ex.InnerException == null ? String.Empty : ex.InnerException.Message).ToUpperInvariant();
        }

        private String TrataErro(Exception ex)
        {
            String Menssagem = GetMensException(ex);

            if (CONSTANTES.TrataErro == "N")
                return Menssagem;

            if (Menssagem.Contains("is not a valid date"))
                return "Data inválida, proceda a correção.";
            else if (Menssagem.Contains("must have a value"))
            {
                return "É obrigatório o preenchimento do campo";
            }
            else if (Menssagem.Contains("key violation"))
                return "Houve violação de Chave.  Registro já incluido.";
            else if (Menssagem.Contains("Input value"))
                return "Campo preenchido com valor não válido. Proceda a correção.";
            else if (Menssagem.Contains("is not a valid time"))
                return "Hora inválida, proceda a correção.";
            else if (Menssagem.Contains("NUMBER IS OUT OF RANGE"))
                return "Valor informado acima do limite permitido..";
            else if (Menssagem.Contains("master has detail records"))
                return "Proibida a Exclusão e Modificação deste Registro, pois ele faz Referência a Outro(s) Registro(s)!";
            else if (Menssagem.Contains("field value required"))
                return "É obrigatório o preenchimento do campo ";
            else if ((Menssagem.Contains("RECORD LOCKED BY ANOTHER USER.") ||
                     (Menssagem.Contains("deadlock")) || (Menssagem.Contains("update conflicts")) || (Menssagem.Contains("concurrent"))
                     || (Menssagem.Contains("transaction"))))
                return "O Registro está sendo Utilizado por Outro Usuário da Rede. Cancele a Operação e Aguarde a Liberação do Mesmo!";
            else if ((Menssagem.Contains("KEY VIOLATION")) || (Menssagem.Contains("UNIQUE")) || (Menssagem.Contains("PRIMARY")))
                return "Não é possível gravar. Existe um registro idêntico.";
            else if (Menssagem.Contains("LOCKED"))
                return "Impossível confirmar da gravação. Este registro está sendo alterado em outra estação.";
            else if (Menssagem.Contains("INVALID FIELD NAME"))
                return "Nome de campo inválido.";
            else if ((Menssagem.Contains("SIZE MISTMACH")) || (Menssagem.Contains("OVERFLOW")) ||
                     (Menssagem.Contains("THE ALLOWED RANGE IS")) || (Menssagem.Contains("IS NOT A VALID INTEGER VALUE")))
                return "Tamanho de campo inválido.";
            else if (Menssagem.Contains("SHUTDOWN"))
                return "Banco de dados em manutenção.";
            else if (Menssagem.Contains("O MÉTODO SAVEAS DA CLASSE WORKBOOK FALHOU"))
                return "A planilha não pode ser gravada.";
            else if (String.IsNullOrEmpty(Menssagem))
                return "Erro desconhecido";
            else if (Menssagem.Contains("UNABLE TO COMPLETE NETWORK REQUEST TO HOST"))
                return "A operação não foi realizada e a tela será fechada. Falha na comunicação com o servidor.";

            else if ((Menssagem.Contains("RECORD NOT FOUND")) || (Menssagem.Contains("CHANGED BY ANOTHER USER")))
                return "O Registro não foi encontrado ou foi alterado por um outro usuário. Por favor atualize os dados.";
            else if (Menssagem.Contains("EX_"))
            {
                String Mens = Menssagem.Replace(GetNomeException(Menssagem), "");
                Mens = Menssagem.Replace("At trigger", "Rotina ");
                return Mens;
            }
            else if (Menssagem.Contains("CANNOT BE USED IN A FILTER EXPRESSION"))
                return "O Campo selecionado não pode ser usado como filtro.";
            else if ((Menssagem.Contains("OPERATION ABORTED")) || (Menssagem.Contains("REFERÊNCIA DE OBJETO NÃO DEFINIDA")) ||
                     (Menssagem.Contains("NÃO FOI POSSÍVEL GRAVAR DADOS NO SERVIDOR.")))
                return "Não foi possível executar operação, tente novamente mais tarde";
            else if (Menssagem.Contains("DYNAMIC SQL ERROR") || Menssagem.Contains("VALIDATION ERROR FOR COLUMN"))
                return "Não foi possível executar operação, tente novamente mais tarde";
            else
                return string.IsNullOrEmpty(ex.Message) ? Menssagem : ex.Message;
        }

        private String GetNomeException(String Menssagem)
        {
            String Retorno = String.Empty;
            Int32 PosIni = Menssagem.IndexOf("EX_");
            String Ex = Ex = Menssagem.Substring(PosIni, Menssagem.Length - PosIni);

            for (int i = 1; i < Ex.Length; i++)
            {
                if (Ex.ToArray()[i] == ' ')
                    break;
                Retorno += Ex.ToArray()[i];
            }

            return Retorno;
        }

    }
}
