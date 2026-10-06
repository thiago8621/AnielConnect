using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Sinapse.Model.Identity;
using Sinapse.Model;
using Microsoft.AspNet.Identity;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.Repository;
using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using OpenXmlPowerTools;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core.Service;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MySqlX.XDevAPI;
using Aniel.Connect.Repository.Model;
using Resources;
using System.Diagnostics.Contracts;
using System.Net;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Repository.PesquisaSatisfacaoRepository;
using System.Web.WebSockets;
using Aniel.Connect.Repository.ConfigUrl;
using Swashbuckle.Swagger;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ProducaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/ProducaoApiController/Inserir")]
        public async Task<PostApiRetorno<ProducaoModels>> Inserir(ProducaoModels value)
        {
            PostApiRetorno<ProducaoModels> retorno = new PostApiRetorno<ProducaoModels>();

            try
            {
                Custos_TarefaAniel modelo = null;

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(value.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Custos_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryCaderno = new Conexao<Caderno_EncargosAniel>(session))

                    {
                        var classeMO = GetClasseMO(value.Codigo);
                        var documento = await repositoryDoc.selectFirst(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto && p.Numero_Obra_Original == value.Num_Obra && p.Num_Doc == value.Num_Doc);

                        

                        if (String.IsNullOrEmpty(value.Codigo))
                            throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODIGO));

                        var dadosOs = GetOSById(value.Num_Obra);
                        var CodPrecoPadrao = GetCodPrecoPadrao(session, value.CodCt, dadosOs.Projeto, dadosOs.UnidadeCusto, classeMO);

                        modelo = new Custos_TarefaAniel
                        {
                            CodCt = value.CodCt,
                            Projeto = dadosOs.Projeto,
                            Num_Obra = dadosOs.NumeroObra,
                            Num_Doc = Convert.ToInt32(dadosOs.NumeroDocumento),
                            Data_Prod = Convert.ToDateTime(value.Data_Prod),
                            Cod_Tipo_Serv = Convert.ToInt16(dadosOs.CodTipoServico),
                            Cod_Sub_Tipo_Serv = dadosOs.CodSubTipoServico ?? "-",
                            Unidade_Custo = dadosOs.UnidadeCusto,
                            Tipo_Producao = value.Tipo_Producao ?? "AC",
                            Cod_Preco = CodPrecoPadrao,
                            Equipe = dadosOs.Equipe,
                            Tarefa = dadosOs.Tarefa,
                            Codigo = value.Codigo,
                            Quant_Catal = 0,
                            Quant_Prod = Convert.ToDouble(value.Quant_Prod),
                            Dia = DateTime.Now,
                            Hora = DateTime.Now,
                            Usuario = value.Usuario
                        };

                        await repository.SaveOrUpdate(modelo);
                        retorno.Ok = true;
                    }


                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        private OsApiConsulta GetOSById(string num_OS)
        {
            StringBuilder sql = new StringBuilder();

            OsApiConsulta retorno = null;

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT * ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.NUM_OBRA_ORIGINAL =" + num_OS + ") ");

                using (var _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new OsApiConsulta
                        {
                            Tarefa = Convert.ToString(_reader["TAREFA"]),
                            NumeroDocumento = Convert.ToString(_reader["NUM_DOC"]),
                            NumeroObra = Convert.ToString(_reader["NUM_OBRA"]),
                            UnidadeCusto = Convert.ToString(_reader["UNIDADE_CUSTO"]),
                            Equipe = Convert.ToString(_reader["EQUIPE"]),
                            Projeto = Convert.ToString(_reader["Projeto"]),
                            CodTipoServico = Convert.ToString(_reader["COD_TIPO_SERV"]),
                            CodSubTipoServico = Convert.ToString(_reader["COD_SUB_TIPO_SERV"]),

                        };
                    }
                }

                return retorno;
            }
        }
        private string GetClasseMO(string codigo)
        {
            StringBuilder sql = new StringBuilder();

            string retorno = null;

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT * ");
                sql.AppendLine("FROM TB_CADERNO_ENCARGOS A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.CODIGO ='" + codigo + "') ");

                using (var _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = Convert.ToString(_reader["CLASSE_MO"]);
                    }
                }

                return retorno;
            }
        }


        private String GetCodPrecoPadrao(ISession session, String cod_ct, String projeto, String unidade_custo, String classe_mo)
        {
            var retorno = String.Empty;
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", cod_ct);
                parametros.Add("@PROJETO", projeto);
                parametros.Add("@UNIDADE_CUSTO", unidade_custo);
                parametros.Add("@CLASSE_MO", classe_mo);

                StringBuilder sql = new StringBuilder(@"SELECT FIRST 1 RCOD_PRECO AS COD_PRECO
                                                        FROM SP_BUSCAR_VALOR_SERVICO(@CODCT,@PROJETO,'-',@UNIDADE_CUSTO,NULL)
                                                        WHERE RCLASSE_MO = @CLASSE_MO
                                                        ORDER BY RVALOR_FAT DESC");

                using (var _reader = CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return Convert.ToString(_reader["COD_PRECO"]);
                    }
                }
            }

            return retorno;

        }

        public class Equipe
        {
            public String Matricula { get; set; }
            public String CodAux { get; set; }
            public String Nome { get; set; }
            public String CodCtPadrao { get; set; }
            public String ProjetoPadrao { get; set; }

            public Double? Latitude { get; set; }
            public Double? Longitude { get; set; }

        }
        [HttpPost]
        [Route("api/OsApiController/GetEquipe")]

        public Equipe GetEquipe(String Matricula)
        {
            StringBuilder sql = new StringBuilder();

            Equipe retorno = null;

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT FIRST 1 A.EQUIPE, A.EQUIPE_INTEGRATOR, A.NOME , C.CODCT_PADRAO, C.PROJETO_PADRAO, A.CODCT_INTEGRATOR, A.PROJETO_INTEGRATOR, C.LATITUDE, C.LONGITUDE ");
                sql.AppendLine("FROM TB_EQUIPE A ");
                sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ");
                sql.AppendLine("ON (A.NUM_FRE = B.NUM_FRE)");
                sql.AppendLine("LEFT JOIN TB_FILIAL C ");
                sql.AppendLine("ON (C.CODEMPRESA = B.CODEMPRESA AND C.CODFILIAL = B.CODFILIAL) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.EQUIPE = @EQUIPE OR A.EQUIPEAUX = @EQUIPE) ");
                parametros.Add("@EQUIPE", Matricula);

                using (var _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new Equipe()
                        {
                            Matricula = Convert.ToString(_reader["EQUIPE"]),
                            Nome = Convert.ToString(_reader["NOME"]),
                            CodCtPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["CODCT_INTEGRATOR"])) ? Convert.ToString(_reader["CODCT_PADRAO"]) : Convert.ToString(_reader["CODCT_INTEGRATOR"]),
                            ProjetoPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["PROJETO_INTEGRATOR"])) ? Convert.ToString(_reader["PROJETO_PADRAO"]) : Convert.ToString(_reader["PROJETO_INTEGRATOR"]),
                            CodAux = Convert.ToString(_reader["EQUIPE_INTEGRATOR"]),
                            Latitude = _reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LATITUDE"]) : (double?)null,
                            Longitude = _reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LONGITUDE"]) : (double?)null,
                        };
                    }
                }

                return retorno;
            }
        }
        private async Task<List<string>> ExcluiOrdemServico(Documento_ProducaoAniel docProd)
        {
            List<string> erros = new List<string>();
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                string _obs = null;
                string _temOs = "N";
                string numObra = null;
                string dataMaxima = null;
                string usuarioCriacao = null;
                string diaCriacao = null;
                string horaCriacao = null;

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" SELECT A.NUM_OBRA,A.DATA_MAXIMA,A.USUARIO,A.DIA,A.HORA,COALESCE(A.STATUS_EXECUCAO,0)AS STATUS_EXECUCAO,");
                sql.AppendLine("(SELECT FIRST 1 B.CODMAT FROM TB_MATERIAL_TAREFA B");
                sql.AppendLine("WHERE B.CODCT = @CODCT AND B.PROJETO = @PROJETO");
                sql.AppendLine("AND B.NUM_OBRA = A.NUM_OBRA AND B.NUM_DOC = @NUM_DOC) AS MATERIAL");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL ");
                sql.AppendLine(" AND A.NUM_DOC = @NUM_DOC AND A.DIA = @DIA ");

                parametros.Add("@CODCT", docProd.CodCt);
                parametros.Add("@PROJETO", docProd.Projeto);
                parametros.Add("@NUM_OBRA_ORIGINAL", docProd.Numero_Obra_Original);
                parametros.Add("@NUM_DOC", docProd.Num_Doc);
                parametros.Add("@DIA", docProd.Dia);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    _temOs = "N";
                    while (await reader.ReadAsync())
                    {
                        _temOs = "S";
                        numObra = Convert.ToString(reader["NUM_OBRA"]);
                        dataMaxima = Convert.ToString(reader["DATA_MAXIMA"]);
                        usuarioCriacao = Convert.ToString(reader["USUARIO"]);
                        diaCriacao = Convert.ToString(reader["DIA"]);
                        horaCriacao = Convert.ToString(reader["HORA"]);

                        var temMaterial = String.IsNullOrEmpty(Convert.ToString(reader["MATERIAL"])) == false;
                        var osFechada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 1 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 2 || Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 8;
                        var osCancelada = Convert.ToInt64(reader["STATUS_EXECUCAO"]) == 9;

                        if (osFechada == false)
                        {
                            if (osCancelada == false)
                            {
                                if (temMaterial == false)
                                {
                                    using (var comExclusao = session.Connection.CreateCommand())
                                    {
                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_CUSTOS_TAREFA ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        parametros.Add("@NUM_OBRA", numObra);
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        //sql.Clear();
                                        //sql.AppendLine(" DELETE FROM TB_MATERIAL_TAREFA ");
                                        //sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        //sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        //await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);

                                        sql.Clear();
                                        sql.AppendLine(" DELETE FROM TB_DOCUMENTO_PRODUCAO ");
                                        sql.AppendLine(" WHERE CODCT = @CODCT AND PROJETO = @PROJETO ");
                                        sql.AppendLine(" AND NUM_OBRA =  @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                        await CommandHelper.ExecuteNonQuery(comExclusao, sql, parametros, session.Connection, null);
                                    }
                                    _obs = $"A Ordem de Servico {numObra} foi excluída com Sucesso.";
                                    //erros.Add($"A Ordem de Servico {numObra} foi excluída com Sucesso.");
                                }
                                else
                                {
                                    _obs = $"A Ordem de Servico {numObra} possui lançamento de materiais.";
                                    erros.Add($"Ordem de Servico {numObra} possui lançamento de materiais.");
                                }
                            }
                            else
                            {
                                _obs = $"A Ordem de Servico {numObra} encontra-se cancelada.";
                                erros.Add($"Ordem de Servico {numObra} encontra-se cancelada.");
                            }
                        }
                        else
                        {
                            _obs = $"A Ordem de Servico {numObra} encontra-se fechada.";
                            erros.Add($"Ordem de Servico {numObra} encontra-se fechada.");
                        }


                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.CodCt;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.Numero_Obra_Original;
                        pObjLogProd.numDoc = Convert.ToString(docProd.Num_Doc);
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }
                    if (_temOs == "N")
                    {
                        numObra = docProd.Numero_Obra_Original;
                        _obs = $"A Ordem de Servico {docProd.Numero_Obra_Original} não encontrada no sistema.";
                        erros.Add($"A Ordem de Servico {docProd.Numero_Obra_Original} não encontrada no sistema.");

                        DocumentoProducaoLog pObjLogProd = new DocumentoProducaoLog();

                        pObjLogProd.codCt = docProd.CodCt;
                        pObjLogProd.projeto = docProd.Projeto;
                        pObjLogProd.numObra = numObra;
                        pObjLogProd.numObraOriginal = docProd.Numero_Obra_Original;
                        pObjLogProd.numDoc = Convert.ToString(docProd.Num_Doc);
                        pObjLogProd.dataMaxima = String.IsNullOrEmpty(dataMaxima) ? null : dataMaxima;
                        pObjLogProd.usuarioCriacao = usuarioCriacao;
                        pObjLogProd.diaCriacao = String.IsNullOrEmpty(diaCriacao) ? null : diaCriacao;
                        pObjLogProd.horaCriacao = String.IsNullOrEmpty(horaCriacao) ? null : horaCriacao;
                        pObjLogProd.usuarioExclusao = User.Identity.GetUserId();
                        pObjLogProd.nomeMaquina = Dns.GetHostName();
                        IPAddress[] ip = Dns.GetHostAddresses(pObjLogProd.nomeMaquina);
                        if (ip != null && ip.Length >= 10)
                        {
                            pObjLogProd.ipMaquina = ip[9].ToString();
                        }
                        pObjLogProd.obs = _obs;
                        await InsereLog(session, pObjLogProd);
                    }
                }
            }

            return erros;
        }

        private async Task InsereLog(ISession session, DocumentoProducaoLog docProdLog)
        {
            using (var com = session.Connection.CreateCommand())
            {
                string _msn = null;
                try
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    sql.Clear();
                    sql.AppendLine(" INSERT INTO TB_LOG_EXCLUSAO_OS (CODCT,PROJETO,NUM_OBRA,NUM_OBRA_ORIGINAL,NUM_DOC,DATA_MAXIMA,USUARIO, ");
                    sql.AppendLine("                                 DIA,HORA,USUARIO_EXCLUSAO,DIA_EXCLUSAO,HORA_EXCLUSAO, ");
                    sql.AppendLine("                                 HOSTNAME,IP_MAQUINA_ORIGEM,OBS) ");
                    sql.AppendLine("             VALUES (@pCODCT,@pPROJETO,@pNUM_OBRA,@pNUM_OBRA_ORIGINAL,@pNUM_DOC,@pDATA_MAXIMA,@pUSUARIO, ");
                    sql.AppendLine("                     @pDIA,@pHORA,@pUSUARIO_EXCLUSAO,CURRENT_DATE,CURRENT_TIME, ");
                    sql.AppendLine("                     @pHOSTNAME,@pIP_MAQUINA_ORIGEM,@pOBS) ");
                    parametros.Clear();
                    parametros.Add("pCODCT", docProdLog.codCt);
                    parametros.Add("pPROJETO", docProdLog.projeto);
                    parametros.Add("pNUM_OBRA", docProdLog.numObra);
                    parametros.Add("pNUM_OBRA_ORIGINAL", docProdLog.numObraOriginal);
                    parametros.Add("pNUM_DOC", docProdLog.numDoc);
                    parametros.Add("pDATA_MAXIMA", Convert.ToDateTime(docProdLog.dataMaxima));
                    parametros.Add("pUSUARIO", docProdLog.usuarioCriacao);
                    parametros.Add("pDIA", Convert.ToDateTime(docProdLog.diaCriacao));
                    parametros.Add("pHORA", docProdLog.horaCriacao);
                    parametros.Add("pUSUARIO_EXCLUSAO", docProdLog.usuarioExclusao);
                    parametros.Add("pIP_MAQUINA_ORIGEM", docProdLog.ipMaquina);
                    parametros.Add("pHOSTNAME", docProdLog.nomeMaquina);
                    parametros.Add("pOBS", docProdLog.obs);
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
                catch (Exception ex)
                {
                    _msn = ex.Message;
                }
            }
        }

    }
}
