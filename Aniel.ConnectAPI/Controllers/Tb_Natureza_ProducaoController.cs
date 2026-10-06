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
    public class Tb_Natureza_ProducaoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Natureza_ProducaoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Id}")]
        public Response Atualizar(Int32? Id)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Natureza_ProdDao Natureza_ProducaoDao = new Tb_Natureza_ProdDao(conn, null))
                    {
                        List<Tb_Pv_Natureza_Prod> protocolos = Natureza_ProducaoDao.GetPv(Id);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id_Natureza, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoNatureza_Prod
                                    {
                                        Id = g.Key.Id_Natureza,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id_Natureza } into g
                                      select g.Key.Id_Natureza).ToList();

                        List<Tb_Natureza_Producao> _Natureza_Producaos = Natureza_ProducaoDao.Get(chaves);

                        using (Conexao<Natureza_Producao> connNh = new Conexao<Natureza_Producao>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id == chave))
                                    {
                                        ExecutaAcao(connNh, _Natureza_Producaos, chave, acao.Acao);
                                    }

                                    Natureza_ProducaoDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Natureza_ProducaoDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Natureza_Producao));
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

        private void ExecutaAcao(Conexao<Natureza_Producao> connNh, List<Tb_Natureza_Producao> Natureza_Producaos, Int32 Id, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Natureza_Producaos, Id, acao);
            }
            else
            {
                List<Natureza_Producao> _listTmp = connNh.select(p => p.Id == Id);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Natureza_Producao Pump(Conexao<Natureza_Producao> connNh, List<Tb_Natureza_Producao> Natureza_Producaos, Int32 Id, string acao)
        {
            Tb_Natureza_Producao Natureza_Producao = Natureza_Producaos.Find(p => p.Id == Id);

            Natureza_Producao _tmp = new Natureza_Producao();

            Boolean novo = true;

            //VERIFICA SE EXISTE
            List<Natureza_Producao> _listTmp = connNh.select(p => p.Id == Id);
            if (_listTmp.Any())
            {
                _tmp = _listTmp.First();
                novo = false;
            }

            _tmp.Id = Natureza_Producao.Id;
            _tmp.Descricao= Natureza_Producao.Descricao;

            if (novo)
                connNh.Save(_tmp);
            else
                connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Natureza_Producao GetNatureza_Producao(Int32? Id)
        {
            if (Id.HasValue)
            {
                using (Conexao<Natureza_Producao> connNatureza_Producao = new Conexao<Natureza_Producao>(session))
                {
                    List<Natureza_Producao> _Natureza_Producaos = connNatureza_Producao.select(p => p.Id == Id);
                    if (_Natureza_Producaos.Any())
                        return _Natureza_Producaos[0];
                    else
                    {
                        Natureza_Producao Natureza_Producao = new Natureza_Producao
                        {
                            Id = Id.Value
                        };
                        connNatureza_Producao.Save(Natureza_Producao);
                        return Natureza_Producao;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoNatureza_Prod
    {
        public Int32 Id { get; set; }
        public String Acao { get; set; }
    }
}
