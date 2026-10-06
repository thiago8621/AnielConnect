using Sinapse.Model.enumerados;

namespace Aniel.Connect.Repository.Model
{
    public class IntegracaoEnviarSmsRequest
    {
        public string Mensagem { get; set; }
        public string Num_Celular { get; set; }
        public Tipo_Integracao_SMS Id_Integracao { get; set; }
        public IntegracaoEnviarSmsSettings settings { get; set; }
    }

    public class IntegracaoEnviarSmsSettings
    {
        public string user { get; set; }
        public string password { get; set; }
        public string token { get; set; }
    }

    public class IntegracaoEnviarSmsResponse
    {
        public bool ok { get; set; }
        public string error { get; set; }
        public Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel data { get; set; }
    }
}
