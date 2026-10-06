//using Aniel.ConnectAPI.Dal.Aniel;
using Aniel.ConnectAPI.Model.Params;
using Aniel.ConnectAPI.Controllers.Base;
using Aniel.ConnectAPI.Model;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.ConnectAPI.Dal.Aniel;
using AnielTV.Hibernate.Persistence;
using NHibernate;
using Aniel.Hibernate.Entity;
using Aniel.ConnectAPI.Model.Aniel;
using Aniel.ConnectAPI.Model.Temporarios;
using Aniel.ConnectAPI.Dal;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class Tb_Sub_Tipo_ServicoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Sub_Tipo_ServicoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Tipo_Serv}/{Sub_Tipo_Servico}")]
        public Response Atualizar(Int32? Cod_Tipo_Serv, String Sub_Tipo_Servico)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Sub_Tipo_ServicoDao Sub_Tipo_ServicoDao = new Tb_Sub_Tipo_ServicoDao(conn, null))
                    {
                        List<Tb_Pv_Sub_Tipo_Servico> protocolos = Sub_Tipo_ServicoDao.GetPv(Cod_Tipo_Serv,Sub_Tipo_Servico);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Tipo_Serv, o.Cod_Sub_Tipo_Serv, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoSub_Tipo_Servico
                                    {
                                        Cod_Tipo_Serv = g.Key.Cod_Tipo_Serv,
                                        Cod_Sub_Tipo_Servico = g.Key.Cod_Sub_Tipo_Serv,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Tipo_Serv, o.Cod_Sub_Tipo_Serv } into g
                                      select new ChaveSub_Tipo_Servico {
                                         Cod_Tipo_Serv = g.Key.Cod_Tipo_Serv,
                                         Cod_Sub_Tipo_Serv = g.Key.Cod_Sub_Tipo_Serv
                                      }).ToList();

                        List<Tb_Sub_Tipo_Servico> _Sub_Tipo_Servicos = Sub_Tipo_ServicoDao.Get(chaves);

                        using (Conexao<Sub_Tipo_Servico> connNh = new Conexao<Sub_Tipo_Servico>(session))
                        {
                            foreach (ChaveSub_Tipo_Servico chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Tipo_Serv == chave.Cod_Tipo_Serv && p.Cod_Sub_Tipo_Servico == chave.Cod_Sub_Tipo_Serv))
                                    {
                                        ExecutaAcao(connNh, _Sub_Tipo_Servicos, chave.Cod_Tipo_Serv, chave.Cod_Sub_Tipo_Serv, acao.Acao);
                                    }

                                    Sub_Tipo_ServicoDao.AtualizacaoPv(chave.Cod_Tipo_Serv, chave.Cod_Sub_Tipo_Serv);
                                }
                                catch (Exception ex)
                                {
                                    Sub_Tipo_ServicoDao.AtualizacaoPvErro(chave.Cod_Tipo_Serv, chave.Cod_Sub_Tipo_Serv, ex.Message);
                                }
                            }
                        }

                        resposta.result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TrataRetornoErro(this.GetType().Assembly, out resposta, ex, null);
            }

            return resposta;
        }

        [HttpGet("AtualizaTodos")]
        public Response AtualizarTodos()
        {
            return Atualizar(null, String.Empty);
        }

        [HttpGet("AdicionaRegistroPV")]
        public Response AdicionaRegistroPV()
        {
            Response resposta = new Response();
            try
            {
                using (FbConnection conn = CreateConnection())
                {
                    using (DaoBase dao = new DaoBase(conn, null))
                    {
                        dao.AdicionaRegistroPV(typeof(Tb_Sub_Tipo_Servico));
                    }
                }

                resposta.result = true;
            }
            catch (Exception ex)
            {
                TrataRetornoErro(this.GetType().Assembly, out resposta, ex, null);
            }

            return resposta;
        }

        private void ExecutaAcao(Conexao<Sub_Tipo_Servico> connNh, List<Tb_Sub_Tipo_Servico> Sub_Tipo_Servicos, Int32 Cod_Tipo_Serv, String Sub_Tipo_Servico, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Sub_Tipo_Servicos, Cod_Tipo_Serv, Sub_Tipo_Servico, acao);
            }
            else
            {
                IList<Sub_Tipo_Servico> _listTmp = connNh.queryover()
                    .Where(p=> p.Codigo == Sub_Tipo_Servico)
                    .JoinQueryOver(p=>p.Tipo_Servico_Equipe)
                    .Where(p=> p.Cod_Tipo_Serv == Cod_Tipo_Serv).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Sub_Tipo_Servico Pump(Conexao<Sub_Tipo_Servico> connNh, List<Tb_Sub_Tipo_Servico> Sub_Tipo_Servicos, Int32 Cod_Tipo_Serv, String Cod_Sub_Tipo_Serv, string acao)
        {
            Tb_Sub_Tipo_Servico Sub_Tipo_Servico = Sub_Tipo_Servicos.Find(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv && p.Cod_Sub_Tipo_Serv == Cod_Sub_Tipo_Serv);

            Sub_Tipo_Servico _tmp = new Sub_Tipo_Servico();

            //VERIFICA SE EXISTE
            IList<Sub_Tipo_Servico> _listTmp = connNh.queryover()
                    .Where(p => p.Codigo == Cod_Sub_Tipo_Serv)
                    .JoinQueryOver(p => p.Tipo_Servico_Equipe)
                    .Where(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Tipo_Servico_Equipe = new Tb_Tipo_Serv_EquipeController(session).GetTipo_Servico_Equipe(Sub_Tipo_Servico.Cod_Tipo_Serv);
            _tmp.Codigo = Sub_Tipo_Servico.Cod_Sub_Tipo_Serv;
            _tmp.Descricao = Sub_Tipo_Servico.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Sub_Tipo_Servico GetSub_Tipo_Servico(Int32? Cod_Tipo_Serv, String Codigo)
        {
            if (Cod_Tipo_Serv.HasValue && !String.IsNullOrEmpty(Codigo))
            {
                using (Conexao<Sub_Tipo_Servico> connSub_Tipo_Servico = new Conexao<Sub_Tipo_Servico>(session))
                {
                    List<Tipo_Servico_Equipe> _tipos_servico = new Conexao<Tipo_Servico_Equipe>(session).select(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv);

                    Tipo_Servico_Equipe _tipo_Servico = null;
                    if (_tipos_servico.Count == 0)
                        return null;

                    _tipo_Servico = _tipos_servico[0];

                    IList<Sub_Tipo_Servico> _Sub_Tipo_Servicos = connSub_Tipo_Servico.queryover()
                        .Where(p => p.Codigo == Codigo)
                        .JoinQueryOver(p => p.Tipo_Servico_Equipe)
                        .Where(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv).List();

                    if (_Sub_Tipo_Servicos.Any())
                        return _Sub_Tipo_Servicos[0];
                    else
                    {
                        Sub_Tipo_Servico Sub_Tipo_Servico = new Sub_Tipo_Servico
                        {
                            Tipo_Servico_Equipe = _tipo_Servico,
                            Codigo = Codigo
                        };
                        connSub_Tipo_Servico.Save(Sub_Tipo_Servico);
                        return Sub_Tipo_Servico;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoSub_Tipo_Servico
    {
        public Int32 Cod_Tipo_Serv { get; set; }
        public String Cod_Sub_Tipo_Servico { get; set; }
        public String Acao { get; set; }
    }
}
