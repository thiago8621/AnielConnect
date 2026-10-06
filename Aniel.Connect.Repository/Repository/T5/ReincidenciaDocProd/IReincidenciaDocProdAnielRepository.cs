using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ReincidenciaDocProd
{
    public interface IReincidenciaDocProdAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta às OSs reincidentes registradas no sistema
        /// </summary>
        IQueryable<ReincidenciaDocProdAniel> GetAll();

        /// <summary>
        ///     Grava um registro indicando um serviço reincidente lançado no sistema
        /// </summary>
        /// <param name="reincidencia">Objeto com as informações da reincidência</param>
        void Gravar(ReincidenciaDocProdAniel reincidencia);

        /// <summary>
        ///     Retorna a quantidade de serviços reincidentes de uma mesma natureza atribuídos a um técnico 
        ///     dentro de um determinado período
        /// </summary>
        /// <param name="equipe">Matrícula da equipe responsável pelo serviço que gerou reincidência</param>
        /// <param name="natureza">Nome da natureza de serviço</param>
        /// <param name="dataInicio">Data de início do período para busca de OS reincidente</param>
        /// <param name="dataFim">Data de fim do período para busca de OS reincidente</param>
        int QtdServicosReincidentesNaturezaPeriodo(string equipe, string natureza, DateTime dataInicio, DateTime dataFim);
    }
}
