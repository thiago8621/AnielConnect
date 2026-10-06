using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Models;
using System.Web.Http;
using System.Text;
using NHibernate;
using Sinapse.DAO.repository;
using System.Threading.Tasks;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Sinapse.Util;
using System.ComponentModel.DataAnnotations;
using Sinapse.Util.Excel;
using System.Reflection;
using Aniel.Connect.Models.T5;
using Microsoft.AspNet.Identity;
using System.Globalization;
using Aniel.T5.Model.enumerado;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Natureza;
using Sinapse.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using CsvHelper;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class ImportacaoOsApiController : BaseApiController
    {
        public Int64 _codTipoServ;
        public Int64 _codSubTipoServ;
        readonly TimeSpan constanteHorarioInicioAgendamento = TimeSpan.FromHours(8); 

        [HttpPost]
        [Route("api/ImportacaoOsApi/GetCamposImportacao")]
        public async Task<GetApiRetorno<List<ImportacaoOsCampo>>> GetCamposImportacao(ImportacaoCampoRequest request)
        {
            GetApiRetorno<List<ImportacaoOsCampo>> retorno = new GetApiRetorno<List<ImportacaoOsCampo>>
            {
                Objeto = new List<ImportacaoOsCampo>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    if (request.TipoOS == "C")  {
                        retorno.Objeto = await CarregaConfiguracao(session, request.Projeto, request.Contrato);
                    } else {
                        retorno.Objeto = await CarregaConfiguracaoManutencao(session, request.Projeto, request.Contrato);
                    }

                    if (retorno.Objeto.Count == 0)
                    {
                        if (request.TipoOS == "C")
                        {
                            retorno.Objeto = await CarregaNovaConfiguracao(session);
                        }
                        else
                        {
                            retorno.Objeto = await CarregaNovaConfiguracaoManutencao(session);
                        }
                    }

                    return retorno;
                }
                catch (Exception e)
                {
                    return new GetApiRetorno<List<ImportacaoOsCampo>> { Mensagem = e.Message, Ok = false, InnerException = e.InnerException.Message };
                }
            }
        }
        [HttpPost]
        [Route("api/ImportacaoOsApi/Log")]
        public async Task<DataTableResponse<LogImportacao>> Log(LogRequest request)
        {
            List<LogImportacao> retorno = new List<LogImportacao>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT * FROM TB_LOG_IMPORTACAO ");

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("DATA", Convert.ToDateTime(request.Data));

                    sql.AppendLine(" WHERE DATA = @DATA ");
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new LogImportacao { Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy"), Hora = Convert.ToDateTime(reader["HORA"].ToString()).ToString("HH:mm"), Erro = reader["ERRO"].ToString(), OS = reader["OPERACAO"].ToString(), Usuario = reader["USUARIO"].ToString() });
                    }
                }
            }

            return new DataTableResponse<LogImportacao>
            {
                data = retorno
            };

        }

        [HttpPost]
        [Route("api/ImportacaoOsApi/SalvaConfiguracao")]
        public async Task<GetApiRetorno> SalvaConfiguracao(ConfiguracaoSaveRequest request)
        {
            List<ConfigImportacaoDados> configuracaoBanco = new List<ConfigImportacaoDados>();
            List<ConfigImportacaoDadosManut> configuracaoBancoManut = new List<ConfigImportacaoDadosManut>();
            List<ConfigImportacaoDados> configuracaoExcluir = new List<ConfigImportacaoDados>();
            List<ConfigImportacaoDadosManut> configuracaoExcluirManut = new List<ConfigImportacaoDadosManut>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoConfig = new Conexao<ConfigImportacaoDados>(session))
                using (var repoConfigManut = new Conexao<ConfigImportacaoDadosManut>(session))
                {
                    foreach (string[] coluna in request.Campos)
                    {
                        bool importar = Convert.ToBoolean(coluna[4]);

                        if (importar)
                        {
                            string campoAniel = coluna[1] != null ? coluna[1] : "";

                            if (request.TipoOS == "C")
                            {
                                configuracaoBanco.Add(new ConfigImportacaoDados { IdConfig = coluna[0] != null ? Convert.ToInt64(coluna[0]) : 0, CodCt = request.Contrato, Projeto = request.Projeto, Planilha = 1, CampoPlanilha = coluna[3] != null ? coluna[3] : "" });
                            }else
                            {
                                configuracaoBancoManut.Add(new ConfigImportacaoDadosManut { IdConfig = coluna[0] != null ? Convert.ToInt64(coluna[0]) : 0, CodCt = request.Contrato, Projeto = request.Projeto, Planilha = 1, CampoPlanilha = coluna[3] != null ? coluna[3] : "" });
                            }   
                        }
                        else
                        {
                            if (request.TipoOS == "C")
                            {
                                configuracaoExcluir.Add(new ConfigImportacaoDados { IdConfig = coluna[0] != null ? Convert.ToInt64(coluna[0]) : 0, CodCt = request.Contrato, Projeto = request.Projeto, Planilha = 1, CampoPlanilha = coluna[3] != null ? coluna[3] : "" });
                            }
                            else {
                                configuracaoExcluirManut.Add(new ConfigImportacaoDadosManut { IdConfig = coluna[0] != null ? Convert.ToInt64(coluna[0]) : 0, CodCt = request.Contrato, Projeto = request.Projeto, Planilha = 1, CampoPlanilha = coluna[3] != null ? coluna[3] : "" });
                            }
                        }
                    }
                    await repoConfig.SaveOrUpdate(configuracaoBanco);
                    await repoConfigManut.SaveOrUpdate(configuracaoBancoManut);
                    await repoConfig.Remove(configuracaoExcluir);
                    await repoConfigManut.Remove(configuracaoExcluirManut);

                    return new GetApiRetorno { Ok = true };
                }
            }
            catch (Exception e)
            {
                return new GetApiRetorno { Ok = false, InnerException = e.InnerException.Message, Mensagem = e.Message };
            }
        }

        [Route("api/ImportacaoOsApi/EnviarPlanilha")]
        public async Task<GetApiRetorno> EnviarPlanilha(ExcelServiceLog<Dictionary<String, Object>> modelo, List<DocumentoProducao_ImportacaoOS> documentos, string contrato, string projeto, string unidadeCusto, string tipoOS)
        {
            List<Documento_ProducaoAniel> listaBanco = new List<Documento_ProducaoAniel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
            using (var documentoApi = new Documento_ProducaoApiController())
            using (var agendamentoEquipeApi = new Agendamento_EquipeApiController())
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
            using (var repoDocumento = new Conexao<Documento_ProducaoAniel>(session))
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
            using (var repositoryHorario = new Conexao<Horario_Connect>(session))
            using (var repoTipoImovel = new Conexao<Tipo_ImovelAniel>(session))
            using (var repoEquipe = new Conexao<EquipeAniel>(session))
            using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(session))
            using (var repositoryOLT = new Conexao<OLT>(sessionConnect))
            using (var repositoryPOP = new Conexao<POP>(sessionConnect))
            using (var repositoryElementoRede = new Conexao<Elemento_Rede>(session))
            {
                Configuracoes_Gerais config = await repositoryConfig.selectFirst();
                IAreaAnielRepository areaAnielRepository = new AreaAnielRepository(session);
                AreaAnielBusiness areaAnielBusiness = new AreaAnielBusiness(areaAnielRepository);
                ILocalidadeAnielRepository localidadeAnielRepository = new LocalidadeAnielRepository(session);
                LocalidadeAnielBusiness localidadeAnielBusiness = new LocalidadeAnielBusiness(localidadeAnielRepository);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(session);
                var naturezaBusiness = new NaturezaAnielBusiness(repoNatureza);

                foreach (DocumentoProducao_ImportacaoOS documento in documentos)
                {
                    Documento_ProducaoAniel obj = new Documento_ProducaoAniel();
                    try
                    {
                        EscritorioImp escritorio = new EscritorioImp();
                        AreaResponse area = new AreaResponse();
                        LocalidadeResponse localidade = new LocalidadeResponse();

                        if (tipoOS == "M")
                        {
                            IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                            var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                            documento.NUM_DOC = Convert.ToString(documentoBusiness.GetNumDocManutencao());
                        } 

                        if ((String.IsNullOrEmpty(documento.COD_LOCALIDADE) == false) && (String.IsNullOrEmpty(documento.COD_AREA) == false) && (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false))
                        {
                            if (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false)
                            {
                                escritorio.codCliente = 1;
                                escritorio.descricaoEscritorio = documento.COD_ESCRITORIO;
                                escritorio = await VerificaEscritorio(escritorio);
                            }
                            else
                            {
                                escritorio.codEscritorio = null;
                            }

                            localidade.descricaoLocalidade = documento.COD_LOCALIDADE;
                            localidade.codEscritorio = escritorio.codEscritorio;
                            localidade.codCliente = 1;
                            area.descricaoArea = documento.COD_AREA;
                            area.codCliente = 1;
                            area.codEscritorio = escritorio.codEscritorio;
                            if ((String.IsNullOrEmpty(documento.COD_AREA) == false) && (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false))
                            {
                                area = await VerificaArea(area);
                            }
                            else
                            {
                                area.codArea = null;
                            }

                            localidade.codArea = area.codArea;
                            localidade = await VerificaLocalidade(localidade);
                        }
                        else if ((String.IsNullOrEmpty(documento.COD_AREA) == false) && (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false))
                        {
                            area = await VerificaArea(area);
                        }
                        else if (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false)
                        {
                            escritorio.descricaoEscritorio = documento.COD_ESCRITORIO;
                            escritorio.codCliente = 1;
                            if (String.IsNullOrEmpty(documento.COD_ESCRITORIO) == false)
                            {
                                escritorio = await VerificaEscritorio(escritorio);
                            }
                            else
                            {
                                escritorio.codEscritorio = null;
                            }

                            escritorio = await VerificaEscritorio(escritorio);
                        }
                        else if (String.IsNullOrEmpty(documento.ENDERECO_BAIRRO) == false && String.IsNullOrEmpty(documento.ENDERECO_TIPO) == false)
                        {
                            localidade = localidadeAnielBusiness.GetByCidadeBairro(documento.ENDERECO_TIPO, documento.ENDERECO_BAIRRO);
                            if (String.IsNullOrEmpty(documento.ENDERECO_TIPO) == false)
                            {
                                area = areaAnielBusiness.GetByCidade(documento.ENDERECO_TIPO);
                            }
                        }
                        else if (String.IsNullOrEmpty(documento.ENDERECO_TIPO) == false)
                        {
                            area = areaAnielBusiness.GetByCidade(documento.ENDERECO_TIPO);
                        }
                        else
                        {
                            localidade.codLocalidade = null;
                            area.codArea = null;
                            escritorio.codEscritorio = null;
                        }

                        OLT olt = null;
                        POP pop = null;
                        Elemento_Rede elemento_rede = null;
                        if (String.IsNullOrEmpty(documento.ID_OLT)== false) {
                            olt = await repositoryOLT.selectFirst(p => p.Id == Convert.ToInt64(documento.ID_OLT)); 
                        }

                        if (String.IsNullOrEmpty(documento.ID_POP)== false) {
                            pop = await repositoryPOP.selectFirst(p => p.Id == Convert.ToInt64(documento.ID_POP)); 
                        }

                        if (String.IsNullOrEmpty(documento.ID_ELEMENTO_REDE)== false) {
                            elemento_rede = await repositoryElementoRede.selectFirst(p => p.Id == Convert.ToInt64(documento.ID_ELEMENTO_REDE)); 
                        }

                        Tipo_Servico tipoServ = new Tipo_Servico();
                        tipoServ.descricaoTipoServ = documento.COD_TIPO_SERV;
                        Tipo_Servico rtipoServicoNovo = await VerificaTipoServico(tipoServ, contrato);
                        Sub_Tipo_Servico subTipoServ = new Sub_Tipo_Servico();
                        subTipoServ.descricaoSubTipoServ = documento.COD_SUB_TIPO_SERV;

                        if (rtipoServicoNovo.codTipoServ != 0)
                            subTipoServ.codTipoServ = rtipoServicoNovo.codTipoServ;

                        Sub_Tipo_Servico rsubTipoServicoNovo = await VerificaSubTipoServico(subTipoServ);

                        if (!String.IsNullOrEmpty(documento.DATA_MAXIMA))
                        {
                            documento.DATA_MAXIMA = documento.DATA_MAXIMA.Trim();

                            DateTime parsedDate;
                            bool isValidDate = DateTime.TryParse(documento.DATA_MAXIMA, out parsedDate);

                            if (!isValidDate)
                            {
                               throw new Exception("o campo DATA AGENDAMENTO não está no formato de texto ou não possui uma data válida.");
                            }
                        }

                        if (!String.IsNullOrEmpty(documento.HORA_MAXIMA))
                        {
                            DateTime parsedDate;
                            bool isValidDate = DateTime.TryParse(documento.HORA_MAXIMA, out parsedDate);

                            if (!isValidDate)
                            {
                                throw new Exception("o campo HORA AGENDAMENTO não está no formato de texto ou não possui um horário válido.");
                            }
                        }

                        if (!String.IsNullOrEmpty(documento.DATA_CRIACAO))
                        {
                            DateTime parsedDate;
                            bool isValidDate = DateTime.TryParse(documento.DATA_CRIACAO, out parsedDate);

                            if (!isValidDate)
                            {
                                throw new Exception("o campo DATA CRIAÇÃO não está no formato de texto ou não possui uma data válida.");
                            }
                        }

                        if (!String.IsNullOrEmpty(documento.HORA_CRIACAO))
                        {
                            DateTime parsedDate;
                            bool isValidDate = DateTime.TryParse(documento.HORA_CRIACAO, out parsedDate);

                            if (!isValidDate)
                            {
                                throw new Exception("o campo HORA CRIAÇÃO não está no formato de texto ou não possui um horário válido.");
                            }
                        }


                        if (String.IsNullOrEmpty(documento.EMAIL_ASSINANTE) == false)
                        {
                            if (documento.EMAIL_ASSINANTE.Length > 50)
                                throw new Exception("o campo de email não pode ultrapassar 50 caracteres");
                        }

                        if (await VerificaOsExistente(documento, modelo.Erros, contrato, projeto))
                        {
                            throw new Exception("Ordem de serviço já importada.");
                        }

                        StatusExecucaoAniel? _statusExec = null;

                        if (String.IsNullOrEmpty(documento.DATA_MAXIMA) == true)
                        {
                            _statusExec = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                        }
                        else if ((String.IsNullOrEmpty(documento.DATA_MAXIMA) == false) && (String.IsNullOrEmpty(documento.EQUIPE) == true))
                        {
                            _statusExec = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                        }
                        else if ((String.IsNullOrEmpty(documento.DATA_MAXIMA) == false) && (String.IsNullOrEmpty(documento.EQUIPE) == false))
                        {
                            _statusExec = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
                        }

                        obj = new Documento_ProducaoAniel()
                        {

                            Data_Maxima = String.IsNullOrEmpty(documento.DATA_MAXIMA) ? (DateTime?)null : Convert.ToDateTime(documento.DATA_MAXIMA),
                            Hora_Maxima = String.IsNullOrEmpty(documento.HORA_MAXIMA) ? (DateTime?)null : Convert.ToDateTime(documento.HORA_MAXIMA),
                            Projeto = projeto,
                            Num_Doc = int.Parse(documento.NUM_DOC, NumberStyles.AllowThousands),
                            Num_Obra = documento.NUM_OBRA,
                            Numero_Obra_Original = documento.NUM_OBRA_ORIGINAL,
                            Seq = documento.SEQ,
                            Titular = documento.TITULAR,
                            Dia = String.IsNullOrEmpty(documento.DATA_CRIACAO) ? DateTime.Today : Convert.ToDateTime(documento.DATA_CRIACAO),
                            Hora = String.IsNullOrEmpty(documento.HORA_CRIACAO) ? DateTime.Today : Convert.ToDateTime(documento.HORA_CRIACAO),
                            Usuario = User.Identity.GetUserId(),
                            //Cliente = new ClienteAniel { Cod_Cliente = Convert.ToInt32(documento.TITULAR) },
                            Cod_Area = localidade?.codArea ?? area?.codArea, //documento.COD_AREA,
                            CodTurnoServ = !String.IsNullOrEmpty(documento.COD_TURNO_SERV) ? Convert.ToInt32(documento.COD_TURNO_SERV) : (int?)null,
                            CodCt = contrato,
                            Cod_Escritorio = localidade?.codEscritorio ?? (area?.codEscritorio ?? escritorio?.codEscritorio), //"1",//documento.COD_ESCRITORIO,
                            Cod_Localidade = Convert.ToInt32(localidade?.codLocalidade), //Convert.ToInt32(documento.COD_LOCALIDADE),
                            //Cod_Tipo_Serv = Convert.ToInt16(documento.COD_TIPO_SERV),
                            Cod_Tipo_Serv = Convert.ToInt16(rtipoServicoNovo.codTipoServ),
                            Cod_Sub_Tipo_Serv = rsubTipoServicoNovo?.codSubTipoServ,
                            TelefonePrincipal = documento.TAREFA,
                            CelularCliente = StringService.Truncate(documento.TEL_CIENTE, 15),
                            CPF_CLIENTE = documento.CPF_CLIENTE,
                            Tipo = tipoOS,
                            DiaHora_Agendamento = String.IsNullOrEmpty(documento.HORA_MAXIMA) ? (DateTime?)null : Convert.ToDateTime(documento.HORA_MAXIMA),
                            OXBDPEE = documento.OXBDPEE,
                            Observacao_Aux = StringService.Truncate(documento.OBSERVACAO_AUX,900),
                            Segmento = documento.SEGMENTO,

                            Email_Assinante = documento.EMAIL_ASSINANTE,
                            Tarefa = documento.TAREFA,
                            Logradouro = StringService.Truncate(documento.ENDERECO, 100),
                            Cidade = StringService.Truncate(documento.ENDERECO_TIPO, 100),
                            Bairro = StringService.Truncate(documento.ENDERECO_BAIRRO, 30),
                            Cep = documento.ENDERECO_CPL,
                            UF = documento.ENDERECO_UF,
                            Complemento = StringService.Truncate(documento.ENDERECO_COMPL, 100),
                            Numero = StringService.Truncate(documento.ENDERECO_NUMERO, 10),
                            Latitude_Str = documento.LATITUDE_STR,
                            Longitude_Str = documento.LONGITUDE_STR,
                            Unidade_Custo = unidadeCusto,
                            Equipe = documento.EQUIPE,
                            Cod_Tipo_Imovel = documento.PARPRIM_PARA,
                            SenhaRadius = documento.SENHA_BAIXA,
                            LoginRadius = documento.PPPOE,
                            Status = _statusExec,
                            Id_Olt = olt?.Id,
                            Id_Pop = pop?.Id,
                            Id_Elemento_Rede = elemento_rede?.Id
                        };

                        await repoDocumento.SaveOrUpdate(obj);


                        Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == obj.CodCt && p.Projeto == obj.Projeto && p.Num_Doc == obj.Num_Doc && p.Num_Obra == obj.Num_Obra);

                        if (obj.Data_Maxima.HasValue)
                        {
                            var tempoExecucao = await documentoApi.GetTempoExecucao(config, obj.CodCt, obj.Projeto, obj.Num_Obra, obj.Num_Doc);
                            DateTime dataHoraInicio = obj.Data_Maxima.Value;
                            if (obj.Hora_Maxima.HasValue)
                            {
                                dataHoraInicio = (obj?.Data_Maxima.Value.Add(obj.Hora_Maxima.Value.TimeOfDay)).Value;
                            }
                            else if (obj.CodTurnoServ.HasValue)
                            {
                                Turno_ServicoAniel turno = await repositoryTurno.selectFirst(p => p.Id == obj.CodTurnoServ);
                                int codNatureza = naturezaBusiness.GetNaturezaByCodTipoServ((int)rtipoServicoNovo.codTipoServ, contrato);
                                var resultadoAgendamento = await agendamentoEquipeApi.GetDatasDisponiveis(obj.Cod_Tipo_Serv, codNatureza, obj.Cod_Escritorio, obj.Cod_Area, obj.Cod_Localidade, documento.COD_LOCALIDADE, tempoExecucao.Value.TotalMinutes, obj.Data_Maxima);
                                if (resultadoAgendamento.Ok)
                                {
                                    var turnosPossiveis = resultadoAgendamento.Objeto.FindAll(p => p.Data == obj.Data_Maxima.Value.ToString("dd/MM/yyyy")).FirstOrDefault()?.HorasTurno;
                                    var agendamentoTurno = turnosPossiveis != null ? turnosPossiveis.FirstOrDefault(p => p.Display == turno.Descricao) : null;
                                    if (agendamentoTurno != null)
                                    {
                                        obj.Hora_Maxima = agendamentoTurno.Hora_Inicio;
                                    }
                                    else
                                    {
                                        obj.Hora_Maxima = turno.Hora_Ini;
                                    }
                                }
                                else
                                {
                                    obj.Hora_Maxima = turno.Hora_Ini;
                                }

                                dataHoraInicio = (obj?.Data_Maxima.Value.Add(obj.Hora_Maxima.Value.TimeOfDay)).Value;
                            }
                            else
                            {
                                dataHoraInicio = dataHoraInicio.Add(constanteHorarioInicioAgendamento);
                            }

                            if (agendamento == null)
                            {
                                agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = obj.CodCt,
                                    Projeto = obj.Projeto,
                                    Num_Obra = obj.Num_Obra,
                                    Num_Doc = obj.Num_Doc,
                                    Cod_Escritorio = obj.Cod_Escritorio,
                                    Cod_Area = obj.Cod_Area,
                                    Cod_Localidade = obj.Cod_Localidade,
                                    DataHora_Inicio = dataHoraInicio,
                                    DataHora_Fim = dataHoraInicio.Add(tempoExecucao.Value),
                                    Flexivel = null,
                                    Deslocamento = 0,
                                    //Log = GerarLog(value.Objeto.Lista_Forca_Trabalho, value.Objeto.Lista_Ja_Agendados, config),
                                    Tipo_Servico = obj.Cod_Tipo_Serv.HasValue ? await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == obj.Cod_Tipo_Serv) : null,
                                    Tipo_Imovel = obj.Cod_Tipo_Imovel.HasValue ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == obj.Cod_Tipo_Imovel) : null,
                                    UsuarioCriacao = User.Identity.GetUserId(),
                                    Origem = OrigemAgendamentoOS.Integracao,
                                    Latitude = obj.GetLatitude_Str(),
                                    Longitude = obj.GetLongitude_Str()
                                };

                                if (obj.Equipe != null)
                                {
                                    agendamento.Equipe = await repoEquipe.selectFirst(p => p.Matricula == obj.Equipe);
                                }


                                await repositoryAgendamento.SaveOrUpdate(agendamento);
                            }
                            else
                            {
                                dataHoraInicio = (obj?.Data_Maxima.Value.Add(obj.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Inicio = (obj?.Data_Maxima.Value.Add(obj.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Fim = dataHoraInicio.Add(tempoExecucao.Value);
                                if (obj.Equipe != null)
                                {
                                    agendamento.Equipe = await repoEquipe.selectFirst(p => p.Matricula == obj.Equipe);
                                }

                                await repositoryAgendamento.SaveOrUpdate(agendamento);

                            }
                        }
                        await GravaLogImportacao(
                                new LogImportacao { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), OS = documento.NUM_OBRA_ORIGINAL, Erro = "OS cadastrada com sucesso!", Usuario = User.Identity.GetUserId() });

                        modelo.Sucessos.Add($"OS {obj.Numero_Obra_Original} cadastrada com sucesso");
                    }
                    catch (Exception e)
                    {

                        if (e.InnerException?.Message != null)
                        {
                            await GravaLogImportacao(
                                new LogImportacao { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), OS = documento.NUM_OBRA_ORIGINAL, Erro = e.Message + e.InnerException?.Message, Usuario = User.Identity.GetUserId() });

                            modelo.Erros.Add($"falha OS: {documento.NUM_OBRA_ORIGINAL} - erro: {e.Message} {e.InnerException?.Message}");
                        }
                        else
                        {
                            await GravaLogImportacao(
                                new LogImportacao { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), OS = documento.NUM_OBRA_ORIGINAL, Erro = e.Message, Usuario = User.Identity.GetUserId() });

                            modelo.Erros.Add($"falha OS: {documento.NUM_OBRA_ORIGINAL} - erro: {e.Message}");
                        }

                        //return new GetApiRetorno { Mensagem = e.Message, Ok = false };
                    }
                    //return new GetApiRetorno { Mensagem = e.Message, Ok = false };


                }

                return new GetApiRetorno { Ok = true };
            }

        }
        public async Task InsereTipoServico(Tipo_Servico tipoServico, String pCodCt)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_TIPO_SERVICO_EQUIPE (COD_TIPO_SERV,DESCRICAO,CODCT)");
                sql.AppendLine($" VALUES (@CODTIPOSERV,@DESCTIPOSERV,@CODCT)");
                parametros.Add("@CODTIPOSERV", tipoServico.codTipoServ);
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);
                parametros.Add("@CODCT", pCodCt);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }
        public async Task InsereSubTipoServico(Sub_Tipo_Servico subTipoServico)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_SUB_TIPO_SERVICO (COD_SUB_TIPO_SERV,COD_TIPO_SERV,DESCRICAO)");
                sql.AppendLine($" VALUES (@COD_SUB_TIPO_SERV,@CODTIPOSERV,@DESCTIPOSERV)");
                parametros.Add("@CODTIPOSERV", subTipoServico.codSubTipoServ);
                parametros.Add("@DESCTIPOSERV", subTipoServico.descricaoSubTipoServ);
                parametros.Add("@COD_SUB_TIPO_SERV", subTipoServico.codSubTipoServ);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }
        public async Task InsereTipoServicoExtra(Tipo_Servico tipoServico, String pCodCt)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_TIPO_SERVICO_EQUIPE_EXTRA (COD_TIPO_SERV,DESCRICAO_AUX,CODCT)");
                sql.AppendLine($" VALUES (@CODTIPOSERV,@DESCTIPOSERV,@CODCT)");
                parametros.Add("@CODTIPOSERV", tipoServico.codTipoServ);
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);
                parametros.Add("@CODCT", pCodCt);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }
        public async Task OperacaoTipoServico(TipoServicoImpVinculo tipoServicoImpVinculo, Tipo_Servico tipoServico, String pCodCt)
        {
            if (tipoServicoImpVinculo.Acao == "vincular")
            {
                tipoServico.codTipoServ = tipoServicoImpVinculo.TipoServicoVincular;
                await InsereTipoServicoExtra(tipoServico, pCodCt);
            }
            else if (tipoServicoImpVinculo.Acao == "cadastrar")
            {
                tipoServico = await GeraCodTipoServico(tipoServico);
                await InsereTipoServico(tipoServico, pCodCt);
            }

        }

        public async Task<Tipo_Servico> GetIdTipoServicoGravado(Tipo_Servico tipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COD_TIPO_SERV,0)AS COD_TIPO_SERV FROM TB_TIPO_SERVICO_EQUIPE ");
                sql.AppendLine($" WHERE DESCRICAO = @DESCTIPOSERV ");
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tipoServico.codTipoServ = Convert.ToInt64(reader["COD_TIPO_SERV"]);
                    }

                    return tipoServico;
                }
            }
        }

        public async Task<Sub_Tipo_Servico> GetIdSubTipoServicoGravado(Sub_Tipo_Servico subTipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COD_SUB_TIPO_SERV,0)AS COD_SUB_TIPO_SERV FROM TB_SUB_TIPO_SERVICO ");
                sql.AppendLine($" WHERE DESCRICAO = @DESCSUBTIPOSERV ");
                parametros.Add("@DESCSUBTIPOSERV", subTipoServico.descricaoSubTipoServ);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        subTipoServico.codSubTipoServ = Convert.ToString(reader["COD_SUB_TIPO_SERV"]);
                    }

                    return subTipoServico;
                }
            }
        }

        public async Task<Tipo_Servico> GetIdTipoServicoGravadoExtra(Tipo_Servico tipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COD_TIPO_SERV,0)AS COD_TIPO_SERV FROM TB_TIPO_SERVICO_EQUIPE_EXTRA ");
                sql.AppendLine($" WHERE DESCRICAO_AUX = @DESCTIPOSERV ");
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tipoServico.codTipoServ = Convert.ToInt64(reader["COD_TIPO_SERV"]);
                    }

                    return tipoServico;
                }
            }
        }

        public async Task<bool> ExisteSubTipoServPorNome(Sub_Tipo_Servico subTipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COUNT(COD_SUB_TIPO_SERV),0)AS CO FROM TB_SUB_TIPO_SERVICO ");
                sql.AppendLine($" WHERE DESCRICAO = @DESCTIPOSERV ");
                sql.AppendLine($" AND COD_TIPO_SERV = @COD_TIPO_SERV ");
                parametros.Add("@DESCTIPOSERV", subTipoServico.descricaoSubTipoServ);
                parametros.Add("@COD_TIPO_SERV", subTipoServico.codTipoServ);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return (Convert.ToInt64(reader["CO"]) > 0);
                    }
                    else
                        return false;
                }
            }
        }

        public async Task<bool> ExisteTipoServPorNome(Tipo_Servico tipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COUNT(COD_TIPO_SERV),0)AS CO FROM TB_TIPO_SERVICO_EQUIPE ");
                sql.AppendLine($" WHERE DESCRICAO = @DESCTIPOSERV ");
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    await reader.ReadAsync();
                    //tipoServico.codTipoServ = Convert.ToInt64(reader["COD_TIPO_SERV"]);
                    return (Convert.ToInt64(reader["CO"]) > 0);
                }
            }
        }

        public async Task<bool> ExisteTipoServPorNomeExtra(Tipo_Servico tipoServico)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(COUNT(COD_TIPO_SERV),0)AS CO FROM TB_TIPO_SERVICO_EQUIPE_EXTRA ");
                sql.AppendLine($" WHERE DESCRICAO_AUX = @DESCTIPOSERV ");
                parametros.Add("@DESCTIPOSERV", tipoServico.descricaoTipoServ);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    await reader.ReadAsync();
                    return (Convert.ToInt64(reader["CO"]) > 0);
                }
            }
        }
        public async Task<Tipo_Servico> VerificaTipoServico(Tipo_Servico tipoServico, String pCodCt)
        {
            bool _temTipoServ;
            bool _temTipoServExtra;

            Tipo_Servico tipoServ = new Tipo_Servico();

            _temTipoServ = await ExisteTipoServPorNome(tipoServico);
            
            if (_temTipoServ != true)
            {
                _temTipoServExtra = await ExisteTipoServPorNomeExtra(tipoServico);
            }
            else
            {
                _temTipoServExtra = false;
            }


            if (_temTipoServ)
            {
                tipoServ = await GetIdTipoServicoGravado(tipoServico);
            }
            else if (_temTipoServExtra)
            {
                tipoServ = await GetIdTipoServicoGravadoExtra(tipoServico);
            }
            else
            {
                tipoServ.codTipoServ = 0;
            }

            if (tipoServ.codTipoServ != 0)
            {
                return tipoServ;
            }
            else
            {
                //tipoServ = await GeraCodTipoServico(tipoServico);
                //await InsereTipoServico(tipoServico, pCodCt);
                //return tipoServ;
            }

            return tipoServ;
        }

        public async Task<Sub_Tipo_Servico> VerificaSubTipoServico(Sub_Tipo_Servico subTipoServico)
        {
            bool _temSubTipoServ;
            Sub_Tipo_Servico subTipoServ = new Sub_Tipo_Servico();
            subTipoServ.codTipoServ = subTipoServico.codTipoServ;

            _temSubTipoServ = await ExisteSubTipoServPorNome(subTipoServico);
            if (_temSubTipoServ)
            {
                subTipoServ = await GetIdSubTipoServicoGravado(subTipoServ);
            }

            if (string.IsNullOrEmpty(subTipoServ.codSubTipoServ) == false)
            {
                return subTipoServ;
            }
            else
            {
                subTipoServ = await GeraCodSubTipoServico(subTipoServ);
                await InsereSubTipoServico(subTipoServ);
                return subTipoServ;
            }
        }

        public async Task<Tipo_Servico> GeraCodTipoServico(Tipo_Servico tipoServico)
        {
            _codTipoServ = 0;
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT (COALESCE(MAX(COD_TIPO_SERV),0)+1)AS COD_TIPO_SERV FROM TB_TIPO_SERVICO_EQUIPE ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tipoServico.codTipoServ = Convert.ToInt64(reader["COD_TIPO_SERV"]);
                    }

                }
            }
            return tipoServico;
        }

        public async Task<Sub_Tipo_Servico> GeraCodSubTipoServico(Sub_Tipo_Servico subTipoServico)
        {
            _codSubTipoServ = 0;
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT (COALESCE(MAX(CAST(COD_SUB_TIPO_SERV AS INTEGER)),0)+1)AS COD_SUB_TIPO_SERV FROM TB_SUB_TIPO_SERVICO ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        subTipoServico.codSubTipoServ = Convert.ToString(reader["COD_SUB_TIPO_SERV"]);
                    }

                }
            }

            return subTipoServico;
        }


        public async Task<AreaResponse> GeraCodArea(AreaResponse area)
        {
            AreaResponse _area = new AreaResponse();
            _area = area;
            Int64 tempCodArea;
            tempCodArea = 0;
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT MAX(CAST(COD_AREA AS INTEGER))AS COD_AREA FROM TB_AREA ");
                sql.AppendLine(" WHERE( ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%a%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%b%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%c%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%d%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%e%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%f%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%g%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%h%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%i%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%j%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%k%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%l%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%m%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%n%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%o%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%p%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%q%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%r%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%s%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%t%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%u%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%v%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%w%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%x%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%y%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%z%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ç%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%á%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ã%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%â%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%à%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%é%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%è%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ê%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%í%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ì%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%î%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ó%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ò%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ô%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%õ%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ú%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%ù%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%û%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%/%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%-%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%+%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%.%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%,%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%;%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%:%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%>%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%<%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%!%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%@%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%#%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%$%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%&%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%*%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%(%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%)%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%{%') and ");
                sql.AppendLine(" NOT UPPER(COD_AREA) LIKE UPPER('%}%')) ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tempCodArea = Convert.ToInt64(reader["COD_AREA"]) + 1;
                        _area.codArea = Convert.ToString(tempCodArea);
                    }
                }

                return _area;

            }
        }
        public async Task<EscritorioImp> GeraCodExcritorio(EscritorioImp escritorio)
        {
            EscritorioImp _escritorio = new EscritorioImp();
            _escritorio = escritorio;
            Int64 tempCodEscritorio;
            tempCodEscritorio = 0;
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT MAX(CAST(COD_ESCRITORIO AS INTEGER))AS COD_ESCRITORIO FROM TB_ESCRITORIO ");
                sql.AppendLine(" WHERE( ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%a%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%b%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%c%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%d%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%e%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%f%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%g%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%h%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%i%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%j%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%k%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%l%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%m%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%n%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%o%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%p%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%q%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%r%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%s%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%t%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%u%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%v%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%w%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%x%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%y%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%z%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ç%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%á%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ã%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%â%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%à%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%é%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%è%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ê%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%í%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ì%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%î%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ó%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ò%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ô%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%õ%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ú%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%ù%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%û%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%/%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%-%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%+%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%.%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%,%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%;%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%:%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%>%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%<%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%!%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%@%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%#%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%$%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%&%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%*%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%(%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%)%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%{%') and ");
                sql.AppendLine(" NOT UPPER(COD_ESCRITORIO) LIKE UPPER('%}%')) ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tempCodEscritorio = Convert.ToInt64(reader["COD_ESCRITORIO"]) + 1;
                        _escritorio.codEscritorio = Convert.ToString(tempCodEscritorio);
                    }
                }

                return _escritorio;
            }
        }
        public async Task<EscritorioImp> ExisteEscritorioPorNome(EscritorioImp escritorio)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {

                EscritorioImp _escritorio = new EscritorioImp();

                _escritorio = escritorio;
                _escritorio.codEscritorio = "-";

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COD_ESCRITORIO FROM TB_ESCRITORIO ");
                sql.AppendLine(" WHERE DESCRICAO = @pDESCRICAO ");
                parametros.Add("@pDESCRICAO", escritorio.descricaoEscritorio);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        _escritorio = escritorio;
                        _escritorio.codEscritorio = Convert.ToString(reader["COD_ESCRITORIO"]);
                        return _escritorio;
                    }

                }
            }

            return escritorio;
        }

        public async Task InsereEscritorio(EscritorioImp escritorio)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_ESCRITORIO (COD_CLIENTE,COD_ESCRITORIO,DESCRICAO)");
                sql.AppendLine($" VALUES (@pCOD_CLIENTE,@pCOD_ESCRITORIO,@pDESCRICAO)");
                parametros.Add("@pCOD_CLIENTE", escritorio.codCliente);
                parametros.Add("@pCOD_ESCRITORIO", escritorio.codEscritorio);
                parametros.Add("@pDESCRICAO", escritorio.descricaoEscritorio);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<EscritorioImp> VerificaEscritorio(EscritorioImp escritorio)
        {
            EscritorioImp _escritorio = new EscritorioImp();

            _escritorio = await ExisteEscritorioPorNome(escritorio);

            if (_escritorio.codEscritorio == "-")
            {
                _escritorio = await GeraCodExcritorio(escritorio);
                await InsereEscritorio(escritorio);
            }

            return _escritorio;
        }


        public async Task<AreaResponse> ExisteAreaPorNome(AreaResponse area)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                AreaResponse _area = new AreaResponse();

                _area = area;
                _area.codArea = "-";

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COD_AREA FROM TB_AREA ");
                sql.AppendLine(" WHERE DESCRICAO = @pDESCRICAO ");
                sql.AppendLine(" AND COD_ESCRITORIO = @pCOD_ESCRITORIO ");
                sql.AppendLine(" AND COD_CLIENTE = @pCOD_CLIENTE ");
                parametros.Add("@pDESCRICAO", area.descricaoArea);
                parametros.Add("@pCOD_ESCRITORIO", area.codEscritorio);
                parametros.Add("@pCOD_CLIENTE", area.codCliente);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {

                        _area = area;

                        _area.codArea = Convert.ToString(reader["COD_AREA"]);

                        return _area;
                    }

                }
            }
            return area;
        }


        public async Task<AreaResponse> VerificaArea(AreaResponse area)
        {
            AreaResponse _area = new AreaResponse();

            _area = await ExisteAreaPorNome(area);

            if (_area.codArea == "-")
            {
                _area = await GeraCodArea(area);
                await InsereArea(area);
            }

            return _area;
        }

        public async Task InsereArea(AreaResponse area)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_AREA (COD_CLIENTE,COD_AREA,COD_ESCRITORIO,DESCRICAO)");
                sql.AppendLine($" VALUES (@pCOD_CLIENTE,@pCOD_AREA,@pCOD_ESCRITORIO,@pDESCRICAO)");
                parametros.Add("@pCOD_CLIENTE", area.codCliente);
                parametros.Add("@pCOD_ESCRITORIO", area.codEscritorio);
                parametros.Add("@pCOD_AREA", area.codArea);
                parametros.Add("@pDESCRICAO", area.descricaoArea);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<LocalidadeResponse> GeraCodLocalidade(LocalidadeResponse localidade)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();
            LocalidadeResponse _localidade = new LocalidadeResponse();
            _localidade = localidade;
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT (COALESCE(MAX(COD_LOCALIDADE),0)+1)AS COD_LOCALIDADE FROM TB_LOCALIDADE ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    await reader.ReadAsync();
                    _localidade.codLocalidade = Convert.ToInt64(reader["COD_LOCALIDADE"]);
                    return _localidade;
                }
            }
        }
        public async Task<LocalidadeResponse> ExisteLocalidadePorNome(LocalidadeResponse localidade)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                LocalidadeResponse _localidade = new LocalidadeResponse();

                _localidade = localidade;
                _localidade.codLocalidade = 0;

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COD_LOCALIDADE FROM TB_LOCALIDADE ");
                sql.AppendLine(" WHERE LOCALIDADE = @pDESCLOCALIDADE ");
                sql.AppendLine(" AND COD_ESCRITORIO = @pCOD_ESCRITORIO ");
                sql.AppendLine(" AND COD_CLIENTE = @pCOD_CLIENTE ");
                sql.AppendLine(" AND COD_AREA = @pCOD_AREA ");
                parametros.Add("@pDESCLOCALIDADE", localidade.descricaoLocalidade);
                parametros.Add("@pCOD_ESCRITORIO", localidade.codEscritorio);
                parametros.Add("@pCOD_AREA", localidade.codArea);
                parametros.Add("@pCOD_CLIENTE", localidade.codCliente);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {

                        _localidade = localidade;

                        _localidade.codLocalidade = Convert.ToInt64(reader["COD_LOCALIDADE"]);

                        return _localidade;
                    }

                }
            }
            return localidade;
        }

        public async Task InsereLocalidade(LocalidadeResponse localidade)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO TB_LOCALIDADE (COD_CLIENTE,COD_AREA,COD_ESCRITORIO,COD_LOCALIDADE,LOCALIDADE)");
                sql.AppendLine($" VALUES (@pCOD_CLIENTE,@pCOD_AREA,@pCOD_ESCRITORIO,@pCOD_LOCALIDADE,@pLOCALIDADE)");
                parametros.Add("@pCOD_CLIENTE", localidade.codCliente);
                parametros.Add("@pCOD_ESCRITORIO", localidade.codEscritorio);
                parametros.Add("@pCOD_AREA", localidade.codArea);
                parametros.Add("@pCOD_LOCALIDADE", localidade.codLocalidade);
                parametros.Add("@pLOCALIDADE", localidade.descricaoLocalidade);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<LocalidadeResponse> VerificaLocalidade(LocalidadeResponse localidade)
        {
            LocalidadeResponse _localidade = new LocalidadeResponse();

            _localidade = await ExisteLocalidadePorNome(localidade);

            if (_localidade.codLocalidade == 0)
            {
                _localidade = await GeraCodLocalidade(localidade);
                await InsereLocalidade(localidade);
            }

            return _localidade;
        }

        public async Task<bool> VerificaOsExistente(DocumentoProducao_ImportacaoOS documento, List<String> erros, string contrato, string projeto)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoDocumento = new Conexao<Documento_ProducaoAniel>(session))
                {
                    var query = await repoDocumento.select(x => x.Numero_Obra_Original == documento.NUM_OBRA && x.CodCt == contrato && x.Projeto == projeto && x.Num_Doc == int.Parse(documento.NUM_DOC, NumberStyles.AllowThousands));

                    documento.NUM_OBRA_ORIGINAL = documento.NUM_OBRA;
                    documento.NUM_OBRA = documento.NUM_OBRA + "/" + 1;
                    documento.SEQ = 1;

                    if (query.Any())
                    {
                        //var seq = query.Max(p => p.Seq) + 1;
                        //documento.NUM_OBRA_ORIGINAL = documento.NUM_OBRA;
                        //documento.NUM_OBRA = documento.NUM_OBRA + "/" + seq;
                        //documento.SEQ = seq;
                        return true;
                    }
                    else
                    {
                        //documento.NUM_OBRA_ORIGINAL = documento.NUM_OBRA;
                        //documento.NUM_OBRA = documento.NUM_OBRA + "/" + 1;
                        //documento.SEQ = 1;
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ImportacaoOsCampo>> CarregaConfiguracaoLeitura(ISession session, string projeto, string contrato)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                parametros.Add("CONTRATO", contrato);
                parametros.Add("PROJETO", projeto);

                sql.AppendLine(" SELECT A.PLANILHA AS IMPORTAR,B.ID,B.NOME_CAMPO_ANIEL CAMPO,A.CAMPO_PLANILHA PLANILHA,B.APELIDO_CAMPO_ANIEL APELIDO, B.INTERNO FROM TB_CONFIG_IMPORT_NET_DADOS A ");
                sql.AppendLine(" INNER JOIN tb_config_importacao_net B ON(B.ID = A.ID_CONFIG) ");
                sql.AppendLine(" WHERE A.CODCT = @CONTRATO AND A.PROJETO = @PROJETO ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        char interno = reader["INTERNO"] != null && reader["INTERNO"].ToString() != "" ? Convert.ToChar(reader["INTERNO"].ToString().ToUpper()) : 'N';

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = reader["PLANILHA"].ToString(), Chave = interno == 'S' ? true : false, Importar = reader["IMPORTAR"].ToString() == "3" ? false : true});
                    }
                }
                retorno = retorno.Where(p => p.Importar == true).ToList();
            }

            return retorno;
        } 
        public async Task<List<ImportacaoOsCampo>> CarregaConfiguracaoLeituraManut(ISession session, string projeto, string contrato)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                parametros.Add("CONTRATO", contrato);
                parametros.Add("PROJETO", projeto);

                sql.AppendLine(" SELECT A.PLANILHA AS IMPORTAR,B.ID,B.NOME_CAMPO_ANIEL CAMPO,A.CAMPO_PLANILHA PLANILHA,B.APELIDO_CAMPO_ANIEL APELIDO, B.INTERNO FROM TB_CONFIG_IMPORT_MANUT_DADOS A ");
                sql.AppendLine(" INNER JOIN tb_config_importacao_manut B ON(B.ID = A.ID_CONFIG) ");
                sql.AppendLine(" WHERE A.CODCT = @CONTRATO AND A.PROJETO = @PROJETO ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        char interno = reader["INTERNO"] != null && reader["INTERNO"].ToString() != "" ? Convert.ToChar(reader["INTERNO"].ToString().ToUpper()) : 'N';

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = reader["PLANILHA"].ToString(), Chave = interno == 'S' ? true : false, Importar = reader["IMPORTAR"].ToString() == "3" ? false : true});
                    }
                }
                retorno = retorno.Where(p => p.Importar == true).ToList();
            }

            return retorno;
        }

        public async Task<List<ImportacaoOsCampo>> CarregaConfiguracao(ISession session, string projeto, string contrato)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                parametros.Add("CONTRATO", contrato);
                parametros.Add("PROJETO", projeto);

                sql.AppendLine(" SELECT A.PLANILHA AS IMPORTAR,B.ID,B.NOME_CAMPO_ANIEL CAMPO,A.CAMPO_PLANILHA PLANILHA,B.APELIDO_CAMPO_ANIEL APELIDO, B.INTERNO FROM TB_CONFIG_IMPORT_NET_DADOS A ");
                sql.AppendLine(" INNER JOIN TB_CONFIG_IMPORTACAO_NET B ON(B.ID = A.ID_CONFIG) ");
                sql.AppendLine(" WHERE A.CODCT = @CONTRATO AND A.PROJETO = @PROJETO ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        char interno = reader["INTERNO"] != null && reader["INTERNO"].ToString() != "" ? Convert.ToChar(reader["INTERNO"].ToString().ToUpper()) : 'N';

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = reader["PLANILHA"].ToString(), Chave = interno == 'S' ? true : false, Importar = reader["IMPORTAR"].ToString() == "3" ? false : true});
                    }
                }

                StringBuilder sqlNaoImportadas = new StringBuilder();
                sqlNaoImportadas.AppendLine(" SELECT id,nome_campo_aniel CAMPO,APELIDO_CAMPO_ANIEL APELIDO FROM TB_CONFIG_IMPORTACAO_NET ");
                sqlNaoImportadas.AppendLine(" WHERE NOME_CAMPO_ANIEL <> '' ");

                foreach (ImportacaoOsCampo campo in retorno)
                {
                    sqlNaoImportadas.Append($" AND NOME_CAMPO_ANIEL <> '{campo.CampoAniel.ToUpper()}' ");
                }

                sqlNaoImportadas.AppendLine(" ORDER BY APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sqlNaoImportadas, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = "", Chave = false, Importar = false });
                    }
                }

            }

            return retorno;
        }

        public async Task<List<ImportacaoOsCampo>> CarregaConfiguracaoManutencao(ISession session, string projeto, string contrato)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();
                parametros.Add("CONTRATO", contrato);
                parametros.Add("PROJETO", projeto);

                sql.AppendLine(" SELECT A.PLANILHA AS IMPORTAR,B.ID,B.NOME_CAMPO_ANIEL CAMPO,A.CAMPO_PLANILHA PLANILHA,B.APELIDO_CAMPO_ANIEL APELIDO, B.INTERNO FROM TB_CONFIG_IMPORT_MANUT_DADOS A ");
                sql.AppendLine(" INNER JOIN TB_CONFIG_IMPORTACAO_MANUT B ON(B.ID = A.ID_CONFIG) ");
                sql.AppendLine(" WHERE A.CODCT = @CONTRATO AND A.PROJETO = @PROJETO ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        char interno = reader["INTERNO"] != null && reader["INTERNO"].ToString() != "" ? Convert.ToChar(reader["INTERNO"].ToString().ToUpper()) : 'N';

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = reader["PLANILHA"].ToString(), Chave = interno == 'S' ? true : false, Importar = reader["IMPORTAR"].ToString() == "3" ? false : true });
                    }
                }

                StringBuilder sqlNaoImportadas = new StringBuilder();
                sqlNaoImportadas.AppendLine(" SELECT id,nome_campo_aniel CAMPO,APELIDO_CAMPO_ANIEL APELIDO FROM TB_CONFIG_IMPORTACAO_MANUT ");
                sqlNaoImportadas.AppendLine(" WHERE NOME_CAMPO_ANIEL <> '' ");

                foreach (ImportacaoOsCampo campo in retorno)
                {
                    sqlNaoImportadas.Append($" AND NOME_CAMPO_ANIEL <> '{campo.CampoAniel.ToUpper()}' ");
                }

                sqlNaoImportadas.AppendLine(" ORDER BY APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sqlNaoImportadas, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), Apelido = reader["APELIDO"].ToString(), CampoAniel = reader["CAMPO"].ToString(), CampoPlanilha = "", Chave = false, Importar = false });
                    }
                }

            }

            return retorno;
        }

        public async Task<List<ImportacaoOsCampo>> CarregaNovaConfiguracao(ISession session)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT ID,NOME_CAMPO_ANIEL CAMPO, APELIDO_CAMPO_ANIEL APELIDO, INTERNO ");
                sql.AppendLine(" FROM TB_CONFIG_IMPORTACAO_NET ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        bool chave = reader["INTERNO"].ToString() == "S" ? true : false;

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), CampoAniel = reader["CAMPO"].ToString(), Apelido = reader["APELIDO"].ToString(), Chave = chave, Importar = true });
                    }
                }
            }

            return retorno;

        }
        public async Task<List<ImportacaoOsCampo>> CarregaNovaConfiguracaoManutencao(ISession session)
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var com = session.Connection.CreateCommand())
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT ID,NOME_CAMPO_ANIEL CAMPO, APELIDO_CAMPO_ANIEL APELIDO, INTERNO ");
                sql.AppendLine(" FROM TB_CONFIG_IMPORTACAO_MANUT ");
                sql.AppendLine(" ORDER BY INTERNO DESC, APELIDO ASC ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                {
                    while (await reader.ReadAsync())
                    {
                        bool chave = reader["INTERNO"].ToString() == "S" ? true : false;

                        retorno.Add(new ImportacaoOsCampo { Id = Convert.ToInt64(reader["ID"].ToString()), CampoAniel = reader["CAMPO"].ToString(), Apelido = reader["APELIDO"].ToString(), Chave = chave, Importar = true });
                    }
                }
            }

            return retorno;

        }

        public char VerificaInterno(string campoAniel)
        {
            if (campoAniel.Contains("NUM_OBRA") || campoAniel.Contains("NUM_DOC") || campoAniel.Contains("PROJETO") || campoAniel.Contains("CODCT"))
                return 'S';
            else
                return 'N';
        }

        public async Task GravaLogImportacao(LogImportacao log)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_LOG_IMPORTACAO (DATA,HORA,ERRO,OPERACAO,USUARIO)");
                sql.AppendLine($" VALUES (@DATA,@HORA,@ERRO,@OPERACAO,@USUARIO)");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@DATA", log.Data.Replace('/', '-'));
                parametros.Add("@HORA", log.Hora);
                parametros.Add("@ERRO", StringService.Truncate(log.Erro, 255));
                parametros.Add("@OPERACAO", log.OS);
                parametros.Add("@USUARIO", log.Usuario);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

    }

    public class ImportacaoOSPlanilha
    {
        public string[] Campos { get; set; }
    }

    public class LogRequest
    {
        public string Data { get; set; }
    }

    public class ImportacaoCampoRequest
    {
        public string Projeto { get; set; }
        public string Contrato { get; set; }
        public string TipoOS { get; set; }
    }

    public class ConfiguracaoSaveRequest
    {
        public string Projeto { get; set; }
        public string Contrato { get; set; }
        public string[][] Campos { get; set; }
        public string TipoOS { get; set; }
    }

    [ExcelServiceKey(nameof(Codigo))]
    public class ImportacaoTeste : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Código do Grupo")]
        public virtual String CodigoGrupo { get; set; }

        [MaxLength(50)]
        [ExcelService("Nome do Grupo")]
        public virtual String NomeGrupo { get; set; }

        [MaxLength(50)]
        [ExcelService("Código Subgrupo")]
        public virtual String CodigoSubGrupo { get; set; }

        [MaxLength(50)]
        [ExcelService("Nome do Subgrupo")]
        public virtual String NomeSubGrupo { get; set; }

        [MaxLength(50)]
        [ExcelService("Código da Conta Financeira")]
        public virtual String Codigo { get; set; }

        [MaxLength(50)]
        [ExcelService("Nome da Conta Financeira")]
        public virtual String Nome { get; set; }

        [MaxLength(50)]
        [ExcelService("Visivel no demonstrativo")]
        public virtual String Visivel { get; set; }

        [MaxLength(50)]
        [ExcelService("Classificação das Contas")]
        public virtual String Classificacao { get; set; }

        [MaxLength(50)]
        [ExcelService("Tipo de Movimento")]
        public virtual String TipoMovimento { get; set; }

        [MaxLength(50)]
        [ExcelService("Código Conta Contábil")]
        public virtual String CodigoContaContabil { get; set; }
        [MaxLength(50)]
        [ExcelService("Código Centro de Custo")]
        public virtual String CodigoCentroCusto { get; set; }

        public bool Importar { get; set; }
    }

}
