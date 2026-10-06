using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class FuncionarioAnielMap : ClassMap<FuncionarioAniel>
    {
        public FuncionarioAnielMap()
        {
            Table("TB_FUNCIONARIO");

            Id(x => x.Num_Fre, "NUM_FRE").Length(6);

            Map(x => x.Nome, "NOME").Length(50).Not.Update();

            Map(x => x.Cpf, "CPF").Length(14);
            Map(x => x.Matricula_Aux, "NUM_FRE_AUX");
            Map(x => x.Email, "EMAIL");
            
            Map(x => x.Telefone_Celular, "TEL_CELULAR");
            Map(x => x.Supervisor, "SUPERVISOR");
            Map(x => x.Cod_Depto, "COD_DEPTO");

            Map(x => x.Situacao, "SITUACAO").CustomType<SituacaoFuncionarioAniel>();

            Map(x => x.Id_Municipio_Trabalho, "ID_MUNICIPIO_TRABALHO");
            Map(x => x.Uf_Trabalho, "UF_TRABALHO");
            Map(x => x.Municipio_Trabalho, "MUNICIPIO_TRABALHO");
            
            References(x => x.Cod_CCusto, "COD_CCUSTO").ForeignKey("FK_FUNCIONARIO_CC");

            Map(x => x.HorarioServicoAtribuido, "HORARIO_ATRIBUIDO").CustomType<HorarioAtribuido>();
        }
    }
}
