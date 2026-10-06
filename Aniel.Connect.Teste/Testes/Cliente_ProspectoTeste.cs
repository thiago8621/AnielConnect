using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Teste.Testes
{
    [TestClass]
    public class Cliente_ProspectoTeste
    {
        [TestMethod]
        public async Task novoProspecto()
        {
            var obj = new Cliente_ProspectoDadosGeraisViewModel
            {
                //    RG=,CpfCnpj=,CPFResponsavel=,DataNascimento=,Data_Abertura=,Email=,EnderecoProspecto=,EstadoCivil=,
                //nome\": \"Fernando Victor Kaique Duarte\",\r\n\t\t\"idade\": 22,\r\n\t\t\"cpf\": \"190.559.377-54\",\r\n\t\t\"rg\": \"31.627.671-6\",\r\n\t\t\"data_nasc\": \"01/03/2001\",\r\n\t\t\"sexo\": \"Masculino\",\r\n\t\t\"signo\": \"Peixes\",\r\n\t\t\"mae\": \"Elisa Bianca Pietra\",\r\n\t\t\"pai\": \"Manoel Edson Duarte\",\r\n\t\t\"email\": \"fernandovictorduarte@prohaska.com.br\",\r\n\t\t\"senha\": \"gsR3kMrR31\",\r\n\t\t\"cep\": \"25045-049\",\r\n\t\t\"endereco\": \"Rua Sônia\",\r\n\t\t\"numero\": 544,\r\n\t\t\"bairro\": \"São Bento\",\r\n\t\t\"cidade\": \"Duque de Caxias\",\r\n\t\t\"estado\": \"RJ\",\r\n\t\t\"telefone_fixo\": \"(21) 3880-1239\",\r\n\t\t\"celular\": \"(21) 99111-6597\",\r\n\t\t\"altura\": \"1,96\",\r\n\t\t\"peso\": 100,\r\n\t\t\"tipo_sanguineo\": \"A-\",\r\n\t\t\"cor\": \"verde\"\r\n\t}\r\n]\r\n}"
            };
            var retorno = new PostApiRetorno<long>();
            ParamWebApi<Cliente_ProspectoDadosGeraisViewModel> request = new ParamWebApi<Cliente_ProspectoDadosGeraisViewModel>
            {
                Objeto = obj
            };
            var Cliente_ProspectoApiController = new Cliente_ProspectoApiController();

            retorno = await Cliente_ProspectoApiController.Gravar(request);
            if (!retorno.Ok)
            {
                string message = "Problema na execução da Api";
                Assert.Fail(message);
            }

        }
    }
}
