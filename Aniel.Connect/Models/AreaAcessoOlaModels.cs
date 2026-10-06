using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class AreaAcessoOlaModel
    {
        public virtual long Id { get; set; }
        public virtual string Cod_Area { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Observacao { get; set; }
        public virtual bool Ocultar_Acesso { get; set; }
    }

    public class AreaAcessoOlaDataTableRequest : DataTableRequest
    {
        public virtual int? Status_Visibilidade { get; set; }
    }

    public class AlterarVisibilidadeModel
    {
        public virtual long[] ids { get; set; }
        public virtual int ocultar { get; set; }
    }
}