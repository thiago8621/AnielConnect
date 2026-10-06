using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_FUNCIONARIO")]
    public class Tb_Funcionario
    {
        [PVChave("Num_Fre")]
        public String Num_Fre { get; set; }
        public String Nome { get; set; }
        public String Tel_Celular { get; set; }
        public Int16 Situacao { get; set; }
        public Int16 Codempresa { get; set; }
        public Int16 Codfilial { get; set; }

        public Int32? Cod_CCusto { get; set; }

        public String Supervisor { get; set; }

        public Int16? Cod_Depto { get; set; }
        public String Depto { get; set; }

        public virtual String Cpf { get; set; }
        public virtual String Rg { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Complemento { get; set; }
        public virtual Int32? Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Municipio { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }

    }
}
