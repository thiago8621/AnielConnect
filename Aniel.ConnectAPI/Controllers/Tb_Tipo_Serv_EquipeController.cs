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
    public class Tb_Tipo_Serv_EquipeController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Tipo_Serv_EquipeController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Tipo_Serv}")]
        public Response Atualizar(Int16? Cod_Tipo_Serv)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Tipo_Servico_EquipeDao Tipo_Servico_EquipeDao = new Tb_Tipo_Servico_EquipeDao(conn, null))
                    {
                        List<Tb_Pv_Tp_Serv_Equipe> protocolos = Tipo_Servico_EquipeDao.GetPv(Cod_Tipo_Serv);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Tipo_Serv, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoTipo_Serv_Equipe
                                    {
                                        Cod_Tipo_Serv = g.Key.Cod_Tipo_Serv,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Tipo_Serv } into g
                                      select g.Key.Cod_Tipo_Serv).ToList();

                        List<Tb_Tipo_Servico_Equipe> _Tipo_Servico_Equipes = Tipo_Servico_EquipeDao.Get(chaves);

                        using (Conexao<Tipo_Servico_Equipe> connNh = new Conexao<Tipo_Servico_Equipe>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Tipo_Serv == chave))
                                    {
                                        ExecutaAcao(connNh, _Tipo_Servico_Equipes, chave, acao.Acao);
                                    }

                                    Tipo_Servico_EquipeDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Tipo_Servico_EquipeDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Tipo_Servico_Equipe));
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

        private void ExecutaAcao(Conexao<Tipo_Servico_Equipe> connNh, List<Tb_Tipo_Servico_Equipe> Tipo_Servico_Equipes, Int16 Cod_Tipo_Serv, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Tipo_Servico_Equipes, Cod_Tipo_Serv, acao);
            }
            else
            {
                List<Tipo_Servico_Equipe> _listTmp = connNh.select(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Tipo_Servico_Equipe Pump(Conexao<Tipo_Servico_Equipe> connNh, List<Tb_Tipo_Servico_Equipe> Tipo_Servico_Equipes, Int16 Cod_Tipo_Serv, string acao)
        {
            Tb_Tipo_Servico_Equipe Tipo_Servico_Equipe = Tipo_Servico_Equipes.Find(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv);

            Tipo_Servico_Equipe _tmp = new Tipo_Servico_Equipe();

            //VERIFICA SE EXISTE
            List<Tipo_Servico_Equipe> _listTmp = connNh.select(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Tipo_Serv = Tipo_Servico_Equipe.Cod_Tipo_Serv;
            _tmp.Descricao = Tipo_Servico_Equipe.Descricao;
            _tmp.Descricao_Aux = Tipo_Servico_Equipe.Descricao_Aux;
            _tmp.Cod_Tipo_Serv = Tipo_Servico_Equipe.Cod_Tipo_Serv;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Tipo_Servico_Equipe GetTipo_Servico_Equipe(Int32? Cod_Tipo_Serv)
        {
            if (Cod_Tipo_Serv.HasValue)
            {
                using (Conexao<Tipo_Servico_Equipe> connTipo_Servico_Equipe = new Conexao<Tipo_Servico_Equipe>(session))
                {
                    List<Tipo_Servico_Equipe> _Tipo_Servico_Equipes = connTipo_Servico_Equipe.select(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv);
                    if (_Tipo_Servico_Equipes.Any())
                        return _Tipo_Servico_Equipes[0];
                    else
                    {
                        Tipo_Servico_Equipe Tipo_Servico_Equipe = new Tipo_Servico_Equipe
                        {
                            Cod_Tipo_Serv = Cod_Tipo_Serv.Value
                        };
                        connTipo_Servico_Equipe.Save(Tipo_Servico_Equipe);
                        return Tipo_Servico_Equipe;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoTipo_Serv_Equipe
    {
        public Int16 Cod_Tipo_Serv { get; set; }
        public String Acao { get; set; }
    }
}
