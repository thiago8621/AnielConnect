using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Aniel.Connect.Models
{
    public class WorkflowTesteModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual bool Status { get; set; }
        public virtual Tipo_Pessoa TipoPessoa { get; set; }
        public virtual String Categoria { get; set; }
        public virtual String Captacao { get; set; }
        public virtual UF? Uf { get; set; }
        public virtual Int64? Cidade { get; set; }
        public virtual Int32 Prazo { get; set; }
        public virtual Tipo_SLA Tipodias { get; set; }
    }

    public class Workflow_VendasAtividadeTestModel
    {
        public virtual Int64 Id { get; set; }
        public virtual bool Ativ_AgendaTarefa { get; set; }
        public virtual bool Ativ_Status { get; set; }
        public virtual String Ativ_Descricao { get; set; }
        public virtual Int32 Ativ_Prazo { get; set; }
        public virtual Tipo_SLA Ativ_Tipodias { get; set; }
        public virtual string Ativ_StatusVenda { get; set; }
        public virtual Atividade_Decisoria Ativ_Decisao { get; set; }
        public virtual Tipo_Atividade_Decisoria Ativ_TipoDecisao { get; set; }
        public virtual Int32 Ativ_NAtivAprovada { get; set; }
        public virtual Int32 Ativ_NAtivReprovada { get; set; }
    }

    public class ResultSetViewModel
    {
        [Display(Name = "Result")]
        public List<WorkflowTesteModel> ResultSet { get; set; }

    }

    public class CommonViewModel
    {
        public Workflow_VendasAtividadeTestModel AccountVM { get; set; }
        public ResultSetViewModel ResultSetVM { get; set; }
    }



    public class Workflow_VendasListTestModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }

    public class Workflow_VendasTestDataTableResquest : DataTableRequest
    {

    }
}