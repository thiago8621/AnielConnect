using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Model.nhibernate
{
    public class Organizacao : BaseModel
    {
        private IList<Dominio_Organizacao> _dominios;
        private IList<Organizacao_Contato> _contatos;
        

        public virtual String Nome_Fantasia { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual String CNPJ { get; set; }
        public virtual Classificacao_Organizacao Classificacao_Organizacao { get; set; }
        public virtual Status_Default Status { get; set; }

        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; } // ENDERECO
        public virtual Int32? Numero { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Complemento { get; set; }

        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual Int64? Id_Bairro { get; set; }


        public virtual IList<Dominio_Organizacao> Dominios
        {
            get
            {
                if (_dominios == null)
                    _dominios = new List<Dominio_Organizacao>();
                return _dominios;
            }
            set
            {
                _dominios = value;
            }
        }

        public virtual IList<Organizacao_Contato> Contatos
        {
            get
            {
                if (_contatos == null)
                    _contatos = new List<Organizacao_Contato>();
                return _contatos;
            }
            set
            {
                _contatos = value;
            }
        }

    }
}
