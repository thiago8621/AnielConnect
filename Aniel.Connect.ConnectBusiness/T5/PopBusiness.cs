using Aniel.Connect.Repository.T5.Equipe;

using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Core;
using System;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class PopBusiness
    {
        private readonly IPopRepository _repoPop;
        private readonly IContratoAnielRepository _repoContrato;

        public PopBusiness(IPopRepository repoPop)
        {
            this._repoPop = repoPop;
        }


        public System.Collections.Generic.List<PopModelsResponse> GetAll()
        {
            return this._repoPop.GetAll();
        }

        private PostApiRetorno ValidaCampos(PopModels dados)
        {
            var retorno = new PostApiRetorno();

            try
            {
                retorno.Ok = true;

                if ((string.IsNullOrEmpty(dados.CODCT)))
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Preencher o CONTRATO";

                }
                if ((string.IsNullOrEmpty(dados.CODCT)))
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Preencher o CONTRATO";
                }
                if ((dados.ID_FILIAL == 0))
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Preencher o ID_FILIAL";

                }
                if ((dados.ID_EMPRESA == 0))
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Preencher o ID_EMPRESA";

                }
                if ((dados.ID_CIDADE == 0))
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Preencher o ID_CIDADE.";

                }
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.ToString();
                retorno.Mensagem = ex.InnerException.ToString();
            }
            return retorno;
        }
        public PostApiRetorno UpdatePop(PopModels dados, IContratoAnielRepository reposContrato)
        {
            var retornoCampos = ValidaCampos(dados);
            if (!retornoCampos.Ok)
            {
                return retornoCampos;
            }
            var retorno = new PostApiRetorno();
            try
            {

                var contrato = reposContrato.GetByCodCt(dados.CODCT);
                if (contrato is null)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Contrato não localizado";
                    return retorno;
                }
                return _repoPop.AtualizarPop(dados);

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.ToString();
                retorno.Mensagem = ex.InnerException.ToString();
                return retorno;
            }
        }

        public PostApiRetorno AdicionarPop(PopModels dados, IContratoAnielRepository reposContrato)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                var retornoCampos = ValidaCampos(dados);
                if (!retornoCampos.Ok)
                {
                    return retornoCampos;
                }
                var contrato = reposContrato.GetByCodCt(dados.CODCT);
                if (contrato is null)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Contrato não localizado";
                    return retorno;
                }
                retorno = _repoPop.InsertPop(dados);
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException.ToString();
                retorno.Mensagem = ex.InnerException.ToString();
                return retorno;
            }
            return retorno;
        }

    }
}
