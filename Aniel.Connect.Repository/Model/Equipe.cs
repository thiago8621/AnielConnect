using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model
{
    public class EquipeAuxiliaresModel
    {
        public string CodCt { get; set; }
        public string Projeto { get; set; }
        public string Num_Obra { get; set; }
        public string Equipe { get; set; }
        public string Nome { get; set; }
        public string Tarefa { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string TipoServico { get; set; }
        public string Supervisor { get; set; }
        public string TipoData { get; set; }
        public Int32? Num_Doc { get; set; }
        public string Usuario { get; set; }
    }

    public class EquipeAuxiliaresModelRequest : DataTableRequest
    {
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Num_Obra { get; set; }
        public virtual string Num_Doc { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string Equipe { get; set; }
        public string Supervisor { get; set; }
        public string CodTipoServ { get; set; }
        public string TipoData { get; set; }
    }

}
