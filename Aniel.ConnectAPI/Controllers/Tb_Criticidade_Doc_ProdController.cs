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
    public class Tb_Criticidade_Doc_ProdController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Criticidade_Doc_ProdController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Criticidade_Prod}")]
        public Response Atualizar(Int64? Criticidade_Prod)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Criticidade_Doc_ProdDao CriticidadeDao = new Tb_Criticidade_Doc_ProdDao(conn, null))
                    {
                        List<Tb_Pv_Criticidade_Prod> protocolos = CriticidadeDao.GetPv(Criticidade_Prod);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Criticidade_Prod, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoCriticidade
                                    {
                                        Criticidade_Prod = g.Key.Criticidade_Prod,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Criticidade_Prod } into g
                                      select g.Key.Criticidade_Prod).ToList();

                        List<Tb_Criticidade_Doc_Prod> _Criticidade_Doc_Prods = CriticidadeDao.Get(chaves);

                        using (Conexao<Criticidade_Doc_Prod> connNh = new Conexao<Criticidade_Doc_Prod>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Criticidade_Prod == chave))
                                    {
                                        ExecutaAcao(connNh, _Criticidade_Doc_Prods, chave, acao.Acao);
                                    }

                                    CriticidadeDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    CriticidadeDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Criticidade_Doc_Prod));
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

        private void ExecutaAcao(Conexao<Criticidade_Doc_Prod> connNh, List<Tb_Criticidade_Doc_Prod> Criticidade_Doc_Prods, Int64 Criticidade_Prod, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Criticidade_Doc_Prods, Criticidade_Prod, acao);
            }
            else
            {
                List<Criticidade_Doc_Prod> _listTmp = connNh.select(p => p.Id == Criticidade_Prod);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Criticidade_Doc_Prod Pump(Conexao<Criticidade_Doc_Prod> connNh, List<Tb_Criticidade_Doc_Prod> Criticidade_Doc_Prods, Int64 Criticidade_Prod, string acao)
        {
            Tb_Criticidade_Doc_Prod Criticidade_Doc_Prod = Criticidade_Doc_Prods.Find(p => p.Id == Criticidade_Prod);

            Criticidade_Doc_Prod _tmp = new Criticidade_Doc_Prod();

            //VERIFICA SE EXISTE
            List<Criticidade_Doc_Prod> _listTmp = connNh.select(p => p.Id == Criticidade_Prod);
                if (_listTmp.Any())
                    _tmp = _listTmp.First();
            

            _tmp.Descricao = Criticidade_Doc_Prod.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Criticidade_Doc_Prod GetCriticidade_Doc_Prod(Int64? Criticidade_Prod)
        {
            if (Criticidade_Prod.HasValue)
            {
                using (Conexao<Criticidade_Doc_Prod> connCriticidade_Doc_Prod = new Conexao<Criticidade_Doc_Prod>(session))
                {
                    List<Criticidade_Doc_Prod> _Criticidade_Doc_Prod = connCriticidade_Doc_Prod.select(p => p.Id == Criticidade_Prod);
                    if (_Criticidade_Doc_Prod.Any())
                        return _Criticidade_Doc_Prod[0];
                    else
                    {
                        Criticidade_Doc_Prod Criticidade_Doc_Prod = new Criticidade_Doc_Prod
                        {
                            Id = Criticidade_Prod.Value
                        };
                        connCriticidade_Doc_Prod.Save(Criticidade_Doc_Prod);
                        return Criticidade_Doc_Prod;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoCriticidade
    {
        public Int64 Criticidade_Prod { get; set; }
        public String Acao { get; set; }
    }
}
