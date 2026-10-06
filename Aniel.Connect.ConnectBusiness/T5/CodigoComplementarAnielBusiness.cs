using Aniel.Connect.Repository.Repository.T5.CodigoComplementar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class CodigoComplementarAnielBusiness
    {
        private readonly ICodigoComplementarAnielRepository _codigo;
        public CodigoComplementarAnielBusiness(ICodigoComplementarAnielRepository codigo)
        {
            _codigo = codigo;
        }
    }
}
