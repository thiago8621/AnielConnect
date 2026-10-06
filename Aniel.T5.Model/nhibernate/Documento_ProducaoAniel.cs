using System;
using System.Collections.Generic;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Alterar_Status
    {
        public List<String> ids { get; set; }
        public string justificativa { get; set; }
    }
    public class AlteraVinculoManutencao

    {
        public String Projeto_Manut { get; set; }
        public String CodCt_Manut { get; set; }
        public String Num_Obra_Manut { get; set; }
        public Int32 numDoc_Manut { get; set; }

        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public String Informacao { get; set; }
        public Int32 numDoc { get; set; }
    }

    public class DocumentoProducaoChaveModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
    }

    public class VincularOsModel
    {
        public List<String> ids { get; set; }
        public String idOsManutencao { get; set; }
        public String usuario { get; set; }
        public String Projeto_Manut { get; set; }
        public String CodCt_Manut { get; set; }
        public String Num_Obra_Manut { get; set; }
        public Int32 numDoc_Manut { get; set; }

    }


    public class Documento_ProducaoAniel
    {
        public virtual string Cod_Encerramento_TOA { get; set; }
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Num_Obra { get; set; }


        public virtual int Num_Doc { get; set; }
        public virtual string CodCt_Manut { get; set; }
        public virtual string Projeto_Manut { get; set; }
        public virtual string NumObra_Manut { get; set; }
        public virtual int NumDoc_Manut { get; set; }


        public virtual string Unidade_Custo { get; set; }

        public virtual int Seq { get; set; }
        public virtual string Numero_Obra_Original { get; set; }

        public virtual string OXBDPEE { get; set; }

        public virtual StatusExecucaoAniel? Status { get; set; }
        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }
        public virtual Status_AuditoriaAniel? Status_Auditoria { get; set; }
        public virtual String UsuarioAuditoria { get; set; }
        public virtual string Equipe { get; set; }

        public virtual String Exige_Pagamento_Instalacao { get; set; }

        /// <summary>
        ///     Objeto utilizado somente para leitura em consultas IQueryable
        ///     com o NHibernate
        /// </summary>
        public virtual EquipeAniel EquipeAniel { get; set; }

        public virtual string Nome_Cliente { get; set; }

        public virtual string Devolvida { get; set; }

        public virtual string Cod_Escritorio { get; set; }
        public virtual string Cod_Area { get; set; }
        public virtual int? Cod_Localidade { get; set; }

        public virtual short? Cod_Tipo_Serv { get; set; }

        /// <summary>
        ///     Registro de agendamento para OS agendada quando a configuração de empresa
        ///     'Considera agendamento por período' desligada.
        ///
        ///     Objeto utilizado somente para leitura em consultas IQueryable
        ///     com o NHibernate.
        /// </summary>
        public virtual Agendamento_EquipeAniel Agendamento { get; set; }

        /// <summary>
        ///     Objeto utilizado somente para leitura em consultas IQueryable
        ///     com o NHibernate
        /// </summary>
        public virtual Tipo_Servico_EquipeAniel TipoServico { get; set; }
        public virtual bool TodasSubTipoServico { get; set; }
        public virtual bool TodasCod_Encerramento { get; set; }
        public virtual string Cod_Sub_Tipo_Serv { get; set; }
        public virtual string Id_Cobranca { get; set; }
        public virtual string Segmento { get; set; }

        /// <summary>
        /// Utilizado para ponto de referencia na tela de cadastro de OS
        /// </summary>
        public virtual string Observacao { get; set; }
        public virtual string ObservacaoPausa { get; set; }
        public virtual string InformacoesComplementares { get; set; }
        public virtual int? Cod_Tipo_Imovel { get; set; }
        public virtual string Tipo_Producao { get; set; }
        public virtual ClienteAniel Cliente { get; set; }
        public virtual long? Id_PlanoProduto { get; set; }
        public virtual string CodigoPlanoIntegrator { get; set; }
        public virtual long? Id_Cliente_PlanoProdutoConnect { get; set; }
        public virtual long? Id_Cliente_ContratoConnect { get; set; }
        public virtual long? Id_PlanoProduto_Ant { get; set; }
        public virtual long? Id_Cliente_PlanoConnect_Ant { get; set; }
        public virtual string DescPlanoAnterior { get; set; }
        public virtual string CategoriaPlanoAnterior { get; set; }
        public virtual string CPF_CLIENTE { get; set; }
        public virtual string Titular { get; set; }
        public virtual string Logradouro { get; set; }
        public virtual string Bloco { get; set; }
        public virtual string Apartamento { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string UF { get; set; }
        public virtual string Cep { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Complemento { get; set; }
        public virtual string Pais { get; set; }
        public virtual long? Id_Endereco_Cliente_Antigo { get; set; }
        public virtual string Endereco_Antigo { get; set; }
        public virtual string Endereco_Complemento_Antigo { get; set; }
        public virtual string Endereco_Numero_Antigo { get; set; }
        public virtual string Endereco_Bairro_Antigo { get; set; }
        public virtual string Endereco_Cidade_Antigo { get; set; }
        public virtual string Endereco_UF_Antigo { get; set; }
        public virtual string Endereco_Cep_Antigo { get; set; }
        public virtual string TelefonePrincipal { get; set; }
        public virtual string CelularCliente { get; set; }
        public virtual string TelefoneComercial { get; set; }
        public virtual string RG_Assinante { get; set; }
        public virtual string Email_Assinante { get; set; }
        public virtual DateTime? DataHora_Deslocamento { get; set; }
        public virtual DateTime? DataHora_Ini_Atend { get; set; }
        public virtual DateTime? DataHora_Fim_Atend { get; set; }
        public virtual DateTime? DataHora_Pausou { get; set; }
        public virtual DateTime? DataHora_Reinicio { get; set; }
        public virtual DateTime? DataHora_Interromper_Deslocamento { get; set; }
        public virtual DateTime? Data_Maxima { get; set; }
        public virtual DateTime? Hora_Maxima { get; set; }
        public virtual DateTime? Data_Prod { get; set; }
        public virtual DateTime? Hora_Prod { get; set; }
        public virtual DateTime? Data_Indicador { get; set; }
        public virtual DateTime? Hora_Indicador { get; set; }
        public virtual DateTime? Data_Encerramento { get; set; }
        public virtual DateTime? Hora_Encerramento { get; set; }
        public virtual string Cod_Encerramento { get; set; }
        public virtual string justificativaReprova { get; set; }
        public virtual string justificativaAprova { get; set; }
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual string Latitude_Str { get; set; }
        public virtual string Longitude_Str { get; set; }
        public virtual char Repetida { get; set; }
        public virtual string OsDuplicada { get; set; }
        public virtual Boolean UtilizaEndCond { get; set; }
        public virtual long? IdCondominioPredioEndereco { get; set; }
        public virtual int? BuscarEnderecoPorId { get; set; }
        public virtual string RoteirizadoAuto { get; set; }
        public virtual string MotivoCancelamento { get; set; }
        public virtual int? Cod_Motivo_Cancel { get; set; }
        public virtual DateTime? Data_DashBoard { get; set; }

        public virtual double? GetLatitude_Str()
        {
            if (!string.IsNullOrEmpty(Latitude_Str))
            {
                try
                {
                    return Convert.ToDouble(Latitude_Str.Replace(".", ","));
                }
                catch (FormatException)
                {
                    throw new FormatException("Latitude inválida");
                }
            }
            else
                return null;
        }

        public virtual double? GetLongitude_Str()
        {
            if (!string.IsNullOrEmpty(Longitude_Str))
            {
                try
                {
                    return Convert.ToDouble(Longitude_Str.Replace(".", ","));
                }
                catch (FormatException)
                {
                    throw new FormatException("Longitude inválida");
                }
            }

            else
                return null;
        }

        public virtual string Num_RVT { get; set; }
        public virtual string Tarefa { get; set; }
        public virtual string Observacao_Aux { get; set; }
        public virtual string Observacao_Teste { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Usuario_Baixa { get; set; }
        public virtual string Modulo_Baixa { get; set; }

        /// <summary>
        /// Código alfanumérico de 6 dígitos gerado pelo BackOffice para autorizar Baixa do Serviço no Aniel Touch
        /// </summary>
        public virtual string Senha_Autorizacao_Baixa { get; set; }
        public virtual Boolean AlterarIndicador { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }

        /// <summary>
        ///     Campo apenas para leitura utilizado em consultas com NHibernate
        /// </summary>
        public virtual SenhaAniel Monitor { get; set; }
        public virtual string Usuario_Monitor { get; set; }
        public virtual string Cod_Superv { get; set; }
        public virtual string Usuario_Agendamento { get; set; }
        public virtual DateTime? DiaHora_Agendamento { get; set; }
        public virtual Origem_Documento_Producao Origem_Documento { get; set; }
        public virtual long? Id_Atendimento { get; set; }
        public virtual string Sigla_Modulo { get; set; }
        public virtual ResponsabilidadeOS? Responsabilidade { get; set; }
        public virtual string Roterizar { get; set; }
        public virtual string SenhaRadius { get; set; }
        public virtual string LoginRadius { get; set; }
        public virtual int? CodTurnoServ { get; set; }

        /// <summary>
        ///     Período agendado para execução do serviço.
        ///     Campo somente para leitura pelo NHibernate
        /// </summary>
        public virtual Turno_ServicoAniel TurnoServico { get; set; }

        public virtual string CaixaFTTH { get; set; }
        public virtual string PortaFTTH { get; set; }
        public virtual long? IdLogin { get; set; }
        public virtual string Adicional_1 { get; set; }
        public virtual string Adicional_2 { get; set; }
        public virtual string Adicional_3 { get; set; }
        public virtual string Adicional_4 { get; set; }
        public virtual string Adicional_5 { get; set; }
        public virtual string Adicional_6 { get; set; }
        public virtual string Adicional_7 { get; set; }
        public virtual string Adicional_8 { get; set; }
        public virtual string Adicional_9 { get; set; }
        public virtual string Adicional_10 { get; set; }
        public virtual long? Id_Venda { get; set; }
        public virtual string NomeInstalacao { get; set; }
        public virtual string CpfInstalacao { get; set; }
        public virtual string TelefoneInstalacao { get; set; }
        public virtual Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }
        public virtual string WO { get; set; }
        public virtual string TemOsDigital { get; set; }
        //public virtual string UltimoSeq { get; set; }
        public virtual long? CodDepto { get; set; }
        public virtual string Tipo { get; set; }

        public virtual string Ssid_Wifi { get; set; }
        public virtual string Senha_Wifi { get; set; }

        //***IDS CONNECT***
        public virtual long? Id_Olt { get; set; }
        public virtual long? Id_Pop { get; set; }
        public virtual long? Id_Elemento_Rede { get; set; }

        /// <summary>
        ///     "S" ou "N". Informa se o serviço veio de uma Integracao
        /// </summary>
        public virtual string Integracao { get; set; }
        public virtual Int64? CodMotivoReagendamento { get; set; }
        public virtual String ObsMotivoReagendamento { get; set; }
        public virtual string DataAbertura { get; set; }
        public virtual string HoraAbertura { get; set; }

        public virtual string IdJobNotificacaoAtraso { get; set; }
        public virtual string IdJobSla { get; set; }
        public virtual TipoManutencaoOs? TipoManutencaoOs { get; set; }
        public virtual string DescricaoPlanoAssinante { get; set; }


        /// <summary>
        /// Campo vindo de integração que define qual a tecnologia do equipamento que será manipulado na O.S (Fibra, Rádio, etc)
        /// </summary>
        public virtual string Tecnologia { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Documento_ProducaoAniel other = obj as Documento_ProducaoAniel;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && Num_Obra == other.Num_Obra && Num_Doc == other.Num_Doc)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodCt + "|" + Projeto + "|" + Num_Obra + "|" + Num_Doc).GetHashCode();
        }

        public static implicit operator List<object>(Documento_ProducaoAniel v)
        {
            throw new NotImplementedException();
        }
    }
}
