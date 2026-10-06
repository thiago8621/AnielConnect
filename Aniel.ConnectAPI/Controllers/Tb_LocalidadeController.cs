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
    public class Tb_LocalidadeController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_LocalidadeController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Escritorio}/{Cod_Area}/{Localidade}")]
        public Response Atualizar(String Cod_Escritorio, String Cod_Area, String Localidade)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_LocalidadeDao LocalidadeDao = new Tb_LocalidadeDao(conn, null))
                    {
                        List<Tb_Pv_Localidade> protocolos = LocalidadeDao.GetPv(Cod_Escritorio, Cod_Area, Localidade);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Escritorio, o.Cod_Area, o.Localidade, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoLocalidade
                                    {
                                        Cod_Escritorio = g.Key.Cod_Escritorio,
                                        Cod_Area = g.Key.Cod_Area,
                                        Localidade = g.Key.Localidade,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Escritorio, o.Cod_Area, o.Localidade } into g
                                      select new ChaveLocalidade {
                                         Cod_Escritorio = g.Key.Cod_Escritorio,
                                         Cod_Area = g.Key.Cod_Area,
                                         Localidade = g.Key.Localidade
                                      }).ToList();

                        List<Tb_Localidade> _localidades = LocalidadeDao.Get(chaves);

                        using (Conexao<Localidade> connNh = new Conexao<Localidade>(session))
                        {
                            foreach (ChaveLocalidade chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Escritorio == chave.Cod_Escritorio && p.Cod_Area == chave.Cod_Area && p.Localidade == chave.Localidade))
                                    {
                                        ExecutaAcao(connNh, _localidades, chave.Cod_Escritorio, chave.Cod_Area, chave.Localidade, acao.Acao);
                                    }

                                    LocalidadeDao.AtualizacaoPv(chave.Cod_Escritorio, chave.Cod_Area, chave.Localidade);
                                }
                                catch (Exception ex)
                                {
                                    LocalidadeDao.AtualizacaoPvErro(chave.Cod_Escritorio, chave.Cod_Area, chave.Localidade, ex.Message);
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
            return Atualizar(String.Empty, String.Empty, String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Localidade));
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

        private void ExecutaAcao(Conexao<Localidade> connNh, List<Tb_Localidade> Localidades, String Cod_Escritorio, String Cod_Area, String Localidade, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Localidades, Cod_Escritorio, Cod_Area, Localidade, acao);
            }
            else
            {
                IList<Localidade> _listTmp = connNh.queryover()
                    .Where(p =>  p.LocalidadeStr == Localidade)
                    .JoinQueryOver(p => p.Area)
                    .Where(p => p.Cod_Area == Cod_Area)
                    .JoinQueryOver(p => p.Escritorio)
                    .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Localidade Pump(Conexao<Localidade> connNh, List<Tb_Localidade> Localidades, String Cod_Escritorio, String Cod_Area, String Localidade, string acao)
        {
            Tb_Localidade _localidade = Localidades.Find(p => p.Cod_Escritorio == Cod_Escritorio && p.Cod_Area == Cod_Area && p.Localidade == Localidade);

            Localidade _tmp = new Localidade();

            //VERIFICA SE EXISTE
            IList<Localidade> _listTmp = connNh.queryover()
                    .Where(p => p.LocalidadeStr == Localidade)
                    .JoinQueryOver(p => p.Area)
                    .Where(p => p.Cod_Area == Cod_Area)
                    .JoinQueryOver(p => p.Escritorio)
                    .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Area = new Tb_AreaController(session).GetArea(_localidade.Cod_Escritorio, _localidade.Cod_Area);
            _tmp.Cod_Localidade = _localidade.Cod_Localidade;
            _tmp.LocalidadeStr = _localidade.Localidade;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet("GetLocalidade")]
        public Localidade GetLocalidade(String Cod_Escritorio, String Cod_Area, String Localidade)
        {
            if (!String.IsNullOrEmpty(Cod_Escritorio) && !String.IsNullOrEmpty(Cod_Area) && !String.IsNullOrEmpty(Localidade))
            {
                using (Conexao<Localidade> connLocalidade = new Conexao<Localidade>(session))
                {
                    IList<Area> _areas = new Conexao<Area>(session).queryover()
                        .Where(p => p.Cod_Area == Cod_Area)
                        .JoinQueryOver(p=>p.Escritorio)
                        .Where(p=> p.Cod_Escritorio == Cod_Escritorio).List();

                    Area _area = null;
                    if (_areas.Count == 0)
                        return null;

                    _area = _areas[0];

                    IList<Localidade> _localidades = connLocalidade.queryover()
                        .Where(p=> p.LocalidadeStr == Localidade)
                        .JoinQueryOver(p=>p.Area)
                        .Where(p=> p.Cod_Area == Cod_Area)
                        .JoinQueryOver(p=>p.Escritorio)
                        .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

                    if (_localidades.Any())
                        return _localidades[0];
                    else
                    {
                        Localidade _localidade = new Localidade
                        {
                            Area = _area,
                            LocalidadeStr = Localidade
                        };
                        connLocalidade.Save(_localidade);
                        return _localidade;
                    }
                }
            }
            else
                return null;
        }

        [HttpGet("GetLocalidadeInt")]
        public Localidade GetLocalidade(String Cod_Escritorio, String Cod_Area, Int32? Cod_Localidade)
        {
            if (!String.IsNullOrEmpty(Cod_Escritorio) && !String.IsNullOrEmpty(Cod_Area) && Cod_Localidade.HasValue)
            {
                using (Conexao<Localidade> connLocalidade = new Conexao<Localidade>(session))
                {
                    IList<Area> _areas = new Conexao<Area>(session).queryover()
                        .Where(p => p.Cod_Area == Cod_Area)
                        .JoinQueryOver(p => p.Escritorio)
                        .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

                    Area _area = null;
                    if (_areas.Count == 0)
                        return null;

                    _area = _areas[0];

                    IList<Localidade> _localidades = connLocalidade.queryover()
                        .Where(p => p.Cod_Localidade == Cod_Localidade)
                        .JoinQueryOver(p => p.Area)
                        .Where(p => p.Cod_Area == Cod_Area)
                        .JoinQueryOver(p => p.Escritorio)
                        .Where(p => p.Cod_Escritorio == Cod_Escritorio).List();

                    if (_localidades.Any())
                        return _localidades[0];
                    else
                    {
                        Localidade _localidade = new Localidade
                        {
                            Area = _area,
                            Cod_Localidade = Cod_Localidade.Value
                        };
                        connLocalidade.Save(_localidade);
                        return _localidade;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoLocalidade
    {
        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public String Localidade { get; set; }
        public String Acao { get; set; }
    }
}
