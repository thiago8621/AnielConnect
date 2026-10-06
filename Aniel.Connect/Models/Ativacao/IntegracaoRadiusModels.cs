using Aniel.Connect.Core;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models.Ativacao
{
    public class PlanoProdutoRadiusRequest
    {
        public PlanoProdutoRadiusRequest() { }
        public PlanoProdutoRadiusRequest(long idPlanoProduto) 
        {
            this.IdClientePlanoProduto = idPlanoProduto;
        }

        public long IdClientePlanoProduto { get; set; }
        public string Usuario { get; set; }
    }
    public class ClientesRadiusRequest
    {
        public List<long> ListIdCliente { get; set; }
    }

    public class GetHistoricoConexaoRadius_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Login_Radius { get; set; }
        public virtual string Login_Radius { get; set; }
    }

    public class GetHistoricoConexaoRadius_Response
    {
        public List<RelatorioHistoricoConexaoModel> lista { get; set; }
        public Int32 recordsTotal { get; set; }
    }
}