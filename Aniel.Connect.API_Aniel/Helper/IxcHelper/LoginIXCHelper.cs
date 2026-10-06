using Aniel.Connect.API_Aniel.Models;
using log4net;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Helper.IxcHelper
{
    public class LoginIXCHelper : HttpHelper
    {


        private ResponsePostIXC Post(Config_Aniel_Integrator obj, LoginIXC objToPost)
        {
            var retorno = new ResponsePostIXC();

            try
            {
                retorno = JsonService.DeserializeObject<ResponsePostIXC>(base.Post(obj.URL, obj.Token, JsonService.SerializeObject(objToPost)));
            }
            catch (Exception)
            {
                throw new Exception("Erro ao se conectar nas apis do IXC");
            }
            return retorno;
        }
        private ResponsePostIXC Post(Config_Aniel_Integrator obj, ClienteFibraIXC objToPost)
        {
            var retorno = new ResponsePostIXC();

            try
            {
                retorno = JsonService.DeserializeObject<ResponsePostIXC>(base.Post(obj.URL, obj.Token, JsonService.SerializeObject(objToPost)));
            }
            catch (Exception)
            {
                throw new Exception("Erro ao se conectar nas apis do IXC");
            }
            return retorno;
        }
        private ResponsePostIXC Get(Config_Aniel_Integrator obj, LoginIXC objToPost)
        {
            var retorno = new ResponsePostIXC();
            try
            {
                retorno = JsonService.DeserializeObject<ResponsePostIXC>(base.Get(obj.URL, obj.Token, "", ""));
            }
            catch (Exception)
            {
                throw new Exception("Erro ao se conectar nas apis do IXC");
            }
            return retorno;
        }
        public ResponsePostIXC AtualizarCaixaPorta(Config_Aniel_Integrator obj, LoginIXC objToPost)
        {
            var retorno = new ResponsePostIXC();
            string responseApi = this.Put(obj.URL + "radusuarios", obj.Token, JsonService.SerializeObject(objToPost), objToPost.id);
            retorno = JsonService.DeserializeObject<ResponsePostIXC>(responseApi);
            LogManager.GetLogger("Aniel.Connect.API_Aniel.IXC").Error($"[IXC - AtualizarCaixaPorta] - Json Request: {JsonService.SerializeObject(objToPost)} \n  Json Response: {responseApi} {Environment.NewLine} {Environment.NewLine}");
            return retorno;
        }
        public ResponsePostIXC AtualizarClienteFibra(Config_Aniel_Integrator obj, ClienteFibraIXC objToPost)
        {
            var retorno = new ResponsePostIXC();

            string responseApi = this.Put(obj.URL + "radpop_radio_cliente_fibra", obj.Token, JsonService.SerializeObject(objToPost), objToPost.id);
            retorno = JsonService.DeserializeObject<ResponsePostIXC>(responseApi);
            LogManager.GetLogger("Aniel.Connect.API_Aniel.IXC").Error($"[IXC - AtualizarClienteFibra] - Json Request: {JsonService.SerializeObject(objToPost)} \n  Json Response: {responseApi} {Environment.NewLine}  {Environment.NewLine}");
            return retorno;
        }
        public ResponseIXC<LoginIXC> GetLogin(string id, Config_Aniel_Integrator config)
        {
            try
            {
                var url = config.URL + "radusuarios";

                var request = new RequestIXC()
                {
                    qtype = "radusuarios.id",
                    query = $"{id}",
                    oper = "=",
                    page = "1",
                    rp = "20",
                    sortname = "radusuarios.id",
                    sortorder = "desc"
                };

                var requestStr = JsonService.SerializeObject(request);
                var responseStr = Get(url, config.Token, requestStr, "listar");

                LogManager.GetLogger("Aniel.Connect.API_Aniel.IXC").Error($"[IXC - GetLogin] - Json Request: {requestStr} \n  Json Response: {responseStr} {Environment.NewLine} {Environment.NewLine}");

                return JsonService.DeserializeObject<ResponseIXC<LoginIXC>>(responseStr);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IXC: {ex.Message}");
            }
        }
        public ResponseIXC<ClienteFibraIXC> GetClienteFibra(string id, Config_Aniel_Integrator config)
        {
            try
            {
                var url = config.URL + "radpop_radio_cliente_fibra";

                var request = new RequestIXC()
                {
                    qtype = "id_login",
                    query = $"{id}",
                    oper = "=",
                    page = "1",
                    rp = "20",
                    sortname = "radpop_radio_cliente_fibra.id",
                    sortorder = "desc"
                };

                var requestStr = JsonService.SerializeObject(request);
                var responseStr = Get(url, config.Token, requestStr, "listar");

                LogManager.GetLogger("Aniel.Connect.API_Aniel.IXC").Error($"[IXC - GetLogin] - Json Request: {requestStr} \n  Json Response: {responseStr} {Environment.NewLine} {Environment.NewLine}");

                return JsonService.DeserializeObject<ResponseIXC<ClienteFibraIXC>>(responseStr);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IXC: {ex.Message}");
            }
        }
        public ResponseIXC<DadosCaixaResponseModel> GetClienteFibraByCaixa(string idCaixa, Config_Aniel_Integrator config)
        {
            try
            {
                var url = config.URL + "radpop_radio_cliente_fibra";

                var request = new RequestIXC()
                {
                    qtype = "id_caixa_ftth",
                    query = $"{idCaixa}",
                    oper = "=",
                    page = "1",
                    rp = "900",
                    sortname = "radpop_radio_cliente_fibra.id",
                    sortorder = "desc"
                };

                var requestStr = JsonService.SerializeObject(request);
                var responseStr = Get(url, config.Token, requestStr, "listar");

                LogManager.GetLogger("Aniel.Connect.API_Aniel.IXC").Error($"[IXC - GetClienteFibraByCaixa] - Json Request: {requestStr} \n  Json Response: {responseStr} {Environment.NewLine} {Environment.NewLine}");

                return JsonService.DeserializeObject<ResponseIXC<DadosCaixaResponseModel>>(responseStr);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IXC: {ex.Message}");
            }
        }




    }

    public class RequestIXC
    {
        public string qtype { get; set; }
        public string query { get; set; }
        public string oper { get; set; }
        public string page { get; set; }
        public string rp { get; set; }
        public string sortname { get; set; }
        public string sortorder { get; set; }
    }
    public class ResponseIXC<T> where T : class
    {
        public string page { get; set; }
        public string total { get; set; }
        public List<T> registros { get; set; }
    }
    public class ResponsePostIXC
    {
        public string type { get; set; }
        public string message { get; set; }
        public string id { get; set; }
    }
}