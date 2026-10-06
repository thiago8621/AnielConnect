using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.InventarioCliente
{
    public interface IInventarioClienteAnielRepository
    {
        /// <summary>
        ///     Retorna se já existe um inventário pendente para o mesmo cliente. 
        ///     Se for informado Código do Material e Serial (codcpl), retorna se já existe inventário para o mesmo cliente com este serial.
        /// </summary>
        Task<bool> ExisteInventarioPendenteAvaliacao(string cpf_cnpj, string codMat, string serial);

        /// <summary>
        ///     Gera a capa para um novo Inventário de Cliente pendente de avaliação na TB_INVENTARIO_CLIENTE.
        /// </summary>
        Task<Int64> GerarProtocoloInvCapa(Aniel.T5.Model.nhibernate.ClienteAniel cliente, string usuarioCriacao);

        /// <summary>
        ///     Gera o registro de material no Inventário de Cliente na TB_INVENTARIO_CLIENTE_MAT.
        /// </summary>
        Task InserirMaterialCliente(string codMat, string serial, string usuario, Int64 protocolo);

        /// <summary>
        ///     Retorna o próximo sequencial na TB_INVENTARIO_CLIENTE.
        /// </summary>
        Task<string> getProximoSequencialInventario();

        /// <summary>
        ///     Retorna o próximo sequencial na TB_INVENTARIO_CLIENTE_MAT.
        /// </summary>
        Task<long> getProximoSequencialMaterial();
    }
}
