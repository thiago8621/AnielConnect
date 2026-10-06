using MongoDB.Bson.Serialization.Attributes;
using NHibernate.Mapping.ByCode.Impl;
using System.Collections.Generic;

namespace Aniel.T5.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class PontosAfetadosManutencaoMongo : BaseModelMongo
    {
        [BsonRequired]
        public string codct { get; set; }
        [BsonRequired]
        public string projeto { get; set; }
        [BsonRequired]
        public string numDoc { get; set; }
        [BsonRequired]
        public string numObra { get; set; }
        public List<PopsAfetadosMongo> pops { get; set; }
        public List<ElementosRedeAfetadosMongo> elementosRede { get; set; }
        public List<TipoElementosRedeMongo> tiposElementosRede { get; set; }
        public ClientesAfetadosMongo clientesAfetados { get; set; }
        public EnderecosAfetadosMongo enderecosAfetados { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ClientesAfetadosMongo
    {
		 public long Count { get; set; }
         public List<DadosClientesAfetadosMongo> Clientes { get; set; }
    }

	public class DadosClientesAfetadosMongo
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Cpfcnpj { get; set; }
        public string Plano { get; set; }
        public long Id_Login_Radius { get; set; }
        public string Login_Radius { get; set; }
        public string Ponto_Afetado { get; set; }
        public string Endereco {  get; set; }


    }

    [BsonIgnoreExtraElements]
    public class ElementosRedeAfetadosMongo
    {
		public long Id { get; set; }
        public List<long> Ids_Portas { get; set; }
    }


    [BsonIgnoreExtraElements]
    public class PopsAfetadosMongo
    {
        public long? IdPop { get; set; }
        public List<long?> idsOlts { get; set; }
        public List<long?> idsSlots { get; set; }
        public List<long?> idsPons { get; set; }
        public List<long?> idsConcentradoras { get; set; }
        public List<long?> idsRadius { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class TipoElementosRedeMongo
    {
        public long? Id_Tipo { get; set; }
        public List<ElementosRedeAfetadosMongo> ElementosRede { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EnderecosAfetadosMongo
    {
        public List<long?> Ids_Condominios { get; set; }
        public List<long?> Ids_Enderecos { get; set; }
    }

    public class PontosAfetadosRequest
    {
        public List<PopsAfetadosMongo> Pops { get; set; }
        public List<TipoElementosRedeMongo> TiposElementosRede { get; set; }
        public EnderecosAfetadosMongo EnderecosAfetados { get; set; }
    }


}
