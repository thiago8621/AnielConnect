using Aniel.Connect.ExecucaoService.Data.Model;
using Newtonsoft.Json;
using System.IO;

namespace Aniel.Connect.ExecucaoService.Data
{
    public class JsonServiceDataAccessor : IServiceDataAccessor
    {
        private readonly string DATA_FILE_PATH = $@"{CONSTANTES.CaminhoFisico}\data.json";
        private readonly JsonSerializer _serializer;

        public JsonServiceDataAccessor(JsonSerializer serializer)
        {
            this._serializer = serializer;
        }

        public ServiceData GetServiceData()
        {
            if (!File.Exists(this.DATA_FILE_PATH))
            {
                return null;
            }

            using (var streamReader = new StreamReader(this.DATA_FILE_PATH))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return this._serializer.Deserialize<ServiceData>(jsonReader);
            }
        }

        public void WriteServiceData(ServiceData data)
        {
            using (var streamWriter = new StreamWriter(this.DATA_FILE_PATH))
            {
                this._serializer.Serialize(streamWriter, data);
            }
        }
    }
}
