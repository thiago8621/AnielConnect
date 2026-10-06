using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Resources;
using Sinapse.Model.enumerado;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Aniel.T5.Model.enumerado;
using RestSharp.Validation;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Escala_TrabalhoController : BaseController
    {

        public Escala_TrabalhoController() : base("Escala_Trabalho",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Escala_Trabalhoapi/FindById?id={0}",
                                                   "api/Escala_Trabalhoapi/Gravar",
                                                   "api/Escala_Trabalhoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Escala_Trabalho
        public ActionResult Index(string equipe = null)
        {
            ViewBag.Areas = Areas;

            List<Int32> anos = new List<Int32>();

            for (var i = 2016; i < 2036; i++)
            {

                anos.Add(i);

            }

            ViewBag.Anos = anos;
            



            return View(new Escala_Trabalho_FiltroModel
            {

                Ano = DateTime.Now.Year,
                Mes = (Mes)Enum.Parse(typeof(Mes), DateTime.Now.Month.ToString()),
                Situacao_Equipe = Situacao_Escala_Trabalho.Ativo,
                Id_Equipe = equipe
            });
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id, Int16? mes, Int16? ano, Int16? dia, String equipe, String tecnicos, String horarioAtribuido)
        {
            if (id == 0)
            {
                return View(new Escala_TrabalhoCadastroModel
                {
                    Ano = ano,
                    Mes = (Mes)Enum.Parse(typeof(Mes), Convert.ToString(mes)),
                    Dia = dia,
                    Equipe = Convert.ToString(equipe),
                    Tecnicos = tecnicos,
                    HorarioServicoAtribuido = (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(horarioAtribuido))
                });
            }
            else
                return base.CadastroBase<Escala_TrabalhoCadastroModel, Int64?>(id);
        }


        // POST: Escala_Trabalho/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Escala_TrabalhoCadastroModel pModel)
        {
            Boolean erro = false;
            
                if (!String.IsNullOrEmpty(pModel.Data_Inicial))
                {
                    if (Convert.ToDateTime(pModel.Data_Inicial) < DateTime.Now.Date)
                    {
                        AddModelError(RsCadastros.DATAINICIAL_INFERIORDATAATUAL);
                        erro = true;
                    }
                   
                }
                if (!String.IsNullOrEmpty(pModel.Data_Final))
                {
                    if (Convert.ToDateTime(pModel.Data_Final) < DateTime.Now.Date)
                    {
                        AddModelError(RsCadastros.DATAFINAL_INFERIORDATAATUAL);
                        erro = true;
                    }
                  
                }
                if (erro == false)
                {
                    if (Convert.ToDateTime(pModel.Data_Final) < Convert.ToDateTime(pModel.Data_Inicial))
                    {
                        AddModelError(Resources.RsBase.PERIODO_INVALIDO);
                        erro = true;
                    }
                }
            

            if (pModel.EscalarEmMassa == true)
            {

                if (String.IsNullOrEmpty(pModel.Data_Inicial) == false && String.IsNullOrEmpty(pModel.Data_Final))
                {
                    pModel.Data_Final = pModel.Data_Inicial;
                }

                var model = new ParamWebApi<Escala_TrabalhoCadastroModel>
                {
                    Objeto = new Escala_TrabalhoCadastroModel()
                    {
                        Data_Inicial = pModel.Data_Inicial,
                        Data_Final = pModel.Data_Final,
                        Id_Horario = pModel.Id_Horario,
                        Id_PerfilAgendamento = pModel.Id_PerfilAgendamento,
                        Tecnicos = pModel.Tecnicos
                    }

                };
                model.Objeto.Tecnicos.Split(',');
            }


            if (erro)
            {
                FillListasView(pModel);
                return View(pModel);
            }

            ActionResult retorno = base.CadastroBase<Escala_TrabalhoCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                ViewBag.Sucesso = true;

                FillListasView(pModel);

                return View(pModel);
            }

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

            ViewBag.Horarios = new List<Horario_ListModel>();
            var _retornohorarios = new HorarioApiController().GetListServicos().Result;

            if (!_retornohorarios.Ok)
            {
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornohorarios.Mensagem, _retornohorarios.InnerException);
            }

            ViewBag.Horarios = _retornohorarios.Objeto;

            ViewBag.Feriados = new List<FeriadoListaModel>();

        }

        // GET: Escala_Trabalho/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Escala_TrabalhoCadastroModel, Int64>(id.Value);
        }

        // POST: Escala_Trabalho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Escala_TrabalhoCadastroModel pModel)
        {
            return base.DeleteBase<Escala_TrabalhoCadastroModel, Int64>(pModel);
        }

        public ActionResult Historico_Alteracao()
        {
            ViewBag.Areas = Areas;
            return View();
        }



        public ActionResult EscalarEmMassa(Int64? id, Int16? mes, Int16? ano, Int16? dia, String equipe, String tecnicos)
        {
            if (id == 0)
            {
                return View(new Escala_TrabalhoCadastroModel
                {
                    Ano = ano,
                    Mes = (Mes)Enum.Parse(typeof(Mes), Convert.ToString(mes)),
                    Dia = dia,
                    Equipe = Convert.ToString(equipe),
                    Tecnicos = tecnicos
                });
            }
            else
                return base.CadastroBase<Escala_TrabalhoCadastroModel, Int64?>(id);
        }

        public ActionResult HorarioServicoAtribuido(String tecnico)
        {

            using (Escala_TrabalhoApiController api = new Escala_TrabalhoApiController())
            {
                var horarioAtribuido = api.BuscarHorarioAtribuido(tecnico).Result?.Objeto;

                return View(new Escala_TrabalhoCadastroModel
                {
                    Tecnicos = tecnico,
                    HorarioServicoAtribuido = horarioAtribuido
                });
            }

        }

        [HttpPost]
        public ActionResult HorarioServicoAtribuido(Escala_TrabalhoCadastroModel model)
        {
            using (Escala_TrabalhoApiController api = new Escala_TrabalhoApiController())
            {
                api.GravarHorarioAtribuido(model.Tecnicos,model.HorarioServicoAtribuido).Wait();
                ViewBag.Sucesso = true;
                return View();
            }

        }


        //public ActionResult RetornarEscalaPorId(string equipeId)
        //{
        //    using (Escala_TrabalhoApiController api = new Escala_TrabalhoApiController())
        //    {
        //        api.GetAll(new Escala_Trabalho_DataTableRequest { IdTecnico = equipeId });
        //    }
        //}
    }


}