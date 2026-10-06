using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.TipoServico
{
    public class TipoServicoAnielRepository : ITipoServicoAnielRepository
    {
        private readonly ISession _sessionT5Sql;
        private readonly ISession _sessionT5Nh;

        public TipoServicoAnielRepository(ISession sessionT5Nh,ISession _sessionT5Sql)
        {
            this._sessionT5Nh = sessionT5Nh;
            this._sessionT5Sql = _sessionT5Sql;
        }

        public Tipo_Servico_EquipeAniel GetByDescriacaoContrato(string descricao, string codCt)
        {
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(this._sessionT5Nh))
            {
                return repoTipoServico.selectFirst(p => p.Descricao == descricao && p.CodCt == codCt).Result;
            }
        }

        public Tipo_Servico_EquipeAniel GetByCodigo(int cod)
        {
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(this._sessionT5Nh))
            {
                return repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == cod).Result;
            }
        }

        public TimeSpan? TempoServicoMinimoNaturezas(List<int> naturezas)
        {
            TimeSpan? tempoMinimo = (TimeSpan?)null;
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine($@"SELECT MIN(T.HORAS) AS HORAS
                                    FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL T
                                    LEFT JOIN TB_TIPO_SERVICO_EQUIPE A ON (T.ID_TIPO_SERVICO = A.COD_TIPO_SERV)
                                    WHERE A.COD_NATUREZA IN ({String.Join(",", naturezas)})");

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Sql.Connection, null).Result)
                {
                    if (reader.ReadAsync().Result)
                    {
                        tempoMinimo = reader["HORAS"] != DBNull.Value ? (TimeSpan)(reader["HORAS"]) : (TimeSpan?)null;
                    }
                }
            }

            return tempoMinimo;
        }
    }
}
