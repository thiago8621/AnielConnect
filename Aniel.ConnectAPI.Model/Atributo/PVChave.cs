using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Atributo
{
    public class PVChave : Attribute
    {
        private String _field;

        public PVChave(String Field)
        {
            _field = Field;
        }

        public String GetField()
        {
            return _field;
        }
    }
}
