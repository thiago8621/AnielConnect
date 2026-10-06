using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Model.ImportacaoAreaCobertura
{
    /// <summary>
    ///     Coordenadas da área de cobertura
    /// </summary>
    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Coordinates(Latitude: {0}, Longitude: {1}, Altitude: {2})",
                Latitude,
                Longitude,
                Altitude
            );
        }
    }

    /// <summary>
    ///     Dados do polígono que representa a área de cobertura no mapa
    /// </summary>
    public class AreaCoberturaImportacao
    {
        private IEnumerable<Coordinates> _coordinates;

        /// <summary>
        ///     Nome da área de cobertura
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Coordenadas do polígono que representa a área de cobertura
        /// </summary>
        public IEnumerable<Coordinates> Coordinates
        {
            get
            {
                if (this._coordinates == null)
                    this._coordinates = new List<Coordinates>();

                return this._coordinates;
            }

            set
            {
                this._coordinates = value;
            }
        }

        /// <summary>
        ///     Cor RGBA no formato hexadecimal para a coloração do polígono no mapa (ex.: #AABBCCDD)
        /// </summary>
        public string Color { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Polygon(Name: {0}, Coordinates: [{1}], Color: {2})",
                Name,
                Coordinates.Count(),
                Color
            );
        }
    }
}
