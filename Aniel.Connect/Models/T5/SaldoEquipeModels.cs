using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class SaldoEquipeModel
    {
        public SaldoEquipeModel()
        {
            TOTENT = 0;
            TOTSAI = 0;
            TOTAPLIC = 0;
            TOTREMOV = 0;
            SALDO = 0;
        }

        public String EQUIPE { get; set; }
        public String CODMAT { get; set; }
        public String CODCPL { get; set; }
        public Double? TOTENT { get; set; }
        public Double? TOTSAI { get; set; }
        public Double? TOTAPLIC { get; set; }
        public Double? TOTREMOV { get; set; }
        public Double? SALDO { get; set; }
    }

}