using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Functions;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Teste
{
    [TestClass]
    public class DistanciaTeste
    {

        [TestMethod]
        public void Passo01_RegistrosIguais()
        {
            var resultado = MapaService.GetMenorDistancia(-22.3496466, -48.7806715, -22.3496466, -48.7806715);
            Assert.AreEqual(0, resultado);
        }




    }
}
