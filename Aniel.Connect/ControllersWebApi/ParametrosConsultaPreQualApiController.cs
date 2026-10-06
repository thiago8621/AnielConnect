using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Bairro;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.EquipeVendas;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
//using Aniel.Connect.Repository.ParametrosConsultaPreQualParamMeioCaptacao;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamCategoriaCliente;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamRegiao;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamSetorDepto;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamTipoPessoa;
//using Aniel.Connect.Repository.ParametrosConsultaPreQualParamEquipeVenda;
using Aniel.Connect.Repository.T5.Setor;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ParametrosConsultaPreQualApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ParametrosConsultaPreQualApi/GetAll")]
        public async Task<DataTableResponse<ParametrosConsultaPreQualListModel>> GetAll(DataTableRequest request)
        {
            int recordsTotal = 0;
            var retorno = new List<ParametrosConsultaPreQualListModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                var sqlFrom = new StringBuilder();
                var param = new Dictionary<string, object>();

                sqlFrom.AppendLine("FROM TB_PARAM_CONS_CRED_PREQUAL A");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("WHERE UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS, A.PADRAO");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, param, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ParametrosConsultaPreQualListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Padrao = reader["PADRAO"] != DBNull.Value ? (Convert.ToBoolean(reader["PADRAO"]) ? "Sim" : "Não") : RsBase.NAO
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ParametrosConsultaPreQualListModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost]
        [Route("api/ParametrosConsultaPreQualApi/Gravar")]
        public PostApiRetorno<long> Gravar([FromBody] ParamWebApi<ParametrosConsultaPreQualCadastroModel> param)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ITransaction transaction = sessionNh.BeginTransaction())
            {
                try
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    //IParametrosConsultaPreQualParamMeioCaptacaoRepository repoParamSuperv = new ParametrosConsultaPreQualParamMeioCaptacaoRepository(sessionNh);
                    //IParametrosConsultaPreQualParamEquipeVendaRepository repoParamEquipeVenda = new ParametrosConsultaPreQualParamEquipeVendaRepository(sessionNh);
                    //IParametrosConsultaPreQualParamMeioCaptacaoRepository repoParamMeioCaptacao = new ParametrosConsultaPreQualParamMeioCaptacaoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);

                    

                    ParametrosConsultaPreQual model = perfilBusiness.Gravar(param.Objeto, param.Usuario);
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
        [Route("api/ParametrosConsultaPreQualApi/FindById")]
        public GetApiRetorno<ParametrosConsultaPreQualCadastroModel> FindById(long id)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);

                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);

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
        [Route("api/ParametrosConsultaPreQualApi/GetCombo")]
        public ObjCombo<long> GetCombo(RequestObjCombo<long?> request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);

                IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                IBairroRepository repoBairro = new BairroRepository(sessionNh);

                var perfilBusiness =
                    new ParametrosConsultaPreQualBusiness(
                        repository,
                        repoParamTipoPessoa,
                        repoParamCategoriaCliente,
                        repoParamSetor,
                        repoParamRegiao,
                        repoSetor,
                        repoCategoriaCliente,
                        repoCidade,
                        repoBairro);

                return perfilBusiness.GetCombo(request, CONSTANTES.PaginacaoRegistros);
            }
        }

        [HttpPost]
        [Route("api/ParametrosConsultaPreQualApi/AlterarStatus")]
        public GetApiRetorno AlterarStatus(ParamVisualizacaoVendaAlterarStatusRequest request)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);

                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);


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
        [Route("api/ParametrosConsultaPreQualApi/ExcluirParamTipoPessoa")]
        public GetApiRetorno ExcluirParamTipoPessoa(long id)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);

                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);

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
        [Route("api/ParametrosConsultaPreQualApi/ExcluirParamCategoriaCliente")]
        public GetApiRetorno ExcluirParamCategoriaCliente(long id)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);

                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);

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
        [Route("api/ParametrosConsultaPreQualApi/ExcluirParamSetor")]
        public GetApiRetorno ExcluirParamSetor(long id)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);



                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);


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

        //[HttpGet]
        //[Route("api/ParametrosConsultaPreQualApi/ExcluirParamMeioCaptacao")]
        //public GetApiRetorno ExcluirParamMeioCaptacao(long id)
        //{
        //    var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

        //    try
        //    {
        //        using (ISession sessionNh = HibernateUtil.GetSession())
        //        using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
        //        {
        //            IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

        //            IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
        //            IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
        //            IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
        //            IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

        //            IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
        //            IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
        //            ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
        //            ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
        //            ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);



        //            var perfilBusiness =
        //                new ParametrosConsultaPreQualBusiness(
        //                    repository,
        //                    repoParamTipoPessoa,
        //                    repoParamCategoriaCliente,
        //                    repoParamSetor,
        //                    repoParamRegiao,
        //                    repoSetor,
        //                    repoCategoriaCliente,
        //                    repoCidade);

        //            perfilBusiness.ExcluirParamMeioCaptacao(id);
        //            retorno.Ok = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Ok = false;
        //        retorno.Mensagem = ex.Message;

        //        if (ex.InnerException != null)
        //            retorno.InnerException = ex.InnerException.Message;

        //        CONSTANTES.log4Net.Error(ex.Message, ex);
        //    }

        //    return retorno;
        //}

        [HttpGet]
        [Route("api/ParametrosConsultaPreQualApi/ExcluirParamRegiao")]
        public GetApiRetorno ExcluirParamRegiao(long id)
        {
            var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

                    IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
                    IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
                    IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
                    IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

                    IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
                    IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
                    ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                    ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                    ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);
                    IBairroRepository repoBairro = new BairroRepository(sessionNh);

                    var perfilBusiness =
                        new ParametrosConsultaPreQualBusiness(
                            repository,
                            repoParamTipoPessoa,
                            repoParamCategoriaCliente,
                            repoParamSetor,
                            repoParamRegiao,
                            repoSetor,
                            repoCategoriaCliente,
                            repoCidade,
                            repoBairro);

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



        //[HttpGet]
        //[Route("api/ParametrosConsultaPreQualApi/ExcluirParamEquipeVenda")]
        //public GetApiRetorno ExcluirParamEquipeVenda(long id)
        //{
        //    var retorno = new GetApiRetorno<ParametrosConsultaPreQualCadastroModel>();

        //    try
        //    {
        //        using (ISession sessionNh = HibernateUtil.GetSession())
        //        using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
        //        {
        //            IParametrosConsultaPreQualRepository repository = new ParametrosConsultaPreQualRepository(sessionNh);

        //            IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa = new ParametrosConsultaPreQualParamTipoPessoaRepository(sessionNh);
        //            IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente = new ParametrosConsultaPreQualParamCategoriaClienteRepository(sessionNh);
        //            IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor = new ParametrosConsultaPreQualParamSetorDeptoRepository(sessionNh);
        //            IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao = new ParametrosConsultaPreQualParamRegiaoRepository(sessionNh);

        //            IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);
        //            IEquipeVendasRepository repoEquipeVendas = new EquipeVendasRepository(sessionNh);
        //            ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
        //            ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
        //            ISetorAnielRepository repoSetor = new SetorAnielRepository(sessionT5Nh);


        //            var perfilBusiness =
        //                new ParametrosConsultaPreQualBusiness(
        //                    repository,
        //                    repoParamTipoPessoa,
        //                    repoParamCategoriaCliente,
        //                    repoParamSetor,
        //                    repoParamRegiao,
        //                    repoSetor,
        //                    repoCategoriaCliente,
        //                    repoCidade);

        //            perfilBusiness.ExcluirParamEquipeVenda(id);
        //            retorno.Ok = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Ok = false;
        //        retorno.Mensagem = ex.Message;

        //        if (ex.InnerException != null)
        //            retorno.InnerException = ex.InnerException.Message;

        //        CONSTANTES.log4Net.Error(ex.Message, ex);
        //    }

        //    return retorno;
        //}
    }
}
