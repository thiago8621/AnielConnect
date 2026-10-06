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
    public class Tb_Codigo_EncerramentoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Codigo_EncerramentoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Encerramento}/{CodCt}")]
        public Response Atualizar(String Cod_Encerramento, String CodCt)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Codigo_EncerramentoDao Codigo_EncerramentoDao = new Tb_Codigo_EncerramentoDao(conn, null))
                    {
                        List<Tb_Pv_Codigo_Encerramento> protocolos = Codigo_EncerramentoDao.GetPv(Cod_Encerramento, CodCt);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Encerramento, o.Codct, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoCodigo_Encerramento
                                    {
                                        Cod_Encerramento = g.Key.Cod_Encerramento,
                                        CodCt = g.Key.Codct,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Encerramento, o.Codct } into g
                                      select new ChaveCod_Encerramento
                                      {
                                          Cod_Encerramento = g.Key.Cod_Encerramento,
                                          Codct = g.Key.Codct
                                      }).ToList();

                        List<Tb_Codigo_Encerramento> _codigos = Codigo_EncerramentoDao.Get(chaves);

                        using (Conexao<Codigo_Encerramento> connNh = new Conexao<Codigo_Encerramento>(session))
                        {
                            foreach (ChaveCod_Encerramento chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Encerramento == chave.Cod_Encerramento && p.CodCt == chave.Codct))
                                    {
                                        ExecutaAcao(connNh, _codigos, Cod_Encerramento, CodCt, acao.Acao);
                                    }

                                    Codigo_EncerramentoDao.AtualizacaoPv(chave.Cod_Encerramento, chave.Codct);
                                }
                                catch (Exception ex)
                                {
                                    Codigo_EncerramentoDao.AtualizacaoPvErro(chave.Cod_Encerramento, chave.Codct, ex.Message);
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
            return Atualizar(null, null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Codigo_Encerramento));
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

        private void ExecutaAcao(Conexao<Codigo_Encerramento> connNh, List<Tb_Codigo_Encerramento> filiais, String Cod_Encerramento, String CodCt, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, filiais, Cod_Encerramento, CodCt, acao);
            }
            else
            {
                IList<Codigo_Encerramento> _listTmp = connNh.queryover()
                    .Where(p => p.Cod_Encerramento == Cod_Encerramento)
                    .JoinQueryOver(p=> p.Contrato)
                    .Where(p=> p.CodCt == CodCt).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Codigo_Encerramento Pump(Conexao<Codigo_Encerramento> connNh, List<Tb_Codigo_Encerramento> codigos, String Cod_Encerramento, String CodCt, string acao)
        {
            Tb_Codigo_Encerramento Codigo_Encerramento = codigos.Find(p => p.Cod_Encerramento == Cod_Encerramento && p.Codct == CodCt);

            Codigo_Encerramento _tmp = new Codigo_Encerramento();

            //VERIFICA SE EXISTE
            IList<Codigo_Encerramento> _listTmp = connNh.queryover()
                        .Where(cod=> cod.Cod_Encerramento == Cod_Encerramento)
                        .JoinQueryOver(c => c.Contrato)
                        .Where(c => c.CodCt == CodCt).List();
            

            _tmp.Contrato = new Tb_ContratoController(session).GetContrato(Codigo_Encerramento.Codct);
            _tmp.Cod_Encerramento = Codigo_Encerramento.Cod_Encerramento;
            _tmp.Descricao = Codigo_Encerramento.Descricao;
            _tmp.Status_Execucao = Codigo_Encerramento.Status_Execucao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Codigo_Encerramento GetCodigo_Encerramento(String Cod_Encerramento, String CodCt)
        {
            if (!String.IsNullOrEmpty(Cod_Encerramento) && !String.IsNullOrEmpty(CodCt))
            {
                using (Conexao<Codigo_Encerramento> connCodigo_Encerramento = new Conexao<Codigo_Encerramento>(session))
                {
                    List<Contrato> _contratos = new Conexao<Contrato>(session).select(p => p.CodCt == CodCt);
                    Contrato _contrato = null;
                    if (_contratos.Count == 0)
                        return null;

                    _contrato = _contratos[0];

                    IList<Codigo_Encerramento> _Codigo_Encerramento = connCodigo_Encerramento.queryover()
                        .Where(p => p.Cod_Encerramento == Cod_Encerramento)
                        .JoinQueryOver(p => p.Contrato)
                        .Where(p => p.CodCt == CodCt).List();

                    if (_Codigo_Encerramento.Any())
                        return _Codigo_Encerramento[0];
                    else
                    {
                        Codigo_Encerramento Codigo_Encerramento = new Codigo_Encerramento
                        {
                            Contrato = _contrato,
                            Cod_Encerramento = Cod_Encerramento
                        };
                        connCodigo_Encerramento.Save(Codigo_Encerramento);
                        return Codigo_Encerramento;
                    }
                }
            }
            else
                return null;
        }
    }

    public class AcaoCodigo_Encerramento
    {
        public String Cod_Encerramento { get; set; }
        public String CodCt { get; set; }
        public String Acao { get; set; }
    }
}
