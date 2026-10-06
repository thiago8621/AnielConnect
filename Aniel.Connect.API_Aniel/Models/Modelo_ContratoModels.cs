using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using System.Collections.Generic;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Modelo_ContratoGenerateVariable
    {
        private List<PostApiMensagem> pMensagens;
        private IEnumerable<Cliente_PlanoProduto> pPlanos;
        public List<PostApiMensagem> mensagens
        {
            get
            {
                if (pMensagens == null)
                {
                    pMensagens = new List<PostApiMensagem>();

                }
                return pMensagens;
            }
            set { pMensagens = value; }
        }
        public IEnumerable<Cliente_PlanoProduto> planos
        {
            get
            {
                if (pPlanos == null)
                {
                    pPlanos = new List<Cliente_PlanoProduto>();

                }
                return pPlanos;
            }
            set { pPlanos = value; }
        }
        public ClienteContrato clienteContrato { get; set; }
        public VariaveisModeloContrato item { get; set; }
        public Cliente_PlanoProduto cpp { get; set; }
        public Venda Venda { get; set; }

    }
}