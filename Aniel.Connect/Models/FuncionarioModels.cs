using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class FuncionarioListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Meta { get; set; }
                

        public virtual Int64 Id_Supervisor { get; set; }

        public virtual Int64 Id_Vendedor { get; set; }

        public virtual String Num_Fre { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Tel_Celular { get; set; }


        public virtual Boolean? Vendedor { get; set; }

        public virtual String Supervisor { get; set; }


        public virtual Boolean? E_SUPERVISOR { get; set; }

        public virtual String Meta { get; set; }

        public virtual String Faixa { get; set; }


        public virtual String Departamento { get; set; }
        public virtual String ParametrosConsultaPreQual { get; set; }

        public virtual String Filial { get; set; }

        public virtual String Desc_Comissionamento { get; set; }
        public virtual String PossuiAcessoPremier { get; set; }
        public virtual String TipoUsuario { get; set; }
        public virtual String NomeSupervisor { get; set; }
        public virtual String EquipeVenda { get; set; }
    }

    public class SupervisorListaModel {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
    }
    
    public class FuncionarioCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Funcionario { get; set; }

        public virtual String Num_Fre { get; set; }

        public virtual String Nome { get; set; }
        public virtual String Tel_Celular { get; set; }
        public virtual Int16? Situacao { get; set; }
        public virtual Boolean Vendedor { get; set; }
        public virtual Int64? Id_Supervisor { get; set; }
        public virtual String Desc_Supervisor { get; set; }
        public string Tipo { get; set; }
        public long TipoEnum { get; set; }

        public virtual Boolean E_SUPERVISOR { get; set; }
        public virtual Boolean CadastrarEmMassa { get; set; }
        public virtual String Funcionarios { get; set; }

        public virtual Int64? Id_Meta { get; set; }

        public virtual Int64? Id_Meta_Supervisor { get; set; }

        public virtual String Desc_Meta { get; set; }

        public virtual Int64? Id_Faixa { get; set; }       
       
        public virtual String Desc_Faixa { get; set; }

        public virtual Int64 Id_Departamento { get; set; }

        public virtual String Desc_Departamento { get; set; }

        public virtual Int64? Id_Filial { get; set; }

        public virtual String Desc_Filial { get; set; }
        public virtual Int64? IdEquipeVenda { get; set; }
        public virtual Int64? IdParamPreQual { get; set; }

        public virtual Int64? Id_Comissionamento { get; set; }
        public virtual string CodigoVendedorIntegracao { get; set; }

        private IList<EquipeVenda> _equipevendas;
        public virtual IList<EquipeVenda> equipevendas
        {
            get
            {
                if (_equipevendas == null)
                    _equipevendas = new List<EquipeVenda>();
                return _equipevendas;
            }
            set
            {
                _equipevendas = value;
            }
        }

        public string CodigoVendedorIntegracaoAuxiliar { get; set; }
    }

    public class VendedorMetaFaixaComissionamentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? Id_Meta { get; set; }
        public virtual Int64? Id_Faixa { get; set; }
        public virtual Int64? Id_Comissionamento { get; set; }
    }

    public class Funcionario_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id_Supervisor { get; set; }
        public virtual Int64? Meta { get; set; }
        public virtual Int64? Comissionamento { get; set; }
        public virtual Int64? FaixaComissionamento { get; set; }
        public virtual bool Todos { get; set; }
        public virtual bool Premier { get; set; }
        public virtual bool NaoPremier { get; set; }
        public virtual string Nome { get; set; }
        public virtual string EquipeVenda { get; set; }
    }

    public class VendedorComboModel : ObjComboResult<Int64>
    {
        public virtual Int64? Id_Supervisor { get; set; }
        public virtual Int64? Id_Departamento { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Num_Fre { get; set; }
    }
    
    public class FuncionarioComboModel : ObjComboResult<String>
    {
        public virtual String Id { get; set; }
        public virtual String Num_fre { get; set; }
        public virtual String Nome { get; set; }
    }

    public class ObjComboSupervisor_Funcionario : ObjComboResult<long> 
    {
        public string Nome { get; set; } 
    }

    public class ObjComboResultSupervisor : ObjComboResult<long>
    {
        /// <summary>
        ///     Matrícula do supervisor
        /// </summary>
        public string CodSuperv { get; set; }
    }

    public class ObjComboResultMonitor : ObjComboResult<long>
    {
        /// <summary>
        ///     Matrícula do monitor
        /// </summary>
        public string CodUsuario { get; set; }
        public string CodEquipe { get; set; }
    }

    #region mobile
    public class Funcionario_Vendedor
    {
        public virtual Int64? Id { get; set; }
        public virtual String Num_Fre { get; set; }
        public virtual String Nome { get; set; }
        public virtual DateTime DataHoraCriacao { get; set; }
        public virtual Int64? Id_supervisor { get; set; }
        public virtual Boolean? E_Supervisor { get; set; }
    }
    #endregion
}