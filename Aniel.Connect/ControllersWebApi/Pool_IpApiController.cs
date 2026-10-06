using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NetTools;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections;
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
    public class Pool_IpApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Pool_Ipapi/GetAll")]
        public async Task<DataTableResponse<Pool_IpListaModel>> GetAll(Pool_Ip_DataTableRequest request)
        {
            List<Pool_IpListaModel> retorno = new List<Pool_IpListaModel>();
            IOrderedEnumerable<Pool_IpListaModel> listaOrdenada = null;
           Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> condicoes = new List<String>();
                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.Append("FROM TB_POOL_IP A ");

                if (!String.IsNullOrEmpty(request.Status)) 
                {
                    condicoes.Add("STATUS = @STATUS");
                    parametros.Add("@STATUS", Convert.ToInt16(request.Status));
                }

                if (!String.IsNullOrEmpty(request.Tipo_Ip)) 
                {
                    condicoes.Add("PADRAO_IP = @PADRAO_IP");
                    parametros.Add("@PADRAO_IP", Convert.ToInt16(request.Tipo_Ip));
                }
                
                if (!String.IsNullOrEmpty(request.Concentradora)) 
                {
                    condicoes.Add("ID_CONCENTRADORA = @ID_CONCENTRADORA");
                    parametros.Add("@ID_CONCENTRADORA", Convert.ToInt32(request.Concentradora));
                }

                if (!String.IsNullOrEmpty(request.Radius)) 
                {
                    condicoes.Add("ID_RADIUS = @ID_RADIUS");
                    parametros.Add("@ID_RADIUS", Convert.ToInt32(request.Radius));
                }

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    condicoes.Add(" UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }



                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                }


                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO ");
                    sql.Append(sqlFrom);

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

                    sql.AppendLine(" SELECT A.ID, A.IP_INICIO_RANGE, A.IP_FIM_RANGE, A.MASCARA_IP_CIDR, A.DESCRICAO, A.STATUS"); 
                    sql.Append(sqlFrom);    
                   
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Pool_IpListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Ip_Inicio_Range = Convert.ToString(reader["IP_INICIO_RANGE"]),
                                Ip_Fim_Range = reader["IP_FIM_RANGE"] != DBNull.Value ? Convert.ToString(reader["IP_FIM_RANGE"]) : "-",
                                Ip_Mascara = reader["MASCARA_IP_CIDR"] != DBNull.Value ? Convert.ToString(reader["MASCARA_IP_CIDR"]) : "-",                             
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                var coluna = request.columns[sortColumnIndex].data;
                string sortDirection = request.order[0].dir;
                switch (coluna)
                {
                    case "id":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case "ip_Inicio_Range":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Ip_Inicio_Range);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Ip_Inicio_Range);
                            break;
                }
                    case "ip_Fim_Range":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Ip_Fim_Range);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Ip_Fim_Range);
                            break;
            }
                    case "ip_Mascara":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Ip_Mascara);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Ip_Mascara);
                            break;
                        }
                    case "descricao":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    case "status":
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
            return new DataTableResponse<Pool_IpListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Pool_IpListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Pool_IpListaModel>> retorno = new GetApiRetorno<IEnumerable<Pool_IpListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Pool_Ip>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Pool_IpListaModel
                    {
                        Id = p.Id,
                        
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

        private Pool_IpCadastroModel GetViewObj(Pool_Ip obj)
        {            
            return new Pool_IpCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Nome_Pool_Concentradora = obj.Nome_Pool_Concentradora,
                Ip_Inicio_Range = obj.Ip_Inicio_Range,
                Ip_Fim_Range = obj.Ip_Fim_Range,
                IsPoolBloqueio = obj.IsPoolBloqueio,
                IsPoolConcentradora = obj.IsPoolConcentradora,
                IsPoolIpFixo = obj.IsPoolIpFixo,
                IsPoolReducaoVelocidade = obj.IsPoolReducaoVelocidade,
                IsDistribuicaoBlocoIp = obj.IsDistribuicaoBlocoIp,
                CIDR = obj.Mascara_CIDR,
                PadraoIp = obj.Padrao_Ip,
                Status = obj.Status == Status_Default.Ativo,
                IdRadius = obj.Radius.Id,
                IdConcentradora = obj?.Concentradora?.Id,
                PoolPadraoConcentradora = obj.PoolPadraoConcentradora
            };
        }

        private void BindViewToObj(ISession session, ref Pool_Ip objBanco, Pool_IpCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            objBanco.Nome_Pool_Concentradora = view.Nome_Pool_Concentradora;
            
            objBanco.Ip_Inicio_Range = view.Ip_Inicio_Range;
            objBanco.Ip_Fim_Range = view.Ip_Fim_Range;

            objBanco.IsPoolBloqueio = view.IsPoolBloqueio;
            objBanco.IsPoolConcentradora = view.IsPoolConcentradora;
            objBanco.IsPoolIpFixo = view.IsPoolIpFixo;
            objBanco.IsPoolReducaoVelocidade = view.IsPoolReducaoVelocidade;
            objBanco.IsDistribuicaoBlocoIp = view.IsDistribuicaoBlocoIp;
            objBanco.Mascara_CIDR = view.CIDR;
            objBanco.Padrao_Ip = view.PadraoIp;
            objBanco.PoolPadraoConcentradora = view.PoolPadraoConcentradora;


            /*List<Pool_Ip_Endereco> listaIps = new List<Pool_Ip_Endereco>();
            foreach(var ip in objBanco.RangeIp)
            {
                listaIps.Add(new Pool_Ip_Endereco { 
                    Ip = ip.ToString(),
                    Status_Ip = Pool_Ip_Range_StatusEnum.Disponivel,
                    Pool_Ip = objBanco
                });
            }
            objBanco.Pool_Ip_Enderecos = listaIps;*/

            using (var repositoryRadius = new Conexao<Sinapse.Model.Radius>(session))
            {
                objBanco.Radius = repositoryRadius.selectFirst(p => p.Id == view.IdRadius).Result;
            }

            if(/*view.IsPoolConcentradora*/ view.IdConcentradora.HasValue)
            {
                using(var repositoryConcentradora = new Conexao<Sinapse.Model.Concentradora>(session))
                {
                    objBanco.Concentradora = repositoryConcentradora.selectFirst(p => p.Id == view.IdConcentradora).Result;
                }
            }
        }

        [HttpPost()]
        [Route("api/Pool_Ipapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Mensagens = new List<PostApiMensagem>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    var listaIdsAtualizar = new List<long>();
                    foreach(var id in value.ids)
                    {
                        //Se é uma inativação, checa se as Pools possuem IPs em uso
                        if (value.status == 0)
                        {
                            using (var repoPoolIpEndereco = new Conexao<Pool_Ip_Endereco>(session))
                            {
                                var ipEmUso = await repoPoolIpEndereco.selectFirst(p => p.Pool_Ip.Id == id && p.Status_Ip != Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel);
                                if (ipEmUso != null)
                                    retorno.Mensagens.Add(new PostApiMensagem { Mensagem = $"{ipEmUso.Pool_Ip?.Descricao} possui IPs em uso", Tipo = PostApiMensagemType.Erro });
                                else
                                    listaIdsAtualizar.Add(id);
                            }
                        }
                        else
                        {
                            listaIdsAtualizar.Add(id);
                        }
                    }

                    if(listaIdsAtualizar.Count > 0)
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        using (var com = session.Connection.CreateCommand())
                        {
                            String _ids = $"({String.Join(",", listaIdsAtualizar)})";

                            StringBuilder sql = new StringBuilder($"UPDATE TB_Pool_Ip SET STATUS = {value.status} WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                            retorno.Ok = true;
                        }
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
        [Route("api/Pool_Ipapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_Pool_Ip WHERE ID IN {_ids}");

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
        [Route("api/Pool_IpApi/FindById")]
        public async Task<GetApiRetorno<Pool_IpCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Pool_IpCadastroModel> retorno = new GetApiRetorno<Pool_IpCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Pool_Ip>(session))
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


        [HttpPost()]
        [Route("api/Pool_IpApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Pool_IpCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Pool_Ip modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Pool_Ip>(session))
                    {
                        var poolIPFixo = value.Objeto.IsPoolIpFixo;

                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Pool_Ip
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {

                            if(modelo.IsPoolConcentradora != value.Objeto.IsPoolConcentradora)
                            {
                                throw new Exception("Não é possível alterar controle da pool após pool já ter sido cadastrada.");
                            }

                            if(modelo.IsPoolIpFixo != value.Objeto.IsPoolIpFixo)
                            {
                                throw new Exception("Não é possível alterar o tipo da pool após a pool já ter sido cadastrada.");
                            }


                            if (modelo.Status == Status_Default.Ativo && value.Objeto.Status == false)
                            {
                                using(var repoPoolIpEndereco = new Conexao<Pool_Ip_Endereco>(session))
                                {
                                    var ipEmUso = await repoPoolIpEndereco.selectFirst(p => p.Pool_Ip.Id == modelo.Id && p.Status_Ip != Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel);
                                    if(ipEmUso != null)
                                        throw new Exception("Não é possível inativar esta Pool pois já existem IPs atribuídos a logins");
                                }
                            }

                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        //Validação do range de ip sem máscara
                        if(!String.IsNullOrEmpty(value.Objeto.Ip_Inicio_Range) && !String.IsNullOrEmpty(value.Objeto.Ip_Fim_Range)
                            && String.IsNullOrEmpty(value.Objeto.CIDR))
                        {
                            IPAddress ip_inicio = null;
                            IPAddress.TryParse(value.Objeto.Ip_Inicio_Range, out ip_inicio);

                            if(ip_inicio == null)
                                throw new Exception("Ip Inicial não é válido");

                            IPAddress ip_fim = null;
                            IPAddress.TryParse(value.Objeto.Ip_Fim_Range, out ip_fim);

                            if (ip_fim == null)
                                throw new Exception("Ip Final não é válido");

                            var result = ((IStructuralComparable)ip_inicio.GetAddressBytes()).CompareTo(ip_fim.GetAddressBytes(), Comparer<byte>.Default);

                            if (result >= 0)
                            {
                                throw new Exception("Range de Ip inválido: Ip inicial não pode ser maior que Ip final");
                            }

                            //Validação de redução no range de IP pelo IP inicial
                            if (!String.IsNullOrEmpty(modelo.Ip_Inicio_Range))
                            {
                                IPAddress ip_inicio_antigo = IPAddress.Parse(modelo.Ip_Inicio_Range);

                                var resultIpsIniciais = ((IStructuralComparable)ip_inicio.GetAddressBytes()).CompareTo(ip_inicio_antigo.GetAddressBytes(), Comparer<byte>.Default);

                                if (resultIpsIniciais > 0)
                                {
                                    throw new Exception("Não é possível reduzir um range de IP já cadastrado em sistema");
                                }
                            }

                            //Validação de redução no range de IP pelo IP final
                            if (!String.IsNullOrEmpty(modelo.Ip_Fim_Range))
                            {
                                IPAddress ip_fim_antigo = IPAddress.Parse(modelo.Ip_Fim_Range);

                                var resultIpsFinais = ((IStructuralComparable)ip_fim_antigo.GetAddressBytes()).CompareTo(ip_fim.GetAddressBytes(), Comparer<byte>.Default);

                                if (resultIpsFinais > 0)
                                {
                                    throw new Exception("Não é possível reduzir um range de IP já cadastrado em sistema");
                                }
                            }



                            // Validações pool IP fixo
                            if (poolIPFixo == true)
                            {
                                                                                                   
                                   if(!String.IsNullOrEmpty(modelo.Ip_Inicio_Range) && !String.IsNullOrEmpty(modelo.Ip_Fim_Range))
                                   {   
                                       //Separa os campos do IP
                                       var parteIPInicioAtual = value.Objeto.Ip_Inicio_Range.Split('.');
                                       var parteIPFinalAtual = value.Objeto.Ip_Fim_Range.Split('.');
                                       var parteIPInicioAnterior = modelo.Ip_Inicio_Range.Split('.');
                                       var parteIPFinalAnterior = modelo.Ip_Fim_Range.Split('.');
                                       
                                       //Junta os campos do IP referentes ao bloco, em uma string
                                       var blocoIPInicioAtual = string.Join(".", parteIPInicioAtual[0], parteIPInicioAtual[1], parteIPInicioAtual[2]);
                                       var blocoIPFinalAtual = string.Join(".", parteIPFinalAtual[0], parteIPFinalAtual[1], parteIPFinalAtual[2]);
                                       var blocoIPInicioAnterior = string.Join(".", parteIPInicioAnterior[0], parteIPInicioAnterior[1], parteIPInicioAnterior[2]);
                                       var blocoIPFinalAnterior = string.Join(".", parteIPFinalAnterior[0], parteIPFinalAnterior[1], parteIPFinalAnterior[2]);

                                       //Compara os campos para validar se a range mudou
                                       if(blocoIPInicioAtual != blocoIPInicioAnterior)
                                       {
                                          throw new Exception("Não é possível mudar o bloco de IP fixo");
                                       }
                                       else if(blocoIPFinalAtual != blocoIPFinalAnterior)
                                       {
                                          throw new Exception("Não é possível mudar o bloco de IP fixo");
                                       }

                                   }                                                                                                    
                            }

                        }

                        //Validação do range de ip com máscara
                        if (!String.IsNullOrEmpty(modelo.Mascara_CIDR) && !String.IsNullOrEmpty(value.Objeto.CIDR))
                        {
                            var mascaraAnterior = Convert.ToInt32(modelo.Mascara_CIDR);
                            var mascaraAtual = Convert.ToInt32(value.Objeto.CIDR);
                            if (mascaraAtual > mascaraAnterior)
                            {
                                throw new Exception("Não é possível reduzir um range de IP já cadastrado em sistema");
                            }
                        }

                        //Validação para inserir apenas IP final da range ou a máscara de IP
                        if (!String.IsNullOrEmpty(value.Objeto.Ip_Fim_Range)
                             && !String.IsNullOrEmpty(value.Objeto.CIDR))
                        {
                            throw new Exception("Por favor, insira apenas o IP final da range ou a máscara de IP.");
                        }

                        var Novo_PoolName = String.Empty;
                        if (!String.IsNullOrEmpty(modelo.Descricao) && !modelo.Descricao.Equals(value.Objeto.Descricao))
                            Novo_PoolName = value.Objeto.Descricao;

                       

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);                        

                        //Cadastrar Pool no Radius e Range de IPs no banco do Connect                       
                        var novoRegistro = value.Objeto.Id == 0;

                        if(!modelo.IsPoolConcentradora)
                        {
                            if (novoRegistro)
                            {                                
                                await new Pool_Ip_EnderecoApiController().GravarRangeIps(modelo.RangeIp, modelo.Id, isEdicao: false, poolIPFixo);
                                new IntegracaoRadiusApiController().CadastrarPoolIp(modelo);
                            }
                            else
                            {
                               
                                await new Pool_Ip_EnderecoApiController().GravarRangeIps(modelo.RangeIp, modelo.Id, isEdicao: true, poolIPFixo);
                                new IntegracaoRadiusApiController().EditarPoolIp(modelo, Novo_PoolName);
                            }
                        }
                        else 
                        {
                            //Quando Pool for controlada na Concentradora só precisa cadastrar na base do Connect para referência.
                            //No Radius não é necessário
                            if (novoRegistro)
                                await new Pool_Ip_EnderecoApiController().GravarRangeIps(modelo.RangeIp, modelo.Id, isEdicao: false, poolIPFixo);
                            else
                                await new Pool_Ip_EnderecoApiController().GravarRangeIps(modelo.RangeIp, modelo.Id, isEdicao: true, poolIPFixo);
                        }
                        
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

        [Route("api/Pool_IpApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Pool_Ip>(session))
                    {

                        Pool_Ip modelo = null;
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
        [Route("api/pool_Ipapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?,Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_POOL_IP A ");
                    sqlFrom.AppendLine("INNER JOIN TB_CONCENTRADORA B ");
                    sqlFrom.AppendLine("ON (A.ID_CONCENTRADORA = B.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if(request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CONCENTRADORA = {request.idpai}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.DESCRICAO != '' AND ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"])
                                    });

                                }
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

        [HttpPost()]
        [Route("api/pool_Ipapi/GetComboDisponiveisOld")]
        [Obsolete("Depreciado, utilizar o novo GetComboDisponiveis")]
        public async Task<ObjCombo<Int64>> GetComboDisponiveisOld(RequestComboIpPoolDisponiveis<Int64?,Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM (SELECT A.ID, A.DESCRICAO, 1 AS IS_POOL_RADIUS, A.POOL_IP_FIXO FROM TB_POOL_IP A INNER JOIN TB_POOL_IP_ENDERECO B ON(A.ID = B.ID_POOL_IP) ");
                    sqlFrom.AppendLine("INNER JOIN TB_CONCENTRADORA C ");
                    sqlFrom.AppendLine("ON (A.ID_CONCENTRADORA = C.ID) ");

                    List<String> condicoes = new List<String>();                    
                    List<String> condicoesPoolRadius = new List<String>();                    
                    List<String> condicoesPoolConcentradora = new List<String>();                    

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CONCENTRADORA = {request.idpai}");
                    }

                    //Filtrar somente Pools que não são destinadas a Bloqueio de Sinal ou Redução de Banda e que são administradas pelo Radius
                    condicoesPoolRadius.Add($"A.POOL_BLOQUEIO_SINAL = 0");
                    condicoesPoolRadius.Add($"A.POOL_REDUCAO_VELOCIDADE = 0");
                    condicoesPoolRadius.Add($"A.CONTROLE_POOL_CONCENTRADORA = 0");

                    //Filtrar Pools que possuem IPs disponíveis
                    condicoes.Add($"B.STATUS_IP = 1");

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0 || condicoesPoolRadius.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.DESCRICAO != '' ");
                        if(condicoes.Count() > 0)
                            sqlFrom.AppendLine($" AND ( {String.Join(" AND ", condicoes) } ) ");

                        if(condicoesPoolRadius.Count() > 0)
                            sqlFrom.AppendLine($" OR ( {String.Join(" AND ", condicoesPoolRadius) } ) ");
                    }

                    sqlFrom.AppendLine($"UNION ");
                    sqlFrom.AppendLine($"SELECT A.ID, A.DESCRICAO, 0 AS IS_POOL_RADIUS, A.POOL_IP_FIXO ");
                    sqlFrom.AppendLine($"FROM TB_POOL_IP A ");                    

                    condicoes.Remove($"A.CONTROLE_POOL_CONCENTRADORA = 0");
                    condicoes.Remove($"B.STATUS_IP = 1");
                    condicoesPoolConcentradora.Add($"A.CONTROLE_POOL_CONCENTRADORA = 1");

                    if (condicoes.Count() > 0 || condicoesPoolConcentradora.Count() > 0)
                    {
                        condicoesPoolConcentradora.AddRange(condicoes);
                        sqlFrom.AppendLine($" WHERE A.DESCRICAO != '' AND ( {String.Join(" AND ", condicoesPoolConcentradora) } ) ");
                    }                    

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT ID, DESCRICAO, IS_POOL_RADIUS, POOL_IP_FIXO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, A.POOL_IP_FIXO) ");
                        sql.AppendLine(" ORDER BY UPPER(DESCRICAO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjComboIpPool<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]),
                                        Is_Pool_Radius = Convert.ToBoolean(reader["IS_POOL_RADIUS"]),
                                        IsPoolIpFixo = Convert.ToInt16(reader["POOL_IP_FIXO"]) == 1
                                    });

                                }
                            }
                            retorno.total_count = retorno.results.Count;
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
        [Route("api/pool_Ipapi/GetComboDisponiveis")]
        public async Task<ObjCombo<Int64>> GetComboDisponiveis(RequestComboIpPoolDisponiveis<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_POOL_IP A  ");
                    //sqlFrom.AppendLine("LEFT JOIN TB_POOL_IP_ENDERECO B  ");
                    //sqlFrom.AppendLine("ON(A.ID = B.ID_POOL_IP)  ");
                    sqlFrom.AppendLine("LEFT JOIN TB_CONCENTRADORA C ");
                    sqlFrom.AppendLine("ON (A.ID_CONCENTRADORA = C.ID) ");
                    //sqlFrom.AppendLine("WHERE ");                                       

                    List<String> condicoes = new List<String>();
                    List<String> condicoesPoolRadius = new List<String>();
                    List<String> condicoesPoolConcentradora = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }                    

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    condicoes.Add(" A.DESCRICAO != '' ");

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    if (request.soativos)
                    {                        
                        //Pools controladas pelo Radius com IP disponível que não são de bloqueio de sinal e redução de velocidade
                        condicoesPoolRadius.Add(" A.CONTROLE_POOL_CONCENTRADORA = 0 ");
                        condicoesPoolRadius.Add(" A.POOL_BLOQUEIO_SINAL = 0 ");
                        condicoesPoolRadius.Add(" A.POOL_REDUCAO_VELOCIDADE = 0 ");
                        condicoesPoolRadius.Add(" (SELECT COUNT(B.ID) FROM TB_POOL_IP_ENDERECO B WHERE A.ID = B.ID_POOL_IP AND B.STATUS_IP = 1) > 0 ");

                        //Pools controladas por Concentradora que não são de bloqueio de sinal e redução de velocidade
                        condicoesPoolConcentradora.Add(" A.POOL_BLOQUEIO_SINAL = 0 ");
                        condicoesPoolConcentradora.Add(" A.POOL_REDUCAO_VELOCIDADE = 0 ");
                        condicoesPoolConcentradora.Add(" A.CONTROLE_POOL_CONCENTRADORA = 1 ");

                        if (request.idpai.HasValue)
                        {
                            condicoesPoolConcentradora.Add($"A.ID_CONCENTRADORA = {request.idpai}");
                        }
                       
                        //sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoesPoolRadius) } ) ");

                        if (condicoes.Count() > 0)
                        {
                            sqlFrom.AppendLine($" AND (( {String.Join(" AND ", condicoesPoolRadius) } ) ");
                            sqlFrom.AppendLine($" OR ( {String.Join(" AND ", condicoesPoolConcentradora) } )) ");
                        }                        
                    }

                   

                    /*if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoesPoolRadius.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                        condicoesPoolConcentradora.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }   */

                    

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(*) AS CO FROM (");
                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.CONTROLE_POOL_CONCENTRADORA, A.POOL_IP_FIXO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, A.CONTROLE_POOL_CONCENTRADORA, A.POOL_IP_FIXO ");
                        sql.AppendLine(" ORDER BY UPPER(DESCRICAO)) ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.CONTROLE_POOL_CONCENTRADORA, A.POOL_IP_FIXO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, A.CONTROLE_POOL_CONCENTRADORA, A.POOL_IP_FIXO ");
                        sql.AppendLine(" ORDER BY UPPER(DESCRICAO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
                                {
                                    retorno.results.Add(new ObjComboIpPool<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["DESCRICAO"]),
                                        Is_Pool_Radius = Convert.ToBoolean(reader["CONTROLE_POOL_CONCENTRADORA"]) == false,
                                        IsPoolIpFixo = Convert.ToInt16(reader["POOL_IP_FIXO"]) == 1
                                    });

                                }
                            }
                            
                            //retorno.total_count = retorno.results.Count;
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

        public async Task<Int64?> GetPoolIpMenosUsuarios(long? idConcentradora)
        {
            Int64? retorno = null;

            using (ISession session = HibernateUtil.GetSession())
            {
                try 
                {
                    using (var com = session.Connection.CreateCommand())
                    {                        
                        StringBuilder sql = new StringBuilder("SELECT FIRST 1 A.ID, COUNT(B.ID) AS IPS_DISPONIVEIS FROM TB_POOL_IP A ");
                        sql.AppendLine("INNER JOIN TB_POOL_IP_ENDERECO B ON(A.ID = B.ID_POOL_IP) ");
                        sql.AppendLine("LEFT JOIN TB_CONCENTRADORA C ON(A.ID_CONCENTRADORA = C.ID) ");
                        sql.AppendLine("WHERE B.STATUS_IP = 1 ");   
                        
                        if(idConcentradora.HasValue)
                        {
                            sql.AppendLine($"OR (A.ID_CONCENTRADORA = {idConcentradora} AND B.STATUS_IP = 1) ");
                            sql.AppendLine("GROUP BY A.ID, A.ID_CONCENTRADORA ");
                            sql.AppendLine("ORDER BY A.ID_CONCENTRADORA, IPS_DISPONIVEIS DESC ");
                        }
                        else
                        {
                            sql.AppendLine("GROUP BY A.ID ");
                            sql.AppendLine("ORDER BY IPS_DISPONIVEIS DESC ");
                        }                                                 

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno = Convert.ToInt64(reader["ID"]);
                        }
                    }
                }
                catch(Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno = null;
                }
            }
            return retorno;
        }

        public async Task<Int64?> GetPoolIpPadraoConcentradora(long? idConcentradora)
        {
            Int64? retorno = null;

            if (!idConcentradora.HasValue)
                return null;

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT FIRST 1 A.ID, COUNT(B.ID) AS IPS_DISPONIVEIS FROM TB_POOL_IP A ");
                        sql.AppendLine("INNER JOIN TB_POOL_IP_ENDERECO B ON(A.ID = B.ID_POOL_IP) ");
                        sql.AppendLine("LEFT JOIN TB_CONCENTRADORA C ON(A.ID_CONCENTRADORA = C.ID) ");
                        sql.AppendLine("WHERE B.STATUS_IP = 1 ");                        
                        sql.AppendLine($"AND A.ID_CONCENTRADORA = {idConcentradora} AND A.POOL_PADRAO_CONCENTRADORA = 1 ");

                        sql.AppendLine("GROUP BY A.ID ");
                        sql.AppendLine("ORDER BY IPS_DISPONIVEIS DESC ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno = Convert.ToInt64(reader["ID"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno = null;
                }
            }
            return retorno;
        }
    }
}
