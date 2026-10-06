
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Data;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Equipe
{
    public interface IPopRepository
    {
        System.Collections.Generic.List<PopModelsResponse> GetAll();
        PostApiRetorno InsertPop(PopModels dados);
        PostApiRetorno AtualizarPop(PopModels dados);
    }
}
