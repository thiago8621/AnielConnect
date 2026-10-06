using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.Hubsoft.ClienteModel;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.Connect.Repository.Repository.Log;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.T5.Model.nhibernate;
using NHibernate.Mapping;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.SerasaApi.Server.Responses;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ElementoRedeBusiness
    {
        private IElementoRedeAnielRepository _repository;
        private Dictionary<string, object> _cores;

        public ElementoRedeBusiness(IElementoRedeAnielRepository repository, Dictionary<string, object> cores)
        {
            this._repository = repository;
            this._cores = cores;
        }

        public async Task<Elemento_RedeCadastroModel> FindById(Int64? Id)
        {
            Elemento_RedeCadastroModel retorno = new Elemento_RedeCadastroModel();

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (var repoPais = new Conexao<Pais>(session))
                using (var repoCidade = new Conexao<Model.nhibernate.Cidade>(session))
                using (var repoBloco = new Conexao<Condominio_PredioBloco>(session))
                using (var repoApto = new Conexao<Condominio_PredioApartamento>(session))
                using (var repoCondominioPredio = new Conexao<Condominio_Predio>(session))
                using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
                {

                    var elemento = this._repository.GetById(Id.GetValueOrDefault(0));
                    retorno = this.GetViewObj(elemento);
                    retorno.Lista_Objetos = this._repository.GetCamposDinamicosByElement(Id.GetValueOrDefault(0))
                        .Select(p =>
                        {
                            dynamic dinamico = new
                            {
                                value = p.Valor,
                                idDinamico = p.Id_Campo_Dinamico,
                                tipo = p.Tipo
                            };
                            object obj = dinamico;
                            return obj;
                        }).ToList();

                    retorno.Arquivos = this._repository.GetFilesByElement(Id.GetValueOrDefault(0)).Select(p =>
                        new Elemento_RedeFotoArquivo
                        {
                            Descricao = p.Descricao,
                            Id = p.Id,
                            Tipo = p.Tipo_FotoArquivo,
                        }).ToList();

                    if (elemento?.Tipo?.E_Cabo ?? false)
                    {
                        var obj = this._repository.GetFibrasByElement(Id.GetValueOrDefault(0));

                        retorno.Conexoes = this.GetConexoes(obj);
                    }

                    var Enderecos = this._repository.GetEnderecosByElement(Id.GetValueOrDefault(0));

                    // Lista para armazenar endereços e condomínios
                    var listaEnderecos = new List<Elemento_RedeEnderecosModel>();
                    var listaCondominios = new List<Elemento_RedeCondominiosModel>();

                    var dict_End_Log = new Dictionary<long, Endereco_Logradouro>();
                    if (Enderecos.Count() > 0 && Enderecos.Any(p => p.Id_EnderecoLogradouro != null && p.Id_EnderecoLogradouro != 0))
                    {
                        Enderecos.Where(p => p.Id_EnderecoLogradouro != null).ToList().ForEach(async endereco =>
                        {
                            dict_End_Log.Add(endereco.Id, (await repoEnderecoLogradouro.selectFirst(p => p.Id == endereco.Id_EnderecoLogradouro)));
                        });
                    }

                    //Condomínios
                    if (Enderecos.Where(p => p.Id_CondominioPredio != null && p.Id_CondominioPredio != 0).Count() > 0)
                    {
                        foreach (var parametro in Enderecos.Where(p => p.Id_CondominioPredio != null && p.Id_CondominioPredio != 0))
                        {
                            Condominio_Predio condominio = (await repoCondominioPredio.selectFirst(p => p.Id == parametro.Id_CondominioPredio));
                            Pais pais = condominio.Id_Pais != null ? (await repoPais.selectFirst(p => p.Id == condominio.Id_Pais)) : condominio.Endereco_Logradouro?.Pais;
                            Model.nhibernate.Cidade cidade = condominio.Id_Cidade != null ? (await repoCidade.selectFirst(p => p.Id == condominio.Id_Cidade)) : condominio.Endereco_Logradouro?.Cidade;
                            Condominio_PredioBloco bloco = parametro.Todos_Bloco == true ? new Condominio_PredioBloco() : await repoBloco.selectFirst(p => p.Id == Convert.ToInt16(parametro.Bloco));
                            Condominio_PredioApartamento apto = parametro.Todos_Apto_Numero == true ? new Condominio_PredioApartamento() : await repoApto.selectFirst(p => p.Id == Convert.ToInt16(parametro.Apto_numero));

                            var enderecoModel = new Elemento_RedeCondominiosModel
                            {
                                Id = parametro.Id,
                                Numero = parametro.Numero,
                                Id_Logradouro = parametro.Id_EnderecoLogradouro.GetValueOrDefault(0),
                                Pais = pais?.Nome,
                                Uf = condominio?.Uf ?? condominio.Endereco_Logradouro?.UF?.Descricao,
                                Bairro = condominio?.Bairro ?? condominio.Endereco_Logradouro?.Bairro?.Descricao,
                                Cidade = cidade?.Descricao,
                                Cep = condominio?.Cep,
                                Descricao = condominio?.Descricao,
                                Status = condominio?.Status != null ? HtmlEnumExtensions.GetEnumDescription(condominio?.Status) : "",
                                Id_Condominio = condominio?.Id,
                                Bloco = parametro.Todos_Bloco == true ? "Todos" : bloco?.Descricao,
                                Id_Bloco = parametro.Todos_Bloco == true ? null : bloco?.Id,
                                Apto_Numero = parametro.Todos_Apto_Numero == true ? "Todos" : apto?.Descricao,
                                Id_Apto_Numero = parametro.Todos_Apto_Numero == true ? null : apto?.Id
                            };

                            if (condominio.Endereco_Logradouro != null)
                            {
                                enderecoModel.Tipo_Endereco_Logradouro = condominio.Endereco_Logradouro.Tipo == null ? "-" : HtmlEnumExtensions.GetEnumDescription(condominio?.Endereco_Logradouro.Tipo);
                                enderecoModel.Tipo_Atendimento_Zona = condominio.Endereco_Logradouro.Tipo_Zona == null ? "-" : HtmlEnumExtensions.GetEnumDescription(condominio?.Endereco_Logradouro.Tipo_Zona);
                            }
                            else
                            {
                                enderecoModel.Tipo_Endereco_Logradouro = "-";
                                enderecoModel.Tipo_Atendimento_Zona = "-";
                            }

                            retorno.Condominios.Add(enderecoModel);
                        }
                    }

                    //Endereços
                    if (Enderecos.Where(p => p.Id_EnderecoLogradouro != null && p.Id_EnderecoLogradouro != 0).Count() > 0)
                    {
                        foreach (var parametro in dict_End_Log)
                        {
                            if (parametro.Value == null)
                                continue;

                            var enderecoModel = new Elemento_RedeEnderecosModel
                            {
                                Id = parametro.Key,
                                Numero = Enderecos.FirstOrDefault(p => p.Id == parametro.Key)?.Numero,
                                Id_Logradouro = parametro.Value?.Id ?? 0,
                                Pais = parametro.Value.Pais?.Nome,
                                Uf = parametro.Value.UF?.Descricao,
                                Bairro = parametro.Value.Bairro?.Descricao,
                                Cidade = parametro.Value.Cidade?.Descricao,
                                Cep = parametro.Value?.Cep,
                                Descricao = parametro.Value?.Descricao,
                                Status = parametro.Value?.Status != null ? HtmlEnumExtensions.GetEnumDescription(parametro.Value?.Status) : "",
                                Tipo_Endereco_Logradouro = parametro.Value.Tipo == null ? String.Empty : HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(parametro.Value.Tipo.Value),
                                Tipo_Atendimento_Zona = parametro.Value.Tipo_Zona == null ? string.Empty : HtmlEnumExtensions.GetEnumDescription<Tipo_Atendimento_Zona>(parametro.Value.Tipo_Zona.Value)
                            };

                            retorno.Enderecos.Add(enderecoModel);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return retorno;
        }
        private Elemento_RedeCadastroModel GetViewObj(Elemento_Rede bancoElemento)
        {
            var retorno = new Elemento_RedeCadastroModel();

            retorno.Id = bancoElemento.Id;
            retorno.Descricao = bancoElemento.Descricao;
            retorno.Capacidade = bancoElemento.Capacidade;
            retorno.Coordenadas = bancoElemento.Coordenadas;
            retorno.Bairro = bancoElemento.Bairro;
            retorno.Pais = bancoElemento.Pais;
            retorno.Cep = bancoElemento.CEP;
            retorno.Cidade = bancoElemento.Cidade;
            retorno.Complemento = bancoElemento.Complemento;
            retorno.Endereco = bancoElemento.Endereco;
            retorno.Latitude = bancoElemento.Latitude;
            retorno.Longitude = bancoElemento.Longitude;
            retorno.Uf = bancoElemento.UF;
            retorno.Tem_Certificado = bancoElemento.Certificado;
            retorno.Cor_Linha_Ativa = bancoElemento.Cor_linha_Ativa;
            retorno.Cor_Linha_Inativa = bancoElemento.Cor_linha_Inativa;
            retorno.Cabo = bancoElemento.Cabo?.Id;
            retorno.Id_OLT = bancoElemento.Id_OLT;
            retorno.Id_Slot = bancoElemento.Id_Slot;
            retorno.Id_Pon = bancoElemento.Id_Pon;
            retorno.Poste = bancoElemento.Poste?.Id;
            retorno.Contrato = bancoElemento.Contrato;
            retorno.Id_POP = bancoElemento.Id_POP;
            retorno.Numero = bancoElemento.Numero;
            retorno.Nos = bancoElemento.Nos != null ? bancoElemento.Nos.Split('/')[0] : null;
            retorno.UsuarioCriacao = bancoElemento.UsuarioCriacao;
            retorno.DataCriacao = bancoElemento.DataHoraCriacao;
            retorno.NumDoc = bancoElemento.NumDoc;
            retorno.Observacao = bancoElemento.Observacao;
            retorno.Projeto = bancoElemento.Projeto;
            retorno.Situacao = bancoElemento.Situacao;
            retorno.IdTipo = bancoElemento.Tipo?.Id;
            retorno.DescricaoTipo = bancoElemento.Tipo?.Descricao;
            retorno.Metragem = bancoElemento.Metragem.HasValue ? bancoElemento.Metragem.Value.ToString() : null;
            retorno.Caixa = bancoElemento.Caixa?.Id;
            retorno.AreaAtendimento = bancoElemento.AreaAtendimento;
            return retorno;
            
        }
        public IEnumerable<Elemento_RedeConexoes> GetConexoes(IEnumerable<Elemento_Rede_Fibra> Fibras)
        {
            var retorno = new List<Elemento_RedeConexoes>();

            using (var sessionT5 = HibernateUtil.GetSessionT5())

            /*         
                =-=-=-          Repository Usage          =-=-=-                            
             */
            using (var RepoElemento = new Conexao<Elemento_Rede>(sessionT5))
            using (var RepoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
            using (var RepoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

            {
                Func<Elemento_Rede_Fibra, int, Task<Elemento_RedeConexoes>> getConexaoAsync = async (p, i) =>
                {

                    Elemento_Rede Origem = (await RepoElemento.selectFirst(param => param.Id == p.Ponta_A)),
                                  Destino = (await RepoElemento.selectFirst(param => param.Id == p.Ponta_B));

                    Tipo_Elemento_Rede TipoOrigem = (await RepoTipo.selectFirst(param => param.Id == Origem.Tipo.Id)),
                                       TipoDestino = (await RepoTipo.selectFirst(param => param.Id == Destino.Tipo.Id));


                    String colorLoose = this.getColorLoose(p.Elemento_Cabo.Padrao_Cores, Convert.ToInt16(p.numLoose), this._cores).text;
                    return new Elemento_RedeConexoes
                    {
                        Id = ++i,

                        Id_Cabo = p.Elemento_Cabo.Id,
                        Descricao_Cabo = $"{p.Elemento_Cabo.Id} - Loose:{colorLoose} - {p.numFibra}",
                        Id_FibraUtilizada = p.Id,

                        Numero_Fibra = p.numFibra,
                        Numero_Loose = p.numLoose,

                        portaDestino = (await RepoPorta.selectFirst(parametro => parametro.Id == p.Conexao_Ponta_A)).Porta.Value,
                        portaOrigem = (await RepoPorta.selectFirst(parametro => parametro.Id == p.Conexao_Ponta_B)).Porta.Value,

                        Id_Origem = p.Ponta_A.Value,
                        Id_Conexao_Origem = p.Conexao_Ponta_A.Value,
                        Descricao_Elemento_Origem = Origem.Descricao,
                        Tipo_Elemento_Origem = TipoOrigem.Id,
                        Descricao_Tipo_Elemento_Origem = TipoOrigem.Descricao,

                        Id_Destino = p.Ponta_B.Value,
                        Id_Conexao_Destino = p.Conexao_Ponta_B.Value,
                        Descricao_Elemento_Destino = Destino.Descricao,
                        Tipo_Elemento_Destino = TipoDestino.Id,
                        Descricao_Tipo_Elemento_Destino = TipoDestino.Descricao,

                    };
                };
                if (Fibras != null)
                {
                    //List<Elemento_RedeConexoes>
                    var localListagemConexoes = Fibras.Where(p => p.Ponta_A != null && p.Ponta_B != null)
                        .ToList()
                        .OrderBy(p => p.numLoose)
                        .Select((p, i) => getConexaoAsync(p, i).Result).ToList();

                    retorno = localListagemConexoes;
                }
            }
            return retorno;
        }
        public List<TuboLoose_DefinicoesModel> GetFibrasLoose(Elemento_Rede cabo)
        {
            var lista = new List<TuboLoose_DefinicoesModel>();
            var fibras = new List<Fibra_DefinicoesModel>();
            try
            {
                lista = cabo.Fibras.Select(p => p.numLoose).Distinct().Select(p =>
                {
                    return new TuboLoose_DefinicoesModel
                    {
                        Id = p,
                        Cor = getColorLoose(cabo.Padrao_Cores, Convert.ToInt32(p), this._cores).hexadecimal,
                        Fibras = cabo.Fibras.Where(condition => condition.numLoose == p).Select(param =>
                        {
                            return new Fibra_DefinicoesModel
                            {
                                Id = param.Id,
                                Cor = getColorFibra(cabo.Padrao_Cores, Convert.ToInt32(param.numFibra), this._cores).hexadecimal,
                                NumeroFibra = Convert.ToInt32(param.numFibra),
                                NumeroLoose = Convert.ToInt32(param.numLoose),
                            };
                        }).ToList()
                    };
                }).ToList();
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return lista;
        }
        public (String hexadecimal, String text) getColorFibra(Padrao_Cor padrao, int fibra, Dictionary<String, Object> Cores)
        {
            (string, string) colorEnum = (String.Empty, String.Empty);
            switch (padrao)
            {
                case Padrao_Cor.Nacional:
                    colorEnum = (Cores.First(parameter => parameter.Key == Enum.GetName(typeof(Padrao_ABNT), fibra)).Value.ToString(),
                        Enum.GetName(typeof(Padrao_ABNT), fibra));
                    break;

                case Padrao_Cor.Internacional:
                    colorEnum = (Cores.First(parameter => parameter.Key == Enum.GetName(typeof(Padrao_EIA598_A), fibra)).Value.ToString(),
                        Enum.GetName(typeof(Padrao_EIA598_A), fibra));
                    break;
            }
            return colorEnum;
        }
        public (String hexadecimal, String text) getColorLoose(Padrao_Cor padrao, int loose, Dictionary<String, Object> Cores)
        {
            (string, string) colorEnum = (String.Empty, String.Empty);


            switch (padrao)
            {
                case Padrao_Cor.Nacional:
                    if (loose == 1 || loose == 2)
                    {
                        colorEnum = (this._cores.First(parameter => parameter.Key == Enum.GetName(typeof(Padrao_ABNT), loose)).Value.ToString(), Enum.GetName(typeof(Padrao_ABNT), loose));
                    }
                    //*****************************************************************************************
                    //No padrão Nacional aparentemente depois do segundo tubo todos asssumem a coloração branca
                    //*****************************************************************************************

                    else
                    {
                        colorEnum = (this._cores.First(parameter => parameter.Key == Enum.GetName(typeof(Padrao_ABNT), 3)).Value.ToString(), Enum.GetName(typeof(Padrao_ABNT), 3));
                    }
                    break;

                case Padrao_Cor.Internacional:
                    colorEnum = (this._cores.First(parameter => parameter.Key == Enum.GetName(typeof(Padrao_EIA598_A), loose)).Value.ToString(), Enum.GetName(typeof(Padrao_EIA598_A), loose));
                    break;
            }
            return colorEnum;
        }
        public Response<string> ImportaElementosRede(string urlLogin, string descricaoElementoRede, LoginHubsoftModel login, string padraoIntegracaoTipo = null)
        {
            var retorno = new Response<string>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var loginRepo = new Conexao<LoginRadius>(session))
                {
                    var ListaCtos = this._repository.ImportacaoIntegracao(urlLogin, descricaoElementoRede, login);

                    if (ListaCtos.Count() == 0)
                    {
                        throw new Exception("Elemento de rede não encontrado");
                    }

                    foreach (var caixa in ListaCtos)
                    {
                        try
                        {
                            Elemento_Rede elementoAniel = _repository.GetById(caixa.id_caixa_optica);
                            var apiTipo = _repository.GetTipoElementoRedeByCodAuxiliar(Convert.ToString(caixa.id_tipo_caixa_optica));
                            if (apiTipo == null)
                            {
                                if (String.IsNullOrEmpty(padraoIntegracaoTipo))
                                {
                                    throw new Exception($"Nenhum Id de Tipo de Elemento de Rede encontrado no CONSTANTES.");
                                    
                                }

                                apiTipo = _repository.GetTipoElementoRedeById(Convert.ToInt64(padraoIntegracaoTipo));
                                if (apiTipo == null)
                                {
                                    throw new Exception($"Nenhum Id de Tipo de Elemento de Rede encontrado com valor informado no CONSTANTES. CONSTANTES.PadraoIntegracaoTipo: {padraoIntegracaoTipo}");
                                }                                
                            }                            

                            if (elementoAniel == null)
                            {
                                retorno.valor = "cadastro";
                                elementoAniel = ConversionToAniel(caixa, apiTipo);
                                _repository.SaveOrUpdate(elementoAniel);


                                if (caixa.portas == null || caixa.portas.Count <= 0)
                                    throw new Exception($"Nenhuma porta informada pelo hubsoft para a Caixa Id {elementoAniel.Id}");

                                foreach (var portaHubsoft in caixa.portas)
                                {
                                    Elemento_Rede_Porta porta = null;
                                    LoginRadius loginRadius = null;
                                    var clienteServico = portaHubsoft.clientes_servicos.FirstOrDefault();

                                    porta = new Elemento_Rede_Porta
                                    {
                                        Cliente_Plano_Prod = null,
                                        Login = "",
                                        Caixa = elementoAniel,
                                        Porta = portaHubsoft.sequencia + 1,
                                        Status = portaHubsoft.reservado ? Situacao_Porta.Reservado : (clienteServico != null ? Situacao_Porta.Ocupado : Situacao_Porta.Disponivel),
                                        CodigoAuxiliar = Convert.ToString(portaHubsoft.id_porta_atendimento)
                                    };


                                    if (clienteServico?.pivot?.id_cliente_servico != null)
                                    {
                                        loginRadius = loginRepo.selectFirst(p => p.CodigoAuxiliar == Convert.ToString(clienteServico.pivot.id_cliente_servico)).Result;
                                        porta.Login = loginRadius?.Login;
                                        porta.IdLogin = loginRadius?.Id;
                                    }

                                    _repository.SaveOrUpdatePorta(porta);

                                    if (loginRadius != null)
                                    {
                                        if (loginRadius != null)
                                        {
                                            loginRadius.Id_Caixa_FTTH = elementoAniel.Id;
                                            loginRadius.Id_Porta_FTTH = porta.Id;
                                            loginRepo.SaveOrUpdate(loginRadius).Wait();
                                        }
                                    }
                                }

                            }
                            else
                            {
                                retorno.valor = "atualizacao";
                                elementoAniel = ConversionToAniel(caixa, apiTipo, elementoAniel);
                                _repository.SaveOrUpdate(elementoAniel);

                                var listaPortasAniel = _repository.GetAllPortas().Where(p => p.Caixa.Id == elementoAniel.Id);

                                foreach (var portaHubsoft in caixa.portas)
                                {
                                    var clienteServico = portaHubsoft.clientes_servicos.FirstOrDefault();
                                    LoginRadius loginRadius = null;

                                    Elemento_Rede_Porta porta = listaPortasAniel.FirstOrDefault(p => p.Porta == portaHubsoft.sequencia + 1);

                                    if (porta == null)
                                    {
                                        porta = new Elemento_Rede_Porta
                                        {
                                            Id = listaPortasAniel.FirstOrDefault(p => p.Porta == portaHubsoft.sequencia + 1).Id,
                                            Cliente_Plano_Prod = null,
                                            Login = "",
                                            Caixa = elementoAniel,
                                            Porta = portaHubsoft.sequencia + 1,
                                            Status = portaHubsoft.reservado ? Situacao_Porta.Reservado : (clienteServico != null ? Situacao_Porta.Ocupado : Situacao_Porta.Disponivel),
                                            CodigoAuxiliar = Convert.ToString(portaHubsoft.id_porta_atendimento)
                                        };
                                    }
                                    else
                                    {
                                        porta.Cliente_Plano_Prod = null;
                                        porta.Login = "";
                                        porta.Caixa = elementoAniel;
                                        porta.Porta = portaHubsoft.sequencia + 1;
                                        porta.Status = portaHubsoft.reservado ? Situacao_Porta.Reservado : (clienteServico != null ? Situacao_Porta.Ocupado : Situacao_Porta.Disponivel);
                                        porta.CodigoAuxiliar = Convert.ToString(portaHubsoft.id_porta_atendimento);

                                    }


                                    if (clienteServico?.pivot?.id_cliente_servico != null)
                                    {
                                        loginRadius = loginRepo.selectFirst(p => p.CodigoAuxiliar == Convert.ToString(clienteServico.pivot.id_cliente_servico)).Result;
                                        if (loginRadius == null)
                                        {
                                            IntegrarLoginHubsoft(urlLogin, clienteServico?.pivot?.id_cliente_servico, login, loginRadius);
                                            loginRadius = loginRepo.selectFirst(p => p.CodigoAuxiliar == Convert.ToString(clienteServico.pivot.id_cliente_servico)).Result;
                                        }

                                        porta.Login = loginRadius?.Login;
                                        porta.IdLogin = loginRadius?.Id;
                                    }


                                    _repository.SaveOrUpdatePorta(porta);

                                    if (loginRadius != null)
                                    {
                                        if (loginRadius != null)
                                        {
                                            loginRadius.Id_Caixa_FTTH = elementoAniel.Id;
                                            loginRadius.Id_Porta_FTTH = porta.Id;
                                            loginRepo.SaveOrUpdate(loginRadius).Wait();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"CaixaAtendimento - Falha na Execucao. Id Caixa: {caixa?.id_caixa_optica} | Mensagem Erro: {ex.Message}", ex);
                        }
                    }
                    retorno.result = true;
                }
            }
            catch (Exception ex)
            {
                retorno.stacktrace = ex.StackTrace;
                retorno.exception = ex.Message;
                retorno.message = ex.Message;
                retorno.result = false;
            }
            return retorno;
        }

        private Elemento_Rede ConversionToAniel(ImportacaoElementoHubSoft caixa, Tipo_Elemento_Rede tipo, Elemento_Rede element = null)
        {

            if (element == null)
            {
                element = new Elemento_Rede();
            }

            element.Id = caixa.id_caixa_optica;
            element.Descricao = caixa.nome;
            element.UsuarioCriacao = "INTEGRACAO HUBSOFT X ANIEL";
            element.Capacidade = caixa.portas.Count;
            element.Latitude = double.TryParse(caixa?.ponto_juncao?.latitude?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat) ? lat : (double?)null;
            element.Longitude = double.TryParse(caixa?.ponto_juncao?.longitude?.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lng) ? lng : (double?)null;

            //element.Cidade = caixa.cidade;
            //element.UF = caixa.estado ?? "";

            element.Tipo = tipo;
            element.Modelo = tipo.Modelo;

            //short? status = (Int16?)(caixa.status == "ativada" ? 1 : 0);
            var status = Status_Elemento.Ativo;

            element.Situacao = status;
            element.Certificado = Convert.ToBoolean(status);
            return element;
        }

        public Boolean IntegrarLoginHubsoft(string urlLogin, long? idClienteServicoBusca, LoginHubsoftModel login, LoginRadius loginModel)
        {

            var repositoryLogin = new IntegracaoLoginHubSoft(urlLogin);
            var responseConsultaCliente = repositoryLogin.GetLogin(urlLogin, login, Convert.ToString(idClienteServicoBusca.Value));
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var session = HibernateUtil.GetSession())
            using (var clienteRepository = new Conexao<Aniel.Connect.Model.nhibernate.Cliente>(session))
            using (var planoRepository = new Conexao<Plano>(session))
            using (var clienteContratoRepository = new Conexao<Aniel.Connect.Model.nhibernate.ClienteContrato>(session))
            using (var loginRepository = new Conexao<Aniel.Connect.Model.nhibernate.LoginRadius>(session))
            using (var clientePlanoProdutoRepository = new Conexao<Aniel.Connect.Model.nhibernate.Cliente_PlanoProduto>(session))
            using (var clienteAnielRepository = new Conexao<Aniel.T5.Model.nhibernate.ClienteAniel>(sessionT5))
            {

                foreach (var item in responseConsultaCliente)
                {

                    var clienteConnect = clienteRepository.selectFirst(p => p.CpfCnpj == item.cpf_cnpj).Result;
                    if (clienteConnect == null)
                    {
                        var clienteAniel = ConversaoAniel(item);
                        ClienteAniel cliente_consulta = clienteAnielRepository.selectFirst(p => p.CPF == clienteAniel.CPF).Result;

                        if (cliente_consulta != null)
                            clienteAniel.Cod_Cliente = cliente_consulta.Cod_Cliente;

                        clienteAnielRepository.SaveOrUpdate(clienteAniel).Wait();
                        clienteConnect = clienteRepository.selectFirst(p => p.CpfCnpj == item.cpf_cnpj).Result;
                    }


                    foreach (var servico in item.servicos)
                    {
                        loginModel = loginRepository.selectFirst(p => p.CodigoAuxiliar == Convert.ToString(servico.id_cliente_servico)).Result;
                        var idCliente = Convert.ToString(clienteConnect?.Id);
                        //#region Cliente Contrato
                        var contratoCliente = new ClienteContrato();
                        var proxNumero = session.Query<ClienteContrato>()
                            .Select(c => c.NumeroContrato)
                            .ToList()
                            .Select(num => int.TryParse(num, out var n) ? n : 0)
                            .DefaultIfEmpty(0)
                            .Max() + 1;


                        contratoCliente.Descricao = Convert.ToString(proxNumero);
                        contratoCliente.NumeroContrato = Convert.ToString(proxNumero);
                        contratoCliente.Cliente = clienteConnect;
                        clienteContratoRepository.SaveOrUpdate(contratoCliente).Wait();
                        Plano plano = planoRepository.selectFirst(p => p.CodigoAuxiliar == servico.nome).Result;

                        if (plano == null)
                        {
                            plano = new Plano();
                            plano.Descricao = servico.nome;
                            plano.StatusInicial = Status_PlanoProduto.Habilitado;

                        }

                        plano.SituacaoPlano = Situacao_plano.Ativo;
                        plano.Tipo_Tecnologia = Tipo_Tecnologia.Internet;

                        planoRepository.SaveOrUpdate(plano).Wait();

                        loginModel = this.conversionLogin(servico, clienteConnect, loginModel);

                        var clientePlanoProduto = new Cliente_PlanoProduto();
                        clientePlanoProduto.Cliente = clienteConnect;
                        clientePlanoProduto.ClienteContrato = contratoCliente;
                        clientePlanoProduto.Plano = plano;
                        clientePlanoProduto.Login_Radius = loginModel.Login;
                        clientePlanoProduto.Senha_Radius = loginModel.SenhaPPPOE;

                        clientePlanoProdutoRepository.SaveOrUpdate(clientePlanoProduto).Wait();

                        loginModel.Cliente_PlanoProduto = clientePlanoProduto;
                        loginModel.CodigoAuxiliar = Convert.ToString(loginModel.Id);
                        loginRepository.SaveOrUpdate(loginModel).Wait();
                    }

                    if (idClienteServicoBusca.HasValue && item.servicos.Count == 0)
                    {
                        loginModel = loginRepository.selectFirst(p => p.CodigoAuxiliar == Convert.ToString(idClienteServicoBusca)).Result;
                        var idCliente = Convert.ToString(clienteConnect?.Id);
                        Repository.Model.Hubsoft.Servico servico = new Repository.Model.Hubsoft.Servico
                        {
                            id_cliente_servico = (int)idClienteServicoBusca,
                            login = "INATIVO",
                            status = "N",
                            nome = "INATIVO"
                        };

                        loginModel = this.conversionLogin(servico, clienteConnect, loginModel);
                        loginModel.CodigoAuxiliar = Convert.ToString(loginModel.Id);
                        loginRepository.SaveOrUpdate(loginModel).Wait();
                    }
                }
            }

            return true;
        }

        public ClienteAniel ConversaoAniel(ImportacaoClienteHubSoft cliente)
        {
            ClienteAniel clienteAniel = new ClienteAniel();
            clienteAniel.CPF = cliente.cpf_cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            clienteAniel.NomeFantasia = cliente.nome_razaosocial;
            clienteAniel.Tipo = "1";
            clienteAniel.Status = Status_Cliente.Ativo;


            clienteAniel.Email_Principal = cliente.email_principal;
            clienteAniel.Telefone_Fixo = Convert.ToString(cliente.telefone_primario);
            clienteAniel.Telefone_Celular = Convert.ToString(cliente.telefone_secundario);



            var endereco = cliente.servicos.FirstOrDefault();

            if (endereco != null)
            {
                clienteAniel.Logradouro = endereco.endereco_instalacao?.endereco;
                clienteAniel.Numero = endereco.endereco_instalacao?.numero;
                clienteAniel.Bairro = endereco.endereco_instalacao?.bairro;
                clienteAniel.Cep = endereco.endereco_instalacao?.cep;
                clienteAniel.Numero_Contrato = Convert.ToString(cliente.id_cliente);
            }

            return clienteAniel;
        }
        private LoginRadius conversionLogin(Repository.Model.Hubsoft.Servico item, Model.nhibernate.Cliente cliente, LoginRadius login = null)
        {
            if (login == null)
                login = new LoginRadius();

            login.Login = item.login;
            login.Id = Convert.ToInt64(item.id_cliente_servico);
            login.SenhaPPPOE = item.senha;
            login.MAC = item.mac_addr;
            login.Cliente = cliente;
            login.Status = item.status == "S" ? Status_Default.Ativo : Status_Default.Inativo;
            login.EnderecoIPV6 = item.ipv6;
            login.EnderecoIPV4 = item.ipv4;
            return login;
        }


    }
}
