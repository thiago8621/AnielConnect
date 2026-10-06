using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class SerasaConsultaBlocosPJMap : ClassMapBase<SerasaConsultaBlocosPJ>
    {    
            public SerasaConsultaBlocosPJMap() : base()
            {
                Table("TB_CONSULTA_SERASA_BLOCOSPJ");

                Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONSULTA_SERASAPJ");
                Map(u => u.BlocoN200, "BLOCO_N200").Length(8000);
                Map(u => u.BlocoN210, "BLOCO_N210").Length(8000);
                Map(u => u.BlocoN230, "BLOCO_N230").Length(8000);
                Map(u => u.BlocoN240, "BLOCO_N240").Length(8000);
                Map(u => u.BlocoN250, "BLOCO_N250").Length(8000);
                Map(u => u.BlocoN270, "BLOCO_N270").Length(8000);        
                Map(u => u.BlocoN440, "BLOCO_N440").Length(8000);
                Map(u => u.BlocoN700, "BLOCO_N700").Length(8000);
                Map(u => u.BlocoN705, "BLOCO_N690").Length(8000);
                Map(u => u.BlocoF900, "BLOCO_F900").Length(8000);
                Map(u => u.Cnpj, "CNPJ").Length(15);
            }
        

    }
}
