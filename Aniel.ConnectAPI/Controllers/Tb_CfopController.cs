using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.ConnectAPI.Controllers.Base;
using Aniel.ConnectAPI.Dal;
using Aniel.ConnectAPI.Model.Aniel;
using Aniel.ConnectAPI.Model.Params;
using Aniel.Hibernate.Entity;
using AnielTV.Hibernate.Persistence;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using NHibernate;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class Tb_CfopController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_CfopController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{CodCfop}")]
        public async Task<Response> Atualizar(String CodCfop)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_CfopDao CfopDao = new Tb_CfopDao(conn, null))
                    {
                        List<Tb_Pv_Cfop> protocolos = CfopDao.GetPv(CodCfop);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codigo, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoCfop
                                    {
                                        Codigo = g.Key.Codigo,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codigo } into g
                                      select g.Key.Codigo).ToList();

                        List<Tb_Cfop> _Cfops = CfopDao.Get(chaves);

                        using (Conexao<Cfop> connNh = new Conexao<Cfop>(session))
                        {
                            foreach (String chave in chaves)
                            { 
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Codigo == chave))
                                    {
                                        await ExecutaAcao(connNh, _Cfops, chave, acao.Acao);
                                    }

                                    CfopDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    CfopDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Cfop));
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

        private async Task ExecutaAcao(Conexao<Cfop> connNh, List<Tb_Cfop> Cfops, String Cod_Cfop, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Cfops, Cod_Cfop, acao);
            }
            else
            {
                IList<Cfop> _listTmp = await connNh.select(p => p.Codigo == Cod_Cfop);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Cfop> Pump(Conexao<Cfop> connNh, List<Tb_Cfop> Cfops, String Cod_Cfop, string acao)
        {
            Tb_Cfop Cfop = Cfops.Find(p => p.Cfop == Cod_Cfop);

            Cfop _tmp = new Cfop();

            //VERIFICA SE EXISTE
            IList<Cfop> _listTmp = await connNh.select(p => p.Codigo == Cod_Cfop);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            
            _tmp.Codigo = Cfop.Cfop;
            _tmp.Descricao = Cfop.Descricao;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet("AtualizaTodos")]
        public async Task<Response> AtualizarTodos()
        {
            return await Atualizar(null);
        }

        [HttpGet()]
        public async Task<Cfop> GetCfop(String Cod_Cfop)
        {
            if (Cod_Cfop != String.Empty)
            {
                using (Conexao<Cfop> connCfop = new Conexao<Cfop>(session))
                {
                    IList<Cfop> _Cfops = await connCfop.select(p => p.Codigo == Cod_Cfop);
                    if (_Cfops.Any())
                        return _Cfops[0];
                    else
                    {
                        Cfop Cfop = new Cfop
                        {
                            Codigo = Cod_Cfop
                        };

                        await connCfop.Save(Cfop);
                        return Cfop;
                    }
                }
            }
            else
                return null;
        }
    }

    public class AcaoCfop
    {
        public String Codigo { get; set; }
        public String Acao { get; set; }
    }
}