using System.Collections.Generic;

namespace Aniel.Connect.Models.T5
{
    public class ManutRequest
    {
        public int start { get; set; }
        public int length { get; set; }
        public int recordsTotal { get; set; }
        public string pesquisa { get; set; }
    }
    
    public class ManutResponse<T>
    {
        public T IdObjeto { get; set; }
        public bool Ok { get; set; }
        public string Mensagem { get; set; }
        public long? recordsTotal { get; set; }
        public long? recordsClientes { get; set; }

    }
}