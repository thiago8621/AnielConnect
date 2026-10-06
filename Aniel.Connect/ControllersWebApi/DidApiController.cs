using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Integracoes.Saperx.Services;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class DidApiController : BaseApiController
    {
        /// <summary>
        /// Get All Did
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        [Route("api/DidApi/GetAll")]
        public async Task<DataTableResponse<DidListaModel>> GetAll(Did_DataTableRequest request)
        {
            List<DidListaModel> retorno = new List<DidListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_DID A "),
                                  sqlWhere = new StringBuilder("WHERE A.NUMERO IS NOT NULL ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlWhere.AppendLine($"AND ((UPPER(A.NUMERO) LIKE '%{request.search.value.ToUpper()}%')) ");
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sqlWhere.AppendLine(" AND A.ID_CIDADE = @CIDADE ");
                        parametros.Add("@CIDADE", request.Cidade);
                    }
                    if (!String.IsNullOrEmpty(request.Uf))
                    {
                        sqlWhere.AppendLine(" AND A.UF =  @UF ");
                        parametros.Add("@UF", request.Uf);
                    }
                    if (!String.IsNullOrEmpty(request.Pais))
                    {
                        sqlWhere.AppendLine(" AND A.ID_PAIS = @PAIS ");
                        parametros.Add("@PAIS  ", request.Pais);
                    }
                    if (!String.IsNullOrEmpty(request.Numero))
                    {
                        sqlWhere.AppendLine(" AND A.NUMERO = @NUMERO ");
                        parametros.Add("@NUMERO", request.Numero);
                    }
                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sqlWhere.AppendLine(" AND A.STATUS_DID = @STATUS ");
                        parametros.Add("@STATUS", request.Status);
                    }
                    if (parametros.Count() > 0)
                        sqlCount.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    StringBuilder sql = new StringBuilder(@"SELECT A.ID,A.NUMERO,A.STATUS_DID,A.RESERVADO_DDR, B.DESCRICAO AS CIDADE, C.NOME AS PAIS, A.UF
                                                            FROM TB_DID A
                                                            LEFT JOIN TB_CIDADE B ON (A.ID_CIDADE = B.ID)
                                                            LEFT JOIN TB_PAIS C ON (A.ID_PAIS = C.ID)");
                    sql.AppendLine(sqlWhere.ToString());



                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new DidListaModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Numero = Convert.ToString(reader["NUMERO"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<StatusDid>(Convert.ToString(reader["STATUS_DID"])),
                                Uf = Convert.ToString(reader["UF"]),
                                Reservado_DDR = Convert.ToBoolean(reader["RESERVADO_DDR"]) ? Resources.RsBase.SIM : Resources.RsBase.NAO,
                            });
                        }
                    }
                }

            }

            return new DataTableResponse<DidListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno
            };

        }

        /// <summary>
        /// Did FindById 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns> DidCadastroModel Obj </returns>
        [HttpGet()]
        [Route("api/DidApi/FindById")]
        public async Task<GetApiRetorno<DidCadastroModel>> FindById(Int64 Id)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Did>(session))
            {
                try
                {
                    Did circuitoTelefoniaModel = await repository.selectFirst(p => p.Id == Id);
                    DidCadastroModel retorno = GetViewObj(circuitoTelefoniaModel);
                    return new GetApiRetorno<DidCadastroModel>
                    {
                        Ok = true,
                        Mensagem = "",
                        InnerException = "",
                        Objeto = retorno
                    };
                }
                catch (Exception ex)
                {
                    return new GetApiRetorno<DidCadastroModel>
                    {
                        Ok = false,
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty
                    };
                }
            }
        }

        [HttpPost()]
        [Route("api/DidApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<DidCadastroModel> value)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Did>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {

                    Did did = null,
                        modelo_Anterior = null;

                    if (value.Objeto.Id == 0)
                    {
                        did = new Did();

                        did.UsuarioCriacao = value.Usuario;
                        did.DataHoraCriacao = DateTime.Now;

                    }
                    else
                    {
                        did = await repository.selectFirst(p => p.Id == value.Objeto.Id);
                        modelo_Anterior = (Did)did.Clone();
                        did.UsuarioAlteracao = value.Usuario;
                        did.DataHoraAlteracao = DateTime.Now;

                    }

                    BindViewObj(value.Objeto, did, session);




                    await repository.SaveOrUpdate(did);


                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        if (value.Objeto.Id == 0)
                        {
                            var response = await Integracao_Cadastro_DID(did);
                            did.CodigoAuxiliar = response?.result?.id;
                            await repository.Update(did);
                        }
                        else if (String.IsNullOrEmpty(did.CodigoAuxiliar) == false)
                        {
                            await Integracao_Editar_DID(did);
                        }
                    }



                    if (modelo_Anterior != null)
                    {
                        using (var repositoryLog = new Conexao<Log_Did>(sessionLog))
                        {
                            did.UsuarioAlteracao = value.Usuario;
                            var log = did.GeraLog(modelo_Anterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLog.Save(log);
                        }
                    }

                    else
                    {
                        using (var repositoryLog = new Conexao<Log_Did>(sessionLog))
                        {
                            var log = did.GeraLog(modelo_Anterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                            await repositoryLog.Save(log);
                        }

                    }


                    return new PostApiRetorno<long>
                    {
                        IdObjeto = did.Id,
                        Ok = true,
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno<long>
                {
                    Mensagem = ex.Message,
                    Ok = false
                };
            }
        }

        private DidCadastroModel GetViewObj(Did did)
        {
            return new DidCadastroModel
            {
                Id = did.Id,
                ChamadaCobrar = did.ChamadaCobrar,
                Numero = did.Numero,
                DiaLiberado = did.DiaLiberado.HasValue ? did.DiaLiberado?.ToString("dd/MM/yyyy") : String.Empty,
                IdCidade = did.Cidade?.Id,
                IdPais = did.Pais?.Id,
                IdRota = did.RotaTelefonia?.Id,
                StatusDid = did.StatusDid,
                MotivoIndisponivel = did.MotivoIndisponivel,
                TipoDiscagem = did.TipoDiscagem,
                UF = did.UF,
                Valor = did.Valor.HasValue ? did.Valor?.ToString("N2") : String.Empty,
                Reservado_Ddr = did.Reservado_Ddr ? Padrao.Sim : Padrao.Nao
            };
        }

        private Did BindViewObj(DidCadastroModel viewModel, Did model, ISession session)
        {
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoRota = new Conexao<RotaTelefonia>(session))
            {
                model.Pais = repoPais.selectFirst(p => p.Id == viewModel.IdPais).Result;
                model.ChamadaCobrar = viewModel.ChamadaCobrar;
                model.Cidade = repoCidade.selectFirst(p => p.Id == viewModel.IdCidade).Result;
                model.DiaLiberado = String.IsNullOrEmpty(viewModel.DiaLiberado) ? (DateTime?)null : Convert.ToDateTime(viewModel.DiaLiberado);
                model.UF = viewModel.UF;
                model.RotaTelefonia = repoRota.selectFirst(p => p.Id == viewModel.IdRota).Result;
                model.StatusDid = viewModel.StatusDid;
                model.Numero = viewModel.Numero;
                model.MotivoIndisponivel = viewModel.MotivoIndisponivel;
                model.TipoDiscagem = viewModel.TipoDiscagem;
                model.Valor = Convert.ToDouble(viewModel.Valor);
                model.Reservado_Ddr = viewModel.Reservado_Ddr == Padrao.Sim;
            }

            return model;

        }
        /// <summary>
        /// Método para gravação de Dids em grande quantidade
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DidApi/Gravar_Intervalo")]
        public async Task<PostApiRetorno<List<long>>> Gravar_Intervalo(ParamWebApi<Did_CadastroIntervaloModel> request)
        {
            PostApiRetorno<List<long>> retorno = new PostApiRetorno<List<long>> { IdObjeto = new List<long>() };
            List<Did> Dids_Intervalo = new List<Did>();
            string parametros = $"({request.Objeto.Prefixo})-";

            request.Objeto.Numero_Inicial = request.Objeto.Numero_Inicial.Replace(" ", string.Empty).Replace($"({request.Objeto.Prefixo})", string.Empty);
            request.Objeto.Numero_Final = request.Objeto.Numero_Final.Replace(" ", string.Empty).Replace($"({request.Objeto.Prefixo})", string.Empty);


            List<string> Numeros = Get_List_Intervalo(request.Objeto.Numero_Inicial, request.Objeto.Numero_Final, parametros, request.Objeto.Prefixo);
            try
            {
                //List<String> Numeros = new List<String>();

                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Did>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                using (var repoCidade = new Conexao<Cidade>(session))
                using (var repoPais = new Conexao<Pais>(session))
                {
                    foreach (string Number in Numeros)
                    {
                        var existente = await repo.selectFirst(p => p.Numero == Number);

                        if (existente == null)
                        {

                            DidCadastroModel obj = new DidCadastroModel
                            {
                                Id = await repo.Max(p => p.Id) + 1,
                                Numero = Number,
                                IdPais = request.Objeto.IdPais,
                                IdCidade = request.Objeto.IdCidade,
                                IdRota = request.Objeto.IdRota,
                                UF = request.Objeto.UF,
                                ChamadaCobrar = true,
                                DiaLiberado = request.Objeto.DiaLiberado,
                                Valor = request.Objeto.Valor,
                                MotivoIndisponivel = request.Objeto.MotivoIndisponivel,
                                TipoDiscagem = request.Objeto.TipoDiscagem,
                                StatusDid = request.Objeto.StatusDid,

                            };
                            Did did = BindViewObj(obj, new Did { UsuarioCriacao = request.Usuario, DataHoraCriacao = DateTime.Now }, session);
                            Dids_Intervalo.Add(did);
                            request.Objeto.Log_DidIntervalo.Add(new Did_LogMensagem { Numero = Number, Mensagem = "Numero Criado Com Sucesso" });

                        }
                        else { request.Objeto.Log_DidIntervalo.Add(new Did_LogMensagem { Numero = Number, Mensagem = "Número já existe" }); }
                        //retorno.IdObjeto.AddRange((List<long?>)Dids_Intervalo.Select(p => p.Id));
                    }

                    await repo.Save(Dids_Intervalo);
                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);

                    foreach (var did in Dids_Intervalo)
                    {
                        if (integracao != null)
                        {

                            var response = await Integracao_Cadastro_DID(did);
                            did.CodigoAuxiliar = response?.result?.id;
                            await repo.Update(did);

                        }
                    }


                    retorno.IdObjeto.AddRange(Dids_Intervalo.Select(p => p.Id));
                    retorno.Ok = true;

                }
            }
            catch (Exception e)
            {
                retorno.Ok = false;
                retorno.Mensagem = e.Message;
                retorno.InnerException = !String.IsNullOrEmpty(e.InnerException.Message) ? e.InnerException.Message : String.Empty;
            }
            return retorno;
        }
        /// <summary>
        /// Método responsável por formar uma lista de valores a serem cadastrados no banco de dados.
        /// </summary>
        /// <param name="valor_Inicial">Primeiro valor da sequencia</param>
        /// <param name="valor_Final">último valor da Sequência</param>
        /// <param name="parametros">Parâmetros a serem editados</param>
        /// <returns>Lista de Numeros de Did</returns>
        private List<String> Get_List_Intervalo(String valor_Inicial, String valor_Final, String parametros, string prefixo)
        {
            var retorno = new List<String>();
            var indexes = new List<long>();
            string placeholder = "(00)00000-0000";
            prefixo = prefixo.Replace("(", "").Replace(")", "");

            foreach (Char param in parametros)
            {
                indexes.Add(placeholder.IndexOf(param));
                valor_Inicial = valor_Inicial.Replace($"{param}", "");
                valor_Final = valor_Final.Replace($"{param}", "");
            }
            long Inicial = Convert.ToInt64(valor_Inicial), Final = Convert.ToInt64(valor_Final);
            var intervalo = Final >= Inicial ? Final - Inicial : Inicial - Final;
            for (long iterator = 0; iterator <= intervalo; ++iterator)
            {
                retorno.Add(Convert.ToString(valor_Inicial + iterator));
            }
            //Func<String, String, long, String> print = (p, y, z) => p.Insert(Convert.ToInt32(z), y);

            retorno = retorno.Select(p => prefixo + p).ToList();
            Func<String, String> print = (p) => p.Insert(0, "(").Insert(3, ")").Insert(9, "-");
            retorno = retorno.Select(print).ToList();


            return retorno;
        }

        //private List<String> Get_List_Did_Auto(Did Dids, Venda vendas)
        //{
        //    var numeros = Dids.Numero;
        //    var cidade = Dids.Cidade;
            
            
        //    foreach (item in Dids)
        //    {
                
        //    }
        //    long Inicial = Convert.ToInt64(valor_Inicial), Final = Convert.ToInt64(valor_Final);
        //    var intervalo = Final >= Inicial ? Final - Inicial : Inicial - Final;
        //    for (long iterator = 0; iterator <= intervalo; ++iterator)
        //    {
        //        retorno.Add(Convert.ToString(valor_Inicial + iterator));
        //    }
        //    //Func<String, String, long, String> print = (p, y, z) => p.Insert(Convert.ToInt32(z), y);

        //    retorno = retorno.Select(p => prefixo + p).ToList();
        //    Func<String, String> print = (p) => p.Insert(0, "(").Insert(3, ")").Insert(9, "-");
        //    retorno = retorno.Select(print).ToList();


        //    return retorno;
        //}

        //[HttpPost()]
        //[Route("api/bancoapi/GetCombo")]
        //public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        //{
        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sqlFrom = new StringBuilder("FROM TB_BANCO A ");

        //            List<String> condicoes = new List<String>();

        //            if (request.soativos)
        //            {
        //                condicoes.Add($"STATUS = 1");
        //            }

        //            if (request.id.HasValue)
        //            {
        //                condicoes.Add($"UPPER(A.ID) = {request.id}");
        //            }

        //            if (request != null && !String.IsNullOrEmpty(request.q))
        //            {
        //                condicoes.Add($"( UPPER(A.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 5)}%' OR " +
        //                              $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
        //            }

        //            if (condicoes.Count() > 0)
        //            {
        //                sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
        //            }

        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
        //                sql.AppendLine(sqlFrom.ToString());

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    if (await reader.ReadAsync())
        //                        retorno.total_count = Convert.ToInt32(reader["CO"]);
        //                }
        //            }

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT A.ID, A.CODIGO, A.DESCRICAO ");
        //                sql.AppendLine(sqlFrom.ToString());
        //                sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

        //                using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var obj = new ObjComboResult<Int64>
        //                        {
        //                            Id = Convert.ToInt64(reader["ID"]),
        //                            Text = $"{Convert.ToString(reader["CODIGO"])} - {Convert.ToString(reader["DESCRICAO"])}"
        //                        };
        //                        retorno.results.Add(obj);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retorno;

        //}
        /// <summary> Insercao de Novos Registros de Did via Planilha Excel </summary>
        /// <param name="item">Objeto da View</param>
        /// <param name="did">Ojbeto a ser salvo no Banco</param>
        public async Task<List<ExcelServiceLogItem>> Importar_Did(Did_PlanilhaImportacao item, Did did)
        {
            var retorno = new List<ExcelServiceLogItem>();
            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoPais = new Conexao<Pais>(session))
                using (var repoCidade = new Conexao<Cidade>(session))
                using (var repoDid = new Conexao<Did>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                {
                    did.ChamadaCobrar = item.ChamadaCobrar;
                    did.Cidade = await repoCidade.selectFirst(p => p.Descricao == item.Cidade);
                    did.DiaLiberado = item.DiaLiberado;
                    did.MotivoIndisponivel = item.MotivoIndisponivel;
                    did.Numero = item.Numero;
                    did.Pais = await repoPais.selectFirst(p => p.Nome == item.Pais);
                    //did.RotaTelefonia = item.rot;
                    did.StatusDid = item.StatusDid;
                    did.TipoDiscagem = item.TipoDiscagem;
                    did.UF = (await repoCidade.selectFirst(p => p.UF == item.UF)).UF;
                    did.Valor = item.Valor;
                    await repoDid.SaveOrUpdate(did);

                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        var response = await Integracao_Cadastro_DID(did);
                        did.CodigoAuxiliar = response?.result?.id;
                        await repoDid.Update(did);

                    }

                }
                if (String.IsNullOrEmpty(item.Numero))
                {
                    retorno.Add(new ExcelServiceLogItem { Chave = item.Numero, Erro = "Numero Ausente" });
                }


            }
            return retorno;
        }
        /// <summary>
        /// Rotina Automática de Atualização de Dids que estão em Quarentena
        /// </summary>
        /// <returns>Lista de Ids em Quarentena</returns>
        public async Task<PostApiRetorno<List<long>>> Atualizar_Did_Automatico()
        {
            PostApiRetorno<List<long>> retorno = new PostApiRetorno<List<Int64>>();
            List<Did> Dids_Quarentena = new List<Did>();
            DateTime Dia = DateTime.Now;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNH = HibernateUtil.GetSession())
                using (System.Data.Common.DbCommand comm = session.Connection.CreateCommand())
                using (System.Data.Common.DbCommand commUpdate = sessionNH.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT A.ID, A.STATUS_DID, A.DIA_LIBERADO FROM TB_DID A WHERE A.STATUS_DID='6' "),
                        sqlUp = new StringBuilder();
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            Dids_Quarentena.Add(new Did
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                StatusDid = (StatusDid)Enum.Parse(typeof(StatusDid), Convert.ToString(reader["STATUS_DID"])),
                                DiaLiberado = Convert.ToDateTime(reader["DIA_LIBERADO"]),
                            });
                        }

                    }
                    //Dids_Quarentena = Dids_Quarentena.Where(p => p.DiaLiberado <= Dia).ToList();
                    List<long> Ids = Dids_Quarentena.Where(p => p.DiaLiberado <= Dia).Select(p => p.Id).ToList();
                    if (Ids.Count > 0)
                    {
                        retorno.IdObjeto.AddRange(Ids);
                        sqlUp.AppendLine($"UPDATE INTO TB_DID SET STATUS_DID = '1', DIA_LIBERADO = NULL WHERE ID IN ({String.Join(",", Ids)}) ");
                        await CommandHelper.ExecuteNonQuery(commUpdate, sqlUp, sessionNH.Connection, null);
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }

        /// <summary> Operacao Interna de mudança do Did para o Estado de Quarentena, uma vez que a venda foi cancelada </summary>
        /// <param name="planosprodutos"> Planos e Produtos oriundos da Venda Cancelada</param>
        //public async void Set_Did_Quarentena(List<Cliente_PlanoProduto> planosprodutos)
        //{
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            using (Conexao<CircuitoTelefonia> repoCircuito = new Conexao<CircuitoTelefonia>(session))
        //            using (Conexao<Did> repoDid = new Conexao<Did>(session))
        //            using (Conexao<CircuitoTelefoniaDid> repoDidsInCircuito = new Conexao<CircuitoTelefoniaDid>(session))
        //            using (Conexao<Configuracoes_Gerais> repoConfig = new Conexao<Configuracoes_Gerais>(session))
        //            //using (Conexao<Cliente_PlanoProduto> repoClientePlanoProd = new Conexao<Cliente_PlanoProduto>(session))
        //            {
        //                var config = await repoConfig.selectFirst();
        //                DateTime Dia_Liberado = DateTime.Now.AddDays(Convert.ToDouble(config.Dias_Liberacao_Did));
        //                List<long> query = planosprodutos
        //                    .Where(p => p.Status == Status_PlanoProduto.Venda_Cancelada)
        //                    .Select(p => p.Id)
        //                    .ToList();

        //                List<CircuitoTelefonia> Circuitos = (await repoCircuito.select(p => p.Clientes_PlanosProdutos == p.Clientes_PlanosProdutos
        //                                                                       .Where(param => param.Id.IsIn(query)))).ToList();

        //                List<long> Circuitos_Did_Quarentena = (await repoDidsInCircuito.select(p => p.CircuitoTelefonia.IsIn(Circuitos)))
        //                                                                        .Select(p => p.Did.Id)
        //                                                                        .ToList();
        //                List<Did> Dids = (await repoDid.select(p => p.Id.IsIn(Circuitos_Did_Quarentena)))
        //                    .Select(p => { p.StatusDid = StatusDid.Quarentena; p.DiaLiberado = Dia_Liberado; return p; })
        //                    .ToList();
        //                await repoDid.SaveOrUpdate(Dids);

        //            }
        //        }
        //    }
        //    catch (Exception erro)
        //    {
        //        string messages = erro.InnerException != null ? $"{erro.Message} - {erro.InnerException.Message}" : $"{erro.Message}";
        //    }
        //}
        [HttpPost]
        [Route("api/DidApi/GetComboNum_Did")]
        public async Task<ObjCombo<Int64>> GetComboNum_Did(RequestObjComboPai<Int64?,string> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_DID A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS_DID = 1");

                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request.idpai == "DDR")
                    {
                        condicoes.Add($"A.RESERVADO_DDR = 1");
                    }
                    else if (request.idpai == "DID")
                    {
                        condicoes.Add($"COALESCE(A.RESERVADO_DDR,0) = 0");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.NUMERO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NUMERO, A.VALOR ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.ID)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new DidComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NUMERO"]),
                                    Valor = String.Format("{0:N2}", reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]) : 0)
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
        [HttpGet()]
        [Route("api/DidApi/Get_Random_Did")]
        public async Task<GetApiRetorno<List<Did>>> Get_Random_Did(Did_Random_ApiRequest request, ISession session)
        {
            GetApiRetorno<List<Did>> retorno = new GetApiRetorno<List<Did>>();
            try
            {

                using (Conexao<Did> repoDid = new Conexao<Did>(session))

                {
                    Random random = new Random();

                    List<Did> dids = (await repoDid.select(p => p.StatusDid == StatusDid.Disponivel)).ToList(),
                              ListaDidsFinal = new List<Did>();

                    Dictionary<ValueTuple<String, bool>, List<Did>> Registros = new Dictionary<ValueTuple<String, bool>, List<Did>>();

                    if (request != null)
                    {
                        Registros = await GetDidItem_FilterValue(request,dids,session);
                        dids = Registros.Where(p => p.Key.Item2 == true).Select(p => p.Value).First();
                        Int32 numero_DidsFinal = dids.Count();
                        if (request.NumeroDids <= numero_DidsFinal)
                        {

                            for (int i = 0; i < request.NumeroDids; i++)
                            {
                                Int32 R_index = random.Next(dids.Count());
                                ListaDidsFinal.Add(dids[R_index]);
                            }
                            retorno.Objeto = ListaDidsFinal;

                        }

                    }

                }
                if (retorno.Objeto == null) 
                {
                    throw new Exception("Não foi possível obter a quantidade de dids requisitados no Plano");
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException += erro.InnerException?.Message;
            }
            return retorno;
        }
        public async Task<Dictionary<ValueTuple<String, Boolean>, List<Did>>> GetDidItem_FilterValue(Did_Random_ApiRequest request, List<Did> dids, ISession session)
        {
            var Registros = new Dictionary<ValueTuple<String, bool>, List<Did>>();
            using (Conexao<Cidade> repoCidade = new Conexao<Cidade>(session))
            using (Conexao<Uf> repoUf = new Conexao<Uf>(session))
            using (Conexao<Pais> repoPais = new Conexao<Pais>(session))
            {
                Uf uf_Request = await repoUf.selectFirst(p => p.Sigla == request.UF);
                Cidade cidade_Request = await repoCidade.selectFirst(p => p.Id == Convert.ToInt64(request.Id_Cidade));
                Pais Pais_Request = await repoPais.selectFirst(p => p.Id == Convert.ToInt64(request.Id_Pais));
                List<Did> dids_Filtered = new List<Did>();
                dids_Filtered.AddRange(dids);
                long Num_Dids = 0;

                string CidadeText = "Cidade",
                       PaisText = "Pais",
                       UfText = "Uf";

                dids_Filtered = dids.Where(p => p.Cidade == cidade_Request && p.UF == uf_Request.Sigla && p.Pais == Pais_Request).ToList();
                Num_Dids = dids_Filtered.Count();

                if (Num_Dids == 0)
                {
                    Registros.Add(ValueTuple.Create(String.Format("{0}-{1}-{2}", CidadeText, PaisText, UfText), false), dids);

                    dids_Filtered = dids.Where(p => p.UF == request.UF && p.Pais == Pais_Request).ToList();
                    Num_Dids = dids_Filtered.Count();

                    if (Num_Dids == 0)
                    {

                        Registros.Add(ValueTuple.Create(String.Format("{0}-{1}", UfText, PaisText), false), dids);

                        dids_Filtered = dids.Where(p => p.Cidade == cidade_Request && p.Pais == Pais_Request).ToList();
                        Num_Dids = dids_Filtered.Count();

                        if (Num_Dids == 0)
                        {
                            Registros.Add(ValueTuple.Create(String.Format("{0}-{1}", CidadeText, PaisText), false), dids);

                            dids_Filtered = dids.Where(p => p.Pais == Pais_Request).ToList();
                            Num_Dids = dids_Filtered.Count();

                            if (Num_Dids == 0)
                            {
                                throw new Exception("Did Indisponível em seu País");
                            }

                            else
                            {
                                Registros.Add(
                                    ValueTuple.Create(String.Format("{0}", PaisText), true),
                                    dids_Filtered);
                            }
                        }

                        else
                        {
                            Registros.Add(
                                ValueTuple.Create(String.Format("{0} - {1}", CidadeText, PaisText), true),
                                dids_Filtered);
                        }

                    }

                    else
                    {
                        Registros.Add(
                            ValueTuple.Create(String.Format("{0} - {1}", UfText, PaisText), true),
                            dids_Filtered);
                    }

                }

                else
                {
                    Registros.Add(
                        ValueTuple.Create(String.Format("{0} - {1} - {2}", CidadeText, PaisText, UfText), true),
                        dids_Filtered);
                }
            }
            return Registros;

        }

        /// <summary>
        /// Ao selecionar o item no GetCombo, esse será meu numero inicial, a partir dele procurar a sequencia de dids reservados para ddr
        /// </summary>
        /// <param name="NumeroInicial">Número origina-se do GetCombo de Ddr</param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/DidApi/CapturarSequenciaDdr")]
        public async Task<GetApiRetorno<requestDDrSequenciaModel>> CapturarSequenciaDdr(string NumeroInicial)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            GetApiRetorno<requestDDrSequenciaModel> retorno = new GetApiRetorno<requestDDrSequenciaModel>();

            String buscaDDR = $"%{NumeroInicial.Replace("", "____")}%";

            StringBuilder sql = new StringBuilder(),
                          sqlCount = new StringBuilder();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {

                    parametros.Add("@BUSCA_NUMERO_DDR", buscaDDR);
                    using (var command = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendLine(" SELECT COUNT(A.id) AS CO FROM TB_DID A   WHERE  ")
                                .AppendLine(" (A.RESERVADO_DDR = 1 AND A.STATUS_DID = 1 ")
                                .AppendLine(" AND A.numero LIKE @BUSCA_NUMERO_DDR) ");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(command, sqlCount, parametros, session.Connection, null, 0, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto.Numero_ItensDDr = Convert.ToInt64(reader["CO"]);
                            }
                        }
                    }
                    using (var command = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(" SELECT A.ID, A.NUMERO FROM TB_DID A ")
                           .AppendLine(" WHERE( A.RESERVADO_DDR = 1 AND A.STATUS_DID = 1 AND A.NUMERO LIKE @BUSCA_NUMERO_DDR) ")
                           .AppendLine(" ORDER BY A.NUMERO ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(command, sql, parametros, session.Connection, null, 0, null))
                        {
                            List<object> ItensDDr = new List<object>();
                            while (await reader.ReadAsync())
                            {
                                //retorno.Objeto.Numero_ItensDDr = Convert.ToInt64(reader["CO"]);
                                ItensDDr.Add(new
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Numero = Convert.ToString(reader["NUMERO"])
                                });
                            }
                            retorno.Objeto.JsonItensSequenciaDDR = JsonService.SerializeObject(ItensDDr);
                        }
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }
            return retorno;

        }


        /// <summary>
        /// Tentaiva de Api de Integração dos Dids
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DidIntegracaoResponse> Integracao_Cadastro_DID(Did obj)
        {
            DidIntegracaoSaperxService integracao = new DidIntegracaoSaperxService();
            return integracao.Cadastrar(obj);

        }

        public async Task<DidIntegracaoResponse> Integracao_Editar_DID(Did obj)
        {
            DidIntegracaoSaperxService integracao = new DidIntegracaoSaperxService();
            return integracao.Atualizar(obj);

        }


    }
}