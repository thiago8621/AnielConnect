using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Requisicao
{
    public class RequisicaoAnielRepository : IRequisicaoAnielRepository
    {
        private readonly ISession _sessionT5;

        public RequisicaoAnielRepository(ISession sessionT5)
        {
            _sessionT5 = sessionT5;
        }

        public async Task<string> GetSeqRMDM(string codct, string projeto)
        {
            String sSeqRmdm = String.Empty;
            Int64 SeqRmdm = 0;
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            StringBuilder sql = new StringBuilder(@"EXECUTE PROCEDURE SP_PEGA_ULTIMA_RMDM(@CODCT, @PROJETO)");

            parametros.Add("@CODCT", codct);
            parametros.Add("@PROJETO", projeto);

            using (var com = _sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5.Connection, null))
            {
                if (await reader.ReadAsync())
                {
                    SeqRmdm = (Convert.ToInt64(reader["RSEQRMDM"]) + 1);
                }
            }
            sSeqRmdm = Convert.ToString(SeqRmdm).PadLeft(7,'0');

            return sSeqRmdm;
        }
    }
}
