using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    //Atendimento: CPF ou CNPJ do cliente, plano e tipo de atendimento.

    public class OsApiRequest : ApiRequest
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
        public string DataAbertura { get; set; }
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
        /// Descrição da caixa FTTH
        /// </summary>
        public string CaixaFTTH { get; set; }
        /// <summary>
        /// Número da porta FTTH
        /// </summary>
        public long? PortaFTTH { get; set; }
        /// <summary>
        /// Id do elemento de rede
        /// </summary>
        public bool UsarNumeroSequencial { get; set; }
        public string CodigoAuxiliarLogin { get; set; }
        /// <summary>
        /// Campo destinado para inserção de informacoes Complementares na OS (Campo renomeado: Observacao_OITV)
        /// </summary>
        public string InformacoesComplementares { get; set; }
        public string UsuarioCriacao { get; set; }
        /// <summary>
        /// Campo vindo de integração que define qual a tecnologia do equipamento que será manipulado na O.S (Fibra, Rádio, etc)
        /// </summary>
        public string Tecnologia { get; set; }
        /// <summary>
        /// Define se o tipo de serviço da OS precisa estar cadastrado previamente no Aniel
        /// </summary>
        public bool NaoCriarOsSemTipo { get; set; }
    }

    public class OrdemServicoAnexoApiRequest : ApiRequest
    {
        public string File { get; set; }
        /// <summary>
        /// Parâmetro para acesso a OS
        /// </summary>
        [Required]
        public string NumObraOriginal { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }
    }
    public class OrdemServicoApiRequest : ApiRequest 
    {
        /// <summary>
        /// Parâmetro para acesso a OS
        /// </summary>
        [Required]
        public string NumObraOriginal { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }
    }
    
    public class AplicacaoRemocaoMaterialApiRequest : ApiRequest 
    {
        [Required]
        public string numOS { get; set; }
        public string ClienteCpfCnpj { get; set; }
        public short TipoServico { get; set; }
        public List<AplicacaoRemocaoMaterial> Materiais { get; set; }
        public string Equipe { get; set; }
    }
    
    public class AplicacaoRemocaoMaterial
    {
        public string CodMaterial { get; set; }
        public float Quantidade { get; set; }
    /// <summary>
        /// 'A' para aplicacao/ 'R' para remocao
        /// </summary>
        public char Operacao { get; set; }
        /// <summary>
        /// Serial do material, vazio ou nulo quando nao houver
        /// </summary>
        public string CodCompl { get; set; }
    }

    public class MovimentacaoMaterialResponse
    {
        public int CodCt { get; set; } // Código do contrato vinculado
        public string Projeto { get; set; } // Nome do projeto associado ao material
        public string NumObra { get; set; } // Número da obra vinculada
        public string NumDoc { get; set; } // Número do documento relacionado à retirada do material
        public string Tarefa { get; set; } // Tarefa associada à movimentação do material
        public string Equipe { get; set; } // Equipe responsável pelo registro
        public string ClienteCpfCnpj { get; set; } // Identificação do cliente
        public string CodMat { get; set; } // Código do material
        public string CodCpl { get; set; } // Código complementar do material
        public DateTime DataAplic { get; set; } // Data de aplicação do material
        public string Usuario { get; set; } // Nome do usuário que realizou a operação
        public DateTime Dia { get; set; } // Dia do registro
        public TimeSpan Hora { get; set; } // Hora da operação
        public float QuantAplic { get; set; } // Quantidade Aplicada
        public float QuantRemov { get; set; } // Quantidade removida
    }


    /// </summary>
    public class OS_AnexosRequestModel : OrdemServicoApiRequest
    {                     
    }   
  
}