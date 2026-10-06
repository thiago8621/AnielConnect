using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Padrao = Sinapse.Model.enumerados.Padrao;

namespace Aniel.Connect.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Change
    {
        public string field { get; set; }
        public Value value { get; set; }
    }

    public class Contact
    {
        public Profile profile { get; set; }
        public string wa_id { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public List<Change> changes { get; set; }
    }
    public class Image
    {
        public string caption { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string id { get; set; }
    }
    public class Audio
    {
        public string caption { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string id { get; set; }
        public bool voice { get; set; }
    }
    public class Document
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string id { get; set; }
    }
    public class Text
    {
        public string body { get; set; }
        public string type { get; set; }
        public string field { get; set; }
    }
    public class Video
    {
        public string caption { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string id { get; set; }
    }

    public class Message
    {
        public string from { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
        public Text text { get; set; }
        public Image image { get; set; }
        public Audio audio { get; set; }
        public Document document { get; set; }
        public Video video { get; set; }
    }

    public class Metadata
    {
        public string display_phone_number { get; set; }
        public string phone_number_id { get; set; }
    }

    public class Profile
    {
        public string name { get; set; }
    }

    public class MetaWhatsAppRequest
    {
        public string @object { get; set; }
        public List<Entry> entry { get; set; }
    }

    public class Value
    {
        public string messaging_product { get; set; }
        public Metadata metadata { get; set; }
        public List<Contact> contacts { get; set; }
        public List<Message> messages { get; set; }
    }
}