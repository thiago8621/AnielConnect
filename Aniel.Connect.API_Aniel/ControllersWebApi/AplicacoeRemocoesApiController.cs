using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Repository.T5.Material;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class AplicacoesRemocoesApiController : BaseApiController
    { /// <summary>
      /// Retorna a Listagem de aplicações e remoções lançadas em RDOs ou OSs no sistema Aniel.
      /// </summary>
      /// <param name="request">Objeto de request</param>
        [HttpPost]
        [Route("api/AplicacoesRemocoesApi/GetAll")]
        public async Task<ApiResponse<AplicacoesRemocoesListModel>> GetAll(AplicacoesRemocoesApiRequest request)
        {
            List<AplicacoesRemocoesListModel> retorno = new List<AplicacoesRemocoesListModel>();
            int recordsTotal = 0;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODMAT) CO FROM TB_MATERIAL_TAREFA");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    StringBuilder sqlWhere = new StringBuilder();

                    sqlWhere.AppendLine("WHERE A.DATA_APLIC BETWEEN @DATA_INI AND @DATA_FIM AND(A.QUANTIDADE> 0 or A.QUANT_REMOV > 0)");
                    parametros.Add("@DATA_INI", request.Data_Ini);
                    parametros.Add("@DATA_FIM", request.Data_Fim);
                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sqlWhere.Append("AND A.PROJETO = @PROJETO ");
                        parametros.Add("@PROJETO", request.Projeto);
                    }
                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlWhere.Append("AND A.CODCT = @CONTRATO ");
                        parametros.Add("@CONTRATO", request.Contrato);
                    }
                    if (request.Alterado.HasValue)
                    {
                        sqlWhere.Append("AND COALESCE(A.ALTERADO,0) = @ALTERADO ");
                        parametros.Add("@ALTERADO", request.Alterado);
                    }
                    if (request.Comodato)
                    {
                        sqlWhere.Append("AND C.COMODATO = 'S' ");
                    }

                    if (!String.IsNullOrEmpty(request.CodTecnico))
                    {
                        using (var _repoEquipe = new Conexao<EquipeAniel>(sessionT5))
                        {
                            var objEquipe = await _repoEquipe.selectFirst(x => x.EquipeAux == request.CodTecnico || x.Matricula == request.CodTecnico);
                            if (objEquipe == null)
                            {
                                throw new Exception("Equipe não encontrada!!");
                            }
                            sqlWhere.Append("AND A.EQUIPE = @EQUIPE ");
                            parametros.Add("@EQUIPE", objEquipe.Matricula);
                        }
                    }

                    if (request.Integrado.HasValue)
                    {
                        sqlWhere.Append("AND COALESCE(A.INTEGRADO,0) = @INTEGRADO");
                        parametros.Add("@INTEGRADO", request.Integrado);
                    }

                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine(@" 
                        FROM TB_MATERIAL_TAREFA A
                        INNER JOIN TB_MATERIAL C
                        ON(A.CODMAT = C.CODMAT)
                        LEFT JOIN TB_CODIGO_COMPLEMENTAR D ON (A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO
                                                               AND A.CODMAT = D.CODMAT AND A.CODCPL = D.CODCPL)
                        LEFT JOIN TB_EQUIPE E ON (E.EQUIPE = A.EQUIPE)");

                    #region //Select

                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@"
                       SELECT A.CODCT,A.PROJETO, A.NUM_PROTOCOLO, A.NUM_DOC, A.EQUIPE, C.CODCLI,C.DESC_AUXILIAR,
                        A.CODMAT, C.DESCRICAO, C.UNID, A.DATA_APLIC, C.VALOR, C.CLASSIFICACAO, A.NUM_DOC, A.NUM_OBRA, A.DIA, A.DATA_APLIC,
                        A.QUANTIDADE, A.QUANT_REMOV, A.QUANT_CATAL,C.CODAUX, C.CODMAT_AUX,A.CODCPL,A.LOCALIZACAO,A.TAREFA,E.EQUIPEAUX

                       ");

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());

                        #endregion

                        #region lidando com os erros

                        if (String.IsNullOrEmpty(request.Data_Ini))
                        {
                            throw new Exception("Data Inicial é obrigatório");
                        }
                        if (String.IsNullOrEmpty(request.Data_Fim))
                        {
                            throw new Exception("Data Final é obrigatório");
                        }

                        DateTime? Data_Ini = Convert.ToDateTime(request.Data_Ini);
                        DateTime? Data_Fim = Convert.ToDateTime(request.Data_Fim);

                        if ((Data_Fim.Value - Data_Ini.Value).TotalDays >= 90
                         || ((Data_Fim.Value - Data_Ini.Value).TotalDays < 0))
                        {
                            throw new Exception("Informe um período dentro de 90 dias");
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new AplicacoesRemocoesListModel
                                {
                                    //CodCpl_Aux = !String.IsNullOrEmpty(Convert.ToString(reader["CODMAT_AUX"])) ? Convert.ToString(reader["CODMAT_AUX"]) : "-",
                                    CodCpl_Aux = !String.IsNullOrEmpty(Convert.ToString(reader["LOCALIZACAO"])) ? Convert.ToString(reader["LOCALIZACAO"]) : "-",
                                    CodCpl = !String.IsNullOrEmpty(Convert.ToString(reader["CODCPL"])) ? Convert.ToString(reader["CODCPL"]) : "-",
                                    Contrato = !String.IsNullOrEmpty(Convert.ToString(reader["CODCT"])) ? Convert.ToString(reader["CODCT"]) : "-",
                                    Cod_Material = !String.IsNullOrEmpty(Convert.ToString(reader["CODMAT"])) ? Convert.ToString(reader["CODMAT"]) : "-",
                                    Desc_Auxiliar = !String.IsNullOrEmpty(Convert.ToString(reader["DESC_AUXILIAR"])) ? Convert.ToString(reader["DESC_AUXILIAR"]) : "-",
                                    Contrato_Assinante = !String.IsNullOrEmpty(Convert.ToString(reader["CODCLI"])) ? Convert.ToString(reader["CODCLI"]) : "-",
                                    DataHora_Agendamento = !String.IsNullOrEmpty(Convert.ToString(reader["DIA"])) ? Convert.ToString(reader["DIA"]) : "-",
                                    DataHora_Aplicacao = !String.IsNullOrEmpty(Convert.ToString(reader["DATA_APLIC"])) ? Convert.ToString(reader["DATA_APLIC"]) : "-",
                                    Numero_OS = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_DOC"])) ? Convert.ToString(reader["NUM_DOC"]) : "-",
                                    Equipe = !String.IsNullOrEmpty(Convert.ToString(reader["EQUIPE"])) ? Convert.ToString(reader["EQUIPE"]) : "-",
                                    EquipeAux = !String.IsNullOrEmpty(Convert.ToString(reader["EQUIPEAUX"])) ? Convert.ToString(reader["EQUIPEAUX"]) : "-",
                                    Numero_da_Obra = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_OBRA"])) ? Convert.ToString(reader["NUM_OBRA"]) : "-",
                                    Projeto = !String.IsNullOrEmpty(Convert.ToString(reader["PROJETO"])) ? Convert.ToString(reader["PROJETO"]) : "-",
                                    Quantidade_Aplic = !String.IsNullOrEmpty(Convert.ToString(reader["QUANTIDADE"])) ? Convert.ToString(reader["QUANTIDADE"]) : "-",
                                    Quantidade_Removida = !String.IsNullOrEmpty(Convert.ToString(reader["QUANT_REMOV"])) ? Convert.ToString(reader["QUANT_REMOV"]) : "-",
                                    RDO = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_PROTOCOLO"])) ? Convert.ToString(reader["NUM_PROTOCOLO"]) : "-",
                                    Tarefa = !String.IsNullOrEmpty(Convert.ToString(reader["TAREFA"])) ? Convert.ToString(reader["TAREFA"]) : "-"
                                });
                            }
                        }
                    }

                    return new ApiResponse<AplicacoesRemocoesListModel>()
                    {
                        data = retorno,
                        draw = request.settings.length,
                        recordsFiltered = retorno.Count,
                        quantidadeTotalRegistros = recordsTotal,
                        error = "",
                    };
                }
                catch (Exception e)
                {
                    return new ApiResponse<AplicacoesRemocoesListModel>()
                    {
                        error = e.Message
                    };
                }
            }
        }

        /// <summary>
        ///Endpoint para lançar uma remoção de material em uma OS
        ///</summary>
        ///<param name = "request"> </param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/AplicacoesRemocoesApi/LancarAplicacaoRemocaoMaterial")]
        public async Task<PostApiRetorno<string>> LancarAplicacaoRemocaoMaterial(AplicacaoRemocaoMaterialApiRequest request)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoMaterialTarefa = new Conexao<MaterialTarefa>(sessionT5))
                {
                    bool informouOS = !string.IsNullOrEmpty(request.numOS);

                    var osApi = new OsApiController();
                    var materialApi = new MaterialApiController();


                    var OS = string.IsNullOrEmpty(request.numOS) ? null : (await osApi.ConsultarOS(new OsConsultaApiRequest { settings = request.settings, Num_Obra_Original = request.numOS })).data.FirstOrDefault();

                    if (informouOS && OS == null)
                        throw new Exception($"Nao foi possivel encontrar a Ordem de Servico {request.numOS}");

                    await ValidaRequestMaterial(request.Materiais, materialApi, request);


                    if (informouOS)
                    {
                        var materiaisInserir = new List<Material_TarefaAniel>();

                        if (informouOS && OS.Equipe != null && request.Equipe != null)
                        {
                            throw new Exception($"Nao foi possivel encontrar técnico responsável pelo movimento");
                        }

                        foreach (var material in request.Materiais)
                        {
                            Material_TarefaAniel materialInserir = new Material_TarefaAniel
                            {
                                Data_Aplic = DateTime.Now,
                                CodMat = material.Operacao == 'R' ? $"R{material.CodMaterial}" : material.CodMaterial,
                                CodCpl = material.CodCompl,
                                Equipe = OS.Equipe,
                                Quantidade = material.Operacao == 'A' ? material.Quantidade : 0,
                                Quant_Remov = material.Operacao == 'R' ? material.Quantidade : 0,

                                Dia = DateTime.Now,
                                Hora = DateTime.Now,

                                Usuario = request.settings.user,

                                CodCt = OS.CodContrato,
                                Tarefa = string.IsNullOrEmpty(OS.Tarefa) ? "-" : OS.Tarefa,
                                Projeto = OS.Projeto,
                                Num_Obra = OS.NumeroObra,
                                Num_Doc = Convert.ToInt32(OS.NumeroDocumento),
                                Cod_Tipo_Serv = Convert.ToInt16(OS.TipoServico)
                            };

                            materiaisInserir.Add(materialInserir);
                        }

                        await GravarMateriaisTarefa(materiaisInserir, sessionT5);
                    }
                    else //rotina de inventario
                    {
                        if (string.IsNullOrEmpty(request.ClienteCpfCnpj))
                            throw new Exception("CPF/CNPJ obrigatorio");

                        var cliente = await buscaClienteCPFCNPJ(request.ClienteCpfCnpj, sessionT5);

                        if (cliente == null)
                            throw new Exception($"Não foi possível localizar um cliente com o CPF {request.ClienteCpfCnpj} em nosso sistema. Verifique se o número está correto e tente novamente.");


                        var inventarioCodCtProjeto = await InvTouch(sessionT5);
                        var materiaisInserir = new List<Material_TarefaAniel>();
                        var inventario = new CabecalhoInventarioCliente
                        {
                            cpfCnpj = request.ClienteCpfCnpj,
                            data_criacao = DateTime.Now,
                            idCliente = cliente.codCliente,
                            nomeCliente = cliente.fantasia,
                            equipe = request.Equipe
                        };

                        var protocolo = await GerarProtocoloInvCapa(inventario, sessionT5);

                        foreach (var material in request.Materiais)
                        {
                            Material_TarefaAniel materialInserir = new Material_TarefaAniel
                            {
                                Data_Aplic = DateTime.Now,
                                CodMat = material.Operacao == 'R' ? $"R{material.CodMaterial}" : material.CodMaterial,
                                CodCpl = material.CodCompl,
                                Equipe = request.Equipe,
                                Quantidade = material.Operacao == 'A' ? material.Quantidade : 0,
                                Quant_Remov = material.Operacao == 'R' ? material.Quantidade : 0,
                                Dia = DateTime.Now,
                                Hora = DateTime.Now,
                                Usuario = request.settings.user,
                                CodCt = inventarioCodCtProjeto.codct,
                                Tarefa = "-",
                                Projeto = inventarioCodCtProjeto.projeto,
                                Num_Obra = $"API{Convert.ToInt32(protocolo)}",
                                Num_Doc = Convert.ToInt32(protocolo),
                                Cod_Tipo_Serv = Convert.ToInt16(request.TipoServico),
                                Id_Cliente = cliente?.codCliente,

                            };

                            materiaisInserir.Add(materialInserir);
                        }

                        await GravarMateriaisTarefa(materiaisInserir, sessionT5);
                        retorno.IdObjeto = Convert.ToString(protocolo);
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Ok = false;
            }

            return retorno;
        }

        /// <summary>
        ///  Retorna a Listagem de Material Tarefa Alterados e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AplicacoesRemocoesApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<object>>> GetIntegracao(ApiRequest request)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            List<object> retorno = new List<object>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<TransferenciaFornecedor>(sessionT5))
                {
                    var listaBanco = repo.select(x => x.Alterado == true).Result.ToList();

                    foreach (var item in listaBanco)
                    {
                        item.Alterado = false;
                    }

                    await repo.SaveOrUpdate(listaBanco);
                }

                return new ApiResponseObj<List<object>> { ok = true, data = retorno };
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<object>> { ok = true, error = e.Message };
            }
        }

        /// <summary>
        /// Atualiza o campo INTEGRADO para verdadeiro
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AplicacoesRemocoesApi/AlteracaoIntegrado")]
        public async Task<ApiResponseObj<Material_TarefaAniel>> AlteracaoIntegrado(AplicacoesRemocoesApiRequestModel request)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            var retorno = new object();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<Material_TarefaAniel>(sessionT5))
                using (var repoIntegracaoProtheusReverse = new Conexao<IntegracaoProtheusReverseAniel>(sessionT5))
                {
                    var d3Local = !string.IsNullOrEmpty(request.CodLocal) ? request.CodLocal : null;
                    var objIntegracao = repoIntegracaoProtheusReverse.selectFirst(x => x.Empresa == request.CodEmpresa && x.D3_Filial == request.CodFilial && x.D3_Local == d3Local).Result;
                    string codct = objIntegracao.CodCt;
                    string projeto = objIntegracao.Projeto;

                    var materialAnielRepository = new MaterialAnielRepository(sessionT5);
                    var materialAnielBusiness = new MaterialAnielBusiness(materialAnielRepository);
                    var material = await materialAnielBusiness.GetById(request.Cod_Material);

                    if (material is null)
                    {
                        throw new Exception("material não encontrado");
                    }


                    var obj = repo.selectFirst(x => x.CodCt == codct && x.Projeto == projeto && x.Num_Obra == request.Num_Obra
                    && x.Tarefa == request.Tarefa && x.CodMat == material.CodMat && x.CodCpl == request.Cod_Complementar && x.Num_Doc == Convert.ToInt32(request.Num_Doc)
                    && x.Equipe == request.Equipe).Result;

                    if (obj != null)
                    {
                        obj.Integrado = true;

                        await repo.SaveOrUpdate(obj);
                        return new ApiResponseObj<Material_TarefaAniel> { ok = true, data = obj };
                    }
                    return new ApiResponseObj<Material_TarefaAniel> { ok = false, error = "Erro ao realizar a alteração, verifique os campos informados!!" };
                }

            }
            catch (Exception e)
            {
                return new ApiResponseObj<Material_TarefaAniel> { ok = false, error = e.Message };
            }
        }

        public async Task ValidaRequestMaterial(List<AplicacaoRemocaoMaterial> materiais, MaterialApiController materialApi, AplicacaoRemocaoMaterialApiRequest request)
        {
            int i = 1;

            foreach (var material in materiais)
            {
                if (string.IsNullOrEmpty(material.CodMaterial))
                    throw new Exception($"Material numero {i} da lista nao tem codigo informado");

                if (material.Quantidade < 1)
                    throw new Exception($"Material numero {i} da lista ({material.CodMaterial}), nao tem quantidade informada");

                var buscaMaterial = (await materialApi.BuscarPorCodigo(new Material_ApiRequest { settings = request.settings, Codigo_Material = material.CodMaterial })).data;

                if (buscaMaterial == null)
                    throw new Exception($"Material numero {i} da lista ({material.CodMaterial}), nao cadastrado");

                if (buscaMaterial.SubCod == "S" && string.IsNullOrEmpty(material.CodCompl))
                    throw new Exception($"Material numero {i} da lista ({material.CodMaterial}), nao tem serial informado");

                if (buscaMaterial.SubCod == "S" && material.Quantidade > 1)
                    throw new Exception($"Material numero {i} da lista ({material.CodMaterial}), material com serial deve possuir quantidade igual a 1");

                i++;
            }
        }

        public async Task<bool> GravarMateriaisTarefa(List<Material_TarefaAniel> materiais, ISession sessionT5)
        {
            StringBuilder sql = new StringBuilder();
            var transaction = CommandHelper.BeginTransaction(sessionT5.Connection);

            using (var com = sessionT5.Connection.CreateCommand())
            {
                int i = 0;

                foreach (var item in materiais)
                {

                    Dictionary<string, object> parametros = new Dictionary<string, object>
                    {
                        { $"codCt{i}", item.CodCt },
                        { $"projeto{i}", item.Projeto },
                        { $"numObra{i}", item.Num_Obra },
                        { $"tarefa{i}", item.Tarefa },
                        { $"codMat{i}", item.CodMat },
                        { $"codCpl{i}", item.CodCpl },
                        { $"numDoc{i}", Convert.ToInt32(item.Num_Doc) },
                        { $"equipe{i}", item.Equipe },
                        { $"codServ{i}", item.Cod_Tipo_Serv },
                        { $"quantidade{i}", item.Quantidade },
                        { $"quantidadeRemov{i}", item.Quant_Remov },
                        { $"data_aplic{i}", item.Data_Aplic },
                        { $"usuario{i}", item.Usuario },
                        { $"cliente{i}", item.Id_Cliente },
                    };

                    sql = new StringBuilder($@"INSERT INTO TB_MATERIAL_TAREFA (CODCT, PROJETO, NUM_OBRA, TAREFA, CODMAT, CODCPL, NUM_DOC, EQUIPE, COD_TIPO_SERV,QUANTIDADE,QUANT_REMOV, DATA_APLIC, USUARIO,ID_CLIENTE,DIA,HORA)
                                               VALUES (@codct{i}, @projeto{i}, @numObra{i}, @tarefa{i}, @codMat{i}, @codCpl{i}, @numDoc{i}, @equipe{i}, @codServ{i},@quantidade{i},@quantidadeRemov{i},@data_aplic{i},@usuario{i},@cliente{i},CURRENT_DATE,CURRENT_TIMESTAMP );");

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, transaction);

                    StringBuilder sqlLog = new StringBuilder("INSERT INTO TB_LOG_LANC_SERVICO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, ALTERACAO, VALOR_ANT, VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, ")
                             .AppendLine(" USUARIO, DIA, HORA, DATA_MAXIMA, COD_TIPO_SERV) ")
                             .AppendLine(" VALUES(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @ALTERACAO, @VALOR_ANT,@VALOR_ATUALIZADO,@OBS_STATUS_COBRANCA, ")
                             .AppendLine(" @USUARIO, CURRENT_DATE, CURRENT_TIME, @DATA_MAXIMA, @COD_TIPO_SERV) ");

                    parametros.Add("@CODCT", item.CodCt);
                    parametros.Add("@PROJETO", item.Projeto);
                    parametros.Add("@NUM_OBRA", item.Num_Obra);
                    parametros.Add("@NUM_DOC", item.Num_Doc);


                    parametros.Add("@ALTERACAO", "Incluiu Material");
                    parametros.Add("@VALOR_ANT", null);
                    parametros.Add("@VALOR_ATUALIZADO", item.CodMat + " -" + "Compl.: " + item.CodCpl);
                    parametros.Add("@OBS_STATUS_COBRANCA", null);
                    parametros.Add("@USUARIO", item.Usuario);
                    parametros.Add("@DATA_MAXIMA", item.Data_Aplic);
                    parametros.Add("@COD_TIPO_SERV", item.Cod_Tipo_Serv);
                    await CommandHelper.ExecuteNonQuery(com, sqlLog, parametros, sessionT5.Connection, transaction);






                    // await InsertLog_Lanc_Servico(sessionT5, item.CodCt, item.Projeto, item.Num_Obra, item.Num_Doc, "Incluiu Material",nullnull, item.Usuario, item.Data_Aplic, item.Cod_Tipo_Serv, item.CodMat + " -" + "Compl.: " + item.CodCpl, null);

                    i++;
                }

                transaction.Commit();

            }

            return true;
        }


        public async Task<ClienteInventario> buscaClienteCPFCNPJ(String CPFCNPJ, ISession sessionT5)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            ClienteInventario retorno = null;

            if (!String.IsNullOrEmpty(CPFCNPJ) && !String.IsNullOrEmpty(Regex.Replace(CPFCNPJ, @"[^\d]", "")))
            {
                sql.Append("SELECT ");
                sql.Append("COD_CLIENTE, RAZAO_SOCIAL, CNPJ, INSC_ESTADUAL, INSC_MUNICIPAL, ENDERECO, BAIRRO, ");
                sql.Append("MUNICIPIO, CEP, UF, PAIS, TELEFONE, CELULAR, FAX, CONTATO, EMAIL, TIPO, FANTASIA, ");
                sql.Append("COMPLEMENTO, ENDERECO_NUMERO ");
                sql.Append("FROM TB_CLIENTE ");
                sql.Append("WHERE CNPJ = @CNPJ");

                parametros.Add("@CNPJ", CPFCNPJ);

                using (var com = sessionT5.Connection.CreateCommand())
                using (var _reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (_reader.Read())
                    {
                        retorno = new ClienteInventario()
                        {
                            codCliente = Convert.ToInt64(_reader["COD_CLIENTE"]),
                            razaoSocial = _reader["RAZAO_SOCIAL"].ToString(),
                            cnpj = _reader["CNPJ"].ToString(),
                            inscEstadual = _reader["INSC_ESTADUAL"].ToString(),
                            inscMunicipal = _reader["INSC_MUNICIPAL"].ToString(),
                            endereco = _reader["ENDERECO"].ToString(),
                            bairro = _reader["BAIRRO"].ToString(),
                            municipio = _reader["MUNICIPIO"].ToString(),
                            CEP = _reader["CEP"].ToString(),
                            UF = _reader["UF"].ToString(),
                            pais = _reader["PAIS"].ToString(),
                            telefone = _reader["TELEFONE"].ToString(),
                            celular = _reader["CELULAR"].ToString(),
                            fax = _reader["FAX"].ToString(),
                            contato = _reader["CONTATO"].ToString(),
                            email = _reader["EMAIL"].ToString(),
                            tipo = _reader["TIPO"].ToString(),
                            fantasia = _reader["FANTASIA"].ToString(),
                            complemento = _reader["COMPLEMENTO"].ToString(),
                            enderecoNumero = _reader["ENDERECO_NUMERO"].ToString()
                        };
                    }
                }
            }

            return retorno == null ? null : retorno;
        }

        public async Task<Int64> GerarProtocoloInvCapa(CabecalhoInventarioCliente inventario, ISession sessionT5)
        {
            Int64 retorno = 0;
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            sql.Append("INSERT INTO TB_INVENTARIO_CLIENTE(SEQUENCIAL, ID_CLIENTE, DATA, NOME_CLIENTE, ORIGEM, CPF_CLIENTE, USUARIO_CRIACAO, STATUS) ");
            sql.Append("VALUES(@SEQUENCIAL, @CLIENTE, @DATA, @NOME, @ORIGEM, @CPF, @USUARIO_CRIACAO, @STATUS) ");
            sql.Append("RETURNING SEQUENCIAL ");

            var proxseq = await getProximoSequencial(sessionT5);
            parametros.Add("@SEQUENCIAL", proxseq);
            parametros.Add("@CLIENTE", inventario.idCliente);
            parametros.Add("@DATA", DateTime.Now);
            parametros.Add("@ORIGEM", 0);
            parametros.Add("@NOME", inventario.nomeCliente);
            parametros.Add("@CPF", inventario.cpfCnpj);
            parametros.Add("@USUARIO_CRIACAO", inventario.equipe);
            parametros.Add("@STATUS", 0);

            using var com = sessionT5.Connection.CreateCommand();

            var gravou = await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            if (gravou > 0)
            {
                retorno = Convert.ToInt64(proxseq);
            }

            return retorno;
        }

        public async Task<string> getProximoSequencial(ISession sessionT5)
        {
            int proxSequencial = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COALESCE(MAX(CAST(A.SEQUENCIAL AS INTEGER)),0) + 1 AS SEQ FROM TB_INVENTARIO_CLIENTE A ");

            using (var com = sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), sessionT5.Connection, null))
            {
                if (reader.Read())
                {
                    proxSequencial = Convert.ToInt32(reader["SEQ"].ToString());
                }
            }
            return String.Format("{0:0000000000}", proxSequencial);
        }

        public async Task<int> getProximoIDMaterial(ISession sessionT5, DbTransaction transaction)
        {
            int proxId = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COALESCE(MAX(CAST(A.ID AS INTEGER)),0) + 1 AS ID FROM TB_INVENTARIO_CLIENTE_MAT A");

            using (var com = sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), sessionT5.Connection, transaction))
                if (reader.Read())
                {
                    proxId = Convert.ToInt32(reader["ID"].ToString());
                }

            return proxId;
        }

        public async Task<Boolean> GetBaixaInventarioAutomatico(ISession sessionT5)
        {
            Boolean retorno = true;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT BAIXA_INVENTARIO_AUTOMATICO FROM TB_MASTER ");

            using (var com = sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), sessionT5.Connection, null))
                if (reader.Read())
                {
                    retorno = (reader["BAIXA_INVENTARIO_AUTOMATICO"] != null ? reader["BAIXA_INVENTARIO_AUTOMATICO"].ToString() : "S") == "S";
                }

            return retorno;
        }

        private async Task<(string codct, string projeto)> InvTouch(ISession sessionT5)
        {
            string codct = null;
            string projeto = null;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT FIRST 1 CODCT_INV_TOUCH, PROJETO_INV_TOUCH FROM TB_MASTER");

            using (var com = sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), sessionT5.Connection, null))
                if (reader.Read())
                {
                    codct = reader["CODCT_INV_TOUCH"] != null ? reader["CODCT_INV_TOUCH"].ToString() : null;
                    projeto = reader["PROJETO_INV_TOUCH"] != null ? reader["PROJETO_INV_TOUCH"].ToString() : null;
                }

            return (codct, projeto);
        }

        public List<Material_TarefaAniel> ConvertListaMaterialTarefaInventario(List<AplicacaoRemocaoMaterial> materiais)
        {
            var retorno = new List<Material_TarefaAniel>();

            foreach (var item in materiais)
            {
                var novo = new Material_TarefaAniel
                {
                    CodMat = item.CodMaterial,
                    CodCpl = item.CodCompl,
                    Quantidade = item.Quantidade,
                };

                retorno.Add(novo);
            }

            return retorno;
        }

        private async Task InserirMaterialCliente(Int64 IdCliente, List<Material_TarefaAniel> material, Int64 protocolo, ISession sessionT5)
        {
            var data_hora = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using var com = sessionT5.Connection.CreateCommand();
            DbTransaction transaction = CommandHelper.BeginTransaction(sessionT5.Connection);

            sql.Append("INSERT INTO TB_INVENTARIO_CLIENTE_MAT(ID, SEQUENCIAL, CODMAT, CODCPL, USUARIO, DIA, HORA, QTDE_LEVANTADA) ");
            sql.Append("VALUES(@ID, @SEQUENCIAL, @CODMAT, @CODCPL, @USUARIO, CURRENT_DATE, CURRENT_TIME, @QTDE_LEVANTADA) ");

            parametros.Add("@SEQUENCIAL", String.Format("{0:0000000000}", protocolo));
            parametros.Add("@USUARIO", "API ANIEL");
            parametros.Add("@DIA_HORA", data_hora);

            foreach (var item in material)
            {
                var proxId = (await getProximoIDMaterial(sessionT5, transaction)).ToString();

                if (!parametros.ContainsKey("@ID"))
                {
                    parametros.Add("@ID", proxId);
                }
                else
                {
                    parametros["@ID"] = proxId;
                }

                if (!parametros.ContainsKey("@QTDE_LEVANTADA"))
                {
                    parametros.Add("@QTDE_LEVANTADA", item.Quantidade);
                }
                else
                {
                    parametros["@QTDE_LEVANTADA"] = item.Quantidade;
                }

                if (!parametros.ContainsKey("@CODMAT"))
                {
                    if (!parametros.ContainsValue(item.CodMat))
                    {
                        parametros.Add("@CODMAT", item.CodMat);
                    }
                }
                else
                {
                    parametros["@CODMAT"] = item.CodMat;
                }

                if (item.CodCpl != null)
                {
                    if (!parametros.ContainsKey("@CODCPL"))
                    {
                        parametros.Add("@CODCPL", item.CodCpl);
                    }
                    else
                    {
                        parametros["@CODCPL"] = item.CodCpl;
                    }

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, transaction);
                }
                else
                {
                    parametros["@CODCPL"] = "-";
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, transaction);
                }
            }

            transaction.Commit();
        }

        private async Task<string> BaixaInventarioCliente(Int64 protocolo, ISession sessionT5)
        {
            Boolean baixado = false;
            String sequencial = null;
            String Obs = null;

            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            sql.Append("SELECT RSEQUENCIAL, RBAIXADO, ROBS FROM SP_BAIXA_INVENTARIO_CLIENTE(@SEQUENCIAL) ");
            parametros.Add("@SEQUENCIAL", String.Format("{0:0000000000}", protocolo));

            using (var com = sessionT5.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                if (reader.Read())
                {
                    baixado = (reader["RBAIXADO"] != null ? reader["RBAIXADO"].ToString() : "N") == "S";
                    sequencial = reader["RSEQUENCIAL"] != null ? reader["RSEQUENCIAL"].ToString() : null;
                    Obs = reader["ROBS"] != null ? reader["ROBS"].ToString() : null;
                }

            if (!baixado)
            {
                throw new EAnielException($"Sequencial: {sequencial} - {Obs}");
            }
            else
            {
                return Obs;
            }
        }
        private async Task<Boolean> InsertLog_Lanc_Servico(ISession session, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc,
            String Alteracao, String Valor_Anterior, String Usuario, DateTime? Data_Maxima, Int16? Cod_Tipo_Serv, String Valor_Atualizado, String Obs_Cobranca_Status, ITransaction transaction = null)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder("INSERT INTO TB_LOG_LANC_SERVICO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, ALTERACAO, VALOR_ANT, VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, ")
                            .AppendLine(" USUARIO, DIA, HORA, DATA_MAXIMA, COD_TIPO_SERV) ")
                            .AppendLine(" VALUES(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @ALTERACAO, @VALOR_ANT,@VALOR_ATUALIZADO,@OBS_STATUS_COBRANCA, ")
                            .AppendLine(" @USUARIO, CURRENT_DATE, CURRENT_TIME, @DATA_MAXIMA, @COD_TIPO_SERV) ");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Os);
                parametros.Add("@NUM_DOC", Num_Doc);

                parametros.Add("@ALTERACAO", Alteracao);
                parametros.Add("@VALOR_ANT", Valor_Anterior);
                parametros.Add("@VALOR_ATUALIZADO", Valor_Atualizado);
                parametros.Add("@OBS_STATUS_COBRANCA", Obs_Cobranca_Status);
                parametros.Add("@USUARIO", Usuario);
                parametros.Add("@DATA_MAXIMA", Data_Maxima);
                parametros.Add("@COD_TIPO_SERV", Cod_Tipo_Serv);

                if (transaction != null)
                {
                    transaction.Enlist(com);
                }

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction != null ? com.Transaction : null);

                return true;

            }

        }

    }
}
