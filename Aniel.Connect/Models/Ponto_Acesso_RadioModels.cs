using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class PontoAcessoRadioListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Mac { get; set; }
        public virtual String Ip { get; set; }
        public virtual String Situacao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }

    public class PontoAcessoRadioCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }

        public virtual String Mac { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
        public virtual String Ip { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual string Cod_Escritorio { get; set; }
        public virtual string Cod_Area { get; set; }

        private IList<Ponto_Acesso_Radio_Grupo_AreaModels> _gruposarea;
        public virtual IList<Ponto_Acesso_Radio_Grupo_AreaModels> GruposArea
        {
            get
            {
                if (_gruposarea == null)
                    _gruposarea = new List<Ponto_Acesso_Radio_Grupo_AreaModels>();
                return _gruposarea;
            }
            set
            {
                _gruposarea = value;
            }
        }
    }

    public class PontoAcessoRadio_Request: DataTableRequest
    {
        public virtual String Id_Ponto { get; set; }
        public String Status_Filtro { get; set; }

        public String Ip { get; set; }
    }

    public class PontoAcessoRadioTreeViewModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class CadastrarInterfaceRadio_Request
    {
        public virtual String Descricao { set; get; }
        public virtual String Codigo_Auxiliar { get; set; }
        public virtual Int64? Id_Ponto_Acesso { get; set; }
    }


}
