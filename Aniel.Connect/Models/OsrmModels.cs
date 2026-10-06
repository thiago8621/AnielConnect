using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Geometry
    {
        public List<List<double>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Intersection
    {
        public int @out { get; set; }
        public List<bool> entry { get; set; }
        public List<int> bearings { get; set; }
        public List<double> location { get; set; }
        public int? @in { get; set; }
    }

    public class Leg
    {
        public List<Step> steps { get; set; }
        public string summary { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public double distance { get; set; }
    }

    public class Maneuver
    {
        public int bearing_after { get; set; }
        public int bearing_before { get; set; }
        public List<double> location { get; set; }
        public string modifier { get; set; }
        public string type { get; set; }
    }

    public class OsrmDistanceMatrixModel
    {
        public string code { get; set; }
        public List<Route> routes { get; set; }
        public List<Waypoint> waypoints { get; set; }
    }

    public class Route
    {
        public Geometry geometry { get; set; }
        public List<Leg> legs { get; set; }
        public string weight_name { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public double distance { get; set; }
    }

    public class Step
    {
        public Geometry geometry { get; set; }
        public Maneuver maneuver { get; set; }
        public string mode { get; set; }
        public string driving_side { get; set; }
        public string name { get; set; }
        public List<Intersection> intersections { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public double distance { get; set; }
    }

    public class Waypoint
    {
        public string hint { get; set; }
        public double distance { get; set; }
        public string name { get; set; }
        public List<double> location { get; set; }
    }



}