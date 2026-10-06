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
    public class Tb_Classif_QualidController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Classif_QualidController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Id}")]
        public Response Atualizar(Int32? Id)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Classif_QualidDao Classificacao_QualidadeDao = new Tb_Classif_QualidDao(conn, null))
                    {
                        List<Tb_Pv_Classif_Qualid> protocolos = Classificacao_QualidadeDao.GetPv(Id);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id_Classif, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoClassif_Qualid
                                    {
                                        Id = g.Key.Id_Classif,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id_Classif } into g
                                      select g.Key.Id_Classif).ToList();

                        List<Tb_Classificacao_Qualidade> _Classificacao_Qualidades = Classificacao_QualidadeDao.Get(chaves);

                        using (Conexao<Classificacao_Qualidade> connNh = new Conexao<Classificacao_Qualidade>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id == chave))
                                    {
                                        ExecutaAcao(connNh, _Classificacao_Qualidades, chave, acao.Acao);
                                    }

                                    Classificacao_QualidadeDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Classificacao_QualidadeDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Classificacao_Qualidade));
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

        private void ExecutaAcao(Conexao<Classificacao_Qualidade> connNh, List<Tb_Classificacao_Qualidade> Classificacao_Qualidades, Int16 Id, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Classificacao_Qualidades, Id, acao);
            }
            else
            {
                List<Classificacao_Qualidade> _listTmp = connNh.select(p => p.Id == Id);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Classificacao_Qualidade Pump(Conexao<Classificacao_Qualidade> connNh, List<Tb_Classificacao_Qualidade> Classificacao_Qualidades, Int16 Id, string acao)
        {
            Tb_Classificacao_Qualidade Classificacao_Qualidade = Classificacao_Qualidades.Find(p => p.Id == Id);

            Classificacao_Qualidade _tmp = new Classificacao_Qualidade();

            Boolean novo = true;

            //VERIFICA SE EXISTE
            List<Classificacao_Qualidade> _listTmp = connNh.select(p => p.Id == Id);
            if (_listTmp.Any())
            { 
               _tmp = _listTmp.First();
               novo = false;
            }

            _tmp.Id = Classificacao_Qualidade.Id;
            _tmp.Qualidade = Classificacao_Qualidade.Qualidade;

            if (novo)
                connNh.Save(_tmp);
            else
                connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Classificacao_Qualidade GetClassificacao_Qualidade(Int16? Id)
        {
            if (Id.HasValue)
            {
                using (Conexao<Classificacao_Qualidade> connClassificacao_Qualidade = new Conexao<Classificacao_Qualidade>(session))
                {
                    List<Classificacao_Qualidade> _Classificacao_Qualidades = connClassificacao_Qualidade.select(p => p.Id == Id);
                    if (_Classificacao_Qualidades.Any())
                        return _Classificacao_Qualidades[0];
                    else
                    {
                        Classificacao_Qualidade Classificacao_Qualidade = new Classificacao_Qualidade
                        {
                            Id = Id.Value
                        };
                        connClassificacao_Qualidade.Save(Classificacao_Qualidade);
                        return Classificacao_Qualidade;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoClassif_Qualid
    {
        public Int32 Id { get; set; }
        public String Acao { get; set; }
    }
}
