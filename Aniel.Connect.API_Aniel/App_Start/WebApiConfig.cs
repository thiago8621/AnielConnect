using Aniel.Connect.API_Aniel;
using Aniel.Connect.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiThrottle;

namespace AnielConnectIntegracoes
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Serviços e configuração da API da Web

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Rotas da API da Web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy(perSecond: CONSTANTES.PorSegundo ?? 5, perMinute: CONSTANTES.PorMinuto ?? 120,
                                            perHour: CONSTANTES.PorHora ?? 3600, perDay: CONSTANTES.PorDia ?? 28800)
                {
                    EndpointThrottling = true,
                    ClientThrottling = CONSTANTES.ClienteRateLimit ?? false,
                    EndpointRules = CONSTANTES.CarregarEndpointRules(),
                    IpWhitelist = CONSTANTES.IpWhitelist.Count == 0 ?  new List<string> { "::1" } : CONSTANTES.IpWhitelist
                },
                Repository = new CacheRepository(),
                QuotaExceededMessage = "Limite de requisições excedido, tente novamente mais tarde."
            });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Redirect root to Swagger UI
            //config.Routes.MapHttpRoute(
            //    name: "Swagger UI",
            //    routeTemplate: "",
            //    defaults: null,
            //    constraints: null,
            //    handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "swagger/ui/index"));

            config.Filters.Add(new ExceptionHandlingAttribute());

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
        }
    }
}
