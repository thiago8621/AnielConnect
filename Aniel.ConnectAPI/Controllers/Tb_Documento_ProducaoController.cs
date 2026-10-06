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
    public class Tb_Documento_ProducaoController : BaseController, IAtualizacaoAutomatica
    {
        private readonly ISession session;

        public Tb_Documento_ProducaoController(ISession session)
        {
            this.session = session;
        }

        [HttpGet("Atualiza/{Codct}/{Projeto}/{Num_Obra}/{Num_Doc}")]
        public Response Atualizar(String Codct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            Response resposta = new Response();

            try
            {
                StringBuilder retorno = new StringBuilder();

                using (FbConnection conn = CreateConnection())
                {
                    using (Tb_Documento_ProducaoDao documentoDao = new Tb_Documento_ProducaoDao(conn, null))
                    {
                        List<Tb_Pv_Documento_Producao> protocolos = documentoDao.GetPv(Codct,Projeto, Num_Obra, Num_Doc);

                        if (protocolos.Count == 0)
                        {
                            resposta.result = true;
                            return resposta;
                        }

                        var acoes = from o in protocolos
                                    group o by new { o.Codct, o.Projeto, o.Num_Obra, o.Num_Doc, o.Acao } into g
                                    orderby g.Min(p => p.Dia)
                                    select new AcaoDocumento_Producao
                                    {
                                        Codct = g.Key.Codct,
                                        Projeto = g.Key.Projeto,
                                        Num_Obra = g.Key.Num_Obra,
                                        Num_Doc = g.Key.Num_Doc,
                                        Acao = g.Key.Acao
                                    };

                        var chaves = (from o in protocolos
                                      group o by new { o.Codct, o.Projeto, o.Num_Obra, o.Num_Doc } into g
                                      select new ChaveDocumento_Producao {
                                          Codct = g.Key.Codct,
                                          Projeto = g.Key.Projeto,
                                          Num_Obra = g.Key.Num_Obra,
                                          Num_Doc = g.Key.Num_Doc
                                      }).ToList();

                        List<Tb_Documento_Producao> _documentos = documentoDao.Get(chaves);

                        using (Conexao<Documento_Producao> connNh = new Conexao<Documento_Producao>(session))
                        {
                            foreach (ChaveDocumento_Producao chave in chaves)
                            {
                                try
                                {
                                    foreach (var acao in acoes.Where(p => p.Codct == chave.Codct && p.Projeto == chave.Projeto && 
                                                                          p.Num_Obra == chave.Num_Obra && p.Num_Doc == chave.Num_Doc))
                                    {
                                        ExecutaAcao(connNh, _documentos, chave.Codct, chave.Projeto, chave.Num_Obra, chave.Num_Doc, acao.Acao);
                                    }

                                    documentoDao.AtualizacaoPv(chave.Codct, chave.Projeto, chave.Num_Obra, chave.Num_Doc);
                                }
                                catch (Exception ex)
                                {
                                    documentoDao.AtualizacaoPvErro(chave.Codct, chave.Projeto, chave.Num_Obra, chave.Num_Doc, ex.Message);
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
            return Atualizar(String.Empty, String.Empty, String.Empty, null);
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
                        dao.AdicionaRegistroPV(typeof(Tb_Documento_Producao));
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

        private void ExecutaAcao(Conexao<Documento_Producao> connNh, List<Tb_Documento_Producao> projetos, String codct, String projeto, String obra, Int32 num_doc, String acao)
        {
            if (acao == "I" || acao == "U")
            {
                Pump(connNh, projetos, codct, projeto, obra, num_doc, acao);
            }
            else
            {
                IList<Documento_Producao> _listTmp = connNh.queryover()
                    .Where(p => p.Num_Obra == obra && p.Num_Doc == num_doc)
                    .JoinQueryOver(p => p.Projeto)
                    .Where(p => p.Codigo == projeto)
                    .JoinQueryOver(p => p.Contrato)
                    .Where(p=> p.CodCt == codct).List();

                if (_listTmp.Any())
                    connNh.Remove(_listTmp.First());
            }
        }

        private Documento_Producao Pump(Conexao<Documento_Producao> connNh, List<Tb_Documento_Producao> obras, String codct, String projeto, String obra, Int32 num_doc, string acao)
        {
            Tb_Documento_Producao documento = obras.Find(p => p.CodCt == codct && p.Projeto == projeto && p.Num_Obra == obra && p.Num_Doc == num_doc);

            Documento_Producao _tmp = new Documento_Producao();

            //VERIFICA SE EXISTE
            IList<Documento_Producao> _listTmp = connNh.queryover()
                    .Where(p => p.Num_Obra == obra && p.Num_Doc == num_doc)
                    .JoinQueryOver(p=> p.Projeto)
                    .Where(p=> p.Codigo == projeto)
                    .JoinQueryOver(p=> p.Contrato)
                    .Where(p=> p.CodCt == codct).List();

            if (_listTmp.Any())
                _tmp = _listTmp.First();

            using (ITransaction trans = session.BeginTransaction())
            {
                _tmp.Projeto = new Tb_ProjetoController(session).GetProjeto(documento.CodCt, documento.Projeto);
                _tmp.Num_Obra = documento.Num_Obra;
                _tmp.Num_Doc = documento.Num_Doc;
                _tmp.Num_RVT = documento.Num_RVT;
                _tmp.Tarefa = documento.Tarefa;

                _tmp.Escritorio = new Tb_EscritorioController(session).GetEscritorio(documento.Cod_Escritorio);
                _tmp.Area = new Tb_AreaController(session).GetArea(documento.Cod_Escritorio, documento.Cod_Area);
                _tmp.Localidade = new Tb_LocalidadeController(session).GetLocalidade(documento.Cod_Escritorio, documento.Cod_Area, documento.Cod_Localidade);

                _tmp.Equipe = new Tb_EquipeController(session).GetEquipe(documento.Equipe);

                _tmp.Codigo_Encerramento = new Tb_Codigo_EncerramentoController(session).GetCodigo_Encerramento(documento.Cod_Encerramento, documento.CodCt);

                _tmp.Tipo_Servico_Equipe = new Tb_Tipo_Serv_EquipeController(session).GetTipo_Servico_Equipe(documento.Cod_Tipo_Serv);
                _tmp.Sub_Tipo_Servico = new Tb_Sub_Tipo_ServicoController(session).GetSub_Tipo_Servico(documento.Cod_Tipo_Serv, documento.Cod_Sub_Tipo_Serv);

                _tmp.Data_Agendamento = documento.Data_Maxima;
                _tmp.Data_Producao = documento.Data_Prod;
                _tmp.Hora_Producao = documento.Hora_Prod;
                _tmp.Observacao = documento.Observacao;
                _tmp.Logradouro = documento.Endereco;
                _tmp.Numero = documento.Endereco_Num;
                _tmp.Complemento = documento.Endereco_Compl;
                _tmp.Bairro = documento.Endereco_Bairro;
                _tmp.Cep = documento.Endereco_Cpl;
                _tmp.Latitude_Baixa = documento.Latitude;
                _tmp.Longitude_Baixa = documento.Longitude;

                connNh.SaveOrUpdate(_tmp);

                trans.Commit();
            }
            return _tmp;
        }

        public Documento_Producao GetProjeto(String Codct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            if (!String.IsNullOrEmpty(Codct) && !String.IsNullOrEmpty(Projeto) && !String.IsNullOrEmpty(Num_Obra) && Num_Doc.HasValue)
            {
                using (Conexao<Documento_Producao> connDocumento = new Conexao<Documento_Producao>(session))
                {
                    IList<Projeto> _projetos = new Conexao<Projeto>(session).queryover()
                        .Where(p => p.Codigo == Projeto)
                        .JoinQueryOver(p => p.Contrato)
                        .Where(p => p.CodCt == Codct).List();

                    Projeto _projeto = null;
                    if (_projetos.Count == 0)
                        return null;

                    _projeto = _projetos[0];

                    IList<Documento_Producao> _docs = connDocumento.queryover()
                        .Where(p => p.Num_Obra == Num_Obra && p.Num_Doc == Num_Doc)
                        .JoinQueryOver(p => p.Projeto)
                        .Where(p=> p.Codigo == Projeto)
                        .JoinQueryOver(p => p.Contrato)
                        .Where(p => p.CodCt == Codct).List();

                    if (_docs.Any())
                        return _docs[0];
                    else
                    {
                        Documento_Producao doc = new Documento_Producao
                        {
                            Projeto = _projeto,
                            Num_Obra = Num_Obra,
                            Num_Doc = Num_Doc.Value
                        };
                        connDocumento.Save(doc);
                        return doc;
                    }
                }
            }
            else
                return null;
        }

    }

    public class AcaoDocumento_Producao
    {
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32 Num_Doc { get; set; }
        public String Acao { get; set; }
    }
}
