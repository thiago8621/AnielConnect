using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Testing;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerado;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Custos_TarefaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Custos_TarefaApi/Prod_Lancada")]
        public async Task<DataTableResponse<Prod_LancadaListaModel>> GetProd_Lancada(Material_Aplicado_DataTableRequest request)
        {
            List<Prod_LancadaListaModel> retorno = new List<Prod_LancadaListaModel>();
            IOrderedEnumerable<Prod_LancadaListaModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                String SIGLA_MODULO = await GetSiglaModulo(session, request.Num_Obra, Convert.ToInt32(request.Num_Doc), request.Projeto, request.CodCt);
                String CHAVE = GetCampoChave(SIGLA_MODULO);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", request.CodCt);
                parametros.Add("@PROJETO", request.Projeto);
                parametros.Add("@NUM_OBRA", String.Format("{0}%", request.Num_Obra.Split('/')[0]));
                parametros.Add("@NUM_DOC", request.Num_Doc);

                if (SIGLA_MODULO == "MCO")
                    parametros.Add("@CHAVE", String.Format("{0}", request.Num_Obra));
                else
                    parametros.Add("@CHAVE", String.Format("{0}%", request.Num_Obra.Split('/')[0]));


                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder($@"SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.TAREFA, 
                                                                    A.UNIDADE_CUSTO, A.EQUIPE, D.NOME, A.TIPO_PRODUCAO, A.DATA_PROD, A.USUARIO, A.DIA, A.HORA,
                                                                    A.CODIGO, B.DESCRICAO, B.QUANTIDADE AS VALOR, A.COD_PRECO, A.COD_TIPO_SERV, A.COD_SUB_TIPO_SERV,
                                                                    A.QUANT_CATAL AS QUANT_PREVISTA, A.QUANT_PROD AS QUANT_REALIZADA 
                                                             FROM TB_CUSTOS_TAREFA A 
                                                             INNER JOIN TB_CADERNO_ENCARGOS B 
                                                             ON(A.CODIGO = B.CODIGO AND A.UNIDADE_CUSTO = B.UNIDADE_CUSTO) 
                                                             LEFT JOIN SP_BUSCAR_VALOR_SERVICO(A.CODCT, A.PROJETO, NULL, A.UNIDADE_CUSTO, A.COD_PRECO) C 
                                                             ON(B.CLASSE_MO = C.RCLASSE_MO) 
                                                             INNER JOIN TB_EQUIPE D 
                                                             ON(A.EQUIPE = D.EQUIPE) 
                                                             WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.{ CHAVE } LIKE @CHAVE 
                                                               AND A.NUM_DOC = @NUM_DOC");



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Prod_LancadaListaModel
                            {
                                CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                Num_Obra = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty,
                                Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : 0,
                                Tarefa = reader["TAREFA"] != DBNull.Value ? Convert.ToString(reader["TAREFA"]) : String.Empty,
                                Unidade_Custo = reader["UNIDADE_CUSTO"] != DBNull.Value ? Convert.ToString(reader["UNIDADE_CUSTO"]) : String.Empty,
                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : String.Empty,
                                NomeEquipe = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) + " - " +Convert.ToString(reader["NOME"]) : String.Empty,
                                Tipo_Producao = reader["TIPO_PRODUCAO"] != DBNull.Value ? Convert.ToString(reader["TIPO_PRODUCAO"]) : String.Empty,
                                Data_Prod = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]).ToString("dd/MM/yyyy") : String.Empty,
                                Codigo = reader["CODIGO"] != DBNull.Value ? Convert.ToString(reader["CODIGO"]) : String.Empty,
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                Cod_Preco = reader["COD_PRECO"] != DBNull.Value ? Convert.ToString(reader["COD_PRECO"]) : String.Empty,
                                Cod_Sub_Tipo_Serv = reader["COD_SUB_TIPO_SERV"] != DBNull.Value ? Convert.ToString(reader["COD_SUB_TIPO_SERV"]) : String.Empty,
                                Cod_Tipo_Serv = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt16(reader["COD_TIPO_SERV"]) : (Int16)0,
                                Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]).ToString("N2") : "0,00",
                                Quant_Catal = reader["QUANT_PREVISTA"] != DBNull.Value ? Convert.ToDouble(reader["QUANT_PREVISTA"]).ToString("N2") : "0,00",
                                Quant_Prod = reader["QUANT_REALIZADA"] != DBNull.Value ? Convert.ToDouble(reader["QUANT_REALIZADA"]).ToString("N2") : "0,00",
                                Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : String.Empty,
                                Dia = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]).ToString("dd/MM/yyyy") : String.Empty,
                                Hora = reader["HORA"] != DBNull.Value ? ((TimeSpan)reader["HORA"]).ToString(@"hh\:mm") : String.Empty,
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Codigo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Prod_LancadaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count,
                data = listaOrdenada,
                error = ""
            };

        }

        public String GetCampoChave(string sigla_modulo)
        {           
            return sigla_modulo == "MCO" ? "TAREFA" : "NUM_OBRA";
        }

        public async Task<String> GetSiglaModulo(ISession session, String num_Obra, Int32 num_Doc, String projeto, String cod_Ct)
        {

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", cod_Ct);
                parametros.Add("@PROJETO", projeto);
                parametros.Add("@NUM_OBRA", String.Format("{0}%", num_Obra.Split('/')[0]));
                parametros.Add("@NUM_DOC", num_Doc);

                StringBuilder sql = new StringBuilder(@"SELECT D.SIGLA_MODULO FROM TB_DOCUMENTO_PRODUCAO D
                                                         WHERE D.CODCT = @CODCT AND D.PROJETO = @PROJETO AND
                                                               D.NUM_OBRA LIKE @NUM_OBRA AND D.NUM_DOC = @NUM_DOC");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return Convert.ToString(reader["SIGLA_MODULO"]);
                    }
                }
            }

            return String.Empty;

        }

        public async Task<String> GetCodPrecoPadrao(ISession session, String cod_ct, String projeto, String unidade_custo, String classe_mo)
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

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return Convert.ToString(reader["COD_PRECO"]);
                    }
                }
            }

            return retorno;

        }

        [HttpPost()]
        [Route("api/Custos_TarefaApi/Inserir")]
        public async Task<PostApiRetorno<Prod_LancadaListaModel>> Inserir(ParamWebApi<Prod_LancadaListaModel> value)
        {
            PostApiRetorno<Prod_LancadaListaModel> retorno = new PostApiRetorno<Prod_LancadaListaModel>();

            try
            {
                Custos_TarefaAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Custos_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryCaderno = new Conexao<Caderno_EncargosAniel>(session))
                    using (var apiEquipe = new EquipeApiController())
                    using (var apiFechamento = new FechamentoPeriodoApiController())
                    using (var apiDocumento = new Documento_ProducaoApiController())
                    {
                        if (value.Objeto.Num_Obra.Contains("/")) {
                            value.Objeto.Num_Obra = value.Objeto.Num_Obra.Split('/')[0];
                        }

                        var lista_os = await repositoryDoc.select(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Doc == value.Objeto.Num_Doc && p.Numero_Obra_Original == value.Objeto.Num_Obra);
                        var documento = lista_os.OrderByDescending(p => p.Seq).FirstOrDefault();

                        if (String.IsNullOrEmpty(value.Objeto.Codigo))
                            throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODIGO));

                        modelo = (await repository.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == documento.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc &&
                                                                    p.Codigo == value.Objeto.Codigo));

                        if (modelo == null)
                        {
                            if (String.IsNullOrEmpty(documento.Unidade_Custo))
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.UNIDADE_CUSTO));

                            var caderno = await repositoryCaderno.selectFirst(p => p.Codigo == value.Objeto.Codigo && p.Unidade_Custo == documento.Unidade_Custo);

                            var cod_precoPadrao = await GetCodPrecoPadrao(session, documento.CodCt, documento.Projeto, documento.Unidade_Custo, caderno.Classe_MO);
                            var data_Prod = documento.Data_Prod ?? documento.Data_Maxima ?? DateTime.Now;

                            if (String.IsNullOrEmpty(documento.Equipe))
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.EQUIPE));

                            if (await apiEquipe.Afastado(session, documento.Equipe, null))
                                throw new Exception(String.Format(RsCadastros.EQUIPE_ENCERRADA, documento.Equipe));

                            if (String.IsNullOrEmpty(cod_precoPadrao))
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODPRECO_PADRAO));

                            if (documento.Roterizar == "S")
                                throw new Exception(RsCadastros.TANOMOBILE);


                            var periodo = await apiFechamento.PeriodoFechado(session, documento.CodCt, documento.Projeto, DateTime.Now);
                            if (periodo.Item1 != null)
                                throw new Exception(String.Format(RsCadastros.CONTRATO_PROJETO_PERIODO_FECHADO, documento.CodCt, documento.Projeto, periodo.Item1.Value.ToString("dd/MM/yyyy"), periodo.Item2.Value.ToString("dd/MM/yyyy")));

                            modelo = new Custos_TarefaAniel
                            {
                                CodCt = documento.CodCt,
                                Projeto = documento.Projeto,
                                Num_Obra = documento.Num_Obra,
                                Num_Doc = documento.Num_Doc,
                                Data_Prod = data_Prod,
                                Cod_Tipo_Serv = documento.Cod_Tipo_Serv,
                                Cod_Sub_Tipo_Serv = documento.Cod_Sub_Tipo_Serv ?? "-",
                                Unidade_Custo = documento.Unidade_Custo,
                                Tipo_Producao = documento.Tipo_Producao ?? "AC",
                                Cod_Preco = cod_precoPadrao,
                                Equipe = documento.Equipe,
                                Tarefa = "-",
                                Codigo = value.Objeto.Codigo,
                                Quant_Catal = 0,
                                Quant_Prod = Convert.ToDouble(value.Objeto.Quant_Prod),
                                Dia = DateTime.Now,
                                Hora = DateTime.Now,
                                Usuario = value.Usuario
                            };

                            retorno.IdObjeto = new Prod_LancadaListaModel
                            {
                                CodCt = modelo.CodCt,
                                Projeto = modelo.Projeto,
                                Num_Obra = modelo.Num_Obra,
                                Num_Doc = modelo.Num_Doc,
                                Unidade_Custo = modelo.Unidade_Custo,
                                Tarefa = modelo.Tarefa,
                                Equipe = modelo.Equipe,
                                Cod_Tipo_Serv = modelo.Cod_Tipo_Serv,
                                Cod_Sub_Tipo_Serv = modelo.Cod_Sub_Tipo_Serv,
                                Tipo_Producao = modelo.Tipo_Producao,
                                Codigo = modelo.Codigo,
                                Data_Prod = modelo.Data_Prod?.ToString("dd/MM/yyyy"),
                                Cod_Preco = modelo.Cod_Preco,
                                Descricao = caderno.Descricao,
                                Dia = modelo.Dia?.ToString("dd/MM/yyyy"),
                                Hora = modelo.Hora?.ToString("HH:mm"),
                                NomeEquipe = $"{modelo.Equipe} - {await apiEquipe.GetNome(session, modelo.Equipe)}",
                                Quant_Catal = "0.00",
                                Quant_Prod = modelo.Quant_Prod.HasValue ? modelo.Quant_Prod.Value.ToString("N2") : "0.00",
                                Usuario = modelo.Usuario,
                                Valor = caderno.Quantidade.HasValue ? caderno.Quantidade.Value.ToString("N2") : "0.00"
                            };


                        }
                        else
                        {
                            throw new Exception(RsCadastros.CADERNO_ENCARGOS_JA_LANCADO);
                        }

                        await repository.SaveOrUpdate(modelo);

                        await apiDocumento.InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Alteração - Incluiu Produção", null, value.Usuario, documento.Data_Maxima, documento.Cod_Tipo_Serv,null,null);

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

        [Route("api/Custos_TarefaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Prod_LancadaChave>> Excluir(ParamWebApi<Prod_LancadaChave> value)
        {
            DeleteApiRetorno<Prod_LancadaChave> retorno = new DeleteApiRetorno<Prod_LancadaChave>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Custos_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var apiDocumento = new Documento_ProducaoApiController())
                    {

                        Custos_TarefaAniel modelo = null;
                        if (value.Objeto?.Codigo != String.Empty)
                        {
                            var documento = await repositoryDoc.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);

                            modelo = (await repository.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc &&
                                                                        p.Tarefa == value.Objeto.Tarefa && p.Unidade_Custo == value.Objeto.Unidade_Custo && p.Codigo == value.Objeto.Codigo));


                            await apiDocumento.InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Alteração - Excluiu Produção", null, value.Usuario, documento.Data_Maxima, documento.Cod_Tipo_Serv,null,null);

                            await repository.Remove(modelo);

                            retorno.IdObjeto = value.Objeto;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
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
    }
}