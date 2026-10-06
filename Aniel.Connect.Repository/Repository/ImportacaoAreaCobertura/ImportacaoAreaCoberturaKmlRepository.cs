using Aniel.Connect.Repository.Model.ImportacaoAreaCobertura;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Aniel.Connect.Repository.Repository.ImportacaoAreaCobertura
{
    /// <summary>
    ///     <para>
    ///         Classe para extrair dados dos polígonos que representam áreas de coberturas
    ///         de um arquivo no formato KML.
    ///     </para>
    ///     
    ///     <para>
    ///         Esse arquivo KML é um XML com campos especiais obtido ao extrair um arquivo do
    ///         tipo KMZ, um arquivo ZIP que contém o arquivo KML.
    ///     </para>
    ///     
    ///     <para>
    ///         Referência do formato do arquivo: <see href="https://developers.google.com/kml/documentation/kmlreference" />
    ///     </para>
    /// </summary>
    public class ImportacaoAreaCoberturaKmlRepository : IImportacaoAreaCoberturaRepository
    {
        private readonly XDocument _kmlDocument = null;

        /// <summary>
        ///     Formato utilizado para converter as strings de coordenada para double
        ///     pegando as casas decimais corretamente
        /// </summary>
        private readonly CultureInfo _coordinateFormattingCulture = new CultureInfo("en-US");

        /// <summary>
        ///     Construtor para criar um objeto de importação de áreas de cobertura que extrai 
        ///     os dados das áreas a partir de um arquivo KML.
        /// </summary>
        /// <param name="kmlDocument">Arquivo KML processado pelo <see cref="System.Xml.Linq.XDocument.Load(string)"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ImportacaoAreaCoberturaKmlRepository(XDocument kmlDocument)
        {
            if (kmlDocument == null)
                throw new ArgumentNullException(nameof(kmlDocument));

            this._kmlDocument = kmlDocument;
        }

        /// <summary>
        ///     Retorna os elementos do arquivo KML com os dados dos polígonos
        ///     que representam as áreas de cobertura.
        ///     
        ///     <para>
        ///         Referência para tags do arquivo KML usados nesse método:
        ///     </para>
        ///     
        ///     <list type="bullet">
        ///         <item>
        ///             <term>
        ///                 Polygon
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#polygon"/>
        ///             </description>            
        ///         </item>
        ///     </list>
        /// </summary>
        /// </summary>
        private IEnumerable<XElement> GetPolygonKmlElements()
        {
            return this._kmlDocument
                .Descendants()
                ?.Where(e => e.Name.LocalName == "Polygon")
                ?.ToList();
        }

        /// <summary>
        ///     Retorna o elemento do arquivo KML que contém as coordenadas 
        ///     dos vértices do polígono.
        ///     
        ///     <para>
        ///         Referência para tags do arquivo KML usados nesse método:
        ///     </para>
        ///     
        ///     <list type="bullet">
        ///         <item>
        ///             <term>
        ///                 Polygon
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#polygon"/>
        ///             </description>            
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 LinearRing (contém o elemento com as coordenadas)
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#linearring"/>
        ///             </description>            
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="kmlPolygon">Elemento do polígono no arquivo KML</param>
        private XElement GetCoordinatesFromPolygonKmlElement(XElement kmlPolygon)
        {
            XElement coordinateKml = null;

            XElement outerBoundary =
                kmlPolygon
                    .Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "outerBoundaryIs");

            if (outerBoundary != null)
            {
                XElement linearRing =
                    outerBoundary
                        .Descendants()
                        .FirstOrDefault(e => e.Name.LocalName == "LinearRing");

                if (linearRing != null)
                {
                    XElement coordinates =
                        linearRing
                            .Descendants()
                            .FirstOrDefault(e => e.Name.LocalName == "coordinates");

                    coordinateKml = coordinates;
                }
            }

            return coordinateKml;
        }

        /// <summary>
        ///     Pega a cor associada à um Style definido no arquivo KML que
        ///     possui o ID informado no parâmetro
        ///     
        ///     <para>
        ///         Referência para tags do arquivo KML usados nesse método:
        ///     </para>
        ///     
        ///     <list type="bullet">
        ///         <item>
        ///             <term>
        ///                 Style
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#style"/>
        ///             </description>            
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 StyleMap (o elemento Pair está incluso na documentação do StyleMap)
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#stylemap"/>
        ///             </description>            
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 PolyStyle
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#polystyle"/>
        ///             </description>            
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="styleId">ID do elemento Style no arquivo KML que contém a cor</param>
        private string GetColorFromStyleId(string styleId)
        {
            string color = null;

            XElement styleKmlElement =
                this._kmlDocument
                    .Descendants()
                    .FirstOrDefault(e => e.Attribute("id")?.Value == styleId);

            if (styleKmlElement != null)
            {
                if (styleKmlElement.Name.LocalName == "StyleMap")
                {
                    // Objeto Pair que contem o ID do Style com a cor utilizada pelo StyleMap
                    XElement styleMapPairKmlElement =
                        styleKmlElement
                            .Descendants()
                            .FirstOrDefault(e => e.Name.LocalName == "key" && e.Value == "normal")
                            ?.Parent;

                    if (styleMapPairKmlElement != null)
                    {
                        XElement pairStyleUrlKmlElement =
                            styleMapPairKmlElement
                                .Descendants()
                                .FirstOrDefault(e => e.Name.LocalName == "styleUrl");

                        if (pairStyleUrlKmlElement != null && !String.IsNullOrEmpty(pairStyleUrlKmlElement.Value))
                        {
                            string styleMapTrueStyleId = pairStyleUrlKmlElement.Value.Replace("#", "");

                            styleKmlElement =
                                this._kmlDocument
                                    .Descendants()
                                    .FirstOrDefault(e => e.Attribute("id")?.Value == styleMapTrueStyleId);
                        }
                    }
                }

                if (styleKmlElement != null)
                {
                    XElement polystyleKmlElement =
                        styleKmlElement
                            .Descendants()
                            .FirstOrDefault(e => e.Name.LocalName == "PolyStyle");

                    if (polystyleKmlElement != null)
                    {
                        color =
                            polystyleKmlElement
                                .Descendants()
                                .FirstOrDefault(e => e.Name.LocalName == "color")
                                ?.Value;

                        if (!String.IsNullOrEmpty(color) && !color.Contains("#"))
                        {
                            color = "#" + color;
                        }
                    }
                }
            }

            return color;
        }

        /// <summary>
        ///     Retorna dados dos polígonos presentes no arquivo KMZ
        ///     <para>
        ///         Referência para tags do arquivo KML usados nesse método:
        ///     </para>
        ///     
        ///     <list type="bullet">
        ///         <item>
        ///             <term>
        ///                 Placemark
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#placemark"/>
        ///             </description>            
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 Polygon
        ///             </term>        
        ///             <description>
        ///                 <see href="https://developers.google.com/kml/documentation/kmlreference#polygon"/>
        ///             </description>            
        ///         </item>
        ///     </list>
        /// </summary>
        public IEnumerable<AreaCoberturaImportacao> GetDadosAreaCobertura()
        {
            IList<AreaCoberturaImportacao> polygons = new List<AreaCoberturaImportacao>();

            IEnumerable<XElement> kmlPolygons = this.GetPolygonKmlElements();

            if (kmlPolygons != null)
            {
                foreach (XElement kmlPolygon in kmlPolygons)
                {
                    var polygon = new AreaCoberturaImportacao();

                    IList<Coordinates> polyCoordinates = new List<Coordinates>();

                    XElement coordinatesKml = this.GetCoordinatesFromPolygonKmlElement(kmlPolygon);

                    if (!String.IsNullOrEmpty(coordinatesKml.Value))
                    {
                        IEnumerable<string> coordinatesStr = coordinatesKml.Value.Trim().Split(' ');

                        foreach (string coordinateStr in coordinatesStr)
                        {
                            string[] coordsArray = coordinateStr.Split(',');

                            // O elemento <coordinates> do KML traz as coordenadas no formato longitude, latitude, altitude (opcional)
                            var coordinates = new Coordinates
                            {
                                Longitude = Convert.ToDouble(coordsArray[0], this._coordinateFormattingCulture),
                                Latitude = Convert.ToDouble(coordsArray[1], this._coordinateFormattingCulture),
                                Altitude = Convert.ToDouble(coordsArray[2], this._coordinateFormattingCulture)
                            };

                            polyCoordinates.Add(coordinates);
                        }
                    }

                    polygon.Coordinates = polyCoordinates;

                    XElement kmlPlacemarkElement = kmlPolygon.Parent;

                    polygon.Name =
                        kmlPlacemarkElement
                            .Descendants()
                            .FirstOrDefault(e => e.Name.LocalName == "name")
                            ?.Value;

                    XElement kmlPlacemarkStyleUrlElement =
                        kmlPlacemarkElement
                            .Descendants()
                            .FirstOrDefault(e => e.Name.LocalName == "styleUrl");

                    if (kmlPlacemarkStyleUrlElement != null && !String.IsNullOrEmpty(kmlPlacemarkStyleUrlElement.Value))
                    {
                        string styleUrl = kmlPlacemarkStyleUrlElement.Value.Replace("#", "");
                        polygon.Color = this.GetColorFromStyleId(styleUrl);
                    }

                    polygons.Add(polygon);
                }
            }

            return polygons;
        }
    } 
}
