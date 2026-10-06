using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Teste
{
    [TestClass]
    public class Tipo_DescontoTeste : TesteBase<Tipo_DescontoCadastroModel>
    {

        [TestMethod]
        public async Task Passo01_GetAll()
        {
            await GetAll<Tipo_DescontoApiController, Tipo_Desconto_DataTableRequest, Tipo_DescontoListaModel>();
        }

        private Boolean Compare(Tipo_DescontoCadastroModel pObjInsert, Tipo_DescontoCadastroModel pObjFindById)
        {
            return pObjInsert.Descricao != pObjFindById?.Descricao;
        }

        private Tipo_DescontoCadastroModel Alter(Tipo_DescontoCadastroModel pObjFindById)
        {
            pObjFindById.Descricao = StringService.AlfanumericoAleatorio(100);
            return pObjFindById;
        }

        public Tipo_DescontoCadastroModel GetNewMock()
        {
            return new Tipo_DescontoCadastroModel
            {
                Codigo = IntService.IntAleatorio(),
                Descricao = StringService.AlfanumericoAleatorio(100)
            };
        }

        public async Task Excluir(Int64 Id)
        {
            await Excluir<Tipo_DescontoApiController, Int64, Int64>(new ParamWebApi<Int64>
            {
                Objeto = Id,
                Usuario = CONSTANTES.USUARIO_PADRAO
            });
        }

        [TestMethod]
        public async Task Passo02_Crud()
        {
            Tipo_DescontoCadastroModel ObjInsert = GetNewMock();

            await Crud_Basico<Tipo_DescontoApiController, Tipo_DescontoCadastroModel>(ObjInsert,
            (pObjInsert, pObjFindById) => Compare(pObjInsert, pObjFindById)
            ,
            (pObjFindById) => Alter(pObjFindById));
        }

        //[TestMethod]
        //public async Task Passo03_GetCombo()
        //{
        //    using (var api = new Tipo_DescontoApiController())
        //    {
        //        try
        //        {
        //            var retorno = await api.GetCombo(new RequestObjCombo<string>());
        //        }
        //        catch (Exception ex)
        //        {
        //            Assert.IsTrue(false, ex.Message);
        //        }
        //    }
        //}

    }
}
