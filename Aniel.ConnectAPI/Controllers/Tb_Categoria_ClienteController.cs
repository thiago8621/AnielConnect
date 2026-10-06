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
    public class Tb_Categoria_ClienteController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Categoria_ClienteController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Id_Categoria_Cliente}")]
        public Response Atualizar(Int32? Id_Categoria_Cliente)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Categoria_ClienteDao Categoria_ClienteDao = new Tb_Categoria_ClienteDao(conn, null))
                    {
                        List<Tb_Pv_Cat_Cliente> protocolos = Categoria_ClienteDao.GetPv(Id_Categoria_Cliente);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Id_Cat_Cliente, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoCategoria_Cliente
                                    {
                                        Id_Categoria_Cliente = g.Key.Id_Cat_Cliente,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Id_Cat_Cliente } into g
                                      select g.Key.Id_Cat_Cliente).ToList();

                        List<Tb_Categoria_Cliente> _Categoria_Clientes = Categoria_ClienteDao.Get(chaves);

                        using (Conexao<Categoria_Cliente> connNh = new Conexao<Categoria_Cliente>(session))
                        {
                            foreach (Int32 chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Id_Categoria_Cliente == chave))
                                    {
                                        ExecutaAcao(connNh, _Categoria_Clientes, chave, acao.Acao);
                                    }

                                    Categoria_ClienteDao.AtualizacaoPv(chave);
                                }
                                catch (Exception ex)
                                {
                                    Categoria_ClienteDao.AtualizacaoPvErro(chave, ex.Message);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Categoria_Cliente));
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

        private void ExecutaAcao(Conexao<Categoria_Cliente> connNh, List<Tb_Categoria_Cliente> Categoria_Clientes, Int32 Id_Categoria_Cliente, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, Categoria_Clientes, Id_Categoria_Cliente, acao);
            }
            else
            {
                List<Categoria_Cliente> _listTmp = connNh.select(p => p.Id == Id_Categoria_Cliente);

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Categoria_Cliente Pump(Conexao<Categoria_Cliente> connNh, List<Tb_Categoria_Cliente> Categoria_Clientes, Int32 Id_Categoria_Cliente, string acao)
        {
            Tb_Categoria_Cliente Categoria_Cliente = Categoria_Clientes.Find(p => p.Id == Id_Categoria_Cliente);

            Categoria_Cliente _tmp = new Categoria_Cliente();

            //VERIFICA SE EXISTE
            List<Categoria_Cliente> _listTmp = connNh.select(p => p.Id == Id_Categoria_Cliente);
                if (_listTmp.Any())
                    _tmp = _listTmp.First();

            _tmp.Id = Categoria_Cliente.Id;
            _tmp.Descricao = Categoria_Cliente.Descricao;

            connNh.SaveOrUpdate(_tmp);

            return _tmp;
        }

        [HttpGet()]
        public Categoria_Cliente GetCategoria(Int32? Id_Categoria_Cliente)
        {
            if (Id_Categoria_Cliente.HasValue)
            {
                using (Conexao<Categoria_Cliente> connPerfil = new Conexao<Categoria_Cliente>(session))
                {
                    List<Categoria_Cliente> _perfis = connPerfil.select(p => p.Id == Id_Categoria_Cliente);
                    if (_perfis.Any())
                        return _perfis[0];
                    else
                    {
                        Categoria_Cliente perfil = new Categoria_Cliente
                        {
                            Id = Id_Categoria_Cliente.Value
                        };
                        connPerfil.Save(perfil);
                        return perfil;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoCategoria_Cliente
    {
        public Int32 Id_Categoria_Cliente { get; set; }
        public String Acao { get; set; }
    }

}
