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

    public class MessengerModels
    {
        public class Sender
        {
            public string id { get; set; }
        }

        public class Recipient
        {
            public string id { get; set; }
        }

        public class QuickReply
        {
            public string payload { get; set; }
        }

        public class Payload
        {
            public string url { get; set; }
        }

        public class Attachment
        {
            public string type { get; set; }
            public Payload payload { get; set; }
        }


        public class Message
        {
            public string mid { get; set; }
            public string text { get; set; }
            public QuickReply quick_reply { get; set; }
            //public List<Attachment> attachments { get; set; }
            public object attachments { get; set; }
        }

        public class Messaging
        {
            public Sender sender { get; set; }
            public Recipient recipient { get; set; }
            public long timestamp { get; set; }
            public Message message { get; set; }
        }

        public class Postback
        {
            public string mid { get; set; }
            public string title { get; set; }
            public string payload { get; set; }
        }

        public class Value
        {
            public Sender sender { get; set; }
            public Recipient recipient { get; set; }
            public int timestamp { get; set; }
            public Postback postback { get; set; }
        }
      
        public class Entry
        {
            public string id { get; set; }
            public long time { get; set; }
            public List<Messaging> messaging { get; set; }
        }

        public class Root
        {
            public string @object { get; set; }
            public List<Entry> entry { get; set; }
        }

        public class Objeto
        {
            public string objeto { get; set; }
        }

    }
}