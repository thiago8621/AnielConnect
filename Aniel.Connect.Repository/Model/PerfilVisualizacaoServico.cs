using Sinapse.Model.enumerados;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class PerfilVisualizacaoServicoListModel
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
    }

    public class ParamVisualizacaoServicoAlterarStatusRequest
    {
        public IEnumerable<long> Ids { get; set; }
        public Status_Default Status { get; set; }
    }

    public class ParamVisualizacaoServicoContratoProjeto
    {
        public long Id { get; set; }

        public string CodCt { get; set; }
        public string Projeto { get; set; }

        public string NomeContrato { get; set; }
        public string NomeProjeto { get; set; }
    }

    public class ParamVisualizacaoServicoArea
    {
        public long Id { get; set; }

        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public string CodLocalidade { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }
    }

    public class ParamVisualizacaoServicoSetor
    {
        public long Id { get; set; }

        public string CodDepto { get; set; }
        public string Setor { get; set; }
    }

    public class ParamVisualizacaoServicoSupervisor
    {
        public long Id { get; set; }

        public string CodSuperv { get; set; }
        public string Supervisor { get; set; }
    }

    public class ParamVisualizacaoServicoMonitor
    {
        public long Id { get; set; }

        public string UsuarioMonitor { get; set; }
        public string Monitor { get; set; }
    }

    public class PerfilVisualizacaoServicoCadastroModel
    {
        private IEnumerable<ParamVisualizacaoServicoContratoProjeto> _paramsContratoProjeto;
        private IEnumerable<ParamVisualizacaoServicoArea> _paramsArea;
        private IEnumerable<ParamVisualizacaoServicoSetor> _paramsSetor;
        private IEnumerable<ParamVisualizacaoServicoSupervisor> _paramsSuperv;
        private IEnumerable<ParamVisualizacaoServicoMonitor> _paramsMonitor;

        public long Id { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }

        public IEnumerable<ParamVisualizacaoServicoContratoProjeto> ParamsContratoProjeto
        {
            get
            {
                if (this._paramsContratoProjeto == null)
                    this._paramsContratoProjeto = new List<ParamVisualizacaoServicoContratoProjeto>();

                return this._paramsContratoProjeto;
            }
            set
            {
                this._paramsContratoProjeto = value;
            }
        }

        public IEnumerable<ParamVisualizacaoServicoArea> ParamsArea
        {
            get
            {
                if (this._paramsArea == null)
                    this._paramsArea = new List<ParamVisualizacaoServicoArea>();

                return this._paramsArea;
            }
            set
            {
                this._paramsArea = value;
            }
        }

        public IEnumerable<ParamVisualizacaoServicoSetor> ParamsSetor
        {
            get
            {
                if (this._paramsSetor == null)
                    this._paramsSetor = new List<ParamVisualizacaoServicoSetor>();

                return this._paramsSetor;
            }
            set
            {
                this._paramsSetor = value;
            }
        }

        public IEnumerable<ParamVisualizacaoServicoSupervisor> ParamsSuperv
        {
            get
            {
                if (this._paramsSuperv == null)
                    this._paramsSuperv = new List<ParamVisualizacaoServicoSupervisor>();

                return this._paramsSuperv;
            }
            set
            {
                this._paramsSuperv = value;
            }
        }

        public IEnumerable<ParamVisualizacaoServicoMonitor> ParamsMonitor
        {
            get
            {
                if (this._paramsMonitor == null)
                    this._paramsMonitor = new List<ParamVisualizacaoServicoMonitor>();

                return this._paramsMonitor;
            }
            set
            {
                this._paramsMonitor = value;
            }
        }
    }
}
