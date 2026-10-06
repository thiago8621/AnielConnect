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
    public class Tb_FilialController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_FilialController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Codempresa}/{Codfilial}")]
        public async Task<Response> Atualizar(Int16? Codempresa, Int16? Codfilial)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_FilialDao filialDao = new Tb_FilialDao(conn, null))
                    {
                        List<Tb_Pv_Filial> protocolos = filialDao.GetPv(Codempresa, Codfilial);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codempresa, o.Codfilial, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoFilial
                                    {
                                        Codempresa = g.Key.Codempresa,
                                        Codfilial = g.Key.Codfilial,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codempresa, o.Codfilial } into g
                                      select new ChaveFilial {
                                          Codempresa = g.Key.Codempresa,
                                          Codfilial = g.Key.Codfilial
                                      }).ToList();

                        List<Tb_Filial> _filiais = filialDao.Get(chaves);

                        if (_filiais.Count > 0)
                        {
                            using (Conexao<Filial> connNh = new Conexao<Filial>(session))
                            {
                                foreach (var tb_filial in _filiais.FindAll(p => !p.Id_Filial_Connect.HasValue))
                                {
                                    IList<Filial> _listTmp = connNh.queryover()
                                            .Where(p => p.CodFilial == tb_filial.Codfilial)
                                            .JoinQueryOver(p => p.Empresa)
                                            .Where(p => p.CodEmpresa == tb_filial.Codempresa).List();

                                    if (_listTmp.Any())
                                        filialDao.AtualizacaoIdConnect(tb_filial.Codempresa, tb_filial.Codfilial, _listTmp[0].Id);
                                }

                                foreach (ChaveFilial chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Codempresa == chave.Codempresa && p.Codfilial == chave.Codfilial))
                                        {
                                            await ExecutaAcao(filialDao, connNh, _filiais, chave.Codempresa, chave.Codfilial, acao.Acao);
                                        }

                                        filialDao.AtualizacaoPv(chave.Codempresa, chave.Codfilial);
                                    }
                                    catch (Exception ex)
                                    {
                                        filialDao.AtualizacaoPvErro(chave.Codempresa, chave.Codfilial, $"{ex.Message}\n{ex.InnerException?.Message}");
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
            return await Atualizar(null, null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Filial));
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

        private async Task ExecutaAcao(Tb_FilialDao dao, Conexao<Filial> connNh, List<Tb_Filial> filiais, Int16 codempresa, Int16 codfilial, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(dao, connNh, filiais, codempresa, codfilial, acao);
            }
            else
            {
                IList<Filial> _listTmp = connNh.queryover()
                    .Where(p=> p.CodFilial == codfilial)
                    .JoinQueryOver(p=>p.Empresa)
                    .Where(p => p.CodEmpresa == codempresa).List();

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Filial> Pump(Tb_FilialDao dao, Conexao<Filial> connNh, List<Tb_Filial> filiais, Int16 codempresa, Int16 codfilial, string acao)
        {
            Tb_Filial filial = filiais.Find(p => p.Codempresa == codempresa && p.Codfilial == codfilial);

            Filial _tmp = new Filial();

            //VERIFICA SE EXISTE
            IList<Filial> _listTmp = connNh.queryover()
                    .Where(p=> p.CodFilial == codfilial)
                    .JoinQueryOver(p=>p.Empresa)
                    .Where(p => p.CodEmpresa == codempresa).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Empresa = await new Tb_EmpresaController(session).GetEmpresa(filial.Codempresa);
            _tmp.CodFilial = filial.Codfilial;
            _tmp.Nome = filial.Nome;
            _tmp.Cnpj = filial.Cnpj;
            _tmp.CodigoCedente = filial.CodigoCedente;
            _tmp.DigitoVerificadorCedente = filial.DigitoVerificadorCedente;
            _tmp.Carteira = filial.Carteira;

            _tmp.Endereco = filial.Endereco;
            _tmp.Numero = filial.Numero;
            _tmp.Cidade = filial.Cidade;
            _tmp.Bairro = filial.Bairro;
            _tmp.Cep = filial.Cep;
            _tmp.Uf = filial.Uf;
            _tmp.Natureza_tributacao = filial.Natureza_tributacao;
            _tmp.Razao_Social = filial.Razao_Social;
            _tmp.Optante_Simples_Nacional = filial.Optante_Simples_Nacional;
            _tmp.Regime_Tributario = filial.Regime_Tributario;
            _tmp.Incentivo_Cultural = filial.Incentivo_Cultural;
            _tmp.Incentivo_Fiscal = filial.Incentivo_Fiscal;
            _tmp.Regime_Tributario_Especial = filial.Regime_Tributario_Especial;
            _tmp.DDD = filial.DDD;
            _tmp.Email = filial.Email;
            _tmp.Tipo_Tributacao_ISS = filial.Tipo_Tributacao_ISS;
            _tmp.Exigibilidade_ISS = filial.Exigibilidade_ISS;
            _tmp.Num_Processo_Suspensao = filial.Num_Processo_Suspensao;
            _tmp.CNAE_Fiscal = filial.CNAE_Fiscal;

            _tmp.Inscricao_Estadual = filial.Inscricao_Estadual;
            _tmp.Inscricao_Municipal = filial.Inscricao_Municipal;
            _tmp.Telefone = filial.Telefone;


            await connNh.SaveOrUpdate(_tmp);

            dao.AtualizacaoIdConnect(filial.Codempresa, filial.Codfilial, _tmp.Id);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Filial> GetFilial(Int16? CodEmpresa, Int16? CodFilial)
        {
            if (CodEmpresa.HasValue && CodFilial.HasValue && CodEmpresa.Value > 0 && CodFilial.Value > 0)
            {
                using (Conexao<Filial> connFilial = new Conexao<Filial>(session))
                {
                    IList<Empresa> _empresas = await new Conexao<Empresa>(session).select(p => p.CodEmpresa == CodEmpresa);
                    Empresa _empresa = null;
                    if (_empresas.Count == 0)
                        return null;

                    _empresa = _empresas[0];

                    IList<Filial> _filial = connFilial.queryover()
                        .Where(p => p.CodFilial == CodFilial)
                        .JoinQueryOver(p => p.Empresa)
                        .Where(p => p.CodEmpresa == CodEmpresa).List();

                    if (_filial.Any())
                        return _filial[0];
                    else
                    {
                        Filial filial = new Filial
                        {
                            Empresa = _empresa,
                            CodFilial = CodFilial.Value
                        };
                        await connFilial.Save(filial);
                        return filial;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoFilial
    {
        public Int16 Codempresa { get; set; }
        public Int16 Codfilial { get; set; }
        public String Acao { get; set; }
    }
}
