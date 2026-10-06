using Aniel.Connect.Model.nhibernate;
using log4net;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.DAO.service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.API_Aniel.Repositorio
{
    public class RepositoryBase : IDisposable
    {
        public async Task<Boolean> UsuarioValido(ISession sessionT5, String usuarioApiAniel, String senhaApiAniel, String tokenApiAniel, Boolean critica)
        {

            if (String.IsNullOrEmpty(usuarioApiAniel) || String.IsNullOrEmpty(senhaApiAniel) || String.IsNullOrEmpty(tokenApiAniel))
            {
                throw new Exception("Informe todos parâmetros.");
            }

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            var config = new ConfiguracaoUsuario();


            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT USUARIO_APIANIEL, SENHA_APIANIEL,TOKEN_APIANIEL FROM TB_CONFIGURACOES_GERAIS");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        config.Usuario = Convert.ToString(reader["USUARIO_APIANIEL"]);
                        config.Senha = Convert.ToString(reader["SENHA_APIANIEL"]);
                        config.Token = Convert.ToString(reader["TOKEN_APIANIEL"]);
                    }
                }

                if (config == null)
                {
                    throw new Exception("Configurações Gerais não realizadas no Connect");
                }

                if (config.Usuario == null && config.Senha == null && config.Token == null)
                {
                    throw new Exception("API Aniel não configurado em Connect/Configurações Gerais");
                }

                var valido = config.Usuario.ToUpper() == usuarioApiAniel.ToUpper() && config.Senha.ToUpper() == senhaApiAniel.ToUpper() && config.Token.ToUpper() == tokenApiAniel.ToUpper();

                if (!valido)
                {
                    throw new Exception("Usuário ou Senha ou Token inválidos");
                }

                return true;
            }
        }       
        

        public async Task<Boolean> AtualizarSenha(ISession session, String usuario, String senha)
        {

            if (String.IsNullOrEmpty(usuario) || String.IsNullOrEmpty(senha))
            {
                LogManager.GetLogger("Aniel.Connect.API_Aniel").Error($"Usuario e/ou Senha não informados. - Usuario: {usuario} | Senha: {senha}");
                throw new Exception("Usuario e/ou Senha não informados.");
            }

            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine($"UPDATE ASPNETUSERS A SET A.SENHA_ANIEL = @pSenha WHERE ID = @pUsuario ");
                parametros.Add("@pSenha", EncryptionService.EncryptionMD5(senha).ToUpper());
                parametros.Add("@pUsuario", usuario);

                var row = await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                if(row > 0)
                {
                    LogManager.GetLogger("Aniel.Connect.API_Aniel.INFO").Info($"Atualizou usuario: {usuario} com a senha: {EncryptionService.EncryptionMD5(senha).ToUpper()} - ({senha})");
                }

                return row > 0;
            }

        }


        #region IDisposable Support
        private Boolean disposedValue;// To detect redundant calls

        // IDisposable
        protected virtual void Dispose(Boolean disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {

                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    class ConfiguracaoUsuario
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Token { get; set; }
    }
}