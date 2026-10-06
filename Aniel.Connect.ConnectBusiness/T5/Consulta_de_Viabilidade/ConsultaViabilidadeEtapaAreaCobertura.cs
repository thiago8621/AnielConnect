using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade.Interface;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Repository.T5.AreaCoberturaMongo;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.T5.Model.nhibernate;
using NHibernate.Criterion;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using GeoAPI.Geometries;

namespace Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade
{
    public class ConsultaViabilidadeAreaCobertura : IEtapaViabilidade
    {        
        private IAreaCoberturaMongoRepository _repositoryMongoAreaCobertura;        

        public ConsultaViabilidadeAreaCobertura(double? latitude, double? longitude, long? clientePlanoProduto, IAreaCoberturaMongoRepository repositoryMongoAreaCobertura)
        {
            Latitude = latitude.Value;
            Longitude = longitude.Value;
            ClientePlanoProduto = clientePlanoProduto;
            _repositoryMongoAreaCobertura = repositoryMongoAreaCobertura;                       
        }

        public EtapasValidacaoConsultaViabilidadeEnum Etapa
        {
            get
            {
                return EtapasValidacaoConsultaViabilidadeEnum.AreaCobertura;
            }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long? ClientePlanoProduto { get; set; }
        public List<long> CaixasEtapasAnteriores { get; set; }

        public GetApiRetorno<List<Elemento_Rede>> ExecutarConsulta()
        {
            var retorno = new GetApiRetorno<List<Elemento_Rede>>
            {
                Objeto = new List<Elemento_Rede>()
            };
            
            try 
            {
                using (var pesquisaAreas = new GeographyPattern())
                {
                    var location = new Core.Models.Location(this.Latitude, this.Longitude);
                    var areas = this._repositoryMongoAreaCobertura.GetAll().ToList();

                    GeometryFactory factory = new GeometryFactory();
                    Dictionary<long, Polygon> areasCoberturaDesenhado = new Dictionary<long,Polygon>();

                    Coordinate pointCoordinate = new Coordinate(this.Latitude, this.Longitude);
                    Point point = (Point)factory.CreatePoint(pointCoordinate);                                      

                    foreach (var area in areas)
                    {                        
                        var coordenadas = new List<Coordinate>();                                                
                        if (area != null && area.Coordenadas != null)
                        {
                            for (int i = 0; i < area.Coordenadas.Count; i++)
                            {
                                List<double?> coordenada = area.Coordenadas[i];
                                for (var latlong = 0; latlong < coordenada.Count(); latlong++)
                                {
                                    double locLat = coordenada[latlong].HasValue ?   Convert.ToDouble(string.Format("{0:F7}", coordenada[latlong].Value)): 0,
                                            locLong = coordenada[latlong].HasValue ? Convert.ToDouble(string.Format("{0:F7}", coordenada[++latlong].Value)): 0;
                                    var recordCoordenada = new Coordinate(locLat, locLong);
                                    if (!coordenadas.Contains(recordCoordenada)) 
                                    {
                                        coordenadas.Add(recordCoordenada);
                                    }
                                }                               
                            }

                            if (coordenadas.Count > 0 && coordenadas.LastOrDefault() != coordenadas.FirstOrDefault()) 
                            {
                                coordenadas.Add(coordenadas.First());
                            }
                        }                        

                        LinearRing ring = new LinearRing(coordenadas.ToArray());
                        Polygon polygon = new Polygon(ring);

                        areasCoberturaDesenhado.Add(area.Id, polygon);                        
                    }
                    if (areasCoberturaDesenhado.Any(p => p.Value.Contains(point))) 
                    {
                        var potentialRecords = areasCoberturaDesenhado.Where(p => p.Value.Contains(point)).ToDictionary(p => p.Key, p=> p.Value);
                        retorno.Ok = true;
                    }                    
                }
            }
            catch (Exception ex) 
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }
    }
}
