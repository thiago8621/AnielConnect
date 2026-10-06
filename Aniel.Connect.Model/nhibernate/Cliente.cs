using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente : BaseModel
    {

        private IList<Cliente_Desconto> _descontos;
        private IList<Cliente_PlanoProduto> _planos;
        private IList<Forma_Pagamento_Cliente> _formasPagamento;
        //private IEnumerable<Venda> _vendas;

        public Cliente()
        {
            Endereco_Principal = new Endereco_Cliente();
            //Endereco_Cobranca = new Endereco_Cliente();
        }

        //DADOS GERAIS****************************************************************************************************
        #region


        public virtual Funcionario Vendedor { get; set; }
        public virtual Funcionario Supervisor { get; set; }
        public virtual DateTime? Data_Venda { get; set; }
        public virtual DateTime? Data_Criacao_Cliente { get; set; }

        public virtual String Numero_Contrato { get; set; }
        public virtual String NomeFantasia { get; set; }
        public virtual DateTime? Data_Nascimento { get; set; }
        public virtual Sexo? Sexo { get; set; }
        public virtual String Id_Integrador { get; set; }
        public virtual String CpfCnpj { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual String Nacionalidade { get; set; }
        public virtual Estado_Civil? Estado_Civil { get; set; }
        public virtual Grau_Instrucao? Grau_Instrucao { get; set; }
        public virtual String Profissao { get; set; }
        public virtual Categoria_Cliente Categoria_Cliente { get; set; }
        public virtual Status_Cliente? Status { get; set; }
        public virtual Tipo_Pessoa? Tipo_Pessoa { get; set; }

        public virtual Tipo_Cliente? Tipo_Cliente { get; set; }
        public virtual EtapaVenda Etapa_Venda { get; set; }
        public virtual DateTime? Dt_Ultimo_Contato { get; set; }

        public virtual String Identificador_Telegram { get; set; }

        public virtual String Rg { get; set; }
        public virtual DateTime? Data_Emissao_Rg { get; set; }
        public virtual String Orgao_Emissao_Rg { get; set; }
        public virtual String Filiacao_Pai { get; set; }
        public virtual String Filiacao_Mae { get; set; }
        public virtual bool End_CadastroCondominioProspecto { get; set; }
        public virtual Porte? Porte { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual DateTime? Data_Abertura_Empresa { get; set; }
        public virtual String Inscricao_Estadual { get; set; }
        public virtual String Inscricao_Municipal { get; set; }
        public virtual String Nome_Resp_Financeiro { get; set; }

        public virtual String Cpf_Responsavel { get; set; }
        public virtual String Cargo_Responsavel { get; set; }
        public virtual String Ramo_Atividade { get; set; }
        public virtual EnumInd_InscricaoEstadual Ind_Inscricao_Estadual { get; set; } = EnumInd_InscricaoEstadual.Nao_Contribuinte;

        public virtual String Outro_Motivo { get; set; }

        public virtual String Numero_Validado { get; set; }
        public virtual String Telefone_Fixo { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Telefone_Recado { get; set; }

        public virtual String Celular_Responsavel { get; set; }


        public virtual String Pessoa_Tel_Recado { get; set; }
        public virtual Int64 Regime { get; set; }

        public virtual String Email_Principal { get; set; }
        public virtual String Email_Auxiliar { get; set; }
        public virtual Meio_Captacao Qual_Meio { get; set; }
        public virtual String Outro_Meio { get; set; }
        public virtual Cliente Quem_Indicou { get; set; }
        public virtual Status_Prospecto? Status_Prospecto { get; set; }
        public virtual String Telefone_Fixo2 { get; set; }
        //public virtual String Site { get; set; }
        public virtual String Email_Aux { get; set; }
        public virtual Organizacao Organizacao { get; set; }
        public virtual Motivo_Inviabilidade Motivo_Inviabilidade { get; set; }
        public virtual String Device_id { get; set; }
        public virtual String Informacao_Complementares { get; set; }
        public virtual Int64? IdCampanha { get; set; }


        #endregion

        //PLANOS E PRODUTO****************************************************************************************************
        #region



        //public virtual Double? Valor_Final { get; set; }


        //public virtual Double? Valor_Instalacao_Final { get; set; }


        //public virtual Boolean TesteGratis { get; set; }


        //public virtual Int64? Qtde_Dias { get; set; }


        //public virtual Boolean Periodo_Carencia { get; set; }


        //public virtual Int64? Meses_Carencia { get; set; }


        //public virtual Int64? Numero_Parcelas_Instalacao { get; set; }

        public virtual IList<Cliente_PlanoProduto> PlanosProdutos
        {
            get
            {
                if (_planos == null)
                    _planos = new List<Cliente_PlanoProduto>();
                return _planos;
            }
            set
            {
                _planos = value;
            }
        }

        #endregion

        //MODAL RADIUS****************************************************************************************************
        #region

        public virtual String Login { get; set; }


        public virtual String Senha { get; set; }


        public virtual String Confirmar_Senha { get; set; }

        #endregion

        //FINCANCEIRO****************************************************************************************************
        #region  

        //public virtual Forma_Pagamento Forma_Pagamento { get; set; } //Remover

        public virtual IList<Forma_Pagamento_Cliente> Formas_Pagamento_Cliente
        {
            get
            {
                if (_formasPagamento == null)
                    _formasPagamento = new List<Forma_Pagamento_Cliente>();
                return _formasPagamento;
            }
            set
            {
                _formasPagamento = value;
            }
        }

        /*public virtual Banco Banco { get; set; }


        public virtual String Conta_Corrente { get; set; }


        public virtual String Agencia { get; set; }
        public virtual String Digito { get; set; }


        public virtual Vencimento Vencimento { get; set; }

        public virtual String Dia_Vencimento { get; set; }*/


        public virtual Endereco_Cliente Endereco_Principal { get; set; }
        public virtual Endereco_Cliente Endereco_Prospecto { get; set; }
        public virtual Endereco_Cliente Endereco_Instalacao { get; set; }

        //public virtual Endereco_Cliente Endereco_Cobranca { get; set; }

        public virtual Boolean End_Cobranca_Igual_Principal { get; set; }

        public virtual Boolean End_Instalacao_Igual_Principal { get; set; }

        public virtual String End_Cobranca_Igual_Principal_Str { get; set; }
        public virtual String End_Instalacao_Igual_Principal_Str { get; set; }

        public virtual IList<Cliente_Desconto> Descontos
        {
            get
            {
                if (_descontos == null)
                    _descontos = new List<Cliente_Desconto>();
                return _descontos;
            }
            set
            {
                _descontos = value;
            }
        }


        //public virtual IEnumerable<Venda> Vendas
        //{
        //    get
        //    {
        //        if (_vendas == null)
        //            _vendas = new List<Venda>();
        //        return _vendas;
        //    }
        //    set
        //    {
        //        _vendas = value;
        //    }
        //}


        #endregion

        public virtual String Site { get; set; }
        public virtual bool Alterado { get; set; }

        //public virtual Cliente_FormaEnvio? Forma_Envio { get; set; }
        public virtual IdentityUser Usuario { get; set; }
        public virtual Cliente_Anexo Foto_Perfil { get; set; }
        public virtual Departamento Departamento { get; set; }

        public virtual string UsuarioPlayhub { get; set; }

        public virtual bool IntegradoOla { get; set; }

        public virtual string Origem_Cliente { get; set; }


        public virtual String getTelefone()
        {
            String telefone = "";
            if (!String.IsNullOrEmpty(Telefone_Celular))
            {
                telefone = Telefone_Celular;
            }
            else if (!String.IsNullOrEmpty(Telefone_Fixo))
            {
                telefone = Telefone_Fixo;
            }
            else if (!String.IsNullOrEmpty(Telefone_Fixo2))
            {
                telefone = Telefone_Fixo2;
            }
            else if (!String.IsNullOrEmpty(Telefone_Recado))
            {
                telefone = Telefone_Recado;
            }
            else if (!String.IsNullOrEmpty(Celular_Responsavel))
            {
                telefone = Celular_Responsavel;
            }
            return telefone;
        }

        public static implicit operator Cliente(Task<Cliente> v)
        {
            throw new NotImplementedException();
        }
    }
}
