using Aniel.Connect.Model.nhibernate;
using Aniel.T5.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{

    #region MyRegion
    public class AplicacoesRemocoesListModel
    {
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String EquipeAux { get; set; }
        public virtual String Numero_da_Obra { get; set; }
        public virtual String RDO { get; set; }
        public virtual String Contrato_Assinante { get; set; }
        public virtual String Numero_OS { get; set; }
        public virtual String DataHora_Agendamento { get; set; }
        public virtual String DataHora_Aplicacao { get; set; }
        public virtual String Cod_Material { get; set; }
        public virtual String Desc_Auxiliar { get; set; }
        public virtual String CodCpl { get; set; }
        public virtual String CodCpl_Aux { get; set; }
        public virtual String Quantidade_Aplic { get; set; }
        public virtual String Quantidade_Removida { get; set; }
        public virtual String Tarefa { get; set; }
    }
    #endregion
    public class AplicacoesRemocoesApiRequest : ApiRequest
    {
        /// <summary> Data Inicial do Período de Busca </summary>
        public virtual String Data_Ini { get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        public virtual String Data_Fim { get; set; }
        /// <summary> Pesquisa por um Contrato </summary>
        public virtual String Contrato { get; set; }
        /// <summary> Pesquisa por um Projeto </summary>
        public virtual String Projeto { get; set; }
        public virtual bool? Alterado { get; set; }
        public virtual bool Comodato { get; set; }
        public virtual String CodTecnico { get; set; }
        public virtual Boolean? Integrado { get; set; }
    }

    public class AplicacoesRemocoesApiRequestModel : ApiRequest
    {
        [Required]
        public virtual String CodEmpresa { get; set; }
        [Required]
        public virtual String CodFilial { get; set; }
        [Required]
        public virtual String CodLocal { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Tarefa { get; set; }
        public virtual String Cod_Material { get; set; }
        public virtual String Cod_Complementar { get; set; }
        public virtual string Num_Doc { get; set; }
        public virtual String Equipe { get; set; }
    }

    public class CabecalhoInventarioCliente
    {
        public Int64 protocolo { get; set; }
        public Int64 idCliente { get; set; }
        public string nomeCliente { get; set; }
        public string equipe { get; set; }
        public string cpfCnpj { get; set; }
        public DateTime data_criacao { get; set; }
        public Boolean aguard_aprov { get; set; }
        public Boolean aprov { get; set; }
        public List<Material_TarefaAniel> material { get; set; }
    }

    public class ClienteInventario
    {
        public Int64 codCliente { get; set; }
        public String razaoSocial { get; set; }
        public String cnpj { get; set; }
        public String inscEstadual { get; set; }
        public String inscMunicipal { get; set; }
        public String endereco { get; set; }
        public String bairro { get; set; }
        public String municipio { get; set; }
        public String CEP { get; set; }
        public String UF { get; set; }
        public String pais { get; set; }
        public String telefone { get; set; }
        public String celular { get; set; }
        public String fax { get; set; }
        public String contato { get; set; }
        public String email { get; set; }
        public String tipo { get; set; }
        public String fantasia { get; set; }
        public String complemento { get; set; }
        public String enderecoNumero { get; set; }
    }
}