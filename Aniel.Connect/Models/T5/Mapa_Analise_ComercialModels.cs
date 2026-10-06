using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Aniel.T5.Model.enumerado;
using MongoDB.Bson.Serialization.Attributes;
using Situacao_Porta = Sinapse.Model.enumerados.Situacao_Porta;
//using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Aniel.Connect.Models.T5
{
    public class Mapa_Analise_ComercialListModel
    {
        public virtual String Cor { get; set; }
        public virtual String Coordenadas { get; set; }
        public virtual List<List<Double?>> ListCoordenadas
        {
            get
            {
                List<List<Double?>> CoordenadasDouble2 = new List<List<Double?>>();
                //foreach (String coordenada in Coordenadas)
                //CoordenadasDouble2.Add(Json.Decode<List<Double?>>(coordenada));
                CoordenadasDouble2 = Json.Decode<List<List<Double?>>>(Coordenadas);
                return CoordenadasDouble2;
            }
            set
            {
                Coordenadas = Json.Encode(value);
                //foreach (List<Double?> coordenada in value)
                //Coordenadas.Add(Json.Encode(coordenada));
            }
        }
        public virtual String Id { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Certificado { get; set; }
        public virtual bool Area_Cobertura { get; set; }
        public virtual bool Caixa { get; set; }
        public virtual bool Cliente { get; set; }
        public virtual bool Lead { get; set; }
        public virtual bool Prospect { get; set; }

        private IEnumerable<Analise_Comercial_Caixa> caixas { get; set; }
        private IEnumerable<Analise_Comercial_Plano> planos { get; set; }
        public IEnumerable<Analise_Comercial_Caixa> Caixas
        {
            get
            {
                if (caixas == null)
                {
                    caixas = new List<Analise_Comercial_Caixa>();
                }
                return caixas;
            }
            set { caixas = value; }
        }
        public IEnumerable<Analise_Comercial_Plano> Planos

        {
            get
            {
                if (planos == null)
                {
                    planos = new List<Analise_Comercial_Plano>();
                }
                return planos;
            }
            set
            { planos = value; }
        }

    }
    public class Analise_Comercial_Plano
    {
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Distancia { get; set; }
        public virtual String Portas_Disponiveis { get; set; }
    }
    public class Analise_Comercial_Caixa
    {


        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Carencia { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Periodo_Teste { get; set; }

    }
    public class Caixa
    {
        public virtual String Id { get; set; }
        public virtual String latitude { get; set; }
        public virtual String longitude { get; set; }
    }

    public class ReservarCaixa
    {
        public virtual Int64? Id_caixa { get; set; }
        public virtual Int64? Porta { get; set; }

    }
    public class Analise_ComercialViewModel
    {
        public virtual String ObjJson { get; set; }
        public virtual String GeoCliente { get; set; }
        public virtual String Plano { get; set; }
        public virtual String PoligonoBanco { get; set; }
    }

    public interface IAnalise_RetornoObject
    {
        public long? Id { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }        
        public String Color { get; set; }
        public long? Id_Check { get; set; }
    }
    public class Analise_Caixa : IAnalise_RetornoObject
    {
        public virtual long? Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Icone_Str { get; set; }
        public virtual String Cor_Hexadecimal { get; set; }
        public virtual Int64 Qtd_PortasTotal { get; set; }
        public virtual Int64 Qtd_PortasDisponivel { get; set; }
        public string Color { get; set; }
        public long? Id_Check { get; set; }

    }
    public class Analise_Area_Cobertura : IAnalise_RetornoObject
    {
        public virtual Int64? Id { get; set; }        
        public virtual String Descricao { get; set; }
        public virtual String Color { get; set; }
        public virtual Status_Default Situacao { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual List<List<Double?>> Coordenadas { get; set; }
        public virtual String Cor { get; set; }
        public virtual Tipo_Poligono Tipo { get; set; }
        public long? Id_Check { get; set; }

    }

    public class Analise_Cliente : IAnalise_RetornoObject
    {
        public long? Id { get; set; }
        public String Nome { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        public String Icone_Str { get; set; }
        public String Status { get; set; }
        private IEnumerable<Analise_Login> radius { get; set; }
        public String TipoPessoa { get; set; }
        public String Tipo_Cliente { get; set; }
        public String Status_Prospecto { get; set; }
        public string Color { get; set; }
        public long? Id_Check { get; set; }
        public List<Analise_Login> Radius
        {
            get
            {
                return radius == null ? new List<Analise_Login>() : radius.ToList();
            }
            set
            {
                Radius = value;
            }
        }

    }
    public class Analise_Login
    {
        public long Id_Cliente { get; set; }
        public long Id_PlanoProduto { get; set; }
        public string Conexao { get; set; }
        public long Caixa_FTTH { get; set; }
        public long Porta_FTTH { get; set; }
        public string Mac { get; set; }
        public string Color { get; set; }

    }
    public class Analise_RequestModel
    {
        public Comparativo_QuantidadeEnum? UsaComparativo { get; set; }
        public long? Id_Campanha { get; set; }
        public Sexo? Sexo { get; set; }
        public long? Id_Cliente { get; set; }
        public long? Id_Elemento_Rede { get; set; }
    }
    public class Analise_RequestVendaParamaters
    {
        public long? Id { get; set; }
        public Double? Valor { get; set; }
        public Comparativo_QuantidadeEnum? Comparacao { get; set; }
    }
    public class FilterRecordsAnalise
    {
        public Analise_RequestVendaParamaters Filter_Venda { get; set; }
        private List<short?> Ids_check { get; set; }
        public List<short?> Ids_Check
        {
            get
            {
                return Ids_check == null ? new List<Int16?>() : Ids_check;
            }
            set
            { Ids_check = value; }
        }
        public string Cpf_Cnpj { get; set; }
        public string Nome { get; set; }
        public long? Id_Vendedor { get; set; }
        public long? Meio_Captacao { get; set; }
        public long? Campanha { get; set; }
        public long? Categoria_Cliente { get; set; }
        public long? Uf { get; set; }
        public long? Cidade { get; set; }
        public int? Origem_Lead { get; set; }
        public long? Plano_Produto { get; set; }
        public long? Tecnologia { get; set; }

        public Status_Prospecto? Status_Prospecto { get; set; }
        public Estado_Civil? Estado_Civil { get; set; }
        public Tipo_Pessoa? Tipo_Pessoa { get; set; }
        public Status_Venda? Status_Venda { get; set; }
        public Sexo? Sexo { get; set; }
        public Grau_Instrucao? Grau_Instrucao { get; set; }
        public Status_Elemento? Status_Caixa { get; set; }
        public Situacao_Porta? Status_Porta { get; set; }
    }

}
