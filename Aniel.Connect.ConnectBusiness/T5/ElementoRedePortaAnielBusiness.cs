using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Sinapse.DAO.repository;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ElementoRedePortaAnielBusiness
    {
        private readonly IElementoRedePortaAnielRepository _repository;
        public ElementoRedePortaAnielBusiness(IElementoRedePortaAnielRepository repo) 
        {
            _repository = repo;
        }
                                
        /// <summary>
        /// Alteração de Reserva de caixa e porta (podendo originar, ou do atendimento, ou do próprio cliente)
        /// </summary>       
        public async Task AlterarReservaByAlteracaoEndereco(long?idAtendimento, long? idPlanoProduto) 
        {
            long idClientePlanoProduto = 0;
            string login = "";
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand()) 
                {
                    if (idAtendimento.HasValue)
                    {
                        var sql = new StringBuilder($"SELECT FIRST 1 A.ID_PLANO_PRODUTO FROM TB_ATENDIMENTO A WHERE A.ID = {idAtendimento}");
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null)) 
                        {
                            if (await reader.ReadAsync())
                                idClientePlanoProduto = Convert.ToInt64(reader["ID_PLANO_PRODUTO"]);                     
                        }
                    }                
                    else if (idPlanoProduto.HasValue) 
                    {
                        idClientePlanoProduto = idPlanoProduto.Value;
                    }
                }
                using (var com = session.Connection.CreateCommand()) 
                {
                    var sql = new StringBuilder($"SELECT FIRST 1 A.LOGIN FROM TB_LOGIN_RADIUS A WHERE A.ID = {idClientePlanoProduto}");
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null)) 
                    {
                        if (await reader.ReadAsync())
                            login = Convert.ToString(reader["LOGIN"]);
                    }
                }
            }

            Elemento_ReservaCadastroModel elemento_ = new Elemento_ReservaCadastroModel();
            elemento_.Login = login;
            elemento_.Cliente_PlanoProduto_Novo = idClientePlanoProduto;
            
            //      ---     Limpar porta    ---     //
            await this._repository.LimparPorta(elemento_ );

            //      ---     Alterar porta plano    ---     //
            elemento_.Cliente_PlanoProduto_Antigo = idClientePlanoProduto;
            await this._repository.AlterarPortaPlano(elemento_, false);
        }
    }  
}
