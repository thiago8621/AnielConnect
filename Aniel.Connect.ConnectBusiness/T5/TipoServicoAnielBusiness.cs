using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class TipoServicoAnielBusiness
    {
        private readonly ITipoServicoAnielRepository _repoTipoServicoAnielRepository;

        public TipoServicoAnielBusiness(ITipoServicoAnielRepository repoTipoServ)
        {
            this._repoTipoServicoAnielRepository = repoTipoServ;
        }

        public short? GetTipoServicoPlano(int codTipoServ, string codCt)
        {
            Tipo_Servico_EquipeAniel TipoServicoAnielGenerico = _repoTipoServicoAnielRepository.GetByCodigo(codTipoServ);
            Tipo_Servico_EquipeAniel TipoServicoAnielContrato = TipoServicoAnielGenerico != null ? _repoTipoServicoAnielRepository.GetByDescriacaoContrato(TipoServicoAnielGenerico.Descricao, codCt) : null;
            return TipoServicoAnielContrato?.Cod_Tipo_Serv;
        }

        public Response<TimeSpan?> TempoServicoMinimoNaturezas(List<int> naturezas)
        {
            Response<TimeSpan?> response = new Response<TimeSpan?>();   
            try
            {
                response.valor = this._repoTipoServicoAnielRepository.TempoServicoMinimoNaturezas(naturezas);
            }
            catch (Exception ex)
            {
                response.valor = null;
                response.message = ex.Message;
                response.stacktrace = ex.StackTrace;
                response.exception = ex.Message;
            }

            return response;
        }
    }
}
