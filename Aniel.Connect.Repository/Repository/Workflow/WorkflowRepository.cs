using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.Repository.Workflow
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly ISession _sessionNh;

        public WorkflowRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Connect.Model.nhibernate.Workflow_Vendas> GetAll()
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Workflow_Vendas>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Workflow_Vendas GetById(long Id)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Workflow_Vendas>(this._sessionNh))
            {
                return repo.selectFirst(p => p.Id == Id).Result;
            }
        }

        public async Task<List<Workflow_Vendas>> FindWorkFlow(VerificaWorkflowRequestModel dadosVerificar)
        {
            using (Conexao<Workflow_Vendas> repo = new Conexao<Workflow_Vendas>(this._sessionNh))
            {
                var ativos = await repo.select(p => p.Status == Status_Default.Ativo);

                var cidade = !String.IsNullOrEmpty(dadosVerificar.Cidade) ? Convert.ToInt16(dadosVerificar.Cidade) : (int?)null;
                //Busca todos workflows ativos que forem iguais ao parametro ou a null
                List<Workflow_Vendas> workflow_Vendas = ativos.Where(z => (z.Uf == (UF)Enum.Parse(typeof(UF), dadosVerificar.UF) || z.Uf == null)
                    && (z.Captacao == dadosVerificar.Captacao || z.Captacao == null)
                    && (z.Categoria == dadosVerificar.Categoria || z.Categoria == null)
                    && (z.TipoPessoa == (Tipo_Pessoa)Enum.Parse(typeof(Tipo_Pessoa), dadosVerificar.TipoPessoa) || z.TipoPessoa == null)
                    && (z.Cidade == cidade || z.Cidade == null)
                    ).ToList();

                // Encontrar o objeto com menos valores nulos e consequentemente, maiores iguais aos parêmtros recebidos
                var listaOrdenada = workflow_Vendas
                    .OrderByDescending(obj =>
                        new List<object> { obj.Uf, obj.Captacao, obj.Categoria, obj.TipoPessoa, obj.Cidade }
                            .Count(value => value != null)
                    )
                    .ToList();

                return listaOrdenada;
            }
        }
    }
}