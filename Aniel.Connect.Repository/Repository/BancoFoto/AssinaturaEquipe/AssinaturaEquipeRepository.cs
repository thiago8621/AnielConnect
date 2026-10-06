using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Aniel.Connect.Repository.Repository.BancoFoto.AssinaturaEquipe
{
    public class AssinaturaEquipeRepository : IAssinaturaEquipeRepository
    {
        private readonly ISession _sessionFoto;
      

        public AssinaturaEquipeRepository(ISession sessionFoto)
        {
            this._sessionFoto = sessionFoto;
        }

        public bool ExisteAssinaturaEquipe(string equipe)        {

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            using (var com = _sessionFoto.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder($@"SELECT A.ASSINATURA, A.EQUIPE
                                                                FROM TB_ASSINATURA_EQUIPE A
                                                                WHERE A.EQUIPE = @pEQUIPE");

                parametros.Add("pEQUIPE", equipe);
               
                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionFoto.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var retorno = reader["ASSINATURA"] != DBNull.Value ? Convert.ToString(reader["ASSINATURA"]) : null;
                        
                        if (retorno ==  null)
                        {
                            return (false);
                        }
                        else
                        {
                            return (true);
                        }
                                                    
                    }
                }

            }
            return (false);

        }

        public bool ResetaAssinatura(string equipe)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (var _sessionFotoNh = HibernateUtil.GetSessionFoto())
            using (var comUpdateFoto = _sessionFotoNh.Connection.CreateCommand())
            {

                parametros.Add("@EQUIPE", equipe);

                StringBuilder sqlIns = new StringBuilder($"UPDATE TB_ASSINATURA_EQUIPE A SET A.ASSINATURA = NULL WHERE A.EQUIPE = @EQUIPE  ");
                CommandHelper.ExecuteNonQuery(comUpdateFoto, sqlIns, parametros, _sessionFotoNh.Connection, null).Wait();
                return true;

            }
       
          
        }
    }
}

