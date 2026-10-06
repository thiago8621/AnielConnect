using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PerfilVisualizacaoServico;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamArea;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamContratoProjeto;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamMonitor;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSetorDepto;
using Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSupervisor;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.Funcionario;
using Aniel.Connect.Repository.T5.GrupoArea;
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

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class PerfilVisualizacaoServicoBusiness
    {
        private readonly IPerfilVisualizacaoServicoRepository _repository;

        private readonly IPerfilVisualizacaoServicoParamContratoProjetoRepository _repoParamContratoProjeto;
        private readonly IPerfilVisualizacaoServicoParamAreaRepository _repoParamArea;
        private readonly IPerfilVisualizacaoServicoParamSetorDeptoRepository _repoParamSetor;
        private readonly IPerfilVisualizacaoServicoParamSupervisorRepository _repoParamSuperv;
        private readonly IPerfilVisualizacaoServicoParamMonitorRepository _repoParamMonitor;

        private readonly IContratoAnielRepository _repoContrato;
        private readonly IProjetoAnielRepository _repoProjeto;
        
        private readonly IGrupoAreaAnielRepository _repoGrupoArea;
        private readonly IAreaAnielRepository _repoArea;
        private readonly ILocalidadeAnielRepository _repoLocalidade;
        
        private readonly ISetorAnielRepository _repoSetor;

        private readonly IFuncionarioAnielRepository _repoFuncionario;
        private readonly IUsuarioAnielRepository _repoUsuario;

        public PerfilVisualizacaoServicoBusiness(
            IPerfilVisualizacaoServicoRepository repository,
            IPerfilVisualizacaoServicoParamContratoProjetoRepository repoParamContratoProjeto,
            IPerfilVisualizacaoServicoParamAreaRepository repoParamArea,
            IPerfilVisualizacaoServicoParamSetorDeptoRepository repoParamSetor,
            IPerfilVisualizacaoServicoParamSupervisorRepository repoParamSuperv,
            IPerfilVisualizacaoServicoParamMonitorRepository repoParamMonitor,
            IContratoAnielRepository repoContrato,
            IProjetoAnielRepository repoProjeto,
            IGrupoAreaAnielRepository repoGrupoArea,
            IAreaAnielRepository repoArea,
            ILocalidadeAnielRepository repoLocalidade,
            ISetorAnielRepository repoSetor,
            IFuncionarioAnielRepository repoFuncionario,
            IUsuarioAnielRepository repoUsuario)
        {
            this._repository = repository;

            this._repoParamContratoProjeto = repoParamContratoProjeto;
            this._repoParamArea = repoParamArea;
            this._repoParamSetor = repoParamSetor;
            this._repoParamSuperv = repoParamSuperv;
            this._repoParamMonitor = repoParamMonitor;

            this._repoContrato = repoContrato;
            this._repoProjeto = repoProjeto;

            this._repoGrupoArea = repoGrupoArea;
            this._repoArea = repoArea;
            this._repoLocalidade = repoLocalidade;

            this._repoSetor = repoSetor;
            this._repoFuncionario = repoFuncionario;
            this._repoUsuario = repoUsuario;
        }

        private PerfilVisualizacaoServicoCadastroModel GetViewObj(PerfilVisualizacaoServico model)
        {
            var view = new PerfilVisualizacaoServicoCadastroModel();

            view.Id = model.Id;
            view.Descricao = model.Descricao;
            view.Status = model.Status == Status_Default.Ativo;

            var paramsContratoProjeto = 
                this._repoParamContratoProjeto
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoServicoContratoProjeto
                    {
                        Id = p.Id,
                        CodCt = p.CodCt,
                        Projeto = p.Projeto
                    })
                    .ToArray();

            foreach (var paramContratoProjeto in paramsContratoProjeto)
            {
                paramContratoProjeto.NomeContrato = this._repoContrato.GetByCodCt(paramContratoProjeto.CodCt).Descricao;
                paramContratoProjeto.NomeProjeto = !String.IsNullOrEmpty(paramContratoProjeto.Projeto) ? paramContratoProjeto.Projeto : "Todos";
            }

            view.ParamsContratoProjeto = paramsContratoProjeto;

            var paramsArea =
                this._repoParamArea
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoServicoArea
                    {
                        Id = p.Id,

                        CodEscritorio = p.CodEscritorio,
                        CodArea = p.CodArea,
                        CodLocalidade = Convert.ToString(p.CodLocalidade)
                    })
                    .ToArray();

            foreach (var paramArea in paramsArea)
            {
                paramArea.GrupoArea = this._repoGrupoArea.GetByCodigo(paramArea.CodEscritorio).Descricao;
                
                if (!String.IsNullOrEmpty(paramArea.CodArea))
                {
                    paramArea.Area = this._repoArea.GetByCodigo(paramArea.CodEscritorio, paramArea.CodArea).Descricao;
                }
                else
                {
                    paramArea.Area = "Todas";
                }
                
                if (!String.IsNullOrEmpty(paramArea.CodLocalidade))
                {
                    paramArea.Localidade = 
                        this._repoLocalidade
                            .GetByCodigo(
                                paramArea.CodEscritorio, 
                                paramArea.CodArea, 
                                Convert.ToInt32(paramArea.CodLocalidade)).LocalidadeStr;
                }
                else
                {
                    paramArea.Localidade = "Todas";
                }
            }

            view.ParamsArea = paramsArea;

            var paramsSetor =
                this._repoParamSetor
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoServicoSetor
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

            var paramsSupervisor =
                this._repoParamSuperv
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoServicoSupervisor
                    {
                        Id = p.Id,
                        CodSuperv = p.CodSuperv
                    })
                    .ToArray();

            foreach (var paramSuperv in paramsSupervisor)
            {
                paramSuperv.Supervisor = this._repoFuncionario.GetByNumFre(paramSuperv.CodSuperv).Nome;
            }

            view.ParamsSuperv = paramsSupervisor;

            var paramsMonitor =
                this._repoParamMonitor
                    .GetParamsByPerfil(model.Id)
                    .Select(p => new ParamVisualizacaoServicoMonitor
                    {
                        Id = p.Id,
                        UsuarioMonitor = p.UsuarioMonitor
                    })
                    .ToArray();

            foreach (var paramMonitor in paramsMonitor)
            {
                paramMonitor.Monitor = this._repoUsuario.GetByUsuario(paramMonitor.UsuarioMonitor).Nome;
            }

            view.ParamsMonitor = paramsMonitor;

            return view;
        }

        private PerfilVisualizacaoServicoParamContratoProjeto BindParamContratoProjeto(
            PerfilVisualizacaoServico perfil,
            ParamVisualizacaoServicoContratoProjeto view,
            string usuario)
        {
            var model = new PerfilVisualizacaoServicoParamContratoProjeto();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodCt = view.CodCt;
            model.Projeto = !String.IsNullOrEmpty(view.Projeto) ? view.Projeto : null;

            return model;
        }

        private PerfilVisualizacaoServicoParamArea BindParamArea(
            PerfilVisualizacaoServico perfil,
            ParamVisualizacaoServicoArea view,
            string usuario)
        {
            var model = new PerfilVisualizacaoServicoParamArea();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodEscritorio = view.CodEscritorio;
            model.CodArea = !String.IsNullOrEmpty(view.CodArea) ? view.CodArea : null;
            model.CodLocalidade = !String.IsNullOrEmpty(view.CodLocalidade) ? Convert.ToInt32(view.CodLocalidade) : (int?)null;

            return model;
        }

        private PerfilVisualizacaoServicoParamSetorDepto BindParamSetor(
            PerfilVisualizacaoServico perfil,
            ParamVisualizacaoServicoSetor view,
            string usuario)
        {
            var model = new PerfilVisualizacaoServicoParamSetorDepto();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodDepto = Convert.ToInt16(view.CodDepto);

            return model;
        }

        private PerfilVisualizacaoServicoParamSupervisor BindParamSuperv(
            PerfilVisualizacaoServico perfil,
            ParamVisualizacaoServicoSupervisor view,
            string usuario)
        {
            var model = new PerfilVisualizacaoServicoParamSupervisor();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.CodSuperv = view.CodSuperv;

            return model;
        }

        private PerfilVisualizacaoServicoParamMonitor BindParamMonitor(
            PerfilVisualizacaoServico perfil,
            ParamVisualizacaoServicoMonitor view,
            string usuario)
        {
            var model = new PerfilVisualizacaoServicoParamMonitor();

            model.DataHoraCriacao = DateTime.Now;
            model.UsuarioCriacao = usuario;

            model.PerfilVisualizacao = perfil;

            model.UsuarioMonitor = view.UsuarioMonitor;

            return model;
        }

        public PerfilVisualizacaoServico Gravar(PerfilVisualizacaoServicoCadastroModel view, string usuario)
        {
            PerfilVisualizacaoServico model = this._repository.GetById(view.Id);
                
            if (model == null)
            {
                model = new PerfilVisualizacaoServico
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

            if (model.Id == 0)
                this._repository.Save(model);
            else
                this._repository.Update(model);

            if (view.ParamsContratoProjeto.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoServicoContratoProjeto> novosParamsContratoProjeto = 
                    view.ParamsContratoProjeto.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoServicoContratoProjeto paramContratoProjetoView in novosParamsContratoProjeto)
                {
                    PerfilVisualizacaoServicoParamContratoProjeto paramContratoProjeto =
                        this.BindParamContratoProjeto(model, paramContratoProjetoView, usuario);

                    this._repoParamContratoProjeto.Save(paramContratoProjeto);
                }
            }

            if (view.ParamsArea.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoServicoArea> novosParamsArea = 
                    view.ParamsArea.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoServicoArea paramAreaView in novosParamsArea)
                {
                    PerfilVisualizacaoServicoParamArea paramArea =
                        this.BindParamArea(model, paramAreaView, usuario);

                    this._repoParamArea.Save(paramArea);
                }
            }

            if (view.ParamsSetor.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoServicoSetor> novosParamsSetor = 
                    view.ParamsSetor.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoServicoSetor paramSetorView in novosParamsSetor)
                {
                    PerfilVisualizacaoServicoParamSetorDepto paramSetor =
                        this.BindParamSetor(model, paramSetorView, usuario);

                    this._repoParamSetor.Save(paramSetor);
                }
            }

            if (view.ParamsSuperv.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoServicoSupervisor> novosParamsSuperv =
                    view.ParamsSuperv.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoServicoSupervisor paramSupervView in novosParamsSuperv)
                {
                    PerfilVisualizacaoServicoParamSupervisor paramSuperv =
                        this.BindParamSuperv(model, paramSupervView, usuario);

                    this._repoParamSuperv.Save(paramSuperv);
                }
            }

            if (view.ParamsMonitor.Count() > 0)
            {
                IEnumerable<ParamVisualizacaoServicoMonitor> novosParamsMonitor =
                    view.ParamsMonitor.Where(p => p.Id == 0);

                foreach (ParamVisualizacaoServicoMonitor paramMonitorView in novosParamsMonitor)
                {
                    PerfilVisualizacaoServicoParamMonitor paramMonitor =
                        this.BindParamMonitor(model, paramMonitorView, usuario);

                    this._repoParamMonitor.Save(paramMonitor);
                }
            }

            return model;
        }

        public PerfilVisualizacaoServicoCadastroModel FindById(long id)
        {
            PerfilVisualizacaoServico model = this._repository.GetById(id);
            return this.GetViewObj(model);
        }

        public ObjCombo<long> GetCombo(RequestObjCombo<long?> filtros, int paging)
        {
            var objCombo = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            IQueryable<PerfilVisualizacaoServico> queryPerfil = this._repository.GetAll();

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
            PerfilVisualizacaoServico perfil = this._repository.GetById(idPerfil);

            if (perfil == null)
            {
                throw new Exception("Perfil não encontrado no sistema");
            }

            perfil.Status = status;

            this._repository.Update(perfil);
        }

        public void ExcluirParamContratoProjeto(long idParamContratoProjeto)
        {
            PerfilVisualizacaoServicoParamContratoProjeto paramContratoProjeto = 
                this._repoParamContratoProjeto.GetById(idParamContratoProjeto);

            this._repoParamContratoProjeto.Delete(paramContratoProjeto);
        }

        public void ExcluirParamArea(long idParamArea)
        {
            PerfilVisualizacaoServicoParamArea paramArea = 
                this._repoParamArea.GetById(idParamArea);

            this._repoParamArea.Delete(paramArea);
        }

        public void ExcluirParamSetorDepto(long idParamSetorDepto)
        {
            PerfilVisualizacaoServicoParamSetorDepto paramSetorDepto = 
                this._repoParamSetor.GetById(idParamSetorDepto);

            this._repoParamSetor.Delete(paramSetorDepto);
        }

        public void ExcluirParamSupervisor(long idParamSupervisor)
        {
            PerfilVisualizacaoServicoParamSupervisor paramSupervisor = 
                this._repoParamSuperv.GetById(idParamSupervisor);

            this._repoParamSuperv.Delete(paramSupervisor);
        }

        public void ExcluirParamMonitor(long idParamMonitor)
        {
            PerfilVisualizacaoServicoParamMonitor paramMonitor = 
                this._repoParamMonitor.GetById(idParamMonitor);

            this._repoParamMonitor.Delete(paramMonitor);
        }
    }
}
