using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel
{
    /// <summary>
    /// Request da API
    /// </summary>
    public class ApiRequest
    {
        public ApiRequest()
        {
            settings = new ApiSettings();
        }

        /// <summary>
        /// API settings
        /// </summary>
        public ApiSettings settings { get; set; }
    }

    public interface IApiValidate
    {
        /// <summary>
        /// Usuário da API
        /// </summary>
        string user { get; set; }
        /// <summary>
        /// Senha da API
        /// </summary>
        string password { get; set; }
        /// <summary>
        /// Token da API
        /// </summary>
        string token { get; set; }
    }

    public class ApiOrder
    {
        /// <summary>
        /// Colluns ID
        /// </summary>
        public int? column { get; set; }
        /// <summary>
        /// Direction (Ascending or Descending)
        /// </summary>
        public string dir { get; set; }
    }

    public class ApiResponse<T>
    {
        public int draw { get; set; }
        public int? recordsFiltered { get; set; }
        public int? quantidadeTotalRegistros { get; set; }
        public IEnumerable<T> data { get; set; }
        public string error { get; set; }
    }

    public class ApiResponseObj<T>
    {
        public bool ok { get; set; }
        public T data { get; set; }
        public string error { get; set; }
    }

    

    public class ApiSettings : IApiValidate
    {
        public ApiSettings()
        {
            order = new ApiOrder();
        }

        /// <summary>
        /// It will start from
        /// </summary>
        [Required]
        public int start { get; set; }
        /// <summary>
        /// How many records will be draw
        /// </summary>
        [Required]
        public int length { get; set; }
        /// <summary>
        /// Search for a specific word
        /// </summary>
        
        public string search { get; set; }
        /// <summary>
        /// Sort by
        /// </summary>
        public ApiOrder order { get; set; }
        /// <summary>
        /// Usuário da API
        /// </summary>
        [Required]
        public string user { get; set; }
        /// <summary>
        /// Senha da API
        /// </summary>
        [Required]
        public string password { get; set; }
        /// <summary>
        /// Token da API
        /// </summary>
        [Required]
        public string token { get; set; }
        
    }
    public class JsonApiRequest : ApiRequest 
    {
        public string JsonParams { get; set; }
    }
    public class ObjApiRequest<T> : ApiRequest 
    {
        public T Obj { get; set; }
        public IntegracaoExterna? Tipo_Integracao { get; set; }
    }

    public class ApiResponseScheduling<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }


}
