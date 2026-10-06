using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PerfilVisualizacaoVenda;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamTipoPessoa;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamMeioCaptacao;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamCategoriaCliente;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamSetorDepto;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamRegiao;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamEquipeVenda;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.Funcionario;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.T5.Projeto;
using Aniel.Connect.Repository.T5.Setor;
using Aniel.Connect.Repository.T5.Usuario;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.EquipeVendas;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PerfilVisualizacaoVendaApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/PerfilVisualizacaoVendaApi/GetAll")]
        public async Task<DataTableResponse<PerfilVisualizacaoVendaListModel>> GetAll(DataTableRequest request)
        {
            int recordsTotal = 0;
            var retorno = new List<PerfilVisualizacaoVendaListModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                var sqlFrom = new StringBuilder();
                var param = new Dictionary<string, object>();

                sqlFrom.AppendLine("FROM TB_PERFIL_VIS_VENDA PV");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("WHERE UPPER(PV.DESCRICAO) LIKE UPPER(@SEARCH)");
                    param.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(*) AS QTD_PERFIS");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["QTD_PERFIS"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                { 
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT PV.ID, PV.DESCRICAO, PV.STATUS");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, param, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PerfilVisualizacaoVendaListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<PerfilVisualizacaoVendaListModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoVendaApi/Gravar")]
        public PostApiRetorno<long> Gravar([FromBody] ParamWebApi<PerfilVisualizacaoVendaCadastroModel> param)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ITransaction transaction = sessionNh.BeginTransaction())
            {
                try
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    PerfilVisualizacaoVenda model = perfilBusiness.Gravar(param.Objeto, param.Usuario);
                    transaction.Commit();

                    retorno.Ok = true;
                    retorno.IdObjeto = model.Id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;

                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/FindById")]
        public GetApiRetorno<PerfilVisualizacaoVendaCadastroModel> FindById(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    retorno.Objeto = perfilBusiness.FindById(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoVendaApi/GetCombo")]
        public ObjCombo<long> GetCombo(RequestObjCombo<long?> request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                var perfilBusiness =
                    new PerfilVisualizacaoVendaBusiness(
                        repository,
                        repoParamTipoPessoa,
                        repoParamCategoriaCliente,
                        repoParamSetor,
                        repoParamMeioCaptacao,
                        repoParamEquipeVenda,
                        repoParamRegiao,
                        repoSetor,
                        repoMeioCaptacao,
                        repoEquipeVendas,
                        repoCategoriaCliente,
                        repoCidade);

                return perfilBusiness.GetCombo(request, CONSTANTES.PaginacaoRegistros);
            }
        }

        [HttpPost]
        [Route("api/PerfilVisualizacaoVendaApi/AlterarStatus")]
        public GetApiRetorno AlterarStatus(ParamVisualizacaoVendaAlterarStatusRequest request)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    foreach (long id in request.Ids)
                    {
                        perfilBusiness.AlterarStatus(id, request.Status);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamTipoPessoa")]
        public GetApiRetorno ExcluirParamTipoPessoa(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamTipoPessoa(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamCategoriaCliente")]
        public GetApiRetorno ExcluirParamCategoriaCliente(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamCategoriaCliente(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamSetor")]
        public GetApiRetorno ExcluirParamSetor(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamSetorDepto(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamMeioCaptacao")]
        public GetApiRetorno ExcluirParamMeioCaptacao(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamMeioCaptacao(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamRegiao")]
        public GetApiRetorno ExcluirParamRegiao(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamRegiao(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }



        [HttpGet]
        [Route("api/PerfilVisualizacaoVendaApi/ExcluirParamEquipeVenda")]
        public GetApiRetorno ExcluirParamEquipeVenda(long id)
        {
            var retorno = new GetApiRetorno<PerfilVisualizacaoVendaCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IPerfilVisualizacaoVendaRepository repository = new PerfilVisualizacaoVendaRepository(sessionNh);

                    IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa = new PerfilVisualizacaoVendaParamTipoPessoaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente = new PerfilVisualizacaoVendaParamCategoriaClienteRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor = new PerfilVisualizacaoVendaParamSetorDeptoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamSuperv = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda = new PerfilVisualizacaoVendaParamEquipeVendaRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao = new PerfilVisualizacaoVendaParamMeioCaptacaoRepository(sessionNh);
                    IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao = new PerfilVisualizacaoVendaParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new PerfilVisualizacaoVendaBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamMeioCaptacao,
                            repoParamEquipeVenda,
                            repoParamRegiao,
                            repoSetor,
                            repoMeioCaptacao,
                            repoEquipeVendas,
                            repoCategoriaCliente,
                            repoCidade);

                    perfilBusiness.ExcluirParamEquipeVenda(id);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }
}
