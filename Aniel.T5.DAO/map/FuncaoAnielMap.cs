using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    class FuncaoMap : ClassMap<Funcao>
    {
        public FuncaoMap()
        {
            Table("TB_FUNCAO");

        Id(x => x.Cod_Funcao, "COD_FUNCAO");
            
         Map(x => x.Nome, "NOME");
         Map(x => x.Plano, "PLANO");
         Map(x => x.Classe, "CLASSE");
         Map(x => x.Coluna, "COLUNA");
         Map(x => x.Revisao, "Revisao");
         Map(x => x.HXH, "HXH");
         Map(x => x.Salario, "SALARIO");
         Map(x => x.Num_Cbo, "NUM_CBO");
         Map(x => x.Cod_Sindicato, "COD_SINDICATO");
         Map(x => x.Integracao, "INTEGRACAO");
         Map(x => x.Adic_Insalubre, "ADIC_INSALUBRE");
         Map(x => x.Adic_Periculosidade, "ADIC_PERICULOSIDADE");
         Map(x => x.Adic_Transferencia, "ADIC_TRANSFERENCIA");
         Map(x => x.Requisito, "REQUISITO");
         Map(x => x.Objetivo, "OBJETIVO");
         Map(x => x.Desc_At_Ppp, "DESC_AT_PPP");
         Map(x => x.Responsabilidade, "RESPONSABILIDADE");
         Map(x => x.Piso_Minimo, "PISO_MINIMO");
         Map(x => x.Cod_Funcao_Aux, "COD_FUNCAO_AUX");
         Map(x => x.Funcao_Tributavel, "FUNCAO_TRIBUTAVEL");
         Map(x => x.Cod_Perfil_Equipe, "COD_PERFIL_EQUIPE");
         Map(x => x.Permite_Gerar_Equipe, "PERMITE_GERAR_EQUIPE");
         Map(x => x.Grupo_Funcao, "GRUPO_FUNCAO");
         Map(x => x.Status, "STATUS");

    }
    }
}
