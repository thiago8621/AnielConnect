using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Repository.CondominioPredio;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRede;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class CondominioPredioBusiness
    {
        private ICondominioPredioRepository _repository;
        private IElementoRedeAnielRepository _repositoryElement;
        public CondominioPredioBusiness(ICondominioPredioRepository repository, IElementoRedeAnielRepository repositoryElement)
        {
            this._repository = repository;
            this._repositoryElement = repositoryElement;
        }
        public Condominio_PredioCadastroModel GetViewObj(long id)
        {
            var obj = this._repository.GetById(id);
            var lista = this._repositoryElement.GetAllEnderecos().Where(p => p.Id_CondominioPredio == id).ToList().Select(p => new Condominio_PredioCaixaListaModel
            {
                Id_Condominio_Predio = p.Id_CondominioPredio.Value,
                Id = p.Id,
                Id_Caixa = p.Elemento_Rede.Id,
                Descricao = p.Elemento_Rede.Descricao,
            }).ToList();
            var apartamentosinCondominio = new List<Condominio_PredioApartamentoListaModel>();
            var blocos = this._repository.GetBlocosByCondominio(obj.Id).ToList();
            foreach (var bloco in blocos)
            {
                var apartamentos = this._repository.GetApartamentosByBloco(bloco.Id).ToList();
                foreach (var ap in apartamentos)
                {
                    apartamentosinCondominio.Add(new Condominio_PredioApartamentoListaModel
                    {
                        Id = ap.Id,
                        Descricao = ap.Descricao,
                        Status = ap.Status,
                        Id_Bloco = bloco.Id,
                        Bloco = bloco.Descricao,
                    });
                }
            }
            return new Condominio_PredioCadastroModel
            {
                Id = obj.Id,
                Definir_Caixa_Condominio_Predio = obj.Definir_Caixa_Condominio_Predio,
                Cep = obj.Cep,

                Numero = obj.Numero,
                Descricao = obj.Descricao,
                Endereco_Logradouro = obj?.Endereco_Logradouro?.Id,
                Id_Pais = obj.Endereco_Logradouro != null ? obj?.Endereco_Logradouro?.Pais?.Id : obj.Id_Pais,
                Id_Cidade = obj.Endereco_Logradouro != null ? obj?.Endereco_Logradouro?.Cidade.Id : obj.Id_Cidade,
                Uf = obj.Endereco_Logradouro != null ? obj?.Endereco_Logradouro?.UF?.Sigla : obj.Uf,
                Bairro = obj.Endereco_Logradouro != null ? obj?.Endereco_Logradouro?.Bairro.Descricao : obj.Bairro,
                Logradouro = obj.Endereco_Logradouro != null ? obj?.Endereco_Logradouro?.Descricao : obj.Logradouro,
                Status = obj.Status == Status_Default.Ativo,
                Caixas = lista,
                Apartamentos = apartamentosinCondominio.OrderBy(p => p.Bloco).ToList(),
                Cadastro_Faixa = obj.Cadastro_Faixa,
            };
        }
        public async Task<PostApiRetorno<Int64>> Gravar(ParamWebApi<Condominio_PredioCadastroModel> value, string userId)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Condominio_Predio modelo = null, modelo_Anterior = null;                
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {                                                        
                    if (value.Objeto.Id != 0)
                    {
                        modelo = this._repository.GetById(value.Objeto.Id);
                        modelo_Anterior = (Condominio_Predio)modelo.Clone();
                    }
                    if (modelo == null)
                    {
                        modelo = new Condominio_Predio
                        {
                            Status = Status_Default.Ativo,
                            UsuarioCriacao = value.Usuario
                        };
                    }
                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                    }

                    BindViewToObj(ref modelo, value.Objeto);
                    
                    this._repository.save(modelo);
                    this._repositoryElement.saveEnderecos(modelo.Caixas.ToList());                    
                    IEnumerable<IGrouping<(string bloco, long Id), Condominio_PredioApartamentoListaModel>> blocos = value.Objeto.Apartamentos
                                                                                                                .Where(p => p.Id == 0)
                                                                                                                .GroupBy(p => (p.Bloco, p.Id_Bloco)).ToList();
                    foreach (var block in blocos)
                    {
                        var apartamentosLista = new List<Condominio_PredioApartamento>();
                        var bloco = this._repository.GetAllBlocos().Where(p => p.Condominio.Id == modelo.Id && p.Descricao == block.Key.bloco).FirstOrDefault();
                        if (bloco == null)
                        {
                            bloco = new Condominio_PredioBloco
                            {
                                Condominio = modelo,
                                Status = Status_Default.Ativo,
                                Descricao = block.Key.bloco,
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = userId,

                            };
                        }
                        this._repository.saveBlocos(bloco);
                        foreach (var ap in block)
                        {
                            var apartamento = new Condominio_PredioApartamento
                            {
                                Bloco = bloco,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = ap.Descricao,
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = userId,
                            };
                            apartamentosLista.Add(apartamento);
                        }
                        this._repository.saveApartamentos(apartamentosLista);
                    }
                    

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }
        public void BindViewToObj(ref Condominio_Predio objBanco, Condominio_PredioCadastroModel view)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repoEnderecoLougradouro = new Conexao<Endereco_Logradouro>(session))
            {
                objBanco.Cep = view.Cep;
                objBanco.Definir_Caixa_Condominio_Predio = view.Definir_Caixa_Condominio_Predio;
                objBanco.Numero = view.Numero;
                objBanco.Id_Pais = view.Id_Pais;
                objBanco.Id_Cidade = view.Id_Cidade;

                objBanco.Uf = view.Uf;
                objBanco.Bairro = view.Bairro;
                objBanco.Logradouro = view.Logradouro;
                objBanco.Descricao = view.Descricao;
                objBanco.Endereco_Logradouro = view.Endereco_Logradouro != null ? repoEnderecoLougradouro.selectFirst(p => p.Id == view.Endereco_Logradouro).Result : null;
                if (objBanco.Id != 0)
                {
                    objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                }
            }
            List<Elemento_Rede_Enderecos> CondominioPredioCaixastemp = objBanco.Caixas.ToList();
            foreach (Condominio_PredioCaixaListaModel caixas_condominio in view.Caixas.FindAll(p => p.Id_Caixa == 0))
            {
                CondominioPredioCaixastemp.Add(new Elemento_Rede_Enderecos
                {
                    Elemento_Rede = this._repositoryElement.GetById(caixas_condominio.Id_Caixa),
                    Id_CondominioPredio = view.Id,
                });
            }
            objBanco.Caixas = CondominioPredioCaixastemp;
        }
    }
}
