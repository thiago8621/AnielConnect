using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.TipoServico
{
    public interface ITipoServicoAnielRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descricao">descricao do tipo de servico</param>
        /// <param name="codCt">codigo do contrato</param>
        /// <returns></returns>
        Tipo_Servico_EquipeAniel GetByDescriacaoContrato(string descricao, string codCt);

        /// <summary>
        ///     Retorna o setor associado ao código informado
        /// </summary>
        /// <param name="cod">Código do tipo de servico</param>
       Tipo_Servico_EquipeAniel GetByCodigo(int cod);

        /// <summary>
        ///     Retorna o menor tempo de servico entre as naturezas
        /// </summary>
        /// <param name="naturezas">Códigos das naturezas</param>
        TimeSpan? TempoServicoMinimoNaturezas(List<int> naturezas);
    }
}
