using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    //Ao implementar outros recursos que utilizem o Cloud Storage, adicionar um nome para o Bucket no Enumerado
    //No padrão: "nome-sistema-nome-do-recurso"
    //Pois o bucket deve ter um nome globalmente exclusivo
    public static class CloudStorageBucketEnum
    {
        public const string SERVICO_CHAT_ANEXO = "aniel-connect-servico-chat-anexo";
    }
}
