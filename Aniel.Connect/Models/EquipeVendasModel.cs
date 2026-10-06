using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class EquipeVendasListaModel
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
    }

    public class EquipeVendasModel
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }
    }

    public class EquipeVendasDatatableRequest : DataTableRequest
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
    }

    public class EquipeVendasSupervisorModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public long? Id_Supervisor { get; set; }
    }

    public class EquipeVendasCadastroModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }

        private List<SupervisoresListaModel> _supervisoreslista;
        public List<SupervisoresListaModel> SupervisoresLista
        {
            get
            {
                if (_supervisoreslista == null)
                    _supervisoreslista = new List<SupervisoresListaModel>();
                return _supervisoreslista;
            }
            set
            {
                _supervisoreslista = value;
            }
        }

        public class SupervisoresListaModel
        {
            public long Id { get; set; }
            public long Id_Supervisor { get; set; }
            public string Supervisor { get; set; }
        }

    }

}