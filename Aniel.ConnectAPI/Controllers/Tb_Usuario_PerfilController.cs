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
using NHibernate.Criterion;

namespace Aniel.ConnectAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]/v{version:apiVersion}")]
    [ApiController]
    public class Tb_Usuario_PerfilController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Usuario_PerfilController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Perfil_Usuario}/{Usuario_Perfil}")]
        public Response Atualizar(Int16? Cod_Perfil_Usuario, String Usuario_Perfil)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Usuario_PerfilDao Usuario_PerfilDao = new Tb_Usuario_PerfilDao(conn, null))
                    {
                        List<Tb_Pv_Usuario_Perfil> protocolos = Usuario_PerfilDao.GetPv(Cod_Perfil_Usuario ,Usuario_Perfil);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Perfil_Usuario, o.Usuario, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoUsuario_Perfil
                                    {
                                        Cod_Perfil_Usuario = g.Key.Cod_Perfil_Usuario,
                                        Usuario = g.Key.Usuario,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Perfil_Usuario, o.Usuario } into g
                                      select new ChaveUsuario_Perfil {
                                         Cod_Perfil_Usuario = g.Key.Cod_Perfil_Usuario,
                                         Usuario = g.Key.Usuario
                                      }).ToList();

                        List<Tb_Usuario_Perfil> _Usuario_Perfils = Usuario_PerfilDao.Get(chaves);

                        using (Conexao<Usuario_Perfil> connNh = new Conexao<Usuario_Perfil>(session))
                        {
                            foreach (ChaveUsuario_Perfil chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Perfil_Usuario == chave.Cod_Perfil_Usuario && p.Usuario == chave.Usuario))
                                    {
                                        ExecutaAcao(connNh, _Usuario_Perfils, chave.Cod_Perfil_Usuario, chave.Usuario, acao.Acao);
                                    }

                                    Usuario_PerfilDao.AtualizacaoPv(chave.Cod_Perfil_Usuario, chave.Usuario);
                                }
                                catch (Exception ex)
                                {
                                    Usuario_PerfilDao.AtualizacaoPvErro(chave.Cod_Perfil_Usuario, chave.Usuario, ex.Message);
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
            return Atualizar(null, String.Empty);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Usuario_Perfil));
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

        private void ExecutaAcao(Conexao<Usuario_Perfil> connNh, List<Tb_Usuario_Perfil> Usuario_Perfils, Int32 Cod_Perfil_Usuario, String Usuario_Perfil, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Usuario_Perfils, Cod_Perfil_Usuario, Usuario_Perfil, acao);
            }
            else
            {
                IList<Usuario_Perfil> _listTmp = connNh.queryover()
                    .Where(Restrictions.Eq(Projections.Property<Usuario>(p => p.Usuario_Aniel), Usuario_Perfil))
                    .Where(Restrictions.Eq(Projections.Property<Perfil_Usuario>(p => p.Cod_Perfil_Usuario), Cod_Perfil_Usuario))
                    .List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Usuario_Perfil Pump(Conexao<Usuario_Perfil> connNh, List<Tb_Usuario_Perfil> Usuario_Perfils, Int32 Cod_Perfil_Usuario, String Usuario, string acao)
        {
            Tb_Usuario_Perfil Usuario_Perfil = Usuario_Perfils.Find(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario && p.Usuario == Usuario);

            Usuario_Perfil _tmp = new Usuario_Perfil();

            //VERIFICA SE EXISTE
            IList<Usuario_Perfil> _listTmp = connNh.queryover()
                    .Where(Restrictions.Eq(Projections.Property<Usuario>(p => p.Usuario_Aniel), Usuario_Perfil))
                    .Where(Restrictions.Eq(Projections.Property<Perfil_Usuario>(p => p.Cod_Perfil_Usuario), Cod_Perfil_Usuario))
                    .List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();
            
            _tmp.Usuario_Aniel = new Tb_SenhaController(session).GetUsuario(Usuario_Perfil.Usuario);
            _tmp.Perfil_Usuario = new Tb_Perfil_UsuarioController(session).GetPerfil_Usuario(Usuario_Perfil.Cod_Perfil_Usuario);

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Usuario_Perfil GetUsuario_Perfil(Int16? Cod_Perfil_Usuario, String Usuario)
        {
            if (Cod_Perfil_Usuario.HasValue && !String.IsNullOrEmpty(Usuario))
            {
                using (Conexao<Usuario_Perfil> connUsuario_Perfil = new Conexao<Usuario_Perfil>(session))
                {
                    List<Usuario> _usuarios = new Conexao<Usuario>(session).select(p => p.Usuario_Aniel == Usuario);
                    Usuario _usuario = null;
                    if (_usuarios.Count == 0)
                        return null;
                    _usuario = _usuarios[0];

                    List<Perfil_Usuario> _perfis = new Conexao<Perfil_Usuario>(session).select(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);
                    Perfil_Usuario _perfil = null;
                    if (_perfis.Count == 0)
                        return null;
                    _perfil = _perfis[0];


                    IList<Usuario_Perfil> _Usuario_Perfils = connUsuario_Perfil.queryover()
                    .Where(Restrictions.Eq(Projections.Property<Usuario>(p => p.Usuario_Aniel), Usuario))
                    .Where(Restrictions.Eq(Projections.Property<Perfil_Usuario>(p => p.Cod_Perfil_Usuario), Cod_Perfil_Usuario))
                    .List();

                    if (_Usuario_Perfils.Any())
                        return _Usuario_Perfils[0];
                    else
                    {
                        Usuario_Perfil Usuario_Perfil = new Usuario_Perfil
                        {
                            Perfil_Usuario = _perfil,
                            Usuario_Aniel = _usuario
                        };
                        connUsuario_Perfil.Save(Usuario_Perfil);
                        return Usuario_Perfil;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoUsuario_Perfil
    {
        public Int32 Cod_Perfil_Usuario { get; set; }
        public String Usuario { get; set; }
        public String Acao { get; set; }
    }
}
