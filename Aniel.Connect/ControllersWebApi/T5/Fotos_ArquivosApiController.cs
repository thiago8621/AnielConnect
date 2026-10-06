using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using SautinSoft.Document;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Fotos_ArquivosApiController : BaseApiController
    {
        [HttpGet]
        [Route("api/Fotos_ArquivosApi/ExcluirArquivo")]
        public async Task<HttpResponseMessage> ExcluirArquivo(long id, string origem)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(id.ToString())
            };

            string tabela = origem.Trim().Equals("FOTO") ? "TB_IMAGEM_DOCPROD_MOBILE" : "TB_ARQUIVO_DOCPROD";

            using (ISession session = HibernateUtil.GetSessionFoto())
            using (var repoArquivo = new Conexao<Arquivo_DocProd>(session))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    
                    using (var comDelete = session.Connection.CreateCommand())
                    {
                        parametros.Add("@ID", id);

                        StringBuilder sqlDelete = new StringBuilder($"DELETE FROM {tabela} WHERE ID = @ID");
                        await CommandHelper.ExecuteNonQuery(comDelete, sqlDelete, parametros, session.Connection, null);
                    }

                    return result;
                    
                }
                catch (Exception e)
                {
                    result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.Message)
                    };

                    return result;
                }

            }
        }

        [HttpPost()]
        [Route("api/Fotos_ArquivosApi/Get_Fotos_Arquivos")]
        public async Task<DataTableResponse<Fotos_ArquivosListaModel>> Get_Fotos_Arquivos(Fotos_ArquivosDataTableRequest request)
        {
            List<Fotos_ArquivosListaModel> retorno = new List<Fotos_ArquivosListaModel>();
            IOrderedEnumerable<Fotos_ArquivosListaModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO AS NOME, A.TIPO, 'ARQUIVO' AS TABELA ");
                        sql.AppendLine(" FROM TB_ARQUIVO_DOCPROD A ");
                        sql.AppendLine(" WHERE A.CHAVE = @CHAVE AND A.NUM_OBRA LIKE @NUM_OBRA ");

                        sql.AppendLine(" UNION ");

                        sql.AppendLine(" SELECT A.ID, 'FOTO' AS NOME, A.TIPO, 'FOTO' AS TABELA ");
                        sql.AppendLine(" FROM TB_IMAGEM_DOCPROD_MOBILE A ");
                        sql.AppendLine(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA LIKE @NUM_OBRA ");
                        sql.AppendLine("   AND A.NUM_DOC = @NUM_DOC");

                        parametros.Add("@CHAVE", $"{request.CodCt}&{request.Projeto}&{request.Num_Doc}");
                        parametros.Add("@CODCT", request.CodCt);
                        parametros.Add("@PROJETO", request.Projeto);
                        parametros.Add("@NUM_OBRA", String.Format("%{0}%", request.Num_Obra.Split('/')[0]));
                        parametros.Add("@NUM_DOC", request.Num_Doc);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            Int32 iArquivo = 0;
                            Int32 iFoto = 0;

                            while (await reader.ReadAsync())
                            {
                                if (Convert.ToString(reader["TABELA"]) == "ARQUIVO")
                                    iArquivo++;
                                else
                                    iFoto++;

                                retorno.Add(new Fotos_ArquivosListaModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = String.Concat(Convert.ToString(reader["NOME"]), (Convert.ToString(reader["TABELA"]) == "ARQUIVO") ? iArquivo : iFoto),
                                    Tabela = Convert.ToString(reader["TABELA"]),
                                    Tipo = Convert.ToString(reader["TIPO"]),
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
            return new DataTableResponse<Fotos_ArquivosListaModel>
            {
                draw = request.draw,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet]
        [Route("api/Fotos_ArquivosApi/GetFotoArquivo")]
        public async Task<HttpResponseMessage> GetFotoArquivo(Int64 Id, String Tabela)
        {
            Byte[] arquivo = null;
            String tipo = String.Empty;
            using (ISession session = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        if (Tabela == "ARQUIVO")
                        {
                            sql.AppendLine(" SELECT A.ARQUIVO, A.TIPO ");
                            sql.AppendLine(" FROM TB_ARQUIVO_DOCPROD A ");
                            sql.AppendLine(" WHERE A.ID = @ID ");
                        }
                        else
                        {
                            sql.AppendLine(" SELECT A.ARQUIVO, A.TIPO ");
                            sql.AppendLine(" FROM TB_IMAGEM_DOCPROD_MOBILE A ");
                            sql.AppendLine(" WHERE A.ID = @ID ");
                        }

                        parametros.Add("@ID", Id);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                arquivo = (Byte[])reader["ARQUIVO"];
                                tipo = Convert.ToString(reader["TIPO"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(arquivo)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{Id}.{tipo}"
                };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

        [HttpGet]
        [Route("api/Fotos_ArquivosApi/ExibirFotoArquivo")]
        public async Task<GetApiRetorno<Caminho_Foto_Arquivo>> ExibirFotoArquivo(Int64 Id, String Tabela)
        {
            GetApiRetorno<Caminho_Foto_Arquivo> retorno = new GetApiRetorno<Caminho_Foto_Arquivo>();
            Caminho_Foto_Arquivo foto_arquivo = new Caminho_Foto_Arquivo();

            Byte[] arquivo = null;
            String tipo = String.Empty;

            System.Drawing.Image img;

            using (ISession session = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        if (Tabela == "ARQUIVO")
                        {
                            sql.AppendLine(" SELECT A.ARQUIVO, A.TIPO ");
                            sql.AppendLine(" FROM TB_ARQUIVO_DOCPROD A ");
                            sql.AppendLine(" WHERE A.ID = @ID ");
                        }
                        else
                        {
                            sql.AppendLine(" SELECT A.ARQUIVO, A.TIPO ");
                            sql.AppendLine(" FROM TB_IMAGEM_DOCPROD_MOBILE A ");
                            sql.AppendLine(" WHERE A.ID = @ID ");
                        }

                        parametros.Add("@ID", Id);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                tipo = Convert.ToString(reader["TIPO"]);
                                arquivo = (Byte[])reader["ARQUIVO"];

                                if (tipo.ToUpperInvariant() == "PNG" || tipo.ToUpperInvariant() == "BMP" || tipo.ToUpperInvariant() == "JPG")
                                {
                                    img = ImageService.BytesToImage(arquivo);

                                    try
                                    {
                                        foto_arquivo.Tipo = tipo.ToLowerInvariant();
                                        var nomeArq = DateTime.Now.ToString("ddMMyyyyHHmmss");

                                        if (!Directory.Exists($@"{CONSTANTES.CaminhoFisico}\Tmp"))
                                        {
                                            Directory.CreateDirectory($@"{CONSTANTES.CaminhoFisico}\Tmp");
                                        }

                                        img.Save($@"{CONSTANTES.CaminhoFisico}\Tmp\{nomeArq}.{foto_arquivo.Tipo}");
                                        foto_arquivo.Foto = $"{CONSTANTES.URLPortal}/Tmp/{nomeArq}.{tipo}";
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        MemoryStream mStream = new MemoryStream(arquivo);

                                        foto_arquivo.Tipo = tipo.ToLowerInvariant();
                                        var nomeArq = DateTime.Now.ToString("ddMMyyyyHHmmss");

                                        if (!Directory.Exists($@"{CONSTANTES.CaminhoFisico}\Tmp"))
                                        {
                                            Directory.CreateDirectory($@"{CONSTANTES.CaminhoFisico}\Tmp");
                                        }

                                        File.WriteAllBytes($@"{CONSTANTES.CaminhoFisico}\Tmp\{nomeArq}.{foto_arquivo.Tipo}", arquivo);

                                        foto_arquivo.Foto = $"{CONSTANTES.URLPortal}/Tmp/{nomeArq}.{tipo}";
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
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

            retorno.Ok = true;
            retorno.Objeto = foto_arquivo;

            return retorno;
        }

        [HttpGet]
        [Route("api/Fotos_ArquivosApi/GetArquivoFoto")]
        public async Task<String> quiz_getArquivoFoto(Int64 Id)
        {
            using (ISession session = HibernateUtil.GetSessionFoto())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ARQUIVO");
                        sql.AppendLine("FROM TB_QUIZ_FOTO A");
                        sql.AppendLine("WHERE ID = @ID");

                        parametros.Add("@ID", Id);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                return Sinapse.Util.ImageService.ConvertTo64((Byte[])reader["ARQUIVO"]);
                            }
                            else
                            {
                                return String.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}