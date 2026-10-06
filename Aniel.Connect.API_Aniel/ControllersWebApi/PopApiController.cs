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
using System.Data.Common;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Models;
using Sinapse.Model;
using System.Linq.Expressions;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class PopApiController : BaseApiController
    {
        /// <summary>
        /// Listar Pop
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ID_CIDADE Valor deve ser obtido pelo endpoint CIdade </returns>
        [HttpPost()]
        [Route("api/PopApiController/GetAll")]
        public async Task<GetApiRetorno<List<Aniel.Connect.Repository.Model.PopModelsResponse>>> GetAll(ApiRequest request)
        {
            List<PopModelsResponse> retorno = new List<PopModelsResponse>();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    IPopRepository repoPop = new PopRepository(sessionNh);
                    return new GetApiRetorno<List<Aniel.Connect.Repository.Model.PopModelsResponse>>
                    {
                        Objeto = new PopBusiness(repoPop).GetAll().ToList(),
                        Ok = true
                    };
                        

                }
            }
            catch (Exception erro)
            {
                return new GetApiRetorno<List<Aniel.Connect.Repository.Model.PopModelsResponse>>
                {
                    Mensagem = erro.Message,
                    Ok = false,
                    Objeto = null,
                    InnerException = erro.InnerException != null ? erro.InnerException.Message : null
                };
            }

        }

        /// <summary>
        /// Inserir Pop - Campo ID_CIDADE pode ser visto no endPOint /api/CidadeApi/GetAll 
        /// </summary>
        [HttpPost()]
        [Route("api/PopApiController/InsertPop")]
        public async Task<PostApiRetorno> InsertPop(PopModels dados)
        {
            var retorno = new PostApiRetorno();
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(dados.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }
            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                try

                {

                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoEmpresa = new Conexao<Empresa>(session))
                    {
                        var lista = await repoEmpresa.queryover().ListAsync();

                        var listaFiltrada = lista.Where(a => a.Id == dados.ID_EMPRESA && !String.IsNullOrWhiteSpace(a.Nome)).ToList();
                        if (listaFiltrada.Count == 0)
                        {
                            retorno.Mensagem = "Empresa não localizada";
                            retorno.Ok = false;
                            return retorno;
                        }

                    }
                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoCidade = new Conexao<Cidade>(session))
                    {
                        var lista = await repoCidade.queryover().ListAsync();
                        var listaFiltrada = lista.Where(a => a.Id == dados.ID_CIDADE).ToList();
                        if (listaFiltrada.Count == 0)
                        {
                            retorno.Mensagem = "Cidade não localizada";
                            retorno.Ok = false;
                            return retorno;
                        }
                        else
                        {
                            dados.Cidade = listaFiltrada.FirstOrDefault().Descricao;
                        }

                    }
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    {

                        IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5);
                        IPopRepository repoPop = new PopRepository(sessionNh);


                        retorno = new PopBusiness(repoPop).AdicionarPop(BindToPopApiModel(dados), repoContrato);
                    }
                    return retorno;

                }
                catch (Exception erro)
                {
                    retorno.InnerException = erro.InnerException.ToString();
                    retorno.Mensagem = erro.InnerException.ToString();

                    retorno.Ok = false;
                    return retorno;
                }
            }
        }



        /// <summary>
        /// Atualizar Pop Campo ID_CIDADE pode ser visto no endPOint /api/CidadeApi/GetAll 
        /// </summary>
        [HttpPost()]
        [Route("api/PopApiController/UpdatePop")]
        public async Task<PostApiRetorno> UpdatePop(PopModels dados)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(dados.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }
            var retorno = new PostApiRetorno();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoEmpresa = new Conexao<Empresa>(session))
            {
                var lista = await repoEmpresa.queryover().ListAsync();
                var listaFiltrada = lista.Where(a => a.Id == dados.ID_EMPRESA && !String.IsNullOrWhiteSpace(a.Nome)).ToList();
                if (listaFiltrada.Count == 0)
                {
                    retorno.Mensagem = "Empresa não localizada";
                    retorno.Ok = false;
                    return retorno;
                }
               
            }
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCidade = new Conexao<Cidade>(session))
            {
                var lista = await repoCidade.queryover().ListAsync();
                var listaFiltrada = lista.Where(a => a.Id == dados.ID_CIDADE).ToList();
                if (listaFiltrada.Count == 0)
                {
                    retorno.Mensagem = "Cidade não localizada";
                    retorno.Ok = false;
                    return retorno;
                }
                else
                {
                    dados.Cidade = listaFiltrada.FirstOrDefault().Descricao;
                }
            }
            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                try
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    {

                        IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5);
                        IPopRepository repoPop = new PopRepository(sessionT5);
                        return new PopBusiness(repoPop).UpdatePop(BindToPopApiModel(dados), repoContrato);
                    }
                }
                catch (Exception erro)
                {
                    retorno.InnerException = erro.InnerException.ToString();
                    retorno.Mensagem = erro.InnerException.ToString();
                    retorno.Ok = false;
                    return retorno;
                }
            }
        }
        private Aniel.Connect.Repository.Model.PopModels BindToPopApiModel(PopModels request)
        {

            if (request == null) return null;

            return new Repository.Model.PopModels
            {
                ID = request.ID,
                TOKEN = request.TOKEN,
                NOME = request.NOME,
                CODCT = request.CODCT,
                PROJETO = request.PROJETO,
                CEP = request.CEP,
                ENDERECO = request.ENDERECO,
                NUMERO = request.NUMERO,
                BAIRRO = request.BAIRRO,
                ID_CIDADE = request.ID_CIDADE,
                UF = request.UF,
                COMPLEMENTO = request.COMPLEMENTO,
                LATITUDE = request.LATITUDE,
                LONGITUDE = request.LONGITUDE,
                ID_EMPRESA = request.ID_EMPRESA,
                ID_FILIAL = request.ID_FILIAL,
                STATUS = request.STATUS,
                DESCRICAO = request.DESCRICAO,
                CODIGO = request.CODIGO,
                CIDADE = request.Cidade
            };
        }
    }
}
