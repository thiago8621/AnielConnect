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
    public class Pesquisa_SatisfacaoMap : ClassMapBase<Pesquisa_Satisfacao>
    {
        public Pesquisa_SatisfacaoMap() : base()
        {

            Table("TB_PESQUISA_SATISFACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PESQUISA_SATISFACAO");

            Map(x => x.Titulo_Da_Pesquisa, "TITULO_DA_PESQUISA").Length(200);

            Map(x => x.Descricao, "DESCRICAO").Length(500);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.DisparoAuto, "DISPAROAUTO").CustomType<PesquisaSatisfacaoDisparoAuto>();

            Map(x => x.QuemPreenche, "QUEMPREENCHE").CustomType<PesquisaSatisfacaoQuemPreenche>();

            HasMany(x => x.Perguntas).KeyColumn("ID_PESQUISA_SATISFACAO").Cascade.All().Inverse();

            HasMany(x => x.Respostas).KeyColumn("ID_PESQUISA_SATISFACAO").Cascade.All().Inverse();

            Map(x => x.Tem_Caixa_Texto, "TEM_CAIXA_TEXTO");
           
            Map(x => x.Envia_Msg_Emai_Sms, "ENVIAR_MSG_EMAIL_SMS");
            
            Map(x => x.Modelo_Envio, "MODELO_ENVIO");

            Map(x => x.TipoCaixaTexto, "TIPO_CAIXA_TEXTO").CustomType<PesquisaSatisfacaoTipoCaixaTexto>();
        }
    }
}