using Aniel.Connect.Core;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class PerfilAtivacaoListaModel
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
    }

    public class PerfilAtivacaoCadastroModel
    {
        public Int64 Id { get; set; }

        public String Nome { get; set; }
        public String LineProfile {get; set;}
        public String ScriptOLTBase64 { get; set; }
        public String ScriptOLTVozBase64 { get; set; }

        public String IpGerencia { get; set; }
        public String Login { get; set; }
        public String Senha { get; set; }

        //
        public String PortaTelnet { get; set; }
        public String PortaHttp { get; set; }

        public String GemPort { get; set; }
        public String ServicePort { get; set; }
        public String SenorId { get; set; }
        //

        public String ScriptONUBase64 { get; set; }
        public bool ObrigaWifi { get; set; }
        public virtual string Cod_Escritorio { get; set; }
        public virtual string Cod_Area { get; set; }

        private List<Perfil_Ativacao_Grupo_AreaListModel> GrupoAreaLista;
        public virtual List<Perfil_Ativacao_Grupo_AreaListModel> GruposArea
        {
            get
            {
                if (GrupoAreaLista == null)
                    GrupoAreaLista = new List<Perfil_Ativacao_Grupo_AreaListModel>();
                return GrupoAreaLista;
            }
            set
            {
                GrupoAreaLista = value;
            }
        }
    }

    public class PerfilAtivacaoObjComboResult : IObjComboResult<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Text { get; set; }
        public virtual String ScriptAtivacao { get; set; }
        public virtual String ScriptONU { get; set; }
    }

    [ExcelServiceKey("CodMat")]
    [ExcelServiceSheetName("PERFIL_ATIVACAO")]
    public class Perfil_Ativacao_Vinculo_Importacao : IExcelService
    {
        [Required]
        [ExcelService("CODMAT")]
        [MaxLength(12)]
        public virtual String CodMat { get; set; }
        
        [ExcelService("ID_OLT")]
        public virtual Int64? Id_Olt { get; set; }

        [Required]
        [ExcelService("ID_PERFIL_ATIVACAO")]
        public virtual Int64? Id_Perfil_Ativacao { get; set; }
        
        [ExcelService("ID_PERFIL_ATIVACAO_ONU")]
        public virtual Int64? Id_Perfil_Ativacao_Onu { get; set; }
        
        [ExcelService("CODIGO_AUXILIAR_OLT")]
        public virtual string Codigo_Auxiliar_Olt { get; set; }

        [ExcelService("USA_PROVISIONAMENTO_CONNECT")]
        public virtual string Usa_Provisionamento_Connect { get; set; }

        public Boolean Importar { get; set; }       
    }

    public class LogImportacaoVinculoPerfilAtivacaoRequest : DataTableRequest
    {
        public string Data { get; set; }
    }
}