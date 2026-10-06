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
    public class Tb_Faixa_Rx_TxController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Faixa_Rx_TxController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Codct}/{Projeto}")]
        public Response Atualizar(String Codct, String Projeto)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Faixa_RX_TXDao faixaDao = new Tb_Faixa_RX_TXDao(conn, null))
                    {
                        List<Tb_Pv_Faixa_Rx_Tx> protocolos = faixaDao.GetPv(Codct,Projeto);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id_Faixa, o.CodCt, o.Projeto, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoFaixa
                                    {
                                        Id_Faixa = g.Key.Id_Faixa,
                                        CodCt = g.Key.CodCt,
                                        Projeto = g.Key.Projeto,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id_Faixa, o.CodCt, o.Projeto } into g
                                      select new ChaveFaixa
                                      {
                                         Id_Faixa = g.Key.Id_Faixa,
                                         Codct = g.Key.CodCt,
                                         Projeto = g.Key.Projeto
                                      }).ToList();

                        List<Tb_Faixas_Rx_Tx> _faixas = faixaDao.Get(chaves);

                        using (Conexao<Faixas_Rx_Tx_Snr> connNh = new Conexao<Faixas_Rx_Tx_Snr>(session))
                        {
                            foreach (ChaveFaixa chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id_Faixa == chave.Id_Faixa && p.CodCt == chave.Codct && p.Projeto == chave.Projeto))
                                    {
                                        ExecutaAcao(connNh, _faixas, chave.Id_Faixa, chave.Codct, chave.Projeto, acao.Acao);
                                    }

                                    faixaDao.AtualizacaoPv(chave.Id_Faixa, chave.Codct, chave.Projeto);
                                }
                                catch (Exception ex)
                                {
                                    faixaDao.AtualizacaoPvErro(chave.Id_Faixa, chave.Codct, chave.Projeto, ex.Message);
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
            return Atualizar(String.Empty, String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Faixas_Rx_Tx));
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

        private void ExecutaAcao(Conexao<Faixas_Rx_Tx_Snr> connNh, List<Tb_Faixas_Rx_Tx> projetos, Int64 id_faixa, String codct, String projeto, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, projetos, id_faixa, codct, projeto, acao);
            }
            else
            {
                IList<Faixas_Rx_Tx_Snr> _listTmp = connNh.queryover()
                    .Where(p => p.Id == id_faixa)
                    .JoinQueryOver(p=> p.Projeto)
                    .Where(p => p.Codigo == projeto)
                    .JoinQueryOver(p=> p.Contrato)
                    .Where(p=> p.CodCt == codct).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Faixas_Rx_Tx_Snr Pump(Conexao<Faixas_Rx_Tx_Snr> connNh, List<Tb_Faixas_Rx_Tx> projetos, Int64 Id_Faixa, String codct, String codigo, string acao)
        {
            Tb_Faixas_Rx_Tx faixa = projetos.Find(p => p.Codct == codct && p.Projeto == codigo);

            Faixas_Rx_Tx_Snr _tmp = new Faixas_Rx_Tx_Snr();

            //VERIFICA SE EXISTE
            IList<Faixas_Rx_Tx_Snr> _listTmp = connNh.queryover()
                    .Where(p => p.Id == Id_Faixa)
                    .JoinQueryOver(p=>p.Projeto)
                    .Where(p => p.Codigo == codigo)
                    .JoinQueryOver(p=>p.Contrato)
                    .Where(p=> p.CodCt == codct).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Id = Id_Faixa;
            _tmp.Projeto = new Tb_ProjetoController(session).GetProjeto(faixa.Codct, faixa.Projeto);
            _tmp.Max_Rx = faixa.Max_Rx;
            _tmp.Max_Snr = faixa.Max_Snr;
            _tmp.Max_Tx = faixa.Max_Tx;
            _tmp.Min_Rx = faixa.Min_Rx;
            _tmp.Min_Snr = faixa.Min_Snr;
            _tmp.Min_Tx = faixa.Min_Tx;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public IList<Faixas_Rx_Tx_Snr> GetFaixas(String Codct, String Projeto)
        {
            if (!String.IsNullOrEmpty(Codct) && !String.IsNullOrEmpty(Projeto))
            {
                using (Conexao<Faixas_Rx_Tx_Snr> connProjeto = new Conexao<Faixas_Rx_Tx_Snr>(session))
                {
                    IList<Faixas_Rx_Tx_Snr> _faixas = new Conexao<Faixas_Rx_Tx_Snr>(session).queryover()
                        .JoinQueryOver(p=>p.Projeto)
                        .Where(p => p.Codigo == Projeto)
                        .JoinQueryOver(p=>p.Contrato)
                        .Where(p=> p.CodCt == Codct).List();

                    return _faixas;
                }
            }
            else
                return null;
        }

    }

    public class AcaoFaixa
    {
        public Int64 Id_Faixa { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Acao { get; set; }
    }
}
