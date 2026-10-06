using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using NHibernate.Criterion;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sinapse.DAO.repository;
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
    public class HorarioApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/HorarioApi/GetAll")]
        public async Task<DataTableResponse<Horario_ListModel>> GetAll(Horario_DataTableRequest request)
        {

            List<Horario_ListModel> retorno = new List<Horario_ListModel>();
            IOrderedEnumerable<Horario_ListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_HORARIO_CONNECT"); //WHERE STATUS = 1  

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

                    sql.AppendLine("SELECT A.ID, A.CODIGO, A.DESCRICAO, A.TIPO, A.STATUS, A.PADRAO");
                    sql.AppendLine(" FROM TB_HORARIO_CONNECT A");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Horario_ListModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Hora_Connect>(Convert.ToString(reader["TIPO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Padrao = Convert.ToBoolean(reader["PADRAO"]) ? "Sim" : "Não"
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
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Horario_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Horario_CadastroModel GetViewObj(Horario_Connect obj)
        {
            return new Horario_CadastroModel
            {
                Id = obj.Id,
                Codigo = obj.Codigo,
                Descricao = obj.Descricao,
                Tipo = obj.Tipo,
                Status = obj.Status == Status_Default.Ativo ? true : false,
                Padrao = obj.Padrao ? true : false,
                Opcao_Trabalha_Feriado = obj.Opcao_Trabalha_Feriado,

                Inicio_Feriado = obj?.Inicio_Feriado?.ToString("HH:mm"),
                Termino_Feriado = obj.Inicio_Intervalo_Feriado?.ToString("HH:mm") != null ? obj?.Termino_Feriado?.ToString("HH:mm") : null,
                Inicio_Intervalo_Feriado = obj.Inicio_Intervalo_Feriado?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Feriado?.ToString("HH:mm") : obj.Termino_Feriado?.ToString("HH:mm"),
                Termino_Intervalo_Feriado = obj?.Termino_Intervalo_Feriado?.ToString("HH:mm"),
                Qtd_HE_Feriado = obj?.Qtde_HE_Feriado?.ToString("HH:mm"),

                Inicio_Domingo = obj?.Inicio_Domingo?.ToString("HH:mm"),
                Termino_Domingo = obj.Inicio_Intervalo_Domingo?.ToString("HH:mm") != null ? obj?.Termino_Domingo?.ToString("HH:mm") : null,
                Inicio_Intervalo_Domingo = obj.Inicio_Intervalo_Domingo?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Domingo?.ToString("HH:mm") : obj.Termino_Domingo?.ToString("HH:mm"),
                Termino_Intervalo_Domingo = obj?.Termino_Intervalo_Domingo?.ToString("HH:mm"),
                Qtd_HE_Domingo = obj?.Qtde_HE_Domingo?.ToString("HH:mm"),

                Inicio_Segunda = obj?.Inicio_Segunda?.ToString("HH:mm"),
                Termino_Segunda = obj.Inicio_Intervalo_Segunda?.ToString("HH:mm") != null ? obj?.Termino_Segunda?.ToString("HH:mm") : null,
                Inicio_Intervalo_Segunda = obj.Inicio_Intervalo_Segunda?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Segunda?.ToString("HH:mm") : obj.Termino_Segunda?.ToString("HH:mm"),
                Termino_Intervalo_Segunda = obj?.Termino_Intervalo_Segunda?.ToString("HH:mm"),
                Qtd_HE_Segunda = obj?.Qtde_HE_Segunda?.ToString("HH:mm"),

                Inicio_Terca = obj?.Inicio_Terca?.ToString("HH:mm"),
                Termino_Terca = obj.Inicio_Intervalo_Terca?.ToString("HH:mm") != null ? obj?.Termino_Terca?.ToString("HH:mm") : null,
                Inicio_Intervalo_Terca = obj.Inicio_Intervalo_Terca?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Terca?.ToString("HH:mm") : obj.Termino_Terca?.ToString("HH:mm"),
                Termino_Intervalo_Terca = obj?.Termino_Intervalo_Terca?.ToString("HH:mm"),
                Qtd_HE_Terca = obj?.Qtde_HE_Terca?.ToString("HH:mm"),

                Inicio_Quarta = obj?.Inicio_Quarta?.ToString("HH:mm"),
                Termino_Quarta = obj.Inicio_Intervalo_Quarta?.ToString("HH:mm") != null ? obj?.Termino_Quarta?.ToString("HH:mm") : null,
                Inicio_Intervalo_Quarta = obj.Inicio_Intervalo_Quarta?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Quarta?.ToString("HH:mm") : obj.Termino_Quarta?.ToString("HH:mm"),
                Termino_Intervalo_Quarta = obj?.Termino_Intervalo_Quarta?.ToString("HH:mm"),
                Qtd_HE_Quarta = obj?.Qtde_HE_Quarta?.ToString("HH:mm"),

                Inicio_Quinta = obj?.Inicio_Quinta?.ToString("HH:mm"),
                Termino_Quinta = obj.Inicio_Intervalo_Quinta?.ToString("HH:mm") != null ? obj?.Termino_Quinta?.ToString("HH:mm") : null,
                Inicio_Intervalo_Quinta = obj.Inicio_Intervalo_Quinta?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Quinta?.ToString("HH:mm") : obj.Termino_Quinta?.ToString("HH:mm"),
                Termino_Intervalo_Quinta = obj?.Termino_Intervalo_Quinta?.ToString("HH:mm"),
                Qtd_HE_Quinta = obj?.Qtde_HE_Quinta?.ToString("HH:mm"),

                Inicio_Sexta = obj?.Inicio_Sexta?.ToString("HH:mm"),
                Termino_Sexta = obj.Inicio_Intervalo_Sexta?.ToString("HH:mm") != null ? obj?.Termino_Sexta?.ToString("HH:mm") : null,
                Inicio_Intervalo_Sexta = obj.Inicio_Intervalo_Sexta?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Sexta?.ToString("HH:mm") : obj.Termino_Sexta?.ToString("HH:mm"),
                Termino_Intervalo_Sexta = obj?.Termino_Intervalo_Sexta?.ToString("HH:mm"),
                Qtd_HE_Sexta = obj?.Qtde_HE_Sexta?.ToString("HH:mm"),

                Inicio_Sabado = obj?.Inicio_Sabado?.ToString("HH:mm"),
                Termino_Sabado = obj.Inicio_Intervalo_Sabado?.ToString("HH:mm") != null ? obj?.Termino_Sabado?.ToString("HH:mm") : null,
                Inicio_Intervalo_Sabado = obj.Inicio_Intervalo_Sabado?.ToString("HH:mm") != null ? obj?.Inicio_Intervalo_Sabado?.ToString("HH:mm") : obj.Termino_Sabado?.ToString("HH:mm"),
                Termino_Intervalo_Sabado = obj?.Termino_Intervalo_Sabado?.ToString("HH:mm"),
                Qtd_HE_Sabado = obj?.Qtde_HE_Sabado?.ToString("HH:mm"),
            };

        }

        private void BindViewToObj(ISession session, Horario_Connect objBanco, Horario_CadastroModel view)
        {
            objBanco.Id = view.Id;

            objBanco.Tipo = view.Tipo;
            objBanco.Codigo = view.Codigo;
            objBanco.Descricao = view.Descricao;
            objBanco.Opcao_Trabalha_Feriado = view.Opcao_Trabalha_Feriado;
            objBanco.Padrao = view.Padrao;

            objBanco.Inicio_Feriado = !String.IsNullOrEmpty(view.Inicio_Feriado) ? Convert.ToDateTime(view.Inicio_Feriado) : (DateTime?)null;
            objBanco.Termino_Feriado = !String.IsNullOrEmpty(view.Termino_Feriado) ? Convert.ToDateTime(view.Termino_Feriado) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Feriado = !String.IsNullOrEmpty(view.Inicio_Intervalo_Feriado) ? Convert.ToDateTime(view.Inicio_Intervalo_Feriado) : (DateTime?)null;
            objBanco.Termino_Intervalo_Feriado = !String.IsNullOrEmpty(view.Termino_Intervalo_Feriado) ? Convert.ToDateTime(view.Termino_Intervalo_Feriado) : (DateTime?)null;
            objBanco.Qtde_HE_Feriado = !String.IsNullOrEmpty(view.Qtd_HE_Feriado) ? Convert.ToDateTime(view.Qtd_HE_Feriado) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Feriado) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Feriado))
            {
                objBanco.Termino_Feriado = Convert.ToDateTime(view.Inicio_Intervalo_Feriado);
                objBanco.Inicio_Intervalo_Feriado = (DateTime?)null;
            }

            objBanco.Inicio_Domingo = !String.IsNullOrEmpty(view.Inicio_Domingo) ? Convert.ToDateTime(view.Inicio_Domingo) : (DateTime?)null;
            objBanco.Termino_Domingo = !String.IsNullOrEmpty(view.Termino_Domingo) ? Convert.ToDateTime(view.Termino_Domingo) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Domingo = !String.IsNullOrEmpty(view.Inicio_Intervalo_Domingo) ? Convert.ToDateTime(view.Inicio_Intervalo_Domingo) : (DateTime?)null;
            objBanco.Termino_Intervalo_Domingo = !String.IsNullOrEmpty(view.Termino_Intervalo_Domingo) ? Convert.ToDateTime(view.Termino_Intervalo_Domingo) : (DateTime?)null;
            objBanco.Qtde_HE_Domingo = !String.IsNullOrEmpty(view.Qtd_HE_Domingo) ? Convert.ToDateTime(view.Qtd_HE_Domingo) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Domingo) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Domingo))
            {
                objBanco.Termino_Domingo = Convert.ToDateTime(view.Inicio_Intervalo_Domingo);
                objBanco.Inicio_Intervalo_Domingo = (DateTime?)null;
            }

            objBanco.Inicio_Segunda = !String.IsNullOrEmpty(view.Inicio_Segunda) ? Convert.ToDateTime(view.Inicio_Segunda) : (DateTime?)null;
            objBanco.Termino_Segunda = !String.IsNullOrEmpty(view.Termino_Segunda) ? Convert.ToDateTime(view.Termino_Segunda) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Segunda = !String.IsNullOrEmpty(view.Inicio_Intervalo_Segunda) ? Convert.ToDateTime(view.Inicio_Intervalo_Segunda) : (DateTime?)null;
            objBanco.Termino_Intervalo_Segunda = !String.IsNullOrEmpty(view.Termino_Intervalo_Segunda) ? Convert.ToDateTime(view.Termino_Intervalo_Segunda) : (DateTime?)null;
            objBanco.Qtde_HE_Segunda = !String.IsNullOrEmpty(view.Qtd_HE_Segunda) ? Convert.ToDateTime(view.Qtd_HE_Segunda) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Segunda) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Segunda))
            {
                objBanco.Termino_Segunda = Convert.ToDateTime(view.Inicio_Intervalo_Segunda);
                objBanco.Inicio_Intervalo_Segunda = (DateTime?)null;
            }

            objBanco.Inicio_Terca = !String.IsNullOrEmpty(view.Inicio_Terca) ? Convert.ToDateTime(view.Inicio_Terca) : (DateTime?)null;
            objBanco.Termino_Terca = !String.IsNullOrEmpty(view.Termino_Terca) ? Convert.ToDateTime(view.Termino_Terca) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Terca = !String.IsNullOrEmpty(view.Inicio_Intervalo_Terca) ? Convert.ToDateTime(view.Inicio_Intervalo_Terca) : (DateTime?)null;
            objBanco.Termino_Intervalo_Terca = !String.IsNullOrEmpty(view.Termino_Intervalo_Terca) ? Convert.ToDateTime(view.Termino_Intervalo_Terca) : (DateTime?)null;
            objBanco.Qtde_HE_Terca = !String.IsNullOrEmpty(view.Qtd_HE_Terca) ? Convert.ToDateTime(view.Qtd_HE_Terca) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Terca) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Terca))
            {
                objBanco.Termino_Terca = Convert.ToDateTime(view.Inicio_Intervalo_Terca);
                objBanco.Inicio_Intervalo_Terca = (DateTime?)null;
            }

            objBanco.Inicio_Quarta = !String.IsNullOrEmpty(view.Inicio_Quarta) ? Convert.ToDateTime(view.Inicio_Quarta) : (DateTime?)null;
            objBanco.Termino_Quarta = !String.IsNullOrEmpty(view.Termino_Quarta) ? Convert.ToDateTime(view.Termino_Quarta) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Quarta = !String.IsNullOrEmpty(view.Inicio_Intervalo_Quarta) ? Convert.ToDateTime(view.Inicio_Intervalo_Quarta) : (DateTime?)null;
            objBanco.Termino_Intervalo_Quarta = !String.IsNullOrEmpty(view.Termino_Intervalo_Quarta) ? Convert.ToDateTime(view.Termino_Intervalo_Quarta) : (DateTime?)null;
            objBanco.Qtde_HE_Quarta = !String.IsNullOrEmpty(view.Qtd_HE_Quarta) ? Convert.ToDateTime(view.Qtd_HE_Quarta) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Quarta) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Quarta))
            {
                objBanco.Termino_Quarta = Convert.ToDateTime(view.Inicio_Intervalo_Quarta);
                objBanco.Inicio_Intervalo_Quarta = (DateTime?)null;
            }

            objBanco.Inicio_Quinta = !String.IsNullOrEmpty(view.Inicio_Quinta) ? Convert.ToDateTime(view.Inicio_Quinta) : (DateTime?)null;
            objBanco.Termino_Quinta = !String.IsNullOrEmpty(view.Termino_Quinta) ? Convert.ToDateTime(view.Termino_Quinta) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Quinta = !String.IsNullOrEmpty(view.Inicio_Intervalo_Quinta) ? Convert.ToDateTime(view.Inicio_Intervalo_Quinta) : (DateTime?)null;
            objBanco.Termino_Intervalo_Quinta = !String.IsNullOrEmpty(view.Termino_Intervalo_Quinta) ? Convert.ToDateTime(view.Termino_Intervalo_Quinta) : (DateTime?)null;
            objBanco.Qtde_HE_Quinta = !String.IsNullOrEmpty(view.Qtd_HE_Quinta) ? Convert.ToDateTime(view.Qtd_HE_Quinta) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Quinta) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Quinta))
            {
                objBanco.Termino_Quinta = Convert.ToDateTime(view.Inicio_Intervalo_Quinta);
                objBanco.Inicio_Intervalo_Quinta = (DateTime?)null;
            }

            objBanco.Inicio_Sexta = !String.IsNullOrEmpty(view.Inicio_Sexta) ? Convert.ToDateTime(view.Inicio_Sexta) : (DateTime?)null;
            objBanco.Termino_Sexta = !String.IsNullOrEmpty(view.Termino_Sexta) ? Convert.ToDateTime(view.Termino_Sexta) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Sexta = !String.IsNullOrEmpty(view.Inicio_Intervalo_Sexta) ? Convert.ToDateTime(view.Inicio_Intervalo_Sexta) : (DateTime?)null;
            objBanco.Termino_Intervalo_Sexta = !String.IsNullOrEmpty(view.Termino_Intervalo_Sexta) ? Convert.ToDateTime(view.Termino_Intervalo_Sexta) : (DateTime?)null;
            objBanco.Qtde_HE_Sexta = !String.IsNullOrEmpty(view.Qtd_HE_Sexta) ? Convert.ToDateTime(view.Qtd_HE_Sexta) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Sexta) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Sexta))
            {
                objBanco.Termino_Sexta = Convert.ToDateTime(view.Inicio_Intervalo_Sexta);
                objBanco.Inicio_Intervalo_Sexta = (DateTime?)null;
            }

            objBanco.Inicio_Sabado = !String.IsNullOrEmpty(view.Inicio_Sabado) ? Convert.ToDateTime(view.Inicio_Sabado) : (DateTime?)null;
            objBanco.Termino_Sabado = !String.IsNullOrEmpty(view.Termino_Sabado) ? Convert.ToDateTime(view.Termino_Sabado) : (DateTime?)null;
            objBanco.Inicio_Intervalo_Sabado = !String.IsNullOrEmpty(view.Inicio_Intervalo_Sabado) ? Convert.ToDateTime(view.Inicio_Intervalo_Sabado) : (DateTime?)null;
            objBanco.Termino_Intervalo_Sabado = !String.IsNullOrEmpty(view.Termino_Intervalo_Sabado) ? Convert.ToDateTime(view.Termino_Intervalo_Sabado) : (DateTime?)null;
            objBanco.Qtde_HE_Sabado = !String.IsNullOrEmpty(view.Qtd_HE_Sabado) ? Convert.ToDateTime(view.Qtd_HE_Sabado) : (DateTime?)null;
            if (String.IsNullOrEmpty(view.Termino_Sabado) && !String.IsNullOrEmpty(view.Inicio_Intervalo_Sabado))
            {
                objBanco.Termino_Sabado = Convert.ToDateTime(view.Inicio_Intervalo_Sabado);
                objBanco.Inicio_Intervalo_Sabado = (DateTime?)null;
            }



        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Horario_ListModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Horario_ListModel>> retorno = new GetApiRetorno<IEnumerable<Horario_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Connect>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = (lista.Select(p => new Horario_ListModel
                    {
                        Id = p.Id,
                        Descricao = $"{p.Codigo} - {p.Descricao}"

                    }));

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
        public async Task<GetApiRetorno<IEnumerable<Horario_ListModel>>> GetListServicos()
        {
            GetApiRetorno<IEnumerable<Horario_ListModel>> retorno = new GetApiRetorno<IEnumerable<Horario_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Connect>(session))
            {
                try
                {
                    var lista = await repository.queryover().Where(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico && p.Status == Status_Default.Ativo).ListAsync();

                    retorno.Objeto = (lista.Select(p => new Horario_ListModel
                    {
                        Id = p.Id,
                        Descricao = $"{p.Codigo} - {p.Descricao}"
                    }));

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
        public async Task<GetApiRetorno<IEnumerable<Horario_ListModel>>> GetListAtendimento()
        {
            GetApiRetorno<IEnumerable<Horario_ListModel>> retorno = new GetApiRetorno<IEnumerable<Horario_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Connect>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Atendimento && p.Status == Status_Default.Ativo);

                    retorno.Objeto = (lista.Select(p => new Horario_ListModel
                    {
                        Id = p.Id,
                        Descricao = $"{p.Codigo} - {p.Descricao}"

                    }));

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
        public async Task<GetApiRetorno<IEnumerable<Horario_ListModel>>> GetListAtendimentoPadrao()
        {
            GetApiRetorno<IEnumerable<Horario_ListModel>> retorno = new GetApiRetorno<IEnumerable<Horario_ListModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Connect>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Atendimento && p.Status == Status_Default.Ativo && p.Padrao == true);

                    retorno.Objeto = (lista.Select(p => new Horario_ListModel
                    {
                        Id = p.Id,
                        Descricao = $"{p.Codigo} - {p.Descricao}"

                    }));

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
        [Route("api/HorarioApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_HORARIO_CONNECT SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/HorarioApi/FindById")]
        public async Task<GetApiRetorno<Horario_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Horario_CadastroModel> retorno = new GetApiRetorno<Horario_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Connect>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

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
        [Route("api/HorarioApi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_HORARIO_CONNECT WHERE ID IN {_ids}");

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
        [Route("api/Horarioapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Horario_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Horario_Connect modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Horario_Connect>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        //bool padrao = value.Objeto.Padrao == "Sim" ? true : false;

                        if (value.Objeto.Padrao)
                        {
                            if (value.Objeto.Status == true || value.Objeto.Id == 0)
                            {

                                var checarPadrao = value.Objeto.Padrao ? (await repository.select(p => p.Tipo == value.Objeto.Tipo && p.Padrao == true && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo)) : null;

                                if (checarPadrao != null)
                                {
                                    if (checarPadrao.Count() > 0)
                                        throw new Exception("Existe horário padrão já cadastrado para este tipo de horário.");
                                }
                            }
                        }

                        if (value.Objeto.Tipo == Tipo_Hora_Connect.Horario_Agendamento)
                        {

                            if (value.Objeto.Status == true || value.Objeto.Id == 0)
                            {
                                var _horarioagendamento = (await repository.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Agendamento && p.Status == Status_Default.Ativo && p.Id != value.Objeto.Id));
                                if (_horarioagendamento.Count() > 0)
                                    throw new Exception("Não é possível ativar mais de um horário do Tipo Agendamento no sistema.");

                            }
                        }

                        if (modelo == null)
                        {
                            modelo = new Horario_Connect
                            {
                                Status = Status_Default.Ativo,
                                //Codigo = Convert.ToString((await repository.MaxString("CODIGO", "TB_HORARIO_CONNECT")) + 1),
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Horarioapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_HORARIO_CONNECT A WHERE ");

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
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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
        [Route("api/Horarioapi/GetComboJornadaDia")]
        public async Task<ObjCombo<Int64>> GetComboJornadaDia(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<Horario_Connect>(session))
            {
                try
                { //CARD 203 Apoio Implantação - Daiane Fontoura
                    var lista = await repo.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico);
                    if (request.soativos)
                    {
                        lista = lista.Where(p => p.Status == Status_Default.Ativo).ToList();
                    }

                    if (request.id.HasValue)
                    {
                        lista = lista.Where(p => p.Id == request.id).ToList();
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        lista = lista.Where(p => p.Descricao.Contains(request.q)).ToList();
                    }

                    foreach (var item in lista)
                    {
                        var resultado = RetornaHorarioDia(item, DateTime.Now, false);
                        retorno.results.Add(new HorarioComboResult
                        {
                            Id = item.Id,
                            Text = $"{item.Codigo} - {item.Descricao}",
                            JornadaDia = resultado.inicio.HasValue ? (resultado.inicio.Value.ToString("HH:mm") + "-" + resultado.termino.Value.ToString("HH:mm")) : String.Empty
                        });
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        #region Mobile
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Horarioapi/GetIsFeriado")]
        public async Task<GetApiRetorno<bool>> GetIsFeriado(GetIsFeriadoModel value)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();
            try
            {
                var data = !String.IsNullOrEmpty(value.data) ? DateTime.Parse(value.data).Date : DateTime.Now.Date;
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Feriado>(session))
                {
                    //bool feriado = (await repo.select(p => p.Data.Value >= data && p.Data.Value < data.AddHours(23).AddMinutes(59).AddSeconds(59))).ToList().Count() > 0;
                    bool feriado = (await repo.select(p => p.Data.Value == data)).ToList().Count() > 0;                    

                    retorno.Objeto = feriado;
                }

                retorno.Ok = true;

            }
            catch (Exception ex)
            {
                retorno.Objeto = false;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }

        #endregion

        public (DateTime? inicio, DateTime? inicio_intervalo, DateTime? termino_intervalo, DateTime? termino) RetornaHorarioDia(Horario_Connect horario, DateTime date, bool feriado)
        {
            if (feriado)
                return (horario.Inicio_Feriado, horario.Inicio_Intervalo_Feriado, horario.Termino_Intervalo_Feriado, horario.Termino_Feriado);

            switch (date.Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return (horario.Inicio_Domingo, horario.Inicio_Intervalo_Domingo, horario.Termino_Intervalo_Domingo, horario.Termino_Domingo);
                case DayOfWeek.Monday:
                    return (horario.Inicio_Segunda, horario.Inicio_Intervalo_Segunda, horario.Termino_Intervalo_Segunda, horario.Termino_Segunda);
                case DayOfWeek.Tuesday:
                    return (horario.Inicio_Terca, horario.Inicio_Intervalo_Terca, horario.Termino_Intervalo_Terca, horario.Termino_Terca);
                case DayOfWeek.Wednesday:
                    return (horario.Inicio_Quarta, horario.Inicio_Intervalo_Quarta, horario.Termino_Intervalo_Quarta, horario.Termino_Quarta);
                case DayOfWeek.Thursday:
                    return (horario.Inicio_Quinta, horario.Inicio_Intervalo_Quinta, horario.Termino_Intervalo_Quinta, horario.Termino_Quinta);
                case DayOfWeek.Friday:
                    return (horario.Inicio_Sexta, horario.Inicio_Intervalo_Sexta, horario.Termino_Intervalo_Sexta, horario.Termino_Sexta);
                case DayOfWeek.Saturday:
                    return (horario.Inicio_Sabado, horario.Inicio_Intervalo_Sabado, horario.Termino_Intervalo_Sabado, horario.Termino_Sabado);
                default:
                    return (null, null, null, null);

            }
        }


        public DateTime? RetornaHoraExtraDia(Horario_Connect horario, DateTime date, bool feriado)
        {
            if (feriado)
                return horario.Qtde_HE_Feriado;

            switch (date.Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return horario.Qtde_HE_Domingo;
                case DayOfWeek.Monday:
                    return horario.Qtde_HE_Segunda;
                case DayOfWeek.Tuesday:
                    return horario.Qtde_HE_Terca;
                case DayOfWeek.Wednesday:
                    return horario.Qtde_HE_Quarta;
                case DayOfWeek.Thursday:
                    return horario.Qtde_HE_Quinta;
                case DayOfWeek.Friday:
                    return horario.Qtde_HE_Sexta;
                case DayOfWeek.Saturday:
                    return horario.Qtde_HE_Sabado;
                default:
                    return (null);

            }
        }

        public (DateTime? inicio, DateTime? inicio_intervalo, DateTime? termino_intervalo, DateTime? termino) RetornaHorarioDiaPoint(Tb_Pt_Jornada_Funcionario horario, DateTime date, bool feriado)
        {
            if (feriado)
                return (horario.Inicio_Feriado, horario.Inicio_Intervalo_Feriado, horario.Termino_Intervalo_Feriado, horario.Termino_Feriado);

            switch (date.Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return (horario.Inicio_Domingo, horario.Inicio_Intervalo_Domingo, horario.Termino_Intervalo_Domingo, horario.Termino_Domingo);
                case DayOfWeek.Monday:
                    return (horario.Inicio_Segunda, horario.Inicio_Intervalo_Segunda, horario.Termino_Intervalo_Segunda, horario.Termino_Segunda);
                case DayOfWeek.Tuesday:
                    return (horario.Inicio_Terca, horario.Inicio_Intervalo_Terca, horario.Termino_Intervalo_Terca, horario.Termino_Terca);
                case DayOfWeek.Wednesday:
                    return (horario.Inicio_Quarta, horario.Inicio_Intervalo_Quarta, horario.Termino_Intervalo_Quarta, horario.Termino_Quarta);
                case DayOfWeek.Thursday:
                    return (horario.Inicio_Quinta, horario.Inicio_Intervalo_Quinta, horario.Termino_Intervalo_Quinta, horario.Termino_Quinta);
                case DayOfWeek.Friday:
                    return (horario.Inicio_Sexta, horario.Inicio_Intervalo_Sexta, horario.Termino_Intervalo_Sexta, horario.Termino_Sexta);
                case DayOfWeek.Saturday:
                    return (horario.Inicio_Sabado, horario.Inicio_Intervalo_Sabado, horario.Termino_Intervalo_Sabado, horario.Termino_Sabado);
                default:
                    return (null, null, null, null);

            }
        }

        public bool AptoHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino,
                                DateTime hora_inicio, DateTime? hora_fim)
        {
            if (!horario_inicio.HasValue || !horario_termino.HasValue)
                return false;

            if (hora_inicio.TimeOfDay < horario_inicio?.TimeOfDay || hora_fim?.TimeOfDay > horario_termino?.TimeOfDay)
            {
                return false;
            }

            if (horario_inicio_intervalo.HasValue && horario_termino_intervalo.HasValue)
            {

                if (Between(hora_inicio, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (Between(hora_fim, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (hora_inicio.TimeOfDay < horario_inicio_intervalo?.TimeOfDay && hora_fim?.TimeOfDay > horario_inicio_intervalo?.TimeOfDay)
                    return false;

            }



            return true;
        }

        public double MinutosTrabalhadosHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino)
        {
            double totalMinutos = 0;
            if (horario_inicio.HasValue && horario_termino.HasValue)
            {
                totalMinutos += (horario_termino.Value - horario_inicio.Value).TotalMinutes;

                if (horario_inicio_intervalo.HasValue && horario_termino_intervalo.HasValue)
                {
                    var minutosIntervalo = (horario_termino_intervalo.Value - horario_inicio_intervalo.Value).TotalMinutes;
                    totalMinutos -= minutosIntervalo;
                }
            }

            return totalMinutos;
        }

        public static bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            date1 = date1.Value.AddMinutes(1);
            date2 = date2.Value.AddMinutes(-1);
            return (input?.TimeOfDay > date1.Value.TimeOfDay && input.Value.TimeOfDay < date2.Value.TimeOfDay);
        }
        public static bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }
        public static bool IgualDatas(DateTime? date1, DateTime? date2)
        {
            return (date1.Value.TimeOfDay == date2.Value.TimeOfDay);
        }
    }
}