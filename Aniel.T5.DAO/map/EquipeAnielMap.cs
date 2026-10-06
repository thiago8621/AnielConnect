using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class EquipeAnielMap : ClassMap<EquipeAniel>
    {
        public EquipeAnielMap()
        {
            Table("TB_EQUIPE");

            Id(x => x.Matricula, "EQUIPE").Length(10);

            Map(x => x.EquipeAux, "EQUIPEAUX");
            Map(x => x.Nome, "NOME");
            Map(x => x.Encerramento, "ENCERRAMENTO");
            Map(x => x.Device_Id, "DEVICE_ID");

            References(x => x.Supervisor, "COD_SUPERV").ReadOnly();
            Map(x => x.CodSuperv, "COD_SUPERV");
            
            References(x => x.Ponto_Partida, "ID_PONTO_PARTIDA").ForeignKey("FK_EQUIPE_PONTO_PARTIDA");

            Map(x => x.Num_Fre, "NUM_FRE");
            Map(x => x.CodCtIntegrator, "CODCT_INTEGRATOR");
            Map(x => x.ProjetoIntegrator, "PROJETO_INTEGRATOR");
            Map(x => x.Cod_Fornecedor, "COD_FORNECEDOR");
            Map(x => x.Connect, "EQUIPE_CONNECT").CustomSqlType("Char(1)").Default("'N'").Index("IDX_EQUIPE_CONNECT");

            References(x => x.PerfilAgendamento, "ID_PERFIL_AGENDAMENTO").ForeignKey("FK_EQUIPE_PERFIL_AGEND");

            Map(x => x.Monitor, "USUARIO_MONITOR").Length(10).Index("IDX_EQUIPE_MONITOR");

            //Where("EQUIPE_CONNECT = 'S'");
        }
    }
}
