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
    public class Tb_Status_Atend_Doc_ProdController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Status_Atend_Doc_ProdController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Status}")]
        public Response Atualizar(Int16? Status)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Status_Atend_Doc_ProdDao Status_Atend_Doc_ProdDao = new Tb_Status_Atend_Doc_ProdDao(conn, null))
                    {
                        List<Tb_Pv_St_Atend_Docprod> protocolos = Status_Atend_Doc_ProdDao.GetPv(Status);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Status, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoStatus_Atend_Doc_Prod
                                    {
                                        Status = g.Key.Status,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Status } into g
                                      select g.Key.Status).ToList();

                        List<Tb_Status_Atend_Doc_Prod> _Status_Atend_Doc_Prods = Status_Atend_Doc_ProdDao.Get(chaves);

                        using (Conexao<Status_Atend_Doc_Prod> connNh = new Conexao<Status_Atend_Doc_Prod>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Status == chave))
                                    {
                                        ExecutaAcao(connNh, _Status_Atend_Doc_Prods, chave, acao.Acao);
                                    }

                                    Status_Atend_Doc_ProdDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Status_Atend_Doc_ProdDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Status_Atend_Doc_Prod));
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

        private void ExecutaAcao(Conexao<Status_Atend_Doc_Prod> connNh, List<Tb_Status_Atend_Doc_Prod> Status_Atend_Doc_Prods, Int16 Status, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Status_Atend_Doc_Prods, Status, acao);
            }
            else
            {
                List<Status_Atend_Doc_Prod> _listTmp = connNh.select(p => p.Status == Status);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Status_Atend_Doc_Prod Pump(Conexao<Status_Atend_Doc_Prod> connNh, List<Tb_Status_Atend_Doc_Prod> Status_Atend_Doc_Prods, Int16 Status, string acao)
        {
            Tb_Status_Atend_Doc_Prod Status_Atend_Doc_Prod = Status_Atend_Doc_Prods.Find(p => p.Status == Status);

            Status_Atend_Doc_Prod _tmp = new Status_Atend_Doc_Prod();

            //VERIFICA SE EXISTE
            List<Status_Atend_Doc_Prod> _listTmp = connNh.select(p => p.Status == Status);
                if (_listTmp.Any())
                    _tmp = _listTmp.First();
            

            _tmp.Status = Status_Atend_Doc_Prod.Status;
            _tmp.Descricao = Status_Atend_Doc_Prod.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Status_Atend_Doc_Prod GetStatus_Atend_Doc_Prod(Int16? Status)
        {
            if (Status.HasValue)
            {
                using (Conexao<Status_Atend_Doc_Prod> connStatus_Atend_Doc_Prod = new Conexao<Status_Atend_Doc_Prod>(session))
                {
                    List<Status_Atend_Doc_Prod> _Status_Atend_Doc_Prods = connStatus_Atend_Doc_Prod.select(p => p.Status == Status);
                    if (_Status_Atend_Doc_Prods.Any())
                        return _Status_Atend_Doc_Prods[0];
                    else
                    {
                        Status_Atend_Doc_Prod Status_Atend_Doc_Prod = new Status_Atend_Doc_Prod
                        {
                            Status = Status.Value
                        };
                        connStatus_Atend_Doc_Prod.Save(Status_Atend_Doc_Prod);
                        return Status_Atend_Doc_Prod;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoStatus_Atend_Doc_Prod
    {
        public Int16 Status { get; set; }
        public String Acao { get; set; }
    }
}
