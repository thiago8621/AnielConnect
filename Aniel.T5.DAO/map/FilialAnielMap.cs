using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class FilialAnielMap : ClassMapBase<FilialAniel>
    {
        public FilialAnielMap() : base()
        {
            Table("TB_FILIAL");

            CompositeId()
            .KeyProperty(x => x.CodEmpresa, "CODEMPRESA")
            .KeyProperty(x => x.CodFilial, "CODFILIAL");
            

            Map(p => p.Nome, "NOME").Length(40);
            Map(p => p.Id_Filial_Connect, "ID_FILIAL_CONNECT");
            Map(p => p.CodFilialAux, "CODFILIAL_AUX").Length(50);


        }
    }
}
