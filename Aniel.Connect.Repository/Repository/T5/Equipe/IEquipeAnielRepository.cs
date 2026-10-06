
﻿using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Data;
﻿using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Equipe
{
    public interface IEquipeAnielRepository
    {
        IQueryable<EquipeAniel> GetAll();
        /// <summary>
        ///     Retorna o nome da equipe especificada
        /// </summary>
        /// <param name="equipe">Código da equipe</param>
        string GetNome(string equipe);


        List<EquipeAuxiliaresModel> GetTecnicosAuxiliares(EquipeAuxiliaresModel equipe);

        DataTableResponse<EquipeAuxiliaresModel> GetTecnicosAuxiliaresRelatorio(EquipeAuxiliaresModelRequest request);

        bool AdicionarTecnicoAuxiliar(string Equipe,Documento_ProducaoAniel documento);
        bool RemoverTecnicoAuxiliar(string Equipe, Documento_ProducaoAniel documento);

        /// <summary>
        /// Busca equipe atraves de codigo auxiliar e retorna dados uteis para integracao
        /// </summary>
        /// <param name="Matricula"></param>
        /// <returns></returns>
        EquipeIntegracaoModel GetEquipe(String Matricula);

    }
}
