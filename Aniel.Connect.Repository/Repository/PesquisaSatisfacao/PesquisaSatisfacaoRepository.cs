using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aniel.Connect.Model.nhibernate;

namespace Aniel.Connect.Repository.PesquisaSatisfacaoRepository
{
    public class PesquisaSatisfacaoRepository : IPesquisaSatisfacaoRepository
    {
        private readonly ISession _session;

        public PesquisaSatisfacaoRepository(ISession session)
        {
            this._session = session;
        }

        public void InsertPesquisaResposta(Aniel.Connect.Model.nhibernate.Cliente cliente,string usuario,long idPesquisa, long? idAtendimento)
        {
            using (var com = _session.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder($"insert into TB_PESQUISA_RESPOSTA (ID, USUARIOCRIACAO, DATAHORACRIACAO, ID_CLIENTE, ID_PESQUISA_SATISFACAO,OBS, ID_ATENDIMENTO)");
                sql.AppendLine(" values((SELECT COALESCE(MAX(ID), 0) + 1 FROM TB_PESQUISA_RESPOSTA), @USUARIOCRIACAO, CURRENT_TIMESTAMP, @ID_CLIENTE, @ID_PESQUISA_SATISFACAO,@OBS, @ID_ATT)");
                parametros.Clear();
                parametros.Add("@USUARIOCRIACAO", usuario);
                parametros.Add("@ID_CLIENTE", cliente.Id);
                parametros.Add("@ID_PESQUISA_SATISFACAO", idPesquisa);
                parametros.Add("@ID_ATT", idAtendimento);

                if (!String.IsNullOrEmpty(cliente.Email_Principal))
                {
                    parametros.Add("@OBS", "Pesquisa Enviada Com Sucesso");

                }
                else
                {
                    parametros.Add("@OBS", "Pesquisa Não Enviada, Cliente Sem E-mail");

                }

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _session.Connection, null).Wait();
            }

        }
        public IQueryable<Pesquisa_Satisfacao> GetPesquisaRespostaList()
        {
            using (var repoPesquisa = new Conexao<Pesquisa_Satisfacao>(_session))
            {
                return repoPesquisa.QueryableSelect();

            }
        }
    }
}
