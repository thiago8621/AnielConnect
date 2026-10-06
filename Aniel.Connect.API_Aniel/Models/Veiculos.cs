using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Veiculos
    {
        public virtual string codEmpresa { get; set; }
        public virtual string empresa { get; set; }
        public virtual string codFilial { get; set; }
        public virtual string filial { get; set; }
        public virtual string codFrota { get; set; }
        public virtual string placa { get; set; }
        public virtual string codMarca { get; set; }
        public virtual string marca { get; set; }
        public virtual string codModelo { get; set; }
        public virtual string modelo { get; set; }
        public virtual string numFre { get; set; }
        public virtual string condutor { get; set; }
        public virtual string codCCusto { get; set; }
        public virtual string centroCusto { get; set; }
        public virtual string codFornecedor { get; set; }
        public virtual string fornecedorLocadora { get; set; }
        public virtual string codSituacao { get; set; }
        public virtual string descricaoSituacao { get; set; }
        public virtual string codProprietario { get; set; }
        public virtual string proprietario { get; set; }
        public virtual string km { get; set; }
        public virtual string chassi { get; set; }
        public virtual string codRenavam { get; set; }
        public virtual string ano { get; set; }
        public virtual string cor { get; set; }
        public virtual string codCombustivel { get; set; }
        public virtual string tipoCombustivel { get; set; }
        public virtual string cidade { get; set; }
        public virtual string dataAquisicao { get; set; }
        public virtual string dataBaixa { get; set; }
        public virtual string observacao { get; set; }
        public virtual string historico { get; set; }
        public virtual string numPassageiro { get; set; }
        public virtual string anoCarroceria { get; set; }
        public virtual string bancoAlienacao { get; set; }
        public virtual string codCategoria { get; set; }
        public virtual string categoria { get; set; }
        public virtual string localidade { get; set; }
        public virtual string mediaKmLitro { get; set; }
        public virtual string mediaKmGnv { get; set; }
        public virtual string contabilizado { get; set; }
        public virtual string valorVenda { get; set; }
        public virtual string valorAquisicao { get; set; }
        public virtual string localAquisicao { get; set; }
        public virtual string codCliente { get; set; }
        public virtual string cliente { get; set; }
        public virtual string uf { get; set; }
        public virtual string seguro { get; set; }
        public virtual string anoFabricacao { get; set; }
        public virtual string notaFiscal { get; set; }
        public virtual string kmOriginal { get; set; }
        public virtual string capacidadeTanque { get; set; }
        public virtual string dataCadastro { get; set; }
        public virtual string codPool { get; set; }
        public virtual string pool { get; set; }
        public virtual string codTarifaLocacao { get; set; }
        public virtual string dataInicPrevCt { get; set; }
        public virtual string dataTermPrevCt { get; set; }
        public virtual string codCtOrigem { get; set; }
        public virtual string valorLocacao { get; set; }
        public virtual string validadeExtintor { get; set; }
        public virtual string tag { get; set; }
        public virtual string anoIpva { get; set; }
        public virtual string crv { get; set; }
        public virtual string capacidadeCilindro { get; set; }
        public virtual string codGaragem { get; set; }
        public virtual string localGaragem { get; set; }
        public virtual string enderecoGaragem { get; set; }
        public virtual string numeroGaragem { get; set; }
        public virtual string bairroGaragem { get; set; }
        public virtual string municipioGaragem { get; set; }
        public virtual string cepGaragem { get; set; }
        public virtual string ufGaragem { get; set; }
        public virtual string latitudeGaragem { get; set; }
        public virtual string longitudeGaragem { get; set; }
        public virtual string codDepartamentoFrota { get; set; }
        public virtual string departamentoFrota { get; set; }
        public virtual string celulaRif { get; set; }
        public virtual string kmContratado { get; set; }
        public virtual string numCartao { get; set; }
        public virtual string codTipoVeiculo { get; set; }
        public virtual string tipoVeiculo { get; set; }

    }

    public class VeiculosRequest : ApiRequest
    {
        

    }
}