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
    public class Tb_Area_ModuloController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Area_ModuloController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Sigla_Modulo}/{Cod_Area}")]
        public Response Atualizar(String Sigla_Modulo, String Cod_Area)
         {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                //Conectou ao Aniel
                using (FbConnection conn = CreateConnection())
                {
                    //Construiu a Dao Aniel
                    using (Tb_Area_ModuloDao Cod_AreaDao = new Tb_Area_ModuloDao(conn, null))
                    {
                        //Pegou todos os protocolos não executados com base no filtro
                        List<Tb_Pv_Area_Modulo> protocolos = Cod_AreaDao.GetPv(Sigla_Modulo,Cod_Area);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        //Agrupa as acoes
                        var acoes = from o in protocolos
                                    group o by new { o.Sigla_Modulo, o.Cod_Area, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoArea_Modulo
                                    {
                                        Sigla_Modulo = g.Key.Sigla_Modulo,
                                        Cod_Area = g.Key.Cod_Area,
                                        Acao = g.Key.Acao
                                    };

                        //Agrupa as chaves
                        var chaves = (from o in protocolos
                                      group o by new { o.Sigla_Modulo, o.Cod_Area } into g
                                      select new ChaveArea_Modulo {
                                         Sigla_Modulo = g.Key.Sigla_Modulo,
                                         Cod_Area = g.Key.Cod_Area
                                      }).ToList();


                        List<Tb_Area_Modulo> _areas = Cod_AreaDao.Get(chaves);

                        //Conecta ao banco do connect
                        using (Conexao<Area_Modulo> connNh = new Conexao<Area_Modulo>(session))
                        {
                            //Loop em todas as alterações
                            foreach (ChaveArea_Modulo chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Sigla_Modulo == chave.Sigla_Modulo && p.Cod_Area == chave.Cod_Area))
                                    {
                                        ExecutaAcao(connNh, _areas, chave.Sigla_Modulo, chave.Cod_Area, acao.Acao);
                                    }

                                    Cod_AreaDao.AtualizacaoPv(chave.Sigla_Modulo, chave.Cod_Area);
                                }
                                catch (Exception ex)
                                {
                                    Cod_AreaDao.AtualizacaoPvErro(chave.Sigla_Modulo, chave.Cod_Area, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Area_Modulo));
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

        
        private void ExecutaAcao(Conexao<Area_Modulo> connNh, List<Tb_Area_Modulo> Areas, String Sigla_Modulo, String Cod_Area, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Areas, Sigla_Modulo, Cod_Area, acao);
            }
            else
            {
                List<Area_Modulo> _listTmp = connNh.select(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        
        private Area_Modulo Pump(Conexao<Area_Modulo> connNh, List<Tb_Area_Modulo> Areas, String Sigla_Modulo, String Cod_Area, string acao)
        {
            Tb_Area_Modulo _area = Areas.Find(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);

            Area_Modulo _tmp = new Area_Modulo();

            //VERIFICA SE EXISTE
            List<Area_Modulo> _listTmp = connNh.select(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Sigla_Modulo = _area.Sigla_Modulo;
            _tmp.Cod_Area = _area.Cod_Area;
            _tmp.Descricao = _area.Descricao;
            _tmp.Sub_Acesso = _area.Sub_Acesso;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Area_Modulo GetArea(String Sigla_Modulo, String Cod_Area)
        {
            if (!String.IsNullOrEmpty(Sigla_Modulo) && !String.IsNullOrEmpty(Cod_Area))
            {
                using (Conexao<Area_Modulo> connArea = new Conexao<Area_Modulo>(session))
                {
                    List<Area_Modulo> _Cod_Areas = connArea.select(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);
                    if (_Cod_Areas.Any())
                        return _Cod_Areas[0];
                    else
                    {
                        Area_Modulo area = new Area_Modulo
                        {
                            Sigla_Modulo = Sigla_Modulo,
                            Cod_Area = Cod_Area
                        };
                        connArea.Save(area);
                        return area;
                    }
                }
            }
            else 
                return null;
        }

    }

    public class AcaoArea_Modulo
    {
        public String Sigla_Modulo { get; set; }
        public String Cod_Area { get; set; }
        public String Acao { get; set; }
    }
}
