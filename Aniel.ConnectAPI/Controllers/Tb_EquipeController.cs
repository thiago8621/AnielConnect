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
using Aniel.ConnectAPI.Dal;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class Tb_EquipeController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_EquipeController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Equipe}")]
        public Response Atualizar(String Equipe)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_EquipeDao equipeDao = new Tb_EquipeDao(conn, null))
                    {
                        List<Tb_Pv_Equipe> protocolos = equipeDao.GetPv(Equipe);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Equipe, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoEquipe
                                    {
                                        Equipe = g.Key.Equipe,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Equipe } into g
                                      select g.Key.Equipe).ToList();

                        List<Tb_Equipe> _equipes = equipeDao.Get(chaves);

                        using (Conexao<Equipe> connNh = new Conexao<Equipe>(session))
                        {
                            foreach (String matricula in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Equipe == matricula))
                                    {
                                        ExecutaAcao(connNh, _equipes, matricula, acao.Acao);
                                    }

                                    equipeDao.AtualizacaoPv(matricula);
                                }
                                catch (Exception ex)
                                {
                                    equipeDao.AtualizacaoPvErro(matricula, ex.Message);
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
            return Atualizar(String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Equipe));
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

        private void ExecutaAcao(Conexao<Equipe> connNh, List<Tb_Equipe> equipes, String matricula, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, equipes, matricula, acao);
            }
            else
            {
                List<Equipe> _listTmp = connNh.select(p => p.Matricula == matricula);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Equipe Pump(Conexao<Equipe> connNh, List<Tb_Equipe> equipes, String matricula, String acao)
        {
            Tb_Equipe equipe = equipes.Find(p => p.Equipe == matricula);

            Equipe _tmp = new Equipe();

            //VERIFICA SE EXISTE
            List<Equipe> _listTmp = connNh.select(p => p.Matricula == matricula);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Encerramento = equipe.Encerramento;
            _tmp.EquipeAux = equipe.EquipeAux;
            _tmp.Matricula = equipe.Equipe;
            _tmp.Nome = equipe.Nome;

            _tmp.Funcionario = new Tb_FuncionarioController(session).GetFuncionario(equipe.Num_Fre);
            _tmp.Supervisor = new Tb_FuncionarioController(session).GetFuncionario(equipe.Cod_Superv);
            _tmp.Perfil_Equipe = new Tb_Perfil_EquipeController(session).GetPerfil(equipe.Cod_Perfil_Equipe);
            _tmp.Regiao = new Tb_RegiaoController(session).GetRegiao(equipe.Cod_Regiao);

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Equipe GetEquipe(String Equipe)
        {
            if (!String.IsNullOrEmpty(Equipe))
            {
                using (Conexao<Equipe> connEquipe = new Conexao<Equipe>(session))
                {
                    List<Equipe> _Equipes = connEquipe.select(p => p.Matricula == Equipe);
                    if (_Equipes.Any())
                        return _Equipes[0];
                    else
                    {
                        Equipe equipe = new Equipe
                        {
                            Matricula = Equipe
                        };
                        connEquipe.Save(equipe);
                        return equipe;
                    }
                }
            }
            else
                return null;
        }
    }

    public class AcaoEquipe
    {
        public String Equipe { get; set; }
        public String Acao { get; set; }
    }
}
