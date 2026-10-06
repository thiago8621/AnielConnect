using Aniel.T5.Model.nhibernate;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.API_Aniel.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using NHibernate.Hql.Ast;
using System.Data.Common;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Repository.T5.Olt;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class OltApiController : BaseApiController
    {


        /// <summary>
        /// Listar OLTS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/OltApiController/GetAll")]
        public async  Task<GetApiRetorno<System.Collections.Generic.List<Aniel.Connect.Repository.Model.OltListModel>>> GetAll(OltRequest request)
        {
            GetApiRetorno<List<Aniel.Connect.Repository.Model.OltListModel>> retorno = new GetApiRetorno<System.Collections.Generic.List<Aniel.Connect.Repository.Model.OltListModel>>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }
                }
                var bind = BindToOltrequest(request);

                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IOltRepository repoOlt = new OltRepository(sessionNh);
                    retorno.Objeto = new OltBusiness(repoOlt).GetAll(bind).ToList();
                    retorno.Ok = true;
                    return retorno;
                }
            }
            catch (Exception erro)
            {

                retorno.InnerException = erro.InnerException.ToString();
                retorno.Mensagem = erro.InnerException.ToString();
                return retorno;
            }

        }
        private Aniel.Connect.Repository.Model.OLTModels BindOltModel(OLTModels request)
        {

            return new Aniel.Connect.Repository.Model.OLTModels
            {
                ID = request.ID,
                OLTID = request.OLTID,
                DESCRICAO = request.DESCRICAO,
                NUM_OLT = request.NUM_OLT,
                TIPO = request.TIPO,
                IP = request.IP,
                USUARIOCRIACAO = request.USUARIOCRIACAO,
                DATAHORACRIACAO = request.DATAHORACRIACAO,
                USUARIOALTERACAO = request.USUARIOALTERACAO,
                DATAHORAALTERACAO = request.DATAHORAALTERACAO,
                TOKEN = request.TOKEN,
                MARCA_MODELO = request.MARCA_MODELO,
                LOGIN = request.LOGIN,
                SENHA = request.SENHA,
                PORTA_TELNET = request.PORTA_TELNET,
                PORTA_API = request.PORTA_API,
                PORTA_HTTP = request.PORTA_HTTP,
                PORTA = request.PORTA,
                IP_TL1 = request.IP_TL1,
                LOGIN_TL1 = request.LOGIN_TL1,
                SENHA_TL1 = request.SENHA_TL1,
                GABINETE = request.GABINETE,
                SUBRACK = request.SUBRACK,
                STATUS = request.STATUS,
                DHCP_DNS_PRIMARIO = request.DHCP_DNS_PRIMARIO,
                DHCP_DNS_SECUNDARIO = request.DHCP_DNS_SECUNDARIO,
                DHCP_LAN_IP = request.DHCP_LAN_IP,
                DHCP_LAN_MASCARA = request.DHCP_LAN_MASCARA,
                DHCP_INICIO_RANGE = request.DHCP_INICIO_RANGE,
                DHCP_FINAL_RANGE = request.DHCP_FINAL_RANGE,
                TESTE_IP1 = request.TESTE_IP1,
                TESTE_IP2 = request.TESTE_IP2,
                TIPO_IDENTIFICACAO = request.TIPO_IDENTIFICACAO,
                ID_FILIAL = request.ID_FILIAL,
                GERENCIA_REDEGENERICA = request.GERENCIA_REDEGENERICA,
                GERENCIA_MASCARAREDEGENERICA = request.GERENCIA_MASCARAREDEGENERICA,
                GERENCIA_LOGIN = request.GERENCIA_LOGIN,
                GERENCIA_SENHA = request.GERENCIA_SENHA,
                GERENCIA_PORTA_GENERICA = request.GERENCIA_PORTA_GENERICA,
                GERENCIA_VLAN = request.GERENCIA_VLAN,
                GERENCIA_SINALMINIMO = request.GERENCIA_SINALMINIMO,
                GERENCIA_SINALMAXIMO = request.GERENCIA_SINALMAXIMO,
                ID_SERVIDOR_GERENCIA = request.ID_SERVIDOR_GERENCIA,
                ID_POP = request.ID_POP,
                UTILIZA_CONVERSAO_MAC = request.UTILIZA_CONVERSAO_MAC,
                CODIGO_AUXILIAR = request.CODIGO_AUXILIAR,
                VLAN_UPLINK = request.VLAN_UPLINK,
                DELAY = request.DELAY,
                GrupoAreaLista = request.GrupoAreaLista.Select(g => new Aniel.Connect.Repository.Model.OLT_GrupoAreaListModel
                {
                    Id_Area = g.Id_Area,
                    Cod_Area = g.Cod_Area,
                    Desc_Area = g.Desc_Area,
                    Cod_Escritorio = g.Cod_Escritorio,
                    Desc_Escritorio = g.Desc_Escritorio
                }).ToList()
            };
        }

        /// <summary>
        /// Cadastro de OLT
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>

        [HttpPost()]
        [Route("api/OLTApi/InsertOLT")]
        public async Task<PostApiRetorno> InsertOLT(OLTModels dados)
        {
            var retorno = new PostApiRetorno();
            try
            {
                if (!dados.ID_FILIAL.HasValue)
                {
                    throw new Exception("Campo ID_FILIAL é obrigatório");

                }
                if (String.IsNullOrEmpty(dados.DESCRICAO))
                {
                    throw new Exception("Campo Nome é obrigatório");

                }
                if (String.IsNullOrEmpty(dados.IP))
                {
                    throw new Exception("Campo IP é obrigatório");

                }

                if (!dados.ID_SERVIDOR_GERENCIA.HasValue)
                {
                    throw new Exception("Campo ID_SERVIDOR_GERENCIA é obrigatório");

                }
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(dados.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }
                }
                var bind = BindOltModel(dados);
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IOltRepository repoOlt = new OltRepository(sessionNh);
                    retorno = new OltBusiness(repoOlt).InsertOLT(BindOltModel(dados));
                    return retorno;
                }
            }
            catch (Exception erro)
            {
                retorno.InnerException = erro.Message;
                retorno.Mensagem = "erro";
                retorno.Ok = false;
                return retorno;
            }
        }
        private Aniel.Connect.Repository.Model.OltRequest BindToOltrequest(OltRequest request)
        {
            if (request == null) return null;

            return new Aniel.Connect.Repository.Model.OltRequest
            {
                ID = Convert.ToInt32(request.Id)

            };
        }

         /// <summary>
         /// Atualização de OLT
         /// </summary>
         /// <param name="dados"></param>
         /// <returns></returns>

        [HttpPost()]
        [Route("api/OLTApi/UpdateOLT")]
        public async Task<PostApiRetorno> UpdateOLT(OLTModels dados)
        {
            var retorno = new PostApiRetorno();
            try
            {
                if (!dados.ID_FILIAL.HasValue)
                {
                    throw new Exception("Campo ID_FILIAL é obrigatório");

                }
                if (String.IsNullOrEmpty(dados.DESCRICAO))
                {
                    throw new Exception("Campo Nome é obrigatório");

                }

                if (String.IsNullOrEmpty(dados.IP))
                {
                    throw new Exception("Campo IP é obrigatório");

                }

                if (!dados.ID_SERVIDOR_GERENCIA.HasValue)
                {
                    throw new Exception("Campo ID_SERVIDOR_GERENCIA é obrigatório");

                }

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(dados.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }
                }

                var bind = BindOltModel(dados);

                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IOltRepository repoOlt = new OltRepository(sessionNh);
                    retorno = new OltBusiness(repoOlt).UpdateOLT(BindOltModel(dados));
                    retorno.Ok = true;

                    return retorno;
                }

            }

            catch (Exception erro)
            {
                retorno.InnerException = erro.Message;
                retorno.Mensagem = "erro";
                retorno.Ok = false;
                return retorno;

            }

        }



    }

}