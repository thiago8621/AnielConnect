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
    public class Tb_EmpresaController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_EmpresaController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Codempresa}")]
        public async Task<Response> Atualizar(Int16? Codempresa)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_EmpresaDao EmpresaDao = new Tb_EmpresaDao(conn, null))
                    {
                        List<Tb_Pv_Empresa> protocolos = EmpresaDao.GetPv(Codempresa);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codempresa, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoEmpresa
                                    {
                                        Codempresa = g.Key.Codempresa,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codempresa } into g
                                      select g.Key.Codempresa).ToList();

                        List<Tb_Empresa> _Empresas = EmpresaDao.Get(chaves);

                        if (_Empresas.Count > 0)
                        {
                            using (Conexao<Empresa> connNh = new Conexao<Empresa>(session))
                            {
                                foreach (Int16 chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Codempresa == chave))
                                        {
                                            await ExecutaAcao(connNh, _Empresas, chave, acao.Acao);
                                        }

                                        EmpresaDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        EmpresaDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
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
                        dao.AdicionaRegistroPV(typeof(Tb_Empresa));
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

        private async Task ExecutaAcao(Conexao<Empresa> connNh, List<Tb_Empresa> Empresas, Int16 Codempresa, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Empresas, Codempresa, acao);
            }
            else
            {
                IList<Empresa> _listTmp = await connNh.select(p => p.CodEmpresa == Codempresa);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Empresa> Pump(Conexao<Empresa> connNh, List<Tb_Empresa> Empresas, Int16 Codempresa, string acao)
        {
            Tb_Empresa Empresa = Empresas.Find(p => p.Codempresa == Codempresa);

            Empresa _tmp = new Empresa();

            //VERIFICA SE EXISTE
            IList<Empresa> _listTmp = await connNh.select(p => p.CodEmpresa == Codempresa);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            
            _tmp.Id = Empresa.Codempresa;
            _tmp.CodEmpresa = Empresa.Codempresa;
            _tmp.Razao_Social = Empresa.Razao_Social;
            _tmp.Nome = Empresa.Empresa;
            _tmp.Logotipo = Empresa.Logotipo;
            _tmp.Data_Alteracao = Empresa.Data_Alteracao;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Empresa> GetEmpresa(Int16? Codempresa)
        {
            if (Codempresa.HasValue && Codempresa.Value > 0)
            {
                using (Conexao<Empresa> connEmpresa = new Conexao<Empresa>(session))
                {
                    IList<Empresa> _Empresas = await connEmpresa.select(p => p.CodEmpresa == Codempresa);
                    if (_Empresas.Any())
                        return _Empresas[0];
                    else
                    {
                        Empresa Empresa = new Empresa
                        {
                            CodEmpresa = Codempresa.Value
                        };
                        await connEmpresa.Save(Empresa);
                        return Empresa;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoEmpresa
    {
        public Int16 Codempresa { get; set; }
        public String Acao { get; set; }
    }
}
