
using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Resources;
using Sinapse.Model.menus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect
{
    public static class MENU
    {
        public static IEnumerable<IMenu_IFrame> GetMenus(String usuario)
        {
            List<IMenu_IFrame> retorno = new List<IMenu_IFrame>();

            //retorno.Add(new Title_IFrame
            //{
            //    id = 1,
            //    text = "Menus",
            //    icon = String.Empty,
            //    isHeader = true
            //});

            retorno.Add(new Menu_IFrame
            {
                id = 1,
                text = " " + RsCadastros.INICIO,
                icon = "la la-home la-1-4x",
                url = GetUrlHome(usuario).Result,
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = null
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.COMERCIAL,
                icon = "la la-briefcase la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Comercial/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "121"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.CLIENTE,
                icon = "la la-user la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Cliente/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "22"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.ATENDIMENTO,
                icon = "la la-headphones la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Atendimento/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "04"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.GESTAO_EQUIPES,
                icon = "la la-users la-1-4x", 
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Gestao_Equipe/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "124"
            });

            retorno.Add(new Menu_IFrame
            {
                /*id = 2, card #136 */

                text = " " + RsCadastros.ROTEIRIZACAO,
                icon = "la la-map-marker la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Painel_Roteirizacao/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "128"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.RELATORIO,
                icon = "la la-copy la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Relatorio/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "126"
            });

            //retorno.Add(new Menu_IFrame
            //{
            //    text = " Indicadores",
            //    icon = "la la-bar-chart la-2x",
            //    url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Indicador/Index"),
            //    urlType = "absolute",
            //    targetType = "iframe-tab",
            //    cod_area = "127"
            //});

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.GESTAO_CONEXOES,
                icon = "icofont icofont-network-tower la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Gerenciamento_Rede/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "149"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.GEOGRAPHY,
                icon = "fas fa-sitemap",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Projeto_Rede/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "293"
            });

            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.CONFIGURACOES,
                icon = "la la-cog la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Configuracao/Index"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "125"
            });
           
            retorno.Add(new Menu_IFrame
            {
                text = " " + RsCadastros.AJUDA,
                icon = "la la-question-circle la-1-4x",
                url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Ajuda/Desenvolvimento"),
                urlType = "absolute",
                targetType = "iframe-tab",
                cod_area = "129"
            });

          

            return retorno;
        }

        private static async Task<String> GetUrlHome(String usuario)
        {
            var pagina = await DashBoardService.GetPaginaInicial(usuario);

            switch (pagina)
            {
                case Sinapse.Model.enumerados.DashboardInicialConnect.AtendimentoCliente:
                    return String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Dashboard/Atendimento");
                case Sinapse.Model.enumerados.DashboardInicialConnect.Comercial:
                    return String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Dashboard/Vendas");
                case Sinapse.Model.enumerados.DashboardInicialConnect.Servicos:
                    return String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Dashboard/Servicos");
                default:
                    return String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Home/Index");
            }

        }

    }
}