using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.Models
{
    public class DescontoMeioCaptacaoListModel
    {
        public long Id_Desconto { get; set; }
        public long Id_Meio_Captacao { get; set; }
    }

    public class DescontoMeioCaptacao
    {
        public Desconto_Captacao desconto_Captacao { get; set; }
        public long Id { get; set; }
        public string Text { get; set; }
        public string Valor_Desconto { get; set; }
        public string Percentual_Desconto { get; set; }
        public string Tipo_Desconto { get; set; }
    }
}