using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Vincular_UsuarioPerfilController : BaseController
    {

        public Vincular_UsuarioPerfilController() : base("Vincular_UsuarioPerfil",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Vincular_UsuarioPerfilApi/FindById?id={0}",
                                                   "api/Vincular_UsuarioPerfilApi/Gravar",
                                                   "api/Vincular_UsuarioPerfilApi/Excluir",
                                                   String.Empty)
        {

        }

        // Dentro de Vincular_UsuarioPerfilController.cs

        // GET: Vincular_UsuarioPerfil
        public ActionResult Index()
        {
            List<SelectValue> combo = new List<SelectValue>();

            foreach (int item in Enum.GetValues(typeof(SecaoConnectPerfilEnum)))
            {
                combo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(SecaoConnectPerfilEnum)).GetValue(item).ToString()) });
            }

            List<SelectValue> comboTipo = new List<SelectValue>();

            foreach (int item in Enum.GetValues(typeof(TipoUsuarioEnum)))
            {
                comboTipo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(TipoUsuarioEnum)).GetValue(item).ToString()) });
            }

            List<SelectValue> comboAgentes = new List<SelectValue>();

            // 1. Declaração da lista de modelos para ser preenchida dentro do 'using'
            var listaModelos = new List<System.Web.Mvc.SelectListItem>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            using (var user = new Conexao<IdentityUser>(session))
            {
                var perfis = repository.select().Result;
                foreach (var agente in perfis)
                {
                    comboAgentes.Add(new SelectValue() { Id = agente.Id, Descricao = agente.Descricao });
                }

                try
                {
                    // ASSUMINDO que a sua classe de persistência é Aniel.Connect.Model.nhibernate.Modelo_PainelServico
                    // E QUE O CAMPO DE DESCRIÇÃO é 'Descricao'

                    using (var repoModelos = new Conexao<Aniel.Connect.Model.nhibernate.Modelo_PainelServico>(session))
                    {
                        // Busca modelos ativos e ordena (Adapte a condição 'p.Ativo' se necessário)
                        var modelos = repoModelos.select().Result.OrderBy(p => p.Descricao);

                        foreach (var modelo in modelos)
                        {
                            listaModelos.Add(new System.Web.Mvc.SelectListItem
                            {
                                Value = modelo.Id.ToString(), // ID como valor
                                Text = modelo.Descricao // Descrição como texto exibido
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção de forma adequada.
                }
                // ==============================================================================================

            } // Fim do bloco 'using' principal

            ViewBag.Secao = combo;
            ViewBag.PerfilAgente = comboAgentes;
            ViewBag.Tipos = comboTipo;
            ViewBag.Areas = Areas;

            // 2. Atribuição da lista de modelos ao ViewBag
            ViewBag.ListaModelosPainelServico = listaModelos;

            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(String id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Vincular_UsuarioPerfilCadastroModel, String>(id);
            var listaModelos = new List<System.Web.Mvc.SelectListItem>();
            Perfil_DashboardApiController con = new Perfil_DashboardApiController();
            Vincular_UsuarioPerfilApiController api = new Vincular_UsuarioPerfilApiController();

            List<SelectValue> comboAgente = new List<SelectValue>();

            foreach (var obj in api.GetCombo(new RequestObjComboPai<string, string>()).Result.results)
            {
                comboAgente.Add(new SelectValue() { IdString = obj.Id, Descricao = obj.Text });
            }
           
            //List<Combo> combo = new List<Combo>();

            //foreach (var obj in con.GetCombo(new ParamWebApi<Perfil_DashboardCadastroModel>()).Result.results)
            //{
            //    combo.Add(new Combo() { IdString = obj.Id, Descricao = obj.Text });
            //}

            //ViewBag.Perfis = combo;

            ViewBag.Agentes = comboAgente;
            ViewBag.ListaModelosPainelServico = listaModelos;


            return retorno;
        }

        // POST: Vincular_UsuarioPerfil/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Vincular_UsuarioPerfilCadastroModel pModel)
        {
            return base.CadastroBase<Vincular_UsuarioPerfilCadastroModel, String>(pModel);
        }
        
        [HttpGet()]
        public ActionResult CadastroMultiplo(string id)
        {
            List<string> ids = id.Split(',').ToList<string>();

            Vincular_MultiploPerfilCadastroModel retorno = new Vincular_MultiploPerfilCadastroModel() { Id = ids };

            List<SelectValue> combo = new List<SelectValue>();
            var listaModelos = new List<System.Web.Mvc.SelectListItem>();
            foreach (int item in Enum.GetValues(typeof(SecaoConnectPerfilEnum)))
            {
                combo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(SecaoConnectPerfilEnum)).GetValue(item).ToString()) });
            }

            List<SelectValue> comboTipo = new List<SelectValue>();

            foreach (int item in Enum.GetValues(typeof(TipoUsuarioEnum)))
            {
                comboTipo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(TipoUsuarioEnum)).GetValue(item).ToString()) });
            }

            List<SelectValue> comboAgentes = new List<SelectValue>();

            Vincular_UsuarioPerfilApiController api = new Vincular_UsuarioPerfilApiController();

            foreach (var obj in api.GetCombo(new RequestObjComboPai<string, string>()).Result.results)
            {
                comboAgentes.Add(new SelectValue() { IdString = obj.Id, Descricao = obj.Text });
            }

           

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            using (var user = new Conexao<IdentityUser>(session))
            {
            
                foreach (var idUsuario in ids)
                {
                    if(user.select(x => x.Id == idUsuario).Result[0].Tipo_Usuario != null || user.select(x => x.Id == idUsuario).Result[0].Tipo_Usuario != 0)
                    {
                        retorno.TemPerfil = true;
                        break;
                    }
                }
                try
                {
                    // ASSUMINDO: Aniel.Connect.Model.nhibernate.Modelo_PainelServico é sua classe
                    using (var repoModelos = new Conexao<Aniel.Connect.Model.nhibernate.Modelo_PainelServico>(session))
                    {
                        // Busca modelos ativos e ordena
                        var modelos = repoModelos.select().Result.OrderBy(p => p.Descricao);

                        foreach (var modelo in modelos)
                        {
                            listaModelos.Add(new System.Web.Mvc.SelectListItem
                            {
                                Value = modelo.Id.ToString(),
                                Text = modelo.Descricao
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Implementar log
                }
            }

            ViewBag.Secao = combo;
            ViewBag.PerfilAgente = comboAgentes;
            ViewBag.Tipos = comboTipo;
            ViewBag.ListaModelosPainelServico = listaModelos;


            return View(retorno);
        }

        // POST: Vincular_UsuarioPerfil/Cadastro/5
        [HttpPost]
        public ActionResult CadastroMultiplo(Vincular_MultiploPerfilCadastroModel pModel)
        {
            var model = new ParamWebApi<Vincular_MultiploPerfilCadastroModel>() { Objeto = pModel };

            var api = new Vincular_UsuarioPerfilApiController().GravarMultiplo(model);

            return RedirectToAction(nameof(Index));

        }

        [HttpGet()]
        public ActionResult Historico_Alteracao(Int64? id)
        {
            ViewBag.Chave = "Id";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
            List<SelectValue> combo = new List<SelectValue>();

            foreach (int item in Enum.GetValues(typeof(SecaoConnectPerfilEnum)))
            {
                combo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(SecaoConnectPerfilEnum)).GetValue(item).ToString()) });
            }

            List<SelectValue> comboTipo = new List<SelectValue>();

            foreach (int item in Enum.GetValues(typeof(TipoUsuarioEnum)))
            {
                comboTipo.Add(new SelectValue() { Id = item, Descricao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(TipoUsuarioEnum)).GetValue(item).ToString()) });
            }

            ViewBag.Secao = combo;
            ViewBag.Tipos = comboTipo;

        }

        // GET: Vincular_UsuarioPerfil/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Vincular_UsuarioPerfilCadastroModel, Int64>(id.Value);
        }

        // POST: Vincular_UsuarioPerfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Vincular_UsuarioPerfilCadastroModel pModel)
        {
            return base.DeleteBase<Vincular_UsuarioPerfilCadastroModel, Int64>(pModel);
        }
    }

    public class SelectValue
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string IdString { get; set; }
    }

}