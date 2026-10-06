using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class EquipeAniel : BaseModel
    {
        private IEnumerable<Equipe_LocalidadeAniel> _localidades;
        private IEnumerable<Equipe_NaturezaAniel> _naturezas;

        public virtual String Matricula { get; set; }
        public virtual String EquipeAux { get; set; }
        public virtual String Nome { get; set; }
        public virtual DateTime? Encerramento { get; set; }
        public virtual Ponto_Partida Ponto_Partida { get; set; }
        public virtual String Device_Id { get; set; }

        /// <summary>
        ///     Objeto somente leitura para utilizar com o NHibernate
        /// </summary>
        public virtual FuncionarioAniel Supervisor { get; set; }
        public virtual String CodSuperv { get; set; }

        public virtual String Num_Fre { get; set; }
        public virtual String CodCtIntegrator { get; set; }
        public virtual String ProjetoIntegrator { get; set; }
        public virtual String Cod_Fornecedor { get; set; }


        /// <summary>
        /// S ou N
        /// </summary>
        public virtual String Connect { get; set; }

        public virtual String Monitor { get; set; }
        public virtual PerfilAgendamento PerfilAgendamento { get; set; }

        public virtual IEnumerable<Equipe_NaturezaAniel> Naturezas
        {
            get
            {
                if (_naturezas == null)
                    _naturezas = new List<Equipe_NaturezaAniel>();
                return _naturezas;
            }
            set
            {
                _naturezas = value;
            }
        }

        public virtual IEnumerable<Equipe_LocalidadeAniel> Localidades
        {
            get
            {
                if (_localidades == null)
                    _localidades = new List<Equipe_LocalidadeAniel>();
                return _localidades;
            }
            set
            {
                _localidades = value;
            }
        }

        public virtual List<LogVincularPerfilAniel> GeraLog(EquipeAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogVincularPerfilAniel> logs = new List<LogVincularPerfilAniel>();

            switch(pTipoOperLog)
            {
                case TipoOperLog.Alteracao:
               
                    logs.Add(new LogVincularPerfilAniel
                    {
                        Id = 0,
                        TipoOperacao = TipoOperLog.Alteracao,
                        UsuarioCriacao = pUsuario,
                        NomeUsuarioCriacao = pNomeUsuario,
                        Campo_ValorAntes = pOld.PerfilAgendamento.Descricao,
                        Campo_ValorDepois = this.PerfilAgendamento.Descricao,
                        DataHoraCriacao = DateTime.Now,
                        Tecnico = this.Nome
                    });
                    break;
                case TipoOperLog.Exclusao:
                    logs.Add(new LogVincularPerfilAniel
                    {
                        Id = 0,
                        TipoOperacao = TipoOperLog.Exclusao,
                        UsuarioCriacao = pUsuario,
                        NomeUsuarioCriacao = pNomeUsuario,
                        Campo_ValorAntes = pOld.PerfilAgendamento.Descricao,
                        Campo_ValorDepois = null,
                        DataHoraCriacao = DateTime.Now,
                        Tecnico = this.Nome
                    });
                    break;
                case TipoOperLog.Insercao:
                    logs.Add(new LogVincularPerfilAniel
                    {
                        Id = 0,
                        TipoOperacao = TipoOperLog.Insercao,
                        UsuarioCriacao = pUsuario,
                        NomeUsuarioCriacao = pNomeUsuario,
                        Campo_ValorAntes = null,
                        Campo_ValorDepois = this.PerfilAgendamento.Descricao,
                        DataHoraCriacao = DateTime.Now,
                        Tecnico = this.Nome
                    });
                    break;
            }
   
            return logs;

        }

        public virtual List<LogVincularMonitorTecnico> GeraLogVincularMonitorTecnico(EquipeAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogVincularMonitorTecnico> logs = new List<LogVincularMonitorTecnico>();


            switch (pTipoOperLog)
            {                
                case TipoOperLog.Exclusao:
                    logs.Add(new LogVincularMonitorTecnico
                    {
                        Id = 0,
                        TipoOperacao = TipoOperLog.Exclusao,
                        UsuarioCriacao = pUsuario,
                        NomeUsuarioCriacao = pUsuario,
                        Campo_ValorAntes = pOld.Monitor,
                        Campo_ValorDepois = null,
                        DataHoraCriacao = DateTime.Now,
                        Tecnico = this.Nome,
                        Monitor = this.Monitor
                    });
                    break;
                case TipoOperLog.Insercao:
                    logs.Add(new LogVincularMonitorTecnico
                    {
                        Id = 0,
                        TipoOperacao = TipoOperLog.Insercao,
                        UsuarioCriacao = pUsuario,
                        NomeUsuarioCriacao = pNomeUsuario,
                        Campo_ValorAntes = null,
                        Campo_ValorDepois = this.Monitor,
                        DataHoraCriacao = DateTime.Now,
                        Tecnico = this.Nome,
                        Monitor = this.Monitor
                    });
                    break;
            }

            return logs;

        }
    }
}
