


using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Conversations.V1;

namespace Aniel.Connect.Acoes
{
    public class AtendimentoAcoes : BaseApiController
    {

        public async Task<String> AlterarStatusEmAtendimento(AlterarStatusAtendimentoViewModel value)
        {
            value.status = 5;
            var atendimentoApi = new AtendimentoApiController();
            var api = await atendimentoApi.AlterarStatus(value);

            return api.Mensagem;
        }

        public async Task<PostApiRetorno<Int64>> AlterarStatusAguardando(AlterarStatusAtendimentoViewModel value)
        {
            value.status = 2;
            var atendimentoApi = new AtendimentoApiController();
            return await atendimentoApi.AlterarStatus(value);
        }

        public async Task<PostApiRetornoAtendimento> ConcluirAtendimento(string protocolo)
        {
            //recupera id do Atendimento 
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Atendimento>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            using (var conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                
                var result = await repository.selectFirst(p => p.Numero == protocolo);

                var id = result.Id;
                if (id == 0)
                {
                    throw new Exception("Id incorreto");
                }

                var atendimentoApi = new AtendimentoApiController();

                var atendimento = await atendimentoApi.FindById(id);

                if (atendimento.Objeto == null)
                {
                    throw new Exception("Atendimento não encontrado");
                }

                atendimento.Objeto.Status_Atendimento = Status_Atendimento.Concluido;

                var param = new ParamWebApi<AtendimentoCadastroModel>
                {
                    Objeto = atendimento.Objeto
                };


                return await atendimentoApi.Gravar(param);
            }
        }

        public async Task<PostApiRetorno<Int64>> TransferenciaSetor(TransferirAtendimentoRequestModel value)
        {
            if (value.novoDepartamento == 0)
            {
                throw new Exception("Informe o novo setor");
            }
            var atendimentoApi = new AtendimentoApiController();
            return await atendimentoApi.TransferirAtendimento(value);
        }

        public async Task<PostApiRetorno<Int64>> TransferenciaAtendente(TransferirAtendimentoRequestModel value)
        {
            if (String.IsNullOrEmpty(value.novoAtendente))
            {
                throw new Exception("Informe a nova atendente");
            }
            var atendimentoApi = new AtendimentoApiController();
            return await atendimentoApi.TransferirAtendimento(value);
        }

        //public async Task<AtendimentoCadastroModel> EnviarPesquisaSatisfacao(ISession session, AtendimentoCadastroModel value)
        //{
        //    using (var repoCliente = new Conexao<Cliente>(session))
        //    {
        //        var cliente = value.Id_Cliente.HasValue ? repoCliente.selectFirst(p => p.Id == value.Id_Cliente.Value).Result : null;

        //        if (value.Status_Atendimento == Status_Atendimento.Concluido)
        //        {
        //            var pesquisa_disparo_auto = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Padrao_Fim_Atend).Result;
        //            if (pesquisa_disparo_auto.Count != 0)
        //            {
        //                await new Pesquisa_SatisfacaoApiController().EnviarEmMassa(new Pesquisa_Satisfacao_PesquisaEmMassaModel
        //                {
        //                    ids = new List<long> { cliente.Id },
        //                    ids_pesquisa = pesquisa_disparo_auto.Select(p => p.Id).ToList(),
        //                    usuario = User.Identity.GetUserId()
        //                });
        //            }
        //        }
        //        //TODO arrumar retorno
        //        return value;
        //    }
        //}
    }
}