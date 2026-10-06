using Aniel.Connect.Core;
using Aniel.Connect.ModelLog.enumerados;
using System;

namespace Aniel.Connect.Models
{


    public class IntegracaoConfigIntegratorLogDataTableRequest : DataTableRequest
    {
        public virtual String Data { get; set; }

    }

    public class IntegracaoConfigIntegratorLogListaModel
    {
        public virtual String Data { get; set; }
        public virtual DateTime DataHoraDateTime { get; set; }
        public virtual String Nome { get; set; }
    }

    public class IntegracaoConfigLogModel : DataTableRequest
    {
        public virtual String Data { get; set; }
        public virtual String IdMomentoLog { get; set; }
        public virtual String NumeroOrdemServico { get; set; }
        public virtual String Status { get; set; }
    }

    public class IntegracaoLogMensagensModels
    {
        public virtual String Id { get; set; }
        public virtual String Servico { get; set; }
        public virtual String NumOrdemServico { get; set; }
        public virtual String Mensagem { get; set; }
        public virtual String Momento { get; set; }
        public virtual Boolean? Situacao { get; set; }
        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual String NUM_DOC { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String JsonEnvio { get; set; }

        public virtual String JsonRetorno { get; set; }

    }
}