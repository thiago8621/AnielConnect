using Aniel.Connect.Repository.Repository.T5.InventarioCliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class InventarioClienteAnielBusiness
    {
        private readonly IInventarioClienteAnielRepository _inventarioCliente;

        public InventarioClienteAnielBusiness(IInventarioClienteAnielRepository inventarioCliente)
        {
            _inventarioCliente = inventarioCliente;
        }

        public async Task<bool> ExisteInventarioPendenteAvaliacao(string cpf_cnpj, string codMat, string serial)
        {
            return await this._inventarioCliente.ExisteInventarioPendenteAvaliacao(cpf_cnpj, codMat, serial);
        }

        public async Task GerarInventarioCliente(Aniel.T5.Model.nhibernate.ClienteAniel clienteAniel, string usuario, string codMat, string serial)
        {
            var protocoloInventario = await this._inventarioCliente.GerarProtocoloInvCapa(clienteAniel, usuario);
            
            await this._inventarioCliente.InserirMaterialCliente(codMat, serial, usuario, protocoloInventario);            
        }
    }
}
