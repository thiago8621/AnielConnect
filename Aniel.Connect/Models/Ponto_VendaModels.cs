using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
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
    public class Ponto_VendaListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Nome { get; set; }

        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Status { get; set; }

        public virtual Int64 Id_Supervisor { get; set; }
        public virtual Int64 Id_Vendedor { get; set; }
    }

    public class Validacao_VendedorModel
    {
        public virtual Int64 Id_Vendedor { get; set; }
        public virtual Int64 Id_Ponto_Venda { get; set; }
        public virtual String dataini { get; set; }
        public virtual String datafim { get; set; }
    }

    public class Ponto_VendaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Boolean Status { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Nome { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Cep { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Logradouro { get; set; }

        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Numero { get; set; }

        public virtual Int64? Id_Bairro { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Bairro { get; set; }

        public virtual Int64? Id_Cidade { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Cidade { get; set; }

        public virtual UF? UF { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Complemento { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Latitude { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Longitude { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual String DataInicial { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual String DataFinal { get; set; }

        public virtual Int64? Id_Vendedor { get; set; }
        public virtual String Vendedor { get; set; }

        private List<Ponto_Venda_VendedorCadastroModel> VendedoresLista;

        public virtual List<Ponto_Venda_VendedorCadastroModel> Vendedores

        {
            get
            {
                if (VendedoresLista == null)
                    VendedoresLista = new List<Ponto_Venda_VendedorCadastroModel>();
                return VendedoresLista;
            }
            set
            {
                VendedoresLista = value;
            }
        }
    }

    public class Ponto_VendaMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }


    }


    public class Ponto_Venda_DataTableRequest : DataTableRequest
    {

        public String Estado { get; set; }

        public String Cidade { get; set; }

        public String Bairro { get; set; }

        public String Supervisor { get; set; }

        public String Vendedor { get; set; }

        public Int64 Id_Vendedor { get; set; }

        public Int64 Id_Supervisor { get; set; }
        public String DataInicial { get; set; }
        public String DataFinal { get; set; }
        public virtual Int32? Status_Filtro { get; set; }
    }

    public class Ponto_Venda_VendedorCadastroModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }
        public Int64 Id_Vendedor { get; set; }
        public String Nome { get; set; }

        public String DataInicial { get; set; }
        public String DataFinal { get; set; }
    }

    public class PontoVenda_Mobile
    {
        public Ponto_Venda pontoVenda { get; set; }
        public Int64 idVendedor { get; set; }
        public String dataInicial { get; set; }
        public String dataFinal { get; set; }
    }



    [ExcelServiceKey("Logradouro")]
    [ExcelServiceSheetName("ponto_venda")]
    public class Ponto_VendaImportacao : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Codigo")]
        public virtual String Codigo { get; set; }

        [ExcelService("Status")]
        public virtual String Status { get; set; }

        [ExcelService("Descricao")]
        public virtual String Descricao { get; set; }

        [ExcelService("Cep")]
        public virtual String Cep { get; set; }

        [MaxLength(100)]
        [ExcelService("Logradouro")]
        public virtual String Logradouro { get; set; }

        [MaxLength(10)]
        [ExcelService("Numero")]
        public virtual String Numero { get; set; }

        [MaxLength(100)]
        [ExcelService("Bairro")]
        public virtual String Bairro { get; set; }


        [MaxLength(30)]
        [ExcelService("Cidade")]
        public virtual String Cidade { get; set; }

        [MaxLength(2)]
        [ExcelService("UF")]
        public virtual String UF { get; set; }

        [MaxLength(100)]
        [ExcelService("Complemento")]
        public virtual String Complemento { get; set; }


        [MaxLength(100)]
        [ExcelService("Longitude")]
        public virtual String Longitude { get; set; }


        [MaxLength(100)]
        [ExcelService("Latitude")]
        public virtual String Latitude { get; set; }
       

        public Boolean Importar { get; set; }
    }


    [ExcelServiceKey("MatriculaVendedor")]
    [ExcelServiceSheetName("vendedores")]
    public class VendedorImportacao : IExcelService
    {

        [ExcelService("MatriculaVendedor")]
        public virtual String MatriculaVendedor { get; set; }

        [ExcelService("Data_Inicial")]
        public virtual DateTime Data_Inicial { get; set; }

        [ExcelService("Data_Final")]
        public virtual DateTime Data_Final { get; set; }
        [MaxLength(50)]
        [ExcelService("Codigo")]
        public virtual String Codigo { get; set; }
        public Boolean Importar { get; set; }
    }

}