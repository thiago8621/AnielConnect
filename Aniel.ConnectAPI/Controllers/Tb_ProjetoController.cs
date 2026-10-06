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
    public class Tb_ProjetoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_ProjetoController(ISession session)
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
                    using (Tb_ProjetoDao projetoDao = new Tb_ProjetoDao(conn, null))
                    {
                        List<Tb_Pv_Projeto> protocolos = projetoDao.GetPv(Codct,Projeto);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codct, o.Projeto, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoProjeto
                                    {
                                        Codct = g.Key.Codct,
                                        Projeto = g.Key.Projeto,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codct, o.Projeto } into g
                                      select new ChaveProjeto {
                                         Codct = g.Key.Codct,
                                         Projeto = g.Key.Projeto
                                      }).ToList();

                        List<Tb_Projeto> _projetos = projetoDao.Get(chaves);

                        using (Conexao<Projeto> connNh = new Conexao<Projeto>(session))
                        {
                            foreach (ChaveProjeto chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Codct == chave.Codct && p.Projeto == chave.Projeto))
                                    {
                                        ExecutaAcao(connNh, _projetos, chave.Codct, chave.Projeto, acao.Acao);
                                    }

                                    projetoDao.AtualizacaoPv(chave.Codct, chave.Projeto);
                                }
                                catch (Exception ex)
                                {
                                    projetoDao.AtualizacaoPvErro(chave.Codct, chave.Projeto, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Projeto));
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

        private void ExecutaAcao(Conexao<Projeto> connNh, List<Tb_Projeto> projetos, String codct, String projeto, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, projetos, codct, projeto, acao);
            }
            else
            {
                IList<Projeto> _listTmp = connNh.queryover()
                    .Where(p=> p.Codigo == projeto)
                    .JoinQueryOver(p=>p.Contrato)
                    .Where(p => p.CodCt == codct).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Projeto Pump(Conexao<Projeto> connNh, List<Tb_Projeto> projetos, String codct, String codigo, string acao)
        {
            Tb_Projeto projeto = projetos.Find(p => p.CodCt == codct && p.Projeto == codigo);

            Projeto _tmp = new Projeto();

            //VERIFICA SE EXISTE
            IList<Projeto> _listTmp = connNh.queryover()
                    .Where(p => p.Codigo == codigo)
                    .JoinQueryOver(p => p.Contrato)
                    .Where(p => p.CodCt == codct).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Contrato = new Tb_ContratoController(session).GetContrato(projeto.CodCt);
            _tmp.Codigo = projeto.Projeto;
            _tmp.Filial = new Tb_FilialController(session).GetFilial(projeto.CodEmpresa, projeto.CodFilial);
            _tmp.Localidade = projeto.Localidade;
            _tmp.IniPrev = projeto.InicPrev;
            _tmp.IniReal = projeto.InicReal;
            _tmp.TermPrev = projeto.TermPrev;
            _tmp.TermReal = projeto.TermReal;
            _tmp.TermReal2 = projeto.TermReal2;
            _tmp.Projeto_Regulador = new Tb_ProjetoController(session).GetProjeto(projeto.CodCt, projeto.Projeto);

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Projeto GetProjeto(String Codct, String Codigo)
        {
            if (!String.IsNullOrEmpty(Codct) && !String.IsNullOrEmpty(Codigo))
            {
                using (Conexao<Projeto> connProjeto = new Conexao<Projeto>(session))
                {
                    List<Contrato> _contratos = new Conexao<Contrato>(session).select(p => p.CodCt == Codct);

                    Contrato _contrato = null;
                    if (_contratos.Count == 0)
                        return null;

                    _contrato = _contratos[0];

                    IList<Projeto> _projetos = connProjeto.queryover()
                        .Where(p=> p.Codigo == Codigo)
                        .JoinQueryOver(p=>p.Contrato)
                        .Where(p => p.CodCt == Codct).List();
                    if (_projetos.Any())
                        return _projetos[0];
                    else
                    {
                        Projeto projeto = new Projeto
                        {
                            Contrato = _contrato,
                            Codigo = Codigo
                        };
                        connProjeto.Save(projeto);
                        return projeto;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoProjeto
    {
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public String Acao { get; set; }
    }
}
