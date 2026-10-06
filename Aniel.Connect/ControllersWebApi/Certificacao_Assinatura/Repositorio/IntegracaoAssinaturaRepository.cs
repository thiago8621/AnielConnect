using Aniel.Connect.ControllersWebApi.Certificacao_Assinatura.Interfaces;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.Certificação_Assinatura;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;

namespace Aniel.Connect.ControllersWebApi.Certificacao_Assinatura.Repositorio
{
    public class IntegracaoAssinaturaRepository
    {
        public Consultar_Documento_ResponseModel Consultar_Documento(IntegracaoAssinatura integracao, ISession session, Configuracoes_Gerais configEmpresa, Venda venda)
        {
            return integracao.Consultar_Documento(session, configEmpresa, venda);
        }
        public Enviar_Documento_ResponseModel Enviar_Documento(IntegracaoAssinatura integracao, ISession session, Configuracoes_Gerais configEmpresa, Venda venda, Cliente cliente, String termo_adesao_base64)
        {
            return integracao.Enviar_Documento(session, configEmpresa, venda, cliente, termo_adesao_base64);
        }
        public String Get_Documento_Pdf(IntegracaoAssinatura integracao, Configuracoes_Gerais configEmpresa, Venda venda, String url_pdf)
        {
            return integracao.Get_Documento_Pdf(configEmpresa, venda, url_pdf);
        }
    }
}