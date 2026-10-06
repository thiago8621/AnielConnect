using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PerfilVisualizacaoVenda;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamCategoriaCliente;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamTipoPessoa;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamEquipeVenda;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamSetorDepto;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamMeioCaptacao;
using Aniel.Connect.Repository.PerfilVisualizacaoVendaParamRegiao;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.Funcionario;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.T5.Projeto;
using Aniel.Connect.Repository.T5.Setor;
using Aniel.Connect.Repository.T5.Usuario;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.EquipeVendas;
using Sinapse.DAO.repository;
using Aniel.Connect.Model.nhibernate;
using NHibernate;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class PerfilVisualizacaoVendaBusiness
    {
        private readonly IPerfilVisualizacaoVendaRepository _repository;

        private readonly IPerfilVisualizacaoVendaParamTipoPessoaRepository _repoParamTipoPessoa;
        private readonly IPerfilVisualizacaoVendaParamCategoriaClienteRepository _repoParamCategoriaCliente;
        private readonly IPerfilVisualizacaoVendaParamSetorDeptoRepository _repoParamSetor;
        private readonly IPerfilVisualizacaoVendaParamMeioCaptacaoRepository _repoParamMeioCaptacao;
        private readonly IPerfilVisualizacaoVendaParamEquipeVendaRepository _repoParamEquipeVenda;
        private readonly IPerfilVisualizacaoVendaParamRegiaoRepository _repoParamRegiao;
        
        private readonly ISetorAnielRepository _repoSetor;
        private readonly IMeioCaptacaoRepository _repoMeioCaptacao;
        private readonly IEquipeVendasRepository _repoEquipeVendas;

        private readonly ICategoriaClienteRepository _repoCategoriaCliente;
        private readonly ICidadeRepository _repoCidade;

        public PerfilVisualizacaoVendaBusiness(
            IPerfilVisualizacaoVendaRepository repository,
            IPerfilVisualizacaoVendaParamTipoPessoaRepository repoParamTipoPessoa,
            IPerfilVisualizacaoVendaParamCategoriaClienteRepository repoParamCategoriaCliente,
            IPerfilVisualizacaoVendaParamSetorDeptoRepository repoParamSetor,
            IPerfilVisualizacaoVendaParamMeioCaptacaoRepository repoParamMeioCaptacao,
            IPerfilVisualizacaoVendaParamEquipeVendaRepository repoParamEquipeVenda,
            IPerfilVisualizacaoVendaParamRegiaoRepository repoParamRegiao,
            ISetorAnielRepository repoSetor,
            IMeioCaptacaoRepository repoMeioCaptacao,
            IEquipeVendasRepository repoEquipeVendas,
            ICategoriaClienteRepository repoCategoriaCliente,
            ICidadeRepository repoCidade
            )
        {
            this._repository = repository;

            this._repoParamTipoPessoa = repoParamTipoPessoa;
            this._repoParamCategoriaCliente = repoParamCategoriaCliente;
            this._repoParamSetor = repoParamSetor;
            this._repoParamMeioCaptacao = repoParamMeioCaptacao;
            this._repoParamEquipeVenda = repoParamEquipeVenda;
            this._repoParamRegiao = repoParamRegiao;

            this._repoSetor = repoSetor;
            this._repoMeioCaptacao = repoMeioCaptacao;
            this._repoEquipeVendas = repoEquipeVendas;
            this._repoCategoriaCliente = repoCategoriaCliente;
            this._repoCidade = repoCidade;
        }

        private PerfilVisualizacaoVendaCadastroModel GetViewObj(PerfilVisualizacaoVenda model)
        {
            var view = new PerfilVisualizacaoVendaCadastroModel();

            view.Id = model.Id;
            view.Descricao = model.Descricao;
            view.Status = model.Status == Status_Default.Ativo;
            view.PerfilSolicitaAutorizacao = model.PerfilSolicitaAutorizacao;
            view.PadraoVisualizacaoLead = model.PadraoVisualizaLead;
            view.PadraoVisualizacaoVenda = model.PadraoVisualizaVenda;
            view.Qtd_Dias_Bloqueio_Lead = model.Qtd_Dias_Bloqueio_Lead;

            ///**** TIPO PESSOA ///****
            var paramsTipoPessoa = 
                this._repoParamTipoPessoa
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaTipoPessoa
                    {
                        Id = p.Id,
                        CodTipoPessoa = Convert.ToInt32(p.CodTipoPessoa)
                    })
                    .ToArray();

            foreach (var paramPessoa in paramsTipoPessoa)
            {
                paramPessoa.TipoPessoa = HtmlEnumExtensions.GetEnumDescription((Tipo_Pessoa)paramPessoa.CodTipoPessoa);
            }
            view.ParamsTipoPessoa = paramsTipoPessoa;


            ///**** CATEGORIA CLIENTE ///****
            var paramsCategoriaCliente =
                this._repoParamCategoriaCliente
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaCategoriaCliente
                    {
                        Id = p.Id,
                        CodCategoriaCliente = Convert.ToString(p.CodCategoriaCliente),
                    })
                    .ToArray();

            foreach (var paramCatCliente in paramsCategoriaCliente)
            {
                paramCatCliente.CategoriaCliente = this._repoCategoriaCliente.GetById(Convert.ToInt16(paramCatCliente.CodCategoriaCliente)).Descricao;
            }
            view.ParamsCategoriaCliente = paramsCategoriaCliente;


            ///**** SETOR ///****
            var paramsSetor =
                this._repoParamSetor
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaSetor
                    {
                        Id = p.Id,
                        CodDepto = Convert.ToString(p.CodDepto)
                    })
                    .ToArray();

            foreach (var paramSetor in paramsSetor)
            {
                paramSetor.Setor = this._repoSetor.GetByCodigo(Convert.ToInt16(paramSetor.CodDepto)).Descricao;
            }
            view.ParamsSetor = paramsSetor;


            ///**** MEIO CAPTAÇÃO ///****
            var paramsMeioCaptacao =
                this._repoParamMeioCaptacao
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaMeioCaptacao
                    {
                        Id = p.Id,
                        CodMeioCaptacao = p.CodMeioCaptacao
                    })
                    .ToArray();

            foreach (var paramMeioCaptacao in paramsMeioCaptacao)
            {
                paramMeioCaptacao.MeioCaptacao = this._repoMeioCaptacao.GetById(Convert.ToInt16(paramMeioCaptacao.CodMeioCaptacao)).Descricao;
            }
            view.ParamsMeioCaptacao = paramsMeioCaptacao;


            ///**** EQUIPE VENDA ///****
            var paramsEquipeVenda =
                this._repoParamEquipeVenda
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaEquipeVenda
                    {
                        Id = p.Id,
                        CodEquipe = Convert.ToString(p.CodEquipeVenda)
                    })
                    .ToArray();

            foreach (var paramEquipeVenda in paramsEquipeVenda)
            {
                paramEquipeVenda.Equipe = this._repoEquipeVendas.GetById(paramEquipeVenda.CodEquipe).Descricao;
            }

            view.ParamsEquipeVenda = paramsEquipeVenda;


            ///**** REGIÃO ///****
            var paramsRegiao =
                this._repoParamRegiao
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoVendaRegiao
                    {
                        Id = p.Id,
                        CodCidade = Convert.ToString(p.CodCidade),
                        CodUF = Convert.ToString(p.CodUF)
                    })
                    .ToArray();

            foreach (var paramRegiao in paramsRegiao)
            {
                paramRegiao.UF = this._repoCidade.GetById(paramRegiao.CodCidade, paramRegiao.CodUF).UF;
                paramRegiao.Cidade = this._repoCidade.GetById(paramRegiao.CodCidade, paramRegiao.CodUF).Descricao;
            }

            view.ParamsRegiao = paramsRegiao;

            return view;
        }

        private PerfilVisualizacaoVendaParamTipoPessoa BindParamTipoPessoa(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaTipoPessoa view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamTipoPessoa();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodTipoPessoa = (Tipo_Pessoa)Convert.ToInt32(view.CodTipoPessoa);

            return model;
        }

        private PerfilVisualizacaoVendaParamCategoriaCliente BindParamCategoriaCliente(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaCategoriaCliente view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamCategoriaCliente();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodCategoriaCliente = !String.IsNullOrEmpty(view.CodCategoriaCliente) ? view.CodCategoriaCliente : null;

            return model;
        }

        private PerfilVisualizacaoVendaParamSetorDepto BindParamSetor(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaSetor view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamSetorDepto();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodDepto = Convert.ToInt16(view.CodDepto);

            return model;
        }

        private PerfilVisualizacaoVendaParamMeioCaptacao BindParamMeioCaptacao(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaMeioCaptacao view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamMeioCaptacao();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodMeioCaptacao = !String.IsNullOrEmpty(view.CodMeioCaptacao) ? view.CodMeioCaptacao : null;

            return model;
        }

        private PerfilVisualizacaoVendaParamEquipeVenda BindParamEquipeVenda(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaEquipeVenda view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamEquipeVenda();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodEquipeVenda = Convert.ToInt16(view.CodEquipe);

            return model;
        }
        
        private PerfilVisualizacaoVendaParamRegiao BindParamRegiao(
            PerfilVisualizacaoVenda perfil,
            ParamVisualizacaoVendaRegiao view,
            string usuario)
        {
            var model = new PerfilVisualizacaoVendaParamRegiao();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodUF = Convert.ToInt32(view.CodUF);
            model.CodCidade = Convert.ToInt32(view.CodCidade);

            return model;
        }

        public PerfilVisualizacaoVenda Gravar(PerfilVisualizacaoVendaCadastroModel view, string usuario)
        {
            PerfilVisualizacaoVenda model = this._repository.GetById(view.Id);
                
            if (model == null)
            {
                model = new PerfilVisualizacaoVenda
                {
                    DataHoraCriacao = DateTime.Now,
                    UsuarioCriacao = usuario,

                    Status = Status_Default.Ativo
                };
            }
            else
            {
                model.DataHoraAlteracao = DateTime.Now;
                model.UsuarioAlteracao = usuario;

                model.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }

            model.Descricao = view.Descricao;
            model.PerfilSolicitaAutorizacao = view.PerfilSolicitaAutorizacao ? true : false;
            model.PadraoVisualizaLead = view.PadraoVisualizacaoLead;
            model.PadraoVisualizaVenda = view.PadraoVisualizacaoVenda;

            if(model.PadraoVisualizaLead == false)
            {
                model.Qtd_Dias_Bloqueio_Lead = view.Qtd_Dias_Bloqueio_Lead;
            }

            if (model.Id == 0)
                this._repository.Save(model);
            else
                this._repository.Update(model);

            if (view.ParamsTipoPessoa.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaTipoPessoa> novosParamsTipoPessoa = 
                    view.ParamsTipoPessoa.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaTipoPessoa paramTipoPessoaView in novosParamsTipoPessoa)
                {
                    PerfilVisualizacaoVendaParamTipoPessoa paramTipoPessoa =
                        this.BindParamTipoPessoa(model, paramTipoPessoaView, usuario);

                    this._repoParamTipoPessoa.Save(paramTipoPessoa);
                }
            }

            if (view.ParamsCategoriaCliente.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaCategoriaCliente> novosParamsCategoriaCliente = 
                    view.ParamsCategoriaCliente.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaCategoriaCliente paramCategoriaClienteView in novosParamsCategoriaCliente)
                {
                    PerfilVisualizacaoVendaParamCategoriaCliente paramCategoriaCliente =
                        this.BindParamCategoriaCliente(model, paramCategoriaClienteView, usuario);

                    this._repoParamCategoriaCliente.Save(paramCategoriaCliente);
                }
            }

            if (view.ParamsSetor.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaSetor> novosParamsSetor = 
                    view.ParamsSetor.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaSetor paramSetorView in novosParamsSetor)
                {
                    PerfilVisualizacaoVendaParamSetorDepto paramSetor =
                        this.BindParamSetor(model, paramSetorView, usuario);

                    this._repoParamSetor.Save(paramSetor);
                }
            }

            if (view.ParamsMeioCaptacao.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaMeioCaptacao> novosParamsMeioCaptacao =
                    view.ParamsMeioCaptacao.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaMeioCaptacao paramMeioCaptacaoView in novosParamsMeioCaptacao)
                {
                    PerfilVisualizacaoVendaParamMeioCaptacao paramMeioCaptacao =
                        this.BindParamMeioCaptacao(model, paramMeioCaptacaoView, usuario);

                    this._repoParamMeioCaptacao.Save(paramMeioCaptacao);
                }
            }

            if (view.ParamsEquipeVenda.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaEquipeVenda> novosParamsEquipeVenda =
                    view.ParamsEquipeVenda.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaEquipeVenda paramEquipeVendaView in novosParamsEquipeVenda)
                {
                    PerfilVisualizacaoVendaParamEquipeVenda paramEquipeVenda =
                        this.BindParamEquipeVenda(model, paramEquipeVendaView, usuario);

                    this._repoParamEquipeVenda.Save(paramEquipeVenda);
                }
            }

            if (view.ParamsRegiao.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoVendaRegiao> novosParamsRegiao =
                    view.ParamsRegiao.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoVendaRegiao paramRegiaoView in novosParamsRegiao)
                {
                    PerfilVisualizacaoVendaParamRegiao paramRegiao =
                        this.BindParamRegiao(model, paramRegiaoView, usuario);

                    this._repoParamRegiao.Save(paramRegiao);
                }
            }

            return model;
        }

        public PerfilVisualizacaoVendaCadastroModel FindById(long id)
        {
            PerfilVisualizacaoVenda model = this._repository.GetById(id);
            return this.GetViewObj(model);
        }

        public ObjCombo<long> GetCombo(RequestObjCombo<long?> filtros, int paging)
        {
            var objCombo = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            IQueryable<PerfilVisualizacaoVenda> queryPerfil = this._repository.GetAll();

            if (filtros.id.HasValue)
            {
                queryPerfil = queryPerfil.Where(p => p.Id == filtros.id.Value);
            }

            if (!String.IsNullOrEmpty(filtros.q))
            {
                string truncatedSearch = StringService.Truncate(filtros.q.ToUpperInvariant(), 20);
                queryPerfil = queryPerfil.Where(p => p.Descricao.ToUpperInvariant().Contains(truncatedSearch));
            }

            int skip = filtros.page > 0 ? paging * (filtros.page - 1) : 0;

            objCombo.total_count = queryPerfil.Count();
            objCombo.results.AddRange(
                queryPerfil.Select(p => new ObjComboResult<long>
                {
                    Id = p.Id,
                    Text = p.Descricao
                })
                .Skip(skip)
                .Take(paging)
                .ToArray()
            );

            return objCombo;
        }

        public void AlterarStatus(long idPerfil, Status_Default status)
        {
            PerfilVisualizacaoVenda perfil = this._repository.GetById(idPerfil);

            if (perfil == null)
            {
                throw new Exception("Perfil não encontrado no sistema");
            }

            perfil.Status = status;

            this._repository.Update(perfil);
        }

        public void ExcluirParamTipoPessoa(long idParamTipoPessoa)
        {
            PerfilVisualizacaoVendaParamTipoPessoa paramTipoPessoa = 
                this._repoParamTipoPessoa.GetById(idParamTipoPessoa);

            this._repoParamTipoPessoa.Delete(paramTipoPessoa);
        }

        public void ExcluirParamCategoriaCliente(long idParamCategoriaCliente)
        {
            PerfilVisualizacaoVendaParamCategoriaCliente paramCategoriaCliente = 
                this._repoParamCategoriaCliente.GetById(idParamCategoriaCliente);

            this._repoParamCategoriaCliente.Delete(paramCategoriaCliente);
        }

        public void ExcluirParamSetorDepto(long idParamSetorDepto)
        {
            PerfilVisualizacaoVendaParamSetorDepto paramSetorDepto = 
                this._repoParamSetor.GetById(idParamSetorDepto);

            this._repoParamSetor.Delete(paramSetorDepto);
        }

        public void ExcluirParamMeioCaptacao(long idParamMeioCaptacao)
        {
            PerfilVisualizacaoVendaParamMeioCaptacao paramMeioCaptacao = 
                this._repoParamMeioCaptacao.GetById(idParamMeioCaptacao);

            this._repoParamMeioCaptacao.Delete(paramMeioCaptacao);
        }

        public void ExcluirParamEquipeVenda(long idParamEquipeVenda)
        {
            PerfilVisualizacaoVendaParamEquipeVenda paramEquipeVenda = 
                this._repoParamEquipeVenda.GetById(idParamEquipeVenda);

            this._repoParamEquipeVenda.Delete(paramEquipeVenda);
        }
        
        public void ExcluirParamRegiao(long idParamRegiao)
        {
            PerfilVisualizacaoVendaParamRegiao paramRegiao = 
                this._repoParamRegiao.GetById(idParamRegiao);

            this._repoParamRegiao.Delete(paramRegiao);
        }
    }
}
