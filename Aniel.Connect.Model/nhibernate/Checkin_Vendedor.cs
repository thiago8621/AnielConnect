using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Checkin_Vendedor : BaseModel
    {
        public virtual Funcionario Vendedor { get; set; }
        public virtual DateTime? Data { get; set; }
        public virtual DateTime? DataInicial { get; set; }
        public virtual DateTime? DataFinal { get; set; }
        public virtual String Endereco_Checkin { get; set; }
        public virtual Double? Latitude_Checkin { get; set; }
        public virtual Double? Longitude_Checkin { get; set; }
        public virtual Double? Longitude_Checkout { get; set; }
        public virtual Double? Latitude_Checkout { get; set; }
        public virtual String Endereco_Checkout { get; set; }


        private Int64? _idPontoVenda;
        public virtual Int64? idPontoVenda
        {
            get => _idPontoVenda ?? Ponto_Venda?.Id;
            set { _idPontoVenda = value; }
        }
        public virtual Ponto_Venda Ponto_Venda { get; set; }

        public virtual DateTime? Atualizado_Em { get; set; }
        public virtual Double? Precisao { get; set; }
        public virtual Double? Bateria { get; set; }
        public virtual String Status_Gps { get; set; }
        public virtual String Dispositivo { get; set; }
        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }
    }
}
