using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    /// <summary>
    ///     Classe que contém os campos da chave composta da TB_DOCUMENTO_PRODUCAO
    /// </summary>
    public class ChavesDocumentoProducao
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumObra { get; set; }
        public int NumDoc { get; set; }
    }

    public class DocumentoProducao_GetConfigFluxoBaixa
    {
        public String codCt { get; set; }
        public String projeto { get; set; }
        public String numObraOriginal { get; set; }
        public Int32 numDoc { get; set; }
        public String codEncerramento { get; set; }
        public Int32 codNatureza { get; set; }
        public String fluxoAoEncerrar { get; set; }
        public Int32? comportamentoTipoServ { get; set; }
        public Int32? codTipoServ { get; set; }
        public Int32? comportamentoSetor { get; set; }
        public Int32? codDepto { get; set; }
        public String utilizaNatureza { get; set; }
    }

    public class PlanoRequest
    {
        public virtual long? Id { get; set; }
        public virtual string CodigoIntegracao { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String CodigoTipoPlanoIntegracao { get; set; }
        public virtual string IdLogin { get; set; }
        public virtual String Login { get; set; }
        public virtual String Senha { get; set; }
        public virtual String IdClientePlano { get; set; }
    }

    public class OsApiModel
    {

        /// <summary> Pesquisa por Cpf de Funcionário </summary>         
        public virtual String Cpf { get; set; }

        public virtual String CodigoIntegratorTipoServico { get; set; }

        ///<summary>Tipo de Serviço </summary>
        public virtual String TipoServico { get; set; }
        public virtual String SubTipoServico { get; set; }

        ///<summary> Origem do Serviço(0: Connect, 1: Ola Cliente) </summary>
        public virtual Origem_Documento_Producao? Origem { get; set; }

        /// <summary> Projeto da Ordem de Serviço </summary>
        public virtual String Projeto { get; set; }

        /// <summary> Numero da Os </summary>
        public virtual String CodCt { get; set; }

        /// <summary> Numero da Os </summary>
        public virtual String NumOS { get; set; }
        public virtual String Uni { get; set; }
        public virtual DateTime? DataHoraAgendamento { get; set; }
        public string TipoImovel { get; set; }
        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }
        public string Endereco { get; set; }
        public string NumeroEndereco { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string PontoReferencia { get; set; }
        public string UF { get; set; }
        public string Observacao { get; set; }
        public string Facilidade { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Tecnico { get; set; }
        public string NomeCliente { get; set; }
        public string TelefoneCelularCliente { get; set; }
        public string TelefoneFixoCliente { get; set; }
        public string EmailCliente { get; set; }
        public string LoginRadius { get; set; }
        public string SenhaRadius { get; set; }
        public string ContratoCliente { get; set; }
        public string ContratoClienteAnterior { get; set; }
        public string CategoriaCliente { get; set; }
        public string NumDoc { get; set; }
        public bool CadastrarSequencia { get; set; }
        public virtual String Wo { get; set; }
        public string DataCadastro { get; set; }
        public string DataEncerramento { get; set; }
        public string DataAgendamento { get; set; }
        public string DataVencimento { get; set; }
        public string Segmento { get; set; }
        public string Departamento { get; set; }
        public Tipo_PrioridadeT5 Prioridade { get; set; }
        public PlanoRequest Plano { get; set; }
        public string DescricaoPlanoProduto { get; set; }
        public PlanoRequest PlanoAnterior { get; set; }
        public List<PlanoRequest> PlanosAdicionais { get; set; }

        public virtual String Adicional1 { get; set; }
        public virtual String Adicional2 { get; set; }
        public virtual String Adicional3 { get; set; }
        public virtual String Adicional4 { get; set; }
        public virtual String Adicional5 { get; set; }
        public virtual String Adicional6 { get; set; }
        public virtual String Adicional7 { get; set; }
        public virtual String Adicional8 { get; set; }
        public virtual String Adicional9 { get; set; }
        public virtual String Adicional10 { get; set; }

        public string Cod_Encerramento_TOA { get; set; }
        /// <summary>
        /// Tipo de Ordem de Serviço (C: Cliente, 1: Manutenção)
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Id do POP
        /// </summary>
        public long? IdPop { get; set; }

        /// <summary>
        /// Id da OLT
        /// </summary>
        public long? IdOlt { get; set; }
        /// <summary>
        /// Id do elemento de rede
        /// </summary>
        public long? IdElementoRede { get; set; }
        /// <summary>
        /// Id do elemento de rede
        /// </summary>
        public bool UsarNumeroSequencial { get; set; }
        public string CodigoAuxiliarLogin { get; set; }
        public string UsuarioIntegracao { get; set; }
        public string CaixaFTTH { get; set; }
        public long? PortaFTTH { get; set; }
        public string Num_Doc { get; set; }
        public string InformacoesComplementares { get; set; }
        public DateTime DataAbertura { get; set; }
        /// <summary>
        /// Campo vindo de integração que define qual a tecnologia do equipamento que será manipulado na O.S (Fibra, Rádio, etc)
        /// </summary>
        public string Tecnologia { get; set; }
        /// <summary>
        /// Define se o Tipo De Serviço deve estar previamente criado no Aniel
        /// </summary>
        public bool NaoCriaOsSemTipo { get; set; }
    }

    public class EquipeIntegracaoModel
    {
        public String Matricula { get; set; }
        public String CodAux { get; set; }
        public String Nome { get; set; }
        public String CodCtPadrao { get; set; }
        public String ProjetoPadrao { get; set; }

        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }

    }
    public class RequestAlteraOs 
    {        
        public string Usuario { get; set; }
        public virtual String Os { get; set; }        
        public virtual String Contrato { get; set; }        
        public virtual String Projeto { get; set; }        
        public virtual int? CodigoCliente { get; set; }

        public virtual string GrupoArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public string Tecnico { get; set; }
        public ResponsabilidadeOS? Responsabilidade { get; set; }
        public string Data_HoraAgendamento { get; set; }
        public string Periodo { get; set; }
        public string TipoServico { get; set; }
        public string SubtipoServico { get; set; }
        public string Segmento { get; set; }
        public string Setor { get; set; }
        public string Prioridade { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
        public string TipoImovel { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DetalhesOS { get; set; }
        public string InformacoesComplementares { get; set; }
        public string ObservacaoTeste { get; set; }
        public CamposAdicionais CamposAdicionais { get; set; }
    }
    public class CamposAdicionais
    {
        public string Adicional_1 { get; set; }
        public string Adicional_2 { get; set; }
        public string Adicional_3 { get; set; }
        public string Adicional_4 { get; set; }
        public string Adicional_5 { get; set; }
        public string Adicional_6 { get; set; }
        public string Adicional_7 { get; set; }
        public string Adicional_8 { get; set; }
        public string Adicional_9 { get; set; }
        public string Adicional_10 { get; set; }
    }
}
