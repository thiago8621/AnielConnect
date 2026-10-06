using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.SMS;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.HubDesenvolvedor;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using EnumEtapaVenda = Sinapse.Model.enumerados.EnumEtapaVenda;
using Image = System.Drawing.Image;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Cliente_ProspectoController : BaseController
    {
        public Cliente_ProspectoController() : base("Cliente_Prospecto",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            "api/Cliente_Prospectoapi/FindById?id={0}",
                                            "api/Cliente_Prospectoapi/Gravar",
                                            "api/Cliente_Prospectoapi/Excluir",
                                            String.Empty)
        {

        }

        // GET: Cliente_Prospecto
        public ActionResult Index()
        {

            if (Areas.Find(p => p.Cod_Area == "25") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;
            return View();

        }

        //
        public ActionResult CadastroClientePasso1()
        {
            ViewBag.HabilitaCadastro = false;

            return View(new Cliente_ProspectoPasso1ViewModel
            {
                TipoPessoa = Tipo_Pessoa.Fisica
            });
        }

        [HttpPost]
        public ActionResult CadastroClientePasso1(Cliente_ProspectoPasso1ViewModel valor)
        {
            ViewBag.HabilitaCadastro = false;

            Cliente_ProspectoPasso1ViewModel retorno = valor;



            if (Request.Form["validar"] != null)
            {
                try
                {
                    if (DateTimeUtil.GetIdade(Convert.ToDateTime(valor.DataNascimento)) < 18)
                    {
                        throw new Exception(RsCadastros.NAO_PERMITIDO_18);
                    }

                    if (valor.TipoPessoa == Tipo_Pessoa.Fisica)
                    {
                        String _cpf = valor.CPF.Replace(".", String.Empty).Replace("-", String.Empty);

                        if (new ClienteApiController().Existecpf(_cpf).Result > 0)
                            throw new Exception(String.Format("O CPF {0} já se encontra cadastrado", _cpf));

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CPF, _cpf, valor.DataNascimento);
                        ResponseHub<ResponseCpf> resultadoHUB = GetByUrl<ResponseHub<ResponseCpf>>(_url);

                        retorno.retornoCPF = resultadoHUB;

                        if (retorno.retornoCPF.result == null)
                        {
                            Boolean acesso137 = TemAcesso("137");
                            String _mensage = retorno.retornoCPF.message;
                            retorno.retornoCPF = null;
                            ViewBag.HabilitaCadastro = acesso137;

                            if (_mensage == "Token Inválido ou sem saldo para a consulta.")
                            {
                                if (acesso137)
                                {
                                    throw new Exception(RsCadastros.SEM_ACESSO_BASE_RECEITA);
                                }
                                else
                                {
                                    throw new Exception(RsCadastros.USUARIO_SEM_ACESSO137);
                                }
                            }
                            else
                            {
                                throw new Exception(_mensage);
                            }
                        }
                    }

                    else if (valor.TipoPessoa == Tipo_Pessoa.Juridica)
                    {
                        String _cnpj = valor.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                        if (new ClienteApiController().Existecpf(_cnpj).Result > 0)
                            throw new Exception(String.Format("O CNPJ {0} já se encontra cadastrado", _cnpj));

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CNPJ, _cnpj);
                        ResponseHub<ResponseCnpj> resultadoHUB = GetByUrl<ResponseHub<ResponseCnpj>>(_url);

                        retorno.retornoCNPJ = resultadoHUB;

                        if (retorno.retornoCNPJ.result == null)
                        {
                            String _mensage = retorno.retornoCNPJ.message;
                            retorno.retornoCNPJ = null;
                            ViewBag.HabilitaCadastro = true;

                            if (_mensage == "Token Inválido ou sem saldo para a consulta.")
                            {
                                throw new Exception(RsCadastros.SEM_ACESSO_BASE_RECEITA);
                            }
                            else
                            {
                                throw new Exception(_mensage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    if (ex.Message == RsCadastros.NAO_PERMITIDO_18)
                    {
                        ViewBag.HabilitaCadastro = false;
                    }

                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.GetType() == typeof(System.Net.Http.HttpRequestException))
                        {
                            Boolean acesso137 = TemAcesso("137");
                            ViewBag.HabilitaCadastro = acesso137;
                            AddModelError(RsCadastros.ERRO_COMUNICACAO_VALIDACAO_CPF, String.Empty, String.Empty);
                        }
                    }
                    else
                    {
                        AddModelError(ex.Message, String.Empty, String.Empty);
                    }
                }

                return View(retorno);
            }
            else if (Request.Form["cadastrar"] != null)
            {
                return RedirectToAction("ClienteDadosGerais", new
                {
                    tipo_pessoa = valor.TipoPessoa,
                    cpfcnpj = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.CPF : valor.CNPJ,
                    datanascimento = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.DataNascimento : null
                });
            }

            return RedirectToAction("Index", "Cliente_Prospecto");
        }

        [HttpPost]
        public async Task<String> VerificarClienteReceita(Cliente_ProspectoPasso1ViewModel valor)
        {
            var retorno = valor;
            try
            {
                if (DateTimeUtil.GetIdade(Convert.ToDateTime(valor.DataNascimento)) < 18)
                {
                    throw new Exception(RsCadastros.NAO_PERMITIDO_18);
                }

                if (valor.TipoPessoa == Tipo_Pessoa.Fisica)
                {
                    String _cpf = valor.CPF.Replace(".", String.Empty).Replace("-", String.Empty);

                    if (new ClienteApiController().Existecpf(_cpf).Result > 0)
                        throw new Exception(String.Format("O CPF {0} já se encontra cadastrado", _cpf));

                }

                else if (valor.TipoPessoa == Tipo_Pessoa.Juridica)
                {
                    String _cnpj = valor.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                    if (new ClienteApiController().Existecpf(_cnpj).Result > 0)
                        throw new Exception(String.Format("O CNPJ {0} já se encontra cadastrado", _cnpj));
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.Message == RsCadastros.NAO_PERMITIDO_18)
                {
                    ViewBag.HabilitaCadastro = false;
                }

                if (ex.InnerException != null)
                {
                    if (ex.InnerException.GetType() == typeof(System.Net.Http.HttpRequestException))
                    {
                        Boolean acesso137 = TemAcesso("137");
                        ViewBag.HabilitaCadastro = acesso137;
                        //AddModelError(RsCadastros.ERRO_COMUNICACAO_VALIDACAO_CPF, String.Empty, String.Empty);
                    }
                }
                else
                {
                    AddModelError(ex.Message, String.Empty, String.Empty);
                }
            }

            return JsonConvert.SerializeObject(retorno);
        }


        static RequiredAttribute requeridoAttr = new RequiredAttribute();

        public async Task<ActionResult> ClienteDadosGerais(Int64? Id, Tipo_Pessoa? tipo_pessoa, String cpfcnpj, String datanascimento)
        {
            TempData["Usuario_Vendedor"] = false;
            using (ISession session = HibernateUtil.GetSession())
            using (var repoUsuario = new Conexao<IdentityUser>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                var usuario = repoUsuario.selectFirst(p => p.Id == User.Identity.GetUserId()).Result;

                if (usuario?.Funcionario != null && usuario.Funcionario.Vendedor)
                {
                    TempData["Usuario_Vendedor"] = usuario.Funcionario.Vendedor;
                }
            }

            ViewResult retorno = (ViewResult)base.CadastroBase<Cliente_ProspectoDadosGeraisViewModel, Int64?>(Id);

            if (!Id.HasValue)
            {
                Cliente_ProspectoDadosGeraisViewModel _model = ((Cliente_ProspectoDadosGeraisViewModel)retorno.Model);
                _model.TipoPessoa = tipo_pessoa ?? Tipo_Pessoa.Fisica;

                if (_model.TipoPessoa == Tipo_Pessoa.Fisica)
                    _model.Cpf = cpfcnpj;
                else
                    _model.Cnpj = cpfcnpj;

                _model.DataNascimento = datanascimento;

                // Pegando as consultas habilitadas no cadastro do lead quando o cadastro está
                // sendo aberto para um novo lead
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                    IEnumerable<EnumEtapaVenda> consultasHabilitadas = configBusiness.GetConsultasRestricaoHabilitadasLead();

                    if (consultasHabilitadas.Count() > 0)
                    {
                        _model.ConsultaViabilidadeHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaViabilidade);

                        _model.ConsultaPendenciaFinanceiraHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaFinanceira);

                        _model.ConsultaPendenciaEnderecoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaEndereco);

                        _model.ConsultaCreditoHabilitada =
                            consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaCredito);
                    }

                    _model.ConsultaPendenciaViabilidadeValidade = config.ConsultaViabilidadeTempoValidade != null ? config.ConsultaViabilidadeTempoValidade : null;
                    _model.ConsultaPendenciaEnderecoValidade = config.ConsultaPendenciaEnderecoTempoValidade != null ? config.ConsultaPendenciaEnderecoTempoValidade : null;
                }
            }

            FillListasView(retorno);

            return retorno;

        }

        [HttpPost]
        public async Task<ActionResult> ClienteDadosGerais(Cliente_ProspectoDadosGeraisViewModel valor)
        {
            try
            {
                using (var configApi = new Configuracoes_GeraisApiController())
                {

                    var config = (await configApi.FindById(1)).Objeto;

                    if (config.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato)
                    {
                        using (NHibernate.ISession session = HibernateUtil.GetSession())
                        {
                            List<ICriterion> listaCriteria = new List<ICriterion>();
                            var temp_identificacao_cliente = valor.Numero_Contrato == null ? String.Empty : valor.Numero_Contrato;

                            listaCriteria.Add(NHibernate.Criterion.Expression.Eq(nameof(Cliente.Numero_Contrato), temp_identificacao_cliente).IgnoreCase());
                            if (valor.Id != 0)
                                listaCriteria.Add(NHibernate.Criterion.Expression.Not(Restrictions.Eq(nameof(Cliente.Id), valor.Id)));

                            var cliente = new Conexao<Cliente>(session).selectCriteria(listaCriteria.ToArray()).Result.FirstOrDefault();

                            if (cliente != null)
                            {
                                AddModelError(RsCadastros.IDENTIFICACAO_CLIENTE_JA_EXISTE);
                            }

                        }
                    }
                }


                if (ModelState.IsValid && Validacoes(valor))
                {
                    PostApiRetorno<Int64> retorno = PostByUrl<Cliente_ProspectoDadosGeraisViewModel, Int64>(valor, UrlPortal, UrlPost);

                    if (retorno.Ok) //Edição
                    {
                        try
                        {
                            using (var configApi = new Configuracoes_GeraisApiController())
                            using (NHibernate.ISession sessionNh = HibernateUtil.GetSession())
                            {
                                //--   Sms   --//
                                var config = (await configApi.FindById(1)).Objeto;

                                if (config.ValidaNumeroTelefone)
                                {
                                    using (var repoIntegracaoSMS = new Conexao<Sinapse.Model.SMS>(sessionNh))
                                    using (var repoModeloMSG = new Conexao<Configuracao_Envio_Mensagens>(sessionNh))
                                    using (var SMSapi = new IntegracaoSMSApiController())
                                    {
                                        Configuracao_Envio_Mensagens modeloMSG = await repoModeloMSG.selectFirst(p => p.Id == config.IdModeloMensagemSMS);
                                        Sinapse.Model.SMS MensagemAnterior = (await repoIntegracaoSMS.selectFirst(p => p.Num_Celular == valor.Telefone_Celular));

                                        var request = new EnviarSMS_RequestModel
                                        {
                                            Id_Integracao = MensagemAnterior?.Id,
                                            isReenvio = MensagemAnterior != null ? true : false,
                                            Mensagem = modeloMSG.Descricao,
                                            Num_Celular = valor.Telefone_Celular,
                                            Cliente = retorno.IdObjeto,
                                            Salvar_Em_Banco = true,
                                            Tipo_Integracao = config.Tipo_Integracao_SMS.Value,
                                        };

                                        var respostaEndpoint = await SMSapi.EnviarSMS(request);

                                        if (respostaEndpoint.Ok)
                                        {
                                            // o que acontece se for enviada com sucesso na gravação de registro?
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        return RedirectToAction("ClienteDadosGerais", new { Id = retorno.IdObjeto, Sucesso = "S" });


                    }
                    else
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }
                }


                FillListasView(valor);
            }
            catch (Exception ex)
            {
                FillListasView(valor);

                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(valor);
        }

        public override bool Validacoes<T>(T pModel)
        {
            List<CamposObrigatoriosLeadModels> campos = new CamposObrigatoriosLeadApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            Cliente_ProspectoDadosGeraisViewModel _model = pModel as Cliente_ProspectoDadosGeraisViewModel;

            if (_model.TipoPessoa == Tipo_Pessoa.Juridica)
            {
                foreach (var campo in campos)
                {
                    if (campo.Obrigatorio)
                        switch (campo.Campo)
                        {
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CPF_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.CPFResponsavel))
                                    erros.Add("CPFResponsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CPF_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CARGO_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.Cargo_Responsavel))
                                    erros.Add("Cargo_Responsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CARGO_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue && !_model.Id_Categoria_ClienteJuridico.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.Telefone_Celular))
                                    erros.Add("Telefone_Celular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.INSCRICAO_ESTADUAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Estadual))
                                    erros.Add("Inscricao_Estadual", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_ESTADUAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.INSCRICAO_MUNICIPAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Municipal))
                                    erros.Add("Inscricao_Municipal", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_MUNICIPAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.PORTE:
                                if (!_model.Porte.HasValue)
                                    erros.Add("Porte", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PORTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.REPRESENTANTE:
                                if (String.IsNullOrEmpty(_model.Representante))
                                    erros.Add("Representante", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.REPRESENTANTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.RAMO_ATIVIDADE:
                                if (String.IsNullOrEmpty(_model.Ramo_Atividade))
                                    erros.Add("RamoAtividade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.RAMO_ATIVIDADE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.RESPONSAVEL_FINANCEIRO:
                                if (String.IsNullOrEmpty(_model.Responsavel_Financeiro))
                                    erros.Add("ResponsavelFinanceiro", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.RESPONSAVEL_FINANCEIRO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.DATA_ABERTURA:
                                if (!_model.Data_Abertura.HasValue)
                                    erros.Add("Data_Abertura", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATA_ABERTURA));
                                break;

                            case Model.enumerado.CamposObrigatoriosLeadEnum.PAIS:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (!_model.EnderecoProspecto.Id_Pais.HasValue)
                                        erros.Add("Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CEP:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.CEP))
                                        erros.Add("CEP", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.COMPLEMENTO:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.Complemento))
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.Ponto_Referencia))
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TIPO_IMOVEL:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (!_model.EnderecoProspecto.Id_Tipo_Imovel.HasValue)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                        }
                }
            }
            else
            {
                foreach (var campo in campos)
                {
                    if (campo.Obrigatorio)
                        switch (campo.Campo)
                        {
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue && !_model.Id_Categoria_ClienteJuridico.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.Telefone_Celular))
                                    erros.Add("Telefone_Celular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.MEIO_CAPTACAO:
                                if (!_model.Id_Meio_Captacao.HasValue)
                                    erros.Add("Id_Meio_Captacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.MEIO_CAPTACAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.EMAIL_PRINCIPAL:
                                if (String.IsNullOrEmpty(_model.Email))
                                    erros.Add("Email", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EMAIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.ESTADO_CIVIL:
                                if (!_model.EstadoCivil.HasValue)
                                    erros.Add("EstadoCivil", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ESTADO_CIVIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.NACIONALIDADE:
                                if (String.IsNullOrEmpty(_model.Nacionalidade))
                                    erros.Add("Nacionalidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NACIONALIDADE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.NOME_MAE:
                                if (String.IsNullOrEmpty(_model.Mae))
                                    erros.Add("Mae", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.MAE));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.NOME_PAI:
                                if (String.IsNullOrEmpty(_model.Pai))
                                    erros.Add("Pai", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAI));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.RG:
                                if (String.IsNullOrEmpty(_model.RG))
                                    erros.Add("RG", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.RG));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.SEXO:
                                if (!_model.Sexo.HasValue)
                                    erros.Add("Sexo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SEXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.DATA_NASCIMENTO:
                                if (String.IsNullOrEmpty(_model.DataNascimento))
                                    erros.Add("DataNascimento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATANASCIMENTO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.DATA_EMISSAO:
                                if (String.IsNullOrEmpty(_model.DataEmissao))
                                    erros.Add("DataEmissao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATAEMISSAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.ORGAO:
                                if (String.IsNullOrEmpty(_model.Orgao))
                                    erros.Add("Orgao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ORGAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.PAIS:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (!_model.EnderecoProspecto.Id_Pais.HasValue)
                                        erros.Add("Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.CEP:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.CEP))
                                        erros.Add("CEP", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.COMPLEMENTO:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.Complemento))
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoProspecto.Ponto_Referencia))
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA));
                                }

                                break;
                            case Model.enumerado.CamposObrigatoriosLeadEnum.TIPO_IMOVEL:
                                if (_model.EnderecoProspecto != null)
                                {
                                    if (!_model.EnderecoProspecto.Id_Tipo_Imovel.HasValue)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                        }
                }
            }

            if (!String.IsNullOrEmpty(_model.DataNascimento))
            {

                if (DateTimeUtil.GetIdade(Convert.ToDateTime(_model.DataNascimento)) < 18)
                {
                    erros.Add("", String.Format(Resources.RsBase.REQUIRED, RsCadastros.NAO_PERMITIDO_18));
                }
            }

            if (erros.Count > 0)
            {
                AddModelError(erros);
            }

            return erros.Count == 0;
        }


        protected override void FillListasView<T>(T pModel)
        {

            TempData["Usuario_Vendedor"] = false;
            using (ISession session = HibernateUtil.GetSession())
            using (var repoUsuario = new Conexao<IdentityUser>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                var usuario = repoUsuario.selectFirst(p => p.Id == User.Identity.GetUserId()).Result;

                if (usuario?.Funcionario != null && usuario.Funcionario.Vendedor)
                {
                    TempData["Usuario_Vendedor"] = usuario.Funcionario.Vendedor;
                }
            }

            var campos = new CamposObrigatoriosLeadApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campos);

            List<CamposObrigatoriosClienteModels> camposObrigatoriosCliente = new CamposObrigatoriosClienteApiController().GetCampos().Result;
            ViewBag.CamposObrigatoriosCliente = camposObrigatoriosCliente;

            var config = new Configuracoes_GeraisApiController().FindById(1).Result;
            ViewBag.Identificacao_Cliente = Convert.ToInt32(config.Objeto.Identificacao_Cliente);

            ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(config.Objeto.Utiliza_Cadastro_Endereco);
            ViewBag.Tipos_Imovel = new Tipo_ImovelAnielApiController().GetList().Result.Objeto;
            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = Areas;
            ViewBag.Localidades = new List<String>();
            ViewBag.Ofertar_Plano_Produto_Cobertura = new Conexao<Configuracoes_Gerais>(HibernateUtil.GetSession()).selectFirst().Result.Ofertar_Plano_Produto_Cobertura;


            ViewBag.HabilitarConsultaCredito = config.Objeto.ConsultaCreditoExibeLead;
            ViewBag.ValidaNumeroTelefone = config.Objeto.ValidaNumeroTelefone;
            ViewBag.HabilitarConsultaPendenciaEndereco = config.Objeto.ConsultaPendenciaEnderecoExibeLead;

            ViewBag.ConsultaPendenciaViabilidadeValidade = config.Objeto.ConsultaViabilidadeTempoValidade != null ? config.Objeto.ConsultaViabilidadeTempoValidade : 0;
            ViewBag.ConsultaPendenciaEnderecoValidade = config.Objeto.ConsultaPendenciaEnderecoTempoValidade != null ? config.Objeto.ConsultaPendenciaEnderecoTempoValidade : 0;
            ViewBag.ValidarDocumentoLead = config.Objeto.ValidarDocumentoLead;


            var _retornoModelos_Contrato = new Modelo_ContratoApiController().GetList().Result;
            ViewBag.Modelos_Contrato = new List<Modelo_ContratoListaModel>();
            if (!_retornoModelos_Contrato.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoModelos_Contrato.Mensagem, _retornoModelos_Contrato.InnerException);

            ViewBag.Modelos_Contrato = _retornoModelos_Contrato.Objeto;

            using (var session = HibernateUtil.GetSession())
            using (var repoUser = new Conexao<IdentityUser>(session))
            {
                string loginUsuario = User.Identity.GetUserId();
                ViewBag.DepartamentoUsuarioLogado = repoUser.selectFirst(p => p.Id == loginUsuario).Result?.Departamento?.Id;

            }




            ViewBag.HabilitarConsultaCredito = config.Objeto.ConsultaCreditoExibeLead;
            ViewBag.ValidaNumeroTelefone = config.Objeto.ValidaNumeroTelefone;
            ViewBag.HabilitarConsultaPendenciaEndereco = config.Objeto.ConsultaPendenciaEnderecoExibeLead;

            ViewBag.ConsultaPendenciaViabilidadeValidade = config.Objeto.ConsultaViabilidadeTempoValidade != null ? config.Objeto.ConsultaViabilidadeTempoValidade : 0;
            ViewBag.ConsultaPendenciaEnderecoValidade = config.Objeto.ConsultaPendenciaEnderecoTempoValidade != null ? config.Objeto.ConsultaPendenciaEnderecoTempoValidade : 0;
            ViewBag.ValidarDocumentoLead = config.Objeto.ValidarDocumentoLead;



        }
        

        private void GetListaAnexarArquivo(Int64 Id_Cliente_Prospecto)
        {
            GetApiRetorno<List<ClienteAnexoModelView>> retornoLista = base.GetAll<ClienteAnexoModelView>($"api/Cliente_Prospectoapi/GetAnexos?id_Cliente_Prospecto={Id_Cliente_Prospecto}");

            if (!retornoLista.Ok)
            {
                AddModelError(retornoLista.Mensagem);
                ViewBag.Lista = new List<ClienteAnexoModelView>();
            }
            else
            {
                ViewBag.Lista = retornoLista.Objeto;
                ViewBag.Areas = Areas;
            }

        }

        public async Task<ActionResult> ListaLoginRadius(Int64 Id_Cliente_Prospecto)
        {
            var retorno = await new PlanoProdutoApiController().GetListLogin(Id_Cliente_Prospecto);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

        public ActionResult AnexarArquivo(Int64 Id_Cliente)
        {
            GetListaAnexarArquivo(Id_Cliente);
            if (Areas.Find(p => p.Cod_Area == "263") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;
            return View(new ClienteAnexoModelView
            {
                Id_Cliente = Id_Cliente
            });
        }

        [HttpPost]
        public ActionResult AnexarArquivo(ClienteAnexoModelView Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Form["gravarfoto"] != null)
                    {
                        return AnexarArquivoAux(Model, true);
                    }
                    else
                    {
                        return AnexarArquivoAux(Model, false);
                    }
                }
                else
                {
                    return View(Model);
                }
            }
            catch (Exception ex)
            {
                if (ViewBag.Lista == null) GetListaAnexarArquivo(Model.Id_Cliente);

                AddModelError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                return View(Model);
            }
        }

        public ActionResult AnexarArquivoAux(ClienteAnexoModelView Model, Boolean WebCam)
        {
            try
            {
                if (WebCam)
                {
                    Model.Nome_Arquivo = "Camera.jpg";
                    Model.Extensao_Arquivo = "jpg";
                }
                else if (Request.Files.Count > 0)
                {
                    Model.Nome_Arquivo = Request.Files[0].FileName;
                    Model.Extensao_Arquivo = Model.Nome_Arquivo.Split('.').Last();
                }

                PostApiRetorno<Int64> retornoPost = base.PostByUrl<ClienteAnexoModelView, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Cliente_Prospectoapi/GravarAnexo");

                GetListaAnexarArquivo(Model.Id_Cliente);

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    return View(Model);
                }
                else
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente_Prospecto\{Model.Id_Cliente}";
                    String _arquivo = $@"{_pasta}\{retornoPost.IdObjeto}.{Model.Extensao_Arquivo}";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    if (WebCam)
                    {
                        Image imagem1 = ImageService.Base64ToImage(Model.FotoWebCam.Replace("data:image/png;base64,", String.Empty));
                        imagem1.Save(_arquivo);
                    }
                    else if (Request.Files.Count > 0)
                    {
                        Request.Files[0].SaveAs(_arquivo);
                    }

                    return RedirectToAction("AnexarArquivo", new { Id_Cliente_Prospecto = Model.Id_Cliente });
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(Model);
            }
        }

        public void CarregaListasEndereco()
        {
            ViewBag.ComboTmp = new List<ComboObj>();
        }

        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Cliente_ProspectoImportacao> modelo = new ExcelServiceLog<Cliente_ProspectoImportacao>
            {
                Items = new List<Cliente_ProspectoImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Arquivo)
        {
            ExcelServiceLog<Cliente_ProspectoFisicaImportacao> modeloFisico = new ExcelServiceLog<Cliente_ProspectoFisicaImportacao>
            {
                Items = new List<Cliente_ProspectoFisicaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<Cliente_ProspectoJuridicaImportacao> modeloJuridico = new ExcelServiceLog<Cliente_ProspectoJuridicaImportacao>
            {
                Items = new List<Cliente_ProspectoJuridicaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<Cliente_ProspectoImportacao> modelo = new ExcelServiceLog<Cliente_ProspectoImportacao>
            {
                Items = new List<Cliente_ProspectoImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            try
            {
                if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                {
                    AddModelError("Informe Planilha a ser Importada");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Cliente_ProspectoFisicaImportacao)));
                        modeloFisico = ExcelService.CastDataTableImportacao<Cliente_ProspectoFisicaImportacao>(dt);

                        DataTable dtJuridico = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Cliente_ProspectoJuridicaImportacao)));
                        modeloJuridico = ExcelService.CastDataTableImportacao<Cliente_ProspectoJuridicaImportacao>(dtJuridico);

                        using (Cliente_ProspectoApiController prospecto_api = new Cliente_ProspectoApiController())
                        {
                            foreach (var item in modeloFisico.Items)
                            {
                                try
                                {
                                    modeloFisico.Log.AddRange(await prospecto_api.ImportacaoModeloFisico(item));
                                }
                                catch (Exception ex)
                                {
                                    modeloFisico.Log.Add(new ExcelServiceLogItem
                                    {
                                        Chave = item.Numero_Contrato,
                                        Erro = $"Erro ao processar o prospect com Identificação de Cliente: {item.Numero_Contrato}"
                                    });
                                    CONSTANTES.log4Net.Error(ex.Message, ex);
                                }
                            }

                            foreach (var item in modeloJuridico.Items)
                            {
                                try
                                {
                                    modeloJuridico.Log.AddRange(await prospecto_api.ImportacaoModeloJuridico(item));
                                }
                                catch (Exception ex)
                                {
                                    modeloFisico.Log.Add(new ExcelServiceLogItem
                                    {
                                        Chave = item.Numero_Contrato,
                                        Erro = $"Erro ao processar o prospect com Identificação de Cliente: {item.Numero_Contrato}"
                                    });
                                    CONSTANTES.log4Net.Error(ex.Message, ex);
                                }
                            }
                        }

                    }

                    modelo.Items.AddRange(modeloFisico.Items);
                    modelo.Log.AddRange(modeloFisico.Log);
                    modelo.Items.AddRange(modeloJuridico.Items);
                    modelo.Log.AddRange(modeloJuridico.Log);

                    ViewBag.Sucesso = modelo.Log.Count == 0 ? "S" : "N";
                    return View(modelo);

                }
                else
                {
                    return View(modelo);
                }

            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(modelo);
            }
        }


        public ActionResult ConsultaCredito(string Id_Cliente)
        {
            ViewBag.IdCliente = Id_Cliente;
            return View();
        }
        public ActionResult ConsultaPendenciaEndereco()
        {
            return View();
        }

    }
}