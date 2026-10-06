using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
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
    public class PonApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/PonApi/GetAll")]
        public async Task<DataTableResponse<PonListaModel>> GetAll(Pon_DataTableRequest request)
        {

            List<PonListaModel> retorno = new List<PonListaModel>();
            IOrderedEnumerable<PonListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_PON A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_SLOT B ");
                sqlFrom.AppendLine(" ON A.ID_SLOT = B.ID ");
                sqlFrom.AppendLine(" LEFT JOIN TB_OLT C ");
                sqlFrom.AppendLine(" ON B.ID_OLT = C.ID ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FILIAL D ");
                sqlFrom.AppendLine(" ON C.ID_FILIAL = D.ID ");
                sqlFrom.AppendLine(" LEFT JOIN TB_EMPRESA E ");
                sqlFrom.AppendLine(" ON D.ID_EMPRESA = E.ID ");
                sqlFrom.AppendLine(" WHERE A.ID > 0");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "pon")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "olt")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(C.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "slot")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(B.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }

                if (!String.IsNullOrEmpty(request.Id_Pon))
                {
                    sqlFrom.AppendLine("AND (A.ID = @ID_PON)");
                    parametros.Add("@ID_PON", Convert.ToInt32(request.Id_Pon));
                }

                if (!String.IsNullOrEmpty(request.Id_Olt))
                {
                    sqlFrom.AppendLine("AND (C.ID = @IDOLT)");
                    parametros.Add("@IDOLT", Convert.ToInt32(request.Id_Olt));
                }

                if (!String.IsNullOrEmpty(request.Id_Slot))
                {
                    sqlFrom.AppendLine("AND (A.ID_SLOT = @ID_SLOT)");
                    parametros.Add("@ID_SLOT", Convert.ToInt32(request.Id_Slot));
                }

                if (!String.IsNullOrEmpty(request.Status_Filtro))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
                }

                //sqlFrom.AppendLine(" ORDER BY A.ID ");

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine(" SELECT A.ID, A.NUM_PON, A.DESCRICAO, A.INTERFACE, B.DESCRICAO AS SLOT, C.DESCRICAO AS OLT, A.STATUS, B.SLOTID, C.OLTID, D.CODFILIAL, E.CODEMPRESA ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PonListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Num_PON = Convert.ToString(reader["NUM_PON"]),
                                Interface = Convert.ToString(reader["INTERFACE"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Slot = Convert.ToString(reader["SLOT"]),
                                OLT = Convert.ToString(reader["OLT"]),
                                Situacao = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO
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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Id);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<PonListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/PONApi/GetTreeView")]
        public async Task<GetApiRetorno<List<SlotTreeViewModel>>> GetTreeView(Int64 Id_Slot, Int64? Id_PON)
        {
            GetApiRetorno<List<SlotTreeViewModel>> retorno = new GetApiRetorno<List<SlotTreeViewModel>>
            {
                Objeto = new List<SlotTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>
                        {
                            { "@ID_SLOT", Id_Slot }
                        };
                        StringBuilder sql = new StringBuilder($"SELECT ID, DESCRICAO FROM TB_PON WHERE ID_SLOT = @ID_SLOT");

                        if (Id_PON.HasValue)
                        {
                            sql.AppendLine(" AND ID = @ID_PON");
                            parametros.Add("@ID_PON", Id_PON);
                        }

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new SlotTreeViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"])
                            });
                        }

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<PonListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<PonListaModel>> retorno = new GetApiRetorno<IEnumerable<PonListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<PON>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new PonListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,


                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }


        [HttpGet()]
        public async Task<GetApiRetorno<IEnumerable<SlotListaModel>>> GetListSlot()
        {
            GetApiRetorno<IEnumerable<SlotListaModel>> retorno = new GetApiRetorno<IEnumerable<SlotListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoSlot = new Conexao<Slot>(session))
            {
                try
                {
                    var lista = await repoSlot.queryover().ListAsync();

                    retorno.Objeto = new List<SlotListaModel>(lista.Select(p => new SlotListaModel
                    {
                        Id = p.Id,
                        Descricao = $"{p.SlotID}"

                    }));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/PONApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PON A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_SLOT = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = $"{reader["DESCRICAO"]}"
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retorno;

        }

        private PonCadastroModel GetViewObj(PON obj)
        {
            return new PonCadastroModel
            {
                Id = obj.Id,

                NumPON = obj.Num_PON,
                Descricao = obj.Descricao,

                Interface = obj.Interface,

                Id_OLT = obj.OLT?.Id,
                Id_Slot = obj.Slot?.Id,

                Status = obj.Status == Status_Default.Ativo,
                PotenciaSaida = obj.Potencia_Saida,
                LimitePotenciaONUBaixa = Convert.ToString(obj.LimitePotenciaONUBaixa, System.Globalization.CultureInfo.InvariantCulture),

                VLAN_Uplink = obj.VLAN_Uplink,
                VLAN_PPPOE = obj.VLAN_PPPOE,
                VLAN_DHCP = obj.VLAN_DHCP,
                VLAN_IPTV = obj.VLAN_IPTV,
                VLAN_VOIP = obj.VLAN_VOIP,
                VLAN_TR69 = obj.VLAN_TR69,
                VLAN_Outros = obj.VLAN_Outros,
                VLAN_Profile = obj.VLAN_Profile,
                CodigoAuxiliar = obj.CodigoAuxiliar
            };
        }

        private async Task BindViewToObj(ISession session, PON objBanco, PonCadastroModel view)
        {
            using (var repoPON = new Conexao<PON>(session))
            using (var repoOLT = new Conexao<OLT>(session))
            using (var repoSlot = new Conexao<Slot>(session))
            {
                objBanco.Descricao = view.Descricao;

                if (view.NumPON.HasValue)
                {
                    var PONNumRepetido =
                        await repoPON.selectFirst(
                            p => p.Num_PON == view.NumPON &&
                                 p.OLT.Id == view.Id_OLT &&
                                 p.Slot.Id == view.Id_Slot &&
                                 p.Id != objBanco.Id
                        );

                    if (PONNumRepetido != null)
                        throw new Exception("Num. PON já cadastrado no sistema");
                    else
                        objBanco.Num_PON = view.NumPON;
                }

                objBanco.Interface = view.Interface;

                objBanco.OLT = view.Id_OLT.HasValue ? await repoOLT.selectFirst(p => p.Id == view.Id_OLT) : null;
                objBanco.Slot = view.Id_Slot.HasValue ? await repoSlot.selectFirst(p => p.Id == view.Id_Slot) : null;

                objBanco.Potencia_Saida = view.PotenciaSaida;

                if(view.LimitePotenciaONUBaixa != null)
                {
                    objBanco.LimitePotenciaONUBaixa = double.Parse(view.LimitePotenciaONUBaixa.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                }

                objBanco.VLAN_Uplink = view.VLAN_Uplink;
                objBanco.VLAN_PPPOE = view.VLAN_PPPOE;
                objBanco.VLAN_DHCP = view.VLAN_DHCP;
                objBanco.VLAN_IPTV = view.VLAN_IPTV;
                objBanco.VLAN_VOIP = view.VLAN_VOIP;
                objBanco.VLAN_TR69 = view.VLAN_TR69;
                objBanco.VLAN_Outros = view.VLAN_Outros;
                objBanco.VLAN_Profile = view.VLAN_Profile;
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;
                
            }
        }

        [HttpPost()]
        [Route("api/PonApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_SLOT WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/PonApi/FindById")]
        public async Task<GetApiRetorno<PonCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<PonCadastroModel> retorno = new GetApiRetorno<PonCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<PON>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/PonApi/FindByPonId")]
        public async Task<GetApiRetorno<PON>> FindByPonId(Int64 PonId, Int64 SlotId, Int64 Id_OLT)
        {
            GetApiRetorno<PON> retorno = new GetApiRetorno<PON>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    PON pon = null;
                    Slot slot = null;
                    OLT olt = null;
                    Filial filial = null;
                    Empresa empresa = null;

                    retorno.Objeto = (await session.QueryOver(() => pon)
                                                   .JoinAlias(() => pon.Slot, () => slot)
                                                   .JoinAlias(() => slot.OLT, () => olt)
                                                   .JoinAlias(() => olt.Filial, () => filial)
                                                   .JoinAlias(() => filial.Empresa, () => empresa)
                                                   .Where(p => p.PONID == PonId && slot.SlotID == SlotId && olt.Id == Id_OLT).Take(1).ListAsync()).FirstOrDefault();

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/PonApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<PonCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                PON modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<PON>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new PON
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo,
                                PONID = (await repository.Max(p => p.PONID, y => y.Slot.Id == value.Objeto.Id_Slot)) + 1
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/SlotApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<PON>(session))
                    {

                        PON modelo = null;
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
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/PONapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PON SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/PONapi/CriarPONsEmMassa")]
        public async Task<GetApiRetorno<bool>> CriarPONsEmMassa(CriarPONSsEmMassa_Request value)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();
            bool pon_existe = false;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOlt = new Conexao<OLT>(session))
                using (var repoSlot = new Conexao<Slot>(session))
                using (var repoPON = new Conexao<PON>(session))
                {
                    OLT olt = await repoOlt.selectFirst(p => p.Id == value.Id_OLT.Value);
                    Slot slot = await repoSlot.selectFirst(p => p.OLT.Id == olt.Id && p.Id == value.Id_Slot);

                    if (!value.Num_PON_Fim.HasValue)
                    {
                        value.Num_PON_Fim = value.Num_PON_Inicial;
                    }

                    List<PON> lista_pons = new List<PON>();
                    var lista_pons_banco = (await repoPON.select(p => p.OLT.Id == olt.Id && p.Slot.Id == slot.Id)).ToList();

                    for (var i = value.Num_PON_Inicial; i <= value.Num_PON_Fim; i++)
                    {
                        if (!lista_pons_banco.Exists(x => x.Num_PON == i))
                        {
                            lista_pons.Add(new PON
                            {
                                DataHoraCriacao = DateTime.Now,
                                Status = Status_Default.Ativo,
                                OLT = olt,
                                Slot = slot,
                                PONID = (await repoPON.Max(p => p.PONID, y => y.Slot.Id == slot.Id)) + 1,
                                Num_PON = i,
                                Descricao = StringService.Truncate(String.Format("PON {0} - {1} - {2}", i, slot.Descricao, olt.Descricao), 100),
                                Potencia_Saida = 0,
                                VLAN_Uplink = value.Vlan_PON.HasValue ? value.Vlan_PON : (short?)null
                            });
                        }
                        else
                        {
                            pon_existe = true;
                            PON pon = lista_pons_banco.FirstOrDefault(x => x.Num_PON == i);
                            if (pon != null)
                            {
                                if (value.Vlan_PON.HasValue)
                                    pon.VLAN_Uplink = value.Vlan_PON;

                                lista_pons.Add(pon);
                            }
                        }
                    }

                    if (lista_pons.Count > 0)
                    {
                        await repoPON.SaveOrUpdate(lista_pons);
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task<GetPonByInterfaceModel> GetPonByInterface(string DescOLT, int NumSlot, int NumPon)
        {
            GetPonByInterfaceModel retorno = null;

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                /*SELECT A.ID AS ID_OLT, B.ID AS ID_SLOT, B.NUM_SLOT AS NUM_SLOT, C.ID AS ID_PON, C.NUM_PON AS NUM_PON 
                FROM TB_OLT A
                INNER JOIN TB_SLOT B 
                ON (A.ID = B.ID_OLT)
                INNER JOIN TB_PON C
                ON (B.ID = C.ID_SLOT)
                WHERE UPPER(A.DESCRICAO = :DESCRICAO)
                AND B.NUM_SLOT = :NUM_SLOT
                AND C.NUM_PON  = :NUM_PON*/
                sql.AppendLine(" SELECT A.ID AS ID_OLT, A.TIPO_IDENTIFICACAO, B.ID AS ID_SLOT, B.NUM_SLOT AS NUM_SLOT, C.ID AS ID_PON, C.NUM_PON AS NUM_PON ");
                sql.AppendLine(" FROM TB_OLT A ");
                sql.AppendLine(" LEFT JOIN TB_SLOT B ");
                sql.AppendLine(" ON (A.ID = B.ID_OLT) ");
                sql.AppendLine(" LEFT JOIN TB_PON C ");
                sql.AppendLine(" ON (B.ID = C.ID_SLOT) ");
                sql.AppendLine(" WHERE UPPER(A.DESCRICAO) = UPPER(@DESCRICAO_OLT) ");
                sql.AppendLine(" AND B.NUM_SLOT = @NUM_SLOT ");
                sql.AppendLine(" AND C.NUM_PON  = @NUM_PON ");

                parametros.Add("@DESCRICAO_OLT", DescOLT);
                parametros.Add("@NUM_SLOT", NumSlot);
                parametros.Add("@NUM_PON", NumPon);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new GetPonByInterfaceModel();
                        retorno.Id_OLT = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : (long?)null;
                        retorno.Id_Slot = reader["ID_SLOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_SLOT"]) : (long?)null;
                        retorno.Id = reader["ID_PON"] != DBNull.Value ? Convert.ToInt64(reader["ID_PON"]) : 0;
                        retorno.NumPON = reader["NUM_PON"] != DBNull.Value ? Convert.ToInt64(reader["NUM_PON"]) : 0;
                        retorno.ONU_Tipo_Identificacao = reader["TIPO_IDENTIFICACAO"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_IDENTIFICACAO"]) : 0;
                    }
                }
            }

            return retorno;
        }
    }
}
