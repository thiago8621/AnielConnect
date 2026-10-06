using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade.Interface;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.EnderecoLogradouro;
using Aniel.Connect.Repository.T5.ElementoRedeEnderecos;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade
{
    public class ConsultaViabilidadeCadastroEndereco : IEtapaViabilidade
    {
        private bool _utilizaCadastroEndereco;
        private long? _idEnderecoLogradouro;
        private long? _idCondominio;
        private long? _idBloco;
        private long? _idApto_Numero;
        private string _numero;
        private string _logradouro;
        private long? _cidade;
        private ICidadeRepository _repository;
        private IElementoRedeEnderecosAnielRepository _repositoryElementoEndereco;
        private IEnderecoLogradouroRepository _repositoryEnderecoLogradouro;
        public ConsultaViabilidadeCadastroEndereco(double? latitude, double? longitude, long? clientePlanoProduto, bool utilizaCadastroEndereco, 
            long? idEnderecoLogradouro, long? idCondominio, string numero,long? cidade, string logradouro, long? idBloco, long? idApto_Numero, ICidadeRepository repository, 
            IElementoRedeEnderecosAnielRepository repositoryElementoEndereco,
            IEnderecoLogradouroRepository repositoryEnderecoLogradouro)
        {
            Latitude = latitude.Value;
            Longitude = longitude.Value;
            ClientePlanoProduto = clientePlanoProduto;
            _utilizaCadastroEndereco = utilizaCadastroEndereco;
            _idEnderecoLogradouro = idEnderecoLogradouro;
            _idCondominio = idCondominio;
            _idBloco = idBloco;
            _idApto_Numero = idApto_Numero;
            _numero = numero;
            _cidade = cidade;
            _logradouro = logradouro;
            _repository = repository;
            _repositoryElementoEndereco = repositoryElementoEndereco;
            _repositoryEnderecoLogradouro = repositoryEnderecoLogradouro;
        }

        public EtapasValidacaoConsultaViabilidadeEnum Etapa
        {
            get
            {
                return EtapasValidacaoConsultaViabilidadeEnum.CadastroEndereco;
            }
        }

        public double Latitude { get; set ; }
        public double Longitude { get; set; }
        public long? ClientePlanoProduto { get; set; }

        public GetApiRetorno<List<Elemento_Rede>> ExecutarConsulta()
        {
            var retorno = new GetApiRetorno<List<Elemento_Rede>>
            {
                Objeto = new List<Elemento_Rede>()
            };
            try 
            {
                var listagemLogradouroElemento = new List<Elemento_Rede_Enderecos>();
                var elementos = new List<Elemento_Rede>();
                var caixasCadastroEndereco = new List<Elemento_Rede>();

                if (this._utilizaCadastroEndereco)
                {

                    listagemLogradouroElemento = this._repositoryElementoEndereco.GetAll()
                                                                                 .Where(p => p.Todos_Bloco == true ? p.Id_CondominioPredio == this._idCondominio :
                                                                                 p.Todos_Apto_Numero == true ? p.Id_CondominioPredio == this._idCondominio && p.Bloco == this._idBloco :
                                                                                 p.Id_CondominioPredio == this._idCondominio && p.Bloco == this._idBloco && p.Apto_numero == this._idApto_Numero 
                                                                                 || p.Id_EnderecoLogradouro == this._idEnderecoLogradouro && p.Numero == this._numero)
                                                                                 .ToList();

                    if (listagemLogradouroElemento.Count > 0)
                    {
                        elementos = listagemLogradouroElemento.Select(p => p.Elemento_Rede).ToList();
                    }
                }
                else
                {
                    Condominio_Predio condominio = null;
                    using (var session = HibernateUtil.GetSession())
                    using (var repoCondominio = new Conexao<Condominio_Predio>(session))
                    {
                        condominio = repoCondominio.selectFirst(p => p.Id == this._idCondominio).Result;
                    }
                    if (condominio != null)
                    {
                        listagemLogradouroElemento = this._repositoryElementoEndereco.GetAll()
                                                                                             .Where(p => p.Todos_Bloco == true ? p.Id_CondominioPredio == condominio.Id :
                                                                                             p.Todos_Apto_Numero == true ? p.Id_CondominioPredio == condominio.Id && p.Bloco == this._idBloco :
                                                                                             p.Id_CondominioPredio == condominio.Id && p.Bloco == this._idBloco && p.Apto_numero == this._idApto_Numero)
                                                                                             .ToList();
                        elementos = listagemLogradouroElemento.Select(p => p.Elemento_Rede).ToList();
                    }
                    else 
                    {
                        var registroCidade = this._repository.GetAll().Where(cid => cid.Id == this._cidade).FirstOrDefault();
                        
                        if (registroCidade != null)
                        {
                                                   
                            var enderecoLogradouro = this._repositoryEnderecoLogradouro.GetAll()
                                                                                       .Where(p => p.Descricao == this._logradouro && p.Cidade == registroCidade)
                                                                                       .FirstOrDefault();

                            if (enderecoLogradouro != null)
                            {
                                listagemLogradouroElemento = this._repositoryElementoEndereco.GetAll()
                                                                                             .Where(p => p.Id_EnderecoLogradouro == enderecoLogradouro.Id && p.Numero == this._numero)
                                                                                             .ToList();
                                elementos = listagemLogradouroElemento.Select(p => p.Elemento_Rede).ToList();
                            }
                        }
                    }
                }

                foreach (var elemento in elementos.Distinct())
                {
                    caixasCadastroEndereco.Add(elemento);
                }

                retorno.Ok = caixasCadastroEndereco.Count > 0;
                retorno.Objeto = caixasCadastroEndereco;
            }
            catch (Exception ex) 
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }
    }
}
