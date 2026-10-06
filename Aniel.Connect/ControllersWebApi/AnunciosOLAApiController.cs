using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Spreadsheet;
using NHibernate;
using PrimS.Telnet;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AnunciosOLAApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost()]
        [Route("api/AnunciosOLAapi/GetAll")]
        public async Task<GetApiRetorno<List<Anuncios_OLAModel>>> GetAll()
        {

            GetApiRetorno<List<Anuncios_OLAModel>> retorno = new GetApiRetorno<List<Anuncios_OLAModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.URL_IMAGEM_ANUNCIO, A.URL_DESTINO, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_ANUNCIOS_OLA A ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            List<Anuncios_OLAModel> listaAnuncios = new List<Anuncios_OLAModel>();

                            while (await reader.ReadAsync())
                            {
                                byte[] imagemAnuncioAux = reader["URL_IMAGEM_ANUNCIO"] != DBNull.Value ? (byte[])reader["URL_IMAGEM_ANUNCIO"] : null;

                                Int64 itemId = Convert.ToInt64(reader["ID"]);

                                String caminho = CONSTANTES.CaminhoFisico;

                                string pasta = String.Format(@"{0}\Arquivos\AnunciosOla", caminho);
                                if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                                string arquivo = String.Format(@"{0}\{1}.png", pasta, itemId);

                                if (!File.Exists(arquivo))
                                {
                                    if (imagemAnuncioAux != null)
                                    {
                                        using (FileStream arqv = new FileStream(arquivo, FileMode.Create))
                                        {
                                            arqv.Write(imagemAnuncioAux, 0, imagemAnuncioAux.Length);
                                            arqv.Close();

                                            //Image img = Image.FromStream(ms);
                                            //var foto64 = new ImageUtils().ImageToBase64(img, ImageFormat.Png);
                                            //img.Save(arquivo, ImageFormat.Png);
                                        }
                                    }
                                }

                                String urlBase = "";

                                if (File.Exists(arquivo)) urlBase = String.Format(@"{0}/Arquivos/AnunciosOla/{1}.png", CONSTANTES.URLPortal, itemId);

                                listaAnuncios.Add(new Anuncios_OLAModel
                                {
                                    Id = itemId,
                                    descricaoAnuncio = (String)reader["DESCRICAO"],
                                    urlImagemAnuncio = urlBase,
                                    urlDestino = (String)reader["URL_DESTINO"],                                  

                                });
                            }
                            retorno.Objeto = listaAnuncios;
                            retorno.Ok = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex);
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                }


                return retorno;

            }
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/AnunciosOLAapi/getAnuncios")]
        public async Task<GetApiRetorno<List<Anuncios_OLAModel>>> Anuncios_Ola()
        {

            GetApiRetorno<List<Anuncios_OLAModel>> retorno = new GetApiRetorno<List<Anuncios_OLAModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.URL_IMAGEM_ANUNCIO, A.URL_DESTINO, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_ANUNCIOS_OLA A ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            List<Anuncios_OLAModel> listaAnuncios = new List<Anuncios_OLAModel>();

                            while (await reader.ReadAsync())
                            {
                                byte[] imagemAnuncioAux = reader["URL_IMAGEM_ANUNCIO"] != DBNull.Value ? (byte[])reader["URL_IMAGEM_ANUNCIO"] : null;

                                Int64 itemId = Convert.ToInt64(reader["ID"]);

                                String caminho = CONSTANTES.CaminhoFisico;

                                string pasta = String.Format(@"{0}\Arquivos\AnunciosOla", caminho);
                                if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                                string arquivo = String.Format(@"{0}\{1}.png", pasta, itemId);

                                if (!File.Exists(arquivo))
                                {
                                    if (imagemAnuncioAux != null)
                                    {
                                        using (FileStream arqv = new FileStream(arquivo, FileMode.Create))
                                        {
                                            arqv.Write(imagemAnuncioAux, 0, imagemAnuncioAux.Length);
                                            arqv.Close();

                                            //Image img = Image.FromStream(ms);
                                            //var foto64 = new ImageUtils().ImageToBase64(img, ImageFormat.Png);
                                            //img.Save(arquivo, ImageFormat.Png);
                                        }
                                    }
                                }

                                String urlBase = "";

                                if (File.Exists(arquivo)) urlBase = String.Format(@"{0}/Arquivos/AnunciosOla/{1}.png", CONSTANTES.URLPortal, itemId);

                                listaAnuncios.Add(new Anuncios_OLAModel
                                {
                                    Id = itemId,
                                    descricaoAnuncio = (String)reader["DESCRICAO"],
                                    urlImagemAnuncio = urlBase,
                                    urlDestino = (String)reader["URL_DESTINO"],
                                    imagemAnuncioBase64 = imagemAnuncioAux != null ? Convert.ToBase64String(imagemAnuncioAux) : null

                                });
                            }
                            retorno.Objeto = listaAnuncios;
                            retorno.Ok = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex);
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                }


                return retorno;

            }
        }


        [Route("api/AnunciosOLAapi/Excluir")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> Excluir(Int64 id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Anuncios_OLA>(session))
                    {

                        Anuncios_OLA modelo = null;
                        if (id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == id)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
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





    }
}