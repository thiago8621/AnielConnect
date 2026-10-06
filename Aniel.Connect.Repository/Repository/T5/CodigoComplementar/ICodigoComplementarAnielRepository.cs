using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.CodigoComplementar
{
    public interface ICodigoComplementarAnielRepository
    {
        Task GetById(ProductToApproval model, Requisicao_SequenciaRmdmAniel obj, MaterialAniel materialAniel);
    }
}
