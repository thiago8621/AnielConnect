
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

namespace Aniel.Connect.Repository.T5.Olt
{
    public interface IOltRepository
    {
         System.Collections.Generic.List<OltListModel> GetAll(OltRequest request);
         PostApiRetorno InsertOLT(OLTModels dados);
        PostApiRetorno UpdateOLT(OLTModels dados);
        }
}
