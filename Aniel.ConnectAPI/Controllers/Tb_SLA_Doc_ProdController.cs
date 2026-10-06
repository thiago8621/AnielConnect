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
    public class Tb_SLA_Doc_ProdController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_SLA_Doc_ProdController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Id}")]
        public Response Atualizar(Int64? Id)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_SLA_Doc_ProdDao SLADao = new Tb_SLA_Doc_ProdDao(conn, null))
                    {
                        List<Tb_Pv_SLA_Doc_Prod> protocolos = SLADao.GetPv(Id);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoSLA
                                    {
                                        Id = g.Key.Id,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id } into g
                                      select g.Key.Id).ToList();

                        List<Tb_SLA_Doc_Prod> _SLA_Doc_Prods = SLADao.Get(chaves);

                        using (Conexao<SLA_Doc_Prod> connNh = new Conexao<SLA_Doc_Prod>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id == chave))
                                    {
                                        ExecutaAcao(connNh, _SLA_Doc_Prods, chave, acao.Acao);
                                    }

                                    SLADao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    SLADao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_SLA_Doc_Prod));
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

        private void ExecutaAcao(Conexao<SLA_Doc_Prod> connNh, List<Tb_SLA_Doc_Prod> SLA_Doc_Prods, Int64 Id, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, SLA_Doc_Prods, Id, acao);
            }
            else
            {
                List<SLA_Doc_Prod> _listTmp = connNh.select(p => p.Id == Id);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private SLA_Doc_Prod Pump(Conexao<SLA_Doc_Prod> connNh, List<Tb_SLA_Doc_Prod> SLA_Doc_Prods, Int64 Id, string acao)
        {
            Tb_SLA_Doc_Prod SLA_Doc_Prod = SLA_Doc_Prods.Find(p => p.Id == Id);

            SLA_Doc_Prod _tmp = new SLA_Doc_Prod();

            //VERIFICA SE EXISTE
            List<SLA_Doc_Prod> _listTmp = connNh.select(p => p.Id == Id);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Categoria_Cliente = new Tb_Categoria_ClienteController(session).GetCategoria(SLA_Doc_Prod.Id_Categoria);
            _tmp.Criticidade = new Tb_Criticidade_Doc_ProdController(session).GetCriticidade_Doc_Prod(SLA_Doc_Prod.Id_Criticidade);
            _tmp.Tipo_Servico = new Tb_Tipo_Serv_EquipeController(session).GetTipo_Servico_Equipe(SLA_Doc_Prod.Cod_Tipo_Serv);
            _tmp.Porc_Ini = SLA_Doc_Prod.Porc_Ini;
            _tmp.Porc_Fim = SLA_Doc_Prod.Porc_Ini;
            _tmp.Cor = SLA_Doc_Prod.Cor;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public SLA_Doc_Prod GetSLA_Doc_Prod(Int64? Id)
        {
            if (Id.HasValue)
            {
                using (Conexao<SLA_Doc_Prod> connSLA_Doc_Prod = new Conexao<SLA_Doc_Prod>(session))
                {
                    List<SLA_Doc_Prod> _SLA_Doc_Prod = connSLA_Doc_Prod.select(p => p.Id == Id);
                    if (_SLA_Doc_Prod.Any())
                        return _SLA_Doc_Prod[0];
                    else
                    {
                        SLA_Doc_Prod SLA_Doc_Prod = new SLA_Doc_Prod
                        {
                            Id = Id.Value
                        };
                        connSLA_Doc_Prod.Save(SLA_Doc_Prod);
                        return SLA_Doc_Prod;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoSLA
    {
        public Int64 Id { get; set; }
        public String Acao { get; set; }
    }
}
