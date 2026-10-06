using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class TwilioModels
    {
        public class MediaContentTwilio
        {
            public string sid { get; set; }
            public string service_sid { get; set; }
            public string date_created { get; set; }
            public string date_upload_updated { get; set; }
            public string date_updated { get; set; }
            public LinksMedia links { get; set; }
            public Int64? size { get; set; }
            public string content_type { get; set; }
            public string filename { get; set; }
            public string author { get; set; }
            public string category { get; set; }
            public string message_sid { get; set; }
            public string channel_sid { get; set; }
            public string url { get; set; }
            public bool is_multipart_upstream { get; set; }

        }

        public class LinksMedia
        {
            public string content { get; set; }
            public string content_direct_temporary { get; set; }
        }
    }
}