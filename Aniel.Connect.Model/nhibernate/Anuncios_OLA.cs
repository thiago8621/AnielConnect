using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Anuncios_OLA : BaseModel
    {
        public virtual byte[] imagemAnuncio { get; set; }
        public virtual String urlDestino { get; set; }
        public virtual String descricaoAnuncio { get; set; }
        public virtual long? Id_Configuracoes_Gerais { get; set; }
        public virtual String urlImagemAnuncio { get; set; }
    }
}
