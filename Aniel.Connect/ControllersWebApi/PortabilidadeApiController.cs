using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Newtonsoft.Json;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PortabilidadeApiController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        [Route("api/PortabilidadeApi/GetAll")]
        #region GetAll
        public async Task<DataTableResponse<Portabilidade_ListViewModel>> GetAll(Portabilidade_DatatableRequest request)
        {
            List<Portabilidade_ListViewModel> retorno = new List<Portabilidade_ListViewModel>();
            List<String> Condicoes = new List<String>();
            int records = 0;

            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                #region StrinbBuilderDefinition
                StringBuilder sql = new StringBuilder(),
                              sqlFrom = new StringBuilder(),
                              sqlWhere = new StringBuilder(),
                              sqlOrder = new StringBuilder(),
                              sqlCount = new StringBuilder();
                #endregion

                sqlCount.AppendLine("SELECT COUNT (A.CODIGO) COD FROM TB_PORTABILIDADE A");
                using (var readerCount = await CommandHelper.ExecuteReader(com, sqlCount, session.Connection, null))
                {
                    while (await readerCount.ReadAsync())
                    {
                        records = Convert.ToInt32(readerCount["COD"]);
                    }
                }

                #region filtros
                //if (!String.IsNullOrEmpty(request.Cliente)) { Condicoes.Add(""); }
                //if (!String.IsNullOrEmpty(request.DID)) { Condicoes.Add(""); }
                //if (!String.IsNullOrEmpty(request.Numero_Circuito)) { Condicoes.Add(""); }
                //if (!String.IsNullOrEmpty(request.Status)) { Condicoes.Add(""); }
                if (Condicoes.Count > 0) { sqlWhere.AppendLine($"WHERE{String.Join("AND", Condicoes)}"); }
                #endregion
                //A.NUMERO_CIRCUITO,
                sql.AppendLine("SELECT A.CODIGO, A.ID_CLIENTE, A.UF, A.CIDADE, A.DIA_PORTABILIDADE, A.NUMERO_DID_PORTABILIDADE, A.NUM_SIP ");
                sqlFrom.AppendLine("FROM TB_PORTABILIDADE A");
                sqlOrder.AppendLine("ORDER BY A.CODIGO");
                sql.AppendLine(sqlFrom.ToString() + " \r\n " + sqlWhere.ToString() + " \r\n " + sqlOrder.ToString());
                using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var obj = new Portabilidade_ListViewModel
                        {
                            Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : String.Empty,
                            Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["ID_CLIENTE"]) : String.Empty,
                            Codigo = reader["CODIGO"] != DBNull.Value ? Convert.ToString(reader["CODIGO"]) : String.Empty,
                            Dia_Previsto_Portabilidade = reader["DIA_PORTABILIDADE"] != DBNull.Value ? Convert.ToString(reader["DIA_PORTABILIDADE"]) : String.Empty,
                            Did = reader["NUMERO_DID_PORTABILIDADE"] != DBNull.Value ? Convert.ToString(reader["NUMERO_DID_PORTABILIDADE"]) : String.Empty,
                            Numero_Circuito = reader["NUM_SIP"] != DBNull.Value ? Convert.ToString(reader["NUM_SIP"]) : String.Empty,
                            UF = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : String.Empty,
                        };
                        retorno.Add(obj);
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Portabilidade_ListViewModel>
            {
                draw = request.draw,
                //recordsTotal = ,
                recordsFiltered = records,
                data = retorno,
                error = ""
            };
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PortabilidadeApiController/Gravar")]
        #region Gravar
        public async Task<PostApiRetorno<long>> Gravar(ParamWebApi<Portabilidade_CadastroViewModel> value)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                Portabilidade port = null;
                using (var session = HibernateUtil.GetSession())
                using (var repoPortabilidade = new Conexao<Portabilidade>(session))
                {
                    if (port == null)
                    {
                        port = new Portabilidade();
                    }
                    else
                    {
                        port = await repoPortabilidade.selectFirst(p => p.Codigo == value.Objeto.Codigo);
                    }

                    BindViewToObj(session, port, value.Objeto);


                    if (port.Status == Model.enumerado.Status_Portabilidade.Portado)
                    {
                        GetApiRetorno<Did> DidRetornado = await Portabilidade_CriacaoDid(session, port.Did_Substituido, port.NumeroDid_Portabilidade);


                        if (DidRetornado.Ok)
                        {
                            //Daqui a rotina externa precisa ser executada

                            bool response = await Portabilidade_MudancasExternas(session, port.CircuitoSelecionado, DidRetornado.Objeto);
                            if (response)
                            {
                                //Daqui as rotinas Internas precisam ser executadas
                                await Portabilidade_MudancasInternas(session, port.CircuitoSelecionado, port.Did_Substituido, DidRetornado.Objeto);

                                bool success = true;
                            }
                        }
                    }

                }
                retorno.IdObjeto = port.Codigo;
                retorno.Ok = true;
            }

            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        #region GetViewObj
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private Portabilidade_CadastroViewModel GetViewObj(Portabilidade port)
        {
            return new Portabilidade_CadastroViewModel
            {
                Cidade = port.Cidade,
                Cliente = port.Cliente.Id,
                Codigo = port.Codigo,
                Did = port.NumeroDid_Portabilidade,
                Numero_Sip = Convert.ToInt64(port.Numero_Sip),
                Pais = port.Pais,
                Status = port.Status,
                UF = port.UF,
                Dia_Previsto_Portabilidade = port.Dia_Previsto_Portabilidade.HasValue ? port.Dia_Previsto_Portabilidade.Value.ToString("dd/MM/yyyy") : null,
            };
        }
        #endregion

        #region BindView
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objBanco"></param>
        /// <param name="view"></param>
        private async Task BindViewToObj(ISession session, Portabilidade objBanco, Portabilidade_CadastroViewModel view)
        {
            using (Conexao<Did> repoDid = new Conexao<Did>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (Conexao<Portabilidade> repoPortabilidade = new Conexao<Portabilidade>(session))
            {
                objBanco.Dia_Previsto_Portabilidade = Convert.ToDateTime(String.Format(view.Dia_Previsto_Portabilidade, "dd/MM/yyyy"));
                objBanco.NumeroDid_Portabilidade = Convert.ToString(view.Did);
                objBanco.Numero_Sip = Convert.ToString(view.Numero_Sip);
                objBanco.Status = view.Status;

                //Localização
                objBanco.Pais = view.Pais;
                objBanco.Cidade = view.Cidade;
                objBanco.UF = view.UF;

                objBanco.Cliente = (await repoCliente.selectFirst(p => p.Id == view.Cliente));
                objBanco.Did_Substituido = repoDid.selectFirst(p => p.Id == view.Did_Substituido).Result;
                //objBanco.Did = await repoDid.selectFirst(p => p.Id == Convert.ToInt64(view.Did));
                await repoPortabilidade.SaveOrUpdate(objBanco);
            }

        }
        #endregion



        /// <summary>
        /// Tarefa de Portar novo Número para Circuito atual da portabilidade Parte1
        /// </summary>
        /// <param name="session"></param>
        /// <param name="did">did que será desvinculado do circuito</param>
        /// <param name="futuroDid">deste número virá um novo did, que será vinculado ao circuito</param>
        /// <returns></returns>
        public async Task<GetApiRetorno<Did>> Portabilidade_CriacaoDid(ISession session, Did did, String futuroDid)
        {
            var response = new GetApiRetorno<Did>();
            Did ObjetoCriado = null;
            try
            {
                using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repoDid = new Conexao<Did>(session))
                //using (var DidApi = new ())
                using (var DidApi = new DidApiController())
                {
                    ObjetoCriado = await repoDid.selectFirst(p => p.Numero == futuroDid);
                    if (ObjetoCriado == null)
                    {
                        var newDid = new DidCadastroModel
                        {
                            Numero = futuroDid,
                            ChamadaCobrar = did.ChamadaCobrar,
                            IdCidade = did.Cidade.Id,
                            IdPais = did.Pais.Id,
                            IdRota = did.RotaTelefonia.Id,
                            Id_Venda = null,
                            MotivoIndisponivel = did.MotivoIndisponivel,
                            StatusDid = Model.enumerado.StatusDid.AguardandoPortabilidade,
                            Reservado_Ddr = did.Reservado_Ddr == true ? Padrao.Sim : Padrao.Nao,
                            TipoDiscagem = did.TipoDiscagem,
                            UF = did.UF,
                            Valor = String.Format("{0:N2}", did.Valor),
                        };
                        ParamWebApi<DidCadastroModel> Params = new ParamWebApi<DidCadastroModel> { Token = "", NomeUsuario = "", Usuario = "", Objeto = newDid };
                        PostApiRetorno<long> testeApi = (await DidApi.Gravar(Params));
                        if (testeApi.Ok)
                        {
                            ObjetoCriado = await repoDid.selectFirst(p => p.Id == testeApi.IdObjeto);
                            response.Objeto = ObjetoCriado;
                        }
                        else
                        {

                        }
                    }

                }
            }
            catch (Exception erro)
            {
                response.Ok = false;
                response.Mensagem = erro.Message;
            }
            return response;
        }

        /// <summary>
        ///  Tarefa de Portar novo Número para Circuito atual da portabilidade Parte2 : Fatores Externos da Portabilidade
        /// </summary>
        /// <param name="session"></param>
        /// <param name="circuito">Circuito Selecionado na Portabilidade</param>
        /// <param name="Novodid"></param>
        /// <returns></returns>
        public async Task<Boolean> Portabilidade_MudancasExternas(ISession session, CircuitoTelefonia circuito, Did Novodid)
        {
            bool success = false;
            try
            {

                using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repoDid = new Conexao<Did>(session))
                using (var DidApi = new DidApiController())
                {
                    bool conditionSaperX = false;
                    if (conditionSaperX)
                    {
                        success = true;
                    }

                }
            }
            catch (Exception erro)
            {
                string Message = erro.Message;
                throw erro;
            }

            return success;
        }
        ///  <summary>Tarefa de Portar novo Número para Circuito atual da portabilidade Parte3 : Fatores Internos da Portabilidade</summary>>
        ///  <param name="session"></param>  
        ///  <param name="circuito"></param>
        ///  <param name="Did_Antigo"></param> 
        ///  <param name="Novodid"></param>
        public async Task Portabilidade_MudancasInternas(ISession session, CircuitoTelefonia circuito, Did Did_Antigo, Did Novodid)
        {

            using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
            using (var repoDid = new Conexao<Did>(session))
            using (var DidApi = new DidApiController())
            {
                Novodid.StatusDid = Did_Antigo.StatusDid;
                circuito.Did.Add(Novodid);
                circuito.Did.Remove(Did_Antigo);
                await repoCircuito.SaveOrUpdate(circuito);

            }

        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PortabilidadeApiController/FindById")]
        #region FindById
        public async Task<GetApiRetorno<Portabilidade_CadastroViewModel>> FindById(long? Id)
        {
            var retorno = new GetApiRetorno<Portabilidade_CadastroViewModel>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoPort = new Conexao<Portabilidade>(session))
                {
                    Portabilidade_CadastroViewModel viewObj = GetViewObj(await repoPort.selectFirst(p => p.Codigo == Id));
                    retorno.Objeto = viewObj;
                }

                retorno.Ok = true;
                retorno.Objeto = null;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PortabilidadeApiController/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir(ExcluirListaViewModel Value)
        {
            var retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Portabilidade> repoPort = new Conexao<Portabilidade>(session))
                {

                    IList<Portabilidade> port = await repoPort.select(p => Value.ids.Contains(p.Codigo));
                    if (port == null)
                    {
                        throw new Exception("Registro não Encontrado");
                    }
                    //retorno.IdObjeto = port.Codigo;
                    await repoPort.Remove(port);
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion
    }
}