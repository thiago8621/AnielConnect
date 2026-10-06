using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade.Interface;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.T5.ElementoRede;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade
{
    public class ConsultaViabilidadeDentroRaio : IEtapaViabilidade
    {        
        private double? _raio;
        private IElementoRedeAnielRepository _repository;

        public EtapasValidacaoConsultaViabilidadeEnum Etapa
        {
            get
            {
                return EtapasValidacaoConsultaViabilidadeEnum.DistanciaRaio;
            }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long? ClientePlanoProduto { get; set; }

        public ConsultaViabilidadeDentroRaio(double? latitude, double? longitude, long? clientePlanoProduto, double? raio, IElementoRedeAnielRepository repository)
        {
            Latitude = latitude.Value;
            Longitude = longitude.Value;
            ClientePlanoProduto = clientePlanoProduto;
            _raio = raio;
            _repository = repository;

        }

        public GetApiRetorno<List<Elemento_Rede>> ExecutarConsulta()
        {
            var retorno = new GetApiRetorno<List<Elemento_Rede>>
            {
                Objeto = new List<Elemento_Rede>()
            };
            try 
            {
                double kmInLongitudeDegree = 111.320 * Math.Cos(Convert.ToDouble(this.Latitude) / 180.0 * Math.PI);
                double? deltaLat = _raio / 111.1;
                double? deltaLong = _raio / kmInLongitudeDegree;

                double? minLat = this.Latitude - deltaLat;
                double? maxLat = this.Latitude + deltaLat;
                double? minLong = this.Longitude - deltaLong;
                double? maxLong = this.Longitude + deltaLong;

                var caixasRaio = (this._repository.GetAll().Where(p =>
                       (p.Latitude >= minLat) &&
                       (p.Latitude <= maxLat) &&
                       (p.Longitude >= minLong) &&
                       (p.Longitude <= maxLong) &&
                       p.Capacidade > 0
                   )).ToList();

                retorno.Ok = caixasRaio.Count > 0;
                retorno.Objeto = caixasRaio;
            }
            catch (Exception exception) 
            {
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;

                if (exception.InnerException != null)
                    retorno.InnerException = exception.InnerException.Message;
            }

            return retorno;
        }
    }
}
