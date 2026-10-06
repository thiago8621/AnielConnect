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
    public class Tb_AreaController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_AreaController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Escritorio}/{Cod_Area}")]
        public Response Atualizar(String Cod_Escritorio, String Cod_Area)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_AreaDao Cod_AreaDao = new Tb_AreaDao(conn, null))
                    {
                        List<Tb_Pv_Area> protocolos = Cod_AreaDao.GetPv(Cod_Escritorio,Cod_Area);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Escritorio, o.Cod_Area, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoArea
                                    {
                                        Cod_Escritorio = g.Key.Cod_Escritorio,
                                        Cod_Area = g.Key.Cod_Area,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Escritorio, o.Cod_Area } into g
                                      select new ChaveArea {
                                         Cod_Escritorio = g.Key.Cod_Escritorio,
                                         Cod_Area = g.Key.Cod_Area
                                      }).ToList();

                        List<Tb_Area> _Cod_Areas = Cod_AreaDao.Get(chaves);

                        using (Conexao<Area> connNh = new Conexao<Area>(session))
                        {
                            foreach (ChaveArea chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Escritorio == chave.Cod_Escritorio && p.Cod_Area == chave.Cod_Area))
                                    {
                                        ExecutaAcao(connNh, _Cod_Areas, chave.Cod_Escritorio, chave.Cod_Area, acao.Acao);
                                    }

                                    Cod_AreaDao.AtualizacaoPv(chave.Cod_Escritorio, chave.Cod_Area);
                                }
                                catch (Exception ex)
                                {
                                    Cod_AreaDao.AtualizacaoPvErro(chave.Cod_Escritorio, chave.Cod_Area, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Area));
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

        private void ExecutaAcao(Conexao<Area> connNh, List<Tb_Area> Cod_Areas, String Cod_Escritorio, String Cod_Area, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Cod_Areas, Cod_Escritorio, Cod_Area, acao);
            }
            else
            {
                IList<Area> _listTmp = connNh.queryover()
                    .Where(p => p.Cod_Area == Cod_Area)
                    .JoinQueryOver(area => area.Escritorio)
                    .Where(area => area.Cod_Escritorio == Cod_Escritorio).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Area Pump(Conexao<Area> connNh, List<Tb_Area> Cod_Areas, String Cod_Escritorio, String Cod_Area, string acao)
        {
            Tb_Area area = Cod_Areas.Find(p => p.Cod_Escritorio == Cod_Escritorio && p.Cod_Area == Cod_Area);

            Area _tmp = new Area();

            //VERIFICA SE EXISTE
            IList<Area> _listTmp = connNh.queryover()
                    .Where(p => p.Cod_Area == Cod_Area)
                    .JoinQueryOver(g => g.Escritorio)
                    .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Escritorio = new Tb_EscritorioController(session).GetEscritorio(area.Cod_Escritorio);
            _tmp.Cod_Area = area.Cod_Area;
            _tmp.Descricao = area.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Area GetArea(String Cod_Escritorio, String Cod_Area)
        {
            if (!String.IsNullOrEmpty(Cod_Escritorio) && !String.IsNullOrEmpty(Cod_Area))
            {
                using (Conexao<Area> connCod_Area = new Conexao<Area>(session))
                {
                    List<Escritorio> _escritorios = new Conexao<Escritorio>(session).select(p => p.Cod_Escritorio == Cod_Escritorio);

                    Escritorio _escritorio = null;
                    if (_escritorios.Count == 0)
                        return null;

                    _escritorio = _escritorios[0];

                    IList<Area> _Cod_Areas = connCod_Area.queryover()
                        .Where(p=> p.Cod_Area == Cod_Area)
                        .JoinQueryOver(p=> p.Escritorio)
                        .Where(p=> p.Cod_Escritorio == Cod_Escritorio).List();

                    if (_Cod_Areas.Any())
                        return _Cod_Areas[0];
                    else
                    {
                        Area _area = new Area
                        {
                            Escritorio = _escritorio,
                            Cod_Area = Cod_Area
                        };
                        connCod_Area.Save(_area);
                        return _area;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoArea
    {
        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public String Acao { get; set; }
    }
}
