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
    public class Tb_DepartamentoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_DepartamentoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{CodDepartamento}")]
        public async Task<Response> Atualizar(Int16? Cod_Depto)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_DepartamentoDao DepartamentoDao = new Tb_DepartamentoDao(conn, null))
                    {
                        List<Tb_Pv_Departamento> protocolos = DepartamentoDao.GetPv(Cod_Depto);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Depto, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoDepartamento
                                    {
                                        Cod_Depto = g.Key.Cod_Depto,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Depto } into g
                                      select g.Key.Cod_Depto).ToList();

                        List<Tb_Folha_Depto> _Departamentos = DepartamentoDao.Get(chaves);

                        if (_Departamentos.Count > 0)
                        {
                            using (Conexao<Departamento> connNh = new Conexao<Departamento>(session))
                            {
                                foreach (Int16 chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Cod_Depto == chave))
                                        {
                                            await ExecutaAcao(connNh, _Departamentos, chave, acao.Acao);
                                        }

                                        DepartamentoDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        DepartamentoDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
                                    }
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
        public async Task<Response> AtualizarTodos()
        {
            return await Atualizar(null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Folha_Depto));
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

        private async Task ExecutaAcao(Conexao<Departamento> connNh, List<Tb_Folha_Depto> Departamentos, Int16 Cod_Depto, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Departamentos, Cod_Depto, acao);
            }
            else
            {
                IList<Departamento> _listTmp = await connNh.select(p => p.Cod_Depto == Cod_Depto);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Departamento> Pump(Conexao<Departamento> connNh, List<Tb_Folha_Depto> Departamentos, Int16 Cod_Depto, string acao)
        {
            Tb_Folha_Depto Departamento = Departamentos.Find(p => p.Cod_DEPTO == Cod_Depto);

            Departamento _tmp = new Departamento();

            //VERIFICA SE EXISTE
            IList<Departamento> _listTmp = await connNh.select(p => p.Cod_Depto == Cod_Depto);
            if (_listTmp.Any())
                _tmp = _listTmp.First();

            _tmp.Id = Departamento.Cod_DEPTO;
            _tmp.Cod_Depto = Departamento.Cod_DEPTO;
            _tmp.Descricao = Departamento.Descricao;
            _tmp.Num_Depto = Departamento.Num_DEPTO;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Departamento> GetDepartamento(Int16? Cod_Depto, String Depto)
        {
            if (Cod_Depto.HasValue)
            {
                using (Conexao<Departamento> connDepartamento = new Conexao<Departamento>(session))
                {
                    IList<Departamento> _Departamentos = await connDepartamento.select(p => p.Id == Cod_Depto);
                    if (_Departamentos.Count > 0)
                        return _Departamentos[0];
                    else
                    {
                        Departamento Departamento = new Departamento
                        {
                            Id = Cod_Depto.Value,
                            Cod_Depto = Cod_Depto.Value,
                            Descricao = Depto
                        };
                        await connDepartamento.Save(Departamento);
                        return Departamento;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoDepartamento
    {
        public Int16 Cod_Depto { get; set; }
        public String Acao { get; set; }
    }
}
