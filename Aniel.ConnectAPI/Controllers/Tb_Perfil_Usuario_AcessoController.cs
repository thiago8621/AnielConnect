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
    public class Tb_Perfil_Usuario_AcessoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Perfil_Usuario_AcessoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Area}/{Sigla_Modulo}/{Cod_Perfil_Usuario}")]
        public Response Atualizar(String Cod_Area, String Sigla_Modulo, Int32? Cod_Perfil_Usuario)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Perfil_De_Usuario_AcessoDao Perfil_Usuario_AcessoDao = new Tb_Perfil_De_Usuario_AcessoDao(conn, null))
                    {
                        List<Tb_Pv_Perfil_Usu_Acesso> protocolos = Perfil_Usuario_AcessoDao.GetPv(Sigla_Modulo, Cod_Area, Cod_Perfil_Usuario);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Sigla_Modulo, o.Cod_Area, o.Cod_Perfil_Usuario, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoPerfil_Usuario_Acesso
                                    {
                                        Sigla_Modulo = g.Key.Sigla_Modulo,
                                        Cod_Area = g.Key.Cod_Area,
                                        Cod_Perfil_Usuario = g.Key.Cod_Perfil_Usuario,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Sigla_Modulo, o.Cod_Area, o.Cod_Perfil_Usuario } into g
                                      select new ChavePerfil_Usuario_Acesso
                                      {
                                         Sigla_Modulo = g.Key.Sigla_Modulo,
                                         Cod_Area = g.Key.Cod_Area,
                                         Cod_Perfil_Usuario = g.Key.Cod_Perfil_Usuario
                                      }).ToList();

                        List<Tb_Perfil_Usuario_Acesso> _Cod_Perfil_Usuarios = Perfil_Usuario_AcessoDao.Get(chaves);

                        using (Conexao<Perfil_Usuario_Acesso> connNh = new Conexao<Perfil_Usuario_Acesso>(session))
                        {
                            foreach (ChavePerfil_Usuario_Acesso chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Sigla_Modulo == chave.Sigla_Modulo && p.Cod_Area == chave.Cod_Area && p.Cod_Perfil_Usuario == chave.Cod_Perfil_Usuario))
                                    {
                                        ExecutaAcao(connNh, _Cod_Perfil_Usuarios, chave.Sigla_Modulo, chave.Cod_Area, chave.Cod_Perfil_Usuario, acao.Acao);
                                    }

                                    Perfil_Usuario_AcessoDao.AtualizacaoPv(chave.Sigla_Modulo, chave.Cod_Area, chave.Cod_Perfil_Usuario);
                                }
                                catch (Exception ex)
                                {
                                    Perfil_Usuario_AcessoDao.AtualizacaoPvErro(chave.Sigla_Modulo, chave.Cod_Area, chave.Cod_Perfil_Usuario, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Perfil_Usuario_Acesso));
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


        private void ExecutaAcao(Conexao<Perfil_Usuario_Acesso> connNh, List<Tb_Perfil_Usuario_Acesso> Perfil_Usuarios, String Sigla_Modulo, String Cod_Area, Int32 Cod_Perfil_Usuario, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Perfil_Usuarios, Sigla_Modulo, Cod_Area, Cod_Perfil_Usuario, acao);
            }
            else
            {
                List<Perfil_Usuario_Acesso> _listTmp = connNh.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Perfil_Usuario.Cod_Perfil_Usuario == Cod_Perfil_Usuario);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Perfil_Usuario_Acesso Pump(Conexao<Perfil_Usuario_Acesso> connNh, List<Tb_Perfil_Usuario_Acesso> Perfis, String Sigla_Modulo, String Cod_Area, Int32 Cod_Perfil_Usuario, string acao)
        {
            Tb_Perfil_Usuario_Acesso _Perfil_Usuario = Perfis.Find(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area && p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);

            Perfil_Usuario_Acesso _tmp = new Perfil_Usuario_Acesso();

            //VERIFICA SE EXISTE
            List<Perfil_Usuario_Acesso> _listTmp = connNh.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Perfil_Usuario.Cod_Perfil_Usuario == Cod_Perfil_Usuario);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Area_Modulo = new Tb_Area_ModuloController(session).GetArea(_Perfil_Usuario.Sigla_Modulo, _Perfil_Usuario.Cod_Area);
            _tmp.Perfil_Usuario = new Tb_Perfil_UsuarioController(session).GetPerfil_Usuario(_Perfil_Usuario.Cod_Perfil_Usuario);

            connNh.SaveOrUpdate(_tmp);

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Perfil_Usuario_Acesso GetPerfil_Usuario(String Sigla_Modulo, String Cod_Area, Int32? Cod_Perfil_Usuario)
        {
            if (!String.IsNullOrEmpty(Sigla_Modulo) && !String.IsNullOrEmpty(Cod_Area) && Cod_Perfil_Usuario.HasValue)
            {
                using (Conexao<Perfil_Usuario_Acesso> connCod_Perfil_Usuario = new Conexao<Perfil_Usuario_Acesso>(session))
                {
                    List<Area_Modulo> _areas = new Conexao<Area_Modulo>(session).select(p => p.Sigla_Modulo == Sigla_Modulo && p.Cod_Area == Cod_Area);

                    Area_Modulo _area = null;
                    if (_areas.Count == 0)
                        return null;

                    _area = _areas[0];

                    List<Perfil_Usuario> _perfis = new Conexao<Perfil_Usuario>(session).select(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);

                    Perfil_Usuario _perfil = null;
                    if (_perfis.Count == 0)
                        return null;

                    _perfil = _perfis[0];



                    List<Perfil_Usuario_Acesso> _Perfil_Usuarios = connCod_Perfil_Usuario.select(p => p.Area_Modulo.Sigla_Modulo == Sigla_Modulo && p.Area_Modulo.Cod_Area == Cod_Area && p.Perfil_Usuario.Cod_Perfil_Usuario == Cod_Perfil_Usuario);
                    if (_Perfil_Usuarios.Any())
                        return _Perfil_Usuarios[0];
                    else
                    {
                        Perfil_Usuario_Acesso _Perfil_Usuario = new Perfil_Usuario_Acesso
                        {
                            Area_Modulo = _area,
                            Perfil_Usuario = _perfil
                        };
                        connCod_Perfil_Usuario.Save(_Perfil_Usuario);
                        return _Perfil_Usuario;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoPerfil_Usuario_Acesso
    {
        public String Sigla_Modulo { get; set; }
        public String Cod_Area { get; set; }
        public Int32 Cod_Perfil_Usuario { get; set; }
        public String Acao { get; set; }
    }
}
