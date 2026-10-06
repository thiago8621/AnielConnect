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
    public class Tb_FornecedorController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_FornecedorController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Fornecedor}")]
        public async Task<Response> Atualizar(String Cod_Fornecedor)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_FornecedorDao FornecedorDao = new Tb_FornecedorDao(conn, null))
                    {
                        List<Tb_Pv_Fornecedor> protocolos = FornecedorDao.GetPv(Cod_Fornecedor);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Fornecedor, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoFornecedor
                                    {
                                        Cod_Fornecedor = g.Key.Cod_Fornecedor,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Fornecedor } into g
                                      select g.Key.Cod_Fornecedor).ToList();

                        List<Tb_Fornecedor> _Fornecedors = FornecedorDao.Get(chaves);

                        if (_Fornecedors.Count > 0)
                        {
                            using (Conexao<Fornecedor> connNh = new Conexao<Fornecedor>(session))
                            {
                                foreach (String chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Cod_Fornecedor == chave))
                                        {
                                            await ExecutaAcao(connNh, _Fornecedors, chave, acao.Acao);
                                        }

                                        FornecedorDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        FornecedorDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
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
            return await Atualizar(String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Fornecedor));
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

        private async Task ExecutaAcao(Conexao<Fornecedor> connNh, List<Tb_Fornecedor> Fornecedors, String Cod_Fornecedor, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Fornecedors, Cod_Fornecedor, acao);
            }
            else
            {
                IList<Fornecedor> _listTmp = await connNh.select(p => p.Cod_Fornecedor == Cod_Fornecedor);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Fornecedor> Pump(Conexao<Fornecedor> connNh, List<Tb_Fornecedor> Fornecedors, String Cod_Fornecedor, string acao)
        {
            Tb_Fornecedor Fornecedor = Fornecedors.Find(p => p.Cod_Fornecedor == Cod_Fornecedor);

            Fornecedor _tmp = new Fornecedor();

            //VERIFICA SE EXISTE
            IList<Fornecedor> _listTmp = await connNh.select(p => p.Cod_Fornecedor == Cod_Fornecedor);
            if (_listTmp.Any())
                _tmp = _listTmp.First();

            _tmp.Cod_Fornecedor = Fornecedor.Cod_Fornecedor;
            _tmp.RazaoSocial = Fornecedor.RazaoSocial;
            _tmp.Descricao = Fornecedor.Descricao;
            _tmp.Cnpj = Fornecedor.Cnpj;
            _tmp.Endereco = Fornecedor.Endereco;
            _tmp.Complemento = Fornecedor.Complemento;
            _tmp.Numero = Fornecedor.Numero;
            _tmp.Bairro = Fornecedor.Bairro;
            _tmp.Municipio = Fornecedor.Municipio;
            _tmp.Cep = Fornecedor.Cep;
            _tmp.Uf = Fornecedor.Uf;
            _tmp.Pais = Fornecedor.Pais;
            _tmp.Insc_Estadual = Fornecedor.Insc_Estadual;
            _tmp.Telefone = Fornecedor.Telefone;
            _tmp.Email = Fornecedor.Email;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Fornecedor> GetFornecedor(String Cod_Fornecedor)
        {
            if (!String.IsNullOrEmpty(Cod_Fornecedor))
            {
                using (Conexao<Fornecedor> connFornecedor = new Conexao<Fornecedor>(session))
                {
                    IList<Fornecedor> _Fornecedor = await connFornecedor.select(p => p.Cod_Fornecedor == Cod_Fornecedor);
                    if (_Fornecedor.Any())
                        return _Fornecedor[0];
                    else
                    {
                        Fornecedor Fornecedor = new Fornecedor
                        {
                            Cod_Fornecedor = Cod_Fornecedor
                        };
                        await connFornecedor.Save(Fornecedor);
                        return Fornecedor;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoFornecedor
    {
        public String Cod_Fornecedor { get; set; }
        public String Acao { get; set; }
    }

}
