using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.T5.Porta;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Data;
using System.Xml.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class AlteracaoPortaAnielBusiness
    {
        private readonly IPortaAnielRepository _repoPortaAniel;

        public AlteracaoPortaAnielBusiness(IPortaAnielRepository repoPortaAniel)
        {
            this._repoPortaAniel = repoPortaAniel;
        }

        public Elemento_Rede_Porta GetPortabyCliente(Int64 idCliente)
        {
            return this._repoPortaAniel.GetPortabyCliente(idCliente);
        }

        public void AlteraStatusPortaReservada(Elemento_Rede_Porta elemento)
        {
            try
            {
                elemento.Status = Situacao_Porta.Disponivel;
                elemento.DataHoraAlteracao = DateTime.Now;
                this._repoPortaAniel.Update(elemento);
            }
            catch (Exception ex)
            {
                // Aqui você pode personalizar a mensagem de erro com detalhes
                string mensagemDeErro = $"Ocorreu um erro ao tentar alterar o status da porta: {ex.Message}";

                // Você pode também registrar a exceção em um log, se necessário
                // Logger.Log(ex);

                // Lançar a exceção novamente (opcional) para que possa ser tratada em um nível superior
                throw new Exception(mensagemDeErro, ex);
            }
        }
    }
}
