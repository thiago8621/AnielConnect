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
    public class Tb_Tipo_ImovelController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Tipo_ImovelController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Tipo_Imovel}")]
        public async Task<Response> Atualizar(Int32? Cod_Tipo_Imovel)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Tipo_ImovelDao Tipo_ImovelDao = new Tb_Tipo_ImovelDao(conn, null))
                    {
                        List<Tb_Pv_Tipo_Imovel> protocolos = Tipo_ImovelDao.GetPv(Cod_Tipo_Imovel);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Tipo_Imovel, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoTipo_Imovel
                                    {
                                        Cod_Tipo_Imovel = g.Key.Cod_Tipo_Imovel,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Tipo_Imovel } into g
                                      select g.Key.Cod_Tipo_Imovel).ToList();

                        List<Tb_Tipo_Imovel> _Tipo_Imovels = Tipo_ImovelDao.Get(chaves);

                        if (_Tipo_Imovels.Count > 0)
                        {
                            using (Conexao<Tipo_Imovel> connNh = new Conexao<Tipo_Imovel>(session))
                            {
                                foreach (Int32 chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Cod_Tipo_Imovel == chave))
                                        {
                                            await ExecutaAcao(connNh, _Tipo_Imovels, chave, acao.Acao);
                                        }

                                        Tipo_ImovelDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        Tipo_ImovelDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
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
                        dao.AdicionaRegistroPV(typeof(Tb_Tipo_Imovel));
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


        private async Task ExecutaAcao(Conexao<Tipo_Imovel> connNh, List<Tb_Tipo_Imovel> Tipo_Imovels, Int32 Cod_Tipo_Imovel, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Tipo_Imovels, Cod_Tipo_Imovel, acao);
            }
            else
            {
                IList<Tipo_Imovel> _listTmp = await connNh.select(p => p.Cod_Tipo_Imovel == Cod_Tipo_Imovel);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Tipo_Imovel> Pump(Conexao<Tipo_Imovel> connNh, List<Tb_Tipo_Imovel> Tipo_Imovels, Int32 Cod_Tipo_Imovel, string acao)
        {
            Tb_Tipo_Imovel Tipo_Imovel = Tipo_Imovels.Find(p => p.Cod_Tipo_Imovel == Cod_Tipo_Imovel);

            Tipo_Imovel _tmp = new Tipo_Imovel();

            //VERIFICA SE EXISTE
            IList<Tipo_Imovel> _listTmp = await connNh.select(p => p.Cod_Tipo_Imovel == Cod_Tipo_Imovel);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Tipo_Imovel = Tipo_Imovel.Cod_Tipo_Imovel;
            _tmp.Descricao = Tipo_Imovel.Descricao;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Tipo_Imovel> GetTipo(Int16? Cod_Tipo_Imovel)
        {
            if (Cod_Tipo_Imovel.HasValue)
            {
                using (Conexao<Tipo_Imovel> connPerfil = new Conexao<Tipo_Imovel>(session))
                {
                    IList<Tipo_Imovel> _perfis = await connPerfil.select(p => p.Cod_Tipo_Imovel == Cod_Tipo_Imovel);
                    if (_perfis.Any())
                        return _perfis[0];
                    else
                    {
                        Tipo_Imovel perfil = new Tipo_Imovel
                        {
                            Cod_Tipo_Imovel = Cod_Tipo_Imovel.Value
                        };
                        await connPerfil.Save(perfil);
                        return perfil;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoTipo_Imovel
    {
        public Int32 Cod_Tipo_Imovel { get; set; }
        public String Acao { get; set; }
    }

}
