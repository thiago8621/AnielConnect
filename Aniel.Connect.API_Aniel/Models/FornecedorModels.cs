using System;
using Aniel.Connect.Core;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    /// Lista de Fornecedores
    /// </summary>
    public class FornecedorListaModel 
    {
        public virtual String Cod_Fornecedor { get; set; }
        public virtual Int64 Id { get; set; }
        public virtual String Cnpj { get; set; }
        public virtual String Fornecedor { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Municipio { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Telefone2 { get; set; }
        public virtual String Telefone3 { get; set; }
        public virtual String Telefone4 { get; set; }
        public virtual String Fax { get; set; }
        public virtual String Contato { get; set; }
        public virtual String Contato2 { get; set; }
        public virtual String Insc_Estadual { get; set; }
        public virtual String Insc_Municipal { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Email { get; set; }
        public virtual String Tipo_Forn { get; set; }
        public virtual String Prazo_Pagamento { get; set; }
        public virtual String Iso9000 { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Celular { get; set; }
        public virtual String Celular2 { get; set; }
        public virtual String Site { get; set; }
        public virtual String Historico { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Conta { get; set; }
        public virtual String Micro_Empresa { get; set; }
        public virtual String Cod_Atividade { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Opcao_Func { get; set; }
        public virtual String Integracao { get; set; }
        public virtual String Conta_Contabil { get; set; }
        public virtual String Codaux_For { get; set; }
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Data_Cadastro { get; set; }
        public virtual String Hora_Criacao { get; set; }
        public virtual String Usuario_Alteracao { get; set; }
        public virtual String Data_Alteracao { get; set; }
        public virtual String Hora_Alteracao { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String Data_Inativo { get; set; }
        public virtual String NF_Parcial { get; set; }
        public virtual String Responsavel_Fornecedor { get; set; }
        public virtual String Situacao_Integra { get; set; }
        public virtual String Id_Municipio { get; set; }
        public virtual String Endereco_Numero { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String DataHora_Criacao { get; set; }
        public virtual String UsuarioAlteracao { get; set; }
        public virtual String DataHora_Alteracao { get; set; }
        public virtual String Token { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Numero { get; set; }


    }
    /// <summary>
    /// Cadastro de Fornecedores
    /// </summary>
    public class ForncedorCadastroModel : ApiRequest
    {
        /// <summary> Código do Fornecedor </summary>
        public virtual String Cod_Fornecedor { get; set; }
        /// <summary> Razão Social do Fornecedor  </summary>
        public virtual String RazaoSocial { get; set; }
        /// <summary> Descrição do Fornecedor </summary>
        public virtual String Descricao { get; set; }
        /// <summary> CNPJ do Fornecedor </summary>
        public virtual String Cnpj { get; set; }
        /// <summary> Nome Fantasia do Fornecedor </summary>
        public virtual String fornecedor { get; set; }
        /// <summary> Endereço do Fornecedor </summary>
        public virtual String Endereco { get; set; }
        /// <summary> Complemento do Endereço do Fornecedor </summary>
        public virtual String Complemento { get; set; }
        /// <summary> Numero do Estabelecimento do Fornecedor </summary>
        public virtual Int32? Numero { get; set; }
        /// <summary> Bairro do Fornecedor </summary>
        public virtual String Bairro { get; set; }
        /// <summary> Município do Fornecedor </summary>
        public virtual String Municipio { get; set; }
        /// <summary> CEP do Fornecedor </summary>
        public virtual String Cep { get; set; }
        /// <summary> UF do Fornecedor </summary>
        public virtual String Uf { get; set; }
        /// <summary> País do Fornecedor </summary>
        public virtual String Pais { get; set; }
        /// <summary> Inscrição Estadual do Fornecedor </summary>
        public virtual String Insc_Estadual { get; set; }
        /// <summary> Telefone do Fornecedor </summary>
        public virtual String Telefone { get; set; }
        /// <summary> Email do Fornecedor </summary>
        public virtual String Email { get; set; }

    }


}