using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Ponto_VendaApiController : BaseApiController
    {
        
        [HttpPost()]
        [Route("api/ponto_vendaapi/GetAll")]
        public async Task<DataTableResponse<Ponto_VendaListaModel>> GetAll(Ponto_Venda_DataTableRequest request)
        {
            List<Ponto_VendaListaModel> retorno = new List<Ponto_VendaListaModel>();
            IOrderedEnumerable<Ponto_VendaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PONTO_VENDA ");

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

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.STATUS, A.NOME,A.ID, A.CEP, A.LOGRADOURO, A.NUMERO, A.BAIRRO, A.CIDADE, A.COMPLEMENTO, A.UF, ");
                    sql.AppendLine(" A.DATAHORACRIACAO FROM TB_PONTO_VENDA A ");

                    sql.AppendLine(" WHERE A.ID > 0 ");

                    //Filtrando

                    if (request.Status_Filtro.HasValue)
                    {
                        sql.AppendLine(" AND A.STATUS IN(@STATUS)");
                        parametros.Add("@STATUS", request.Status_Filtro.Value);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((A.CEP LIKE @SEARCH) OR (UPPER(A.NOME) LIKE @SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (!String.IsNullOrEmpty(request.Estado))
                    {

                        sql.AppendLine(" AND (UPPER(A.UF) = @ESTADO)");
                        parametros.Add("@ESTADO", request.Estado);
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {

                        sql.AppendLine(" AND (UPPER(A.CIDADE) = @CIDADE)");
                        parametros.Add("@CIDADE", request.Cidade);
                    }

                    if (!String.IsNullOrEmpty(request.Bairro) && request.Bairro != "Todos")
                    {

                        sql.AppendLine(" AND (UPPER(A.BAIRRO) = @BAIRRO)");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }


                    if (request.Id_Vendedor != 0)
                    {
                        sql.AppendLine(" AND A.ID IN ( SELECT DISTINCT ID_PONTO_VENDA FROM TB_PONTO_VENDA_VENDEDOR B ")
                                          .AppendLine(" WHERE B.ID_VENDEDOR = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }

                    if (request.Id_Supervisor != 0)
                    {
                        sql.AppendLine(" AND A.ID IN ( SELECT DISTINCT ID_PONTO_VENDA FROM TB_PONTO_VENDA_VENDEDOR B ")
                                          .AppendLine(" INNER JOIN TB_FUNCIONARIO C ")
                                             .AppendLine(" ON (B.ID_VENDEDOR = C.ID) ")
                                          .AppendLine(" WHERE C.ID_SUPERVISOR = @ID_VENDEDOR ) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Supervisor);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Ponto_VendaListaModel
                            {
                                Nome = Convert.ToString(reader["NOME"]),
                                Id = Convert.ToInt64(reader["ID"]),
                                Cep = Convert.ToString(reader["CEP"]),
                                Logradouro = Convert.ToString(reader["LOGRADOURO"]),
                                Numero = Convert.ToString(reader["NUMERO"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Complemento = Convert.ToString(reader["COMPLEMENTO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
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
                    case 1:     // VENDEDOR
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case 2:     // STATUS
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Ponto_VendaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Ponto_VendaCadastroModel GetViewObj(Ponto_Venda obj, ISession session)
        {
            var retorno = new Ponto_VendaCadastroModel
            {
                Id = obj.Id,
                Nome = obj.Nome,
                Cep = obj.Cep,
                Logradouro = obj.Logradouro,
                Numero = obj.Numero,
                Id_Bairro = obj.Id_Bairro,
                Bairro = obj.Bairro,
                Id_Cidade = obj.Id_Cidade,
                Cidade = obj.Cidade,
                Complemento = obj.Complemento,
                Latitude = Convert.ToString(obj?.Latitude),
                Longitude = Convert.ToString(obj?.Longitude),
                Status = obj.Status == Status_Default.Ativo,
                DataInicial = DateTime.Now.ToString("dd/MM/yyyy"),
                DataFinal = DateTime.Now.ToString("dd/MM/yyyy"),
                Vendedores = new Conexao<Ponto_Venda_Vendedor>(session).select(p => p.Ponto_Venda.Id == obj.Id).Result.Select(p => new Ponto_Venda_VendedorCadastroModel
                {
                    Id = p.Id,
                    Nome = p.Vendedor.Nome,
                    Id_Vendedor = p.Vendedor.Id,
                    DataInicial = p.DataInicial?.ToString("dd/MM/yyyy"),
                    DataFinal = p.DataFinal?.ToString("dd/MM/yyyy")
                }).ToList(),
                UF = String.IsNullOrEmpty(obj.Uf) ? null : (UF?)Enum.Parse(typeof(UF), obj.Uf),
            };

            return retorno;
        }

        private void BindViewToObj(ISession session, ref Ponto_Venda objBanco, Ponto_VendaCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Cep = view.Cep;
            objBanco.Nome = view.Nome;
            objBanco.Logradouro = view.Logradouro;
            objBanco.Numero = view.Numero;
            objBanco.Id_Bairro = view.Id_Bairro;
            objBanco.Bairro = view.Bairro;
            objBanco.Id_Cidade = view.Id_Cidade;
            objBanco.Cidade = view.Cidade;
            objBanco.Complemento = view.Complemento;
            objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            objBanco.Uf = (view.UF.HasValue ? HtmlEnumExtensions.GetEnumDescription<UF>(view.UF.Value) : null);
            objBanco.Latitude = view?.Latitude != null ? Convert.ToDouble(view?.Latitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Longitude = view?.Longitude != null ? Convert.ToDouble(view?.Longitude?.Replace('.', ',')) : (Double?)null;
        }


        [HttpPost()]
        [Route("api/ponto_vendaapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PONTO_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/ponto_vendaapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_PONTO_VENDA WHERE ID IN {_ids}");

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

        [Route("api/ponto_vendaapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Venda_Vendedor>(session))
                    {

                        Ponto_Venda_Vendedor modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        
        [HttpGet()]
        [Route("api/ponto_vendaApi/FindById")]
        public async Task<GetApiRetorno<Ponto_VendaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Ponto_VendaCadastroModel> retorno = new GetApiRetorno<Ponto_VendaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    retorno.Objeto = GetViewObj(await new Conexao<Ponto_Venda>(session).selectFirst(p => p.Id == Id), session);
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

        #region mobile

        [HttpGet()]
        [Route("api/ponto_vendaApi/getByLogradouro")]
        public async Task<GetApiRetorno<List<Ponto_Venda>>> getByLogradouro(int pagina, String likeLogradouro)
        {
            GetApiRetorno<List<Ponto_Venda>> retorno = new GetApiRetorno<List<Ponto_Venda>>();
            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder consulta = new StringBuilder();
                consulta.AppendLine("SELECT FIRST 10 SKIP(:pPAGINA * 10)");
                consulta.AppendLine("       A.ID, A.CEP, A.LOGRADOURO, A.NUMERO,");
                consulta.AppendLine("       A.ID_BAIRRO, A.BAIRRO, A.ID_CIDADE, A.CIDADE,");
                consulta.AppendLine("       A.COMPLEMENTO, A.LATITUDE, A.LONGITUDE, A.UF,");
                consulta.AppendLine("       A.NOME");
                consulta.AppendLine("FROM TB_PONTO_VENDA A");
                consulta.AppendLine("WHERE UPPER(A.LOGRADOURO || A.NOME) LIKE :pLIKE");

                using (Query query = new Query(session))
                {
                    try
                    {
                        query.setParametro("pPAGINA", pagina);
                        query.setParametro("pLIKE", string.Format("%{0}%", likeLogradouro?.ToUpper() ?? String.Empty));

                        retorno.Objeto = await query.Values<Ponto_Venda>(consulta.ToString());
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

        
        [HttpGet()]
        [Route("api/ponto_vendaApi/FindByVendedorMobile")]
        public async Task<GetApiRetorno<List<Checkins_Vendedor_Pdv>>> FindByVendedorMobile(Int64 Id_Vendedor, String data)
        {
            GetApiRetorno<List<Checkins_Vendedor_Pdv>> retorno = new GetApiRetorno<List<Checkins_Vendedor_Pdv>>()
            {
                Objeto = new List<Checkins_Vendedor_Pdv>()
            };
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT A.ID,");
                sql.AppendLine("       A.NOME,");
                sql.AppendLine("       A.CEP,");
                sql.AppendLine("       A.LOGRADOURO,");
                sql.AppendLine("       A.NUMERO,");
                sql.AppendLine("       A.ID_BAIRRO,");
                sql.AppendLine("       A.BAIRRO,");
                sql.AppendLine("       A.ID_CIDADE,");
                sql.AppendLine("       A.CIDADE,");
                sql.AppendLine("       A.COMPLEMENTO,");
                sql.AppendLine("       A.UF,");
                sql.AppendLine("       A.LATITUDE,");
                sql.AppendLine("       A.LONGITUDE,");

                sql.AppendLine("       B.ID AS ID_CHECK,");
                sql.AppendLine("       B.DATA,");
                sql.AppendLine("       B.ENDERECO_CHECKIN,");
                sql.AppendLine("       B.LATITUDE_CHECKIN,");
                sql.AppendLine("       B.LONGITUDE_CHECKIN,");
                sql.AppendLine("       B.DATA_INICIAL,");
                sql.AppendLine("       B.ENDERECO_CHECKOUT,");
                sql.AppendLine("       B.LATITUDE_CHECKOUT,");
                sql.AppendLine("       B.LONGITUDE_CHECKOUT,");
                sql.AppendLine("       B.DATA_FINAL,");
                sql.AppendLine("       B.PRECISAO,");
                sql.AppendLine("       B.BATERIA,");
                sql.AppendLine("       B.STATUS_GPS,");
                sql.AppendLine("       B.DISPOSITIVO,");
                sql.AppendLine("       B.VERSAO_ANDROID,");
                sql.AppendLine("       B.VERSAO_APP,");
                sql.AppendLine("       B.ATUALIZADO_EM");
                sql.AppendLine("FROM TB_PONTO_VENDA A");
                sql.AppendLine("LEFT OUTER JOIN TB_PONTO_VENDA_VENDEDOR C ON(A.ID = C.ID_PONTO_VENDA)");
                sql.AppendLine("LEFT JOIN TB_CHECKIN_VENDEDOR B ON (A.ID = B.ID_PONTO_VENDA AND C.ID_VENDEDOR = B.ID_VENDEDOR AND CAST(B.DATA AS DATE) = @pDATA)");
                sql.AppendLine("WHERE C.ID_VENDEDOR = @pID_VENDEDOR AND @pDATA BETWEEN CAST(C.DATA_INICIAL AS DATE) AND CAST(C.DATA_FINAL AS DATE)");

                Dictionary<String, object> paramns = new Dictionary<string, object>();
                paramns.Add("@pID_VENDEDOR", Id_Vendedor);
                paramns.Add("@pDATA", data);

                List<Checkins_Vendedor_Pdv> valores = new List<Checkins_Vendedor_Pdv>();
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var command = session.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(command, sql, paramns, session.Connection, null))
                        {
                            try
                            {
                                while (reader.Read())
                                {
                                    Int64? idCheck = CommandHelper.convertReturnValue<Int64?>(reader["ID_CHECK"], null);
                                    Checkin_Vendedor check = idCheck == null ? null : new Checkin_Vendedor()
                                    {
                                        Id = idCheck.Value,
                                        Data = CommandHelper.convertReturnValue<DateTime?>(reader["DATA"], null),
                                        Endereco_Checkin = reader["ENDERECO_CHECKIN"]?.ToString(),
                                        Latitude_Checkin = CommandHelper.convertReturnValue<Double?>(reader["LATITUDE_CHECKIN"], null),
                                        Longitude_Checkin = CommandHelper.convertReturnValue<Double?>(reader["LONGITUDE_CHECKIN"], null),
                                        DataInicial = CommandHelper.convertReturnValue<DateTime?>(reader["DATA_INICIAL"], null),

                                        Endereco_Checkout = CommandHelper.convertReturnValue<String>(reader["ENDERECO_CHECKOUT"], null),
                                        Latitude_Checkout = CommandHelper.convertReturnValue<Double?>(reader["LATITUDE_CHECKOUT"], null),
                                        Longitude_Checkout = CommandHelper.convertReturnValue<Double?>(reader["LONGITUDE_CHECKOUT"], null),
                                        DataFinal = CommandHelper.convertReturnValue<DateTime?>(reader["DATA_FINAL"], null),

                                        Precisao = CommandHelper.convertReturnValue<Double?>(reader["PRECISAO"], null),
                                        Bateria = CommandHelper.convertReturnValue<Double?>(reader["BATERIA"], null),
                                        Status_Gps = reader["STATUS_GPS"]?.ToString(),
                                        Dispositivo = reader["DISPOSITIVO"]?.ToString(),
                                        Versao_Android = reader["VERSAO_ANDROID"]?.ToString(),
                                        Versao_App = reader["VERSAO_APP"]?.ToString(),
                                        Atualizado_Em = CommandHelper.convertReturnValue<DateTime?>(reader["ATUALIZADO_EM"], null)
                                    };

                                    Checkins_Vendedor_Pdv ponto = new Checkins_Vendedor_Pdv()
                                    {
                                        Id = CommandHelper.convertReturnValue<Int64>(reader["ID"], 0),
                                        Nome = reader["NOME"]?.ToString(),
                                        Cep = reader["CEP"]?.ToString(),
                                        Logradouro = reader["LOGRADOURO"]?.ToString(),
                                        Numero = reader["NUMERO"]?.ToString(),
                                        Id_Bairro = CommandHelper.convertReturnValue<Int64?>(reader["ID_BAIRRO"], null),
                                        Bairro = reader["BAIRRO"]?.ToString(),
                                        Id_Cidade = CommandHelper.convertReturnValue<Int64?>(reader["ID_CIDADE"], null),
                                        Cidade = reader["CIDADE"]?.ToString(),
                                        Complemento = reader["COMPLEMENTO"]?.ToString(),
                                        Latitude = CommandHelper.convertReturnValue<Double?>(reader["LATITUDE"], null),
                                        Longitude = CommandHelper.convertReturnValue<Double?>(reader["LONGITUDE"], null),
                                        DataInicial = CommandHelper.convertReturnValue<DateTime?>(reader["DATA_INICIAL"], null),
                                        DataFinal = CommandHelper.convertReturnValue<DateTime?>(reader["DATA_FINAL"], null),
                                        Uf = reader["UF"]?.ToString(),
                                        Checkin_Vendedor = check
                                    };
                                    valores.Add(ponto);
                                }
                            }
                            catch (Exception ex)
                            {
                                retorno.Mensagem = ex.Message;
                                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                            }
                        }
                    }

                    retorno.Objeto = valores.GroupBy(g => g.Id, (key, g) => g.OrderByDescending(e => e.Checkin_Vendedor?.Id).First()).ToList();
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        
        [HttpPost()]
        [Route("api/Ponto_VendaApi/GravarMobile")]
        public async Task<GetApiRetorno<object>> GravarMobile(ParamWebApi<PontoVenda_Mobile> value)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();

            try
            {
                Ponto_Venda ponto = value.Objeto.pontoVenda;
                if (ponto.Id == 0)
                {
                    object latLong = new EnderecoApiController().GetLatLong(ponto.Logradouro, ponto.Numero, "Brasil", ponto.Uf, ponto.Cidade, ponto.Bairro);
                    if (latLong.GetType() != typeof(String))
                    {
                        ponto.Latitude = ((dynamic)latLong).lat;
                        ponto.Longitude = ((dynamic)latLong).lng;
                    }
                }
                else
                {
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        Ponto_Venda_Vendedor pontoVendedor = await new Conexao<Ponto_Venda_Vendedor>(session).selectFirst(s => s.Vendedor.Id == value.Objeto.idVendedor &&
                                                                                                                               s.Ponto_Venda.Id == ponto.Id &&
                                                                                                                               (!(s.DataFinal.Value.Date < Convert.ToDateTime(value.Objeto.dataInicial).Date ||
                                                                                                                                  s.DataInicial.Value.Date > Convert.ToDateTime(value.Objeto.dataFinal).Date)));
                        if (pontoVendedor != null)
                        {
                            throw new Exception(Resources.RsBase.JA_VINCULADO_AO_PONTOVENDA);
                        }
                    }
                }

                PostApiRetorno<Int64> retGravar = await this.Gravar(new ParamWebApi<Ponto_VendaCadastroModel>()
                {
                    Token = value.Token,
                    Usuario = value.Token,
                    Objeto = new Ponto_VendaCadastroModel()
                    {
                        Id = ponto.Id,
                        Nome = ponto.Nome,
                        Cep = ponto.Cep,
                        Logradouro = ponto.Logradouro,
                        Numero = ponto.Numero,
                        Id_Bairro = ponto.Id_Bairro,
                        Bairro = ponto.Bairro,
                        Id_Cidade = ponto.Id_Cidade,
                        Cidade = ponto.Cidade,
                        UF = (UF)Enum.Parse(typeof(UF), ponto.Uf),
                        Complemento = ponto.Complemento,
                        Latitude = ponto.Latitude?.ToString(),
                        Longitude = ponto.Longitude?.ToString(),
                        DataInicial = value.Objeto.dataInicial,
                        DataFinal = value.Objeto.dataFinal,
                        Vendedores = new List<Ponto_Venda_VendedorCadastroModel>()
                        {
                            new Ponto_Venda_VendedorCadastroModel()
                            {
                                Id = 0,
                                Id_Vendedor = value.Objeto.idVendedor,
                                DataInicial = value.Objeto.dataInicial,
                                DataFinal = value.Objeto.dataFinal
                            }
                        }
                    }
                });

                if (retGravar.Ok)
                {
                    retorno.Ok = true;
                    retorno.Objeto = new Checkins_Vendedor_Pdv()
                    {
                        Id = retGravar.IdObjeto,
                        Nome = ponto.Nome,
                        Cep = ponto.Cep,
                        Logradouro = ponto.Logradouro,
                        Numero = ponto.Numero,
                        Id_Bairro = ponto.Id_Bairro,
                        Bairro = ponto.Bairro,
                        Id_Cidade = ponto.Id_Cidade,
                        Cidade = ponto.Cidade,
                        Uf = ponto.Uf,
                        Complemento = ponto.Complemento,
                        Latitude = ponto.Latitude,
                        Longitude = ponto.Longitude,
                        Checkin_Vendedor = null
                    };
                }
                else
                {
                    retorno.Ok = false;
                    retorno.Mensagem = retGravar.Mensagem;
                    retorno.InnerException = retGravar.InnerException;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/ponto_vendaApi/FindByVendedor")]
        public async Task<GetApiRetorno<List<Checkins_Vendedor_Pdv>>> FindByVendedor(Int64 Id_Vendedor, String datainicial, String datafinal)
        {
            GetApiRetorno<List<Checkins_Vendedor_Pdv>> retorno = new GetApiRetorno<List<Checkins_Vendedor_Pdv>>();
            List<Checkins_Vendedor_Pdv> pontos = new List<Checkins_Vendedor_Pdv>();
            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.NOME, A.cep, A.logradouro, A.numero, A.id_bairro, A.bairro, A.id_cidade, A.cidade, A.complemento, A.uf, A.latitude, 
                        A.LONGITUDE, B.DATA_INICIAL, B.DATA_FINAL, B.DATA, B.ENDERECO_CHECKIN, B.ENDERECO_CHECKOUT, B.LATITUDE_CHECKIN,
                        B.LONGITUDE_CHECKIN, B.LATITUDE_CHECKOUT, B.LONGITUDE_CHECKOUT, B.DATAHORAALTERACAO,
                        B.PRECISAO, B.BATERIA, B.STATUS_GPS, B.DISPOSITIVO, B.VERSAO_ANDROID, B.VERSAO_APP, B.ATUALIZADO_EM
                          FROM TB_ponto_venda A
                         INNER JOIN TB_PONTO_VENDA_VENDEDOR C
                         ON(A.ID = C.ID_PONTO_VENDA)
                         LEFT JOIN TB_CHECKIN_VENDEDOR B
                         ON(A.ID = B.ID_PONTO_VENDA)
                         WHERE C.ID_VENDEDOR = @ID_VENDEDOR");
                    parametros.Add("@ID_VENDEDOR", Id_Vendedor);
                    if (datainicial != null)
                    {
                        sql.AppendLine(" AND CAST(A.DATA_INICIAL AS DATE) = @DATAINICIAL ");
                        parametros.Add("@DATAINICIAL", datainicial);
                    }
                    if (datafinal != null)
                    {
                        sql.AppendLine(" AND CAST(A.DATA_FINAL AS DATE) = @DATAFINAL ");
                        parametros.Add("@DATAFINAL", datafinal);
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                Checkins_Vendedor_Pdv check = new Checkins_Vendedor_Pdv();

                                check.Id = Convert.ToInt64(reader["ID"]);
                                check.Nome = Convert.ToString(reader["NOME"]);
                                check.Cep = Convert.ToString(reader["CEP"]);
                                check.Logradouro = Convert.ToString(reader["LOGRADOURO"]);
                                check.Numero = Convert.ToString(reader["NUMERO"]);
                                check.Bairro = Convert.ToString(reader["BAIRRO"]);
                                check.Id_Bairro = Convert.ToInt64(reader["ID_BAIRRO"]);
                                check.Cidade = Convert.ToString(reader["CIDADE"]);
                                check.Id_Cidade = Convert.ToInt64(reader["ID_CIDADE"]);
                                check.Complemento = Convert.ToString(reader["COMPLEMENTO"]);
                                check.Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null;
                                check.Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : (Double?)null;
                                check.DataInicial = reader["DATA_INICIAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIAL"]) : (DateTime?)null;
                                check.DataFinal = reader["DATA_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FINAL"]) : (DateTime?)null;
                                check.Uf = Convert.ToString(reader["UF"]);

                                check.Checkin_Vendedor = new Checkin_Vendedor();

                                check.Checkin_Vendedor.DataInicial = reader["DATA_INICIAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIAL"]) : (DateTime?)null;
                                check.Checkin_Vendedor.DataFinal = reader["DATA_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FINAL"]) : (DateTime?)null;
                                check.Checkin_Vendedor.Endereco_Checkin = Convert.ToString(reader["ENDERECO_CHECKIN"]);
                                check.Checkin_Vendedor.Endereco_Checkout = Convert.ToString(reader["ENDERECO_CHECKOUT"]);
                                check.Checkin_Vendedor.Latitude_Checkin = reader["LATITUDE_CHECKIN"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE_CHECKIN"]) : (Double?)null;
                                check.Checkin_Vendedor.Longitude_Checkin = reader["LONGITUDE_CHECKIN"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE_CHECKIN"]) : (Double?)null;
                                check.Checkin_Vendedor.Latitude_Checkout = reader["LATITUDE_CHECKOUT"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE_CHECKOUT"]) : (Double?)null;
                                check.Checkin_Vendedor.Longitude_Checkout = reader["LONGITUDE_CHECKOUT"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE_CHECKOUT"]) : (Double?)null;
                                check.Checkin_Vendedor.Precisao = reader["PRECISAO"] != DBNull.Value ? Convert.ToDouble(reader["PRECISAO"]) : (Double?)null;
                                check.Checkin_Vendedor.Bateria = reader["BATERIA"] != DBNull.Value ? Convert.ToDouble(reader["BATERIA"]) : (Double?)null;
                                check.Checkin_Vendedor.Status_Gps = Convert.ToString(reader["STATUS_GPS"]);
                                check.Checkin_Vendedor.Dispositivo = Convert.ToString(reader["DISPOSITIVO"]);
                                check.Checkin_Vendedor.Versao_Android = Convert.ToString(reader["VERSAO_ANDROID"]);
                                check.Checkin_Vendedor.Versao_App = Convert.ToString(reader["VERSAO_APP"]);
                                check.Checkin_Vendedor.Atualizado_Em = reader["ATUALIZADO_EM"] != DBNull.Value ? Convert.ToDateTime(reader["ATUALIZADO_EM"]) : (DateTime?)null;

                                pontos.Add(check);
                            }
                            retorno.Objeto = pontos;
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

        
        [HttpPost()]
        [Route("api/Ponto_VendaApi/setCheckInOutMobileAutomatico")]
        public async Task<GetApiRetorno<object>> setCheckInOutMobileAutomatico(ParamWebApi<Checkpoint_VendedorMobileModel> value)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();
            try
            {
                Checkin_Vendedor atualizar = value.Objeto.checkPoint;
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (Conexao<Checkin_Vendedor> con = new Conexao<Checkin_Vendedor>(session))
                    {
                        List<Checkin_Vendedor> checks = (await con.select(s => s.idPontoVenda == value.Objeto.idPontoVenda &&
                                                                               s.Endereco_Checkout == null &&
                                                                               s.Data.Value.Date == DateTime.Now.Date)).ToList();
                        if (checks.Count > 1)
                        {
                            throw new Exception("Existe Checkin em aberto");
                        }

                        if (atualizar.DataInicial == null)
                        {
                            Checkin_Vendedor check = checks.FirstOrDefault();
                            if (check == null)
                            {
                                throw new Exception("Checkin não marcado");
                            }

                            atualizar.Id = check.Id;
                            atualizar.DataInicial = check.DataInicial;
                            atualizar.Endereco_Checkin = check.Endereco_Checkin;
                            atualizar.Latitude_Checkin = check.Latitude_Checkin;
                            atualizar.Longitude_Checkin = check.Longitude_Checkin;
                        }
                        else
                        {
                            atualizar.Id = 0;
                        }


                    }
                }

                retorno = await this.setCheckInOutMobile(new ParamWebApi<Checkpoint_VendedorMobileModel>()
                {
                    Usuario = value.Usuario,
                    Token = value.Token,
                    Objeto = new Checkpoint_VendedorMobileModel()
                    {
                        idPontoVenda = value.Objeto.idPontoVenda,
                        idVendedor = value.Objeto.idVendedor,
                        checkPoint = atualizar
                    }
                });
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : null;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        
        [HttpPost()]
        [Route("api/Ponto_VendaApi/setCheckInOutMobile")]
        public async Task<GetApiRetorno<object>> setCheckInOutMobile(ParamWebApi<Checkpoint_VendedorMobileModel> value)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();
            try
            {
                Checkin_Vendedor modelo;
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (Conexao<Checkin_Vendedor> con = new Conexao<Checkin_Vendedor>(session))
                    {
                        Checkin_Vendedor atualizar = value.Objeto.checkPoint;
                        Checkin_Vendedor emAberto = await con.selectFirst(s => s.Vendedor.Id == value.Objeto.idVendedor &&
                                                                               s.Data.Value.Date == atualizar.Data &&
                                                                               s.Id != atualizar.Id &&
                                                                               (s.Endereco_Checkin != null && s.Endereco_Checkout == null));
                        if (emAberto != null)
                        {
                            throw new Exception(String.Format(Resources.RsBase.ENECESSARIOFINALIZARCHECKIN, emAberto.Ponto_Venda.Nome));
                        }

                        if (atualizar.Id == 0)
                        {
                            modelo = new Checkin_Vendedor()
                            {
                                Id = 0,
                                Vendedor = await new Conexao<Funcionario>(session).selectFirst(s => s.Id == value.Objeto.idVendedor),
                                Data = atualizar.Data,
                                Ponto_Venda = await new Conexao<Ponto_Venda>(session).selectFirst(s => s.Id == value.Objeto.idPontoVenda),
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo = await con.selectFirst(s => s.Id == atualizar.Id);
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        if (modelo.DataInicial == null)
                        {
                            modelo.DataInicial = DateTime.Now;
                            modelo.Endereco_Checkin = atualizar.Endereco_Checkin;
                            modelo.Latitude_Checkin = atualizar.Latitude_Checkin;
                            modelo.Longitude_Checkin = atualizar.Longitude_Checkin;
                        }
                        else
                        {
                            modelo.DataFinal = DateTime.Now;
                            modelo.Endereco_Checkout = atualizar.Endereco_Checkin;
                            modelo.Latitude_Checkout = atualizar.Latitude_Checkin;
                            modelo.Longitude_Checkout = atualizar.Longitude_Checkin;
                        }

                        modelo.Atualizado_Em = DateTime.Now;
                        modelo.Precisao = atualizar.Precisao;
                        modelo.Bateria = atualizar.Bateria;
                        modelo.Status_Gps = atualizar.Status_Gps;
                        modelo.Dispositivo = atualizar.Dispositivo;
                        modelo.Versao_Android = atualizar.Versao_Android;
                        modelo.Versao_App = atualizar.Versao_App;

                        await con.SaveOrUpdate(modelo);
                        retorno.Objeto = new
                        {
                            id = modelo.Id,
                            idPontoVenda = modelo.idPontoVenda,
                            data = modelo.Data,
                            dataInicial = modelo.DataInicial,
                            dataFinal = modelo.DataFinal,

                            endereco_Checkin = modelo.Endereco_Checkin,
                            latitude_Checkin = modelo.Latitude_Checkin,
                            longitude_Checkin = modelo.Longitude_Checkin,

                            endereco_Checkout = modelo.Endereco_Checkout,
                            latitude_Checkout = modelo.Latitude_Checkout,
                            longitude_Checkout = modelo.Longitude_Checkout,

                            precisao = modelo.Precisao,
                            bateria = modelo.Bateria,
                            status_Gps = modelo.Status_Gps,
                            dispositivo = modelo.Dispositivo,
                            versao_Android = modelo.Versao_Android,
                            versao_App = modelo.Versao_App,
                            atualizado_Em = modelo.Atualizado_Em
                        };
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : null;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Ponto_VendaApi/SetCheckInOut")]
        public async Task<PostApiRetorno<Int64>> SetCheckInOut([FromBody] ParamWebApi<Checkins_Vendedor_Pdv> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.Append("UPDATE TB_CHECKIN_VENDEDOR SET LONGITUDE_CHECKOUT = @LONGITUDE_CHECKOUT, LATITUDE_CHECKOUT = @LATITUDE_CHECKOUT, ENDERECO_CHECKOUT = @ENDERECO_CHECKOUT, ");
                        sql.Append("LATITUDE_CHECKIN = @LATITUDE_CHECKIN, LONGITUDE_CHECKIN = @LONGITUDE_CHECKIN, ENDERECO_CHECKIN = @ENDERECO_CHECKIN, ");
                        sql.Append("PRECISAO = @PRECISAO, BATERIA = @BATERIA, STATUS_GPS = @STATUS_GPS, DISPOSITIVO = @DISPOSITIVO, ");
                        sql.Append("VERSAO_ANDROID = @VERSAO_ANDROID, VERSAO_APP = @VERSAO_APP, ATUALIZADO_EM = CURRENT_TIMESTAMP ");

                        sql.AppendLine("WHERE ID_PONTO_VENDA = @ID_PONTO_VENDA AND CAST(DATA_INICIAL AS DATAINI) = @DATAINICIAL ");
                        sql.AppendLine("AND CAST(DATA_FINAL AS DATAFIN) = @DATAFINAL");

                        parametros.Add("@ID_PONTO_VENDA", value.Objeto.Id);
                        parametros.Add("@DATAINICIAL", value.Objeto.Checkin_Vendedor.DataInicial);
                        parametros.Add("@DATAFINAL", value.Objeto.Checkin_Vendedor.DataFinal);
                        parametros.Add("@LONGITUDE_CHECKOUT", value.Objeto.Checkin_Vendedor.Longitude_Checkout);
                        parametros.Add("@LATITUDE_CHECKOUT", value.Objeto.Checkin_Vendedor.Latitude_Checkout);
                        parametros.Add("@ENDERECO_CHECKOUT", value.Objeto.Checkin_Vendedor.Endereco_Checkout);
                        parametros.Add("@LATITUDE_CHECKIN", value.Objeto.Checkin_Vendedor.Latitude_Checkin);
                        parametros.Add("@LONGITUDE_CHECKIN", value.Objeto.Checkin_Vendedor.Longitude_Checkin);
                        parametros.Add("@ENDERECO_CHECKIN", value.Objeto.Checkin_Vendedor.Endereco_Checkin);
                        parametros.Add("@PRECISAO", value.Objeto.Checkin_Vendedor.Precisao);
                        parametros.Add("@BATERIA", value.Objeto.Checkin_Vendedor.Bateria);
                        parametros.Add("@STATUS_GPS", value.Objeto.Checkin_Vendedor.Status_Gps);
                        parametros.Add("@DISPOSITIVO", value.Objeto.Checkin_Vendedor.Dispositivo);
                        parametros.Add("@VERSAO_ANDROID", value.Objeto.Checkin_Vendedor.Versao_Android);
                        parametros.Add("@VERSAO_APP", value.Objeto.Checkin_Vendedor.Versao_App);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

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

        //private async void GravaLinhaCheckInOut(Ponto_Venda value)
        //{            
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            Checkin_Vendedor modelo = new Checkin_Vendedor();

        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder();

        //                sql.AppendLine("SELECT A.ID ");
        //                sql.AppendLine(" FROM TB_PONTO_VENDA A ");
        //                sql.AppendLine(" WHERE A.ID_VENDEDOR = @ID_VENDEDOR AND DATA_INICIAL = @DATA_INICIAL AND DATA_FINAL = @DATA_FINAL ");
        //                sql.AppendLine("AND CEP = @CEP AND NOME = @NOME");
        //                parametros.Add("@ID_VENDEDOR", value.Vendedor.Id);
        //                ////parametros.Add("@DATA_INICIAL", value.DataInicial);
        //                ////parametros.Add("@DATA_FINAL", value.DataFinal);
        //                parametros.Add("@CEP", value.Cep);
        //                parametros.Add("@NOME", value.Nome);

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    while (reader.Read())
        //                    {
        //                        modelo.Ponto_Venda = new Ponto_Venda();
        //                        modelo.Vendedor = new Funcionario();
        //                        modelo.Ponto_Venda.Id = Convert.ToInt64(reader["ID"]);                                
        //                        modelo.Vendedor.Id = value.Vendedor.Id;
        //                        //modelo.DataInicial = value.DataInicial.Value;
        //                        //modelo.DataFinal = value.DataFinal.Value;
        //                        modelo.UsuarioCriacao = value.Vendedor.Nome;
        //                        modelo.DataHoraCriacao = DateTime.Now;
        //                        modelo.Atualizado_Em = DateTime.Now;
        //                    }
        //                }
        //            }
        //            using (var repository = new Conexao<Checkin_Vendedor>(session))
        //            {
        //                await repository.SaveOrUpdate(modelo);
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        #endregion mobile

        //[HttpGet()]
        //[Route("api/Ponto_VendaApi/Gravar")]
        //public async Task<PostApiRetorno<Int64>> Gravar(Int64? Id)
        //{
        //    PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

        //    try
        //    {
        //        Ponto_Venda modelo = new Ponto_Venda();

        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            using (var repository = new Conexao<Ponto_Venda>(session))
        //            {

        //                //modelo.Numero = (await repository.Max(p => p.Numero)) + 1;
        //                //modelo.Status = Status_Atendimento.A_Iniciar;
        //                modelo = Id.HasValue ? new BuscaGenericaRepository<Ponto_Venda>(session, null).FindById(Id.Value).Result : null;

        //                repository.SaveOrUpdate(modelo);
        //            }

        //            retorno.Ok = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.IdObjeto = 0;
        //        retorno.Mensagem = ex.Message;
        //        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
        //    }

        //    return retorno;
        //}


        [HttpPost()]
        //[Route("api/ponto_vendaApi/Gravar")]
        [Route("api/Ponto_VendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Ponto_VendaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Ponto_Venda modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Venda>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Ponto_Venda
                            {
                                //Status = Status_Atendimento.A_Iniciar
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }


                        BindViewToObj(session, ref modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);

                        using (var repository_ponto = new Conexao<Ponto_Venda_Vendedor>(session))
                        using (var repository_func = new Conexao<Funcionario>(session))
                        {
                            foreach (var vendedor in value.Objeto.Vendedores.FindAll(p => p.Id == 0))
                            {
                                var func = repository_func.selectFirst(p => p.Id == vendedor.Id_Vendedor).Result;

                                Ponto_Venda_Vendedor ponto = new Ponto_Venda_Vendedor
                                {
                                    Vendedor = func,
                                    Ponto_Venda = modelo,
                                    UsuarioCriacao = value.Usuario,
                                    DataInicial = Convert.ToDateTime(value.Objeto.DataInicial),
                                    DataFinal = Convert.ToDateTime(value.Objeto.DataFinal)
                                };

                                await repository_ponto.SaveOrUpdate(ponto);
                            }
                        }

                        //GravaLinhaCheckInOut(modelo);
                    }

                    retorno.Ok = true;
                    retorno.IdObjeto = modelo.Id;

                }
            }
            catch (Exception ex)
            {
                //retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/ponto_vendaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Venda>(session))
                    {

                        Ponto_Venda modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/ponto_vendaApi/busca")]
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
                        if (term.Length > 4)
                            sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) AND A.E_SUPERVISOR = '0'");
                        else
                            sql.AppendLine("WHERE ((UPPER(A.NUM_FRE) LIKE @VALOR) OR (UPPER(A.NOME) LIKE @VALOR)) AND A.E_SUPERVISOR = '0' ");
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
        [Route("api/Ponto_VendaApi/ValidarVendedor")]

        public async Task<PostApiRetorno<Boolean>> ValidarVendedor(Validacao_VendedorModel validacao)
        {

            var date_ini = Convert.ToDateTime(validacao.dataini).ToString("dd/MM/yyyy");
            var date_fim = Convert.ToDateTime(validacao.datafim).ToString("dd/MM/yyyy"); ;

            var retorno = new PostApiRetorno<Boolean>();
            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    Int32 contador = 0;
                    StringBuilder sql = new StringBuilder(@"SELECT COUNT(A.ID) CO FROM TB_PONTO_VENDA_VENDEDOR A 
                        LEFT JOIN TB_PONTO_VENDA B
                        ON A.ID_PONTO_VENDA = B.ID
                        LEFT JOIN TB_FUNCIONARIO C
                        ON A.ID_VENDEDOR = C.ID
                        WHERE A.ID_VENDEDOR = @IDVENDEDOR AND A.ID_PONTO_VENDA = @IDPONTOVENDA AND A.DATA_INICIAL <= @DATAFINAL
                        AND @DATAINICIAL <= A.DATA_FINAL");

                    //sql.AppendLine("WHERE A.ID_VENDEDOR = @IDVENDEDOR AND (@DATAINICIAL BETWEEN A.DATA_INICIAL AND A.DATA_FINAL) ");
                    //sql.AppendLine("OR (@DATAFINAL BETWEEN A.DATA_INICIAL AND A.DATA_FINAL) ");
                    //sql.AppendLine("OR (@DATAINICIAL >= A.DATA_INICIAL AND @DATAFINAL <= A.DATA_FINAL) ");

                    parametros.Add("@IDVENDEDOR", validacao.Id_Vendedor);
                    parametros.Add("@IDPONTOVENDA", validacao.Id_Ponto_Venda);
                    parametros.Add("@DATAINICIAL", date_ini);
                    parametros.Add("@DATAFINAL", date_fim);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        if (await reader.ReadAsync())
                        {
                            contador = Convert.ToInt32(reader["CO"]);

                            if (contador > 0)
                            {
                                retorno.Ok = false;
                                retorno.Mensagem = "vendedor ocupado";
                            }

                            else
                                retorno.Ok = true;
                        }




                    }

                }
            }

            return retorno;

        }

        // Importação Excel


        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Ponto_VendaImportacao item, Ponto_Venda ponto_vendas)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Ponto_Venda> pontoVenda = new Conexao<Ponto_Venda>(session))
            using (Ponto_VendaApiController ponto_vendaApi = new Ponto_VendaApiController())
            {

                if ((await pontoVenda.selectFirst(p => p.Logradouro == item.Logradouro)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Logradouro,
                        Erro = "A Identificação do endereço já está criado. Endereço não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Numero))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero,
                        Erro = "Número não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Bairro))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Bairro,
                        Erro = "Bairro não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Cidade))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Cidade,
                        Erro = "Cidade não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Complemento))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Complemento,
                        Erro = "Complemento não foi importado."
                    });
                if (String.IsNullOrEmpty(item.UF))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.UF,
                        Erro = "UF não foi importado."
                    });


                if (retorno.Count() > 0)
                    return retorno;

                ponto_vendas.Codigo = item.Codigo;
                ponto_vendas.Status = Status_Default.Ativo;
                ponto_vendas.Nome = item.Descricao;
                ponto_vendas.Cep = item.Cep;
                ponto_vendas.Logradouro = item.Logradouro;
                ponto_vendas.Numero = item.Numero;
                ponto_vendas.Bairro = item.Bairro;
                ponto_vendas.Cidade = item.Cidade;
                ponto_vendas.Complemento = item.Complemento;
                ponto_vendas.Uf = item.UF;
                ponto_vendas.Longitude = Convert.ToDouble(item.Longitude);
                ponto_vendas.Latitude = Convert.ToDouble(item.Latitude);

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(Ponto_VendaImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Ponto_Venda> pontoVenda = new Conexao<Ponto_Venda>(session))
            {
                Ponto_Venda ponto_vendas = new Ponto_Venda();

                retorno = await ImportacaoModeloBase(session, item, ponto_vendas);
                ponto_vendas.Codigo = item.Codigo;
                ponto_vendas.Status = Status_Default.Ativo;
                ponto_vendas.Nome = item.Descricao;
                ponto_vendas.Cep = item.Cep;
                ponto_vendas.Logradouro = item.Logradouro;
                ponto_vendas.Numero = item.Numero;
                ponto_vendas.Bairro = item.Bairro;
                ponto_vendas.Cidade = item.Cidade;
                ponto_vendas.Complemento = item.Complemento;
                ponto_vendas.Longitude = Convert.ToDouble(item.Longitude);
                ponto_vendas.Latitude = Convert.ToDouble(item.Latitude);


                if (retorno.Count() > 0)
                    return retorno;


                await pontoVenda.SaveOrUpdate(ponto_vendas);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Logradouro,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        //importação vendedores

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloVendedor(VendedorImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Funcionario> repoFuncionario = new Conexao<Funcionario>(session))
            using (Conexao<Ponto_Venda> repoPontoVenda = new Conexao<Ponto_Venda>(session))
            using (Conexao<Ponto_Venda_Vendedor> pontoVendaVendedor = new Conexao<Ponto_Venda_Vendedor>(session))

            {
                Ponto_Venda_Vendedor pontovendasVendedor = new Ponto_Venda_Vendedor();

                Funcionario vendedor = (await repoFuncionario.selectFirst(p => p.Num_Fre == item.MatriculaVendedor));
                Ponto_Venda ponto_Venda = (await repoPontoVenda.selectFirst(p => p.Codigo == item.Codigo));

                pontovendasVendedor.Vendedor = vendedor;
                pontovendasVendedor.DataInicial = item.Data_Inicial;
                pontovendasVendedor.DataFinal = item.Data_Final;
                pontovendasVendedor.Ponto_Venda = ponto_Venda;
                
                if (pontovendasVendedor.Vendedor == null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.MatriculaVendedor,
                        Erro = "Vendedor nao foi encontrado."
                    });

                if (ponto_Venda == null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "Ponto de Venda nao foi encontrado."
                    });

                if (retorno.Count() > 0)
                    return retorno;


                await pontoVendaVendedor.SaveOrUpdate(pontovendasVendedor);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.MatriculaVendedor,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

    }
}
