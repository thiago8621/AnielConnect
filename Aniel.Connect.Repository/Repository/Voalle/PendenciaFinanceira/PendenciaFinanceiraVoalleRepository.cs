using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using NHibernate.Criterion;
using Npgsql;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aniel.Connect.Repository.Voalle.PendenciaFinanceira
{
    public class PendenciaFinanceiraVoalleRepository : IPendenciaFinanceiraIntegracaoRepository
    {
        private readonly NpgsqlConnection _npgsqlConnection;
        private bool _utilizaDw;
        public PendenciaFinanceiraVoalleRepository(NpgsqlConnection conn,bool utilizaDw) 
        {
            _npgsqlConnection = conn;
            _utilizaDw = utilizaDw;
        }
        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj) 
        {
            var dados = new List<PendenciaFinanceiraListModel>();

            var sql = new StringBuilder();
            var param = new Dictionary<string, object>();

            string documento = cpfCnpj.Replace("-", String.Empty)
                                      .Replace("/", String.Empty)
                                      .Replace(".", String.Empty);

            if (_utilizaDw)
            {
                sql.AppendLine(@"select p.people_id as id, p.name, p.tx_id as CPF_CNPJ, f.expiration_date, pa.postal_code, pa.street,
                                 pa.number, pa.neighborhood,pa.city, pa.address_complement, pa.state, current_date - f.expiration_date as Dias_Vencimento
                                 from datawarehouse.dwf_financial_receivable_titles f
                                 inner join datawarehouse.dwd_people p on (p.people_id = f.client_id)
                                 inner join datawarehouse.dwd_people_addresses pa on pa.people_address_id = p.people_id
                                 where tx_id = @DOCUMENTO and f.title like '%FAT%' and f.deleted != 'TRUE' and not exists
                                 (select * from datawarehouse.dwf_financial_receipt_titles t where f.financial_receivable_title_id = t.financial_receivable_title_id)
                                 and current_date -f.expiration_date >= 15 order by 12");
            }
            else
            {
                sql.AppendLine(@"select p.id, p.name, p.tx_id as CPF_CNPJ, f.expiration_date, pa.postal_code, pa.street_type, pa.street,")
                   .AppendLine(@" pa.number, pa.neighborhood,pa.city, pa.address_complement, pa.state, current_date - f.expiration_date as Dias_Vencimento ");
                sql.AppendLine(" from financial_receivable_titles f inner join people p on p.id = f.client_id ")
                   .AppendLine(" inner join people_addresses pa on pa.person_id = p.id ");
                sql.AppendLine("where tx_id = @DOCUMENTO and f.title like '%FAT%' and f.deleted != 'TRUE' and not exists")
                   .AppendLine(" (select * from financial_receipt_titles t where f.id = t.financial_receivable_title_id)")
                   .AppendLine(" and current_date - f.expiration_date >= 15 order by 12");
            }
            
            

            param.Add("@DOCUMENTO", documento);

            using (DbDataReader reader = CommandHelper.ExecuteReaderPostgres(sql, param, this._npgsqlConnection))
            {
                while (reader.Read())
                {
                    var values = new PendenciaFinanceiraListModel();
                    bool faturasuspensa = Convert.ToDateTime(reader["expiration_date"]) < DateTime.Now;
                    //Campos encontrados na consulta
                    values.Cod_Cliente = Convert.ToString(reader["id"]);
                    values.Cliente = Convert.ToString(reader["name"]);                                      
                    values.CpfCnpj = Convert.ToString(reader["CPF_CNPJ"]);
                    values.Logradouro = Convert.ToString(reader["street"]);
                    values.Numero = Convert.ToString(reader["number"]);
                    values.Bairro = Convert.ToString(reader["neighborhood"]);
                    values.Cidade = Convert.ToString(reader["city"]);
                    values.Data_Vencimento = Convert.ToString(reader["expiration_date"]);
                    values.Fatura_Suspensa = $"{Convert.ToInt16(faturasuspensa)}";
                    dados.Add(values);
                }
            }
            return dados;
        }
    }
}
