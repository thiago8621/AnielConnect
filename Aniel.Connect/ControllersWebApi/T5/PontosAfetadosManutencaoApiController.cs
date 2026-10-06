using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.PontosAfetados;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.mongo;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MongoDB.Driver;
using NHibernate;
using OpenXmlPowerTools;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Xceed.Words.NET;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PontosAfetadosManutencaoApiController : BaseApiController
    {
        /// <summary>
        /// Obtem todos os Pontos de Manutenções Afetados
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PontosAfetadosManutencaoApi/GetAllPontosAfetados")]
        public async Task<PostApiRetorno<PontosAfetadosManutencaoMongo>> GetAllPontosAfetados(PontosAfetadosRequestModel request)
        {
            PostApiRetorno<PontosAfetadosManutencaoMongo> response = new PostApiRetorno<PontosAfetadosManutencaoMongo>();

            try
            {
                var doc = new PontosAfetadosManut
                {
                    Codct = request.Codct,
                    Projeto = request.Projeto,
                    NumDoc = request.NumDoc,
                    NumObra = request.NumObra
                };

                response.IdObjeto = await GetPontosAfetadoMongo(doc);
            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Ok = false;
                response.Mensagem = ex.Message;
                return response;
            }

            response.Ok = true;
            return response;
        }


        public async Task<PostApiRetorno<bool>> GravarPontosAfetados(PontosAfetadosManut request)
        {
            PostApiRetorno<bool> response = new PostApiRetorno<bool>();

            using (var areaManutApi = new Area_ManutencaoApiController())
            {

                try
                {
                    var pontosAfetados = await GetPontosAfetadoMongo(request);

                    if (pontosAfetados != null)
                    {
                        var ponto = pontosAfetados?.pops != null ? pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault() : null;

                        var tipoElementoRede = pontosAfetados?.tiposElementosRede != null ? pontosAfetados?.tiposElementosRede
                                                                                                 .Where(x => x.Id_Tipo == request.IdTipoElementoRede)
                                                                                                 .FirstOrDefault() : null;
                        //endereco, condominios
                        (List<long?>, List<long?>) tipoEnderecamento = (null, null);

                        if (pontosAfetados?.enderecosAfetados != null)
                        {
                            tipoEnderecamento.Item1 = new List<long?>();
                            tipoEnderecamento.Item1.AddRange(pontosAfetados?.enderecosAfetados.Ids_Enderecos);

                            tipoEnderecamento.Item2 = new List<long?>();
                            tipoEnderecamento.Item2.AddRange(pontosAfetados?.enderecosAfetados.Ids_Condominios);
                        }

                        //Agrupamento de IDs para evitar duplicidade
                        request.Ids = request.Ids != null ? request.Ids.GroupBy(x => x).Select(g => g.First()).ToList() : new List<Int64?>();

                        string[] array_tipo = new string[] { "RADIUS", "CONCENTRADORA", "OLT", "SLOT", "PON" };
                        if ((request.IdPop != null && request.Ids.Count == 0) || (request.IdPop != null && request.Ids.Count > 0 && array_tipo.Contains(request.Tipo)))
                        {
                            if (request.Tipo.ToUpper().Contains("TIPO_"))
                            {
                                request.Tipo = request.Tipo.Substring(5);
                            }
                            if (ponto != null)
                            {
                                switch (request.Tipo)
                                {
                                    case "RADIUS":
                                        {
                                            var novosRadiusAdc = new List<long?>();
                                            if (ponto.idsRadius is null)
                                                ponto.idsRadius = new List<long?>();

                                            if (request.Ids == null || request?.Ids?.Count == 0)
                                            {
                                                var buscar_radius_relacionadas = areaManutApi.BuscarRadiusPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                                                foreach (var item in buscar_radius_relacionadas)
                                                {
                                                    if (!ponto.idsRadius.Contains(item))
                                                        novosRadiusAdc.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                foreach (var radius in request.Ids)
                                                {
                                                    if (!ponto.idsRadius.Contains(radius))
                                                        novosRadiusAdc.Add(radius);
                                                }
                                            }


                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsRadius.AddRange(novosRadiusAdc);
                                            break;
                                        }

                                    case "CONCENTRADORA":
                                        {
                                            var novasConcentradorasAdc = new List<long?>();
                                            if (ponto.idsConcentradoras is null)
                                                ponto.idsConcentradoras = new List<long?>();

                                            if (request.Ids == null || request?.Ids?.Count == 0)
                                            {
                                                var buscar_concentradores_relacionadas = areaManutApi.BuscarConcentradorasPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                                                foreach (var item in buscar_concentradores_relacionadas)
                                                {
                                                    if (!ponto.idsConcentradoras.Contains(item))
                                                        novasConcentradorasAdc.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                foreach (var concen in request.Ids)
                                                {
                                                    if (!ponto.idsConcentradoras.Contains(concen))
                                                        novasConcentradorasAdc.Add(concen);
                                                }
                                            }


                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsConcentradoras.AddRange(novasConcentradorasAdc);
                                            break;
                                        }

                                    case "OLT":
                                        {
                                            var novosOLTAdc = new List<long?>();
                                            var novosSLOTAdc = new List<long?>();
                                            var novosPONAdc = new List<long?>();

                                            if (ponto.idsOlts is null)
                                                ponto.idsOlts = new List<long?>();

                                            if (request.Ids == null || request?.Ids?.Count == 0)
                                            {
                                                var buscar_olt_relacionadas = areaManutApi.BuscarOLTSPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                                                foreach (var item in buscar_olt_relacionadas)
                                                {
                                                    if (!ponto.idsOlts.Contains(item))
                                                        novosOLTAdc.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                foreach (var olt in request.Ids)
                                                {
                                                    if (!ponto.idsOlts.Contains(olt))
                                                        novosOLTAdc.Add(olt);
                                                }
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsOlts.AddRange(novosOLTAdc);

                                            var slotsRelacionados = areaManutApi.BuscarSlotsOLT(new PopRequestManut
                                            {
                                                IdsOlts = pontosAfetados.pops
                                                .Where(x => x.IdPop == request.IdPop)
                                                .FirstOrDefault().idsOlts,
                                                IdPop = request.IdPop.Value
                                            }).Result.IdObjeto.Select(x => x.Id);

                                            foreach (var slot in slotsRelacionados)
                                            {
                                                if (ponto.idsSlots is null)
                                                    ponto.idsSlots = new List<long?>();

                                                if (!ponto.idsSlots.Contains(slot))
                                                    novosSLOTAdc.Add(slot);
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsSlots.AddRange(novosSLOTAdc);

                                            var ponsRelacionados = areaManutApi.BuscarPonsSlots(new PopRequestManut
                                            {
                                                IdsSlots = pontosAfetados.pops
                                                .Where(x => x.IdPop == request.IdPop)
                                                .FirstOrDefault().idsSlots,
                                                IdPop = request.IdPop.Value
                                            }).Result.IdObjeto.Select(x => x.Id);

                                            foreach (var pon in ponsRelacionados)
                                            {
                                                if (ponto.idsPons is null)
                                                    ponto.idsPons = new List<long?>();

                                                if (!ponto.idsPons.Contains(pon))
                                                    novosPONAdc.Add(pon);
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsPons.AddRange(novosPONAdc);
                                            break;
                                        }

                                    case "SLOT":
                                        {
                                            var novosSLOTAdc = new List<long?>();
                                            var novosPONAdc = new List<long?>();

                                            foreach (var slot in request.Ids)
                                            {
                                                if (ponto.idsSlots is null)
                                                    ponto.idsSlots = new List<long?>();

                                                if (!ponto.idsSlots.Contains(slot))
                                                    novosSLOTAdc.Add(slot);
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsSlots.AddRange(novosSLOTAdc);

                                            var ponsRelacionados = areaManutApi.BuscarPonsSlots(new PopRequestManut
                                            {
                                                IdsSlots = pontosAfetados.pops
                                              .Where(x => x.IdPop == request.IdPop)
                                              .FirstOrDefault().idsSlots,
                                                IdPop = request.IdPop.Value
                                            }).Result.IdObjeto.Select(x => x.Id);

                                            foreach (var pon in ponsRelacionados)
                                            {
                                                if (ponto.idsPons is null)
                                                    ponto.idsPons = new List<long?>();

                                                if (!ponto.idsPons.Contains(pon))
                                                    novosPONAdc.Add(pon);
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsPons.AddRange(novosPONAdc);
                                            break;
                                        }

                                    case "PON":
                                        {
                                            var novosPONAdc = new List<long?>();

                                            foreach (var pon in request.Ids)
                                            {
                                                if (ponto.idsPons is null)
                                                    ponto.idsPons = new List<long?>();

                                                if (!ponto.idsPons.Contains(pon))
                                                    novosPONAdc.Add(pon);
                                            }

                                            pontosAfetados.pops.Where(x => x.IdPop == request.IdPop).FirstOrDefault().idsPons.AddRange(novosPONAdc);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                var popAfetado = await CriarPontoAfetadoPop(request);

                                if (pontosAfetados.pops == null)
                                    pontosAfetados.pops = new List<PopsAfetadosMongo>();


                                pontosAfetados.pops.Add(popAfetado);
                            }

                            pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                            {
                                Pops = pontosAfetados.pops,
                                EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                TiposElementosRede = pontosAfetados.tiposElementosRede
                            });

                            await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                        }
                        else if (request.Ids != null && request.Tipo.ToUpper().Contains("POP"))
                        {
                            if(pontosAfetados.pops == null)
                                pontosAfetados.pops = new List<PopsAfetadosMongo>();

                            if (request.IdsTipos?.Count > 1 && request.IdsTipos != null)
                            {
                                foreach (var item in request.IdsTipos)
                                {
                                    var popAfetado = await CriarPontoAfetadoPop(new PontosAfetadosManut { IdPop = request.IdPop, Tipo = item.Substring(5) });
                                    pontosAfetados.pops.Add(popAfetado);
                                }
                            }
                            else
                            {
                                foreach (var item in request.Ids)
                                {
                                    var popAfetado = await CriarPontoAfetadoPop(new PontosAfetadosManut { IdPop = item, Tipo = "POP" });
                                    pontosAfetados.pops.Add(popAfetado);
                                }
                            }

                            pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                            {
                                Pops = pontosAfetados.pops,
                                EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                TiposElementosRede = pontosAfetados.tiposElementosRede
                            });
                            await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                        }
                        else if (request.IdTipoElementoRede != null)
                        {
                            if (tipoElementoRede != null)
                            {
                                switch (request.Tipo)
                                {
                                    case "TIPO_ELEMENTO":
                                        var novosTiposElementos = new List<TipoElementosRedeMongo>();

                                        foreach (var elem in request.Ids)
                                        {
                                            if (!pontosAfetados.tiposElementosRede.Any(x => x.Id_Tipo == elem))
                                            {
                                                var novoTipo = new TipoElementosRedeMongo
                                                {
                                                    //Id_Tipo = request.IdTipoElementoRede,
                                                    Id_Tipo = elem,
                                                    ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                                };

                                                var elementosDoTipo = areaManutApi.BuscarElementos(new ElementoRedeRequestManut { IdTipoElemento = elem.Value }).Result.IdObjeto;

                                                foreach (var item in elementosDoTipo)
                                                {
                                                    var novoElemento = new ElementosRedeAfetadosMongo
                                                    {
                                                        Id = item.Id,
                                                        Ids_Portas = item.IsCaixa ? new List<long>() : null
                                                    };


                                                    if (item.IsCaixa)
                                                    {
                                                        if (novoElemento.Ids_Portas == null)
                                                            novoElemento.Ids_Portas = new List<long>();
                                                    }

                                                    var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                            .Result.IdObjeto.Select(x => x.Id).ToList();

                                                    if (portas != null && portas.Count > 0)
                                                    {
                                                        novoElemento.Ids_Portas.AddRange(portas);
                                                    }

                                                    novoTipo.ElementosRede.Add(novoElemento);
                                                }

                                                novosTiposElementos.Add(novoTipo);
                                            }
                                            else
                                            {
                                                var tipoElementoAdicionado = pontosAfetados.tiposElementosRede.Where(x => x.Id_Tipo == elem).FirstOrDefault();
                                                var elementosDoTipo = areaManutApi.BuscarElementos(new ElementoRedeRequestManut { IdTipoElemento = elem.Value }).Result.IdObjeto;

                                                foreach (var item in elementosDoTipo)
                                                {
                                                    if (!tipoElementoAdicionado.ElementosRede.Any(x => x.Id == item.Id))
                                                    {
                                                        var novoElemento = new ElementosRedeAfetadosMongo
                                                        {
                                                            Id = item.Id,
                                                            Ids_Portas = item.IsCaixa ? new List<long>() : null
                                                        };

                                                        if (item.IsCaixa)
                                                        {
                                                            if (novoElemento.Ids_Portas == null)
                                                                novoElemento.Ids_Portas = new List<long>();
                                                        }

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                                .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        if (portas != null && portas.Count > 0)
                                                        {
                                                            novoElemento.Ids_Portas.AddRange(portas);
                                                        }

                                                        pontosAfetados.tiposElementosRede.Where(x => x == tipoElementoAdicionado)
                                                                                .FirstOrDefault()
                                                                                .ElementosRede
                                                                                .Add(novoElemento);
                                                    }
                                                    else
                                                    {
                                                        var elementoJaAdicionado = tipoElementoAdicionado.ElementosRede.FirstOrDefault(x => x.Id == item.Id);

                                                        if (item.IsCaixa)
                                                            if (elementoJaAdicionado.Ids_Portas == null)
                                                                elementoJaAdicionado.Ids_Portas = new List<long>();

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = elementoJaAdicionado.Id })
                                                                                .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        var novasPortas = new List<long>();
                                                        if (portas != null && portas.Count > 0)
                                                        {
                                                            foreach (var pt in portas)
                                                                if (!elementoJaAdicionado.Ids_Portas.Contains(pt))
                                                                    novasPortas.Add(pt);

                                                            pontosAfetados.tiposElementosRede.Where(x => x == tipoElementoAdicionado)
                                                                           .FirstOrDefault()
                                                                           .ElementosRede
                                                                           .Where(y => y == elementoJaAdicionado)
                                                                           .FirstOrDefault()
                                                                           .Ids_Portas
                                                                           .AddRange(novasPortas);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (novosTiposElementos.Count > 0)
                                        {
                                            if (pontosAfetados.tiposElementosRede == null)
                                            {
                                                pontosAfetados.tiposElementosRede = new List<TipoElementosRedeMongo>();
                                                pontosAfetados.tiposElementosRede.AddRange(novosTiposElementos);
                                            }
                                            else
                                            {
                                                pontosAfetados.tiposElementosRede.AddRange(novosTiposElementos);
                                            }
                                        }

                                        break;
                                    case "ELEMENTO":
                                        {
                                            var novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                            foreach (var elem in request.ElementosRedeRequests)
                                            {
                                                if (!tipoElementoRede.ElementosRede.Any(x => x.Id == elem.IdElemento))
                                                {
                                                    var novoElemento = new ElementosRedeAfetadosMongo();
                                                    novoElemento.Id = elem.IdElemento;

                                                    if (elem.isCaixa == "SIM")
                                                    {
                                                        novoElemento.Ids_Portas = new List<long>();

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                                 .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        if (portas != null && portas.Count > 0)
                                                            novoElemento.Ids_Portas.AddRange(portas);
                                                    }

                                                    novosElementosRede.Add(novoElemento);
                                                }
                                                else
                                                {
                                                    var novoElemento = tipoElementoRede.ElementosRede.Where(x => x.Id == elem.IdElemento).FirstOrDefault();
                                                    List<long> portasNovas = new List<long>();

                                                    if (elem.isCaixa == "SIM")
                                                    {
                                                        if (novoElemento.Ids_Portas == null)
                                                            novoElemento.Ids_Portas = new List<long>();

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                                 .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        if (portas != null && portas.Count > 0)
                                                        {
                                                            foreach (var porta in portas)
                                                            {
                                                                if (!novoElemento.Ids_Portas.Contains(porta))
                                                                {
                                                                    portasNovas.Add(porta);
                                                                }
                                                            }
                                                        }
                                                    }

                                                    pontosAfetados.tiposElementosRede.Where(x => x.Id_Tipo == request.IdTipoElementoRede)
                                                                                     .FirstOrDefault()
                                                                                     .ElementosRede
                                                                                     .Where(x => x.Id == elem.IdElemento)
                                                                                     .FirstOrDefault()
                                                                                     .Ids_Portas
                                                                                     .AddRange(portasNovas);
                                                }
                                            }

                                            if (novosElementosRede.Count > 0)
                                            {
                                                pontosAfetados.tiposElementosRede.Where(x => x.Id_Tipo == request.IdTipoElementoRede)
                                                                                 .FirstOrDefault()
                                                                                 .ElementosRede
                                                                                 .AddRange(novosElementosRede);
                                            }
                                            break;
                                        }
                                    case "PORTA":
                                        {
                                            List<ElementosRedeAfetadosMongo> novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                            foreach (var elem in request.ElementosRedeRequests)
                                            {
                                                List<long> portasNovas = new List<long>();

                                                if (!tipoElementoRede.ElementosRede.Any(x => x.Id == elem.IdElemento))
                                                {
                                                    var novoElemento = new ElementosRedeAfetadosMongo();
                                                    novoElemento.Id = elem.IdElemento;

                                                    if (elem.isCaixa == "SIM")
                                                    {
                                                        novoElemento.Ids_Portas = new List<long>();

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                                 .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        portas = portas.Where(id => elem.idsPortas.Contains(id)).ToList();

                                                        if (portas != null && portas.Count > 0)
                                                            novoElemento.Ids_Portas.AddRange(portas);
                                                    }

                                                    novosElementosRede.Add(novoElemento);
                                                }
                                                else
                                                {
                                                    var novoElemento = tipoElementoRede.ElementosRede.Where(x => x.Id == elem.IdElemento).FirstOrDefault();

                                                    if (elem.isCaixa == "SIM")
                                                    {
                                                        if (novoElemento.Ids_Portas == null)
                                                            novoElemento.Ids_Portas = new List<long>();

                                                        var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                                 .Result.IdObjeto.Select(x => x.Id).ToList();

                                                        portas = portas.Where(id => elem.idsPortas.Contains(id)).ToList();

                                                        if (portas != null && portas.Count > 0)
                                                        {
                                                            foreach (var porta in portas)
                                                            {
                                                                if (!novoElemento.Ids_Portas.Contains(porta))
                                                                {
                                                                    portasNovas.Add(porta);
                                                                }
                                                            }
                                                        }
                                                    }

                                                    pontosAfetados.tiposElementosRede.Where(x => x.Id_Tipo == request.IdTipoElementoRede)
                                                                                 .FirstOrDefault()
                                                                                 .ElementosRede
                                                                                 .Where(x => x.Id == elem.IdElemento)
                                                                                 .FirstOrDefault()
                                                                                 .Ids_Portas
                                                                                 .AddRange(portasNovas);
                                                }
                                            }

                                            if (novosElementosRede.Count > 0)
                                            {
                                                pontosAfetados.tiposElementosRede.Where(x => x.Id_Tipo == request.IdTipoElementoRede)
                                                                                 .FirstOrDefault()
                                                                                 .ElementosRede
                                                                                 .AddRange(novosElementosRede);
                                            }
                                            break;
                                        }
                                }

                                pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                                {
                                    Pops = pontosAfetados.pops,
                                    EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                    TiposElementosRede = pontosAfetados.tiposElementosRede
                                });
                                await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                            }
                            else
                            {
                                var novosTiposElementos = new List<TipoElementosRedeMongo>();
                                switch (request.Tipo)
                                {
                                    case "TIPO_ELEMENTO":
                                        foreach (var elem in request.Ids)
                                        {
                                            var novoTipo = new TipoElementosRedeMongo
                                            {
                                                //Id_Tipo = request.IdTipoElementoRede,
                                                Id_Tipo = elem,
                                                ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                            };

                                            var elementosDoTipo = areaManutApi.BuscarElementos(new ElementoRedeRequestManut { IdTipoElemento = elem.Value }).Result.IdObjeto;

                                            foreach (var item in elementosDoTipo)
                                            {
                                                var novoElemento = new ElementosRedeAfetadosMongo
                                                {
                                                    Id = item.Id,
                                                    Ids_Portas = item.IsCaixa ? new List<long>() : null
                                                };


                                                if (item.IsCaixa)
                                                {
                                                    if (novoElemento.Ids_Portas == null)
                                                        novoElemento.Ids_Portas = new List<long>();
                                                }

                                                var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                        .Result.IdObjeto.Select(x => x.Id).ToList();

                                                if (portas != null && portas.Count > 0)
                                                {
                                                    novoElemento.Ids_Portas.AddRange(portas);
                                                }

                                                novoTipo.ElementosRede.Add(novoElemento);
                                            }

                                            novosTiposElementos.Add(novoTipo);
                                        }
                                        break;
                                    case "ELEMENTO":
                                        {
                                            var novoTipo = new TipoElementosRedeMongo();
                                            var lista_novo_tipo = new List<TipoElementosRedeMongo>();
                                            if (request.Ids?.Count > 1)
                                            {
                                                foreach (var item in request.Ids)
                                                {
                                                    novoTipo = new TipoElementosRedeMongo
                                                    {
                                                        Id_Tipo = item,
                                                        ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                                    };
                                                    lista_novo_tipo.Add(novoTipo);
                                                }
                                            }
                                            else
                                            {
                                                novoTipo = new TipoElementosRedeMongo
                                                {
                                                    Id_Tipo = request.IdTipoElementoRede,
                                                    ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                                };
                                            }

                                            var novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                            foreach (var elem in request.ElementosRedeRequests)
                                            {
                                                var novoElemento = new ElementosRedeAfetadosMongo();
                                                novoElemento.Id = elem.IdElemento;

                                                if (elem.isCaixa == "SIM")
                                                {
                                                    novoElemento.Ids_Portas = new List<long>();

                                                    var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                             .Result.IdObjeto.Select(x => x.Id).ToList();

                                                    if (portas != null && portas.Count > 0)
                                                        novoElemento.Ids_Portas.AddRange(portas);
                                                }


                                                novosElementosRede.Add(novoElemento);
                                            }

                                            novoTipo.ElementosRede.AddRange(novosElementosRede);
                                            if (lista_novo_tipo.Count > 0)
                                                novosTiposElementos.AddRange(lista_novo_tipo);
                                            else
                                                novosTiposElementos.Add(novoTipo);

                                            break;
                                        }
                                    case "PORTA":
                                        {
                                            var novoTipo = new TipoElementosRedeMongo
                                            {
                                                Id_Tipo = request.IdTipoElementoRede,
                                                ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                            };

                                            List<ElementosRedeAfetadosMongo> novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                            foreach (var elem in request.ElementosRedeRequests)
                                            {
                                                List<long> portasNovas = new List<long>();

                                                var novoElemento = new ElementosRedeAfetadosMongo();
                                                novoElemento.Id = elem.IdElemento;

                                                if (elem.isCaixa == "SIM")
                                                {
                                                    novoElemento.Ids_Portas = new List<long>();
                                                    var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                 .Result.IdObjeto.Select(x => x.Id).ToList();

                                                    portas = portas.Where(id => elem.idsPortas.Contains(id)).ToList();

                                                    if (portas != null && portas.Count > 0)
                                                        novoElemento.Ids_Portas.AddRange(portas);
                                                }

                                                novosElementosRede.Add(novoElemento);
                                            }

                                            novoTipo.ElementosRede.AddRange(novosElementosRede);
                                            novosTiposElementos.Add(novoTipo);
                                        }
                                        break;
                                }

                                if (pontosAfetados.tiposElementosRede?.Count > 0)
                                {
                                    pontosAfetados.tiposElementosRede.AddRange(novosTiposElementos);
                                }
                                else if (novosTiposElementos.Count > 0)
                                {
                                    pontosAfetados.tiposElementosRede = new List<TipoElementosRedeMongo>();
                                    pontosAfetados.tiposElementosRede.AddRange(novosTiposElementos);
                                }
                            }

                            pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                            {
                                Pops = pontosAfetados.pops,
                                EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                TiposElementosRede = pontosAfetados.tiposElementosRede
                            });
                            await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                        }
                        else
                        {
                            //item1 = endereco, item2 = condominios
                            if (tipoEnderecamento.Item1 != null || tipoEnderecamento.Item2 != null)
                            {
                                var novosEnderecos = new List<long?>();

                                if (request.Tipo == "ENDERECO")
                                {
                                    foreach (var idEndereco in request.Ids)
                                    {
                                        if (tipoEnderecamento.Item1 == null)
                                            novosEnderecos.Add(idEndereco);
                                        else if (!tipoEnderecamento.Item1.Any(x => x.Value == idEndereco))
                                            novosEnderecos.Add(idEndereco);
                                    }

                                    if (novosEnderecos.Count > 0)
                                        pontosAfetados.enderecosAfetados.Ids_Enderecos.AddRange(novosEnderecos);

                                    pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                                    {
                                        Pops = pontosAfetados.pops,
                                        EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                        TiposElementosRede = pontosAfetados.tiposElementosRede
                                    });
                                    await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                                }
                                else if (request.Tipo == "CONDOMINIO")
                                {
                                    foreach (var idCondomio in request.Ids)
                                    {
                                        if (tipoEnderecamento.Item2 == null)
                                            novosEnderecos.Add(idCondomio);
                                        else if (!tipoEnderecamento.Item2.Any(x => x.Value == idCondomio))
                                            novosEnderecos.Add(idCondomio);
                                    }

                                    if (novosEnderecos.Count > 0)
                                        pontosAfetados.enderecosAfetados.Ids_Condominios.AddRange(novosEnderecos);

                                    pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                                    {
                                        Pops = pontosAfetados.pops,
                                        EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                        TiposElementosRede = pontosAfetados.tiposElementosRede
                                    });
                                    await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                                }
                            }
                            else
                            {
                                pontosAfetados.enderecosAfetados = new EnderecosAfetadosMongo()
                                {
                                    Ids_Condominios = new List<long?>(),
                                    Ids_Enderecos = new List<long?>()
                                };

                                if (request.Tipo == "ENDERECO")
                                    pontosAfetados.enderecosAfetados.Ids_Enderecos.AddRange(request.Ids);
                                else if (request.Tipo == "CONDOMINIO")
                                    pontosAfetados.enderecosAfetados.Ids_Condominios.AddRange(request.Ids);


                                pontosAfetados.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                                {
                                    Pops = pontosAfetados.pops,
                                    EnderecosAfetados = pontosAfetados.enderecosAfetados,
                                    TiposElementosRede = pontosAfetados.tiposElementosRede
                                });
                                await UpdatePontosAfetadosManutencaoMongo(pontosAfetados);
                            }
                        }
                    }
                    else
                    {
                        PontosAfetadosManutencaoMongo pontosAfetadosManut = new PontosAfetadosManutencaoMongo()
                        {
                            codct = request.Codct,
                            projeto = request.Projeto,
                            numDoc = request.NumDoc,
                            numObra = request.NumObra,
                        };

                        if (request.IdPop != null)
                        {
                            pontosAfetadosManut.pops = new List<PopsAfetadosMongo>();
                            var popAfetado = await CriarPontoAfetadoPop(request);
                            pontosAfetadosManut.pops.Add(popAfetado);
                        }
                        else if (request.Ids != null && request.Tipo.ToUpper().Contains("POP"))
                        {
                            pontosAfetadosManut.pops = new List<PopsAfetadosMongo>();
                            foreach (var item in request.Ids)
                            {
                                var popAfetado = await CriarPontoAfetadoPop(new PontosAfetadosManut { IdPop = item, Tipo = "POP" });
                                pontosAfetadosManut.pops.Add(popAfetado);
                            }
                        }
                        else if (request.IdTipoElementoRede != null)
                        {
                            var novosTiposElementos = new List<TipoElementosRedeMongo>();
                            switch (request.Tipo)
                            {
                                case "TIPO_ELEMENTO":
                                    foreach (var elem in request.Ids)
                                    {
                                        var novoTipo = new TipoElementosRedeMongo
                                        {
                                            Id_Tipo = request.IdTipoElementoRede,
                                            ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                        };

                                        var elementosDoTipo = areaManutApi.BuscarElementos(new ElementoRedeRequestManut { IdElemento = elem.Value }).Result.IdObjeto;

                                        foreach (var item in elementosDoTipo)
                                        {
                                            var novoElemento = new ElementosRedeAfetadosMongo
                                            {
                                                Id = item.Id,
                                                Ids_Portas = item.IsCaixa ? new List<long>() : null
                                            };


                                            if (item.IsCaixa)
                                            {
                                                if (novoElemento.Ids_Portas == null)
                                                    novoElemento.Ids_Portas = new List<long>();
                                            }

                                            var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                    .Result.IdObjeto.Select(x => x.Id).ToList();

                                            if (portas != null && portas.Count > 0)
                                            {
                                                novoElemento.Ids_Portas.AddRange(portas);
                                            }

                                            novoTipo.ElementosRede.Add(novoElemento);
                                        }

                                        novosTiposElementos.Add(novoTipo);

                                    }
                                    break;
                                case "ELEMENTO":
                                    {
                                        var novoTipo = new TipoElementosRedeMongo
                                        {
                                            Id_Tipo = request.IdTipoElementoRede,
                                            ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                        };

                                        var novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                        foreach (var elem in request.ElementosRedeRequests)
                                        {
                                            var novoElemento = new ElementosRedeAfetadosMongo();
                                            novoElemento.Id = elem.IdElemento;

                                            if (elem.isCaixa == "SIM")
                                            {
                                                novoElemento.Ids_Portas = new List<long>();

                                                var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                                         .Result.IdObjeto.Select(x => x.Id).ToList();

                                                if (portas != null && portas.Count > 0)
                                                    novoElemento.Ids_Portas.AddRange(portas);
                                            }


                                            novosElementosRede.Add(novoElemento);
                                        }

                                        novoTipo.ElementosRede.AddRange(novosElementosRede);
                                        novosTiposElementos.Add(novoTipo);
                                        break;
                                    }
                                case "PORTA":
                                    {
                                        var novoTipo = new TipoElementosRedeMongo
                                        {
                                            Id_Tipo = request.IdTipoElementoRede,
                                            ElementosRede = new List<ElementosRedeAfetadosMongo>()
                                        };

                                        List<ElementosRedeAfetadosMongo> novosElementosRede = new List<ElementosRedeAfetadosMongo>();

                                        foreach (var elem in request.ElementosRedeRequests)
                                        {
                                            List<long> portasNovas = new List<long>();

                                            var novoElemento = new ElementosRedeAfetadosMongo();
                                            novoElemento.Id = elem.IdElemento;

                                            if (elem.isCaixa == "SIM")
                                            {
                                                novoElemento.Ids_Portas = new List<long>();
                                                var portas = areaManutApi.BuscarPortasElementoRede(new ElementoRedeRequestManut() { IdElemento = novoElemento.Id })
                                                             .Result.IdObjeto.Select(x => x.Id).ToList();

                                                if (portas != null && portas.Count > 0)
                                                    novoElemento.Ids_Portas.AddRange(portas);
                                            }

                                            novosElementosRede.Add(novoElemento);
                                        }

                                        novoTipo.ElementosRede.AddRange(novosElementosRede);
                                        novosTiposElementos.Add(novoTipo);
                                    }
                                    break;
                            }

                            if (novosTiposElementos.Count > 0)
                            {
                                pontosAfetadosManut.tiposElementosRede = new List<TipoElementosRedeMongo>();
                                pontosAfetadosManut.tiposElementosRede.AddRange(novosTiposElementos);
                            }
                        }
                        else if (request.Ids != null && (request.Tipo == "ENDERECO" || request.Tipo == "CONDOMINIO"))
                        {
                            pontosAfetadosManut.enderecosAfetados = new EnderecosAfetadosMongo()
                            {
                                Ids_Condominios = new List<long?>(),
                                Ids_Enderecos = new List<long?>()
                            };

                            if (request.Tipo == "ENDERECO")
                                pontosAfetadosManut.enderecosAfetados.Ids_Enderecos.AddRange(request.Ids);
                            else if (request.Tipo == "CONDOMINIO")
                                pontosAfetadosManut.enderecosAfetados.Ids_Condominios.AddRange(request.Ids);
                        }

                        pontosAfetadosManut.clientesAfetados = await GetClientesVinculados(new PontosAfetadosRequest
                        {
                            Pops = pontosAfetadosManut.pops,
                            EnderecosAfetados = pontosAfetadosManut.enderecosAfetados,
                            TiposElementosRede = pontosAfetadosManut.tiposElementosRede
                        });

                        await SavePontosAfetadosManutMongo(pontosAfetadosManut);
                    }
                }
                catch (Exception e)
                {
                    response.Ok = false;
                    response.Mensagem = e.Message;
                    response.IdObjeto = false;
                    return response;
                }

            }
            response.Ok = true;
            response.Mensagem = "Sucesso!";
            response.IdObjeto = true;
            return response;
        }


        private async Task<ClientesAfetadosMongo> ObterClientesAfetados(PontosAfetadosManutencaoMongo pontosAfetados)
        {

            var obterCliAfetados = await GetClientesVinculados(new PontosAfetadosRequest
            {
                //ElementosRede = pontosAfetados.elementosRede,
                Pops = pontosAfetados.pops
            });

            return obterCliAfetados;
        }

        /// <summary>
        /// Obtem todos os clientes vinculados aos Pontos de Manutenções Afetados
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/PontosAfetadosManutencaoApi/GetClientesVinculados")]
        public async Task<ClientesAfetadosMongo> GetClientesVinculados(PontosAfetadosRequest request)
        {
            ClientesAfetadosMongo clientesVinculados = new ClientesAfetadosMongo();
            List<DadosClientesAfetadosMongo> idsClientesTotal = new List<DadosClientesAfetadosMongo>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    if (request.Pops != null && request.Pops.Count > 0)
                    {
                        var idClientesPOP = await GetIdClientesByPOP(request, session);
                        idsClientesTotal.AddRange(idClientesPOP);
                    }

                    if (request.TiposElementosRede != null && request.TiposElementosRede.Count > 0)
                    {
                        var idClientesElementoRede = await GetIdClientesByElementoRede(request, sessionT5, session);
                        idsClientesTotal.AddRange(idClientesElementoRede);

                    }

                    if (request?.EnderecosAfetados?.Ids_Condominios?.Count > 0)
                    {
                        var idsClientesCondominios = await GetIdClientesByCondominio(request, session);
                        idsClientesTotal.AddRange(idsClientesCondominios);
                    }

                    if (request?.EnderecosAfetados?.Ids_Enderecos?.Count > 0)
                    {
                        var idsClientesEnderecos = await GetIdClientesByEnderecoLogradouro(request, session);
                        idsClientesTotal.AddRange(idsClientesEnderecos);
                    }


                    idsClientesTotal = idsClientesTotal
                                        .GroupBy(x => x.Id_Login_Radius)
                                        .Select(g => g.First())
                                        .ToList();

                    var countClientesTotal = idsClientesTotal.Count();
                    if (countClientesTotal > 0)
                    {
                        clientesVinculados.Clientes = idsClientesTotal;
                        clientesVinculados.Count = countClientesTotal;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



            //Função que retorna lista de clientes vinculados aos pontos afetados pela manutenção

            return clientesVinculados;

        }


        /// <summary>
        /// Obtém todos os clientes vinculados a uma OS que foram gravados no banco.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/PontosAfetadosManutencaoApi/GetClientesByOSManutencao")]
        public async Task<DataTableResponse<DadosClientesAfetadosMongo>> GetClientesByOSManutencao(GetClientesByOSManutencaoRequest request)
        {
            var retorno = new DataTableResponse<DadosClientesAfetadosMongo>();
            try
            {
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryPontos = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
                {
                    var clientes = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);
                    var clientesPaginados = new List<DadosClientesAfetadosMongo>();
                    var allClientes = new List<DadosClientesAfetadosMongo>();

                    if (clientes != null && clientes.clientesAfetados != null)
                    {
                        int pageNumber = request.start;
                        int pageSize = request.length;

                        allClientes = clientes.clientesAfetados.Clientes ?? new List<DadosClientesAfetadosMongo>();

                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            string searchLower = request.search.value.ToLower();
                            allClientes = allClientes
                                .Where(p => p.Cpfcnpj != null && p.Cpfcnpj.ToLower().Contains(searchLower))
                                .ToList();
                        }

                        clientesPaginados = allClientes
                            .Skip(request.start)
                            .Take(request.length)
                            .ToList();
                    }

                    retorno = new DataTableResponse<DadosClientesAfetadosMongo>
                    {
                        draw = request.draw,
                        recordsFiltered = allClientes.Count,
                        data = clientesPaginados,
                        error = ""
                    };
                }
            }
            catch (Exception ex)
            {
                retorno = new DataTableResponse<DadosClientesAfetadosMongo>
                {
                    draw = 0,
                    recordsFiltered = 0,
                    data = null,
                    error = $"{ex.Message}"
                };
            }

            return retorno;
        }

        /// <summary>
        /// Deleta ponto de manutenção e atualiza a listagem de clientes no banco.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/PontosAfetadosManutencaoApi/DeletePontoDeManutencao")]
        public async Task<Response> DeletePontoManutencao(DeletePontoAfetadoRequest request)
        {
            var response = new Response();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryPontos = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
                {

                    switch (request.Tipo)
                    {
                        case "POP":
                            {
                                await repositoryPontos.DeleteItemFromArray<PopsAfetadosMongo>(
                                doc => doc.codct == request.Codct &&
                                        doc.projeto == request.Projeto &&
                                        doc.numDoc == request.NumDoc &&
                                        doc.numObra == request.NumObra,
                                doc => doc.pops,
                                pop => pop.IdPop == request.Id
                                );
                                break;
                            }
                        case "RADIUS":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;

                                    foreach (var pop in document.pops)
                                    {
                                        if (pop.idsRadius != null && pop.idsRadius.Contains(request.Id))
                                        {
                                            pop.idsRadius.Remove(request.Id);
                                            updated = true;
                                            break;
                                        }
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }
                                break;
                            }

                        case "CONCENTRADORA":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;

                                    foreach (var pop in document.pops)
                                    {
                                        if (pop.idsConcentradoras != null && pop.idsConcentradoras.Contains(request.Id))
                                        {
                                            pop.idsConcentradoras.Remove(request.Id);
                                            updated = true;
                                            break;
                                        }
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }

                                break;
                            }

                        case "OLT":
                            {
                                var listaIdsPons = await GetAllIdsPons(request.Id, null, session);
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);
                                if (document != null)
                                {
                                    bool updated = false;

                                    foreach (var pop in document.pops)
                                    {
                                        if (pop.idsPons != null)
                                        {
                                            //var idToRemove = listaIdsPons.FirstOrDefault(id => pop.idsPons.Contains(id));
                                            var lista_idsPons_mongo = listaIdsPons.Where(id => pop.idsPons.Contains(id));
                                            if (lista_idsPons_mongo.Count() > 0)
                                            {
                                                foreach (var idToRemove in lista_idsPons_mongo)
                                                {
                                                    pop.idsPons.Remove(idToRemove);
                                                }
                                                updated = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }
                                break;
                            }

                        case "SLOT":
                            {
                                var listaIdsPons = await GetAllIdsPons(null, request.Id, session);
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);
                                if (document != null)
                                {
                                    bool updated = false;

                                    foreach (var pop in document.pops)
                                    {
                                        if (pop.idsPons != null)
                                        {
                                            //var idToRemove = listaIdsPons.FirstOrDefault(id => pop.idsPons.Contains(id));
                                            var lista_idsPons_mongo = listaIdsPons.Where(id => pop.idsPons.Contains(id));
                                            if (lista_idsPons_mongo.Count() > 0)
                                            {
                                                foreach (var idToRemove in lista_idsPons_mongo)
                                                {
                                                    pop.idsPons.Remove(idToRemove);
                                                }
                                                updated = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }

                                break;
                            }

                        case "PON":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;

                                    foreach (var pop in document.pops)
                                    {
                                        if (pop.idsPons != null && pop.idsPons.Contains(request.Id))
                                        {
                                            pop.idsPons.Remove(request.Id);
                                            updated = true;
                                            break;
                                        }
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }
                                break;
                            }
                        case "PORTA":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;
                                    foreach (var tipoElemento in document.tiposElementosRede)
                                    {
                                        foreach (var elemento in tipoElemento.ElementosRede)
                                        {
                                            if (elemento.Ids_Portas != null && elemento.Ids_Portas.Contains(request.Id))
                                            {
                                                elemento.Ids_Portas.Remove(request.Id);
                                                updated = true;
                                                break;
                                            }
                                        }
                                    }


                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }
                                break;
                            }
                        case "ELEMENTO":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;
                                    foreach (var tipoElemento in document.tiposElementosRede)
                                    {
                                        foreach (var elemento in tipoElemento.ElementosRede)
                                        {
                                            if (elemento.Id != 0 && elemento.Id == request.Id)
                                            {
                                                tipoElemento.ElementosRede.Remove(elemento);
                                                updated = true;
                                                break;
                                            }
                                        }
                                    }


                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }
                                break;
                            }
                        case "TIPO_ELEMENTO":
                            {

                                await repositoryPontos.DeleteItemFromArray<TipoElementosRedeMongo>(
                                    doc => doc.codct == request.Codct &&
                                           doc.projeto == request.Projeto &&
                                           doc.numDoc == request.NumDoc &&
                                           doc.numObra == request.NumObra,
                                    doc => doc.tiposElementosRede,
                                    tipoElementoRede => tipoElementoRede.Id_Tipo == request.Id
                                );
                                break;
                            }
                        case "CONDOMINIO":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;

                                    if (document.enderecosAfetados.Ids_Condominios.Count > 0 && document.enderecosAfetados.Ids_Condominios.Contains(request.Id))
                                    {
                                        document.enderecosAfetados.Ids_Condominios.Remove(request.Id);
                                        updated = true;
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }

                                break;
                            }
                        case "ENDERECO":
                            {
                                var document = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                                if (document != null)
                                {
                                    bool updated = false;

                                    if (document.enderecosAfetados.Ids_Enderecos.Count > 0 && document.enderecosAfetados.Ids_Enderecos.Contains(request.Id))
                                    {
                                        document.enderecosAfetados.Ids_Enderecos.Remove(request.Id);
                                        updated = true;
                                    }

                                    if (updated)
                                    {
                                        await repositoryPontos.Update(
                                            p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra,
                                            document
                                        );
                                    }
                                }

                                break;
                            }



                    }


                    var coletaDadosPontoManutencao = await repositoryPontos.Find(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra);

                    if (coletaDadosPontoManutencao == null)
                    {
                        throw new Exception("Não foram encontrados alertas de manutenção vinculados aos dados da O.S.");
                    }

                    var pontosAfetadosRequest = new PontosAfetadosRequest()
                    {
                        Pops = coletaDadosPontoManutencao.pops,
                        TiposElementosRede = coletaDadosPontoManutencao.tiposElementosRede,
                        EnderecosAfetados = coletaDadosPontoManutencao.enderecosAfetados

                    };


                    var getDadosClientes = await GetClientesVinculados(pontosAfetadosRequest);

                    var atualizaDocumentoMongo = new PontosAfetadosManutencaoMongo()
                    {
                        _id = coletaDadosPontoManutencao._id,
                        codct = coletaDadosPontoManutencao.codct,
                        projeto = coletaDadosPontoManutencao.projeto,
                        numDoc = coletaDadosPontoManutencao.numDoc,
                        numObra = coletaDadosPontoManutencao.numObra,
                        pops = coletaDadosPontoManutencao.pops,
                        tiposElementosRede = coletaDadosPontoManutencao.tiposElementosRede,
                        clientesAfetados = getDadosClientes,
                        enderecosAfetados = coletaDadosPontoManutencao.enderecosAfetados
                    };


                    await repositoryPontos.Update(p => p.codct == request.Codct && p.projeto == request.Projeto && p.numDoc == request.NumDoc && p.numObra == request.NumObra, atualizaDocumentoMongo);

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.exception = ex?.InnerException?.Message;
                response.message = ex.Message;

            }



            return response;
        }



        //Método de teste para inserção de dados no mongo.
        //Este método será removido após a finalização do desenvolvimento.
        [AllowAnonymous]
        [HttpGet]
        [Route("api/PontosAfetadosManutencaoApi/InsertTeste")]
        public async Task<Response> InsertTeste()
        {
            var response = new Response();
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
            {

                var elementosRedeMongo = new ElementosRedeAfetadosMongo()
                {
                    Id = 407,
                    Ids_Portas = new List<long> { 365 },
                };

                var elementosRedeMongo2 = new ElementosRedeAfetadosMongo()
                {
                    Id = 408,
                    Ids_Portas = new List<long> { 365, 567, 234 },
                };

                List<ElementosRedeAfetadosMongo> listelementosRede = new List<ElementosRedeAfetadosMongo>
                {
                    elementosRedeMongo,
                    elementosRedeMongo2
                };

                var tipoElemento = new TipoElementosRedeMongo()
                {
                    Id_Tipo = 50,
                    ElementosRede = listelementosRede,

                };

                var popsMongo = new PopsAfetadosMongo()
                {
                    idsConcentradoras = null,
                    idsOlts = null,
                    idsPons = null,
                    idsRadius = null,
                    idsSlots = null

                };

                var enderecos = new EnderecosAfetadosMongo()
                {
                    Ids_Condominios = new List<long?> { 1, 3, 9 },
                    Ids_Enderecos = new List<long?> { 2, 4, 6 }

                };


                var request = new PontosAfetadosRequest()
                {
                    Pops = new List<PopsAfetadosMongo> { popsMongo },
                    TiposElementosRede = new List<TipoElementosRedeMongo> { tipoElemento },
                    EnderecosAfetados = enderecos


                };


                var getDadosClientes = await GetClientesVinculados(request);


                PontosAfetadosManutencaoMongo mockData = new PontosAfetadosManutencaoMongo()
                {
                    codct = "1234",
                    projeto = "TESTE",
                    numDoc = "1234",
                    numObra = "12345",
                    pops = new List<PopsAfetadosMongo> { popsMongo },
                    tiposElementosRede = new List<TipoElementosRedeMongo> { tipoElemento },
                    clientesAfetados = getDadosClientes,
                    enderecosAfetados = enderecos


                };

                await repository.Save(mockData);
            }

            response.result = true;

            return response;

        }



        //Método responsável por coletar ids de clientes vinculados aos equipamentos de redes afetados pela manutenção
        //(POP, OLT, SLOT e PON)
        public async Task<List<DadosClientesAfetadosMongo>> GetIdClientesByPOP(PontosAfetadosRequest request, ISession session)
        {
            List<DadosClientesAfetadosMongo> ids_clientes = new List<DadosClientesAfetadosMongo>();
            foreach (var equipamentos in request.Pops)
            {
                if (equipamentos.idsPons == null && equipamentos.idsRadius == null && equipamentos.idsConcentradoras == null)
                {
                    continue;
                }
                else
                {
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        List<String> condicoes = new List<String>();

                        sql.AppendLine("SELECT DISTINCT A.ID_CLIENTE, A.ID, A.LOGIN, F.NOMEFANTASIA, I.NOME, F.CPF, J.DESCRICAO AS DESCRICAO_PLANO, ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_CEP, '') || ', ' || ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_LOGRADOURO, '') || ', ' || ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_NUMERO, '') || ', ' || ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_BAIRRO, '') || ', ' || ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_CIDADE, '') || ', ' || ");
                        sql.AppendLine("  COALESCE(H.ENDERECO_UF, '') AS ENDERECO ");
                        sql.AppendLine("FROM TB_LOGIN_RADIUS A ");
                        sql.AppendLine("INNER JOIN TB_CLIENTE F ON (A.ID_CLIENTE = F.ID) ");
                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO G ON (A.ID_PLANO_PRODUTO = G.ID) ");
                        sql.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE H ON (G.ID_ENDERECO_INSTALACAO = H.ID) ");
                        sql.AppendLine("LEFT JOIN TB_ONU B ON(A.ID = B.ID_LOGIN_RADIUS) ");
                        sql.AppendLine("LEFT JOIN TB_CONCENTRADORA C ON (A.ID_CONCENTRADORA = C.ID) ");
                        sql.AppendLine("LEFT JOIN TB_POP I ON (@ID_POP = I.ID) ");
                        sql.AppendLine("LEFT JOIN TB_PLANO J ON (G.ID_PLANO = J.ID) ");
                        parametros.Add("@ID_POP", equipamentos.IdPop);


                        #region Filtros OLTs/SLOTs/PONs                           

                        if (equipamentos.idsPons != null && equipamentos.idsPons.Count() > 0)
                        {
                            string idsPONs = $"({string.Join(",", equipamentos.idsPons)})";
                            condicoes.Add($"B.ID_PON IN {idsPONs}");
                        }

                        #endregion

                        #region Filtro Concentradoras e Radius                           

                        if (equipamentos.idsConcentradoras != null && equipamentos.idsConcentradoras.Count() > 0
                            && equipamentos.idsRadius != null && equipamentos.idsRadius.Count() > 0)
                        {
                            string idsConcentradoras = $"({string.Join(",", equipamentos.idsConcentradoras)})";
                            string idsRadius = $"({string.Join(",", equipamentos.idsRadius)})";
                            condicoes.Add($"A.ID_CONCENTRADORA IN {idsConcentradoras} AND C.ID_RADIUS IN {idsRadius}");
                        }
                        else if (equipamentos.idsConcentradoras != null && equipamentos.idsConcentradoras.Count() > 0)
                        {
                            string idsConcentradoras = $"({string.Join(",", equipamentos.idsConcentradoras)})";
                            condicoes.Add($"A.ID_CONCENTRADORA IN {idsConcentradoras}");
                        }
                        else if (equipamentos.idsRadius != null && equipamentos.idsRadius.Count() > 0)
                        {
                            string idsRadius = $"({string.Join(",", equipamentos.idsRadius)})";
                            condicoes.Add($"C.ID_RADIUS IN {idsRadius}");
                        }



                        #endregion

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE ( {String.Join(" OR ", condicoes)} ) ");
                        }


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                ids_clientes.Add(new DadosClientesAfetadosMongo
                                {
                                    Id = Convert.ToInt64(reader["ID_CLIENTE"]),
                                    Id_Login_Radius = Convert.ToInt64(reader["ID"]),
                                    Login_Radius = Convert.ToString(reader["LOGIN"]),
                                    Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Ponto_Afetado = Convert.ToString(reader["NOME"]),
                                    Endereco = Convert.ToString(reader["ENDERECO"]),
                                    Cpfcnpj = Convert.ToString(reader["CPF"]),
                                    Plano = Convert.ToString(reader["DESCRICAO_PLANO"])

                                });
                            }
                        }


                    }
                }


            }

            return ids_clientes;
        }

        //Método responsável por coletar ids de clientes vinculados aos condomínios
        public async Task<List<DadosClientesAfetadosMongo>> GetIdClientesByCondominio(PontosAfetadosRequest request, ISession session)
        {
            List<DadosClientesAfetadosMongo> ids_clientes = new List<DadosClientesAfetadosMongo>();

            if (request.EnderecosAfetados.Ids_Condominios?.Count > 0)
            {
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    sql.AppendLine("SELECT DISTINCT A.ID_CLIENTE, A.ID, A.LOGIN, F.NOMEFANTASIA, B.DESCRICAO AS DESCRICAO_CONDOMINIO, F.CPF, J.DESCRICAO AS DESCRICAO_PLANO, ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_CEP, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_LOGRADOURO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_NUMERO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_BAIRRO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_CIDADE, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_UF, '') AS ENDERECO ");
                    sql.AppendLine("FROM TB_LOGIN_RADIUS A ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE F ON (A.ID_CLIENTE = F.ID) ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO G ON (A.ID_PLANO_PRODUTO = G.ID) ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE H ON (G.ID_ENDERECO_INSTALACAO = H.ID) ");
                    sql.AppendLine("LEFT JOIN TB_CONDOMINIO_PREDIO B ON (H.ID_CONDOMINIO_PREDIO = B.ID) ");
                    sql.AppendLine("LEFT JOIN TB_PLANO J ON (G.ID_PLANO = J.ID) ");
                    string idsCondominios = $"({string.Join(",", request.EnderecosAfetados.Ids_Condominios)})";
                    sql.AppendLine($"WHERE H.ID_CONDOMINIO_PREDIO IN {idsCondominios}");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            ids_clientes.Add(new DadosClientesAfetadosMongo
                            {
                                Id = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Id_Login_Radius = Convert.ToInt64(reader["ID"]),
                                Login_Radius = Convert.ToString(reader["LOGIN"]),
                                Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                Ponto_Afetado = Convert.ToString(reader["DESCRICAO_CONDOMINIO"]),
                                Endereco = Convert.ToString(reader["ENDERECO"]),
                                Cpfcnpj = Convert.ToString(reader["CPF"]),
                                Plano = Convert.ToString(reader["DESCRICAO_PLANO"])

                            });
                        }
                    }


                }
            }

            return ids_clientes;
        }

        //Método responsável por coletar ids de clientes vinculados aos endereços logradouros
        public async Task<List<DadosClientesAfetadosMongo>> GetIdClientesByEnderecoLogradouro(PontosAfetadosRequest request, ISession session)
        {
            List<DadosClientesAfetadosMongo> ids_clientes = new List<DadosClientesAfetadosMongo>();

            if (request.EnderecosAfetados.Ids_Enderecos?.Count > 0)
            {
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    sql.AppendLine("SELECT DISTINCT A.ID_CLIENTE, A.ID, A.LOGIN, F.NOMEFANTASIA, B.DESCRICAO AS DESCRICAO_ENDERECO, F.CPF, J.DESCRICAO AS DESCRICAO_PLANO, ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_CEP, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_LOGRADOURO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_NUMERO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_BAIRRO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_CIDADE, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(H.ENDERECO_UF, '') AS ENDERECO ");
                    sql.AppendLine("FROM TB_LOGIN_RADIUS A ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE F ON (A.ID_CLIENTE = F.ID) ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO G ON (A.ID_PLANO_PRODUTO = G.ID) ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE H ON (G.ID_ENDERECO_INSTALACAO = H.ID) ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO B ON (H.ID_ENDERECO_LOGRADOURO = B.ID) ");
                    sql.AppendLine("LEFT JOIN TB_PLANO J ON (G.ID_PLANO = J.ID) ");
                    string idsEnderecos = $"({string.Join(",", request.EnderecosAfetados.Ids_Enderecos)})";
                    sql.AppendLine($"WHERE H.ID_ENDERECO_LOGRADOURO IN {idsEnderecos}");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            ids_clientes.Add(new DadosClientesAfetadosMongo
                            {
                                Id = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Id_Login_Radius = Convert.ToInt64(reader["ID"]),
                                Login_Radius = Convert.ToString(reader["LOGIN"]),
                                Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                Ponto_Afetado = Convert.ToString(reader["DESCRICAO_ENDERECO"]),
                                Endereco = Convert.ToString(reader["ENDERECO"]),
                                Cpfcnpj = Convert.ToString(reader["CPF"]),
                                Plano = Convert.ToString(reader["DESCRICAO_PLANO"])

                            });
                        }
                    }


                }
            }

            return ids_clientes;
        }

        //Método responsável por coletar ids de clientes vinculados aos elementos de rede afetados pela manutenção
        //(Caixa e porta)
        public async Task<List<DadosClientesAfetadosMongo>> GetIdClientesByElementoRede(PontosAfetadosRequest request, ISession sessionT5, ISession session)
        {
            Dictionary<String, string> loginVinculadoCaixa = new Dictionary<string, string>();
            List<DadosClientesAfetadosMongo> ids_clientes = new List<DadosClientesAfetadosMongo>();
            List<string> ids_login_radius = new List<string>();

            foreach (var tipo in request.TiposElementosRede)
            {
                if (tipo.ElementosRede.Count == 0 || tipo.ElementosRede == null || !tipo.ElementosRede.Any(e => e.Id != null))
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Dictionary<String, Object> param = new Dictionary<string, object>();

                        List<String> condicoes = new List<String>();

                        sql.AppendLine("SELECT A.ID_LOGIN, A.PORTA, B.DESCRICAO FROM TB_ELEMENTO_REDE_PORTA A ");
                        sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE B ON (A.ID_CAIXA = B.ID) ");
                        sql.AppendLine($"WHERE B.ID_TIPO = @ID_TIPO ");

                        param.Add("@ID_TIPO", tipo.Id_Tipo);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var idLogin = Convert.ToString(reader["ID_LOGIN"]);
                                var porta = Convert.ToInt32(reader["PORTA"]);
                                var descricaoCaixa = Convert.ToString(reader["DESCRICAO"]);

                                var caixaPortaConcat = $"Caixa: {descricaoCaixa}, Porta: {porta}";


                                loginVinculadoCaixa[idLogin] = caixaPortaConcat;

                                if (!string.IsNullOrEmpty(idLogin))
                                    ids_login_radius.Add(idLogin);
                            }
                        }

                    }
                }
                else
                {
                    foreach (var caixa in tipo.ElementosRede)
                    {
                        if (caixa.Id != 0)
                        {
                            using (var com = sessionT5.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder();
                                Dictionary<String, Object> param = new Dictionary<string, object>();

                                List<String> condicoes = new List<String>();

                                sql.AppendLine("SELECT A.ID_LOGIN, A.PORTA, B.DESCRICAO FROM TB_ELEMENTO_REDE_PORTA A ");
                                sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE B ON (A.ID_CAIXA = B.ID) ");
                                if (caixa.Ids_Portas?.Count > 0 && caixa.Ids_Portas != null)
                                {
                                    string idsPortas = $"({string.Join(",", caixa.Ids_Portas)}) ";
                                    sql.AppendLine($"WHERE A.ID_CAIXA = @ID_CAIXA AND A.ID IN {idsPortas} ");
                                }
                                else
                                {
                                    sql.AppendLine($"WHERE A.ID_CAIXA = @ID_CAIXA ");
                                }

                                param.Add("@ID_CAIXA", caixa.Id);


                                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        var idLogin = Convert.ToString(reader["ID_LOGIN"]);
                                        var porta = Convert.ToInt32(reader["PORTA"]);
                                        var descricaoCaixa = Convert.ToString(reader["DESCRICAO"]);

                                        var caixaPortaConcat = $"Caixa: {descricaoCaixa}, Porta: {porta}";


                                        loginVinculadoCaixa[idLogin] = caixaPortaConcat;

                                        if (!string.IsNullOrEmpty(idLogin))
                                            ids_login_radius.Add(idLogin);
                                    }
                                }

                            }
                        }

                    }
                }
            }


            if (ids_login_radius.Count() > 0)
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> param = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    sql.AppendLine("SELECT DISTINCT A.ID_CLIENTE, A.ID, A.LOGIN, B.NOMEFANTASIA, B.CPF, E.DESCRICAO AS DESCRICAO_PLANO, ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_CEP, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_LOGRADOURO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_NUMERO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_BAIRRO, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_CIDADE, '') || ', ' || ");
                    sql.AppendLine("  COALESCE(D.ENDERECO_UF, '') AS ENDERECO ");
                    sql.AppendLine(" FROM TB_LOGIN_RADIUS A ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE B ON (A.ID_CLIENTE = B.ID) ");
                    sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO C ON (A.ID_PLANO_PRODUTO = C.ID) ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_CLIENTE D ON (C.ID_ENDERECO_INSTALACAO = D.ID) ");
                    sql.AppendLine("LEFT JOIN TB_PLANO E ON (C.ID_PLANO = E.ID) ");
                    string idsLogins = $"({string.Join(",", ids_login_radius)})";
                    sql.AppendLine($"WHERE A.ID IN {idsLogins}");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var idLogin = reader["ID"].ToString();
                            string pontoAfetado = null;

                            if (loginVinculadoCaixa.TryGetValue(idLogin, out string nomeCaixa))
                            {
                                pontoAfetado = nomeCaixa;
                            }

                            ids_clientes.Add(new DadosClientesAfetadosMongo
                            {
                                Id = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Id_Login_Radius = Convert.ToInt64(reader["ID"]),
                                Login_Radius = Convert.ToString(reader["LOGIN"]),
                                Nome = Convert.ToString(reader["NOMEFANTASIA"]),
                                Ponto_Afetado = pontoAfetado,
                                Endereco = Convert.ToString(reader["ENDERECO"]),
                                Cpfcnpj = Convert.ToString(reader["CPF"]),
                                Plano = Convert.ToString(reader["DESCRICAO_PLANO"])

                            });
                        }
                    }
                }
            }


            return ids_clientes;
        }
        public async Task<List<long?>> GetAllIdsPons(long? idOlt, long? idSlot, ISession session)
        {
            List<long?> idsPons = new List<long?>();
            using (var com = session.Connection.CreateCommand())
            {


                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> param = new Dictionary<string, object>();
                List<String> condicoes = new List<String>();

                sql.AppendLine(" SELECT A.ID AS ID_PON");
                sql.AppendLine(" FROM TB_PON A ");
                sql.AppendLine(" LEFT JOIN TB_SLOT B ON (B.ID = A.ID_SLOT) ");
                sql.AppendLine(" LEFT JOIN TB_OLT C ON (C.ID = B.ID_OLT) ");

                if (idOlt > 0 && idOlt != null)
                {
                    condicoes.Add($" C.ID = @ID_OLT ");
                    param.Add("@ID_OLT", idOlt);
                }

                if (idSlot > 0 && idSlot != null)
                {
                    condicoes.Add($" B.ID = @ID_SLOT ");
                    param.Add("@ID_SLOT", idSlot);
                }

                if (condicoes.Count() > 0)
                {
                    sql.AppendLine($"WHERE ( {String.Join(" OR ", condicoes)} ) ");
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {

                        var idPon = Convert.ToInt64(reader["ID_PON"]);

                        if (idPon != 0)
                        {
                            idsPons.Add(idPon);
                        }

                    }
                }


            }


            return idsPons;
        }



        private async Task<PopsAfetadosMongo> CriarPontoAfetadoPop(PontosAfetadosManut request)
        {

            var popAfetado = new PopsAfetadosMongo()
            {
                IdPop = request.IdPop,
            };

            using (var areaManutApi = new Area_ManutencaoApiController())
            {
                if (request.Tipo.ToUpper().Contains("RADIUS"))
                {
                    popAfetado.idsRadius = new List<long?>();
                    if (request.Ids == null || request?.Ids?.Count == 0)
                    {
                        var buscar_radius_relacionadas = areaManutApi.BuscarRadiusPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                        foreach (var item in buscar_radius_relacionadas)
                        {
                            popAfetado.idsRadius.Add(item);
                        }
                    }
                    else
                    {
                        popAfetado.idsRadius.AddRange(request.Ids);
                    }
                }

                if (request.Tipo.ToUpper().Contains("CONCENTRADORA"))
                {
                    popAfetado.idsConcentradoras = new List<long?>();
                    if (request.Ids == null || request?.Ids?.Count == 0)
                    {
                        var buscar_concentradores_relacionadas = areaManutApi.BuscarConcentradorasPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                        foreach (var item in buscar_concentradores_relacionadas)
                        {
                            popAfetado.idsConcentradoras.Add(item);
                        }
                    }
                    else
                    {
                        popAfetado.idsConcentradoras.AddRange(request.Ids);
                    }
                }

                if (request.Tipo.ToUpper().Contains("OLT"))
                {
                    popAfetado.idsOlts = new List<long?>();
                    if (request.Ids == null || request?.Ids?.Count == 0)
                    {
                        var buscar_olt_relacionadas = areaManutApi.BuscarOLTSPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                        foreach (var item in buscar_olt_relacionadas)
                        {
                            popAfetado.idsOlts.Add(item);
                        }
                    }
                    else
                    {
                        popAfetado.idsOlts.AddRange(request.Ids);
                    }

                    var slotsRelacionados = areaManutApi.BuscarSlotsOLT(new PopRequestManut
                    {
                        IdsOlts = popAfetado.idsOlts,
                        IdPop = request.IdPop.Value
                    }).Result.IdObjeto.Select(x => x.Id);

                    foreach (var slot in slotsRelacionados)
                    {
                        if (popAfetado.idsSlots is null)
                            popAfetado.idsSlots = new List<long?>();

                        if (!popAfetado.idsSlots.Contains(slot))
                            popAfetado.idsSlots.Add(slot);
                    }

                    var ponsRelacionados = areaManutApi.BuscarPonsSlots(new PopRequestManut
                    {
                        IdsSlots = popAfetado.idsSlots,
                        IdPop = request.IdPop.Value
                    }).Result.IdObjeto.Select(x => x.Id);

                    foreach (var pon in ponsRelacionados)
                    {
                        if (popAfetado.idsPons is null)
                            popAfetado.idsPons = new List<long?>();

                        if (!popAfetado.idsPons.Contains(pon))
                            popAfetado.idsPons.Add(pon);
                    }
                }

                if (request.Tipo.ToUpper() == "SLOT")
                {
                    popAfetado.idsSlots = new List<long?>();
                    popAfetado.idsSlots.AddRange(request.Ids);

                    var ponsRelacionados = areaManutApi.BuscarPonsSlots(new PopRequestManut
                    {
                        IdsSlots = popAfetado.idsSlots,
                        IdPop = request.IdPop.Value
                    }).Result.IdObjeto.Select(x => x.Id);

                    foreach (var pon in ponsRelacionados)
                    {
                        if (popAfetado.idsPons is null)
                            popAfetado.idsPons = new List<long?>();

                        if (!popAfetado.idsPons.Contains(pon))
                            popAfetado.idsPons.Add(pon);
                    }
                }

                if (request.Tipo.ToUpper() == "PON")
                {
                    popAfetado.idsPons = new List<long?>();
                    popAfetado.idsPons.AddRange(request.Ids);
                }

                if (request.Tipo.ToUpper() == "POP")
                {
                    var olts_relacionadas = areaManutApi.BuscarOLTSPop(new PopRequestManut
                    {
                        IdPop = request.IdPop.Value
                    }).Result.IdObjeto.Select(x => x.Id);

                    IEnumerable<long> slotsRelacionados = new List<long>();
                    foreach (var item in olts_relacionadas)
                    {
                        slotsRelacionados = areaManutApi.BuscarSlotsOLT(new PopRequestManut
                        {
                            IdsOlts = new List<long?> { item },
                            IdPop = request.IdPop.Value
                        }).Result.IdObjeto.Select(x => x.Id);
                    }

                    foreach (var slot in slotsRelacionados)
                    {
                        if (popAfetado.idsSlots is null)
                            popAfetado.idsSlots = new List<long?>();

                        if (!popAfetado.idsSlots.Contains(slot))
                            popAfetado.idsSlots.Add(slot);
                    }

                    var ponsRelacionados = areaManutApi.BuscarPonsSlots(new PopRequestManut
                    {
                        IdsSlots = popAfetado.idsSlots,
                        IdPop = request.IdPop.Value
                    }).Result.IdObjeto.Select(x => x.Id);

                    foreach (var pon in ponsRelacionados)
                    {
                        if (popAfetado.idsPons is null)
                            popAfetado.idsPons = new List<long?>();

                        if (!popAfetado.idsPons.Contains(pon))
                            popAfetado.idsPons.Add(pon);
                    }

                    var buscar_concentradores_relacionadas = areaManutApi.BuscarConcentradorasPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                    foreach (var con in buscar_concentradores_relacionadas)
                    {
                        if (popAfetado.idsConcentradoras is null)
                            popAfetado.idsConcentradoras = new List<long?>();

                        if (!popAfetado.idsConcentradoras.Contains(con))
                            popAfetado.idsConcentradoras.Add(con);
                    }

                    var buscar_radius_relacionadas = areaManutApi.BuscarRadiusPop(new PopRequestManut { IdPop = request.IdPop.Value }).Result.IdObjeto.Select(x => x.Id);
                    foreach (var rad in buscar_radius_relacionadas)
                    {
                        if (popAfetado.idsRadius is null)
                            popAfetado.idsRadius = new List<long?>();

                        if (!popAfetado.idsRadius.Contains(rad))
                            popAfetado.idsRadius.Add(rad);
                    }
                }
            }

            return popAfetado;
        }

        private async Task<bool> UpdatePontosAfetadosManutencaoMongo(PontosAfetadosManutencaoMongo pontoAfetado)
        {
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
            {

                await repositoryMongo.Update(x => x.codct == pontoAfetado.codct &&
                                                  x.projeto == pontoAfetado.projeto &&
                                                  x.numDoc == pontoAfetado.numDoc &&
                                                  x.numObra == pontoAfetado.numObra,
                                                  pontoAfetado);
            }
            return true;
        }

        private async Task<bool> SavePontosAfetadosManutMongo(PontosAfetadosManutencaoMongo pontoAfetado)
        {
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
            {
                await repositoryMongo.Save(pontoAfetado);
            }
            return true;
        }

        private async Task<PontosAfetadosManutencaoMongo> GetPontosAfetadoMongo(PontosAfetadosManut documento)
        {
            PontosAfetadosManutencaoMongo retorno = null;
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<PontosAfetadosManutencaoMongo>(conexaoMongo))
            {

                MongoDB.Driver.Linq.IMongoQueryable<PontosAfetadosManutencaoMongo> query =
               (MongoDB.Driver.Linq.IMongoQueryable<PontosAfetadosManutencaoMongo>)repositoryMongo.GetQueryable().Where(x =>
                                                                            x.codct == documento.Codct &&
                                                                            x.projeto == documento.Projeto &&
                                                                            x.numDoc == documento.NumDoc &&
                                                                            x.numObra == documento.NumObra);
                retorno = query.SingleOrDefault();
            }
            return retorno;
        }
    }
}