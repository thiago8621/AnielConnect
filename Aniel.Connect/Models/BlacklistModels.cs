using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;


namespace Aniel.Connect.Models
{
    public class BlacklistModels
    {
    }

    public class BlacklistCadastroModel : IViewModel<long>
    {
        public long Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public string Observacao { get; set; }
        public long IdPlano { get; set; }

        private List<BlacklistModelLista> _blacklistlista;
        public virtual List<BlacklistModelLista> Lista_BlackLists
        {
            get
            {
                if (_blacklistlista == null)
                    _blacklistlista = new List<BlacklistModelLista>();
                return _blacklistlista;
            }
            set
            {
                _blacklistlista = value;
            }
        }

    }

    public class BlacklistDatatableRequest : DataTableRequest
    {
        public int Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public string Observacao { get; set; }
        public Plano IdPlano { get; set; }
    } 
    
    public class BlacklistModelLista
    {
        public long Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public string Observacao { get; set; }
    }
}