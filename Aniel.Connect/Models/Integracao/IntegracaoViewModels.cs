using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Sinapse.Core.Entity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.Integracao
{
    public enum TipoIntegracao
    {
        Excel = 1,
        CopiaCola = 2
    }

    public class IntegracaoIndexViewModels
    {
        [Display(Name = "TIPOINTEGRACAOSELECIONADO", ResourceType = typeof(Resources.RsIntegracao))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsIntegracao),
                  ErrorMessageResourceName = "TIPOINTEGRACAOSELECIONADOREQUIRED")]
        public TipoIntegracao TipoIntegracaoSelecionado { get; set; }
    }

    public class IntegracaoExcelPasso1ViewModels
    {
        [Display(Name = "OBJETO", ResourceType = typeof(Resources.RsIntegracao))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsIntegracao),
                  ErrorMessageResourceName = "OBJETOREQUIRED")]
        public String Objeto { get; set; }

        public String NomeArquivo { get; set; }

    }

    public class IntegracaoExcelPasso2ViewModels
    {
        [Display(Name = "OBJETO", ResourceType = typeof(Resources.RsIntegracao))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsIntegracao),
                  ErrorMessageResourceName = "OBJETOREQUIRED")]
        public String Objeto { get; set; }

        public String NomeArquivo { get; set; }

    }

    public class IntegracaoExcelPasso3ViewModels
    {
        [Display(Name = "OBJETO", ResourceType = typeof(Resources.RsIntegracao))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsIntegracao),
                  ErrorMessageResourceName = "OBJETOREQUIRED")]
        public String Objeto { get; set; }

        public String NomeArquivo { get; set; }

        public List<IntegracaoStatusImportacao> ListaStatus { get; set; }

    }

    public class IntegracaoExcelPasso4ViewModels
    {
        [Display(Name = "OBJETO", ResourceType = typeof(Resources.RsIntegracao))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsIntegracao),
                  ErrorMessageResourceName = "OBJETOREQUIRED")]
        public String Objeto { get; set; }

        public String NomeArquivo { get; set; }

        public List<IntegracaoStatusImportacao> ListaStatus { get; set; }

    }

    public class LogIntegracaoViewModel : IViewModel<Int64>, IBindViewToObj
    {
        public virtual Int64 Id { get; set;}

        [BindModelView]
        public virtual String NomeArquivo { get; set; }
        [BindModelView]
        public virtual String Objeto { get; set; }
        [BindModelView]
        public virtual Int32 QtdeImportado { get; set; }
        [BindModelView]
        public virtual Int32 QtdeNImportado { get; set; }
        [BindModelView]
        public virtual Int32 Total { get; set; }

        public O BindViewModelToObj<O>(O pObj)
        {
            return BindService.BindViewModelToObj<Int64, O>(this, pObj);
        }

        public LogIntegracaoViewModel BindObjToViewModel<O>(O pObj)
        {
            BindService.BindObjToViewModel<O, Int64>(pObj, this);

            return this;
        }

    }

    public class LogIntegracaoListaViewModel : IViewModel<Int64>, IBindViewToObj
    {
        public virtual Int64 Id { get; set; }

        [BindModelView]
        [Display(Name = "NOMEARQUIVO", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual String NomeArquivo { get; set; }
        [BindModelView]
        [Display(Name = "OBJETO", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual String Objeto { get; set; }
        [BindModelView]
        [Display(Name = "QTDEIMPORTADO", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual Int32 QtdeImportado { get; set; }
        [BindModelView]
        [Display(Name = "QTDENIMPORTADO", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual Int32 QtdeNImportado { get; set; }
        [BindModelView]
        [Display(Name = "TOTAL", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual Int32 Total { get; set; }
        [BindModelView]
        [Display(Name = "DATAHORAIMPORTACAO", ResourceType = typeof(Resources.RsIntegracao))]
        public virtual DateTime? DataHoraCriacao { get; set; }

        public O BindViewModelToObj<O>(O pObj)
        {
            return BindService.BindViewModelToObj<Int64, O>(this, pObj);
        }

        public LogIntegracaoListaViewModel BindObjToViewModel<O>(O pObj)
        {
            BindService.BindObjToViewModel<O, Int64>(pObj, this);

            return this;
        }

    }

}