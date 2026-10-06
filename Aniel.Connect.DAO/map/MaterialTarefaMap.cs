using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class MaterialTarefaMap : ClassMapBase<MaterialTarefa>
    {
        public MaterialTarefaMap() : base()
        {
            Table("TB_MATERIAL_TAREFA");

            Id(x => x.Id, "ID");
            
            Map(x => x.Alterado, "ALTERADO");

            Map(x => x.Id_Cliente, "ID_CLIENTE");

            Map(x => x.CodMat_Inst, "CODMAT_INST");

            Map(x => x.Quantidade, "QUANTIDADE");

            Map(x => x.Quantidade_Removida, "QUANT_REMOV");
        }
    }
}
