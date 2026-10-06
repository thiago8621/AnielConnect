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
    public class Tb_FuncionarioController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_FuncionarioController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Num_Fre}")]
        public async Task<Response> Atualizar(String Num_Fre)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_FuncionarioDao FuncionarioDao = new Tb_FuncionarioDao(conn, null))
                    {
                        List<Tb_Pv_Funcionario> protocolos = FuncionarioDao.GetPv(Num_Fre);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Num_Fre, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoFuncionario
                                    {
                                        Num_Fre = g.Key.Num_Fre,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Num_Fre } into g
                                      select g.Key.Num_Fre).ToList();

                        List<Tb_Funcionario> _Funcionarios = ListService.GetListaQuebrada(chaves, 200).Select(p => FuncionarioDao.Get(chaves)).SelectMany(p => p).ToList();

                        if (_Funcionarios.Count > 0)
                        {
                            using (Conexao<Funcionario> connNh = new Conexao<Funcionario>(session))
                            {
                                foreach (String chave in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Num_Fre == chave))
                                        {
                                            await ExecutaAcao(connNh, _Funcionarios, chave, acao.Acao);
                                        }

                                        FuncionarioDao.AtualizacaoPv(chave);
                                    }
                                    catch (Exception ex)
                                    {
                                        FuncionarioDao.AtualizacaoPvErro(chave, $"{ex.Message}\n{ex.InnerException?.Message}");
                                        
                                        TrataRetornoErro(this.GetType().Assembly, out resposta, ex, null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Funcionario));
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

        private async Task ExecutaAcao(Conexao<Funcionario> connNh, List<Tb_Funcionario> Funcionarios, String Num_Fre, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Funcionarios, Num_Fre, acao);
            }
            else
            {
                IList<Funcionario> _listTmp = await connNh.select(p => p.Num_Fre == Num_Fre);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Funcionario> Pump(Conexao<Funcionario> connNh, List<Tb_Funcionario> Funcionarios, String Num_Fre, string acao)
        {
            Tb_Funcionario Funcionario = Funcionarios.Find(p => p.Num_Fre == Num_Fre);

            Funcionario _tmp = new Funcionario();

            //VERIFICA SE EXISTE
            IList<Funcionario> _listTmp = await connNh.select(p => p.Num_Fre == Num_Fre);
            if (_listTmp.Any())
                _tmp = _listTmp.First();

            var Departamento = await new Tb_DepartamentoController(session).GetDepartamento(Funcionario.Cod_Depto, Funcionario.Depto);

            _tmp.Num_Fre = Funcionario.Num_Fre;
            _tmp.Id_Filial = (await new Tb_FilialController(session).GetFilial(Funcionario.Codempresa, Funcionario.Codfilial)).Id;
            _tmp.Nome = Funcionario.Nome;
            _tmp.Situacao = Funcionario.Situacao;
            _tmp.Tel_Celular = Funcionario.Tel_Celular;
            _tmp.Supervisor = Convert.ToInt16(Funcionario?.Supervisor == "S" ? 1 : 0);
            _tmp.Id_Departamento = Departamento?.Id;
            _tmp.Id_Centro_Custo = new Tb_Centro_CustoController(session).GetCentro_Custo(Funcionario?.Cod_CCusto)?.Result?.Id;
            _tmp.Cpf = Funcionario?.Cpf;

            _tmp.Bairro = Funcionario.Bairro;
            _tmp.Cep = Funcionario?.Cep;
            _tmp.Complemento = Funcionario.Complemento;
            _tmp.Cpf = Funcionario.Cpf;
            _tmp.Endereco = Funcionario.Endereco;
            _tmp.Municipio = Funcionario.Municipio;
            _tmp.Numero = Funcionario.Numero;
            _tmp.Rg = Funcionario.Rg;
            _tmp.Uf = Funcionario.Uf;
            _tmp.Pais = Funcionario?.Pais;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Funcionario> GetFuncionario(String Num_Fre)
        {
            if (!String.IsNullOrEmpty(Num_Fre))
            {
                using (Conexao<Funcionario> connFuncionario = new Conexao<Funcionario>(session))
                {
                    IList<Funcionario> _funcionario = await connFuncionario.select(p => p.Num_Fre == Num_Fre);
                    if (_funcionario.Any())
                        return _funcionario[0];
                    else
                    {
                        Funcionario funcionario = new Funcionario
                        {
                            Num_Fre = Num_Fre
                        };
                        await connFuncionario.Save(funcionario);
                        return funcionario;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoFuncionario
    {
        public String Num_Fre { get; set; }
        public String Acao { get; set; }
    }

}
