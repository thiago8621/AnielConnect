using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;

namespace Aniel.Connect.Model.nhibernate
{
    public class Venda : BaseModel
    {
        public virtual string IdLoginIntegracao { get; set; }

        //private IEnumerable<Cliente_PlanoProduto> _planosprodutos;

        //public virtual Atividade_StatusVenda Status { get; set; }
        public virtual Status_Venda Status { get; set; }        
        public virtual String StatusVenda { get; set; }

        //public virtual String Quem_Indicou { get; set; }
        public virtual DateTime? Data_Encerramento { get; set; }

        public virtual DateTime? Data_Venda { get; set; }
        public virtual DateTime? Data_Alteracao_Status { get; set; }

        public virtual DateTime? Data_Ultimo_Contato { get; set; }

        public virtual String Justificativa_Avaliacao_Comercial { get; set; }

        public virtual Int64? Id_Cliente { get; set; }

        public virtual Endereco_Cliente EnderecoInstalacao { get; set; }

        public virtual Funcionario Vendedor { get; set; }
        public virtual Funcionario Supervisor { get; set; }
        public virtual EtapaVenda Etapa_Atual { get; set; }
        public virtual Campanha  Campanha { get; set; }
        public virtual Motivo_Desistencia_Venda Motivo_Desistencia_Venda { get; set; }

        public virtual Int64? Id_Meta { get; set; }
        public virtual Int64? Id_Faixa { get; set; }
        public virtual Int64? Id_Comissionamento { get; set; }
 
        public virtual Cliente_FormaEnvio? Forma_Envio { get; set; }
         public virtual Perfil_Vendedor PerfilVendedor { get; set; }
         public virtual Forma_Pagamento_Cliente FormaPagamentoCliente { get; set; }

        public virtual Int64? Id_Forma_Pagamento_Cliente_Preferida { get; set; } //Usado só para o Premier

        public virtual Boolean Termo_Adesao_Assinado { get; set; }
        public virtual DateTime? DataHoraAssinaturaTermo { get; set; }
        public virtual Boolean Consulta_Viabilidade_Realizada { get; set; }
        public virtual Boolean Consulta_Credito_Realizada { get; set; }
        public virtual Boolean Consulta_Endereco_Realizada { get; set; }
        public virtual Boolean Consulta_Financeiro_Realizada { get; set; }

        public virtual Int64? IdAtendimento { get; set; }
        public virtual Int64? IdFormaPagamento { get; set; }
        public virtual StatusAuditoria? StatusAuditoria { get; set; }
        public virtual Workflow_Vendas Workflow_Vendas { get; set; }
        public virtual Workflow_VendasAtividade Workflow_VendasAtividade { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Departamento Id_Setor_Departamento { get; set; }
        public virtual IdentityUser Id_Responsavel { get; set; }
        public virtual string IdResponsavelString
        {
            get => Id_Responsavel != null ?  $"{Id_Responsavel?.Id} - {Id_Responsavel?.UserName}" : null;
        }


        public virtual Int64? ScoreConsultaCredito { get; set; }
        public virtual Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum? Tipo_Integracao_Assinatura { get; set; }
        public virtual string Token_Documento_Assinatura { get; set; }
        public virtual string Url_Assinar_Documento { get; set; }
        public virtual string Url_Documento_Assinado { get; set; }
        public virtual bool End_Instalacao_Igual_Principal { get; set; }
        public virtual bool End_Instalacao_Igual_Prospecto { get; set; }
        public virtual bool End_CadastroCondominioPrincipal { get; set; }
        public virtual bool End_CadastroCondominioInstalacao { get; set; }
        public virtual bool End_CadastroCondominioCobranca { get; set; }

        public virtual Status_Venda Status_Liberacao { get; set; }
        public virtual AprovacaoCriterioParametroCredito StatusCredito { get; set; }

        // IDs das consultas de restrição que foram aplicadas na venda
        public virtual long? IdConsultaViabilidade { get; set; }
        public virtual long? IdConsultaPendenciaEndereco { get; set; }
        public virtual long? IdConsultaPendenciaFinanceira { get; set; }
        public virtual long? IdConsultaCredito { get; set; }

        public virtual string MotivoAprovManualViabilidade { get; set; }
        public virtual string MotivoAprovManualESC { get; set; }

        public virtual string InformacoesComplementares { get; set; }
        public virtual string IdAtendimentoIntegracao { get; set; }
        public virtual long? EtapaIntegracao { get; set; } 
        [NotMapped]
        public virtual List<Forma_Pagamento_ClienteModel> Formas_Pagamento { get; set; }
        [NotMapped]
        public virtual bool FlagVendaFormatoAvulso { get; set; }
        [NotMapped]
        public virtual string Plano_Combo { get; set; }
        [NotMapped]
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }

        #region AceiteDigitalTermo
        public virtual String IpOrigemAceite { get; set; }
        public virtual String UserAgentTermoAceite { get; set; }
        public virtual String CodigoHashTermoAceite { get; set; }
        public virtual Boolean AdicionadoAceiteDigital { get; set; }
        public virtual String PortaOrigemAceite { get; set; }
        public virtual String UrlAceiteDigital { get; set; }
        #endregion

        private IList<AnexoContratoAceiteModel> _AnexosContrato { get; set; }
        public virtual IList<AnexoContratoAceiteModel> AnexosContrato 
        { 
            get 
            { 
                if (_AnexosContrato == null) 
                { 
                    _AnexosContrato = new List<AnexoContratoAceiteModel>();
                } 
                return _AnexosContrato; 
            } 
            set { _AnexosContrato = value; }
        }

        #region Mobile
        public virtual Boolean Consulta_Viabilidade_Aprovada { get; set; }
        public virtual Boolean Consulta_Viabilidade_AprovadaRessalva { get; set; }
        public virtual Boolean Consulta_Credito_Aprovada { get; set; }
        public virtual Boolean Consulta_Endereco_Aprovada { get; set; }
        public virtual Boolean Consulta_Financeiro_Aprovada { get; set; }
        public virtual Boolean CpfSemTitular { get; set; }

        public virtual string Origem_Venda { get; set; }
        public virtual bool Mudanca_Plano { get; set; }
        public virtual TipoAlteracaoPlanoContratoEnum? Tipo_Alteracao { get; set; }
        public virtual Sinapse.Model.enumerados.Padrao Porta_Reservada { get; set; }
        #endregion
        public virtual List<LogVendas> GeraLog(Venda pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogVendas> logs = new List<LogVendas>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                if (pTipoOperLog == TipoOperLog.Insercao)
                {
                    if (!String.IsNullOrEmpty(this.Plano_Combo))
                    {
                        var log = new LogVendas
                        {
                            IdLog = this.Id,
                            UsuarioCriacao = pUsuario,
                            DataHoraCriacao = DateTime.Now,
                            Chave = Convert.ToString(this.Id),
                            Campo = this.FlagVendaFormatoAvulso ? "Inclusão - Plano Avulso" : "Inclusão - Combo",
                            NomeUsuarioCriacao = pNomeUsuario,
                            TipoOperacao = pTipoOperLog,
                            Campo_ValorAntes = $"",
                            Campo_ValorDepois = $"{this.Plano_Combo}"
                        };
                        logs.Add(log);
                    }
                    else
                    {
                Log_InsereTipoOper<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
                }

                if (pTipoOperLog == TipoOperLog.Exclusao && !String.IsNullOrEmpty(this.Plano_Combo))
                {
                    var log = new LogVendas
                    {
                        IdLog = this.Id,
                        UsuarioCriacao = pUsuario,
                        DataHoraCriacao = DateTime.Now,
                        Chave = Convert.ToString(this.Id),
                        Campo = this.FlagVendaFormatoAvulso ? "Exclusão - Plano Avulso" : "Exclusão - Combo",
                        NomeUsuarioCriacao = pNomeUsuario,
                        TipoOperacao = pTipoOperLog,
                        Campo_ValorAntes = $"{this.Plano_Combo}",
                        Campo_ValorDepois = $"",
                        Id_Cliente_Plano_Produto = this.Id_Cliente_Plano_Produto
                    };
                    logs.Add(log);
                }

                if (this.Formas_Pagamento != null)
                {
                    foreach (var forma_pagamento in this.Formas_Pagamento)
                    {
                        var log = new LogVendas
                        {
                            IdLog = this.Id,
                            UsuarioCriacao = pUsuario,
                            DataHoraCriacao = DateTime.Now,
                            Chave = Convert.ToString(this.Id),
                            Campo = "Exclusão - Forma de Pagamento",
                            NomeUsuarioCriacao = pNomeUsuario
                        };

                        if (pTipoOperLog == TipoOperLog.Exclusao)
                        {
                            log.TipoOperacao = TipoOperLog.Exclusao;
                            log.Campo_ValorAntes = $"{forma_pagamento.Id_Forma_Pagamento}-{forma_pagamento.Desc_Forma_Pagamento}";
                            log.Campo_ValorDepois = "";
                            log.Id_Forma_Pagamento_Cliente = forma_pagamento.Id;
                        }
            else
            {
                            continue; // se não for inserção nem exclusão, pula
                        }

                        logs.Add(log);
                    }
                }
            }
            else
            {
                if (this.Workflow_Vendas != null)
                {
                    Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_VENDA, pOld, this, p => p.StatusVenda);
                }
                else
                {
                    Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                }

                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.JUSTIFICATIVA, pOld, this, p => p.Justificativa_Avaliacao_Comercial);
                Log_ComparaProperty<Venda, LogVendas,Funcionario>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.VENDEDOR, pOld, this, p => p.Vendedor, y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Venda, LogVendas,Departamento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DEPARTAMENTO, pOld, this, p => p.Departamento, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Venda, LogVendas,Funcionario>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SUPERVISOR, pOld, this, p => p.Supervisor, y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Venda, LogVendas, Campanha>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CAMPANHA, pOld, this, p => p.Campanha, y => $"{y.Id} - {y.Titulo}");
                Log_ComparaProperty<Venda, LogVendas,Motivo_Desistencia_Venda>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LISTA_MOTIVO_DESISTENCIA_VENDA, pOld, this, p => p.Motivo_Desistencia_Venda,y => $"{y.Id} - {y.Descricao ?? y.Descricao_Outro}");
                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PERFIL_VENDEDOR, pOld, this, p => p.PerfilVendedor);
                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_AUDITORIA, pOld, this, p => p.StatusAuditoria);
                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.MOTIVO_APROVACAO_DESC, pOld, this, p => p.MotivoAprovManualViabilidade);
                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.MOTIVO_APROVACAO, pOld, this, p => p.MotivoAprovManualESC);
                Log_ComparaProperty<Venda, LogVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.RESPONSAVEL, pOld, this, p => p.IdResponsavelString);
                Log_ComparaProperty<Venda, LogVendas, Departamento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SETOR_DEPARTAMENTO, pOld, this, p => p.Id_Setor_Departamento, y => $"{y.Id} - {y.Descricao}");
            }

            foreach (var item in logs)
            {
                item.StatusVenda = Convert.ToInt32(this.Status);
            }

            if (this.Formas_Pagamento != null)
            {
                foreach (var forma_pagamento in this.Formas_Pagamento)
                {
                    var log = new LogVendas
                    {
                        IdLog = this.Id,
                        UsuarioCriacao = pUsuario,
                        DataHoraCriacao = DateTime.Now,
                        Chave = Convert.ToString(this.Id),
                        Campo = "Inclusão - Forma de Pagamento",
                        NomeUsuarioCriacao = pNomeUsuario
                    };

                    if (forma_pagamento.Id == 0)
                    {
                        log.TipoOperacao = TipoOperLog.Insercao;
                        log.Campo_ValorAntes = "";
                        log.Campo_ValorDepois = $"{forma_pagamento.Id_Forma_Pagamento}-{forma_pagamento.Desc_Forma_Pagamento}";
                    }
                    else
                    {
                        continue; // se não for inserção nem exclusão, pula
                    }

                    logs.Add(log);
                }
            }

            return logs;
        }
        public virtual List<LogAlteracaoPlanoContrato> GeraLogAlteracao(Venda pOld, string pUsuario, string pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAlteracaoPlanoContrato> logs = new List<LogAlteracaoPlanoContrato>();
            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                if (this.Workflow_Vendas != null)
                {
                    Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_VENDA, pOld, this, p => p.StatusVenda);
                }
                else
                {
                    Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                };

                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.JUSTIFICATIVA, pOld, this, p => p.Justificativa_Avaliacao_Comercial);
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato, Funcionario>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.VENDEDOR, pOld, this, p => p.Vendedor, y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato, Departamento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DEPARTAMENTO, pOld, this, p => p.Departamento, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato, Funcionario>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SUPERVISOR, pOld, this, p => p.Supervisor, y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato, Campanha>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CAMPANHA, pOld, this, p => p.Campanha, y => $"{y.Id} - {y.Titulo}");
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato, Motivo_Desistencia_Venda>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LISTA_MOTIVO_DESISTENCIA_VENDA, pOld, this, p => p.Motivo_Desistencia_Venda, y => $"{y.Id} - {y.Descricao ?? y.Descricao_Outro}");
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PERFIL_VENDEDOR, pOld, this, p => p.PerfilVendedor);
                Log_ComparaProperty<Venda, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_AUDITORIA, pOld, this, p => p.StatusAuditoria);
            }
            
            return logs;
        }
    }
}
