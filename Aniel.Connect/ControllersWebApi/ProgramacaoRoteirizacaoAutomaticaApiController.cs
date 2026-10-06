using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ProgramacaoRoteirizacaoAutomaticaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/GetAll")]
        public async Task<DataTableResponse<ProgramacaoRoteirizacaoAutomaticaListaModel>> GetAll(ProgramacaoRoteirizacaoAutomatica_DataTableRequest request)
        {
            List<ProgramacaoRoteirizacaoAutomaticaListaModel> retorno = new List<ProgramacaoRoteirizacaoAutomaticaListaModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<ProgramacaoRoteirizacaoAutomatica>(session))
            {
                retorno = repo.QueryableSelect().Skip(request.start).Take(request.length).Select(p => new ProgramacaoRoteirizacaoAutomaticaListaModel
                {
                    Id = p.Id,
                    Nome = p.Descricao,
                    DataInicio = p.DataHoraInicio.HasValue ? p.DataHoraInicio.Value.Date.ToString("dd/MM/yyyy") : null,
                    HoraInicio = p.DataHoraInicio.HasValue ? p.DataHoraInicio.Value.Date.ToString("HH:mm") : null,
                    Status = HtmlEnumExtensions.GetEnumDescription(p.Status)
                }).ToList();

            }


            return new DataTableResponse<ProgramacaoRoteirizacaoAutomaticaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno.OrderBy(p => p.Nome),
                error = ""
            };
        }

        [HttpGet()]
        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/FindById")]
        public async Task<GetApiRetorno<ProgramacaoRoteirizacaoAutomaticaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ProgramacaoRoteirizacaoAutomaticaCadastroModel> retorno = new GetApiRetorno<ProgramacaoRoteirizacaoAutomaticaCadastroModel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<ProgramacaoRoteirizacaoAutomatica>(sessionT5))
            {
                try
                {
                    var modelo = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(modelo, sessionT5);

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }

            }
            return retorno;
        }

        private ProgramacaoRoteirizacaoAutomaticaCadastroModel GetViewObj(ProgramacaoRoteirizacaoAutomatica obj, ISession sessionT5)
        {
            using (Conexao<EscritorioAniel> repoEscritorio = new Conexao<EscritorioAniel>(sessionT5))
            using (Conexao<AreaAniel> repoArea = new Conexao<AreaAniel>(sessionT5))
            using (Conexao<EmpresaAniel> repoEmpresa = new Conexao<EmpresaAniel>(sessionT5))
            using (Conexao<FilialAniel> repoFilial = new Conexao<FilialAniel>(sessionT5))
            {
                List<DayOfWeek> diasSemana = String.IsNullOrEmpty(obj.DiasSemana) ? new List<DayOfWeek>() : obj.DiasSemana.Split(',').Select(p =>
                {
                    return (DayOfWeek)Convert.ToInt32(p);
                }).ToList();
                List<Mes> meses = String.IsNullOrEmpty(obj.Meses) ? new List<Mes>() : obj.Meses.Split(',').Select(p =>
                {
                    return (Mes)Convert.ToInt32(p);
                }).ToList();
                List<int> dias = String.IsNullOrEmpty(obj.DiasMes) ? new List<int>() : obj.DiasMes.Split(',').Select(p =>
                {
                    return Convert.ToInt32(p);
                }).ToList();

                return new ProgramacaoRoteirizacaoAutomaticaCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,
                    Status = obj.Status == Status_Default.Ativo,
                    DataInicio = obj.DataHoraInicio.HasValue ? obj.DataHoraInicio.Value.ToString("dd/MM/yyyy") : null,
                    HoraInicio = obj.DataHoraInicio.HasValue ? obj.DataHoraInicio.Value.ToString("HH:mm") : null,
                    FormatoDisparoAutomaticoRoteirizacao = obj.FormatoDisparoAutomaticoRoteirizacao,
                    NumPrimeirosDias = obj.NumPrimeirosDias.HasValue ? obj.NumPrimeirosDias.Value.ToString() : null,
                    NumUltimosDias = obj.NumUltimosDias.HasValue ? obj.NumUltimosDias.Value.ToString() : null,
                    Servico_SemAgendamento = obj.Servico_SemAgendamento.GetValueOrDefault(false),
                    Servico_Agendados = obj.Servico_Agendados.GetValueOrDefault(false),
                    Servico_Nao_Executados = obj.Servico_Nao_Executados.GetValueOrDefault(false),
                    Servico_Paralisados = obj.Servico_Paralisados.GetValueOrDefault(false),
                    Servico_Roteirizados = obj.Servico_Roteirizados.GetValueOrDefault(false),
                    TodosDias = obj.TodosDias.GetValueOrDefault(false),
                    NoDia = obj.NoDia.GetValueOrDefault(false),
                    NaSemana = obj.NaSemana.GetValueOrDefault(false),
                    NoMes = obj.NoMes.GetValueOrDefault(false),
                    NosPrimeiros = obj.NosPrimeiros,
                    NosUltimos = obj.NosPrimeiros.GetValueOrDefault(false),
                    Segunda = diasSemana.Contains(DayOfWeek.Monday),
                    Terca = diasSemana.Contains(DayOfWeek.Tuesday),
                    Quarta = diasSemana.Contains(DayOfWeek.Wednesday),
                    Quinta = diasSemana.Contains(DayOfWeek.Thursday),
                    Sexta = diasSemana.Contains(DayOfWeek.Friday),
                    Sabado = diasSemana.Contains(DayOfWeek.Saturday),
                    Domingo = diasSemana.Contains(DayOfWeek.Sunday),
                    Janeiro = meses.Contains(Mes.Janeiro),
                    Fevereiro = meses.Contains(Mes.Fevereiro),
                    Marco = meses.Contains(Mes.Marco),
                    Abril = meses.Contains(Mes.Abril),
                    Maio = meses.Contains(Mes.Maio),
                    Junho = meses.Contains(Mes.Junho),
                    Julho = meses.Contains(Mes.Julho),
                    Agosto = meses.Contains(Mes.Agosto),
                    Setembro = meses.Contains(Mes.Setembro),
                    Outubro = meses.Contains(Mes.Outubro),
                    Novembro = meses.Contains(Mes.Novembro),
                    Dezembro = meses.Contains(Mes.Dezembro),
                    Dia01 = dias.Contains(1),
                    Dia02 = dias.Contains(2),
                    Dia03 = dias.Contains(3),
                    Dia04 = dias.Contains(4),
                    Dia05 = dias.Contains(5),
                    Dia06 = dias.Contains(6),
                    Dia07 = dias.Contains(7),
                    Dia08 = dias.Contains(8),
                    Dia09 = dias.Contains(9),
                    Dia10 = dias.Contains(10),
                    Dia11 = dias.Contains(11),
                    Dia12 = dias.Contains(12),
                    Dia13 = dias.Contains(13),
                    Dia14 = dias.Contains(14),
                    Dia15 = dias.Contains(15),
                    Dia16 = dias.Contains(16),
                    Dia17 = dias.Contains(17),
                    Dia18 = dias.Contains(18),
                    Dia19 = dias.Contains(19),
                    Dia20 = dias.Contains(10),
                    Dia21 = dias.Contains(21),
                    Dia22 = dias.Contains(22),
                    Dia23 = dias.Contains(23),
                    Dia24 = dias.Contains(24),
                    Dia25 = dias.Contains(25),
                    Dia26 = dias.Contains(26),
                    Dia27 = dias.Contains(27),
                    Dia28 = dias.Contains(28),
                    Dia29 = dias.Contains(29),
                    Dia30 = dias.Contains(30),
                    Dia31 = dias.Contains(31),
                    DataExpira = obj.DataExpira.HasValue ? obj.DataExpira.Value.ToString("dd/MM/yyyy") : null,
                    ProgramacaoExpira = obj.DataExpira.HasValue ? true : false,
                    Supervisores = obj.Supervisores.Select(p => new ProgramacaoRoteirizacaoSupervisorViewModel
                    {
                        Id = p.Id,
                        Numfre = p.Numfre
                    }).ToList(),
                    Naturezas = obj.Naturezas.Select(p => new ProgramacaoRoteirizacaoNaturezaProducaoViewModel
                    {
                        Id = p.Id,
                        IdNaturezaProducao = p.NaturezaProducaoAniel?.Id,
                    }).ToList(),
                    Categorias = obj.CategoriasCliente.Select(p => new ProgramacaoRoteirizacaoCategoriaClienteViewModel
                    {
                        Id = p.Id,
                        IdCategoria = p.Categoria_ClienteAniel?.Id
                    }).ToList(),
                    Tecnicos = obj.Tecnicos.Select(p => new ProgramacaoRoteirizacaoTecnicoViewModel
                    {
                        Id = p.Id,
                        Equipe = p.Equipe
                    }).ToList(),
                    Projetos = obj.Projetos.Select(p => new ProgramacaoRoteirizacaoProjetoViewModel
                    {
                        Id = p.Id,
                        CodCt = p.CodCt,
                        DescricaoCodCt = p.CodCt,
                        Projeto = p.Projeto
                    }).ToList(),
                    Localidades = obj.Localidades.Select(p => new ProgramacaoRoteirizacaoLocalidadeViewModel
                    {
                        Id = p.Id,
                        CodArea = p.CodArea,
                        CodEscritorio = p.CodEscritorio,
                        DescricaoEscritorio = repoEscritorio.selectFirst(y => y.Cod_Escritorio == p.CodEscritorio).Result?.Descricao,
                        DescricaoArea = repoArea.selectFirst(y => y.Cod_Escritorio == p.CodEscritorio && y.Cod_Area == p.CodArea).Result?.Descricao,
                        Localidade = p.Localidade

                    }).ToList(),
                    Filiais = obj.Filiais.Select(p => new ProgramacaoRoteirizacaoFilialViewModel
                    {
                        CodEmpresa = p.CodEmpresa,
                        CodFilial = p.CodFilial,
                        DescricaoEmpresa = repoEmpresa.selectFirst(y => y.CodEmpresa == p.CodEmpresa).Result?.Razao_Social,
                        DescricaoFilial = repoFilial.selectFirst(y => y.CodEmpresa == p.CodEmpresa && y.CodFilial == p.CodFilial).Result?.Nome,
                        Id = p.Id
                    }).ToList(),
                    NumeroMaximoAtendimento = Convert.ToString(obj.NumeroMaximoAtendimento),
                    AlterarDataHoraSemAgendamentoAniel = obj.AlterarDataHoraSemAgendamentoAniel.GetValueOrDefault(false)
                };
            }

        }

        [HttpPost()]
        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ProgramacaoRoteirizacaoAutomaticaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                ProgramacaoRoteirizacaoAutomatica modelo = null;
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ProgramacaoRoteirizacaoAutomatica>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new ProgramacaoRoteirizacaoAutomatica
                            {
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);
                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private void BindViewToObj(ISession session, ProgramacaoRoteirizacaoAutomatica objBanco, ProgramacaoRoteirizacaoAutomaticaCadastroModel view)
        {
            using (Conexao<Categoria_ClienteAniel> repoCategoria = new Conexao<Categoria_ClienteAniel>(session))
            using (Conexao<Natureza_ProducaoAniel> repoNatureza = new Conexao<Natureza_ProducaoAniel>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Descricao = view.Descricao;
                objBanco.DataHoraInicio = !String.IsNullOrEmpty(view.DataInicio) ? Convert.ToDateTime(view.DataInicio) : (DateTime?)null;

                if (objBanco.DataHoraInicio.HasValue && String.IsNullOrEmpty(view.HoraInicio) == false)
                {
                    objBanco.DataHoraInicio = objBanco.DataHoraInicio.Value.Add(Convert.ToDateTime(view.HoraInicio).TimeOfDay);
                }

                objBanco.FormatoDisparoAutomaticoRoteirizacao = view.FormatoDisparoAutomaticoRoteirizacao;
                objBanco.Servico_SemAgendamento = view.Servico_SemAgendamento;
                objBanco.Servico_Paralisados = view.Servico_Paralisados;
                objBanco.Servico_Agendados = view.Servico_Agendados;
                objBanco.Servico_Nao_Executados = view.Servico_Nao_Executados;
                objBanco.Servico_Roteirizados = view.Servico_Roteirizados;
                objBanco.NumUltimosDias = !String.IsNullOrEmpty(view.NumPrimeirosDias) ? Convert.ToInt32(view.NumPrimeirosDias) : (int?)null;
                objBanco.NumPrimeirosDias = !String.IsNullOrEmpty(view.NumUltimosDias) ? Convert.ToInt32(view.NumUltimosDias) : (int?)null;
                objBanco.NoDia = view.NoDia;
                objBanco.NoMes = view.NoMes;
                objBanco.NaSemana = view.NaSemana;
                objBanco.NosPrimeiros = view.NosPrimeiros;
                objBanco.NosUltimos = view.NosUltimos;
                objBanco.AlterarDataHoraSemAgendamentoAniel = view.AlterarDataHoraSemAgendamentoAniel;
                objBanco.NumeroMaximoAtendimento = Convert.ToInt16(view.NumeroMaximoAtendimento);

                List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
                if(view.Segunda == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Monday);
                }
                if (view.Terca == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Tuesday);
                }
                if (view.Quarta == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Wednesday);
                }
                if (view.Quinta == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Thursday);
                }
                if (view.Sexta == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Friday);
                }
                if (view.Sabado == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Saturday);
                }
                if (view.Domingo == true)
                {
                    dayOfWeeks.Add(DayOfWeek.Sunday);
                }

                objBanco.DiasSemana = String.Join(",", dayOfWeeks.Select(p => (int)p));

                List<Mes> MesesDoano = new List<Mes>();
                if (view.Janeiro == true)
                {
                    MesesDoano.Add(Mes.Janeiro);
                }
                if (view.Fevereiro == true)
                {
                    MesesDoano.Add(Mes.Fevereiro);
                }
                if (view.Marco == true)
                {
                    MesesDoano.Add(Mes.Marco);
                }
                if (view.Abril == true)
                {
                    MesesDoano.Add(Mes.Abril);
                }
                if (view.Maio == true)
                {
                    MesesDoano.Add(Mes.Maio);
                }
                if (view.Junho == true)
                {
                    MesesDoano.Add(Mes.Junho);
                }
                if (view.Julho == true)
                {
                    MesesDoano.Add(Mes.Julho);
                }
                if (view.Agosto == true)
                {
                    MesesDoano.Add(Mes.Agosto);
                }
                if (view.Setembro == true)
                {
                    MesesDoano.Add(Mes.Setembro);
                }
                if (view.Outubro == true)
                {
                    MesesDoano.Add(Mes.Outubro);
                }
                if (view.Novembro == true)
                {
                    MesesDoano.Add(Mes.Novembro);
                }
                if (view.Dezembro == true)
                {
                    MesesDoano.Add(Mes.Dezembro);
                }

                objBanco.Meses = String.Join(",", MesesDoano.Select(p => (int)p));
                objBanco.DiasMes = String.Join(",", DiasSemana(view));
                


                objBanco.Supervisores = view.Supervisor == null ? new List<ProgramacaoRoteirizacaoSupervisor>() : view.Supervisor.Where(p => objBanco.Supervisores.FirstOrDefault(y => y.Numfre == p) == null).
                                                                                                                                  Select(p => objBanco.Supervisores.FirstOrDefault(y => y.Numfre == p) ?? new ProgramacaoRoteirizacaoSupervisor
                                                                                                                                  {
                                                                                                                                      ProgramacaoRoteirizacaoAutomatica = objBanco,
                                                                                                                                      Id = 0,
                                                                                                                                      Numfre = p,
                                                                                                                                  }).ToList();

                objBanco.Naturezas = view.Id_Natureza_Servico == null ? new List<ProgramacaoRoteirizacaoNaturezaProducao>() : view.Id_Natureza_Servico.Where(p => objBanco.Naturezas.FirstOrDefault(y => Convert.ToString(y.NaturezaProducaoAniel?.Id) == p) == null).
                                                                                                                                                       Select(p => new ProgramacaoRoteirizacaoNaturezaProducao
                                                                                                                                                       {
                                                                                                                                                           ProgramacaoRoteirizacaoAutomatica = objBanco,
                                                                                                                                                           Id = 0,
                                                                                                                                                           NaturezaProducaoAniel = repoNatureza.selectFirst(z => z.Id == Convert.ToInt64(p)).Result
                                                                                                                                                       }).ToList();

                objBanco.CategoriasCliente = view.Id_Categoria_Cliente == null ? new List<ProgramacaoRoteirizacaoCategoriaCliente>() : view.Id_Categoria_Cliente.Where(p => objBanco.CategoriasCliente.FirstOrDefault(y => Convert.ToString(y.Categoria_ClienteAniel?.Id) == p) == null).
                                                                                                                                                                 Select(p => new ProgramacaoRoteirizacaoCategoriaCliente
                                                                                                                                                                 {
                                                                                                                                                                     ProgramacaoRoteirizacaoAutomatica = objBanco,
                                                                                                                                                                     Id = 0,
                                                                                                                                                                     Categoria_ClienteAniel = repoCategoria.selectFirst(z => z.Id == Convert.ToInt64(p)).Result
                                                                                                                                                                 }).ToList();

                objBanco.Tecnicos = view.Equipe == null ? new List<ProgramacaoRoteirizacaoTecnico>() : view.Equipe.Where(p => objBanco.Tecnicos.FirstOrDefault(y => y.Equipe == p) == null).
                                                                                                                                        Select(p => objBanco.Tecnicos.FirstOrDefault(y => y.Equipe == p) ?? new ProgramacaoRoteirizacaoTecnico
                                                                                                                                        {
                                                                                                                                            ProgramacaoRoteirizacaoAutomatica = objBanco,
                                                                                                                                            Id = 0,
                                                                                                                                            Equipe = p,
                                                                                                                                        }).ToList();

                objBanco.Filiais = view.Filiais.Where(p => p.Id == 0).Select(p => new ProgramacaoRoteirizacaoFilial
                {
                    ProgramacaoRoteirizacaoAutomatica = objBanco,
                    Id = 0,
                    CodEmpresa = p.CodEmpresa,
                    CodFilial = p.CodFilial
                }).ToList();

                objBanco.Projetos = view.Projetos.Where(p => p.Id == 0).Select(p => new ProgramacaoRoteirizacaoProjeto
                {
                    ProgramacaoRoteirizacaoAutomatica = objBanco,
                    Id = 0,
                    CodCt = p.CodCt,
                    Projeto = p.Projeto
                }).ToList();

                objBanco.Localidades = view.Localidades.Where(p => p.Id == 0).Select(p => new ProgramacaoRoteirizacaoLocalidade
                {
                    ProgramacaoRoteirizacaoAutomatica = objBanco,
                    Id = 0,
                    CodEscritorio = p.CodEscritorio,
                    CodArea = p.CodArea,
                    Localidade = p.Localidade
                }).ToList();

            }
        }

        private List<int> DiasSemana(ProgramacaoRoteirizacaoAutomaticaCadastroModel view)
        {
            var lista = new List<int>();
            if (view.Dia01 == true)
            {
                lista.Add(1);
            }
            if (view.Dia02 == true)
            {
                lista.Add(2);
            }
            if (view.Dia03 == true)
            {
                lista.Add(3);
            }
            if (view.Dia04 == true)
            {
                lista.Add(4);
            }
            if (view.Dia05 == true)
            {
                lista.Add(5);
            }
            if (view.Dia06 == true)
            {
                lista.Add(6);
            }
            if (view.Dia07 == true)
            {
                lista.Add(7);
            }
            if (view.Dia08 == true)
            {
                lista.Add(8);
            }
            if (view.Dia09 == true)
            {
                lista.Add(9);
            }
            if (view.Dia10 == true)
            {
                lista.Add(10);
            }
            if (view.Dia11 == true)
            {
                lista.Add(11);
            }
            if (view.Dia12 == true)
            {
                lista.Add(12);
            }
            if (view.Dia13 == true)
            {
                lista.Add(13);
            }
            if (view.Dia14 == true)
            {
                lista.Add(14);
            }
            if (view.Dia15 == true)
            {
                lista.Add(15);
            }
            if (view.Dia16 == true)
            {
                lista.Add(16);
            }
            if (view.Dia17 == true)
            {
                lista.Add(17);
            }
            if (view.Dia18 == true)
            {
                lista.Add(18);
            }
            if (view.Dia19 == true)
            {
                lista.Add(19);
            }
            if (view.Dia20 == true)
            {
                lista.Add(20);
            }
            if (view.Dia21 == true)
            {
                lista.Add(21);
            }
            if (view.Dia22 == true)
            {
                lista.Add(22);
            }
            if (view.Dia23 == true)
            {
                lista.Add(23);
            }
            if (view.Dia24 == true)
            {
                lista.Add(24);
            }
            if (view.Dia25 == true)
            {
                lista.Add(25);
            }
            if (view.Dia26 == true)
            {
                lista.Add(26);
            }
            if (view.Dia27 == true)
            {
                lista.Add(27);
            }
            if (view.Dia28 == true)
            {
                lista.Add(28);
            }
            if (view.Dia29 == true)
            {
                lista.Add(29);
            }
            if (view.Dia30 == true)
            {
                lista.Add(30);
            }
            if (view.Dia31 == true)
            {
                lista.Add(31);
            }

            return lista;
        }

        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/ExcluirGridLocalidade")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridCategoria(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ProgramacaoRoteirizacaoLocalidade>(session))
                    {

                        ProgramacaoRoteirizacaoLocalidade modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/ExcluirGridProjeto")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridProjeto(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ProgramacaoRoteirizacaoProjeto>(session))
                    {

                        ProgramacaoRoteirizacaoProjeto modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/ProgramacaoRoteirizacaoAutomaticaApi/ExcluirGridFilial")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridFilial(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<ProgramacaoRoteirizacaoFilial>(session))
                    {

                        ProgramacaoRoteirizacaoFilial modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

    }
}