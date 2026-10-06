using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
   //[Obsolete("Usar a Model de mesmo nome do Core.")]
   // public class AutorizarONUListModel
   // {
   //     public string Id { get; set; }
   //     public string Desc_OLT { get; set; }
   //     public string Desc_Slot { get; set; }
   //     public string Desc_PON { get; set; }
   //     public string OLT { get; set; }
   //     public string Slot { get; set; }
   //     public string PON { get; set; }
   //     public string Mac_Serial { get; set; }
   //     public string Posicao_Estoque { get; set; }
   //     public string Informacao { get; set; }
   //     public string Desc_ONU { get; set; }
   //     public bool Pode_Autorizar_Sem_OS { get; set; }
   //     public long Id_Cliente_Plano_Produto { get; set; }
   // }

    public class AutorizarONURequest
    {
        public List<Sinapse.Integracao.Ativacao.Models.AutorizarONUListModel> ListONU { get; set; }
    }

    public class AutorizarONUException : Exception
    {
        public AutorizarONUException()
        {

        }

        public AutorizarONUException(string message) : base(message)
        {

        }

        public AutorizarONUException(string message, System.Exception inner) : base(message, inner)
        {

        }
    }
}