using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class TelegramModels
    {
        public class From
        {
            public int id { get; set; }
            public bool is_bot { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string username { get; set; }
            public string language_code { get; set; }
        }

        public class Chat
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string username { get; set; }
            public string type { get; set; }
        }

        public class Message
        {
            public int message_id { get; set; }
            public From from { get; set; }
            public Chat chat { get; set; }
            public int? date { get; set; }
            public string text { get; set; }
            public string caption { get; set; }
            public Document document { get; set; }
            public List<Photo> photo { get; set; }
            public Video video { get; set; }
            public Audio audio { get; set; }
            public Voice voice { get; set; }
        }

        public class Photo
        {
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
            public int width { get; set; }
            public int height { get; set; }

        }

        public class Voice
        {
            public string file_id { get; set; }
            public int file_size { get; set; }
            public string duration { get; set; }
            public string mime_type { get; set; }
        }

        public class Document
        {
            public string file_name { get; set; }
            public string mime_type { get; set; }
            public Thumb thumb { get; set; }
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
        }

        public class Thumb
        {
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Video
        {
            public int duration { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string file_name { get; set; }
            public string mime_type { get; set; }
            public Thumb thumb { get; set; }
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
        }

        public class Root
        {
            public int update_id { get; set; }
            public Message message { get; set; }
        }

        public class Imagem
        {
            public bool ok { get; set; }
            public Result result { get; set; }
        }

        public class Audio
        {
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
            public string file_name { get; set; }
            public Thumb thumb { get; set; }
            public int duration { get; set; }   
        }

        public class Result
        {
            public string file_id { get; set; }
            public string file_unique_id { get; set; }
            public int file_size { get; set; }
            public string file_path { get; set; }
        }
    }
}