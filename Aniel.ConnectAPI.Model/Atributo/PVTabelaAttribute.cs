using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Atributo
{
    public class PVTabelaAttribute : Attribute
    {
        private String _tabela;

        public PVTabelaAttribute(String Tabela)
        {
            _tabela = Tabela;
        }

        public String GetTabela()
        {
            return _tabela;
        }
    }
}
