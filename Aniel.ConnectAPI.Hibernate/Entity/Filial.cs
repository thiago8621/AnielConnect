using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Filial : BaseClass
    {
        public virtual Int16 CodFilial { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Cnpj { get; set; }
        public virtual string CodigoCedente { get; set; }
        public virtual string DigitoVerificadorCedente { get; set; }
        public virtual string Carteira { get; set; }
        public virtual Empresa Empresa { get; set; }

        public virtual String Endereco { get; set; }
        public virtual Int32? Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }

        public virtual String Telefone { get; set; }


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
