using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.Tasks
{
    public class Area_ManutencaoTask : BackgroundTask
    {
        [Obsolete("")]
        public async Task<GetApiRetorno<PostApiMensagem>> DesativaAutomatico(long Id)
        {
            var retorno = new GetApiRetorno<PostApiMensagem>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoArea = new Conexao<Area_Manutencao>(sessionT5))
                {

                    var area = await repoArea.selectFirst(obj => obj.Id == Id);
                    if (area.Data_HoraFinal <= DateTime.Now)
                    {
                        var alterar = new AlterarStatusViewModel
                        {
                            ids = new List<long> { Id },
                            //usuario = 
                            status = Convert.ToInt16(Status_Default.Inativo),
                        };
                        using (var api = new Area_ManutencaoApiController())
                        {
                            var StatusinApi = await api.AlterarStatus(alterar);
                            if (StatusinApi.Ok)
                            {
                                retorno.Objeto = new PostApiMensagem
                                {
                                    Mensagem = $"{RsBase.OPERACAOBEMSUCEDIDA} \r\n Manutenção de Id {area.Id} alterado encontra-se {RsBase.STATUS_VENCIDO}",
                                    Tipo = PostApiMensagemType.Info,
                                };
                            }
                            else
                            {
                                retorno.Objeto = new PostApiMensagem
                                {
                                    Mensagem = $"{RsBase.ERROR} \r\n {StatusinApi.Mensagem}",
                                    Tipo = PostApiMensagemType.Info,
                                };
                            }
                        }
                    }
                    else
                    {
                        retorno.Objeto = new PostApiMensagem
                        {
                            Mensagem = $" Manutenção de Id {area.Id} permanece {RsBase.STATUS_ABERTO}",
                            Tipo = PostApiMensagemType.Info,
                        };

                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }

        [Obsolete("")]
        public async Task<GetApiRetorno<PostApiMensagem>> AtivaAutomatico(long Id)
        {
            var retorno = new GetApiRetorno<PostApiMensagem>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Area_Manutencao>(sessionT5))
                using (var repoArea = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                {
                    var area = (await repository.selectFirst(p => p.Id == Id));
                    var elements = (await repoArea.select(obj => obj.AreaManutencao == area));
                    if (area.Data_HoraFinal >= DateTime.Now)
                    {

                        var alterar = new AlterarStatusViewModel
                        {
                            ids = elements.Select(p => p.Id).ToList(),
                            //usuario = 
                            status = Convert.ToInt16(Status_Elemento.Manutencao),
                        };
                        using (var api = new Elemento_RedeApiController())
                        {
                            var StatusinApi = await api.AlterarStatus(alterar);
                            if (StatusinApi.Ok)
                            {
                                retorno.Objeto = new PostApiMensagem
                                {
                                    Mensagem = $" {RsBase.OPERACAOBEMSUCEDIDA} \r\n Os elementos na área de Id: {area.Id} encontram-se sob o status de {RsCadastros.MANUTENCAO}",
                                    Tipo = PostApiMensagemType.Info,
                                };
                            }
                            else
                            {
                                retorno.Objeto = new PostApiMensagem
                                {
                                    Mensagem = $" {RsBase.ERROR} \r\n {StatusinApi.Mensagem}",
                                    Tipo = PostApiMensagemType.Info,
                                };
                            }
                        }
                    }
                    else
                    {
                        retorno.Objeto = new PostApiMensagem
                        {
                            Mensagem = $" Elementos vinculados à Área de Manutenção de Id {area.Id} não tiveram seu status alterado",
                            Tipo = PostApiMensagemType.Info,
                        };

                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        /// <summary>
        /// Mudança de método para notificação de Clientes afetados pela Manutençãode Rede 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public void NotificacaoClientesAfetados() 
        {
        }
    }
}