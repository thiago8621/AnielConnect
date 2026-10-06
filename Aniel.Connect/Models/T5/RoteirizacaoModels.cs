using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{



    public class RoteirizacaoDiaConf
    {
        public virtual DateTime Data { get; set; }
        public virtual bool feriado { get; set; }
        public virtual Int32 NumeroMaxAtendimentoTecnico { get; set; }
        public virtual List<Horario_Connect> horarios_servico { get; set; }
        public virtual bool GravarDados { get; set; }
        public bool Alterar_DataHora_SemAgendamentoAniel { get; set; }
        public bool Encaixe { get; set; }
        public bool EncaixeAutomatico { get; set; }
    }



    public enum ReferenciaDistancia
    {
        PrimeiroServico = 1,
        UltimoServico = 2,
        PontoPartida
    }

    public interface IRoterizar
    {
        Configuracoes_Gerais config { get; set; }
        List<Turno_ServicoAniel> turnos { get; set; }
        RoteirizacaoDiaConf dia { get; set; }
        Queue<TecnicoRoteirizar> tecnicos { get; set; }

        /// <summary>
        /// Função que implementa a lógica da roteirização, distribuição de ordens de serviço entre os técnicos.
        /// </summary>
        /// <param name="documentos"></param>
        /// <param name="tecnicos"></param>
        /// <returns></returns>
        (List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados) Run(List<DocumentoRoteirizar> documentos);
        List<Painel_Servicos_AgendamentosEquipe> listaAusencias { get; set; }
    }

}