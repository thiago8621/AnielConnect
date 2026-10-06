using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class SigaMeModels
    {
    }

    public class SigaMeCadastroModel : IViewModel<long>
    {
        public long Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public bool DefinirHorario { get; set; }
        public bool Segunda { get; set; }
        public bool Terca { get; set; }
        public bool Quarta { get; set; }
        public bool Quinta { get; set; }
        public bool Sexta { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public DateTime HorarioInicial { get; set; }
        public DateTime HorarioFinal { get; set; }
        public long idPlano { get; set; }

        private List<SigaMeListModel> _sigamelista;
        public virtual List<SigaMeListModel> SigaMeLista
        {
            get
            {
                if (_sigamelista == null)
                    _sigamelista = new List<SigaMeListModel>();
                return _sigamelista;
            }
            set
            {
                _sigamelista = value;
            }
        }
    }

    public class SigaMeListModel
    {
        public long Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public bool DefinirHorario { get; set; }
        public bool Segunda { get; set; }
        public bool Terca { get; set; }
        public bool Quarta { get; set; }
        public bool Quinta { get; set; }
        public bool Sexta { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public string HorarioInicial { get; set; }
        public string HorarioFinal { get; set; }
        public int idPlano { get; set; }
    }

    public class DatatableRequestSigaMe : DataTableRequest
    {
        public long Id { get; set; }
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public bool DefinirHorario { get; set; }
        public bool Segunda { get; set; }
        public bool Terca { get; set; }
        public bool Quarta { get; set; }
        public bool Quinta { get; set; }
        public bool Sexta { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public DateTime HorarioInicial { get; set; }
        public DateTime HorarioFinal { get; set; }
        public int idPlano { get; set; }
    }
}