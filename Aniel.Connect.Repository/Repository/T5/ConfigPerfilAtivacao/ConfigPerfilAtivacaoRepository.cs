using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.ConfigPerfilAtivacao
{
    public class ConfigPerfilAtivacaoRepository : IConfigPerfilAtivacaoRepository
    {
        private readonly ISession _sessionT5Nh;

        public ConfigPerfilAtivacaoRepository(ISession sessionT5Nh)
        {
            _sessionT5Nh = sessionT5Nh;
        }

        public async Task<ConfigPerfilAtivacaoAniel> GetByVinculo(string Codmat, string Id_OLT)
        {
            ConfigPerfilAtivacaoAniel retorno = null;

            using (var comT5 = _sessionT5Nh.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($"SELECT A.ID, A.CODMAT, A.ID_PERFIL_ATIVACAO, A.ID_PERFIL_ATIVACAO_ONU, A.ID_OLT ");
                sql.AppendLine(" FROM TB_CONFIG_PERFIL_ATIVACAO A ");
                sql.AppendLine($"WHERE A.ID_OLT=@ID_OLT AND CODMAT=@CODMAT");

                parametros.Add("@CODMAT", Codmat);
                parametros.Add("@ID_OLT", Id_OLT);

                using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, _sessionT5Nh.Connection, null, 1, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new ConfigPerfilAtivacaoAniel();

                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Codmat = Convert.ToString(reader["CODMAT"]);
                        retorno.Id_Perfil_Ativacao = Convert.ToString(reader["ID_PERFIL_ATIVACAO"]);
                        retorno.Id_Perfil_Ativacao_Onu = Convert.ToString(reader["ID_PERFIL_ATIVACAO_ONU"]);
                        retorno.Id_OLT = Convert.ToString(reader["ID_OLT"]);                        
                    }
                }
            }

            return retorno;
        }

        public async Task<long> GetProximoId()
        {
            long proximoId = 0;
            using (var comT5 = _sessionT5Nh.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT COALESCE(MAX(ID),0) + 1 AS ID FROM TB_CONFIG_PERFIL_ATIVACAO ");

                using (var reader = await CommandHelper.ExecuteReader(comT5, sql, parametros, _sessionT5Nh.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        proximoId = Convert.ToInt64(reader["ID"]);
                    }
                }
            }

            return proximoId;
        }

        public async Task InsertOrUpdate(ConfigPerfilAtivacaoAniel objeto)
        {
            using (var comT5 = _sessionT5Nh.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("UPDATE OR INSERT INTO TB_CONFIG_PERFIL_ATIVACAO (ID, CODMAT, ID_PERFIL_ATIVACAO, ID_OLT, ID_PERFIL_ATIVACAO_ONU, USUARIO_IMPORTACAO, DATAHORA_IMPORTACAO) ");
                sql.AppendLine("VALUES (@ID, @CODMAT, @ID_PERFIL_ATIVACAO, @ID_OLT, @ID_PERFIL_ATIVACAO_ONU, @USUARIO_IMPORTACAO, @DATAHORA_IMPORTACAO) ");

                if (objeto.Id == 0)
                    objeto.Id = await this.GetProximoId();

                parametros.Add("@ID", objeto.Id);
                parametros.Add("@CODMAT", objeto.Codmat);
                parametros.Add("@ID_PERFIL_ATIVACAO", objeto.Id_Perfil_Ativacao);
                parametros.Add("@ID_OLT", objeto.Id_OLT);
                parametros.Add("@ID_PERFIL_ATIVACAO_ONU", !String.IsNullOrEmpty(objeto.Id_Perfil_Ativacao_Onu) ? objeto.Id_Perfil_Ativacao_Onu : null);
                parametros.Add("@USUARIO_IMPORTACAO", objeto.Usuario_Importacao);
                parametros.Add("@DATAHORA_IMPORTACAO", objeto.DataHora_Importacao);

                await CommandHelper.ExecuteNonQuery(comT5, sql, parametros, _sessionT5Nh.Connection, null);
                
            }
        }
    }
}
