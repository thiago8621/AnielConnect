using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Resources;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Comercial_SMSController: BaseController
    {
        public Comercial_SMSController() :base("Comercial_SMS", CONSTANTES.URLPortalInterno)
        {
            
        }

        public async Task<ViewResult> Formulario(long cliente, string numero) 
        {
            var model = new Comercial_SMSModel();
            //Gravação da validação do número
            using (var apiCliente = new ClienteApiController())
            {
                var validacao = new ValidaNumeroRequest { Cliente = cliente, NumeroCelular = numero };
                await apiCliente.ValidaNumero(validacao);

                //Retorno para a view
                using (var apiConfig = new Configuracoes_GeraisApiController())
                {
                    var config = (await apiConfig.FindById(1)).Objeto;
                    var recordCliente = (await apiCliente.FindById(cliente)).Objeto;

                    model.Empresa = config.Nome_Cliente_Sinapse;
                    model.Mensagem = RsCadastros.MENSAGEM_AGRADECIMENTO_SMS;
                    model.UrlLogo = config.OLA_URL_Logo;
                    model.NumeroValidado = String.IsNullOrEmpty(recordCliente.NumeroValidado) ? false : true;
                }                
            }
            return View(model);
        }
    }
}
