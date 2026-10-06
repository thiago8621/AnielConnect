using Aniel.Connect.ModelLog.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.T5.Model.nhibernate
{
    public class PerfilAgendamento : BaseModel
    {
        public virtual String Descricao { get; set; }

        public virtual Status_Default Status { get; set; }

        private IList<PerfilAgendamentoNatureza> _naturezas;
        public virtual IList<PerfilAgendamentoNatureza> Naturezas
        {
            get
            {
                if (_naturezas == null)
                    _naturezas = new List<PerfilAgendamentoNatureza>();
                return _naturezas;
            }
            set
            {
                _naturezas = value;
            }
        }

        private IList<PerfilAgendamentoArea> _areas;
        public virtual IList<PerfilAgendamentoArea> Areas
        {
            get
            {
                if (_areas == null)
                    _areas = new List<PerfilAgendamentoArea>();
                return _areas;
            }
            set
            {
                _areas = value;
            }
        }


        public virtual List<LogPerfilAgendamentoAniel> GeraLog(PerfilAgendamento pold, string pusuario, string pnomeusuario, TipoOperLog pTipoOperLog,ISession sessionT5)
        {
            using (var repositoryEscritorio = new Conexao<EscritorioAniel>(sessionT5))
            using (var repositoryArea = new Conexao<AreaAniel>(sessionT5))
            using (var repositoryLocalidade = new Conexao<LocalidadeAniel>(sessionT5))

            {
                try { 
                List<LogPerfilAgendamentoAniel> logs = new List<LogPerfilAgendamentoAniel>();

                if (pTipoOperLog != TipoOperLog.Alteracao)              
                {
                    Log_InsereTipoOper<PerfilAgendamento, LogPerfilAgendamentoAniel>(logs, pusuario, pnomeusuario, pTipoOperLog, this.Id.ToString());
              
                }
                else
                {
                    Log_ComparaProperty<PerfilAgendamento, LogPerfilAgendamentoAniel>(logs, pusuario, pnomeusuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO, pold, this, p => p.Descricao);
                    Log_ComparaProperty<PerfilAgendamento, LogPerfilAgendamentoAniel>(logs, pusuario, pnomeusuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pold, this, p => p.Status);

                    
                    foreach (var item in pold.Naturezas.Select(p => p.Natureza))
                    {
                        if (this.Naturezas.Select(p => p.Natureza?.Id).Contains(item?.Id) == false)
                        {
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                IdLog = this.Id,
                                Campo = RsCadastros.NATUREZA,
                                TipoOperacao = TipoOperLog.Exclusao,
                                Chave = this.Id.ToString(),
                                UsuarioCriacao = pusuario,
                                Natureza = item.Descricao,
                                NomeUsuarioCriacao = pnomeusuario,
                                Campo_ValorAntes = item.Descricao,
                                Campo_ValorDepois = item.Descricao,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = this.Descricao,

                            });
                        }


                    }

                    foreach (var item in this.Naturezas.Select(p => p.Natureza))
                    {
                        if (pold.Naturezas.Select(p => p.Natureza?.Id).Contains(item?.Id) == false)
                        {
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                IdLog = this.Id,
                                Campo = RsCadastros.NATUREZA,
                                TipoOperacao = TipoOperLog.Insercao,
                                Chave = this.Id.ToString(),
                                UsuarioCriacao = pusuario,
                                Natureza = item.Descricao,
                                NomeUsuarioCriacao = pnomeusuario,
                                Campo_ValorAntes = item.Descricao,
                                Campo_ValorDepois = item.Descricao,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = this.Descricao,

                            });
                        }

                    }


                    foreach (var item in pold.Areas)
                    {
                        if (this.Areas.Select(p => (p.Escritorio,p.Area,p.Localidade)).Contains((item.Escritorio,item.Area,item.Localidade)) == false)
                        {
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                IdLog = this.Id,
                                Campo = RsCadastros.GRUPO_AREA,
                                TipoOperacao = TipoOperLog.Exclusao,
                                Chave = this.Id.ToString(),
                                GrupoArea = repositoryEscritorio.selectFirst(y => y.Cod_Escritorio == item.Escritorio).Result?.Descricao,
                                Area = repositoryArea.selectFirst(y => y.Cod_Area == item.Area && y.Cod_Escritorio == item.Escritorio).Result?.Descricao,
                                Localidade = item.Localidade,
                                UsuarioCriacao = pusuario,
                                NomeUsuarioCriacao = pnomeusuario,
                                Campo_ValorAntes = item.Escritorio,
                                Campo_ValorDepois = item.Escritorio,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = this.Descricao,

                            });
                        }


                    }


                    foreach (var item in this.Areas)
                    {
                        if (pold.Areas.Select(p => (p.Escritorio, p.Area, p.Localidade)).Contains((item.Escritorio, item.Area, item.Localidade)) == false)
                        {
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                IdLog = this.Id,
                                Campo = RsCadastros.GRUPO_AREA,
                                TipoOperacao = TipoOperLog.Insercao,
                                Chave = this.Id.ToString(),
                                GrupoArea = repositoryEscritorio.selectFirst(y => y.Cod_Escritorio == item.Escritorio).Result?.Descricao,
                                Area = repositoryArea.selectFirst(y => y.Cod_Area == item.Area && y.Cod_Escritorio == item.Escritorio).Result?.Descricao,
                                Localidade = item.Localidade,
                                UsuarioCriacao = pusuario,
                                NomeUsuarioCriacao = pnomeusuario,
                                Campo_ValorAntes = item.Escritorio,
                                Campo_ValorDepois = item.Escritorio,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = this.Descricao,

                            });
                        }

                    }

                }

                return logs;


                }
                catch (Exception ex)
                {            
                    throw ex;
                }
            }
            
        }
    }
}



