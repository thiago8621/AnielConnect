using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Tb_Funcionario
    {

        public virtual String NUM_FRE { get; set; }
        public virtual String NUM_FRE_AUX { get; set; }
        public virtual String NOME { get; set; }
        public virtual String PAI { get; set; } // 50
        public virtual String MAE { get; set; } // 50
        public virtual DateTime? DATA_NASC { get; set; }

        public virtual Int64? Id_Bairro { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual String E_GESTOR { get; set; }

        public virtual String FUNCIONARIO_NAO_MARCA_PONTO { get; set; }

        public virtual String Device_Id_Ponto { get; set; }

        public virtual String PORTADOR_DEFICIENCIA { get; set; }
        public virtual String DEFICIENCIA { get; set; }
        public virtual String ESTADO_CIVIL { get; set; }
        public virtual Double? LATITUDE { get; set; }
        public virtual Double? LONGITUDE { get; set; }
        public virtual Double? RAIO_DISTANCIA { get; set; }
        public virtual String CPF { get; set; } // 14
        public virtual String RG { get; set; } // 20
        public virtual String UF_RG { get; set; }
        public virtual DateTime? DATA_RG { get; set; }
        public virtual String EMISSOR_RG { get; set; }
        public virtual String CTPS { get; set; } //20
        public virtual DateTime DATA_CTPS { get; set; } // *
        public virtual String SERIE_CTPS { get; set; } //10
        public virtual String UF_CTPS { get; set; }//2        
        public virtual DateTime? DATA_ADMISSAO { get; set; }
        public virtual DateTime? DATA_DEMISSAO { get; set; }
        public virtual Boolean MARCACAO_RESTRITA_RESIDENCIA { get; set; }
        public virtual String ENDERECO { get; set; }
        public virtual Int32? NUMERO { get; set; }
        public virtual String COMPLEMENTO { get; set; } // 30
        public virtual String CEP { get; set; } // 10
        public virtual String BAIRRO { get; set; } // 20
        public virtual String MUNICIPIO { get; set; } // 30
        public virtual Int64 ID_MUNICIPIO_TRABALHO { get; set; } // 30
        public virtual String MUNICIPIO_TRABALHO { get; set; } // 30
        public virtual String UF { get; set; } // 30
        public virtual String UF_TRABALHO { get; set; } // 30
        public virtual String EMAIL { get; set; } //50
        public virtual String TELEFONE { get; set; } //15
        public virtual String TEL_CELULAR { get; set; } //15
        public virtual String NACIONALIDADE { get; set; } // 20
        public virtual String NATURALIDADE { get; set; } // 20
        public virtual Tb_Pt_Jornada JORNADA { get; set; }

        public virtual Int16 CODFILIAL { get; set; }

        public virtual String SUPERVISOR { get; set; }

        public virtual String PIS { get; set; }

        public virtual String GESTOR_PONTO { get; set; }
        public virtual DateTime? DATAINICIO_PRIMEIRAFOLHA { get; set; }
        public virtual DateTime? DATAINICIO_PRIMEIRAFOLGA { get; set; }

        public virtual Double HXH { get; set; }

        public virtual String USUARIO { get; set; }

        public virtual String USUARIO_ALTEROU { get; set; }


        public virtual String QRCODE_FUNCIONARIO { get; set; }

        public override string ToString()
        {
            return NOME;
        }

        public override int GetHashCode()
        {
            return NUM_FRE.GetHashCode();
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return NUM_FRE.Equals(((Tb_Funcionario)obj).NUM_FRE);
        }
    }
}
