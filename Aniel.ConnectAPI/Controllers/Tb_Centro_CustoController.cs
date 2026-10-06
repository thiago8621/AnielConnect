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
    public class Tb_Centro_CustoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Centro_CustoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_CCusto}")]
        public async Task<Response> Atualizar(Int32? Cod_CCusto)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Centro_CustoDao CentroCustoDao = new Tb_Centro_CustoDao(conn, null))
                    {
                        List<Tb_Pv_Centro_Custo> protocolos = CentroCustoDao.GetPv(Cod_CCusto);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_CCusto, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoCentro_Custo
                                    {
                                        Cod_CCusto = g.Key.Cod_CCusto,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_CCusto } into g
                                      select g.Key.Cod_CCusto).ToList();

                        List<Tb_Centro_Custo> _Centros = CentroCustoDao.Get(chaves);

                        if (_Centros.Count > 0)
                        {
                            using (Conexao<Centro_Custo> connNh = new Conexao<Centro_Custo>(session))
                            {
                                foreach (Int16 chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in Enumerable.Where<AcaoCentro_Custo>(acoes, (Func<AcaoCentro_Custo, bool>)(p => (bool)(p.Cod_CCusto == chave))))
                                        {
                                            await ExecutaAcao(connNh, _Centros, chave, acao.Acao);
                                        }

                                        CentroCustoDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        CentroCustoDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
                                    }
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
        public async Task<Response> AtualizarTodos()
        {
            return await Atualizar(null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Centro_Custo));
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

        private async Task ExecutaAcao(Conexao<Centro_Custo> connNh, List<Tb_Centro_Custo> Centro_Custos, Int16 Cod_CCusto, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Centro_Custos, Cod_CCusto, acao);
            }
            else
            {
                IList<Centro_Custo> _listTmp = await connNh.select(p => p.Codigo == Cod_CCusto);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Centro_Custo> Pump(Conexao<Centro_Custo> connNh, List<Tb_Centro_Custo> Centro_Custos, Int16 Cod_CCusto, string acao)
        {
            Tb_Centro_Custo Centro_Custo = Centro_Custos.Find(p => p.Cod_Ccusto == Cod_CCusto);

            Centro_Custo _tmp = new Centro_Custo();

            //VERIFICA SE EXISTE
            IList<Centro_Custo> _listTmp = await connNh.select(p => p.Codigo == Cod_CCusto);
                if (_listTmp.Any())
                    _tmp = _listTmp.First();

            _tmp.Codigo = Centro_Custo.Cod_Ccusto;
            _tmp.Cod_Aux = Centro_Custo.Cod_Aux;
            _tmp.Status = 1;
            _tmp.Descricao = Centro_Custo.Descricao;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Centro_Custo> GetCentro_Custo(Int32? Cod_CCusto)
        {
            if (Cod_CCusto.HasValue)
            {
                using (Conexao<Centro_Custo> connCentro_Custo = new Conexao<Centro_Custo>(session))
                {
                    IList<Centro_Custo> _Centro_Custos = await connCentro_Custo.select(p => p.Codigo == Cod_CCusto);
                    if (_Centro_Custos.Any())
                    {
                        return _Centro_Custos[0];
                    }
                    else
                    {
                        Centro_Custo Centro_Custo = new Centro_Custo
                        {
                            Codigo = Cod_CCusto.Value,
                            Status = 1
                        };
                        await connCentro_Custo.Save(Centro_Custo);
                        return Centro_Custo;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoCentro_Custo
    {
        public Int32 Cod_CCusto { get; set; }
        public String Acao { get; set; }
    }
}
