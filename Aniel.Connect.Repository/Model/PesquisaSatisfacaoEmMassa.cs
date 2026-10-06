using System.Collections.Generic;
using System;

namespace Aniel.Connect.Repository.Model
{
    public class PesquisaSatisfacaoEmMassa
    {
        public List<Int64> ids { get; set; }
        public List<Int64> ids_pesquisa { get; set; }

        public String usuario { get; set; }
    }
}
