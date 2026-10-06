using MongoDB.Bson.Serialization.Attributes;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.T5.Model.nhibernate.mongo
{
    [BsonIgnoreExtraElements]
    public class Elemento_RedeMongo
    {
        [BsonId()]
        public virtual Int64 Id { get; set; }

        public virtual String UsuarioCriacao { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime? DataHoraCriacao { get; set; }

        public virtual String UsuarioAlteracao { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime? DataHoraAlteracao { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Int64 Id_Tipo { get; set; }//--------------------------------------
        public virtual String Tipo { get; set; }
        public virtual Tipo_ElementoIcone Icone { get; set; }
        public virtual String Cor { get; set; }
        public virtual String Fabricante { get; set; }
        public virtual Int64 CapacidadeTipo { get; set; }
        public virtual Boolean E_Cabo { get; set; }
        public virtual Boolean E_Caixa { get; set; }
        public virtual Int64 Id_Tipo_Cabo { get; set; }
        public virtual String Tipo_Cabo { get; set; }
        public virtual Tipo_CaboEnum? Tipo_de_Cabo { get; set; }
        public virtual String Espessura { get; set; }
        public virtual Int32? Num_Pares_Fibra { get; set; }
        public virtual Int32? Num_Tubos_Loose { get; set; }

        public virtual String Cor_Linha_Ativa { get; set; }
        public virtual String Cor_Linha_Inativa { get; set; }
        public virtual String Estilo_Linha_Ativa { get; set; }
        public virtual String Estilo_Linha_Inativa { get; set; }

        public virtual Int64 Id_Modelo { get; set; }
        public virtual String Modelo { get; set; }
        //--------------------------------------



        public virtual List<Double?> Coordenadas { get; set; }
        public virtual String Tipo_Poligono{get;set;}





        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual bool Certificado { get; set; }
        public virtual Status_Elemento Situacao { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int64? Id_POP { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Numero { get; set; }
        public virtual int Capacidade { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Observacao { get; set; }
    }

}
