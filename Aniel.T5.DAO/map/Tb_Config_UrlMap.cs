using Aniel.T5.Model;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{

    public class Tb_Config_UrlMap : ClassMap<Tb_Config_Url>
    {
        public Tb_Config_UrlMap() : base()
        {
            Table("TB_CONFIG_URL");
            Id(u => u.Sistema, "SISTEMA");
            Map(x => x.Url, "URL");
        }
    }

}
