using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator
{
    public class ConfigAnielIntegratorRepository : IConfigAnielIntegratorRepository
    {
        private readonly ISession _sessionT5Nh;

        public ConfigAnielIntegratorRepository(ISession sessionT5Nh)
        {
            _sessionT5Nh = sessionT5Nh;
        }

        public async Task<Config_Aniel_Integrator> GetByProjeto(string codct, string projeto)
        {
            Config_Aniel_Integrator config = new Config_Aniel_Integrator();
            using (var comT5 = _sessionT5Nh.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($"SELECT * ");
                sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");
                sql.AppendLine($"WHERE CODCT_PADRAO=@CODCT AND PROJETO_PADRAO=@PROJETO");

                parametros.Add("@CODCT", codct);
                parametros.Add("@PROJETO", projeto);

                if (_sessionT5Nh.Transaction != null) _sessionT5Nh.Transaction.Enlist(comT5);

                using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, _sessionT5Nh.Connection, comT5.Transaction, 1, null))
                {
                    if (await reader.ReadAsync())
                    {
                        config.URL = Convert.ToString(reader["URL"]);
                        config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                        config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                        config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                        config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                        config.Token = Convert.ToString(reader["TOKEN"]);
                        config.Setores = Convert.ToString(reader["SETORES"]);
                        config.IdFilial = reader["ID_FILIAL"] != DBNull.Value ? Convert.ToInt64(reader["ID_FILIAL"]) : (Int64?)null;
                    }
                }
            }
            return config;
        }

        public async Task<Config_Aniel_Integrator> ObterConfigAnielIntegrator()
        {
            Config_Aniel_Integrator config = new Config_Aniel_Integrator();
            using (var comT5 = _sessionT5Nh.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($"SELECT * FROM TB_CONFIG_ANIEL_INTEGRATOR ");

                if (_sessionT5Nh.Transaction != null)  _sessionT5Nh.Transaction.Enlist(comT5); 
                //DAO IXC usando o Integrator
                using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, _sessionT5Nh.Connection, comT5.Transaction, 1, null))
                {
                    if (await reader.ReadAsync())
                    {
                        config.URL = Convert.ToString(reader["URL"]);
                        config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                        config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                        config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                        config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                        config.Token = Convert.ToString(reader["TOKEN"]);
                        config.Setores = Convert.ToString(reader["SETORES"]);
                        config.IdFilial = reader["ID_FILIAL"] != DBNull.Value ? Convert.ToInt64(reader["ID_FILIAL"]) : (Int64?)null;
                    }
                }
            }
            return config;
        }
    }
}
