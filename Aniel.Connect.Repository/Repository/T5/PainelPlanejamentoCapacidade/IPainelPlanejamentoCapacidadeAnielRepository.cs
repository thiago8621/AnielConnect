using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.T5.PainelPlanejamentoCapacidade
{
    /// <summary>
    ///     Repository para extrair informações pertinentes ao painel de planejamento de capacidade
    /// </summary>
    public interface IPainelPlanejamentoCapacidadeAnielRepository
    {
        /// <summary>
        ///     Retorna informações da escala das equipes em determinada data para o painel de 
        ///     planejamento de capacidade
        /// </summary>
        /// <param name="config">
        ///     Objeto com as configurações do Connect, utilizado para ler 
        ///     configurações que afetam alguns campos do painel
        /// </param>
        /// <param name="data">Data para verificação da escala</param>
        /// <param name="codEscritorio">Código do grupo área que será verificado. Serão retornados somente técnicos que atendem a esse grupo área</param>
        /// <param name="codArea">Código da área que será verificada. Serão retornados somente técnicos que atendem a essa área</param>
        /// <param name="idNatureza">ID da natureza que será verificada. Serão retornados somente técnicos que atendem a natureza especificada</param>
        IEnumerable<PainelPlanejamentoCapacidadeDadosEscalaEquipe> GetEscalaTecnicos(
            Configuracoes_Gerais config, 
            DateTime data, 
            string codEscritorio, 
            string codArea,
            int idNatureza
        );

        List<TimeSpan> GetServicosAgendar(
            Configuracoes_Gerais config,
            int? Natureza,
            string codEscritorio,
            string codArea
        );

        List<TimeSpan> GetQtdServicosAgendados(
           Configuracoes_Gerais config,
           int? natureza,
           string codEscritorio,
           string codArea,
           DateTime data
       );

        List<TimeSpan> GetQtdServicosAtribuidosTecnico(
           Configuracoes_Gerais config,
           int? natureza,
           string codEscritorio,
           string codArea,
           DateTime data
       );

        TimeSpan GetTempoMedioNatureza(
           Configuracoes_Gerais config,
           int? natureza
       );
    }
}
