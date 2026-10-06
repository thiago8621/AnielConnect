using Sinapse.Core;
using System;

namespace Aniel.T5.Model
{
    [Serializable]
    [TokenLess]
    public class Tb_Config_Url
    {
        public virtual String Url { get; set; }

        public virtual String Sistema { get; set; }

    }
}
