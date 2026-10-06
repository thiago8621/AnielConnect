using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.EtapaVenda
{
    public class EtapaVendaRepository : IEtapaVendaRepository
    {
        private readonly ISession _sessionNh;

        public EtapaVendaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.EtapaVenda> GetAll()
        {
            using (var repoEtapaVenda = new Conexao<Aniel.Connect.Model.nhibernate.EtapaVenda>(this._sessionNh))
            {
                return repoEtapaVenda.QueryableSelect();
            }
        }

        public IEnumerable<Connect.Model.nhibernate.EtapaVenda> GetByVenda(long idVenda)
        {
            return this.GetAll()
                       .Where(e => e.Venda.Id == idVenda);
        }
        
        public IEnumerable<Connect.Model.nhibernate.EtapaVenda> GetByTrocaTitularidade(long idTroca)
        {
            return this.GetAll()
                       .Where(e => e.TrocaTitularidade.Id == idTroca);
        }

        public async Task RemoverEtapasVenda(List<Connect.Model.nhibernate.EtapaVenda> etapasVendas)
        {
            using (var repoEtapaVenda = new Conexao<Aniel.Connect.Model.nhibernate.EtapaVenda>(this._sessionNh))
            {
                await repoEtapaVenda.Remove(etapasVendas);
            }
        }

        
        public async Task UpdateEtapaAtualVenda(Connect.Model.nhibernate.EtapaVenda etapa, long idVenda)
        {
            using (var repoVenda = new Conexao<Aniel.Connect.Model.nhibernate.Venda>(this._sessionNh))
            {
                var venda = await repoVenda.selectFirst(x => x.Id == idVenda);
                venda.Etapa_Atual = etapa;

                await repoVenda.Update(venda);
            }
        }

        public async Task<bool> ConsultaGeraEtapaOS(long idVenda) 
        {
            bool retorno = false;
            try 
            {
                using (var com = this._sessionNh.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();
                    sql.AppendLine(" SELECT COUNT(A.ID) AS IDS FROM TB_CLIENTE_PLANO_PRODUTO A ")
                       .AppendLine(" INNER JOIN TB_PLANO B ON A.ID_PLANO = B.ID ")
                       .AppendLine($" WHERE A.ID_VENDA = {idVenda} AND B.STATUSINICIAL = {Convert.ToInt32(Status_PlanoProduto.Aguardando_Criacao_OS)} ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, this._sessionNh.Connection, null))
                    {                        
                        if (await reader.ReadAsync())
                        {
                            retorno = Convert.ToInt32(reader["IDS"]) > 0;
                        }
                    }
                }
            }
            catch (Exception) 
            {
                retorno = false;
            }
            
            return retorno;
        }

        public void Save(Connect.Model.nhibernate.EtapaVenda etapa)
        {
            using (var repoEtapaVenda = new Conexao<Aniel.Connect.Model.nhibernate.EtapaVenda>(this._sessionNh))
            {
                repoEtapaVenda.Save(etapa).Wait();
            }
        }

        public void Update(Connect.Model.nhibernate.EtapaVenda etapa)
        {
            using (var repoEtapaVenda = new Conexao<Aniel.Connect.Model.nhibernate.EtapaVenda>(this._sessionNh))
            {
                repoEtapaVenda.Update(etapa).Wait();
            }
        }
    }
}
