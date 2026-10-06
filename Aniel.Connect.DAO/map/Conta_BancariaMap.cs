using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.DAO.map
{
    public class Conta_BancariaMap : ClassMapBase<Conta_Bancaria>
    {

        public Conta_BancariaMap() : base()
        {
            Table("TB_CONTA_BANCARIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONTA_BANCARIA");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Agencia, "AGENCIA").Length(6);
            Map(x => x.Digito_Agencia, "DIGITO_AGENCIA").Length(2);

            Map(x => x.Numero_Conta, "NUMERO_CONTA").Length(20);
            Map(x => x.Digito_Conta, "DIGITO_CONTA").Length(2);
            Map(x => x.NossoNumero, "NOSSO_NUMERO");

            Map(x => x.Gerente_Conta, "GERENTE_CONTA").Length(50);
            Map(x => x.Telefone_Agencia, "TELEFONE_AGENCIA").Length(18);     
            Map(x => x.Saldo_Inicial, "SALDO_INICIAL");
            Map(x => x.Data_Saldo, "DATA_SALDO"); 
            Map(x => x.Convenio, "CONVENIO"); 

            Map(x => x.Rateio, "RATEIO");
            
            Map(x => x.Observacoes, "OBSERVACOES");

            Map(x => x.Nome_Conta, "NOME_CONTA").Length(50);

            //References(x => x.Grupo_Rateio_Filiais, "ID_GRUPO_RATEIO");
            References(x => x.Empresa, "ID_EMPRESA").ForeignKey("FK_CB_EMPRESA");
            References(x => x.Filial, "ID_FILIAL").ForeignKey("FK_CB_FILIAL");
            References(x => x.Banco, "ID_BANCO").ForeignKey("FK_CB_BANCO");

           
        }
    }
}
