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
    public class Tb_AcessoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_AcessoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Usuario_Aniel}/{Sigla_Modulo}/{Cod_Area}")]
        public Response Atualizar(String Usuario_Aniel, String Sigla_Modulo, String Cod_Area)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_AcessoDao AcessoDao = new Tb_AcessoDao(conn, null))
                    {
                        List<Tb_Pv_Acesso> protocolos = AcessoDao.GetPv(Sigla_Modulo, Cod_Area, Usuario_Aniel);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Sigla_Modulo, o.Cod_Area, o.Usuario_Aniel, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoAcesso
                                    {
                                        Sigla_Modulo = g.Key.Sigla_Modulo,
                                        Cod_Area = g.Key.Cod_Area,
                                        Usuario_Aniel = g.Key.Usuario_Aniel,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Sigla_Modulo, o.Cod_Area, o.Usuario_Aniel } into g
                                      select new ChaveAcesso
                                      {
                                         Sigla_Modulo = g.Key.Sigla_Modulo,
                                         Cod_Area = g.Key.Cod_Area,
                                         Usuario_Aniel = g.Key.Usuario_Aniel
                                      }).ToList();

                        List<Tb_Acesso> _Usuario_Aniels = AcessoDao.Get(chaves);

                        using (Conexao<Acesso> connNh = new Conexao<Acesso>(session))
                        {
                            foreach (ChaveAcesso chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Sigla_Modulo == chave.Sigla_Modulo && p.Cod_Area == chave.Cod_Area && p.Usuario_Aniel == chave.Usuario_Aniel))
                                    {
                                        ExecutaAcao(connNh, _Usuario_Aniels, chave.Sigla_Modulo, chave.Cod_Area, chave.Usuario_Aniel, acao.Acao);
                                    }

                                    AcessoDao.AtualizacaoPv(chave.Sigla_Modulo, chave.Cod_Area, chave.Usuario_Aniel);
                                }
                                catch (Exception ex)
                                {
                                    AcessoDao.AtualizacaoPvErro(chave.Sigla_Modulo, chave.Cod_Area, chave.Usuario_Aniel, ex.Message);
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
            return Atualizar(String.Empty, String.Empty, null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Acesso));
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

        private void ExecutaAcao(Conexao<Acesso> connNh, List<Tb_Acesso> Perfil_Usuarios, String Sigla_Modulo, String Cod_Area, String Usuario_Aniel, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Perfil_Usuarios, Sigla_Modulo, Cod_Area, Usuario_Aniel, acao);
            }
            else
            {
                List<Acesso> _listTmp = connNh.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Usuario.Usuario_Aniel == Usuario_Aniel);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Acesso Pump(Conexao<Acesso> connNh, List<Tb_Acesso> Perfis, String Sigla_Modulo, String Cod_Area, String Usuario_Aniel, string acao)
        {
            Tb_Acesso _Perfil_Usuario = Perfis.Find(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area && p.Usuario == Usuario_Aniel);

            Acesso _tmp = new Acesso();

            //VERIFICA SE EXISTE
            List<Acesso> _listTmp = connNh.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Usuario.Usuario_Aniel == Usuario_Aniel);
            if (_listTmp.Any())
                _tmp = _listTmp.First();           

            _tmp.Area_Modulo = new Tb_Area_ModuloController(session).GetArea(_Perfil_Usuario.Sigla_Modulo, _Perfil_Usuario.Cod_Area);
            _tmp.Usuario = new Tb_SenhaController(session).GetUsuario(_Perfil_Usuario.Usuario);

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }
        
        [HttpGet]
        public Acesso GetPerfil_Usuario(String Sigla_Modulo, String Cod_Area, String Usuario_Aniel)
        {
            if (!String.IsNullOrEmpty(Sigla_Modulo) && !String.IsNullOrEmpty(Cod_Area) && !String.IsNullOrEmpty(Usuario_Aniel))
            {
                using (Conexao<Acesso> connUsuario_Aniel = new Conexao<Acesso>(session))
                {
                    List<Area_Modulo> _areas = new Conexao<Area_Modulo>(session).select(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);

                    Area_Modulo _area = null;
                    if (_areas.Count == 0)
                        return null;

                    _area = _areas[0];

                    List<Usuario> _usuarios = new Conexao<Usuario>(session).select(p => p.Usuario_Aniel == Usuario_Aniel);

                    Usuario _usuario = null;
                    if (_usuarios.Count == 0)
                        return null;

                    _usuario = _usuarios[0];



                    List<Acesso> _Usuarios = connUsuario_Aniel.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Usuario.Usuario_Aniel == Usuario_Aniel);
                    if (_Usuarios.Any())
                        return _Usuarios[0];
                    else
                    {
                        Acesso _Perfil_Usuario = new Acesso
                        {
                            Area_Modulo = _area,
                            Usuario = _usuario
                        };
                        connUsuario_Aniel.Save(_Perfil_Usuario);
                        return _Perfil_Usuario;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoAcesso
    {
        public String Sigla_Modulo { get; set; }
        public String Cod_Area { get; set; }
        public String Usuario_Aniel { get; set; }
        public String Acao { get; set; }
    }
}
