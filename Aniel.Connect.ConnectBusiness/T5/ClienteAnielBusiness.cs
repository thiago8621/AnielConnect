using Aniel.Connect.Repository.T5.Cliente;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ClienteAnielBusiness
    {
        private readonly IClienteAnielRepository _repoClienteAniel;

        public ClienteAnielBusiness(IClienteAnielRepository repoClienteAniel)
        {
            this._repoClienteAniel = repoClienteAniel;
        }

        public string GetNome(int codCliente)
        {
            return this._repoClienteAniel.GetNome(codCliente);
        }

        public Aniel.T5.Model.nhibernate.ClienteAniel GetClientebyCPFCNPJ(string CPFCNPJ)
        {
            if (!String.IsNullOrEmpty(CPFCNPJ) && !String.IsNullOrEmpty(Regex.Replace(CPFCNPJ, @"[^\d]", "")))
                return this._repoClienteAniel.GetAll().Where(p => p.CPF == CPFCNPJ).FirstOrDefault();
            else
                return null;
        }
    }
}
