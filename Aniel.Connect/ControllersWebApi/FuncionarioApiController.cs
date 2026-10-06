using Aniel.Connect.Controllers;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class FuncionarioApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/FuncionarioApi/GetSupervisor")]
        public async Task<DataTableResponse<FuncionarioListaModel>> GetSupervisor(Funcionario_DataTableRequest request)
        {
            List<FuncionarioListaModel> retorno = new List<FuncionarioListaModel>();
            IOrderedEnumerable<FuncionarioListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            StringBuilder sqlFrom = new StringBuilder()
            .AppendLine("FROM TB_FUNCIONARIO A ")
            .AppendLine("LEFT JOIN TB_META C ")
            .AppendLine("ON (A.ID_META = C.ID) ")
            .AppendLine("LEFT JOIN TB_FAIXA_COMISSIONAMENTO D ")
            .AppendLine("ON (A.ID_FAIXA = D.ID) ")
            .AppendLine("LEFT JOIN TB_COMISSIONAMENTO E ")
            .AppendLine("ON (A.ID_COMISSIONAMENTO = E.ID) ")
            .AppendLine("WHERE A.E_SUPERVISOR = 1");


            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ")
                        .AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder()
                    .AppendLine("SELECT A.ID, A.NOME, A.ID_META, A.ID_FAIXA, A.E_SUPERVISOR,")
                    .AppendLine("C.DESCRICAO AS DESCRICAO_META, D.DESCRICAO AS DESCRICAO_FAIXA, E.DESCRICAO AS COMISSIONAMENTO ")
                    .AppendLine(sqlFrom.ToString());

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND (UPPER(A.NOME) LIKE UPPER(@SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FuncionarioListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Meta = Convert.ToString(reader["DESCRICAO_META"]),
                                Faixa = Convert.ToString(reader["DESCRICAO_FAIXA"]),
                                Desc_Comissionamento = Convert.ToString(reader["COMISSIONAMENTO"]),
                                Id_Meta = reader["ID_META"] != DBNull.Value ? Convert.ToInt64(reader["ID_META"]) : (Int64?)null,
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                    case 3:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Meta);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Meta);
                            break;
                        }
                    case 4:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Desc_Comissionamento);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Desc_Comissionamento);
                            break;
                        }
                    case 5:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Faixa);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Faixa);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<FuncionarioListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/FuncionarioApi/GetVendedor")]
        public async Task<DataTableResponse<FuncionarioListaModel>> GetVendedor(Funcionario_DataTableRequest request)
        {
            try
            {
                List<FuncionarioListaModel> retorno = new List<FuncionarioListaModel>();
                IOrderedEnumerable<FuncionarioListaModel> listaOrdenada = null;
                Int32 recordsTotal = 0;

                StringBuilder sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO A ")
                .AppendLine("LEFT JOIN TB_META C ")
                .AppendLine("ON (A.ID_META = C.ID) ")

                .AppendLine("LEFT JOIN TB_FAIXA_COMISSIONAMENTO D ")
                .AppendLine("ON (A.ID_FAIXA = D.ID) ")

                .AppendLine("LEFT JOIN TB_COMISSIONAMENTO E ")
                .AppendLine("ON (A.ID_COMISSIONAMENTO = E.ID) ")

                .AppendLine("LEFT JOIN ASPNETUSERS F ")
                .AppendLine("ON (A.ID = F.ID_FUNCIONARIO) ")

                .AppendLine("LEFT JOIN TB_FUNCIONARIO G")
                .AppendLine("ON(G.ID = A.ID_SUPERVISOR)")

                .AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR S")
                .AppendLine("ON (S.ID_SUPERVISOR = A.ID)")

                .AppendLine("LEFT JOIN TB_EQUIPE_VENDA V")
                .AppendLine("ON (V.ID = A.EQUIPE_VENDA)")

                .AppendLine("LEFT JOIN TB_PARAM_CONS_CRED_PREQUAL H")
                .AppendLine("ON (H.ID = A.ID_PARAM_CONS_CRED_PREQUAL)")

                .AppendLine($" WHERE (A.SITUACAO = 0 OR A.SITUACAO = 4) AND F.TIPO_USUARIO BETWEEN 2 and 3 ");

                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ")
                                                            .AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder()

                        .AppendLine("SELECT DISTINCT A.ID, A.NOME, A.ID_SUPERVISOR, A.ID_META, A.ID_FAIXA, A.E_SUPERVISOR,")
                        .AppendLine("C.DESCRICAO AS DESCRICAO_META, D.DESCRICAO AS DESCRICAO_FAIXA, A.NUM_FRE, ")
                        .AppendLine("E.DESCRICAO AS COMISSIONAMENTO, F.ID_FUNCIONARIO, F.TIPO_USUARIO, G.NOME AS NOME_SUPERVISOR,V.DESCRICAO AS EQUIPEVENDA, H.DESCRICAO AS PARAM_PREQUAL  ")
                        .AppendLine(sqlFrom.ToString());

                        #region //Filtrando

                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            sql.AppendLine("AND (UPPER(A.NOME) LIKE UPPER(@SEARCH))");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        }

                        if (request.Comissionamento.HasValue)
                            sql.AppendLine($" AND E.ID = {request.Comissionamento} ");

                        if (request.FaixaComissionamento.HasValue)
                            sql.AppendLine($" AND D.ID = {request.FaixaComissionamento} ");

                        if (request.Id_Supervisor.HasValue)
                            sql.AppendLine($" AND G.ID = {request.Id_Supervisor} ");

                        if (!String.IsNullOrEmpty(request.EquipeVenda))
                            sql.AppendLine($" AND V.DESCRICAO  = {request.EquipeVenda} ");

                        if (request.Meta.HasValue)
                            sql.AppendLine($" AND C.ID = {request.Meta} ");

                        if (request.Premier)
                            sql.AppendLine($" AND F.ID_FUNCIONARIO <> 0 ");

                        if (request.NaoPremier)
                            sql.AppendLine($" AND F.ID_FUNCIONARIO = 0 ");

                        #endregion

                        sql.AppendLine($" GROUP BY A.ID, A.NOME, A.ID_SUPERVISOR, A.ID_META, A.ID_FAIXA, A.E_SUPERVISOR, C.DESCRICAO, D.DESCRICAO, A.NUM_FRE,E.DESCRICAO, F.ID_FUNCIONARIO, F.TIPO_USUARIO, G.NOME, V.DESCRICAO, H.DESCRICAO");
                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                FuncionarioListaModel objeto = new FuncionarioListaModel();

                                objeto.Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0;
                                objeto.Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : String.Empty;
                                objeto.Meta = reader["DESCRICAO_META"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_META"]) : String.Empty;
                                objeto.Faixa = reader["DESCRICAO_FAIXA"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_FAIXA"]) : String.Empty;
                                objeto.Id_Supervisor = reader["ID_SUPERVISOR"] != DBNull.Value ? Convert.ToInt64(reader["ID_SUPERVISOR"]) : 0;
                                objeto.Num_Fre = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) : String.Empty;
                                objeto.Desc_Comissionamento = reader["COMISSIONAMENTO"] != DBNull.Value ? Convert.ToString(reader["COMISSIONAMENTO"]) : String.Empty;
                                objeto.ParametrosConsultaPreQual = reader["PARAM_PREQUAL"] != DBNull.Value ? Convert.ToString(reader["PARAM_PREQUAL"]) : String.Empty;
                                objeto.PossuiAcessoPremier = reader["ID_FUNCIONARIO"] != DBNull.Value ? Resources.RsBase.SIM : Resources.RsBase.NAO;

                                objeto.TipoUsuario = reader["TIPO_USUARIO"] != DBNull.Value ? Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(TipoUsuarioEnum)).GetValue(Convert.ToInt32(reader["TIPO_USUARIO"])).ToString()) : String.Empty;

                                objeto.NomeSupervisor = reader["NOME_SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["NOME_SUPERVISOR"]) : String.Empty;

                                objeto.EquipeVenda = reader["EQUIPEVENDA"] != DBNull.Value && objeto.TipoUsuario == "Vendedor" ? Convert.ToString(reader["EQUIPEVENDA"]) : "Não Definido";

                                //if (!objeto.TipoUsuario == "Vendedor" && objeto.EquipeVenda.Count() >= 1)
                                //{
                                //    objeto.EquipeVenda = "Definido";
                                //}
                                //else
                                //{
                                //    objeto.EquipeVenda = "Não Definido";
                                //}

                                retorno.Add(objeto);
                            }


                        }

                    }
                }

                //Ordenacao
                if (request.order != null && request.order.Any())
                {
                    int sortColumnIndex = request.order[0].column;
                    string sortDirection = request.order[0].dir;
                    switch (sortColumnIndex)
                    {
                        case 1:     // NUMERO
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Id);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Id);
                                break;
                            }
                        case 2:
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Nome);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                                break;
                            }
                        case 3:
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Meta);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Meta);
                                break;
                            }
                        case 4:
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Desc_Comissionamento);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Desc_Comissionamento);
                                break;
                            }
                        case 5:
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Faixa);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Faixa);
                                break;
                            }
                    }
                }

                //Page e retornando DataTable
                return new DataTableResponse<FuncionarioListaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ""
                };
            }
            catch (Exception e)
            {
                return new DataTableResponse<FuncionarioListaModel>
                {
                    draw = request.draw,
                    error = e.Message
                };
            }

        }

        [HttpGet()]
        [Route("api/Funcionarioapi/FindById")]
        public async Task<GetApiRetorno<FuncionarioCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<FuncionarioCadastroModel> retorno = new GetApiRetorno<FuncionarioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoFuncionario.selectFirst(p => p.Id == Id), session);

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        private void BindViewToObj(ISession session, ref Funcionario objBanco, FuncionarioCadastroModel view)
        {
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoMeta = new Conexao<Meta>(session))
            using (var repoFaixa = new Conexao<Faixa_Comissionamento>(session))
            using (var repoEquipeVenda = new Conexao<EquipeVenda>(session))
            using (var repoSupervisorEquipeVenda = new Conexao<SupervisorEquipeVenda>(session))
            using (var repoPreQual = new Conexao<ParametrosConsultaPreQual>(session))
            {
                objBanco.E_SUPERVISOR = view.E_SUPERVISOR;

                if (!view.E_SUPERVISOR)
                {
                    objBanco.Vendedor = true;
                    objBanco.Supervisor = view.Id_Supervisor.HasValue ? repoFuncionario.selectFirst(p => p.Id == view.Id_Supervisor.Value).Result : null;
                }
                else
                {
                    objBanco.Vendedor = false;
                    objBanco.Supervisor = null;
                }

                objBanco.Vendedor = view.Vendedor;


                objBanco.Meta = view.Id_Meta.HasValue ? repoMeta.selectFirst(p => p.Id == view.Id_Meta.Value).Result : null;
                objBanco.Faixa = view.Id_Faixa.HasValue ? repoFaixa.selectFirst(p => p.Id == view.Id_Faixa.Value).Result : null;
                //   objBanco.Filial = view.Id_Filial.HasValue ? new BuscaGenericaRepository<Filial>(session, null).FindById(view.Id_Filial.Value).Result : null;
                objBanco.Id_Comissionamento = view.Id_Comissionamento;
                objBanco.IdEquipeVenda = view.IdEquipeVenda;
                objBanco.ParametrosConsultaPreQual = view.IdParamPreQual.HasValue ? repoPreQual.selectFirst(p => p.Id == view.IdParamPreQual.Value).Result : null;

                objBanco.CodigoVendedorIntegracao = view.CodigoVendedorIntegracao;
                objBanco.CodigoVendedorAuxiliar = view.CodigoVendedorIntegracaoAuxiliar;
            }
        }

        private void BindViewToObjMassa(ISession session, ref Funcionario objBanco, FuncionarioCadastroModel view)
        {
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoMeta = new Conexao<Meta>(session))
            using (var repoFaixa = new Conexao<Faixa_Comissionamento>(session))
            using (var repoEquipeVenda = new Conexao<EquipeVenda>(session))
            using (var repoSupervisorEquipeVenda = new Conexao<SupervisorEquipeVenda>(session))
            using (var repoPreQual = new Conexao<ParametrosConsultaPreQual>(session))
            {


                objBanco.Supervisor = view.Id_Supervisor.HasValue ? repoFuncionario.selectFirst(p => p.Id == view.Id_Supervisor.Value).Result : objBanco.Supervisor;

                objBanco.Meta = view.Id_Meta.HasValue ? repoMeta.selectFirst(p => p.Id == view.Id_Meta.Value).Result : objBanco.Meta;
                objBanco.Faixa = view.Id_Faixa.HasValue ? repoFaixa.selectFirst(p => p.Id == view.Id_Faixa.Value).Result : objBanco.Faixa;
                objBanco.Id_Comissionamento = view.Id_Comissionamento.HasValue ? view.Id_Comissionamento : objBanco.Id_Comissionamento;
                objBanco.IdEquipeVenda = view.IdEquipeVenda.HasValue ? view.IdEquipeVenda : objBanco.IdEquipeVenda;
                objBanco.ParametrosConsultaPreQual = view.IdParamPreQual.HasValue ? repoPreQual.selectFirst(p => p.Id == view.IdParamPreQual.Value).Result : objBanco.ParametrosConsultaPreQual;

            }
        }

        private FuncionarioCadastroModel GetViewObj(Funcionario obj, ISession session)
        {
            string tipo = "";
            long tipoEnum = 0;

            using (var usuRepo = new Conexao<IdentityUser>(session))
            {
                var result = usuRepo.select(x => x.Funcionario.Id == obj.Id).Result.FirstOrDefault().Tipo_Usuario;

                tipoEnum = result.HasValue ? result.Value : 3;
                tipo = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(Sinapse.Model.enumerados.TipoUsuarioEnum)).GetValue(Convert.ToInt32(tipoEnum)).ToString());
            }

            return new FuncionarioCadastroModel
            {
                Id = obj.Id,
                Num_Fre = obj.Num_Fre,
                Nome = obj.Nome,
                Tel_Celular = obj.Tel_Celular,
                Situacao = obj.Situacao,
                Vendedor = obj.Vendedor,
                //Id_Supervisor = obj.Supervisor?.Id.ToString(),
                Id_Supervisor = obj.Supervisor?.Id,
                Desc_Supervisor = obj.Supervisor?.Nome,
                Tipo = tipo,
                TipoEnum = tipoEnum,
                E_SUPERVISOR = obj.E_SUPERVISOR,

                Id_Meta = obj.Meta?.Id,
                Desc_Meta = obj.Meta?.Descricao,
                Id_Meta_Supervisor = obj.Supervisor?.Meta?.Id,

                Id_Faixa = obj.Faixa?.Id,
                Desc_Faixa = obj.Faixa?.Descricao,

                Id_Departamento = obj.Departamento?.Id ?? 0,
                Desc_Departamento = obj.Departamento?.Descricao,

                IdParamPreQual = obj.ParametrosConsultaPreQual?.Id,

                Id_Filial = obj.Filial?.Id ?? 0,
                Desc_Filial = obj.Filial?.Nome,
                IdEquipeVenda = obj.IdEquipeVenda,
                Id_Comissionamento = obj.Id_Comissionamento,

                CodigoVendedorIntegracao = obj.CodigoVendedorIntegracao,
                CodigoVendedorIntegracaoAuxiliar = obj.CodigoVendedorAuxiliar
            };

        }

        [HttpPost()]
        [Route("api/Funcionarioapi/Gravar")]
        public async Task<PostApiRetorno<long>> Gravar(ParamWebApi<FuncionarioCadastroModel> value)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    using (var repository = new Conexao<Funcionario>(session))
                    using (var repoEquipeVenda = new Conexao<EquipeVenda>(session))
                    using (var repoSupervisorEquipeVenda = new Conexao<SupervisorEquipeVenda>(session))
                    {
                        Funcionario model = null;
                        var idsFuncionarios = new List<long>();

                        if (!string.IsNullOrEmpty(value.Objeto.Funcionarios))
                        {
                            idsFuncionarios = value.Objeto.Funcionarios.Split(',')
                                                                       .Select(p => Convert.ToInt64(p))
                                                                       .ToList();
                        }
                        else
                        {
                            idsFuncionarios.Add(value.Objeto.Id);
                        }

                        foreach (var item in idsFuncionarios)
                        {
                            model = null;
                            EquipeVenda modelEquipeVenda = null;
                            SupervisorEquipeVenda modelSupervEquipeVenda = null;
                            model = await repository.selectFirst(p => p.Id == item);
                            if (value.Objeto.IdEquipeVenda.HasValue)
                                modelEquipeVenda = await repoEquipeVenda.selectFirst(p => p.Id == value.Objeto.IdEquipeVenda);

                            if (model == null)
                            {
                                throw new Exception($"Funcionário não encontrado");
                            }

                            if (value.Objeto.Tipo == "Supervisor comercial" && modelSupervEquipeVenda != null)
                            {
                                if (value.Objeto.CadastrarEmMassa == false)
                                    modelSupervEquipeVenda.Supervisor = model.Supervisor;

                                modelSupervEquipeVenda.DataHoraAlteracao = DateTime.Now;
                                modelSupervEquipeVenda.UsuarioAlteracao = value.Usuario;
                            }
                            else
                            {
                                if (value.Objeto.CadastrarEmMassa == false)
                                    model.IdEquipeVenda = modelEquipeVenda?.Id;

                                model.DataHoraAlteracao = DateTime.Now;
                                model.UsuarioAlteracao = value.Usuario;
                                model.Situacao = 0;
                            }

                            if (value.Objeto.CadastrarEmMassa)
                                BindViewToObjMassa(session, ref model, value.Objeto);
                            else
                                BindViewToObj(session, ref model, value.Objeto);

                            if (modelEquipeVenda != null)
                                await repoEquipeVenda.SaveOrUpdate(modelEquipeVenda);

                            if (modelSupervEquipeVenda != null)
                                await repoSupervisorEquipeVenda.SaveOrUpdate(modelSupervEquipeVenda);

                            await repository.SaveOrUpdate(model);
                        }

                        retorno.IdObjeto = model.Id;
                        retorno.Ok = true;

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Funcionarioapi/GetComboMonitor")]
        public async Task<ObjCombo<long>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<long> retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM ASPNETUSERS A ");
                    sqlFrom.Append(" LEFT JOIN TB_FUNCIONARIO B ");
                    sqlFrom.Append(" ON(B.ID = A.id_funcionario) ");


                    sqlFrom.AppendLine($" WHERE A.TIPO_USUARIO = 4 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine(" AND UPPER(B.NOME) LIKE UPPER(@SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.q.ToUpper()));
                    }


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID COD_USUARIO, B.ID, B.NOME ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResultMonitor
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
                                    CodUsuario = Convert.ToString(reader["COD_USUARIO"]),
                                    CodEquipe = await GetCodEquipeMonitorT5(Convert.ToString(reader["COD_USUARIO"]))
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        private async Task<String> GetCodEquipeMonitorT5(String NumFre)
        {
            var retorno = String.Empty;

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder($"SELECT A.EQUIPE FROM TB_EQUIPE A WHERE A.NUM_FRE = '{NumFre}'");
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno = Convert.ToString(reader["EQUIPE"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/FuncionarioApi/verificaUsuario")]
        public async Task<PostApiRetorno<Int64>> verificaUsuario(Int64 Funcionario_Id)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Ok = true;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    IdentityUser usuario = await new Conexao<IdentityUser>(session).selectFirst(p => p.Funcionario.Id == Funcionario_Id);

                    if (usuario == null)
                    {
                        retorno.Mensagens.Add(new PostApiMensagem()
                        {
                            Mensagem = "Vendedor ainda não possui usuario para logar no Premiere.É Necessário criar usuario no Aniel.",
                            Tipo = PostApiMensagemType.Info
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/FuncionarioApi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Ok = true;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        //  String _ids = $"({String.Join(",", value.ids)})";

                        foreach (var id in value.ids)
                        {
                            StringBuilder sql = new StringBuilder();
                            using (ISession session_teste = HibernateUtil.GetSession())
                            {
                                var _funcionario_count = await new Conexao<Funcionario>(session_teste).select(p => p.Supervisor.Id == id);
                                if ((_funcionario_count.Count == 0))
                                {
                                    sql = new StringBuilder($"UPDATE TB_FUNCIONARIO SET COALESCE(E_SUPERVISOR,0) = 0 WHERE ID = {id}");
                                    await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                                }
                                else
                                {
                                    retorno.Ok = false;
                                    retorno.Mensagem = "Um ou mais supervisores possuem vendedores atrelados.";
                                }
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/FuncionarioApi/excluirlistavendedor")]
        public async Task<PostApiRetorno<Int64>> ExcluirListaVendedor(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_FUNCIONARIO SET VENDEDOR = 0, ID_SUPERVISOR = NULL WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Funcionarioapi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Funcionarioapi/buscaVendedor")]
        public async Task<List<ObjBusca<Int64>>> buscaVendedor(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME,A.VENDEDOR ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");
                        sql.AppendLine("WHERE COALESCE(A.E_SUPERVISOR,0) = 0 AND (UPPER(A.NOME) LIKE @VALOR) AND A.VENDEDOR = 1 ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Funcionarioapi/buscaSupervisorByFuncionario")]
        public async Task<List<ObjBusca<Int64>>> buscaSupervisorByFuncionario(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT B.ID, B.NOME");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");

                        sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                        sql.AppendLine("ON(A.ID_SUPERVISOR = B.ID)");

                        sql.AppendLine("WHERE A.ID = @ID_VENDEDOR");

                        parametros.Add("@ID_VENDEDOR", Convert.ToInt64(term));

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Funcionarioapi/buscaSupervisor")]
        public async Task<List<ObjBusca<Int64>>> buscaSupervisor(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine("FROM TB_FUNCIONARIO A");
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) AND E_SUPERVISOR = 1");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetListVendedor()
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                try
                {
                    var lista = await repoFuncionario.queryover().Where(m => m.E_SUPERVISOR == false).ListAsync();
                    lista = lista.Where(p => !String.IsNullOrEmpty(p.Nome)).OrderBy(p => p.Nome).ToList();

                    retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                    {
                        Id = p.Id,
                        Num_Fre = p.Num_Fre,
                        Nome = p.Nome
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Funcionarioapi/GetListVendedorPorSupervisor")]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetListVendedorPorSupervisor(Int32? id_supervisor)
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                try
                {
                    if (id_supervisor != null && id_supervisor.HasValue)
                    {
                        var lista = await repoFuncionario.queryover().Where(m => m.E_SUPERVISOR == false && m.Supervisor.Id == id_supervisor).ListAsync();
                        lista = lista.Where(p => !String.IsNullOrEmpty(p.Nome)).OrderBy(p => p.Nome).ToList();

                        retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                        {
                            Id = p.Id,
                            Num_Fre = p.Num_Fre,
                            Nome = p.Nome
                        });

                        retorno.Ok = true;
                    }

                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetListSupervisor()
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {

                try
                {
                    var lista = await repoFuncionario.queryover().Where(m => m.E_SUPERVISOR == true).ListAsync();

                    retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                    {
                        Id = p.Id,
                        Num_Fre = p.Num_Fre,
                        Nome = p.Nome

                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        public async Task<GetApiRetorno<List<SupervisorListaModel>>> GetListSupervisorView()
        {
            GetApiRetorno<List<SupervisorListaModel>> retorno = new GetApiRetorno<List<SupervisorListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoUsuario = new Conexao<IdentityUser>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {

                try
                {

                    var lista = await repoUsuario.queryover().Where(m => m.Tipo_Usuario == 2).ListAsync();
                    var funcionarios = lista.Select(p => p.Funcionario).ToList();

                    retorno.Objeto = funcionarios.Select(p => new SupervisorListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Nome
                    }).ToList();

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/Funcionarioapi/GetListSupervisorPorFilial")]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetListSupervisorPorFilial(Int32? id_filial)
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {

                try
                {
                    if (id_filial != null && id_filial.HasValue)
                    {

                        var lista = await repoFuncionario.queryover().Where(m => m.E_SUPERVISOR == true && m.Filial.Id == id_filial.Value).ListAsync();

                        retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                        {
                            Id = p.Id,
                            Num_Fre = p.Num_Fre,
                            Nome = p.Nome

                        });

                        retorno.Ok = true;
                    }

                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetListSupervisorAniel()
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    var lista = await new Conexao<FuncionarioAniel>(session).queryover().Where(m => m.Supervisor == "S").ListAsync();

                    retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                    {
                        Num_Fre = p.Num_Fre,
                        Nome = p.Nome

                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }



        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<FuncionarioListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<FuncionarioListaModel>> retorno = new GetApiRetorno<IEnumerable<FuncionarioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                try
                {
                    var lista = await repoFuncionario.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new FuncionarioListaModel
                    {
                        Id = p.Id,
                        Departamento = Convert.ToString(p.Departamento)
                        //Descricao = p.Descricao

                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }


        //[HttpPost()]
        [AcceptVerbs("Get")]
        [Route("api/funcionarioApi/BuscaFuncionario")]
        public async Task<List<ObjBusca<Int64>>> BuscaFuncionario(String term)
        {
            List<ObjBusca<Int64>> retorno_objeto = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A ");
                        sql.AppendLine("WHERE (A.SITUACAO = 0 OR A.SITUACAO = 4)  ");
                        sql.AppendLine(" AND ( (UPPER(A.NOME) LIKE @VALOR)  ) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno_objeto.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }

                    //retorno_objeto.Ok = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno_objeto;




        }

        public async Task<List<SelectValue>> GetSupervisores()
        {
            List<SelectValue> retorno = new List<SelectValue>();

            using (ISession session = HibernateUtil.GetSession())
            using (var usuRepo = new Conexao<IdentityUser>(session))
            using (var funcRepo = new Conexao<Funcionario>(session))
            {
                var supervisores = await usuRepo.select(x => x.Tipo_Usuario == 2);

                foreach (var supervisor in supervisores)
                {
                    if (string.IsNullOrEmpty(supervisor.Nome))
                    {
                        supervisor.Nome = funcRepo.select(x => x.Id == supervisor.Funcionario.Id).Result.FirstOrDefault().Nome;
                    }

                    retorno.Add(new SelectValue { Id = supervisor.Funcionario.Id, Descricao = supervisor.Nome });
                }
            }

            return retorno;
        }


        [AcceptVerbs("Get")]
        [Route("api/funcionarioApi/BuscaFuncionarioSupervisor")]
        public async Task<List<ObjBusca<Int64>>> BuscaFuncionarioSupervisor(String term)
        {

            List<ObjBusca<Int64>> retorno_objeto = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {

                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A ");
                        sql.AppendLine("WHERE A.SITUACAO = 0 AND A.E_SUPERVISOR = 1 ");
                        sql.AppendLine(" AND ( (UPPER(A.NOME) LIKE @VALOR) ) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno_objeto.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
                            }
                        }
                    }

                    //retorno_objeto.Ok = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno_objeto;

        }

        [AcceptVerbs("Get")]
        [Route("api/funcionarioApi/GetMetaFaixaComissionamento")]
        public async Task<VendedorMetaFaixaComissionamentoModel> GetMetaFaixaComissionamento(Int64 Id)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, COALESCE(A.ID_META, B.ID_META, C.ID_META) AS ID_META, COALESCE(A.ID_FAIXA, B.ID_FAIXA, C.ID_FAIXA_COMISSIONAMENTO) AS ID_FAIXA, ")
                           .AppendLine("       COALESCE(A.ID_COMISSIONAMENTO, B.ID_COMISSIONAMENTO, C.ID_COMISSIONAMENTO) AS ID_COMISSIONAMENTO ")
                           .AppendLine("FROM TB_FUNCIONARIO A ")
                           .AppendLine("INNER JOIN TB_FUNCIONARIO B ")
                           .AppendLine("ON(A.ID_SUPERVISOR = B.ID) ")
                           .AppendLine("INNER JOIN TB_FILIAL C ")
                           .AppendLine("ON(A.ID_FILIAL = C.ID) ")
                           .AppendLine("WHERE A.ID = @ID ");

                        parametros.Add("@ID", Id);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                return new VendedorMetaFaixaComissionamentoModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Id_Comissionamento = reader["ID_COMISSIONAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_COMISSIONAMENTO"]) : (long?)null,
                                    Id_Faixa = reader["ID_FAIXA"] != DBNull.Value ? Convert.ToInt64(reader["ID_FAIXA"]) : (long?)null,
                                    Id_Meta = reader["ID_META"] != DBNull.Value ? Convert.ToInt64(reader["ID_META"]) : (long?)null
                                };
                            }
                            else return new VendedorMetaFaixaComissionamentoModel();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //api para uso apenas da tela de campanha
        [HttpPost()]
        [Route("api/Funcionarioapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO IN (0,4) ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_SUPERVISOR = {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_FILIAL= {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOME LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }


                    if (request.idpai.HasValue && request.idavo.HasValue)
                    {
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                            sql.AppendLine(sqlFrom.ToString());

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                    retorno.total_count = Convert.ToInt32(reader["CO"]);
                            }
                        }


                        //Select
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME ");
                            sql.AppendLine(sqlFrom.ToString());
                            sql.AppendLine("ORDER BY UPPER(A.NOME)");

                            using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                            {
                                while (await reader.ReadAsync())
                                {
                                    var obj = new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["NOME"])
                                    };
                                    retorno.results.Add(obj);
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        //api para uso apenas da tela de notificacoes
        [HttpPost()]
        [Route("api/Funcionarioapi/GetComboAllUsuarios")]
        public async Task<ObjCombo<String>> GetComboAllUsuarios(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM ASPNETUSERS B ");
                    sqlFrom.AppendLine($" LEFT JOIN TB_FUNCIONARIO A ON (B.ID_FUNCIONARIO = A.ID) ");


                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO IN (0,4) ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_SUPERVISOR = {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_FILIAL= {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOME) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }



                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(B.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }


                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT B.ID, A.NUM_FRE, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");


                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new FuncionarioComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Num_fre = Convert.ToString(reader["NUM_FRE"]),
                                    Text = Convert.ToString(reader["NOME"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/Funcionarioapi/GetComboVendedor")]
        public async Task<ObjCombo<Int64>> GetComboVendedor(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_FUNCIONARIO A ");
                    sqlFrom.Append(" LEFT JOIN ASPNETUSERS B ");
                    sqlFrom.Append(" ON (B.ID_FUNCIONARIO = A.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO IN (0,4) ");
                        condicoes.Add($" A.VENDEDOR = 1 ");
                        condicoes.Add($" B.SECAO_CONNECT = 1 ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_SUPERVISOR = {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_FILIAL= {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOME) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} )");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }


                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME, B.USERNAME, A.ID_SUPERVISOR, B.ID_DEPARTAMENTO, A.NUM_FRE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new VendedorComboModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["USERNAME"])} - {Convert.ToString(reader["NOME"])}",
                                    Id_Supervisor = reader["ID_SUPERVISOR"] != DBNull.Value ? Convert.ToInt64(reader["ID_SUPERVISOR"]) : (Int64?)null,
                                    Id_Departamento = reader["ID_DEPARTAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_DEPARTAMENTO"]) : (Int64?)null,
                                    Nome = Convert.ToString(reader["NOME"]),
                                    Num_Fre = Convert.ToString(reader["NUM_FRE"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/Funcionarioapi/GetComboFuncionarioSupervisor")]
        public async Task<ObjCombo<long>> GetComboFuncionarioSupervisor(RequestObjComboAvo<long?, long?, long?> request)
        {
            var retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                //Filtros
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO A");

                var condicoes = new List<String>();

                if (request.soativos)
                {
                    condicoes.Add($"A.SITUACAO IN (0,4) ");
                    condicoes.Add($"A.E_SUPERVISOR = 1 ");
                }

                if (request.id.HasValue)
                {
                    condicoes.Add($"A.ID = {request.id}");
                }

                if (request.idpai.HasValue)
                {
                    condicoes.Add($"A.ID_SUPERVISOR= {request.idpai}");
                }

                if (request.idavo.HasValue)
                {
                    condicoes.Add($"A.ID_FILIAL= {request.idavo}");
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"(UPPER(A.NOME) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%') ");
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                }

                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            retorno.total_count = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT A.ID, A.NOME, A.NUM_FRE ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY UPPER(A.NOME)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResultSupervisor
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Text = Convert.ToString(reader["NOME"]),
                                CodSuperv = Convert.ToString(reader["NUM_FRE"])
                            };

                            retorno.results.Add(obj);
                        }
                    }

                }
            }

            return retorno;
        }

        // api especifica da tela de campanha ///////////////////
        [HttpPost()]
        [Route("api/Funcionarioapi/GetComboSupervisor")]
        public async Task<ObjCombo<Int64>> GetComboSupervisor(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO A LEFT JOIN ASPNETUSERS B ON (B.ID_FUNCIONARIO = A.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAO IN (0,4) ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_SUPERVISOR= {request.idpai}");
                    }

                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"A.ID_FILIAL= {request.idavo} ");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"UPPER(A.NOME) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%' ");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE B.TiPO_USUARIO = 2 AND ( {String.Join(" AND ", condicoes)} ) ");
                    }


                    //if (request.idavo.HasValue)
                    //{
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, B.USERNAME, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboSupervisor_Funcionario
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["USERNAME"])} - {Convert.ToString(reader["NOME"])}",
                                    Nome = Convert.ToString(reader["NOME"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                    //}




                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/Funcionarioapi/FindByNumFre")]
        public async Task<GetApiRetorno<FuncionarioCadastroModel>> FindByNumFre(String NumFre)
        {
            GetApiRetorno<FuncionarioCadastroModel> retorno = new GetApiRetorno<FuncionarioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoIdentity = new Conexao<IdentityUser>(session))
            {
                try
                {
                    var user = await repoIdentity.selectFirst(x => x.Id == NumFre);

                    if (user == null)
                        throw new Exception("Usuario não foi encontrado");

                    if (user.Funcionario == null)
                        throw new Exception("Usuário não possuí registro de funcionário");

                    var funcionario = await repoFuncionario.selectFirst(f => f.Id == user.Funcionario.Id);

                    if (funcionario == null)
                        throw new Exception("Funcionario não foi encontrado");

                    retorno.Objeto = GetViewObj(funcionario, session);

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

    }

}
