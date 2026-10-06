using Aniel.MGTEC.Core.Service;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Aniel.MGTEC.Models
{
    public class EmpresaViewModel : IViewModel<Int64>, IBindViewToObj, IValidatableObject
    {
        [BindModelView]
        public Int64 Id { get; set; }

        [IntegracaoReadOnly]
        [Display(Name = "CODIGO", ResourceType = typeof(Resources.Resources))]
        [BindModelView]
        public Int32 Codigo { get; set; }

        [BindModelView]
        [Display(Name = "FANTASIA", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "FANTASIAREQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Fantasia { get; set; }

        [BindModelView]
        [Display(Name = "RAZAOSOCIAL", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "RAZAOSOCIALREQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string RazaoSocial { get; set; }

        [BindModelView("Logradouro")]
        [Display(Name = "ENDERECO", ResourceType = typeof(Resources.Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Endereco { get; set; }

        [BindModelView]
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.Resources))]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Numero { get; set; }

        [BindModelView]
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Complemento { get; set; }

        [BindModelView]
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Bairro { get; set; }

        [BindModelView]
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.Resources))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Cidade { get; set; }

        [BindModelView("Estado")]
        [Display(Name = "UF", ResourceType = typeof(Resources.Resources))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string UF { get; set; }

        [BindModelView]
        [Display(Name = "CNPJ", ResourceType = typeof(Resources.Resources))]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string CNPJ { get; set; }

        public O BindViewModelToObj<O>(O pObj)
        {
            return BindService.BindViewModelToObj<Int64, O>(this, pObj);
        }

        public EmpresaViewModel BindObjToViewModel<O>(O pObj)
        {
            try
            {
                BindService.BindObjToViewModel<O, Int64>(pObj, this);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.Resources.ERRO_BINDINGOBJ, ex);
            }
            return this;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            /*
            if (this.Enable)
            {
                Validator.TryValidateProperty(this.Prop1,
                    new ValidationContext(this, null, null) { MemberName = "Prop1" },
                    results);
                Validator.TryValidateProperty(this.Prop2,
                    new ValidationContext(this, null, null) { MemberName = "Prop2" },
                    results);

                // some other random test
                if (this.Prop1 > this.Prop2)
                {
                    results.Add(new ValidationResult("Prop1 must be larger than Prop2"));
                }
            }*/
            //results.Add(new ValidationResult("Prop1 must be larger than Prop2"));

            return results;
        }

    }

    public class EmpresaListaModel : IViewModel<Int64>, IBindViewToObj
    {
        [BindModelView]
        public Int64 Id { get; set; }

        [BindModelView]
        [Display(Name = "CODIGO", ResourceType = typeof(Resources.Resources))]
        public Int32 Codigo { get; set; }

        [BindModelView]
        [Display(Name = "FANTASIA", ResourceType = typeof(Resources.Resources))]
        public string Fantasia { get; set; }

        [BindModelView]
        [Display(Name = "RAZAOSOCIAL", ResourceType = typeof(Resources.Resources))]        
        public string RazaoSocial { get; set; }

        [BindModelView("Logradouro")]
        [Display(Name = "ENDERECO", ResourceType = typeof(Resources.Resources))]
        public string Endereco { get; set; }

        [BindModelView]
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.Resources))]
        public string Numero { get; set; }

        [BindModelView]
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.Resources))]
        public string Complemento { get; set; }

        [BindModelView]
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.Resources))]
        public string Bairro { get; set; }

        [BindModelView]
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.Resources))]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        [BindModelView("Estado")]
        [Display(Name = "UF", ResourceType = typeof(Resources.Resources))]
        public string UF { get; set; }

        [BindModelView]
        [Display(Name = "CNPJ", ResourceType = typeof(Resources.Resources))]
        public string CNPJ { get; set; }

        public O BindViewModelToObj<O>(O pObj)
        {
            return BindService.BindViewModelToObj<Int64, O>(this, pObj);
        }

        public EmpresaListaModel BindObjToViewModel<O>(O pObj)
        {
            BindService.BindObjToViewModel<O, Int64>(pObj, this);

            return this;
        }

    }


}