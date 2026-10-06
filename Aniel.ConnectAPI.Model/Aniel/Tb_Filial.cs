using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    [PVTabela("TB_PV_FILIAL")]
    public class Tb_Filial
    {
        [PVChave("Codempresa")]
        public virtual Int16 Codempresa { get; set; }
        [PVChave("Codfilial")]
        public virtual Int16 Codfilial { get; set; }
        public virtual String Nome { get; set; }
        public virtual String CodigoCedente { get; set; }
        public virtual String DigitoVerificadorCedente { get; set; }
        public virtual String Carteira { get; set; }
        public virtual String Cnpj { get; set; }

        public virtual String Endereco { get; set; }
        public virtual Int32? Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }

        public virtual String Telefone { get; set; }

        public virtual Int64? Id_Filial_Connect { get; set; }

        public virtual int? Natureza_tributacao { get; set; }
        public virtual string Razao_Social { get; set; }
        public virtual Boolean Optante_Simples_Nacional { get; set; }
        public virtual int? Regime_Tributario { get; set; }
        public virtual Boolean Incentivo_Cultural { get; set; }
        public virtual Boolean Incentivo_Fiscal { get; set; }
        public virtual int? Regime_Tributario_Especial { get; set; }
        public virtual string DDD { get; set; }
        public virtual string Email { get; set; }
        public virtual string CNAE_Fiscal { get; set; }
        public virtual int? Tipo_Tributacao_ISS { get; set; }
        public virtual int? Exigibilidade_ISS { get; set; }
        public virtual string Num_Processo_Suspensao { get; set; }
        public virtual string Inscricao_Municipal { get; set; }
        public virtual string Inscricao_Estadual { get; set; }
    }
}
