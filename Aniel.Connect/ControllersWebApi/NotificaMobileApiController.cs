using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class NotificaMobileApiController : BaseApiController
    {
        private ISession session;
        public NotificaMobileApiController(ISession session)
        {
            this.session = session;
        }

        public async Task Notifica(long id_funcionario_dest,String label, String titulo, String texto, String intent)
        {
            try
            {
                string ApplicationID = "AIzaSyAQxCpeLifuFejr9FP8ExWgU9wO9vmcSwU";
                string Dest_Device_ID = (await new Conexao<IdentityUser>(session).selectFirst(x=> x.Funcionario.Id == id_funcionario_dest )).tokenPremier;
                
                if (!String.IsNullOrEmpty(Dest_Device_ID))
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new {
                        to = Dest_Device_ID,
                        data = new {
                            tag = label,
                            title = titulo,
                            body = texto,
                            click_action = intent

                        }
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                    tRequest.Headers.Add(string.Format("Authorization: key={0}", ApplicationID));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    String str = sResponseFromServer;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

    }
}