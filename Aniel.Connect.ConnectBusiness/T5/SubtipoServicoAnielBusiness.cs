using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.Connect.Repository.T5.SubtipoServico;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using System;
using System.Globalization;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class SubtipoServicoAnielBusiness
    {
        private readonly ISubtipoServicoAnielRepository _repo;

        public SubtipoServicoAnielBusiness(ISubtipoServicoAnielRepository repoSubtipoServ)
        {
            this._repo = repoSubtipoServ;
        }

        public string GetSubtipoServicoPlano(string codSubTipoServGenerico, short codTipoServGenerico, short codTipoServ)
        {
            Sub_Tipo_ServicoAniel SubtipoServicoAnielGenerico = _repo.GetByCodigo(codSubTipoServGenerico, codTipoServGenerico);
            Sub_Tipo_ServicoAniel SubtipoServicoAnielContrato = SubtipoServicoAnielGenerico != null ? _repo.GetByDescricaoTipoServ(SubtipoServicoAnielGenerico.Descricao, codTipoServ) : null;
            return SubtipoServicoAnielContrato?.Codigo_Sub_Tipo;
        }

       
    }
}
