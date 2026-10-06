using Aniel.Connect.Repository.T5.Equipe;

using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Core;
using System;
using Aniel.Connect.Repository.T5.Olt;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class OltBusiness
    {
        private readonly IOltRepository _repoOlt;
        private readonly IContratoAnielRepository _repoContrato;

        public OltBusiness(IOltRepository repOlt)
        {
            this._repoOlt = repOlt;
        }


        public System.Collections.Generic.List<OltListModel> GetAll(OltRequest request)
         {
            return this._repoOlt.GetAll(request);
        }
        public PostApiRetorno InsertOLT(OLTModels dados)
        {
            return this._repoOlt.InsertOLT(dados);
        }
        public PostApiRetorno UpdateOLT(OLTModels dados)
        {
            return this._repoOlt.UpdateOLT(dados);
        }


    }
}
