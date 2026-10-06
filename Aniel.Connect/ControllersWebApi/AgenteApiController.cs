using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AgenteApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/agenteapi/getAgente")]
        public async Task<AgenteModel> getAgente(String Usuario)
        {
            AgenteModel retorno = new AgenteModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAgente = new Conexao<Perfil_Agente>(session))
            using (var repoUsuario = new Conexao<IdentityUser>(session))
            {
                var usuario = await repoUsuario.selectFirst(p => p.Id == Usuario);

                if (usuario != null)
                {
                    var agente = await repoAgente.selectFirst(p => p.Id == usuario.Id_Perfil_Agente);
                    retorno.Categorias = agente == null ? new List<long?>() : agente.Categorias.Select(p => p.Categoria_Cliente?.Id).ToList();
                    retorno.Localidades = agente == null ? new List<Escritorio_Area>() : agente.Localidades.Select(p => new Escritorio_Area { Cod_Area = p.Cod_Area, Cod_Escritorio = p.Cod_Escritorio }).ToList();
                    retorno.QtdeMaximaAtendimentos = agente == null ? 0 : agente.Numero_Max_Atendi_Simultaneo_Por_Agente.GetValueOrDefault(0);
                    retorno.AtendeChat = agente?.Perfil_Atende_Chat ?? false;
                    retorno.InicioAtendimento = agente?.Inicio_Atendimento;
                    retorno.TerminoAtendimento = agente?.Termino_Atendimento;
                    retorno.NomeAgente = usuario?.UserName;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/agenteapi/getDadosAgenteChat")]
        public async Task<AgenteChat> getDadosAgente(String Usuario)
        {
            AgenteChat retorno = new AgenteChat();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoAgente = new Conexao<Perfil_Agente>(session))
            using (var repoUsuario = new Conexao<IdentityUser>(session))
            {
                var usuario = await repoUsuario.selectFirst(p => p.Id == Usuario);
                var agente = await repoAgente.selectFirst(p => p.Id == usuario.Id_Perfil_Agente);
                retorno.AtendeChat = agente?.Perfil_Atende_Chat ?? false;
                retorno.Departamento = usuario.Departamento.Cod_Depto;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/agenteapi/getFotoAgente")]
        public async Task<String> getFotoAgente(String Usuario)
        {
            String retorno = String.Empty;
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder("SELECT B.NOME, B.FOTO FROM TB_SENHA A INNER JOIN TB_FUNCIONARIO B ON(A.NUM_FRE = B.NUM_FRE) WHERE A.USUARIO = @USUARIO");
                parametros.Add("@USUARIO", Usuario);

                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, 0))
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader["FOTO"] != DBNull.Value)
                        {

                            byte[] myByteArray = (byte[])reader["FOTO"];
                            MemoryStream stream = new MemoryStream();
                            stream.Write(myByteArray, 0, myByteArray.Length);

                            var image = System.Drawing.Image.FromStream(stream);
                            var bitmap = new Bitmap(image, new Size(82, 82));
                            stream = new MemoryStream();
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                            retorno = Convert.ToBase64String(stream.GetBuffer());
                        }
                        else
                        {
                            retorno = String.Empty;
                        }

                    }
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/agenteapi/getAgenteCliente")]
        public async Task<Bot> getBotAgenteCliente(ClienteChat_Mobile cliente, ISession session)
        {
            Bot retorno = null;

            using (var repoPerfilAgente = new Conexao<Perfil_Agente>(session))
            {
                try
                {

                    var ativos = await repoPerfilAgente.select(p => p.Status == Status_Default.Ativo);
                    var botValido = new Bot();
                    List<(string descricao, int soma, long id)> lista = new List<(string descricao, int soma, long id)>();



                    foreach (var item in ativos)
                    {
                        int categoria = 0;
                        int tipoAtendimento = 0;
                        int localidade = 0;
                        int soma = 0;

                        if (item.Categorias != null && item.Categorias.Count > 0)
                        {

                            if (cliente.idCategoriaCliente.HasValue && item.Categorias.Select(p => p.Categoria_Cliente.Id).ToList().Contains(cliente.idCategoriaCliente.Value) == true)
                            {
                                categoria++;
                            }
                            else
                                continue;
                        }

                        if (item.Localidades.Count > 0)
                        {
                            if (item.Localidades.Select(p => (p.Cod_Escritorio, p.Cod_Area)).ToList().Contains((cliente.Escritorio_Area.Cod_Escritorio, cliente.Escritorio_Area.Cod_Area)) == true)
                            {
                                localidade++;
                            }
                            else
                                continue;
                        }

                        if (item.Tipo_atendimento.Count > 0)
                        {
                            if (item.Tipo_atendimento.Select(p => p.Tipo_atendimento.Id).ToList().Contains(cliente.idTipoAtendimento) == true)
                            {
                                tipoAtendimento++;
                            }
                            else
                                continue;
                        }

                        soma = (categoria + tipoAtendimento + localidade);
                        lista.Add((descricao: item.Descricao, soma: soma, id: item.Id));
                    }

                    List<Bot> bot = new List<Bot>();

                    if (lista.Count > 0)
                    {
                        int maiorSoma = lista.Max(p => p.soma);
                        int contador = 0;
                        foreach (var itemLista in lista)
                        {
                            if (itemLista.soma == maiorSoma)
                            {
                                Bot botselec = (await repoPerfilAgente.selectFirst(p => p.Id == itemLista.id)).Bot;
                                bot.Add(botselec);
                                contador++;
                            }
                        }
                    }

                    retorno = bot.First();




                    //List<Perfil_Agente> perfisExistentes = (await repoPerfilAgente.select(p => p.Status == Status_Default.Ativo)).ToList();
                    //List<Perfil_Agente> perfisPossiveis = new List<Perfil_Agente>();

                    //foreach (var item in perfisExistentes)
                    //{
                    //    bool possivel = true;
                    //    if (item.Categorias.Count > 0) {
                    //        if (cliente.idCategoriaCliente.HasValue && item.Categorias.Select(p => p.Categoria_Cliente.Id).ToList().Contains(cliente.idCategoriaCliente.Value) == false)
                    //            possivel = false;
                    //    }

                    //    if (item.Localidades.Count > 0)
                    //    {
                    //        if (item.Localidades.Select(p => (p.Cod_Escritorio,p.Cod_Area)).ToList().Contains((cliente.Escritorio_Area.Cod_Escritorio, cliente.Escritorio_Area.Cod_Area)) == false)
                    //            possivel = false;
                    //    }

                    //    if (item.Tipo_atendimento.Count > 0)
                    //    {
                    //        if (item.Tipo_atendimento.Select(p => p.Tipo_atendimento.Id).ToList().Contains(cliente.idTipoAtendimento) == false)
                    //            possivel = false;
                    //    }

                    //    if (possivel)
                    //        perfisPossiveis.Add(item);

                    //}

                    //retorno = perfisPossiveis.FirstOrDefault()?.Bot;


                    //Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    //using (var com = session.Connection.CreateCommand())
                    //{
                    //    StringBuilder sql = new StringBuilder();

                    //    sql.AppendLine("SELECT A.ID, A.ID_BOT, A.STATUS, A.DESCRICAO FROM TB_AGENTE A ");
                    //    sql.AppendLine("INNER JOIN TB_AGENTE_CATEGORIA B ON A.ID = B.ID_AGENTE ");
                    //    sql.AppendLine("INNER JOIN TB_CLIENTE C ON B.ID_CATEGORIA_CLIENTE = C.ID_CATEGORIA ");
                    //    sql.AppendLine("WHERE C.ID = @ID_CLIENTE AND A.STATUS = 1 ");
                    //   parametros.Add("@ID_CLIENTE", IdCliente);

                    //    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    //    {
                    //        while (await reader.ReadAsync())
                    //        {
                    //            if (!String.IsNullOrEmpty(Convert.ToString(reader["ID"])))
                    //            {
                    //                retorno.Id = Convert.ToInt64(reader["ID"]);
                    //                retorno.IdBot = Convert.ToInt64(reader["ID_BOT"]);
                    //            }
                    //            else
                    //            {
                    //                sql.AppendLine("SELECT D.ID, D.ID_BOT, D.STATUS, D.DESCRICAO FROM TB_AGENTE D ");
                    //                sql.AppendLine("INNER JOIN TB_AGENTE_LOCALIDADE E ON D.ID = E.ID_AGENTE ");
                    //                sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO F ON F.COD_AREA = E.COD_AREA ");
                    //                sql.AppendLine("INNER JOIN TB_CLIENTE G ON G.ID = F.ID_CLIENTE ");
                    //                sql.AppendLine("WHERE F.ID = @ID_CLIENTE AND D.STATUS = 1 ");
                    //                parametros.Add("@ID_CLIENTE", IdCliente);

                    //                using (var readerS = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    //                {
                    //                    while (await readerS.ReadAsync())
                    //                    {
                    //                        retorno.Id = Convert.ToInt64(readerS["ID"]);
                    //                        retorno.IdBot = Convert.ToInt64(readerS["ID_BOT"]);
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/agenteapi/getAll")]
        public async Task<DataTableResponse<AgenteListaModel>> getAll(Agente_DataTableRequest request)
        {
            List<AgenteListaModel> retorno = new List<AgenteListaModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder("SELECT A.ID,A.STATUS,A.DESCRICAO FROM TB_AGENTE A ");

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE @SEARCH))");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new AgenteListaModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Nome = Convert.ToString(reader["DESCRICAO"]),
                            Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                        });
                    }

                }
            }

            return new DataTableResponse<AgenteListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno.OrderBy(p => p.Nome),
                error = ""
            };
        }

        [HttpGet()]
        [Route("api/agenteapi/FindById")]
        public async Task<GetApiRetorno<AgenteCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<AgenteCadastroModel> retorno = new GetApiRetorno<AgenteCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            using (var repositoryUsuario = new Conexao<IdentityUser>(session))

            {
                try
                {
                    var modelo = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(modelo, sessionT5);
                    retorno.Objeto.Usuarios = repositoryUsuario.select(p => p.Id_Perfil_Agente == modelo.Id).Result.Select(p => new AgenteUsuarioModel { Id = p.Id, Usuario = p.UserName, Usuario_Id = p.Id }).ToList();

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

        private AgenteCadastroModel GetViewObj(Perfil_Agente obj, ISession sessionT5)
        {

            using (var area_api = new AreaApiController())
            using (var grupoarea_api = new Grupo_AreaAnielApiController())
            using (var tipo_atendimento_api = new AgenteTipoAtendimentoApiController())
            {
                return new AgenteCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,

                    IdBot = obj.Bot?.Id,

                    Perfil_Atende_Chat = obj.Perfil_Atende_Chat,
                    Cliente_Aguardando_Atendimento = obj.Cliente_Aguardando_Atendimento,
                    Inicio_Atendimento = obj.Inicio_Atendimento,
                    Termino_Atendimento = obj.Termino_Atendimento,
                    Todos_Agentes_Offline = obj.Todos_Agentes_Offline,
                    Ativar_Inatividade = obj.Ativar_Inatividade,
                    Mensagem_Notif_Conclusao_Inatividade = obj.Mensagem_Notif_Conclusao_Inatividade,
                    Mensagem_Enviada_Concluir_Inatividade = obj.Mensagem_Enviada_Concluir_Inatividade,
                    Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos = obj.Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos,
                    Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade = obj.Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade,
                    Numero_Max_Atendi_Simultaneo_Por_Agente = obj.Numero_Max_Atendi_Simultaneo_Por_Agente,

                    Status = obj.Status == Status_Default.Ativo ? true : false,
                    Categorias = obj.Categorias.Select(p => new AgenteCategoriaModel
                    {
                        Categoria = p.Categoria_Cliente.Descricao,
                        Id_Categoria = p.Categoria_Cliente.Id,
                        Id = p.Id
                    }).ToList(),
                    Localidades = obj.Localidades.Select(p => new AgenteLocalidadeModel
                    {
                        Id = p.Id,
                        Cod_Area = p.Cod_Area,
                        Cod_Escritorio = p.Cod_Escritorio,
                        Desc_Area = !p.Todas_Areas ? area_api.GetList(p.Cod_Escritorio).Result.Objeto.Find(y => y.Value == p.Cod_Area).Label : string.Empty,
                        Desc_Escritorio = grupoarea_api.GetList().Result.Objeto.Find(y => y.Value == p.Cod_Escritorio).Label,

                        Cod_Localidade = p.Cod_Localidade,
                        Desc_Localidade = p.Desc_Localidade,
                        Todas_Localidades = p.Todas_Localidades,
                        Todas_Areas = p.Todas_Areas

                    }).ToList(),
                    tipo_atendimento = tipo_atendimento_api.GetViewObj(obj.Tipo_atendimento.ToList())
                    //tipo_atendimento = obj.Tipo_atendimento.Select(p => new AgenteTipo_atendimentoModel 
                    //{ 
                    //    Id = p.Id,
                    //    Id_Tipo_atendimento = p.Tipo_atendimento.Id,
                    //    Tipo_atendimento = p.Tipo_atendimento.Nome
                    //})
                };
            }
        }

        [Route("api/agenteapi/ExcluirGridCategoria")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridCategoria(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Agente_Categoria>(session))
                    {

                        Agente_Categoria modelo = null;
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

        [Route("api/agenteapi/ExcluirGridTipo_atendimento")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridTipo_atendimento(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Agente_Tipo_atendimento>(session))
                    {

                        Agente_Tipo_atendimento modelo = null;
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

        [Route("api/agenteapi/ExcluirGridLocalidade")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridLocalidade(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Agente_Localidade>(session))
                    {

                        Agente_Localidade modelo = null;
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

        [Route("api/agenteapi/ExcluirGridUsuario")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<String>> ExcluirGridUsuario(String value)
        {
            DeleteApiRetorno<String> retorno = new DeleteApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<IdentityUser>(session))
                    {

                        IdentityUser modelo = null;
                        if (!String.IsNullOrEmpty(value))
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            modelo.Id_Perfil_Agente = null;
                            await repository.SaveOrUpdate(modelo);
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
                retorno.IdObjeto = String.Empty;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/agenteapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<AgenteCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Perfil_Agente modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Perfil_Agente>(session))
                    using (var repoUsuario = new Conexao<IdentityUser>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Perfil_Agente
                            {
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        if (value.Objeto.IdBot != null)
                        {
                            using var botApi = new Customizacao_BotApiController();
                            var botModel = await botApi.FindById(value.Objeto.IdBot.Value);
                            var bot = new Bot()
                            {
                                Id = botModel.Objeto.Id
                            };
                            value.Objeto.Bot = bot;
                        }



                        BindViewToObj(session, modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);

                        var listIdUsuario = value.Objeto.Usuarios.Where(y => String.IsNullOrEmpty(y.Id)).Select(y => y.Usuario_Id).ToList();
                        var usuarios = (await repoUsuario.select(p => p.Id.IsIn(listIdUsuario))).ToList();

                        usuarios = usuarios.Select(p =>
                        {
                            p.Id_Perfil_Agente = modelo.Id;
                            return p;
                        }).ToList();

                        await repoUsuario.SaveOrUpdate(usuarios);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
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

        private void BindViewToObj(ISession session, Perfil_Agente objBanco, AgenteCadastroModel view)
        {
            using (var repoTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repoCategoria = new Conexao<Categoria_Cliente>(session))
            {
                objBanco.Descricao = view.Descricao;

                objBanco.Perfil_Atende_Chat = view.Perfil_Atende_Chat;
                objBanco.Cliente_Aguardando_Atendimento = view.Cliente_Aguardando_Atendimento;
                objBanco.Inicio_Atendimento = view.Inicio_Atendimento;
                objBanco.Termino_Atendimento = view.Termino_Atendimento;
                objBanco.Todos_Agentes_Offline = view.Todos_Agentes_Offline;
                objBanco.Ativar_Inatividade = view.Ativar_Inatividade;
                objBanco.Mensagem_Notif_Conclusao_Inatividade = view.Mensagem_Notif_Conclusao_Inatividade;
                objBanco.Mensagem_Enviada_Concluir_Inatividade = view.Mensagem_Enviada_Concluir_Inatividade;
                objBanco.Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos = view.Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos;
                objBanco.Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade = view.Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade;
                objBanco.Numero_Max_Atendi_Simultaneo_Por_Agente = view.Numero_Max_Atendi_Simultaneo_Por_Agente;
                objBanco.Bot = view.Bot;

                objBanco.Categorias = view.Categorias.Where(p => p.Id == 0).Select(p => new Agente_Categoria
                {
                    Agente = objBanco,
                    Categoria_Cliente = repoCategoria.selectFirst(y => y.Id == p.Id_Categoria).Result
                }).ToList();

                objBanco.Localidades = view.Localidades.Where(p => p.Id == 0).Select(p => new Agente_Localidade
                {
                    Agente = objBanco,
                    Cod_Escritorio = p.Cod_Escritorio,
                    Cod_Area = p.Cod_Area,
                    Cod_Localidade = p.Cod_Localidade,
                    Desc_Localidade = p.Desc_Localidade,
                    Todas_Localidades = p.Todas_Localidades,
                    Todas_Areas = p.Todas_Areas
                }).ToList();

                objBanco.Tipo_atendimento = view.tipo_atendimento.Where(p => p.Id == 0).Select(p => new Agente_Tipo_atendimento
                {
                    Agente = objBanco,
                    Tipo_atendimento = repoTipoAtendimento.selectFirst(u => u.Id == p.Id_Tipo_atendimento).Result
                }).ToList();
            }
        }

    }
}