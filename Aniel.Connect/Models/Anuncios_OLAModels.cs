using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class Anuncios_OLAModel
    {

        public virtual String urlImagemAnuncio { get; set; }
        public virtual Int64 Id { get; set; }
        public virtual String descricaoAnuncio { get; set; }
        public virtual byte[] imagemAnuncio { get; set; }
        public virtual String urlDestino { get; set; }
        public virtual Int64? Id_Configuracoes_Gerais { get; set; }

        public virtual string imagemAnuncioBase64 { get; set; }

    }

    public class Anuncio_OLAResponse
    {
        public virtual List<Anuncios_OLAModel> anuncios { get; set; }
    }


    public class Anuncios_OLACadastroModel : Sinapse.Model.nhibernate.IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String urlImagemAnuncio { get; set; }
        public virtual String descricaoAnuncio { get; set; }
        public virtual byte[] imagemAnuncio { get; set; }
        public virtual String urlDestino { get; set; }
        public virtual Int64? Id_Configuracoes_Gerais { get; set; }


    }

    public class Anuncios_OLAConfiguracoesGeraisModel
    {

        public virtual String urlImagemAnuncio { get; set; }
        public virtual Int64 Id { get; set; }
        public virtual String descricaoAnuncio { get; set; }
        public virtual String urlDestino { get; set; }
        public virtual Int64? Id_Configuracoes_Gerais { get; set; }

        public virtual string imagemAnuncioBase64 { get; set; }

    }

}