using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class SLA_ServicoApiController : BaseApiController
    {

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]        
        [Route("api/SLA_Servicoapi/GetAll")]
        public async Task<DataTableResponse<SLA_ServicoListaModel>> GetAll(SLA_Servico_DataTableRequest request)
        {
            List<SLA_ServicoListaModel> retorno = new List<SLA_ServicoListaModel>();
            IOrderedEnumerable<SLA_ServicoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.ID, C.DESCRICAO as CATEGORIA_CLIENTE, A.MINUTOS , A.TIPO_SLA, ")
                    .AppendLine(" D.DESCRICAO as HORARIO_TRABALHO,  B.DESCRICAO as TIPO_SERVICO , A.STATUS, A.CODCT, E.DESCRICAO ")
                    .AppendLine(" FROM TB_SLA_SERVICO A ")

                    .AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ")
                    .AppendLine(" ON (A.ID_TIPO_SERVICO = B.COD_TIPO_SERV) ")

                    .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE_CONNECT C ")
                    .AppendLine(" ON (A.ID_CATEGORIA_CLIENTE = C.ID) ")

                    .AppendLine(" LEFT JOIN TB_HORARIO_CONNECT D ")
                    .AppendLine(" ON (A.id_horario_connect = D.ID) ")

                    .AppendLine(" LEFT JOIN TB_CONTRATO E ")
                    .AppendLine(" ON (A.CODCT = E.CODCT) ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE (UPPER(B.DESCRICAO) LIKE UPPER(@SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var _categoriastring = Convert.ToString(reader["CATEGORIA_CLIENTE"]);
                            retorno.Add(new SLA_ServicoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Contrato = $"{Convert.ToString(reader["CODCT"])} - {Convert.ToString(reader["DESCRICAO"])}",
                                Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                Categoria_Cliente = _categoriastring == "" ? Resources.RsBase.INDIFERENTE : _categoriastring,
                                Horas = reader["MINUTOS"] != DBNull.Value ? IntService.ToTimeString(Convert.ToInt32(reader["MINUTOS"])) : String.Empty,
                                Tipo   = HtmlEnumExtensions.GetEnumDescription<Tipo_SLA>(Convert.ToString(reader["TIPO_SLA"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Horario_Trabalho = Convert.ToString(reader["HORARIO_TRABALHO"]),
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
                    case 1:     // Tipo_Servico
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Servico);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Servico);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<SLA_ServicoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private SLA_ServicoCadastroModel GetViewObj(SLA_Servico obj)
        {
            return new SLA_ServicoCadastroModel
            {
                Id = obj.Id,
                CodCt = obj.CodCt,
                Id_Tipo_Servico = obj.Tipo_Servico?.Cod_Tipo_Serv,
                Tipo_Servico = obj.Tipo_Servico?.Descricao,
                Id_Categoria_Cliente = obj.Categoria_Cliente?.Id,
                Id_Horario_Connect = obj.Horario_Trabalho?.Id,
                Tipo = obj.Tipo,
                Horas = IntService.ToTime99String(obj.Minutos),
                Status = obj?.Status.Value == Status_Default.Ativo ? true : false
            };
        }

        private void BindViewToObj(ISession session, ref SLA_Servico objBanco, SLA_ServicoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.CodCt = view.CodCt;
            objBanco.Tipo = view.Tipo;
            using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(session))
                objBanco.Tipo_Servico = repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == view.Id_Tipo_Servico).Result;

            using (var repoCategoria_ClienteAniel = new Conexao<Categoria_ClienteAniel>(session))
                objBanco.Categoria_Cliente = repoCategoria_ClienteAniel.selectFirst(p => p.Id == view.Id_Categoria_Cliente).Result;

            using (var repoHorario_Connect = new Conexao<Horario_Connect>(session))
                objBanco.Horario_Trabalho = view.Id_Horario_Connect != null ? repoHorario_Connect.selectFirst(p => p.Id == view.Id_Horario_Connect.Value).Result : null;

            objBanco.Minutos = IntService.GetMinutos(view.Horas);
                        

            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }
            
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/SLA_Servicoapi/RecalcularSLA")]
        public async Task<PostApiRetorno<Int64>> RecalcularSLA(long idSla)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionNh = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<SLA_Servico>(sessionNh))
                using (var repositoryOS = new Conexao<Documento_ProducaoAniel>(sessionNh))
                using(var repositoryMaster = new Conexao<Master>(sessionNh))
                {
                    SLA_Servico sla = await repository.selectFirst(p => p.Id == idSla);
                    IList<Documento_ProducaoAniel> docProd = await repositoryOS.select(p => p.Status != StatusExecucaoAniel.Cancelado && 
                    p.Status != StatusExecucaoAniel.Fechada_Improdutiva &&
                    p.Status != StatusExecucaoAniel.Fechada_Produtiva && p.Cod_Tipo_Serv == sla.Tipo_Servico.Cod_Tipo_Serv  ); //|| p.Cliente.Categoria_Cliente.Id == sla.Categoria_Cliente.Id)
                    Master tbMaster = await repositoryMaster.selectFirst();




                    foreach (var item in docProd)
                    {
                        DateTime? dataReferencia = (DateTime?)null;
                        DateTime? horaReferencia = (DateTime?)null;
                        if (tbMaster.SLA_Data_Abertura == Enum_Tipo_Sla_.Abertura)
                        {
                            dataReferencia = item.Dia;
                            horaReferencia = item.Hora;
                        } 
                        else
                            
                        if (tbMaster.SLA_Data_Abertura == Enum_Tipo_Sla_.Agendamento)                          
                        {                     
                            dataReferencia = item.Data_Maxima;
                            horaReferencia = item.Hora_Maxima;                         
                        }

                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        using (var com = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder(@"
                            SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
                            FROM SP_CONNECT_DOCPROD_SLA(@DIA, @HORA, @ID_CLIENTE,
                            @COD_TIPO_SERV, @ENDERECO_UF, @ENDERECO_CIDADE) A");

                            parametros.Add("@DIA", dataReferencia);
                            parametros.Add("@HORA", horaReferencia);
                            parametros.Add("@ID_CLIENTE", item.Cliente?.Cod_Cliente);
                            parametros.Add("@COD_TIPO_SERV", item.Cod_Tipo_Serv);
                            parametros.Add("@ENDERECO_UF", item.UF);
                            parametros.Add("@ENDERECO_CIDADE", item.Cidade);

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    TimeSpan? horaIndicador = reader["RHORA_INDICADOR"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan?) reader["RHORA_INDICADOR"];
                                    DateTime? dataIndicador = reader["RDATA_INDICADOR"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["RDATA_INDICADOR"]);

                                    if (horaIndicador.HasValue && dataIndicador.HasValue)
                                    {
                                        item.Data_Indicador = dataIndicador;
                                        item.Hora_Indicador = new DateTime(horaIndicador.Value.Ticks);
                                        await repositoryOS.Update(item);
                                    }


                                }
                            }
                        }

                    }

             
              }

                retorno.Ok = true;
               
               
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/SLA_Servicoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_SLA_Servico WHERE ID IN {_ids}");

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
        [Route("api/SLA_ServicoApi/FindById")]
        public async Task<GetApiRetorno<SLA_ServicoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<SLA_ServicoCadastroModel> retorno = new GetApiRetorno<SLA_ServicoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<SLA_Servico>(session))
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
        [Route("api/SLA_ServicoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<SLA_ServicoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                SLA_Servico modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<SLA_Servico>(session))
                    {

                        if (value.Objeto.Id_Categoria_Cliente == null)
                        {
                            if (value.Objeto.Status == true || value.Objeto.Id == 0)
                            {
                                var sla_cadastrados = await repository.select(
                                                       p => p.Tipo_Servico.Cod_Tipo_Serv == value.Objeto.Id_Tipo_Servico
                                                       && p.Status == Status_Default.Ativo && p.Id != value.Objeto.Id) ;
                                if (sla_cadastrados.Count() > 0)
                                {
                                    throw new Exception("Não é possível criar/ativar um SLA com categoria indiferente para este tipo de serviço, pois já existem outros SLAs ativos");
                                }
                            }
                        }


                        if (value.Objeto.Status == true || value.Objeto.Id == 0)
                        {
                            var obj2 = await repository.select(p => p.Categoria_Cliente == null &&
                            p.Tipo_Servico.Cod_Tipo_Serv == value.Objeto.Id_Tipo_Servico && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo);
                            if (obj2.Count > 0)
                            {
                                throw new Exception("Este tipo de serviço já possui um SLA com categoria de cliente indiferente ativo.Não é possível criar outro.");
                            }
                        }

                        if (value.Objeto.Status == true || value.Objeto.Id == 0)
                        {
                            var obj = await repository.select(p => p.Categoria_Cliente.Id == value.Objeto.Id_Categoria_Cliente &&
                           p.Tipo_Servico.Cod_Tipo_Serv == value.Objeto.Id_Tipo_Servico && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo);
                            if (obj.Count > 0)
                            {
                                throw new Exception("Ja possui um SLA de serviço com essa configuração");
                            }
                        }

                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new SLA_Servico
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id != 0 ? value.Objeto.Id : 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/SLA_ServicoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<SLA_Servico>(session))
                    {

                        SLA_Servico modelo = null;
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

        [HttpPost()]
        [Route("api/SLA_ServicoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_SLA_Servico SET STATUS = {value.status} " +
                            $"                                  WHERE ID IN {_ids}");

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

    }
}
