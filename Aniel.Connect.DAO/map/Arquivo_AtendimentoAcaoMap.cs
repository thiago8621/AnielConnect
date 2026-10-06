using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Arquivo_AtendimentoAcaoMap : ClassMapBase<Arquivo_AtendimentoAcao>
    {
        public Arquivo_AtendimentoAcaoMap() : base()
        {
            Table("TB_ARQUIVO_ATENDIMENTO_ACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ARQUIVO_ATENDIMENTO_ACAO");

            Map(X => X.Arquivo, "ARQUIVO");
            Map(X => X.Extensao, "EXTENSAO");

            References(x => x.Atendimento_Acao, "ID_ATENDIMENTO_ACAO").ForeignKey("FK_ATENDIMENTO_ACAO");
            

        }
    }
}
