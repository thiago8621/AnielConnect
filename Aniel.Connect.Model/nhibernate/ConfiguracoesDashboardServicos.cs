using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class ConfiguracoesDashboardServicos
    {
        public virtual long Id { get; set; }
        public virtual double? MetaBacklogReparo { get; set; }
        public virtual double? MetaBancoInstalacao { get; set; }
        public virtual double? MetaEficienciaMesInstalacao { get; set; }
        public virtual double? MetaEficienciaMesReparo { get; set; }
        public virtual double? MetaEficienciaDiaTotal { get; set; }
        public virtual double? MetaEficienciaDiaInstalacao { get; set; }
        public virtual double? MetaEficienciaDiaReparo { get; set; }
        public virtual double? MetaEficienciaDiaOutros { get; set; }
        public virtual double? MetaReclamacaoInicial { get; set; }
        public virtual double? MetaRepetidas { get; set; }
        public virtual double? MetaAtendimentoJanelaInstalacao { get; set; }
        public virtual double? MetaAtendimentoJanelaReparo { get; set; }
        public virtual double? MetaAtendimentoJanelaMultiskill { get; set; }
        public virtual double? MetaCustoMedioInstalacao { get; set; }
        public virtual double? MetaCustoMedioReparo { get; set; }

    }

    public class ConfiguracoesDashboardServicosModelo2
    {
        public virtual long Id { get; set; }
        public virtual double? MetaBacklogReparo { get; set; }
        public virtual double? MetaBancoInstalacao { get; set; }
        public virtual double? MetaEficienciaMesInstalacao { get; set; }
        public virtual double? MetaEficienciaMesReparo { get; set; }
        public virtual double? MetaEficienciaDiaTotal { get; set; }
        public virtual double? MetaEficienciaDiaInstalacao { get; set; }
        public virtual double? MetaEficienciaDiaReparo { get; set; }
        public virtual double? MetaEficienciaDiaOutros { get; set; }
        public virtual double? MetaReclamacaoInicial { get; set; }
        public virtual double? MetaRepetidas { get; set; }
        public virtual double? MetaAtendimentoJanelaInstalacao { get; set; }
        public virtual double? MetaAtendimentoJanelaReparo { get; set; }
        public virtual double? MetaAtendimentoJanelaMultiskill { get; set; }
        public virtual double? MetaCustoMedioInstalacao { get; set; }
        public virtual double? MetaCustoMedioReparo { get; set; }
        public virtual List<String> Periodo { get; set;}

    }
}
