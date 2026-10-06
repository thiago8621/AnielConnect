using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Resources;
using Aniel.Connect.Integracoes.Saperx.Services;
using Aniel.Connect.Integracoes.Saperx.Models;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class CircuitoTelefoniaApiController : BaseApiController
    {
        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/CircuitoTelefoniaApi/GetAll")]
        public async Task<DataTableResponse<CircuitoTelefoniaListaModel>> GetAll(CircuitoTelefonia_DataTableRequest request)
        {
            List<CircuitoTelefoniaListaModel> retorno = new List<CircuitoTelefoniaListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_CIRCUITO_TELEFONIA A");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }


                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT A.ID,A.STATUS,D.NOMEFANTASIA,C.DESCRICAO,A.NUMERO_SIP
                                                            FROM TB_CIRCUITO_TELEFONIA A
                                                            LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ON (A.ID_CLIENTE_PLANO_PRODUTO = B.ID)
                                                            LEFT JOIN TB_PLANO C ON (B.ID_PLANO = C.ID)
                                                            LEFT JOIN TB_CLIENTE D ON (B.ID_CLIENTE = D.ID)");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {

                        sql.AppendLine(" WHERE (UPPER(A.NUMERO_SIP) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CircuitoTelefoniaListaModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Numerosip = Convert.ToString(reader["NUMERO_SIP"]),
                                Cliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                Plano = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }
                }

            }

            return new DataTableResponse<CircuitoTelefoniaListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno
            };

        }

        [HttpPost()]
        [Route("api/CircuitoTelefoniaApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_CIRCUITO_TELEFONIA A
                                                                 LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ON (A.ID_CLIENTE_PLANO_PRODUTO = B.ID)
                                                                 LEFT JOIN TB_CLIENTE C ON ( C.ID = B.ID_CLIENTE)"),
                                  sqlCount = new StringBuilder("SELECT COUNT(A.ID) AS CO "),
                                  sql = new StringBuilder("SELECT A.ID, C.NOMEFANTASIA ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(C.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(C.NOMEFANTASIA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {


                                retorno.results.Add(new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOMEFANTASIA"])
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
        [Route("api/CircuitoTelefoniaApi/GetComboSIP")]
        public async Task<ObjCombo<Int64>> GetComboSIP(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_CIRCUITO_TELEFONIA A
                                                                 LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ON (A.ID_CLIENTE_PLANO_PRODUTO = B.ID)
                                                                 LEFT JOIN TB_CLIENTE C ON ( C.ID = B.ID_CLIENTE)"),
                                  sqlCount = new StringBuilder("SELECT COUNT(A.ID) AS CO "),
                                  sql = new StringBuilder("SELECT A.ID, A.NUMERO_SIP ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"((A.NUMERO_SIP) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY (A.ID)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {


                                retorno.results.Add(new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NUMERO_SIP"])
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
        [Route("api/CircuitoTelefoniaApi/GetComboDidVinculado")]
        public async Task<ObjCombo<Int64>> GetComboDidVinculado(RequestObjComboPai<Int64?, long?> request)
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

                    //Select A.id_did From tb_circuito_telefonia_Did A Left Join tb_Did B on A.Id_Did=B.Id
                    StringBuilder sqlFrom = new StringBuilder(@"  FROM TB_DID B
                                                                  LEFT JOIN TB_CIRCUITO_TELEFONIA C ON B.ID_CIRCUITO_TELEFONIA = C.ID"),
                                  sqlCount = new StringBuilder("SELECT COUNT(B.ID) AS CO "),
                                  sql = new StringBuilder("SELECT B.ID, B.NUMERO AS DID");

                    List<String> condicoes = new List<String>();

                    //if (request.soativos)
                    //{
                    //    condicoes.Add($"A.STATUS = 1");
                    //}

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"B.ID = {request.id}");
                    }
                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"C.ID = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(C.NOMEFANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlFrom.ToString());
                        //sql.AppendLine("ORDER BY UPPER(C.NOMEFANTASIA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                retorno.results.Add(new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DID"])
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
        [Route("api/CircuitoTelefoniaApi/FindById")]
        public async Task<GetApiRetorno<CircuitoTelefoniaCadastroModel>> FindById(Int64 Id)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<CircuitoTelefonia>(session))
            {
                try
                {
                    CircuitoTelefonia circuitoTelefoniaModel = await repository.selectFirst(p => p.Id == Id);
                    CircuitoTelefoniaCadastroModel retorno = GetViewObj(circuitoTelefoniaModel, session);
                    return new GetApiRetorno<CircuitoTelefoniaCadastroModel>
                    {
                        Ok = true,
                        Mensagem = "",
                        InnerException = "",
                        Objeto = retorno
                    };
                }
                catch (Exception ex)
                {
                    return new GetApiRetorno<CircuitoTelefoniaCadastroModel>
                    {
                        Ok = false,
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty
                    };
                }
            }
        }

        [HttpPost()]
        [Route("api/CircuitoTelefoniaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<CircuitoTelefoniaCadastroModel> value)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repository = new Conexao<CircuitoTelefonia>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                {

                    CircuitoTelefonia circuitoTelefonia = null;
                    CircuitoTelefonia modeloAnterior = null;
                    if (value.Objeto.Id == 0)
                    {
                        circuitoTelefonia = new CircuitoTelefonia();
                        circuitoTelefonia.UsuarioCriacao = value.Usuario;
                        circuitoTelefonia.DataHoraCriacao = DateTime.Now;
                        circuitoTelefonia.Status = Status_Default.Ativo;
                    }
                    else
                    {
                        circuitoTelefonia = await repository.selectFirst(p => p.Id == value.Objeto.Id);
                        circuitoTelefonia.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        circuitoTelefonia.UsuarioAlteracao = value.Usuario;
                        circuitoTelefonia.DataHoraAlteracao = DateTime.Now;
                        modeloAnterior = (CircuitoTelefonia)circuitoTelefonia.Clone();
                    }

                    BindViewObj(value.Objeto, circuitoTelefonia, session);
                    await repository.SaveOrUpdate(circuitoTelefonia);

                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        if (value.Objeto.Id == 0)
                        {
                            await Integracao_Cadastro_Circuito(circuitoTelefonia);
                        }
                        else if (String.IsNullOrEmpty(circuitoTelefonia.CodigoAuxiliar) == false)
                        {
                            await Integracao_Editar_Circuito(circuitoTelefonia);
                        }
                    }

                    return new PostApiRetorno<long>
                    {
                        IdObjeto = circuitoTelefonia.Id,
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

        private CircuitoTelefoniaCadastroModel GetViewObj(CircuitoTelefonia circuitoTelefonia, ISession session)
        {
            using (var repoCircuitoDid = new Conexao<CircuitoTelefonia>(session))
            using (var repoDid = new Conexao<Did>(session))
            using (var repoCliPP = new Conexao<Cliente_PlanoProduto>(session))
            {
                var dids = repoDid.select(p => p.Circuito == circuitoTelefonia).Result.ToList();
                return new CircuitoTelefoniaCadastroModel
                {
                    Id = circuitoTelefonia.Id,
                    Numero_Sip = circuitoTelefonia.Numero_Sip,
                    IdCliente = circuitoTelefonia.Clientes_PlanosProdutos?.Cliente?.Id,
                    Cliente = circuitoTelefonia.Clientes_PlanosProdutos?.Cliente?.NomeFantasia,
                    IdPlanoProduto = circuitoTelefonia.Clientes_PlanosProdutos?.Id,
                    PlanoProduto = circuitoTelefonia.Clientes_PlanosProdutos?.Plano.Descricao,

                    //IdCliente = circuitoTelefonia.Cliente_PlanoProduto?.Cliente?.Id,
                    //IdPlanoProduto = circuitoTelefonia.Cliente_PlanoProduto?.Id,
                    //Cliente = circuitoTelefonia.Cliente_PlanoProduto?.Cliente?.NomeFantasia,
                    //PlanoProduto = circuitoTelefonia.Cliente_PlanoProduto?.Plano?.Descricao,
                    IdRota = circuitoTelefonia.RotaTelefonia?.Id,
                    QtdLigacaoesSimultaneas = Convert.ToString(circuitoTelefonia.QuantidadeLigacoesSimultaneas),
                    TipoDiscagem = circuitoTelefonia.TipoDiscagem,
                    Senha = circuitoTelefonia.Senha,
                    Status = circuitoTelefonia.Status == Status_Default.Ativo,
                    Status_Circuito = circuitoTelefonia.Status_Circuito,
                    BloqueioChamadaCobrar = circuitoTelefonia.BloqueioChamadaCobrar,
                    HabilitarBina = circuitoTelefonia.HabilitarBina,
                    HabilitarConsumoMinimo = circuitoTelefonia.HabilitarConsumoMinimo,
                    HabilitarLc = circuitoTelefonia.HabilitarLc,
                    HabilitarLdn = circuitoTelefonia.HabilitarLdn,
                    HabilitarVc1 = circuitoTelefonia.HabilitarVc1,
                    HabilitarVc2 = circuitoTelefonia.HabilitarVc2,
                    HabilitarVc3 = circuitoTelefonia.HabilitarVc3,
                    DidVinculados = dids.Select(p => new DidVinculado
                    {
                        Did = p.Numero,
                        ChamadaCobrar = p.ChamadaCobrar ? RsBase.SIM : RsBase.NAO,
                        BlackList = !p.ChamadaCobrar ? RsBase.SIM : RsBase.NAO,
                        SigaMe = !p.ChamadaCobrar ? RsBase.SIM : RsBase.NAO,
                        Mensalidade = Convert.ToDouble(p.Valor).ToString("N2"),

                    }).ToList()
                };
            }
        }

        //Para buscar os Dids da Venda, eu preciso buscar pelo Circuito o Id da venda correspondente e verificar se o mesmo tem Dids Vinculados para levar a view
        [HttpGet()]
        [Route("api/CircuitoTelefoniaApi/GetDidsVenda")]
        public async Task<GetApiRetorno<CircuitoTelefoniaDetalhesModel>> GetDidsVenda(long? idVenda)
        {
            var retorno = new GetApiRetorno<CircuitoTelefoniaDetalhesModel>
            {
                Objeto = new CircuitoTelefoniaDetalhesModel
                {
                    IdVenda = idVenda.GetValueOrDefault(0),
                    DidsVinculados = new List<DidsVinculados>()
                }
            };

            try
            {
                using (ISession session = HibernateUtil.GetSession())

                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    // TODO: Pesquisar uma maneira melhor de fazer essa consulta para trazer os circuitos associados a venda.
                    // Quando tenta fazer a consulta direto na tabela do Circuito gera alguns problemas
                    // com circuitos antigos que não tem o ID do ClientePlanoProduto preenchido.

                    Venda venda = await repoVenda.selectFirst(p => p.Id == idVenda);
                    var cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente);
                    var planoTelefonia = cliente.PlanosProdutos.Where(p => p.Id_Venda == venda.Id
                                                                            && (p.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Telefonia_Fixo ||
                                                                                p.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Telefonia_Movel)
                                                                               ).ToList();

                    if (planoTelefonia.Count == 0)
                    {
                        throw new Exception(RsCadastros.SEM_PLANOS_TELEFONIA_CADASTRADOS);
                    }

                    await GetDids_VendaDidOption(retorno.Objeto, session);

                    await GetDids_Venda_DdrOption(retorno.Objeto, session);

                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.Format("Erro Encontrado: {0}", ex.Message);

                if (ex.InnerException != null)
                    retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task GetDids_VendaDidOption(CircuitoTelefoniaDetalhesModel Objeto, ISession session)
        {
            using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
            using (var repoDid = new Conexao<Did>(session))
            {
                var circuitosVenda = await repoCircuito.select(p => p.Id_Venda == Objeto.IdVenda);
                foreach (CircuitoTelefonia circuito in circuitosVenda)
                {

                    List<Did> dids = (await repoDid.select(prop => prop.Circuito == circuito && prop.Reservado_Ddr == false)).ToList();
                    foreach (Did did in dids)
                    {
                        DidsVinculados didVinculado = new DidsVinculados();

                        if (circuito.Clientes_PlanosProdutos.Plano != null)
                        {
                            didVinculado.BlackList = circuito.Clientes_PlanosProdutos.Plano.ValorBlacklist != null ? String.Format("{0:N2}", circuito.Clientes_PlanosProdutos.Plano.ValorBlacklist.Value) : "";
                            didVinculado.Sigame = circuito.Clientes_PlanosProdutos.Plano.ValorSigame != null ? String.Format("{0:N2}", circuito.Clientes_PlanosProdutos.Plano.ValorSigame) : "";
                            didVinculado.PlanoVinculado = circuito.Clientes_PlanosProdutos.Plano.Descricao;
                        }

                        didVinculado.IdCircuito = circuito.Id;
                        didVinculado.IdDid = did.Id;
                        didVinculado.Did = did.Numero;
                        didVinculado.Ddr = did.Reservado_Ddr ? did.Numero : "-";
                        didVinculado.IdPlanoVinculado = circuito.Clientes_PlanosProdutos.Id;
                        didVinculado.Chamada_Cobrar = did.ChamadaCobrar ? RsBase.SIM : RsBase.NAO;
                        //didVinculado.BlackList = "-";
                        //didVinculado.Sigame = "-";
                        didVinculado.Mensalidade = String.Format("{0:N2}", did.Valor.GetValueOrDefault(0));




                        Objeto.DidsVinculados.Add(didVinculado);
                    }
                }
            }

        }

        public async Task GetDids_Venda_DdrOption(CircuitoTelefoniaDetalhesModel Objeto, ISession session)
        {
            using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
            using (var repoDid = new Conexao<Did>(session))
            {
                var circuitosVenda = await repoCircuito.select(p => p.Id_Venda == Objeto.IdVenda);
                foreach (CircuitoTelefonia circuito in circuitosVenda)
                {

                    List<Did> dids = (await repoDid.select(prop => prop.Circuito == circuito && prop.Reservado_Ddr == true)).ToList();
                    foreach (Did did in dids)
                    {
                        string planoVinculado = "";

                        if (circuito.Clientes_PlanosProdutos.Plano != null)
                            planoVinculado = circuito.Clientes_PlanosProdutos.Plano.Descricao;
                        else if (circuito.Clientes_PlanosProdutos.Produto != null)
                        {
                            planoVinculado = circuito.Clientes_PlanosProdutos.Produto.Descricao;
                        }
                        else
                        {
                            planoVinculado = circuito.Clientes_PlanosProdutos.ClienteContrato.Descricao;
                        }


                        Objeto.DidsVinculados.Add(new DidsVinculados
                        {
                            IdCircuito = circuito.Id,

                            IdDid = did.Id,
                            Did = did.Numero,
                            Ddr = "-",
                            IdPlanoVinculado = circuito.Clientes_PlanosProdutos.Id,
                            PlanoVinculado = planoVinculado,
                            BlackList = "-",
                            Chamada_Cobrar = "-",
                            Sigame = "-",
                            Mensalidade = String.Format("{0:N2}", did.Valor.GetValueOrDefault(0))
                        });
                    }
                }
            }
        }
        public async Task<PostApiRetorno<long>> GravarDidManual(CircuitoTelefoniaDetalhesModel model)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repoDid = new Conexao<Did>(session))
                {
                    CircuitoTelefonia objeto = await repoCircuito.selectFirst(p => p.Id_Venda == model.IdVenda);
                    if (objeto == null)
                    {
                        throw new Exception("Circuito não existe");
                    }
                    foreach (var item in model.DidsVinculados)
                    {
                        Did did = await repoDid.selectFirst(p => p.Numero == item.Did);
                        if (did == null)
                        {
                        }
                        else
                        {
                            throw new Exception("Número de Did passado não tem registro correspondente no Banco de Dados");
                            //did = new Did();
                        }
                        objeto.Did.Add(did);
                    };
                    await repoCircuito.SaveOrUpdate(objeto);

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

        private CircuitoTelefonia BindViewObj(CircuitoTelefoniaCadastroModel viewModel, CircuitoTelefonia model, ISession session)
        {
            using (var repositoryRota = new Conexao<RotaTelefonia>(session))
            using (var repositoryClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                model.Senha = viewModel.Senha;
                //model.Cliente_PlanoProduto = repositoryClientePlanoProduto.selectFirst(p => p.Id == viewModel.IdPlanoProduto).Result;
                model.RotaTelefonia = repositoryRota.selectFirst(p => p.Id == viewModel.IdRota).Result;
                model.TipoDiscagem = viewModel.TipoDiscagem;
                model.QuantidadeLigacoesSimultaneas = Convert.ToInt32(viewModel.QtdLigacaoesSimultaneas);
                model.HabilitarLc = viewModel.HabilitarLc;
                model.HabilitarVc1 = viewModel.HabilitarVc1;
                model.HabilitarVc2 = viewModel.HabilitarVc2;
                model.HabilitarVc3 = viewModel.HabilitarVc3;
                model.HabilitarLdn = viewModel.HabilitarLdn;
                model.HabilitarLc = viewModel.HabilitarLc;
                model.HabilitarLdn = viewModel.HabilitarLdn;
                model.HabilitarConsumoMinimo = viewModel.HabilitarConsumoMinimo;
                model.HabilitarBina = viewModel.HabilitarBina;
                model.BloqueioChamadaCobrar = viewModel.BloqueioChamadaCobrar;
                model.Status_Circuito = Status_PlanoTelefonia.Aguardando_Criacao_OS;
            }

            return model;

        }

        [HttpPost()]
        [Route("api/CircuitoTelefoniaApi/AdicionarDid")]
        public async Task<PostApiRetorno<Int64>> AdicionarDid(AdicionarDidRequestModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repositoryCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repositoryDid = new Conexao<Did>(session))
                using (var repositoryClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var circuito = await repositoryCircuito.selectFirst(p => p.Id_Venda == value.IdVenda);
                    var did = await repositoryDid.selectFirst(p => p.Id == value.IdDid);
                    var plano = await repositoryClientePlanoProduto.selectFirst(p => p.Id == value.IdPlano);

                    if (did != null)
                    {
                        did.Circuito = circuito;
                        did.Valor = Convert.ToDouble(value.Mensalidade);
                        did.StatusDid = StatusDid.Reservado;

                        if (circuito.Clientes_PlanosProdutos == null)
                        {
                            circuito.Clientes_PlanosProdutos = plano;
                        }

                        await repositoryDid.Update(did);
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
        [Route("api/CircuitoTelefoniaApi/ExcluirDid")]
        public async Task<PostApiRetorno<Int64>> ExcluirDid(long value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repositoryCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repositoryDid = new Conexao<Did>(session))
                {
                    var did = await repositoryDid.selectFirst(p => p.Id == value);
                    if (did != null)
                    {
                        did.Circuito = null;
                        did.StatusDid = StatusDid.Disponivel;
                        await repositoryDid.Update(did);
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

        [HttpPost()]
        [Route("api/CircuitoTelefoniaApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CIRCUITO_TELEFONIA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/CircuitoTelefoniaApi/Generate_PasswordsCircuito")]
        public GetApiRetorno<String> Generate_PasswordsCircuito()
        {
            var retorno = new GetApiRetorno<String>();
            try
            {
                List<Char> password_characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUVWXYZ!@#$_".ToCharArray().ToList(),
                           generating = new List<Char>();

                Random random = new Random();
                int length_Password = 20;
                String password = "";

                for (int iterator = 1; iterator < length_Password; iterator++)
                {
                    Int32 R_index = random.Next(password_characters.Count());
                    generating.Add(password_characters[R_index]);
                }
                generating.ForEach(p => password += p);
                retorno.Objeto = password;
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



        public async Task<CircuitoIntegracaoResponse> Integracao_Cadastro_Circuito(CircuitoTelefonia obj)
        {
            CircuitoIntegracaoSaperxService integracao = new CircuitoIntegracaoSaperxService();
            return integracao.Cadastrar(obj);

        }

        public async Task<CircuitoIntegracaoResponse> Integracao_Editar_Circuito(CircuitoTelefonia obj)
        {
            CircuitoIntegracaoSaperxService integracao = new CircuitoIntegracaoSaperxService();
            return integracao.Atualizar(obj);

        }

        [HttpPost()]
        [Route("api/CircuitoTelefoniaApi/ConsultaDidCircuitoPlanoCliente")]
        public async Task<DataTableResponse<CircuitoDidPlanoListaModel>> ConsultaDidCircuitoPlanoCliente(CircuitoDidPlano request)
        {
            var retorno = new List<CircuitoDidPlanoListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_DID A");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.NUMERO, A.VALOR, B.HABILITAR_VC1, B.HABILITAR_VC2, B.HABILITAR_VC3,
                                                            B.HABILITAR_BINA, B.HABILITAR_LC, B.ID AS NUMERO_CIRCUITO, D.COBRAR_BLACKLIST, D.COBRAR_SIGAME
                                                            FROM TB_DID A 
                                                            LEFT JOIN TB_CIRCUITO_TELEFONIA B
                                                            ON(A.ID_CIRCUITO_TELEFONIA = B.ID)
                                                            LEFT JOIN TB_CLIENTE_PLANO_PRODUTO C 
                                                            ON(C.ID = B.ID_CLIENTE_PLANO_PRODUTO)
                                                            LEFT JOIN  TB_PLANO D
                                                            ON(D.ID = C.ID_PLANO)");

                    sql.AppendLine(" WHERE D.ID = @ID ");
                    parametros.Add("@ID", request.IdPlano);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CircuitoDidPlanoListaModel
                            {
                                Id_did = Convert.ToInt32(reader["ID"]),
                                NumeroDid = Convert.ToInt32("NUMERO"),
                                Mensalidade = Convert.ToString(reader["VALOR"]),
                                LC = Convert.ToBoolean(reader["HABILITAR_LC"]),
                                LDN = Convert.ToBoolean(reader["HABILITAR_LDN"]),
                                VC1 = Convert.ToBoolean(reader["HABILITAR_VC1"]),
                                VC2 = Convert.ToBoolean(reader["HABILITAR_VC2"]),
                                VC3 = Convert.ToBoolean(reader["HABILITAR_VC3"]),
                                Bina = Convert.ToBoolean(reader["BINA"]),
                                Blacklist = Convert.ToBoolean(reader["BLACKLIST"]),
                                SigaMe = Convert.ToBoolean(reader["SIGAME"])
                            });
                        }
                    }
                }
                return new DataTableResponse<CircuitoDidPlanoListaModel>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno
                };
            }

        }


    }
}
