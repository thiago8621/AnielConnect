using Aniel.T5.Model;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{

    public class Tb_NCMMap : ClassMap<Tb_NCM>
    {
        public Tb_NCMMap() 
        {
            Table("TB_NCM");
            Id(u => u.Id, "ID_NCM").GeneratedBy.Identity();
            Map(x => x.Codigo_NCM, "CODIGO_NCM").Length(10);
            Map(x => x.Descricao_NCM, "NCM").Length(50); ;
            Map(x => x.Codigo_CEST, "CEST").Length(7); 

            
        }
 
    }

}
