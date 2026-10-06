using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    /// Model base para requisições a api para a tabela TB_LOGIN_RADIUS
    /// </summary>
    public class LoginRadiusApiModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Login { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual String EnderecoIPV4 { get; set; }
        public virtual String EnderecoIPV6 { get; set; }
        public virtual String SenhaPPPOE { get; set; }
        public virtual String UsuarioRoteador { get; set; }
        public virtual String SenhaRoteador { get; set; }
        public virtual String SSID_WIFI { get; set; }
        public virtual String Senha_WIFI { get; set; }
        public virtual String VLAN { get; set; }
        public virtual String Serial { get; set; }
        public virtual String MAC { get; set; }
        public virtual String Agent_Circuit_Id { get; set; }
        //public virtual Atributo_IPOE Atributo_IPOE { get; set; }
        public virtual long? Id_Atributo_IPOE { get; set; }
        public virtual Tipo_Autenticacao_Login? Tipo_Autenticacao_Login { get; set; }
        //public virtual POP POP { get; set; }
        public virtual long? Id_POP { get; set; }
        //public virtual Concentradora Concentradora { get; set; }
        public virtual long? Id_Concentradora { get; set; }
        //public virtual OLT OLT { get; set; }
        public virtual long? Id_OLT { get; set; }
        //public virtual Cliente Cliente { get; set; }
        public virtual long? Id_Cliente { get; set; }
        //public virtual Cliente_PlanoProduto Cliente_PlanoProduto { get; set; }
        public virtual long? Id_Cliente_PlanoProduto { get; set; }
        //public virtual Slot Slot { get; set; }
        public virtual long? Id_Slot { get; set; }
        //public virtual PON PON { get; set; }
        public virtual long? Id_PON { get; set; }
        public virtual Int64? Id_Caixa_FTTH { get; set; }
        public virtual Int64? Id_Porta_FTTH { get; set; }
        public virtual Int64? NumeroPorta { get; set; } 

    }
    public class LoginRadiusAtualizacao
    {

        public long? IdLogin { get; set; }
        public long? IdCaixa { get; set; }
        public long? numeroPorta { get; set; }

    }

    public class DadosLoginResponseModel
    {
        public long? IdLogin { get; set; }
        public string Login { get; set; }
        public long? IdCaixa { get; set; }
        public string DescricaoCaixa { get; set; }
        public long? IdPorta { get; set; }
        public long? NumeroPorta { get; set; }
        public long? IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public string CpfCliente { get; set; }
    }
    public class DadosCaixaResponseModel
    {
        public string id_caixa_ftth { get; set; }
        public string porta_ftth { get; set; }
        public string id_login { get; set; }
    }

    public class DadosLoginRequestModel
    {
        public long? IdCaixa { get; set; }
        public long? NumeroPorta { get; set; }
        public string Cliente { get; set; }
        public Int64? IdLogin { get; set; }
    }
    public class DadosCaixaRequestModel
    {
        public string? IdCaixa { get; set; }
    }
    public class DadosOSModel
    {
        public string numObra { get; set; }
        public string codct { get; set; }
        public string projeto { get; set; }
        public int numdoc { get; set; }
    }
    public class DadosOSResponseModel
    {
        public string Pop { get; set; }
        public string Olt { get; set; }
        public string Plano { get; set; }
    }

}