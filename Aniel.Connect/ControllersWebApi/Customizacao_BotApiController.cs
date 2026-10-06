using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Customizacao_BotApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Customizacao_BotApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Bot_CustomizacaoModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Bot modelo = null;
                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Bot>(session))
                {
                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        retorno.InnerException = "Bot não encontrado para o Id: " + value.Objeto.Id;
                    }
                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }

                    //envia a imagem
                    if (modelo.Avatar == null)
                    {
                        modelo.Avatar = $@"{CONSTANTES.URLPortal}/Arquivos/Bot/Default.jpg";
                    }

                    BindViewToObj(session, ref modelo, value.Objeto);



                    await repository.SaveOrUpdate(modelo);
                }

                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/BotApi/FindById")]
        public async Task<GetApiRetorno<Bot_CustomizacaoModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Bot_CustomizacaoModel> retorno = new GetApiRetorno<Bot_CustomizacaoModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Bot>(session))
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

        public string GravarAvatar(Bot_CustomizacaoModel model)
        {
            string retornoURL = null;
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Bot";

            if(model.Files.Count > 0)
            {
                for (var i = 0; i < model.Files.Count; i++)
                {
                    if (!String.IsNullOrEmpty(model.Files[i].FileName))
                    {

                        if (!Directory.Exists(_pasta))
                        {
                            Directory.CreateDirectory(_pasta);
                        }

                        var extensao = Path.GetExtension(model.Files[i].FileName);
                        byte[] allBytes = new byte[model.Files[i].InputStream.Length];
                        model.Files[i].InputStream.Read(allBytes, 0, (int)model.Files[i].InputStream.Length);
                        String _arquivo = $@"{_pasta}\{model.Id}{extensao}";
                        System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                        retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Bot/{model.Id}{extensao}";
                    }
                }
            }

            return retornoURL;
        }

        private Bot_CustomizacaoModel GetViewObj(Bot obj)
        {
            return new Bot_CustomizacaoModel
            {
                Id = obj.Id,
                Avatar = obj.Avatar,
                Nome = obj.Nome,
                TextoInvalido = obj.TextoInvalido,
                Transferencia = obj.Transferencia.GetValueOrDefault(false),
                Minutos = obj.Minutos,
                Status = obj.Status == Status_Default.Ativo,
                Habilitado = obj.Habilitado == Habilitado_Bot.Ativo
            };
        }

        private void BindViewToObj(ISession session, ref Bot objBanco, Bot_CustomizacaoModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Nome = view.Nome;
            objBanco.TextoInvalido = view.TextoInvalido;
            objBanco.Minutos = view.Minutos;
            objBanco.Transferencia = view.Transferencia;
            objBanco.Habilitado = view.Habilitado == true ? Habilitado_Bot.Ativo : Habilitado_Bot.Inativo;
            if (view.Files.Where(p => p != null).Count() > 0)
            {
                objBanco.Avatar = GravarAvatar(view);
            }
        }
    }
}