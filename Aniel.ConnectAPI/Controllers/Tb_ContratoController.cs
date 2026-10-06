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
using Aniel.ConnectAPI.Dal;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class Tb_ContratoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_ContratoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Codct}")]
        public Response Atualizar(String Codct)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_ContratoDao contratoDao = new Tb_ContratoDao(conn, null))
                    {
                        List<Tb_Pv_Contrato> protocolos = contratoDao.GetPv(Codct);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codct, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoContrato
                                    {
                                        Codct = g.Key.Codct,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codct } into g
                                      select g.Key.Codct).ToList();

                        List<Tb_Contrato> _contratos = contratoDao.Get(chaves);

                        using (Conexao<Contrato> connNh = new Conexao<Contrato>(session))
                        {
                            foreach (String chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Codct == chave))
                                    {
                                        ExecutaAcao(connNh, _contratos, chave, acao.Acao);
                                    }

                                    contratoDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    contratoDao.AtualizacaoPvErro(chave, ex.Message);
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
            return Atualizar(String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Contrato));
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

        private void ExecutaAcao(Conexao<Contrato> connNh, List<Tb_Contrato> contratos, String codct, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, contratos, codct, acao);
            }
            else
            {
                List<Contrato> _listTmp = connNh.select(p => p.CodCt == codct);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Contrato Pump(Conexao<Contrato> connNh, List<Tb_Contrato> contratos, String codct, string acao)
        {
            Tb_Contrato contrato = contratos.Find(p => p.CodCt == codct);

            Contrato _tmp = new Contrato();

            //VERIFICA SE EXISTE
            List<Contrato> _listTmp = connNh.select(p => p.CodCt == codct);
                if (_listTmp.Any())
                    _tmp = _listTmp.First();
            

            _tmp.CodCt = contrato.CodCt;
            _tmp.Descricao = contrato.Descricao;
            _tmp.Filial = new Tb_FilialController(session).GetFilial(contrato.CodEmpresa, contrato.CodFilial);
            _tmp.Fornec_Mat_Padrao = contrato.Fornec_Mat_Padrao;
            _tmp.Localidade = contrato.Localidade;
            _tmp.Projeto_Regulador = new Tb_ProjetoController(session).GetProjeto(contrato.CodCt_Regulador, contrato.Projeto_Regulador); ;
            _tmp.TipoSaldo = contrato.TipoSaldo;
            _tmp.TipoSaldoAplic = contrato.TipoSaldoAplic;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Contrato GetContrato(String Codct)
        {
            if (!String.IsNullOrEmpty(Codct))
            {
                using (Conexao<Contrato> connContrato = new Conexao<Contrato>(session))
                {
                    List<Contrato> _contratos = connContrato.select(p => p.CodCt == Codct);
                    if (_contratos.Any())
                        return _contratos[0];
                    else
                    {
                        Contrato contrato = new Contrato
                        {
                            CodCt = Codct
                        };
                        connContrato.Save(contrato);
                        return contrato;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoContrato
    {
        public String Codct { get; set; }
        public String Acao { get; set; }
    }
}
