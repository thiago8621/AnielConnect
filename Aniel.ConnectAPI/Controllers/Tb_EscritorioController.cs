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
    public class Tb_EscritorioController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_EscritorioController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Escritorio}")]
        public Response Atualizar(String Cod_Escritorio)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_EscritorioDao EscritorioDao = new Tb_EscritorioDao(conn, null))
                    {
                        List<Tb_Pv_Escritorio> protocolos = EscritorioDao.GetPv(Cod_Escritorio);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Escritorio, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoEscritorio
                                    {
                                        Cod_Escritorio = g.Key.Cod_Escritorio,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Escritorio } into g
                                      select g.Key.Cod_Escritorio).ToList();

                        List<Tb_Escritorio> _Escritorios = EscritorioDao.Get(chaves);

                        using (Conexao<Escritorio> connNh = new Conexao<Escritorio>(session))
                        {
                            foreach (String escritorio in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Escritorio == escritorio))
                                    {
                                        ExecutaAcao(connNh, _Escritorios, escritorio, acao.Acao);
                                    }

                                    EscritorioDao.AtualizacaoPv(escritorio);
                                }
                                catch (Exception ex)
                                {
                                    EscritorioDao.AtualizacaoPvErro(escritorio, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Escritorio));
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

        private void ExecutaAcao(Conexao<Escritorio> connNh, List<Tb_Escritorio> Escritorios, String cod_escritorio, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Escritorios, cod_escritorio, acao);
            }
            else
            {
                List<Escritorio> _listTmp = connNh.select(p => p.Cod_Escritorio == cod_escritorio);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Escritorio Pump(Conexao<Escritorio> connNh, List<Tb_Escritorio> Escritorios, String cod_escritorio, String acao)
        {
            Tb_Escritorio Escritorio = Escritorios.Find(p => p.Cod_Escritorio == cod_escritorio);

            Escritorio _tmp = new Escritorio();

            //VERIFICA SE EXISTE
            List<Escritorio> _listTmp = connNh.select(p => p.Cod_Escritorio == cod_escritorio);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Escritorio = Escritorio.Cod_Escritorio;
            _tmp.Descricao = Escritorio.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Escritorio GetEscritorio(String Cod_Escritorio)
        {
            if (!String.IsNullOrEmpty(Cod_Escritorio))
            {
                using (Conexao<Escritorio> connEscritorio = new Conexao<Escritorio>(session))
                {
                    List<Escritorio> _Escritorios = connEscritorio.select(p => p.Cod_Escritorio == Cod_Escritorio);
                    if (_Escritorios.Any())
                        return _Escritorios[0];
                    else
                    {
                        Escritorio Escritorio = new Escritorio
                        {
                            Cod_Escritorio = Cod_Escritorio
                        };
                        connEscritorio.Save(Escritorio);
                        return Escritorio;
                    }
                }
            }
            else
                return null;
        }
    }

    public class AcaoEscritorio
    {
        public String Cod_Escritorio { get; set; }
        public String Acao { get; set; }
    }
}
