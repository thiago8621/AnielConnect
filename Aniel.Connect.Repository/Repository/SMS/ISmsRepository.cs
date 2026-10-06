namespace Aniel.Connect.Repository.SMS
{
    public interface ISmsRepository
    {
        /// <summary>
        ///     Envia uma mensagem por SMS para o número especificado
        /// </summary>
        /// <param name="numero">Número de telefone que receberá o SMS</param>
        /// <param name="mensagem">Mensagem que será enviada</param>
        /// <exception cref="Aniel.Connect.Repository.Exceptions.IntegracaoSmsConnectionException">
        ///     Exceção que ocorre quando a requisição para o endpoint da integração de envio de SMS do API Aniel
        ///     retorna status 500
        /// </exception>
        /// <exception cref="Aniel.Connect.Repository.Exceptions.IntegracaoSmsException">
        ///     Exceção que ocorre quando há algum problema interno no endpoint da integração de envio de SMS do API Aniel
        ///     que faz o campo "ok" do retorno vir como "false"
        /// </exception>
        void EnviarSms(string numero, string mensagem);
    }
}
