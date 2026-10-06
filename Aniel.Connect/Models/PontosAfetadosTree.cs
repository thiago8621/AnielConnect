using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class PontosAfetadosTree
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public Boolean children { get; set; }
        public Dictionary<string, object> li_attr { get; set; }
    }
}