using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;

namespace Aniel.Connect.DAO.map
{
    public class Anuncios_OLAMap : ClassMapBase<Anuncios_OLA>
    {
        public Anuncios_OLAMap() : base()
        {
            Table("TB_ANUNCIOS_OLA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ANUNCIO_OLA");

            Map(x => x.imagemAnuncio, "URL_IMAGEM_ANUNCIO");
            Map(x => x.urlDestino, "URL_DESTINO").Length(5000);
            Map(x => x.descricaoAnuncio, "DESCRICAO").Length(100);
            Map(x => x.Id_Configuracoes_Gerais, "ID_CONFIGURACOES_GERAIS");
        }
    }
}
