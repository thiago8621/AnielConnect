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
    public class Tb_SenhaController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_SenhaController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Usuario_Aniel}")]
        public async Task<Response> Atualizar(String Usuario_Aniel)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_SenhaDao SenhaDao = new Tb_SenhaDao(conn, null))
                    {
                        List<Tb_Pv_Senha> protocolos = SenhaDao.GetPv(Usuario_Aniel);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Usuario_Aniel, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoUsuario
                                    {
                                        Usuario_Aniel = g.Key.Usuario_Aniel,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Usuario_Aniel } into g
                                      select g.Key.Usuario_Aniel).ToList();


                        List<Tb_Senha> _Senhas = ListService.GetListaQuebrada(chaves, 200).Select(p => SenhaDao.Get(p.Value)).SelectMany(p => p).ToList();

                        if (_Senhas.Count > 0)
                        {
                            using (Conexao<Usuario> connNh = new Conexao<Usuario>(session))
                            {
                                foreach (String usuario in chaves)
                                {
                                    try
                                    {
                                        foreach (var acao in acoes.Where(p => p.Usuario_Aniel == usuario))
                                        {
                                            await ExecutaAcao(connNh, _Senhas, usuario, acao.Acao);
                                        }

                                        SenhaDao.AtualizacaoPv(usuario);
                                    }
                                    catch (Exception ex)
                                    {
                                        SenhaDao.AtualizacaoPvErro(usuario, $"{ex.Message}\n{ex.InnerException?.Message}");
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
                        dao.AdicionaRegistroPV(typeof(Tb_Senha));
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

        private async Task ExecutaAcao(Conexao<Usuario> connNh, List<Tb_Senha> Senhas, String Usuario_Aniel, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                await Pump(connNh, Senhas, Usuario_Aniel, acao);
            }
            else
            {
                IList<Usuario> _listTmp = await connNh.select(p => p.Usuario_Aniel == Usuario_Aniel);

                if (_listTmp.Any())
                    await connNh.Remove(_listTmp.First());
            }
        }

        private async Task<Usuario> Pump(Conexao<Usuario> connNh, List<Tb_Senha> Senhas, String Usuario_Aniel, String acao)
        {
            Tb_Senha _senha = Senhas.Find(p => p.Usuario == Usuario_Aniel);

            Usuario _tmp = new Usuario();

            //VERIFICA SE EXISTE
            IList<Usuario> _listTmp = await connNh.select(p => p.Id == Usuario_Aniel);
            if (_listTmp.Any())
                _tmp = _listTmp.First();

            var _funcionario = await new Tb_FuncionarioController(session).GetFuncionario(_senha.Num_Fre);

            _tmp.Id = _senha.Usuario;
            _tmp.Usuario_Aniel = _senha.Usuario;
            _tmp.Nome = _senha.Nome;
            _tmp.Senha_Aniel = String.IsNullOrEmpty(_senha.Senha) ? null : _senha.Senha;
            _tmp.SecurityStamp = "546aee93-1c3a-4217-bb69-442aa6301d7f";
            _tmp.Ativo = _senha.Ativo == "S" ? (Int16?)1 : (Int16?)0;
            _tmp.IdFuncionario = _funcionario?.Id;
            _tmp.IdDepartamento = _funcionario?.Id_Departamento;

            await connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public async Task<Usuario> GetUsuario(String Usuario_Aniel)
        {
            if (!String.IsNullOrEmpty(Usuario_Aniel))
            {
                using (Conexao<Usuario> connUsuario = new Conexao<Usuario>(session))
                {
                    IList<Usuario> _Senhas = await connUsuario.select(p => p.Usuario_Aniel == Usuario_Aniel);
                    if (_Senhas.Any())
                        return _Senhas[0];
                    else
                    {
                        Usuario _usuario = new Usuario
                        {
                            Usuario_Aniel = Usuario_Aniel
                        };
                        await connUsuario.Save(_usuario);
                        return _usuario;
                    }
                }
            }
            else
                return null;
        }
    }

    public class AcaoUsuario
    {
        public String Usuario_Aniel { get; set; }
        public String Acao { get; set; }
    }
}
