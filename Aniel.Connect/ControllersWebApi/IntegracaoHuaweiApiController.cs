using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Renci.SshNet;
using Sinapse.DAO.repository;
using Sinapse.Util.RemoteConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class IntegracaoHuaweiApiController : BaseApiController
    {
        [HttpGet]
        [Route("api/IntegracaoHuaweiApi/ONUsPendentesRegistro")]
        public async Task<DataTableResponse<AutorizarONUListModel>> ONUsPendentesRegistro(Int64 Id_OLT)
        {
            var retorno = new DataTableResponse<AutorizarONUListModel>();
            var listaONU = new List<AutorizarONUListModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoSlot = new Conexao<Slot>(session))
                using (var reposPon = new Conexao<PON>(session))
                {
                    OLT olt = await repository.selectFirst(p => p.Id == Id_OLT);

                    var protocolo = olt.PortaSSH.HasValue ? Protocol.Ssh : Protocol.Telnet;
                    var porta = protocolo == Protocol.Ssh ? olt.PortaSSH.Value : olt.PortaTelnet.Value;

                    using (var rc = new RemoteConnection(olt.IP, porta, olt.Login, olt.Senha, protocolo))
                    {
                        rc.Connect();

                        if (rc.IsConnected()) {
                            if (protocolo == Protocol.Ssh)
                            {
                                rc.InteractiveExecute<ShellStream>((shellStream) =>
                                {
                                    shellStream.WriteLine("scroll");
                                    shellStream.WriteLine("\r");

                                    shellStream.WriteLine("enable");
                                    shellStream.WriteLine("config");

                                    shellStream.WriteLine("cls");

                                    // Formato dos registros de retorno da ONU:
                                    //
                                    // ------------------------------ (separador de registros)
                                    // Number               : (int)
                                    // F/S/P                : (Num OLT: int)/(Num Slot: int)/(Num PON: int)
                                    // Ont SN               : (string)
                                    // Password             : (hex)
                                    // Loid                 :
                                    // Checkcode            :
                                    // VendorID             : (string)
                                    // Ont Version          : (string)
                                    // Ont Software Version : (string)
                            //        // Ont EquipmentID      : (int)
                            //        // Ont Customized Info  : (string)
                            //        // Ont autofind time    : (Date)

                                    shellStream.WriteLine("display ont autofind all");
                                    shellStream.WriteLine("\r");

                                    string currLine;
                                    AutorizarONUListModel currOnu = null;

                                    while ((currLine = shellStream.ReadLine()) != null)
                                    {
                                        if (currLine.Contains("F/S/P"))
                                        {
                                            var lineArray = currLine.Split(':');

                                            var fsp = lineArray[lineArray.Length - 1];
                                            var fspArray = fsp.Split('/');

                                            var numSlot = fspArray[1];
                                            var numPon = fspArray[2];
                            //                // Essa linha é a primeira da qual a gente extrai os dados.
                            //                // Se já houver um registro de ONU, vamos adicioná-lo à lista do retorno

                                            var slot = repoSlot.selectFirst(p => p.OLT == olt &&
                                                                                 p.NumSlot == Convert.ToInt64(numSlot)).Result;

                                            var pon = reposPon.selectFirst(p => p.OLT == olt &&
                                                                                p.Slot == slot &&
                                                                                p.Num_PON == Convert.ToInt64(numPon)).Result;

                                            // Essa linha é a primeira da qual a gente extrai os dados.
                                            // Se já houver um registro de ONU, vamos adicioná-lo à lista do retorno

                            //                if (currOnu != null)
                            //                    listaONU.Add(currOnu);
                                            if (currOnu != null)
                                                listaONU.Add(currOnu);

                            //                // Inicialzando um novo registro de ONU

                                            currOnu = new AutorizarONUListModel
                                            {
                                                Id = fsp,

                                                OLT = olt.NumOLT,
                                                Slot = numSlot,
                                                PON = numPon,

                                                Desc_OLT = olt.Descricao,
                                                Desc_Slot = slot?.Descricao,
                                                Desc_PON = pon?.Descricao
                                            };
                                        }
                                        else if (currLine.Contains("Ont SN"))
                                        {
                                            var lineArray = currLine.Split(':');
                                            var value = lineArray[lineArray.Length - 1].Trim();

                                            currOnu.Mac_Serial = serial;
                                        }
                                        else if (currLine.Contains("The number of GPON autofind is"))
                                        {
                                            break;
                                        }
                                    }
                                });
                            }
                        }
                        else
                        {
                            throw new Exception("Erro ao estabelecer conexão com a OLT");
                        }
                    }

                    retorno.data = listaONU;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);

                //var data = new List<AutorizarONUListModel>();

                //data.Add(new AutorizarONUListModel
                //{
                //    Id = 7,
                //    OLT = "Teste API",
                //    Slot = "12",
                //    PON = "22",
                //    Mac_Serial = "00-88-14-4D-4C-FB"
                //});

                retorno.error = ex.Message;
            }

            return retorno;

        }
    }
}