using Aniel.Connect.Core;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Repository.Model
{
    public class Elemento_RedeListaModel
    {
        #region Elemento_RedeListaModel
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Icone { get; set; }
        public virtual String Cor { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Certificado { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Slot { get; set; }
        public virtual String Pon { get; set; }
        public virtual String POP { get; set; }
        public virtual String Cod_Contrato { get; set; }

        //Presentes no Gestao de Mapa
        public virtual String Descricao_Tipo { get; set; }
        public virtual String Descricao_Modelo { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Poste { get; set; }
        public virtual String Cabo { get; set; }
        public virtual String Nos { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String DataCriacao { get; set; }
        public virtual String NumDoc { get; set; }
        public virtual String CodCT { get; set; }
        public string UsuarioCertificacao { get; set; }
        public string DataCertificacao { get; set; }
        public virtual String IdIntegrador { get; set; }
        #endregion
    }
    public class Elemento_RedeDataTableRequest : DataTableRequest
    {
        #region Elemento_RedeDataTableRequest
        public virtual String Certificado { get; set; }
        public List<Int64?> ListaStatus { get; set; } = new List<Int64?>();
        public virtual Int64? IdTipo { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual bool soativos { get; set; }
        public virtual Int64? Poste { get; set; }
        public virtual Int64? Cabo { get; set; }
        public virtual Int64? Caixa { get; set; }

        #endregion
    }
    public class Elemento_RedeBaseRegistrationModel
    {
        #region Elemento_RedeBaseRegistrationModel
        public Int64? Id { get; set; }
        public String Descricao { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        public bool Tem_Certificado { get; set; }
        public Status_Elemento Situacao { get; set; }
        public String Contrato { get; set; }
        public String Projeto { get; set; }
        public Int64? Id_POP { get; set; }
        public Int64? Id_OLT { get; set; }
        public Int64? Id_Slot { get; set; }
        public Int64? Id_Pon { get; set; }
        public String Cep { get; set; }
        public String Endereco { get; set; }
        public String Numero { get; set; }
        public String Nos { get; set; }
        public String UsuarioCriacao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public String NumDoc { get; set; }

        public Int32? Capacidade { get; set; }
        public String Complemento { get; set; }
        public String Uf { get; set; }
        public String Coordenadas { get; set; }
        public String AreaAtendimento { get; set; }
        public long? Num_Coordenadas { get; set; }
        public String Cidade { get; set; }
        public String Bairro { get; set; }
        public String Pais { get; set; }
        public String Cor_Linha_Inativa { get; set; }
        public String Cor_Linha_Ativa { get; set; }
        public String Observacao { get; set; }
        public Int64? Poste { get; set; }
        public Int64? Cabo { get; set; }
        public Int64? Caixa { get; set; }
        public Int64? IdModelo { get; set; }
        public Int64? IdTipo { get; set; }
        public String DescricaoTipo { get; set; }
        public String Metragem { get; set; }
        private List<HttpPostedFile> Files { get; set; }
        public long? Fiber_Selection { get; set; }
        public long? ItemVinculado_Origem { get; set; }
        public long? ItemVinculado_Destino { get; set; }
        public virtual String Objeto { get; set; }

        private IEnumerable<Object> Lista_Privada;
        public virtual IEnumerable<object> Lista_Objetos
        {
            get
            {
                if (Lista_Privada == null)
                {
                    Lista_Privada = new List<Object>();
                }
                return Lista_Privada;

            }
            set { Lista_Privada = value; }

        }
        private IEnumerable<Elemento_RedeFotoArquivo> arquivos { get; set; }
        public virtual IEnumerable<Elemento_RedeFotoArquivo> Arquivos
        {
            get
            {
                if (arquivos == null)
                {
                    arquivos = new List<Elemento_RedeFotoArquivo>();

                }
                return arquivos;
            }
            set
            {
                arquivos = value;
            }
        }
        private List<Elemento_RedeEnderecosModel> _listagemEnderecos { get; set; }
        public virtual List<Elemento_RedeEnderecosModel> Enderecos
        {
            get
            {
                if (_listagemEnderecos == null)
                {
                    _listagemEnderecos = new List<Elemento_RedeEnderecosModel>();

                }
                return _listagemEnderecos;
            }
            set
            {
                _listagemEnderecos = value;
            }
        }


        private List<Elemento_RedeCondominiosModel> _listagemCondominios { get; set; }
        public virtual List<Elemento_RedeCondominiosModel> Condominios
        {
            get
            {
                if (_listagemCondominios == null)
                {
                    _listagemCondominios = new List<Elemento_RedeCondominiosModel>();

                }
                return _listagemCondominios;
            }
            set
            {
                _listagemCondominios = value;
            }
        }
        #endregion
    }
    public class Elemento_RedeCadastroModel : Elemento_RedeBaseRegistrationModel
    {
        #region Elemento_RedeCadastroModel   
        public long? Numero_Endereco { get; set; }
        private IEnumerable<Elemento_RedeConexoes> Connections { get; set; }
        public virtual IEnumerable<Elemento_RedeConexoes> Conexoes
        {
            get
            {
                if (Connections == null)
                {
                    Connections = new List<Elemento_RedeConexoes>();

                }
                return Connections;
            }
            set
            {
                Connections = value;
            }
        }
        #endregion
    }
    public class Elemento_RedeMapRequestModel : Elemento_RedeBaseRegistrationModel
    {
        #region Elemento_RedeMapRequestModel

        private IEnumerable<Conexao_MapRequestModel> privateConnections;
        public virtual IEnumerable<Conexao_MapRequestModel> Conexoes_Request
        {
            get
            {
                if (privateConnections == null)
                {
                    privateConnections = new List<Conexao_MapRequestModel>();
                }
                return privateConnections;
            }
            set { privateConnections = value; }
        }

        #endregion
    }
    public class Elemento_RedeFotoArquivo
    {
        #region Elemento_RedeFotoArquivo

        public virtual long? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        #endregion
    }
    public class Elemento_RedeEnderecosModel
    {
        #region 
        public long Id { get; set; }
        public long Id_Logradouro { get; set; }
        public long Id_Condominio { get; set; }
        public string Cep { get; set; }
        public string Pais { get; set; }
        public string Tipo_Endereco_Logradouro { get; set; }
        public string Tipo_Atendimento_Zona { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Uf { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string Numero { get; set; }
        #endregion
    }

    public class Elemento_RedeCondominiosModel
    {
        #region 
        public long Id { get; set; }
        public long? Id_Logradouro { get; set; }
        public long? Id_Condominio { get; set; }
        public string Cep { get; set; }
        public string Pais { get; set; }
        public string Tipo_Endereco_Logradouro { get; set; }
        public string Tipo_Atendimento_Zona { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Uf { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string Numero { get; set; }
        public string Bloco { get; set; }
        public string Apto_Numero { get; set; }
        public long? Id_Bloco { get; set; }
        public long? Id_Apto_Numero { get; set; }
        public bool? Todos_Apto_Numero { get; set; }
        public bool? Todos_Bloco { get; set; }
        #endregion
    }
    public class Elemento_RedeConexoes
    {
        #region Elemento_RedeConexoes

        public long Id { get; set; }

        public long? Id_Cabo { get; set; }
        public String Descricao_Cabo { get; set; }
        public long Id_Origem { get; set; }

        public long Numero_Fibra { get; set; }
        public long Numero_Loose { get; set; }
        public int portaOrigem { get; set; }
        public int portaDestino { get; set; }

        public long Id_FibraUtilizada { get; set; }
        public long Id_Destino { get; set; }
        public long Id_Conexao_Origem { get; set; }
        public String Descricao_Elemento_Origem { get; set; }
        public long Id_Conexao_Destino { get; set; }
        public String Descricao_Elemento_Destino { get; set; }

        public long Tipo_Elemento_Origem { get; set; }
        public String Descricao_Tipo_Elemento_Origem { get; set; }
        public long Tipo_Elemento_Destino { get; set; }
        public String Descricao_Tipo_Elemento_Destino { get; set; }
        #endregion
    }
    public class Conexao_MapRequestModel
    {
        #region Conexao_MapRequestModel
        public long? Elemento_Origem { get; set; }
        public long? Porta_Origem { get; set; }
        public long? Elemento_Destino { get; set; }
        public long? Porta_Destino { get; set; }
        #endregion
    }
    public class Elemento_RedeTreeViewRequest
    {
        #region Elemento_RedeTreeViewReqeust
        public virtual String Text_Reference { get; set; }
        public RequestTableTreeView SelectedTable { get; set; }
        public virtual bool AreaCobertura_Reference { get; set; }
        #endregion

    }
    public enum RequestTableTreeView
    {
        #region enum_RequestTableTreeView
        Areacobertura = 0,
        Contrato = 1,
        Elemento = 2,
        Projeto = 3,
        Tipo = 4,
        #endregion

    }
    public class Elemento_RedeCaixaListaModel
    {
        #region Elemento_RedeTreeViewModelContrato
        public virtual Int64? Id { get; set; }
        public virtual String Descricao { get; set; }

        #endregion
    }
    public interface ITreeViewModel
    {
        String Id { get; set; }
        String Text { get; set; }
        TipoAgrupamento Tipo { get; set; }
        //public virtual List<TreeViewModel> Children { get { return new List<TreeViewModel>(); } set; }
    }
    public enum TipoAgrupamento
    {
        ModeloElementoRede = 0,
        ElementoRede = 1,
        AreaCobertura = 2,
        UF = 3
    }
    public class Elemento_RedeTreeViewModel : ITreeViewModel
    {
        #region Elemento_RedeTreeViewModelContrato
        public virtual String Id { get; set; }
        public virtual String Text { get; set; }
        public Boolean factor { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual List<Elemento_RedeTreeViewModel> Children { get; set; }
        public TipoAgrupamento Tipo { get; set; }

        #endregion
    }
    public class Elemento_RedeManutencaoViewModel
    {
        #region Elemento_RedeTreeViewManutencaoModel

        public virtual String Elemento { get; set; }
        public virtual String Tipo_Elemento { get; set; }
        public virtual String Modelo_Elemento { get; set; }
        public virtual String Descricao_Elemento { get; set; }
        public virtual String Descricao_Tipo_Elemento { get; set; }
        public virtual String Descricao_Modelo_Elemento { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        #endregion
    }
    public class Elemento_RedeGestaoViewModel
    {
        #region Elemento_RedeTreeViewModelContrato
        public virtual String CodCt { get; set; }
        public virtual String DescricaoCodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String DescricaoProjeto { get; set; }
        public virtual String DescricaoTipo { get; set; }
        public virtual String DescricaoElemento { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Elemento { get; set; }
        public virtual String Descricao_AreaCobertura { get; set; }
        public virtual String AreaCobertura { get; set; }
        #endregion
    }
    public class Elemento_RedeByCodCt
    {
        #region Elemento_RedeByCodCt
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String NomePop { get; set; }
        public virtual String NomeOlt { get; set; }
        public virtual String Olt { get; set; }
        public virtual String Pop { get; set; }
        #endregion
    }
    public class RequestElementoComboConfig
    {
        #region RequestElementoComboConfig
        public virtual Int64? id { get; set; }
        public virtual Boolean Poste { get; set; }
        public virtual Boolean Cabo { get; set; }
        public virtual Boolean Caixa { get; set; }
        public virtual Boolean Reverse { get; set; }
        public virtual Int64? id_olt { get; set; }
        public virtual Int64? id_slot { get; set; }
        public virtual Int64? id_pon { get; set; }
        public virtual Int64? id_pop { get; set; }
        #endregion

    }
    #region GetCombo Personalizado

    public class Elemento_ObjCombo<T>
    {
        public virtual List<Elemento_ObjComboResult<T>> results { get; set; }
        public virtual Int32 total_count { get; set; }

    }
    public class Elemento_ObjComboResult<T>
    {
        public virtual T Id { get; set; }
        public virtual String Text { get; set; }
        public virtual long? Id_Tipo { get; set; }
        public virtual String Tipo { get; set; }
        public virtual long? Id_Modelo { get; set; }
        public virtual String Modelo { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Pop { get; set; }
        public virtual String Olt { get; set; }
        public virtual String Slot { get; set; }
        public virtual String Pon { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
    }
    #endregion
    public class Request_Remover_Conexao_Elemento
    {
        #region Request_Remover_Conexao_Elemento
        public long? Id_elemento { get; set; }
        public long? Id_cabo { get; set; }
        public long? Num_Fibra { get; set; }
        #endregion
    }
    public class RequestElementoVincularPosteCaixa
    {
        #region RequestElementoComboConfig
        public virtual String Usuario { get; set; }
        public virtual Int64? IdPoste { get; set; }
        public virtual Int64? IdCaixa { get; set; }
        #endregion

    }
    public class Geosite_viabilidadePorta
    {
        public string RequestGooglePlace { get; set; }
        public EnderecoClienteViewModel RequestEndereco { get; set; }
    }
    public class FiberComboModel : IObjComboResult<long>
    {
        #region FiberComboModel
        public long Id { get; set; }
        public long Cabo { get; set; }
        public int Numero_Fibra { get; set; }
        public int Numero_Loose { get; set; }
        public String Text { get; set; }
        public String PadraoUtilizado { get; set; }
        #endregion

    }
    public class Instalacao_CaboRequestModel
    {
        #region Instalacao_CaboRequestModel
        public long Id_Tipo { get; set; }
        public IEnumerable<Double> Pontos_Origem { get; set; }
        public IEnumerable<Double> Pontos_Destino { get; set; }
        #endregion
    }
    public class Diagrama_FusaoSetFile
    {
        #region Diagrama_FusaoSetFile
        public long Id { get; set; }
        public String Descricao { get; set; }
        public String Context { get; set; }
        public String Altura { get; set; }
        public String Largura { get; set; }
        public String Extensao { get; set; }
        public String Base64Image { get; set; }
        public string pdfLocation { get; set; }
        public Boolean OverWrite { get; set; }
        public String OrderElements { get; set; }
        #endregion
    }
    [ExcelServiceKey("Id")]

    [ExcelServiceSheetName("Elemento_Rede")]
    public class Elemento_Rede_Importacao : IExcelService
    {
        [ExcelService("Id")]
        public long? Id { get; set; }


        [ExcelService("Bairro")]
        public String Bairro { get; set; }

        [ExcelService("Pais")]
        public String Pais { get; set; }

        [ExcelService("Cod_Caixa")]
        public long? Cod_Caixa { get; set; }


        [ExcelService("Capacidade")]
        public int Capacidade { get; set; }

        [ExcelService("CEP")]
        public String CEP { get; set; }

        [ExcelService("Certificado")]
        public bool Certificado { get; set; }

        [ExcelService("Cidade")]
        public String Cidade { get; set; }

        [ExcelService("Complemento")]
        public String Complemento { get; set; }

        [ExcelService("Contrato")]
        public String Contrato { get; set; }

        [ExcelService("Cor_linha_Ativa")]
        public String Cor_linha_Ativa { get; set; }

        [ExcelService("Cor_linha_Inativa")]
        public String Cor_linha_Inativa { get; set; }
        [Required]
        [ExcelService("Descricao")]
        public String Descricao { get; set; }

        [ExcelService("Endereco")]
        public String Endereco { get; set; }

        [ExcelService("Id_POP")]
        public Int64? Id_POP { get; set; }

        [ExcelService("Id_OLT")]
        public Int64? Id_OLT { get; set; }

        [ExcelService("Id_Slot")]
        public Int64? Id_Slot { get; set; }

        [ExcelService("Id_Pon")]
        public Int64? Id_Pon { get; set; }

        [ExcelService("Latitude")]
        public double? Latitude { get; set; }

        [ExcelService("Longitude")]
        public double? Longitude { get; set; }

        [ExcelService("Numero")]
        public String Numero { get; set; }

        [ExcelService("Observacao")]
        public String Observacao { get; set; }

        [ExcelService("Projeto")]
        public String Projeto { get; set; }

        [ExcelService("Situacao")]
        public int Situacao { get; set; }


        [ExcelService("UF")]
        public String UF { get; set; }

        [ExcelService("Coordenadas")]
        public String Coordenadas { get; set; }

        [ExcelService("AreaAtendimento")]
        public String AreaAtendimento { get; set; }

        [ExcelService("Num_Coordenadas")]
        public long? Num_Coordenadas { get; set; }

        [ExcelService("Metragem")]
        public Double? Metragem { get; set; }

        [ExcelService("Padrao_Cores")]
        public int Padrao_Cores { get; set; }
        [ExcelService("Cabo")]
        public long? IdCabo { get; set; }

        [ExcelService("Caixa")]
        public long? IdCaixa { get; set; }

        [ExcelService("Poste")]
        public long? IdPoste { get; set; }

        [Required]
        [ExcelService("Tipo")]
        public long? IdTipo { get; set; }
        public bool Importar { get; set; }
    }


    public class ListaCaixaRequest
    {
        public List<long> ListIdCaixa { get; set; }
        public virtual Int64? Id_OLT { get; set; }
        public virtual Int64? Id_Slot { get; set; }
        public virtual Int64? Id_Pon { get; set; }
    }
}
