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
    public class Workflow_VendasCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Tipo_Pessoa? TipoPessoa { get; set; }
        public virtual String Categoria { get; set; }
        public virtual String Captacao { get; set; }
        public virtual String Horario_Atendimento { get; set; }
        public virtual Boolean Definir_Responsavel { get; set; }
        public virtual String Setor_Departamento { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual UF? Uf { get; set; }
        public virtual Int64? Cidade { get; set; }
        public virtual Int64 Prazo { get; set; }
        public virtual Tipo_SLA? Tipodias { get; set; }

        public virtual bool Ativ_AgendaTarefa { get; set; }
        public virtual Boolean Ativ_Status { get; set; }
        public virtual String Ativ_Descricao { get; set; }
        public virtual String Ativ_Prazo { get; set; }
        public virtual Tipo_HORA Ativ_Tipodias { get; set; }
        public virtual string Ativ_StatusVenda { get; set; }
        public virtual Atividade_Decisoria Ativ_Decisao { get; set; }
        //public virtual Tipo_Atividade_Decisoria Ativ_TipoDecisao { get; set; }
        public virtual Int32 Ativ_NAtivAprovada { get; set; }
        public virtual Int32 Ativ_NAtivReprovada { get; set; }


        private IEnumerable<Workflow_VendasAtividadeModel> _atividades;
        public virtual IEnumerable<Workflow_VendasAtividadeModel> Atividades
        {
            get
            {
                if (_atividades == null)
                    _atividades = new List<Workflow_VendasAtividadeModel>();
                return _atividades;
            }
            set
            {
                _atividades = value;
            }
        }
    }
        public class Workflow_VendasAtividadeModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Workflow_Vendas WorkFlow { get; set; }
        public virtual bool Ativ_AgendaTarefa { get; set; }
        public virtual Boolean Ativ_Status { get; set; }
        public virtual String Ativ_Status_Desc { get; set; }
        public virtual String Ativ_Descricao { get; set; }
        public virtual String Ativ_Prazo { get; set; }
        public virtual Tipo_HORA Ativ_Tipodias { get; set; }
        public virtual String Ativ_Tipodias_Desc { get; set; }
        public virtual String Ativ_StatusVenda { get; set; }
        public virtual String Ativ_StatusVenda_Desc { get; set; }
        public virtual String Horario_Atendimento { get; set; }
        public virtual String Horario_Atendimento_Desc { get; set; }
        public virtual Atividade_Decisoria Ativ_Decisao { get; set; }
        //public virtual Tipo_Atividade_Decisoria Ativ_TipoDecisao { get; set; }
        public virtual String Ativ_Decisao_Desc { get; set; }
        public virtual Boolean Definir_Responsavel { get; set; }
        public virtual String Definir_Responsavel_Desc { get; set; }

        public virtual Int64? Setor_Departamento { get; set; }
        public virtual String Setor_Departamento_Desc { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Responsavel_Desc { get; set; }
        public virtual String Ativ_TipoDecisao_Desc { get; set; }
        public virtual Int32? Ativ_NAtivAprovada { get; set; }
        public virtual Int32? Ativ_NAtivReprovada { get; set; }
        public virtual String StatusPadrao { get; set; }
        public virtual Status_Venda? Id_Status_Padrao { get; set; }
    }
    public class Workflow_VendasListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual String Status_Desc { get; set; }

    }


    public class WorkFlow_VendasGetComboModel : IObjComboResult<String> 
    {
        public String Id { get; set; }
        public String Text { get; set; }
        public string Atividade_Status { get; set; }
        //public virtual Workflow_Vendas WorkFlow { get; set; }
        public virtual bool Ativ_AgendaTarefa { get; set; }
        public virtual Boolean Ativ_Status { get; set; }
        public virtual String Ativ_Descricao { get; set; }
        public virtual Int32? Ativ_Prazo { get; set; }
        public virtual Int64? Ativ_Tipodias { get; set; }

        public virtual Int64? Ativ_StatusVenda { get; set; }
        public virtual String Horario_Atendimento { get; set; }
        public virtual Int64? Ativ_Decisao { get; set; }
        public virtual Int32? Ativ_NAtivAprovada { get; set; }
        public virtual Int32? Ativ_NAtivReprovada { get; set; }
        public virtual String StatusPadrao { get; set; }
        public virtual String Ativ_StatusVenda_Desc { get; set; }
        //public virtual String Ativ_StatusVenda_Desc { get; set; }
        //public virtual Int64 Id { get; set; }
        //public virtual String Ativ_Status_Desc { get; set; }
        //public virtual String Ativ_Tipodias_Desc { get; set; }

        //public virtual String Horario_Atendimento_Desc { get; set; }
        //public virtual Tipo_Atividade_Decisoria Ativ_TipoDecisao { get; set; }
        //public virtual String Ativ_Decisao_Desc { get; set; }
        //public virtual String Ativ_TipoDecisao_Desc { get; set; }
    }



    public class Workflow_VendasDataTableResquest : DataTableRequest
    {
        public virtual String Status { get; set; }
    }

    
    public class workflow_VendasGetComboConfig : RequestObjComboPai<Int64?, Int64?> 
    {
        public virtual Boolean? Config { get; set; }
    }


}



