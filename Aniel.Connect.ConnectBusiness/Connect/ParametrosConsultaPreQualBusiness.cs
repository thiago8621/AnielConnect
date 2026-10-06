using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamCategoriaCliente;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamTipoPessoa;
//using Aniel.Connect.Repository.ParametrosConsultaPreQualParamEquipeVenda;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamSetorDepto;
//using Aniel.Connect.Repository.ParametrosConsultaPreQualParamMeioCaptacao;
using Aniel.Connect.Repository.ParametrosConsultaPreQualParamRegiao;
using Aniel.Connect.Repository.T5.Setor;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Repository.Bairro;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ParametrosConsultaPreQualBusiness
    {
        private readonly IParametrosConsultaPreQualRepository _repository;

        private readonly IParametrosConsultaPreQualParamTipoPessoaRepository _repoParamTipoPessoa;
        private readonly IParametrosConsultaPreQualParamCategoriaClienteRepository _repoParamCategoriaCliente;
        private readonly IParametrosConsultaPreQualParamSetorDeptoRepository _repoParamSetor;
        private readonly IParametrosConsultaPreQualParamRegiaoRepository _repoParamRegiao;

        private readonly ISetorAnielRepository _repoSetor;

        private readonly ICategoriaClienteRepository _repoCategoriaCliente;
        private readonly ICidadeRepository _repoCidade;
        private readonly IBairroRepository _repoBairro;

        public ParametrosConsultaPreQualBusiness(
            IParametrosConsultaPreQualRepository repository,
            IParametrosConsultaPreQualParamTipoPessoaRepository repoParamTipoPessoa,
            IParametrosConsultaPreQualParamCategoriaClienteRepository repoParamCategoriaCliente,
            IParametrosConsultaPreQualParamSetorDeptoRepository repoParamSetor,
            IParametrosConsultaPreQualParamRegiaoRepository repoParamRegiao,
            ISetorAnielRepository repoSetor,
            ICategoriaClienteRepository repoCategoriaCliente,
            ICidadeRepository repoCidade,
            IBairroRepository repoBairro
            )
        {
            this._repository = repository;

            this._repoParamTipoPessoa = repoParamTipoPessoa;
            this._repoParamCategoriaCliente = repoParamCategoriaCliente;
            this._repoParamSetor = repoParamSetor;
            this._repoParamRegiao = repoParamRegiao;

            this._repoSetor = repoSetor;
            this._repoCategoriaCliente = repoCategoriaCliente;
            this._repoCidade = repoCidade;
            this._repoBairro = repoBairro;
        }

        private ParametrosConsultaPreQualCadastroModel GetViewObj(ParametrosConsultaPreQual model)
        {
            var view = new ParametrosConsultaPreQualCadastroModel();

            view.Id = model.Id;
            view.Descricao = model.Descricao;
            view.Status = model.Status == Status_Default.Ativo;
            view.Padrao = model.Padrao;

            ///**** TIPO PESSOA ///****
            var paramsTipoPessoa =
                this._repoParamTipoPessoa
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamConsultaPreQualTipoPessoa
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
                    .Select(p => new ParamConsultaPreQualCategoriaCliente
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
                    .Select(p => new ParamConsultaPreQualSetor
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


            /////**** MEIO CAPTAÇÃO ///****
            //var paramsMeioCaptacao =
            //    this._repoParamMeioCaptacao
            //        .GetParamsByPerfil(model.Id)
            //        .Select(p => new ParamConsultaPreQualMeioCaptacao
            //        {
            //            Id = p.Id,
            //            CodMeioCaptacao = p.CodMeioCaptacao
            //        })
            //        .ToArray();

            //foreach (var paramMeioCaptacao in paramsMeioCaptacao)
            //{
            //    paramMeioCaptacao.MeioCaptacao = this._repoMeioCaptacao.GetById(Convert.ToInt16(paramMeioCaptacao.CodMeioCaptacao)).Descricao;
            //}
            //view.ParamsMeioCaptacao = paramsMeioCaptacao;


            /////**** EQUIPE VENDA ///****
            //var paramsEquipeVenda =
            //    this._repoParamEquipeVenda
            //        .GetParamsByPerfil(model.Id)
            //        .Select(p => new ParamConsultaPreQualEquipeVenda
            //        {
            //            Id = p.Id,
            //            CodEquipe = Convert.ToString(p.CodEquipeVenda)
            //        })
            //        .ToArray();

            //foreach (var paramEquipeVenda in paramsEquipeVenda)
            //{
            //    paramEquipeVenda.Equipe = this._repoEquipeVendas.GetById(paramEquipeVenda.CodEquipe).Descricao;
            //}

            //view.ParamsEquipeVenda = paramsEquipeVenda;


            ///**** REGIÃO ///****
            var paramsRegiao =
                this._repoParamRegiao
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamConsultaPreQualRegiao
                    {
                        Id = p.Id,
                        CodCidade = Convert.ToString(p.CodCidade),
                        CodUF = Convert.ToString(p.CodUF),
                        CodBairro = Convert.ToString(p.CodBairro),
                    })
                    .ToArray();

            foreach (var paramRegiao in paramsRegiao)
            {
                paramRegiao.UF = paramRegiao.CodUF != null ? this._repoCidade.GetById(paramRegiao.CodCidade, paramRegiao.CodUF).UF : null;
                paramRegiao.Cidade = paramRegiao.CodCidade != null ? this._repoCidade.GetById(paramRegiao.CodCidade, paramRegiao.CodUF).Descricao : null;
                paramRegiao.Bairro = paramRegiao.CodBairro != null ? this._repoBairro.GetById(paramRegiao.CodBairro, paramRegiao.CodCidade, paramRegiao.CodUF).Descricao : null;
            }

            view.ParamsRegiao = paramsRegiao;

            return view;
        }

        private ParametrosConsultaPreQualParamTipoPessoa BindParamTipoPessoa(
            ParametrosConsultaPreQual perfil,
            ParamConsultaPreQualTipoPessoa view,
            string usuario)
        {
            var model = new ParametrosConsultaPreQualParamTipoPessoa();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.ParametrosConsultaPreQual = perfil;

            model.CodTipoPessoa = (Tipo_Pessoa)Convert.ToInt32(view.CodTipoPessoa);

            return model;
        }

        private ParametrosConsultaPreQualParamCategoriaCliente BindParamCategoriaCliente(
            ParametrosConsultaPreQual perfil,
            ParamConsultaPreQualCategoriaCliente view,
            string usuario)
        {
            var model = new ParametrosConsultaPreQualParamCategoriaCliente();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.ParametrosConsultaPreQual = perfil;

            model.CodCategoriaCliente = !String.IsNullOrEmpty(view.CodCategoriaCliente) ? view.CodCategoriaCliente : null;

            return model;
        }

        private ParametrosConsultaPreQualParamSetorDepto BindParamSetor(
            ParametrosConsultaPreQual perfil,
            ParamConsultaPreQualSetor view,
            string usuario)
        {
            var model = new ParametrosConsultaPreQualParamSetorDepto();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.ParametrosConsultaPreQual = perfil;

            model.CodDepto = Convert.ToInt16(view.CodDepto);

            return model;
        }

        //private ParametrosConsultaPreQualParamMeioCaptacao BindParamMeioCaptacao(
        //    ParametrosConsultaPreQual perfil,
        //    ParamConsultaPreQualMeioCaptacao view,
        //    string usuario)
        //{
        //    var model = new ParametrosConsultaPreQualParamMeioCaptacao();

        //    model.DataHoraCriacao = DateTime.Now;
        //    model.UsuarioCriacao = usuario;

        //    model.PerfilVisualizacao = perfil;

        //    model.CodMeioCaptacao = !String.IsNullOrEmpty(view.CodMeioCaptacao) ? view.CodMeioCaptacao : null;

        //    return model;
        //}

        //private ParametrosConsultaPreQualParamEquipeVenda BindParamEquipeVenda(
        //    ParametrosConsultaPreQual perfil,
        //    ParamConsultaPreQualEquipeVenda view,
        //    string usuario)
        //{
        //    var model = new ParametrosConsultaPreQualParamEquipeVenda();

        //    model.DataHoraCriacao = DateTime.Now;
        //    model.UsuarioCriacao = usuario;

        //    model.PerfilVisualizacao = perfil;

        //    model.CodEquipeVenda = Convert.ToInt16(view.CodEquipe);

        //    return model;
        //}

        private ParametrosConsultaPreQualParamRegiao BindParamRegiao(
            ParametrosConsultaPreQual perfil,
            ParamConsultaPreQualRegiao view,
            string usuario)
        {
            var model = new ParametrosConsultaPreQualParamRegiao();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.ParametrosConsultaPreQual = perfil;

            model.CodUF = Convert.ToInt32(view.CodUF);
            model.CodCidade = Convert.ToInt32(view.CodCidade);
            model.CodBairro = Convert.ToInt32(view.CodBairro);

            return model;
        }

        public ParametrosConsultaPreQual Gravar(ParametrosConsultaPreQualCadastroModel view, string usuario)
        {
            ParametrosConsultaPreQual model = this._repository.GetById(view.Id);

            if (model == null)
            {
                model = new ParametrosConsultaPreQual
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
            model.Padrao = view.Padrao;

            if (model.Id == 0)
                this._repository.Save(model);
            else
                this._repository.Update(model);

            if (view.ParamsTipoPessoa.Count() > 0)
            {
                IEnumerable<ParamConsultaPreQualTipoPessoa> novosParamsTipoPessoa =
                    view.ParamsTipoPessoa.Where(p => p.Id == 0);

                foreach (ParamConsultaPreQualTipoPessoa paramTipoPessoaView in novosParamsTipoPessoa)
                {
                    ParametrosConsultaPreQualParamTipoPessoa paramTipoPessoa =
                        this.BindParamTipoPessoa(model, paramTipoPessoaView, usuario);

                    this._repoParamTipoPessoa.Save(paramTipoPessoa);
                }
            }

            if (view.ParamsCategoriaCliente.Count() > 0)
            {
                IEnumerable<ParamConsultaPreQualCategoriaCliente> novosParamsCategoriaCliente =
                    view.ParamsCategoriaCliente.Where(p => p.Id == 0);

                foreach (ParamConsultaPreQualCategoriaCliente paramCategoriaClienteView in novosParamsCategoriaCliente)
                {
                    ParametrosConsultaPreQualParamCategoriaCliente paramCategoriaCliente =
                        this.BindParamCategoriaCliente(model, paramCategoriaClienteView, usuario);

                    this._repoParamCategoriaCliente.Save(paramCategoriaCliente);
                }
            }

            if (view.ParamsSetor.Count() > 0)
            {
                IEnumerable<ParamConsultaPreQualSetor> novosParamsSetor =
                    view.ParamsSetor.Where(p => p.Id == 0);

                foreach (ParamConsultaPreQualSetor paramSetorView in novosParamsSetor)
                {
                    ParametrosConsultaPreQualParamSetorDepto paramSetor =
                        this.BindParamSetor(model, paramSetorView, usuario);

                    this._repoParamSetor.Save(paramSetor);
                }
            }

            //if (view.ParamsMeioCaptacao.Count() > 0)
            //{
            //    IEnumerable<ParamConsultaPreQualMeioCaptacao> novosParamsMeioCaptacao =
            //        view.ParamsMeioCaptacao.Where(p => p.Id == 0);

            //    foreach (ParamConsultaPreQualMeioCaptacao paramMeioCaptacaoView in novosParamsMeioCaptacao)
            //    {
            //        ParametrosConsultaPreQualParamMeioCaptacao paramMeioCaptacao =
            //            this.BindParamMeioCaptacao(model, paramMeioCaptacaoView, usuario);

            //        this._repoParamMeioCaptacao.Save(paramMeioCaptacao);
            //    }
            //}

            //if (view.ParamsEquipeVenda.Count() > 0)
            //{
            //    IEnumerable<ParamConsultaPreQualEquipeVenda> novosParamsEquipeVenda =
            //        view.ParamsEquipeVenda.Where(p => p.Id == 0);

            //    foreach (ParamConsultaPreQualEquipeVenda paramEquipeVendaView in novosParamsEquipeVenda)
            //    {
            //        ParametrosConsultaPreQualParamEquipeVenda paramEquipeVenda =
            //            this.BindParamEquipeVenda(model, paramEquipeVendaView, usuario);

            //        this._repoParamEquipeVenda.Save(paramEquipeVenda);
            //    }
            //}

            if (view.ParamsRegiao.Count() > 0)
            {
                IEnumerable<ParamConsultaPreQualRegiao> novosParamsRegiao =
                    view.ParamsRegiao.Where(p => p.Id == 0);

                foreach (ParamConsultaPreQualRegiao paramRegiaoView in novosParamsRegiao)
                {
                    ParametrosConsultaPreQualParamRegiao paramRegiao =
                        this.BindParamRegiao(model, paramRegiaoView, usuario);

                    this._repoParamRegiao.Save(paramRegiao);
                }
            }

            return model;
        }

        public ParametrosConsultaPreQualCadastroModel FindById(long id)
        {
            ParametrosConsultaPreQual model = this._repository.GetById(id);
            return this.GetViewObj(model);
        }

        public ObjCombo<long> GetCombo(RequestObjCombo<long?> filtros, int paging)
        {
            var objCombo = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            IQueryable<ParametrosConsultaPreQual> queryPerfil = this._repository.GetAll();

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
            ParametrosConsultaPreQual perfil = this._repository.GetById(idPerfil);

            if (perfil == null)
            {
                throw new Exception("Perfil não encontrado no sistema");
            }

            perfil.Status = status;

            this._repository.Update(perfil);
        }

        public void ExcluirParamTipoPessoa(long idParamTipoPessoa)
        {
            ParametrosConsultaPreQualParamTipoPessoa paramTipoPessoa =
                this._repoParamTipoPessoa.GetById(idParamTipoPessoa);

            this._repoParamTipoPessoa.Delete(paramTipoPessoa);
        }

        public void ExcluirParamCategoriaCliente(long idParamCategoriaCliente)
        {
            ParametrosConsultaPreQualParamCategoriaCliente paramCategoriaCliente =
                this._repoParamCategoriaCliente.GetById(idParamCategoriaCliente);

            this._repoParamCategoriaCliente.Delete(paramCategoriaCliente);
        }

        public void ExcluirParamSetorDepto(long idParamSetorDepto)
        {
            ParametrosConsultaPreQualParamSetorDepto paramSetorDepto =
                this._repoParamSetor.GetById(idParamSetorDepto);

            this._repoParamSetor.Delete(paramSetorDepto);
        }

        //public void ExcluirParamMeioCaptacao(long idParamMeioCaptacao)
        //{
        //    ParametrosConsultaPreQualParamMeioCaptacao paramMeioCaptacao =
        //        this._repoParamMeioCaptacao.GetById(idParamMeioCaptacao);

        //    this._repoParamMeioCaptacao.Delete(paramMeioCaptacao);
        //}

        //public void ExcluirParamEquipeVenda(long idParamEquipeVenda)
        //{
        //    ParametrosConsultaPreQualParamEquipeVenda paramEquipeVenda =
        //        this._repoParamEquipeVenda.GetById(idParamEquipeVenda);

        //    this._repoParamEquipeVenda.Delete(paramEquipeVenda);
        //}

        public void ExcluirParamRegiao(long idParamRegiao)
        {
            ParametrosConsultaPreQualParamRegiao paramRegiao =
                this._repoParamRegiao.GetById(idParamRegiao);

            this._repoParamRegiao.Delete(paramRegiao);
        }
    }
}
