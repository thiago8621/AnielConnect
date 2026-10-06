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
    public class Tb_Perfil_UsuarioController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Perfil_UsuarioController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Cod_Perfil_Usuario}")]
        public Response Atualizar(Int32? Cod_Perfil_Usuario)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Perfil_De_UsuarioDao PerfilDao = new Tb_Perfil_De_UsuarioDao(conn, null))
                    {
                        List<Tb_Pv_Perfil_Usuario> protocolos = PerfilDao.GetPv(Cod_Perfil_Usuario);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Cod_Perfil_Usuario, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoPerfil_Usuario
                                    {
                                        Cod_Perfil_Usuario = g.Key.Cod_Perfil_Usuario,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Cod_Perfil_Usuario } into g
                                      select g.Key.Cod_Perfil_Usuario).ToList();

                        List<Tb_Perfil_Usuario> _perfis = PerfilDao.Get(chaves);

                        using (Conexao<Perfil_Usuario> connNh = new Conexao<Perfil_Usuario>(session))
                        {
                            foreach (Int16 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Cod_Perfil_Usuario == chave))
                                    {
                                        ExecutaAcao(connNh, _perfis, chave, acao.Acao);
                                    }

                                    PerfilDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    PerfilDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Perfil_Usuario));
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

        private void ExecutaAcao(Conexao<Perfil_Usuario> connNh, List<Tb_Perfil_Usuario> Perfil_Usuarios, Int16 Cod_Perfil_Usuario, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Perfil_Usuarios, Cod_Perfil_Usuario, acao);
            }
            else
            {
                List<Perfil_Usuario> _listTmp = connNh.select(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Perfil_Usuario Pump(Conexao<Perfil_Usuario> connNh, List<Tb_Perfil_Usuario> Perfil_Usuarios, Int32 Cod_Perfil_Usuario, string acao)
        {
            Tb_Perfil_Usuario Perfil_Usuario = Perfil_Usuarios.Find(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);

            Perfil_Usuario _tmp = new Perfil_Usuario();

            //VERIFICA SE EXISTE
            List<Perfil_Usuario> _listTmp = connNh.select(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);
            if (_listTmp.Any())
                _tmp = _listTmp.First();
            

            _tmp.Cod_Perfil_Usuario = Perfil_Usuario.Cod_Perfil_Usuario;
            _tmp.Descricao = Perfil_Usuario.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Perfil_Usuario GetPerfil_Usuario(Int32? Cod_Perfil_Usuario)
        {
            if (Cod_Perfil_Usuario.HasValue)
            {
                using (Conexao<Perfil_Usuario> connPerfil_Usuario = new Conexao<Perfil_Usuario>(session))
                {
                    List<Perfil_Usuario> _Perfil_Usuario = connPerfil_Usuario.select(p => p.Cod_Perfil_Usuario == Cod_Perfil_Usuario);
                    if (_Perfil_Usuario.Any())
                        return _Perfil_Usuario[0];
                    else
                    {
                        Perfil_Usuario Perfil_Usuario = new Perfil_Usuario
                        {
                            Cod_Perfil_Usuario = Cod_Perfil_Usuario.Value
                        };
                        connPerfil_Usuario.Save(Perfil_Usuario);
                        return Perfil_Usuario;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoPerfil_Usuario
    {
        public Int32 Cod_Perfil_Usuario { get; set; }
        public String Acao { get; set; }
    }
}
