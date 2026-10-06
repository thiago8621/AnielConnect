using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class TrocaTitularidade : BaseModel
    {
        public virtual Int64 NumeroContrato { get; set; }
        public virtual DateTime? DataAprovacao { get; set; }
        public virtual DateTime? DataCancelamento { get; set; }
        public virtual DateTime? DataAssinaturaTermoAdesao { get; set; }
        public virtual DateTime? DataAssinaturaChecklist { get; set; }
        public virtual Boolean TermoAdesaoAssinado { get; set; }
        public virtual Boolean ChecklistAssinado { get; set; }
        public virtual Int64? IdConsultaPendenciaFinanceira { get; set; }
        public virtual String IpOrigemAceite { get; set; }
        public virtual String PortaOrigemAceite { get; set; }
        public virtual String UserAgentTermoAceite { get; set; }
        public virtual String CodigoHashTermoAceite { get; set; }
        public virtual Boolean AdicionadoAceiteDigital { get; set; }
        public virtual String UrlAceiteDigital { get; set; }
        public virtual Int64? IdConsultaEndereco { get; set; }
        public virtual Int64? IdConsultaCredito { get; set; }
        public virtual string UsuarioAprovacao { get; set; }
        public virtual string UsuarioCancelamento { get; set; }
        public virtual Cliente ClienteTitularAtual { get; set;}
        public virtual Cliente ClienteTitularNovo { get; set; }
        public virtual EtapaVenda Etapa_Atual { get; set; }
        public virtual StatusTrocaTitularidade StatusTrocaTitularidade { get; set; }
    }
}
