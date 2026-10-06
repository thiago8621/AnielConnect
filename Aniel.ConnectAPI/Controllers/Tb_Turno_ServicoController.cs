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
    public class Tb_Turno_ServicoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Turno_ServicoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Id_Turno_Servico}")]
        public Response Atualizar(Int32? Id_Turno_Servico)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Turno_ServicoDao Turno_ServicoDao = new Tb_Turno_ServicoDao(conn, null))
                    {
                        List<Tb_Pv_Turno_Servico> protocolos = Turno_ServicoDao.GetPv(Id_Turno_Servico);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id_Turno_Servico, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoTurno_Servico
                                    {
                                        Id_Turno_Servico = g.Key.Id_Turno_Servico,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id_Turno_Servico } into g
                                      select g.Key.Id_Turno_Servico).ToList();

                        List<Tb_Turno_Servico> _Turno_Servicos = Turno_ServicoDao.Get(chaves);

                        using (Conexao<Turno_Servico> connNh = new Conexao<Turno_Servico>(session))
                        {
                            foreach (Int32 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id_Turno_Servico == chave))
                                    {
                                        ExecutaAcao(connNh, _Turno_Servicos, chave, acao.Acao);
                                    }

                                    Turno_ServicoDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Turno_ServicoDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Turno_Servico));
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

        private void ExecutaAcao(Conexao<Turno_Servico> connNh, List<Tb_Turno_Servico> Turno_Servicos, Int32 Id_Turno_Servico, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Turno_Servicos, Id_Turno_Servico, acao);
            }
            else
            {
                List<Turno_Servico> _listTmp = connNh.select(p => p.Id == Id_Turno_Servico);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Turno_Servico Pump(Conexao<Turno_Servico> connNh, List<Tb_Turno_Servico> Turno_Servicos, Int32 Id_Turno_Servico, string acao)
        {
            Tb_Turno_Servico Turno_Servico = Turno_Servicos.Find(p => p.Id == Id_Turno_Servico);

            Turno_Servico _tmp = new Turno_Servico();

            //VERIFICA SE EXISTE
            List<Turno_Servico> _listTmp = connNh.select(p => p.Id == Id_Turno_Servico);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Id = Turno_Servico.Id;
            _tmp.Descricao = Turno_Servico.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Turno_Servico GetPerfil(Int16? Id_Turno_Servico)
        {
            if (Id_Turno_Servico.HasValue)
            {
                using (Conexao<Turno_Servico> connPerfil = new Conexao<Turno_Servico>(session))
                {
                    List<Turno_Servico> _perfis = connPerfil.select(p => p.Id == Id_Turno_Servico);
                    if (_perfis.Any())
                        return _perfis[0];
                    else
                    {
                        Turno_Servico perfil = new Turno_Servico
                        {
                            Id = Id_Turno_Servico.Value
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

    public class AcaoTurno_Servico
    {
        public Int32 Id_Turno_Servico { get; set; }
        public String Acao { get; set; }
    }

}
