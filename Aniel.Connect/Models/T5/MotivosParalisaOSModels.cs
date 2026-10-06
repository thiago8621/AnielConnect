using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class MotivosParalisaOSModels
    {
        public virtual Int64 Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
    }
}