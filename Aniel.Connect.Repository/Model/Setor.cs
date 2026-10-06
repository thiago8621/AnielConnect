namespace Aniel.Connect.Repository.Model
{
    public class SetorIXCResponse
    {
        public string id { get; set; }
        public string descricao { get; set; }
        public string ativo { get; set; }
        public string id_depto { get; set; }
        public string ultima_atualizacao { get; set; }
        public string operador_ultima_atualizacao { get; set; }
    }

    public class AssuntoIXCResponse
    {
        public string id { get; set; }
        public string assunto { get; set; }
        public string id_processo { get; set; }
    }
}
