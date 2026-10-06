using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator
{
    public interface IConfigAnielIntegratorRepository
    {
        Task<Config_Aniel_Integrator> ObterConfigAnielIntegrator();

        Task<Config_Aniel_Integrator> GetByProjeto(string codct, string projeto);
    }
}
