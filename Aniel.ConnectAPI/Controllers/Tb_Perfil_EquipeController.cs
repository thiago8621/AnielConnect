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
    public class Tb_Perfil_EquipeController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Perfil_EquipeController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Perfil_Equipe}")]
        public Response Atualizar(Int32? Cod_Perfil_Equipe)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Perfil_EquipeDao Perfil_EquipeDao = new Tb_Perfil_EquipeDao(conn, null))
                    {
                        List<Tb_Pv_Perfil_Equipe> protocolos = Perfil_EquipeDao.GetPv(Cod_Perfil_Equipe);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Perfil_Equipe, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoPerfil_Equipe
                                    {
                                        Cod_Perfil_Equipe = g.Key.Cod_Perfil_Equipe,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Perfil_Equipe } into g
                                      select g.Key.Cod_Perfil_Equipe).ToList();

                        List<Tb_Perfil_Equipe> _Perfil_Equipes = Perfil_EquipeDao.Get(chaves);

                        using (Conexao<Perfil_Equipe> connNh = new Conexao<Perfil_Equipe>(session))
                        {
                            foreach (Int32 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Perfil_Equipe == chave))
                                    {
                                        ExecutaAcao(connNh, _Perfil_Equipes, chave, acao.Acao);
                                    }

                                    Perfil_EquipeDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Perfil_EquipeDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Perfil_Equipe));
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


        private void ExecutaAcao(Conexao<Perfil_Equipe> connNh, List<Tb_Perfil_Equipe> Perfil_Equipes, Int32 Cod_Perfil_Equipe, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Perfil_Equipes, Cod_Perfil_Equipe, acao);
            }
            else
            {
                List<Perfil_Equipe> _listTmp = connNh.select(p => p.Cod_Perfil_Equipe == Cod_Perfil_Equipe);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Perfil_Equipe Pump(Conexao<Perfil_Equipe> connNh, List<Tb_Perfil_Equipe> Perfil_Equipes, Int32 Cod_Perfil_Equipe, string acao)
        {
            Tb_Perfil_Equipe Perfil_Equipe = Perfil_Equipes.Find(p => p.Cod_Perfil_Equipe == Cod_Perfil_Equipe);

            Perfil_Equipe _tmp = new Perfil_Equipe();

            //VERIFICA SE EXISTE
            List<Perfil_Equipe> _listTmp = connNh.select(p => p.Cod_Perfil_Equipe == Cod_Perfil_Equipe);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Perfil_Equipe = Perfil_Equipe.Cod_Perfil_Equipe;
            _tmp.Nome = Perfil_Equipe.Nome;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Perfil_Equipe GetPerfil(Int16? Cod_Perfil_Equipe)
        {
            if (Cod_Perfil_Equipe.HasValue)
            {
                using (Conexao<Perfil_Equipe> connPerfil = new Conexao<Perfil_Equipe>(session))
                {
                    List<Perfil_Equipe> _perfis = connPerfil.select(p => p.Cod_Perfil_Equipe == Cod_Perfil_Equipe);
                    if (_perfis.Any())
                        return _perfis[0];
                    else
                    {
                        Perfil_Equipe perfil = new Perfil_Equipe
                        {
                            Cod_Perfil_Equipe = Cod_Perfil_Equipe.Value
                        };
                        connPerfil.Save(perfil);
                        return perfil;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoPerfil_Equipe
    {
        public Int32 Cod_Perfil_Equipe { get; set; }
        public String Acao { get; set; }
    }

}
