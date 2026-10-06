using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.Certificação_Assinatura;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.ControllersWebApi.Certificacao_Assinatura.Interfaces
{
    public interface IntegracaoAssinatura
    {
        Consultar_Documento_ResponseModel Consultar_Documento(ISession session, Configuracoes_Gerais configEmpresa, Venda venda);
        Enviar_Documento_ResponseModel Enviar_Documento(ISession session, Configuracoes_Gerais configEmpresa, Venda venda, Cliente cliente, String termo_adesao_base64);
        Task Assinar_Documento(ISession session, Configuracoes_Gerais configEmpresa, Venda venda, Cliente cliente);
        String Get_Documento_Pdf(Configuracoes_Gerais configEmpresa, Venda venda, String url_pdf);        
    }
}