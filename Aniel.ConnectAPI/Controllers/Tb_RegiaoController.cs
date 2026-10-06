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
    public class Tb_RegiaoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_RegiaoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Regiao}")]
        public Response Atualizar(Int16? Cod_Regiao)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_RegiaoDao RegiaoDao = new Tb_RegiaoDao(conn, null))
                    {
                        List<Tb_Pv_Regiao> protocolos = RegiaoDao.GetPv(Cod_Regiao);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Regiao, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoRegiao
                                    {
                                        Cod_Regiao = g.Key.Cod_Regiao,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Regiao } into g
                                      select g.Key.Cod_Regiao).ToList();

                        List<Tb_Regiao> _Regiaos = RegiaoDao.Get(chaves);

                        using (Conexao<Regiao> connNh = new Conexao<Regiao>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Regiao == chave))
                                    {
                                        ExecutaAcao(connNh, _Regiaos, chave, acao.Acao);
                                    }

                                    RegiaoDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    RegiaoDao.AtualizacaoPvErro(chave, ex.Message);
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
            return Atualizar(null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Regiao));
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

        private void ExecutaAcao(Conexao<Regiao> connNh, List<Tb_Regiao> Regiaos, Int16 Cod_Regiao, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Regiaos, Cod_Regiao, acao);
            }
            else
            {
                List<Regiao> _listTmp = connNh.select(p => p.Cod_Regiao == Cod_Regiao);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Regiao Pump(Conexao<Regiao> connNh, List<Tb_Regiao> Regiaos, Int16 Cod_Regiao, string acao)
        {
            Tb_Regiao Regiao = Regiaos.Find(p => p.Cod_Regiao == Cod_Regiao);

            Regiao _tmp = new Regiao();

            //VERIFICA SE EXISTE
            List<Regiao> _listTmp = connNh.select(p => p.Cod_Regiao == Cod_Regiao);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Regiao = Regiao.Cod_Regiao;
            _tmp.Nome = Regiao.Nome;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Regiao GetRegiao(Int16? Cod_Regiao)
        {
            if (Cod_Regiao.HasValue)
            {
                using (Conexao<Regiao> connRegiao = new Conexao<Regiao>(session))
                {
                    List<Regiao> _regiao = connRegiao.select(p => p.Cod_Regiao == Cod_Regiao);
                    if (_regiao.Any())
                        return _regiao[0];
                    else
                    {
                        Regiao regiao = new Regiao
                        {
                            Cod_Regiao = Cod_Regiao.Value
                        };
                        connRegiao.Save(regiao);
                        return regiao;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoRegiao
    {
        public Int16 Cod_Regiao { get; set; }
        public String Acao { get; set; }
    }
}
