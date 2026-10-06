
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
using System.Threading.Tasks;
using Sinapse.Integracao.Ativacao.Models;

namespace Aniel.Connect.Repository.T5.Olt
{
    public interface IONURepository
    {
        Task InsertONU(ConfigurarONUModel model, string usuarioCriacao);
        Task<OnuExistenteModel> ExisteONUMesmoSerialOuLogin(string serial, string login);
        Task<bool> ExisteONUMesmoONTID(long idOLT, long idSlot, long idPon, int ONTID);
    }
}
