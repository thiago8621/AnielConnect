using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Aniel.ConnectAPI.Dal.Service;

namespace Aniel.ConnectAPI.Dal.Helper
{
    public class CommandHelper
    {
        //public static String Strcnx = ConfigurationManager.AppSetting["ConnectionStrings:DefaultConnection"];

        public static FbConnection CreateConnection()
        {
            FbConnection conn = new FbConnection
            {
                ConnectionString = ConfigurationManager.AppSetting["ConnectionStrings"]
            };

            return conn;
        }

        public static FbConnection CreateConnection(String StringConnection)
        {
            FbConnection conn = new FbConnection
            {
                ConnectionString = StringConnection
            };

            return conn;
        }

        public static FbTransaction BeginTransaction(FbConnection conn)
        {
            FbTransaction transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

            return transaction;
        }


        private static FbCommand CreateCommand(FbConnection FBcon, FbTransaction trans)
        {
            FbCommand com = new FbCommand
            {
                Connection = FBcon
            };

            if (trans != null)
                com.Transaction = trans;

            return com;
        }

        #region ExecuteReader

        public static FbDataReader ExecuteReader(StringBuilder pSql, FbConnection FBcon, FbTransaction trans)
        {
            return ExecuteReader(pSql, new Dictionary<string, string>(), FBcon, trans);
        }

        public static FbDataReader ExecuteReader(StringBuilder pSql, Dictionary<string, string> parametros, FbConnection FBcon, FbTransaction trans)
        {
            if (FBcon == null)
                throw new Exception(String.Format("Coneção não está criada {0}", pSql));


            FbCommand com = CreateCommand(FBcon, trans);

            if (FBcon.State == System.Data.ConnectionState.Closed) FBcon.Open();

            if (com == null)
                throw new Exception(String.Format("Driver de conexão não criou connection {0}", pSql));

            try
            {
                com.CommandText = pSql.ToString();

                foreach (KeyValuePair<string, string> par in parametros)
                {
                    com.Parameters.Add(par.Key, par.Value);
                }

                return com.ExecuteReader();
            }
            catch (FbException ex)
            {
                if (ex.ErrorCode == 335544665)
                {
                    throw new Exception("Não é possível continuar pois existe um registro idêntico.");
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception pEx)
            {
                String mensagem = String.Format("Mensagem:{0}\nSQL:{1}\nParametros:{2}", pEx.Message, pSql.ToString(), parametros.ToString());
                throw new Exception(mensagem, pEx);
            }

        }

        public static FbDataReader ExecuteReader(StringBuilder pSql, Dictionary<string, object> parametros, FbConnection FBcon, FbTransaction trans)
        {
            if (FBcon == null)
                throw new Exception(String.Format("Coneção não está criada {0}", pSql));

            FbCommand com = CreateCommand(FBcon, trans);

            if (com == null)
                throw new Exception(String.Format("Driver de conexão não criou connextion {0}", pSql));

            if (FBcon.State == System.Data.ConnectionState.Closed) FBcon.Open();

            try
            {
                com.CommandText = pSql.ToString();

                foreach (KeyValuePair<string, object> par in parametros)
                {
                    com.Parameters.Add(par.Key, par.Value);
                }

                return com.ExecuteReader();
            }
            catch (FbException ex)
            {
                if (ex.ErrorCode == 335544665)
                {
                    throw new Exception("Não é possível continuar pois existe um registro idêntico.");
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception pEx)
            {
                String mensagem = String.Format("Mensagem:{0}\nSQL:{1}\nParametros:{2}", pEx.Message, pSql.ToString(), parametros.ToString());
                throw new Exception(mensagem, pEx);
            }
        }

        #endregion

        #region ExecuteNumQuery

        public static Int32 ExecuteNonQuery(StringBuilder pSql, FbConnection FBcon, FbTransaction trans)
        {
            return ExecuteNonQuery(pSql, new Dictionary<string, object>(), FBcon, trans);
        }

        public static Int32 ExecuteNonQuery(StringBuilder pSql, Dictionary<string, object> parametros, FbConnection FBcon, FbTransaction trans)
        {
            if (FBcon == null)
                throw new Exception(String.Format("Coneção não está criada {0}", pSql));

            FbCommand com = CreateCommand(FBcon, trans);

            if (com == null)
                throw new Exception(String.Format("Driver de conexão não criou connextion {0}", pSql));

            if (FBcon.State == System.Data.ConnectionState.Closed) FBcon.Open();

            com.CommandText = pSql.ToString();

            foreach (KeyValuePair<string, object> par in parametros)
            {
                com.Parameters.Add(par.Key, par.Value);
            }

            return com.ExecuteNonQuery();

        }

        public static Int32 ExecuteNonQuery(StringBuilder pSql, Dictionary<string, string> parametros, FbConnection FBcon, FbTransaction trans)
        {
            if (FBcon == null)
                throw new Exception(String.Format("Coneção não está criada {0}", pSql));

            FbCommand com = CreateCommand(FBcon, trans);

            if (com == null)
                throw new Exception(String.Format("Driver de conexão não criou connextion {0}", pSql));

            if (FBcon.State == System.Data.ConnectionState.Closed) FBcon.Open();

            com.CommandText = pSql.ToString();

            foreach (KeyValuePair<string, string> par in parametros)
            {
                com.Parameters.Add(par.Key, par.Value);
            }

            return com.ExecuteNonQuery();
        }

        public static long ExecuteNonQueryReturning(StringBuilder pSql, Dictionary<string, object> parametros, FbConnection FBcon, FbTransaction trans)
        {
            if (FBcon == null)
                throw new Exception(String.Format("Coneção não está criada {0}", pSql));

            FbCommand com = CreateCommand(FBcon, trans);

            if (com == null)
                throw new Exception(String.Format("Driver de conexão não criou connextion {0}", pSql));

            if (FBcon.State == System.Data.ConnectionState.Closed) FBcon.Open();

            com.CommandText = pSql.ToString();

            foreach (KeyValuePair<string, object> par in parametros)
            {
                com.Parameters.Add(par.Key, par.Value);
            }

            FbParameter outParam = new FbParameter("id", FbDbType.Integer)
            {
                Direction = ParameterDirection.Output
            };

            com.Parameters.Add(outParam);

            com.ExecuteNonQuery();

            long retorno = outParam.Value as long? ?? default(long);

            return retorno;
        }

        #endregion
    }
}
