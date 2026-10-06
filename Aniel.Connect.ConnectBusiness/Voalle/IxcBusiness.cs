using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using Aniel.Connect.Repository.Model.Mk;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Voalle;
using Aniel.T5.Model.nhibernate;
using NHibernate.Engine;
using Sinapse.DAO;
using Sinapse.Integracao.IntegracoesERP.Ixc;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Aniel.Connect.ConnectBusiness.MK
{
    public class IxcBusiness
    {
        private readonly IIxcRepository _repoIxc;
        private readonly IxcHelper _ixcHelper;
        private readonly ITurnoRepository _repoTurno;
        private readonly IClienteAnielRepository _repoClienteAniel;
        private readonly Config_Aniel_Integrator config;

        public IxcBusiness(IIxcRepository repoIxc, ITurnoRepository repoTurno,IClienteAnielRepository repoClienteAniel, Config_Aniel_Integrator config,IxcHelper ixcHelper)
        {
            this._repoIxc = repoIxc;
            this._repoTurno = repoTurno;
            this._repoClienteAniel = repoClienteAniel;
            this._ixcHelper = ixcHelper;
            this.config = config;
        }

        /// <summary>
        /// Retorna uma Ordem de Serviço do Banco de Dados Voalle 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="numOs"></param>
        /// <returns></returns>
        public OrdemDeServicoResponse ObterOrdemServico(string numOs)
        {
            return this._ixcHelper.GetOs(numOs);
        }

        public void ImportarOs(OrdemDeServicoResponse item)
        {
            try
            {
                Equipe equipe = GetEquipe(item.id_tecnico);
                if (item.id_tecnico != "0" || String.IsNullOrEmpty(item.id_tecnico))
                {
                    if (equipe == null)
                        throw new Exception("Equipe não cadastrada com id_tecnico informado.");
                }

                Ordem_Servico osAniel = new Ordem_Servico();

                osAniel.Equipe = equipe?.Matricula;
                osAniel.EquipeAux = equipe?.CodAux;
                osAniel.CodCt = equipe != null ? (String.IsNullOrEmpty(equipe.CodCtPadrao) ? config.CodCtPadrao : equipe.CodCtPadrao) : config.CodCtPadrao;
                osAniel.Projeto = equipe != null ? (String.IsNullOrEmpty(equipe.ProjetoPadrao) ? config.ProjetoPadrao : equipe.ProjetoPadrao) : config.ProjetoPadrao;
                osAniel.OS = item.protocolo;
                osAniel.IdOriginalIxc = item.id;

                osAniel.Contrato = GetNumDoc(item.id_cliente);
                osAniel.Contrato_Kit = item.id_contrato_kit;


                osAniel.DataAgendamento = (item.status == "A" || item.data_agenda == "0000-00-00 00:00:00") ? null : GetDataHoraMaxima(item.data_agenda, item.protocolo);
                osAniel.HoraAgendamento = osAniel.DataAgendamento;


                Ordem_Servico retornoOrdemAniel = _repoIxc.GetOrdem_Servico(
                    osAniel.CodCt, osAniel.Projeto, osAniel.OS, osAniel.Contrato.GetValueOrDefault(0), DateTime.MinValue);

                if (null != retornoOrdemAniel)
                {
                    throw new Exception("Ordem de Serviço Já cadastrada.");
                }

                osAniel.Cod_Turno = GetCodTurno(item.melhor_horario_agenda, item.protocolo);
                osAniel.Cod_TipoServ_Integrator = item.id_assunto;
                osAniel.Cod_Tipo_Serv = Convert.ToInt16(GetTipoServ(item.id_assunto, osAniel.CodCt).Codigo);

                if (osAniel.Cod_Tipo_Serv.HasValue)
                {
                    osAniel.Cod_SubTipo_Serv = GetSubTipoServ(item.id_assunto, osAniel.Cod_Tipo_Serv).CodigoSubTipo;
                }

                osAniel.Unidade_Custo = _repoIxc.GetUnidadeCusto(osAniel.CodCt) ?? config.Unidade_Custo; //LEMBRAR DE PEGAR DA TB_CONTRATO 
                osAniel.DataAbertura = GetDataHoraAbertura(item.data_abertura);
                osAniel.HoraAbertura = osAniel.DataAbertura;
                osAniel.Bairro = item.bairro;
                osAniel.Usuario = config.UsuarioAniel;

                ResponseIXC<Cliente_ContratoResponse> responseCliCt = _ixcHelper.GetClienteContrato(item.id_contrato_kit);
                var contrato = responseCliCt.registros != null ? responseCliCt.registros.FirstOrDefault() : null;

                double latitudeDouble;
                double longitudeDouble;
                if (Double.TryParse(item.latitude, out latitudeDouble))// && CONSTANTES.ImportarLatitudeLongitude == "S"
                {
                    osAniel.Latitude = item.latitude == String.Empty ? null : item.latitude;
                }
                if (Double.TryParse(item.longitude, out longitudeDouble)) // && CONSTANTES.ImportarLatitudeLongitude == "S"
                {
                    osAniel.Longitude = item.longitude == String.Empty ? null : item.longitude;
                }



                osAniel.Oxbdpee = item.id_cliente;
                InfoCliente infoCliente = GetCliente(item.id_cliente, config);
                osAniel.Id_Cliente = infoCliente.cliente.Codigo.Value;
                osAniel.Cliente = infoCliente.cliente.Razao_Social;
                osAniel.Titular = infoCliente.cliente.Razao_Social;
                osAniel.CpfCliente = infoCliente.cliente.CNPJ;

                osAniel.Rg = infoCliente.rgCliente;
                osAniel.Email_Assinante = infoCliente.cliente.Email;
                osAniel.Telefone_Comercial = infoCliente.cliente.Telefone_Comercial;
                osAniel.Tarefa = infoCliente.cliente.Telefone_Residencial;
                osAniel.Telefone = infoCliente.cliente.Telefone;


                _repoIxc.UpdateOrInsertClienteConnect(infoCliente.cliente);


                if (contrato != null)
                {
                    var cidades = _ixcHelper.GetCidade(contrato.cidade)?.registros;
                    long? IdEndereco = _repoIxc.UpdateOrInsertEndereco(infoCliente, contrato,cidades);
                    _repoIxc.UpdateOrInsertContrato(infoCliente, contrato, IdEndereco);
                }

                _repoIxc.AtualizaGeneratorClienteConnect();

                if (contrato != null)
                {
                    osAniel.Id_Cliente_Contrato = _repoIxc.GetIdContrato(contrato.id, infoCliente.cliente.CNPJ);
                }

                if (String.IsNullOrEmpty(osAniel.Latitude))
                {
                    var dadosEndereco = _repoIxc.GetLocalidadeEnderecoCliente(item.id_contrato_kit, infoCliente.cliente.CNPJ);
                    osAniel.Latitude = Convert.ToString(dadosEndereco?.Latitude);
                    osAniel.Longitude = Convert.ToString(dadosEndereco?.Longitude);
                }

                VdContratoResponse plano = GetDadosPlano(item.id_contrato_kit);
                osAniel.CodPlano = plano != null ? plano.id : "";
                osAniel.DescPlano = plano != null ? plano.nome : "";

                LoginResponse login = GetLogin(item.id_login);

                if (login != null)
                {
                    osAniel.SenhaRadius = login.senha;
                    osAniel.LoginRadius = login.login;
                    osAniel.Id_Login = Convert.ToInt64(login.id);

                    osAniel.Id_Caixa = String.IsNullOrEmpty(login.id_caixa_ftth) ? (long?)null : Convert.ToInt64(login.id_caixa_ftth);
                    osAniel.Id_Porta = String.IsNullOrEmpty(login.ftth_porta) ? (long?)null : Convert.ToInt64(login.ftth_porta); ;

                }

                CaixaFtthResponse caixaFtth = GetCaixaFtth(login.id_caixa_ftth);

                osAniel.Observacao = $"ip: {login.ip}\nmac: {login.mac ?? ""}\ncaixa ftth:{caixaFtth?.descricao ?? ""}\nmensagem:{item.mensagem}";

                if (item.origem_endereco == "C")
                {
                    osAniel.Cidade = infoCliente.cliente.Municipio;
                    osAniel.Bairro = infoCliente.cliente.Bairro;
                    osAniel.Cep = infoCliente.cliente.CEP;
                    osAniel.Numero = infoCliente.cliente.Numero;

                    osAniel.Complemento = infoCliente.cliente.Complemento;
                    osAniel.Uf = infoCliente.cliente.UF;
                    osAniel.Endereco = infoCliente.cliente.Endereco;
                    osAniel.Referencia = infoCliente.cliente.Referencia;
                }
                else if (item.origem_endereco == "CC")
                {
                    if (responseCliCt.registros == null)
                        throw new Exception("id_cliente não cadastrado no IXC.");
                    else
                    {
                        Cliente_ContratoResponse cliente_contrato = responseCliCt.registros?.First();
                        if (cliente_contrato.endereco_padrao_cliente == "S")
                        {
                            osAniel.Cidade = infoCliente.cliente.Municipio;
                            osAniel.Bairro = infoCliente.cliente.Bairro;
                            osAniel.Cep = infoCliente.cliente.CEP;
                            osAniel.Numero = infoCliente.cliente.Numero;

                            osAniel.Complemento = infoCliente.cliente.Complemento;
                            osAniel.Uf = infoCliente.cliente.UF;
                            osAniel.Endereco = infoCliente.cliente.Endereco;
                            osAniel.Referencia = infoCliente.cliente.Referencia;
                        }
                        else
                        {
                            osAniel.Cep = cliente_contrato.cep;
                            osAniel.Bairro = cliente_contrato.bairro;
                            osAniel.Numero = cliente_contrato.numero;

                            osAniel.Referencia = cliente_contrato.referencia;
                            osAniel.Complemento = $"{cliente_contrato.complemento} Bloco: {cliente_contrato.bloco} Apartamento: {cliente_contrato.apartamento}";
                            osAniel.Endereco = cliente_contrato.endereco;
                            if (cliente_contrato.cidade != String.Empty)
                            {
                                List<CidadeResponse> responseCidade = _ixcHelper.GetCidade(cliente_contrato.cidade)?.registros;
                                osAniel.Cidade = responseCidade != null ? responseCidade.First().nome : "";
                            }
                        }
                    }

                }
                else if (item.origem_endereco == "L")
                {
                    ResponseIXC<LoginResponse> responseLogin = _ixcHelper.GetLogin(item.id_login);
                    if (responseLogin.registros == null)
                        throw new Exception("id_cliente não cadastrado no IXC.");
                    else
                    {
                        LoginResponse loginCli = responseLogin.registros?.FirstOrDefault();

                        if (login?.endereco_padrao_cliente == "S")
                        {
                            osAniel.Cidade = infoCliente.cliente.Municipio;
                            osAniel.Bairro = infoCliente.cliente.Bairro;
                            osAniel.Cep = infoCliente.cliente.CEP;
                            osAniel.Numero = infoCliente.cliente.Numero;

                            osAniel.Complemento = infoCliente.cliente.Complemento;
                            osAniel.Uf = infoCliente.cliente.UF;
                            osAniel.Endereco = infoCliente.cliente.Endereco;
                            osAniel.Referencia = infoCliente.cliente.Referencia;
                        }
                        else
                        {
                            osAniel.Cep = loginCli?.cep;
                            osAniel.Bairro = loginCli?.bairro;
                            osAniel.Endereco = loginCli?.endereco;

                            osAniel.Numero = loginCli?.numero;
                            osAniel.Referencia = loginCli?.referencia;
                            osAniel.Complemento = $"{loginCli?.complemento} Bloco: {loginCli?.bloco} Apartamento: {loginCli?.apartamento}";
                            if (loginCli.cidade != String.Empty)
                            {
                                List<CidadeResponse> responseCidade = _ixcHelper.GetCidade(loginCli?.cidade)?.registros;
                                osAniel.Cidade = responseCidade != null ? responseCidade.FirstOrDefault()?.nome : String.Empty;
                                //clienteAniel.CodMunicipio = Convert.ToInt16(responseCidade != null ? responseCidade.First().cod_ibge : "0");
                            }
                        }
                    }
                }
                else if (item.origem_endereco == "M")
                {
                    if (item.id_cidade != String.Empty)
                    {
                        List<CidadeResponse> responseCidade = _ixcHelper.GetCidade(item.id_cidade)?.registros;
                        osAniel.Cidade = responseCidade != null ? responseCidade.First().nome : "";

                        if (responseCidade.Count() > 0 && responseCidade.First().uf != String.Empty)
                        {
                            List<UFResponse> responseUf = _ixcHelper.GetUF(responseCidade.First().uf)?.registros;
                            osAniel.Uf = responseUf != null ? responseUf.First().sigla : "";
                        }
                    }

                    osAniel.Bairro = item.bairro;
                    osAniel.Complemento = item.complemento;
                    osAniel.Endereco = item.endereco;
                    osAniel.Referencia = item.referencia;
                }

                GetEscritorioAreaLocalidade(osAniel);
                _repoIxc.InsertOrdemServico(osAniel);
                _repoIxc.InsertAgendamento(osAniel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private LoginResponse GetLogin(string id_login)
        {
            if (id_login == string.Empty || id_login == "0")
                return new LoginResponse(); //throw new Exception("id_login não informado na OS.");

            ResponseIXC<LoginResponse> response = _ixcHelper.GetLogin(id_login);
            if (response.registros == null)
                throw new Exception("id_login não cadastrado no IXC.");

            return response.registros?.First();
        }

        private CaixaFtthResponse GetCaixaFtth(string id_caixaFtth)
        {
            if (String.IsNullOrEmpty(id_caixaFtth) || id_caixaFtth == "0")
                return new CaixaFtthResponse(); //throw new Exception("id_caixaFtth não informado na OS.");

            ResponseIXC<CaixaFtthResponse> response = _ixcHelper.GetCaixaFtth(id_caixaFtth);
            if (response.registros == null)
                throw new Exception("id_caixaFtth não cadastrado no IXC.");

            return response.registros?.First();
        }

        private VdContratoResponse GetDadosPlano(string id_contrato)
        {
            //if (id_contrato == string.Empty)
            //    throw new Exception("id_contrato não informado na OS.");

            ResponseIXC<Cliente_ContratoResponse> responseCliCt = _ixcHelper.GetClienteContrato(id_contrato);
            //if (responseCliCt.registros == null)
            //    throw new Exception("id_cliente não cadastrado no IXC.");

            if (responseCliCt.registros != null)
            {
                Cliente_ContratoResponse cliente_contrato = responseCliCt.registros?.FirstOrDefault();
                ResponseIXC<VdContratoResponse> responseVdCt = _ixcHelper.GetVdContrato(cliente_contrato?.id_vd_contrato);
                return responseVdCt.registros != null ? responseVdCt.registros?.First() : null;
            }
            else
            {
                return null;
            }
        }
        private Tipo_Servico GetTipoServ(string id_assunto, string codct)
        {
            if (id_assunto == string.Empty)
                throw new Exception("id_assunto não informado na OS.");

            ResponseIXC<AssuntoResponse> response = _ixcHelper.GetAssunto(id_assunto);
            if (response.registros == null)
                throw new Exception("id_assunto não cadastrado no IXC.");
            AssuntoResponse assunto = response.registros?.First();

            Tipo_Servico retorno = _repoIxc.GetTipoServ(assunto.assunto, codct);

            return retorno ?? _repoIxc.InsertTipoServ(assunto.assunto, assunto.id, codct);
        }

        private SubTipo_Servico GetSubTipoServ(string id_assunto, Int32? cod_tipo_serv)
        {
            if (id_assunto == string.Empty)
                throw new Exception("id_assunto não informado na OS.");

            ResponseIXC<AssuntoResponse> response = _ixcHelper.GetAssunto(id_assunto);
            if (response.registros == null)
                throw new Exception("id_assunto não cadastrado no IXC.");
            AssuntoResponse assunto = response.registros?.First();

            SubTipo_Servico retorno = _repoIxc.GetSubTipoServ(assunto.assunto, cod_tipo_serv);

            return retorno ?? _repoIxc.InsertSubTipoServ(assunto.assunto, cod_tipo_serv);
        }

        private List<OrdemDeServicoResponse> FiltrarListaOs(List<OrdemDeServicoResponse> ListaOs, Config_Aniel_Integrator config)
        {
            if (ListaOs.Count > 0)
            {
                List<String> setores = String.IsNullOrEmpty(config.Setores) ? new List<string>() : config.Setores.Split(',').ToList();
                if (setores.Count > 0)
                    ListaOs = ListaOs.FindAll(p => setores.Contains(p.setor));
            }

            return ListaOs;
        }

        private Equipe GetEquipe(string id_tecnico)
        {
            if (id_tecnico == null)
                throw new Exception("id_equipe não informado na OS.");
            if (id_tecnico.Length > 10)
                throw new Exception("id_equipe maior que 10 caracteres.");

            return _repoIxc.GetByMatAux(id_tecnico);
        }

        private int GetNumDoc(string id)
        {
            try
            {
                return Convert.ToInt32(id);
            }
            catch (Exception)
            {
                throw new Exception("id informado com valor numérico inválido.");
            }
        }

        private int? GetCodTurno(string melhor_horario_agenda, string protocolo)
        {
            int? retorno = null;
            string descricao = String.Empty;

            switch (melhor_horario_agenda)
            {
                case "M":
                    descricao = "MANHA";
                    break;
                case "T":
                    descricao = "TARDE";
                    break;
                case "N":
                    descricao = "NOITE";
                    break;
                case "Q":
                    descricao = "QUALQUER";
                    break;

            }

            if (String.IsNullOrEmpty(descricao) == false)
            {
                retorno = _repoTurno.GetAll().Where(p => p.Descricao == descricao).FirstOrDefault()?.Id;
                if (retorno.HasValue == false)
                    retorno = _repoTurno.SaveOrUpdate(new Turno_ServicoAniel
                    {
                        Descricao = descricao,
                        Status = Sinapse.Model.enumerados.Status_Default.Ativo
                    }).Id;
            }


            return retorno;
        }

        private DateTime? GetDataHoraMaxima(string data, string protocolo)
        {
            return Convert.ToDateTime(data);
        }

        private DateTime GetDataHoraAbertura(string data)
        {
            try
            {
                return Convert.ToDateTime(data);
            }
            catch (Exception)
            {
                throw new Exception("data_abertura informada inválido.");
            }
        }
        public void GetEscritorioAreaLocalidade(Ordem_Servico ordemServ)
        {
            ProjetoIxc projeto = _repoIxc.GetByContratoProjeto(ordemServ.CodCt, ordemServ.Projeto);
            ordemServ.Cod_Escritorio = projeto?.CodEscritorio;
            if (!String.IsNullOrEmpty(ordemServ.Cidade) && (!String.IsNullOrEmpty(ordemServ.Bairro)))
            {
                LocalidadeIxc localidade = _repoIxc.GetByLocalidade(StringService.Truncate(ordemServ.Cidade, 35), StringService.Truncate(ordemServ.Bairro, 50), projeto?.CodEscritorio);

                if (localidade != null)
                {
                    ordemServ.Cod_Escritorio = localidade.Cod_Escritorio;
                    ordemServ.Cod_Area = localidade.Cod_Area;
                    ordemServ.Cod_Localidade = localidade.Cod_Localidade;

                }
                else
                {
                    var localidadeContrato = _repoIxc.GetLocalidade(ordemServ.Contrato_Kit, ordemServ.CpfCliente);
                    if (localidadeContrato != null && localidadeContrato.CodLocalidade != null)
                    {
                        ordemServ.Cod_Escritorio = localidadeContrato?.CodEscritorio;
                        ordemServ.Cod_Area = localidadeContrato?.CodArea;
                        ordemServ.Cod_Localidade = localidadeContrato?.CodLocalidade;
                    }
                    else
                    {
                        AreaIxc area = _repoIxc.GetByCidade(StringService.Truncate(ordemServ.Cidade, 35), projeto?.CodEscritorio);
                        if (area != null)
                        {
                            ordemServ.Cod_Escritorio = area?.Cod_Escritorio;
                            ordemServ.Cod_Area = area?.Cod_Area;
                        }
                        else
                        {
                            ordemServ.Cod_Escritorio = projeto?.CodEscritorio;
                        }
                    }
                }
            }
            else
            {
                ordemServ.Cod_Escritorio = projeto?.CodEscritorio;
            }
        }

        private InfoCliente GetCliente(string id_cliente, Config_Aniel_Integrator config)
        {
            if (id_cliente == string.Empty)
                throw new Exception("id_cliente não informado na OS.");

            ResponseIXC<ClienteResponse> response = _ixcHelper.GetCliente(id_cliente);
            if (response.registros == null)
                throw new Exception("id_cliente não cadastrado no IXC.");
            ClienteResponse clienteIXC = response.registros?.First();

            ClienteIxc clienteAniel = new ClienteIxc();
            clienteAniel.CNPJ = clienteIXC.cnpj_cpf.Replace(".", "").Replace("/", "").Replace("-", "");
            clienteAniel.Razao_Social = clienteIXC.razao;
            clienteAniel.TipoCliente = clienteIXC.tipo_pessoa;
            clienteAniel.Situacao = Convert.ToInt16(clienteIXC.ativo == "S" ? 0 : 1);

            clienteAniel.Email = clienteIXC.email;
            clienteAniel.Endereco = clienteIXC.endereco;
            clienteAniel.Numero = clienteIXC.numero;
            clienteAniel.Complemento = $"{clienteIXC.complemento} | Bloco: {clienteIXC.bloco} | Apartamento: {clienteIXC.apartamento}";

            clienteAniel.Bairro = clienteIXC.bairro;
            clienteAniel.CEP = clienteIXC.cep;
            clienteAniel.Endereco = clienteIXC.endereco;

            clienteAniel.Telefone = clienteIXC.telefone_celular;
            clienteAniel.Telefone_Comercial = clienteIXC.telefone_comercial;
            clienteAniel.Telefone_Residencial = clienteIXC.fone;

            clienteAniel.Referencia = clienteIXC.referencia;
            clienteAniel.Latitude = clienteIXC.latitude;
            clienteAniel.Longitude = clienteIXC.longitude;

            if (clienteIXC.cidade != String.Empty)
            {
                List<CidadeResponse> responseCidade = _ixcHelper.GetCidade(clienteIXC.cidade)?.registros;
                clienteAniel.Municipio = responseCidade != null ? responseCidade.First().nome : "";

                if (responseCidade.Count() > 0 && responseCidade.First().uf != String.Empty)
                {
                    List<UFResponse> responseUf = _ixcHelper.GetUF(responseCidade.First().uf)?.registros;
                    clienteAniel.UF = responseUf != null ? responseUf.First().sigla : "";
                }

                //clienteAniel.CodMunicipio = Convert.ToInt16(responseCidade != null ? responseCidade.First().cod_ibge : "0");
            }



            var cliente_consulta = _repoClienteAniel.GetAll().Where(p => p.CPF == clienteAniel.CNPJ).FirstOrDefault();

            if (cliente_consulta != null)
                clienteAniel.Codigo = cliente_consulta.Cod_Cliente;

            if (cliente_consulta == null)
            {
                _repoIxc.InsertCliente(clienteAniel);
                return new InfoCliente()
                {
                    rgCliente = clienteIXC.ie_identidade,
                    cliente = clienteAniel//dao.Get(clienteAniel.CNPJ)
                };
            }
            else
            {
                _repoIxc.UpdateDadosCliente(clienteAniel);
                return new InfoCliente()
                {
                    rgCliente = clienteIXC.ie_identidade,
                    cliente = clienteAniel
                };
            }
        }
    }
}
