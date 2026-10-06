using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http;
using System;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ChecklistApiRequest : ApiRequest
    {
        public string NumObraOriginal { get; set; }
        public string NumObra { get; set; }
        public string NumTarefa { get; set; }
        public string CodCt { get; set; }
        public string Projeto { get; set; }
        //public int NumDoc { get; set; }        
        public string NumOtOr { get; set; }        
    }
    public class ResponseChecklist 
    {
        public long Id { get; set; }
        private List<ResponseChecklistItem> _perguntas;
        public List<ResponseChecklistItem> Perguntas { 
            get 
            {
                if (_perguntas == null)
                {
                    return new List<ResponseChecklistItem>(); 
                }
                return _perguntas;
            } set { _perguntas = value; } }
    }
    public class ResponseChecklistItem 
    {
        public long Id { get; set; }        
        public bool Respondido { get; set; }
        public List<ResponseChecklistItemResposta> OpcoesResposta { get; set; }        
    }
    public class ResponseChecklistItemResposta 
    {
        public long Id { get; set; }
        public long IdItem { get; set; }
        public string Tipo { get; set; }
        public object Valor { get; set; }
        public bool IsResposta { get; set; }
    }
    public class ArquivoChecklist
    {
        public long? Id { get; set; }
        public string Descricao { get; set; }
        public byte[] Arquivo { get; set; }
        public String Tipo { get; set; }
    }
    public class ArquivoChecklistResult : IHttpActionResult
    {
        MemoryStream arquivoStuff;
        string nomeDoArquivo, msg;
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
        HttpStatusCode httpStatusCode;

        public ArquivoChecklistResult(MemoryStream data, HttpRequestMessage request, string filename, HttpStatusCode statusCode = HttpStatusCode.OK, string mensagem = "")
        {
            arquivoStuff = data;
            httpRequestMessage = request;
            nomeDoArquivo = filename;
            httpStatusCode = statusCode;
            msg = mensagem;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (httpStatusCode != HttpStatusCode.OK)
            {
                var obj = new { mensagem = msg, ok = false };

                httpResponseMessage = httpRequestMessage.CreateResponse(httpStatusCode, obj, new HttpConfiguration());
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }
            else
            {
                httpResponseMessage = httpRequestMessage.CreateResponse(httpStatusCode);
                httpResponseMessage.Content = new StreamContent(arquivoStuff);
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = nomeDoArquivo;
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }

            return Task.FromResult(httpResponseMessage);
        }
    }
}