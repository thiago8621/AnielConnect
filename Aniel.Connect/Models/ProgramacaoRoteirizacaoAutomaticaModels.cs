using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class ProgramacaoRoteirizacaoAutomaticaListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Status { get; set; }
        public virtual String Disparado { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String HoraInicio { get; set; }
        public virtual String Meses { get; set; }
        public virtual String DiasDoMes { get; set; }
        public virtual String DiasDaSemana { get; set; }
        public virtual String Repetir { get; set; }
        public virtual String Expira { get; set; }
    }
    public class ProgramacaoRoteirizacaoAutomaticaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String DataInicio { get; set; }
        public virtual String HoraInicio { get; set; }
        public virtual List<ProgramacaoRoteirizacaoCategoriaClienteViewModel> Categorias { get; set; } = new List<ProgramacaoRoteirizacaoCategoriaClienteViewModel>();
        public virtual List<ProgramacaoRoteirizacaoNaturezaProducaoViewModel> Naturezas { get; set; } = new List<ProgramacaoRoteirizacaoNaturezaProducaoViewModel>();
        public virtual List<ProgramacaoRoteirizacaoSupervisorViewModel> Supervisores { get; set; } = new List<ProgramacaoRoteirizacaoSupervisorViewModel>();
        public virtual List<ProgramacaoRoteirizacaoTecnicoViewModel> Tecnicos { get; set; } = new List<ProgramacaoRoteirizacaoTecnicoViewModel>();
        public virtual List<ProgramacaoRoteirizacaoProjetoViewModel> Projetos { get; set; } = new List<ProgramacaoRoteirizacaoProjetoViewModel>();
        public virtual List<ProgramacaoRoteirizacaoLocalidadeViewModel> Localidades { get; set; } = new List<ProgramacaoRoteirizacaoLocalidadeViewModel>();
        public virtual List<ProgramacaoRoteirizacaoFilialViewModel> Filiais { get; set; } = new List<ProgramacaoRoteirizacaoFilialViewModel>();

        public virtual String[] Equipe { get; set; }
        public virtual String[] Supervisor { get; set; }
        public virtual String[] Id_Categoria_Cliente { get; set; }
        public virtual String[] Id_Natureza_Servico { get; set; }
        public virtual Int16 Cod_Filial { get; set; }
        public virtual String Filial { get; set; }

        

        public virtual String Empresa { get; set; }
        
        public virtual String Nome_Equipe { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }

        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual FormatoDisparoAutomaticoRoteirizacao? FormatoDisparoAutomaticoRoteirizacao { get; set; }
        public virtual Boolean Servico_SemAgendamento { get; set; }
        public virtual Boolean Servico_Agendados { get; set; }
        public virtual Boolean Servico_Paralisados { get; set; }
        public virtual Boolean Servico_Nao_Executados { get; set; }
        public virtual Boolean Servico_Roteirizados { get; set; }
        public virtual String NumUltimosDias { get; set; }
        public virtual String NumPrimeirosDias { get; set; }
        public virtual Boolean TodosDias { get; set; }
        public virtual Boolean NoDia { get; set; }
        public virtual Boolean NaSemana { get; set; }
        public virtual Boolean NoMes { get; set; }
        public virtual Boolean? NosPrimeiros { get; set; }
        public virtual Boolean NosUltimos { get; set; }
        public virtual string NumeroMaximoAtendimento { get; set; }
        public virtual Boolean AlterarDataHoraSemAgendamentoAniel { get; set; }


        public virtual Boolean Segunda { get; set; }
        public virtual Boolean Terca { get; set; }
        public virtual Boolean Quarta { get; set; }
        public virtual Boolean Quinta { get; set; }
        public virtual Boolean Sexta { get; set; }
        public virtual Boolean Sabado { get; set; }
        public virtual Boolean Domingo { get; set; }

        public virtual Boolean Janeiro { get; set; }
        public virtual Boolean Fevereiro { get; set; }
        public virtual Boolean Marco { get; set; }
        public virtual Boolean Abril { get; set; }
        public virtual Boolean Maio { get; set; }
        public virtual Boolean Junho { get; set; }
        public virtual Boolean Julho { get; set; }
        public virtual Boolean Agosto { get; set; }
        public virtual Boolean Setembro { get; set; }
        public virtual Boolean Outubro { get; set; }
        public virtual Boolean Novembro { get; set; }
        public virtual Boolean Dezembro { get; set; }


        public virtual Boolean Dia01 { get; set; }
        public virtual Boolean Dia02 { get; set; }
        public virtual Boolean Dia03 { get; set; }
        public virtual Boolean Dia04 { get; set; }
        public virtual Boolean Dia05 { get; set; }
        public virtual Boolean Dia06 { get; set; }
        public virtual Boolean Dia07 { get; set; }
        public virtual Boolean Dia08 { get; set; }
        public virtual Boolean Dia09 { get; set; }
        public virtual Boolean Dia10 { get; set; }
        public virtual Boolean Dia11 { get; set; }
        public virtual Boolean Dia12 { get; set; }
        public virtual Boolean Dia13 { get; set; }
        public virtual Boolean Dia14 { get; set; }
        public virtual Boolean Dia15 { get; set; }
        public virtual Boolean Dia16 { get; set; }
        public virtual Boolean Dia17 { get; set; }
        public virtual Boolean Dia18 { get; set; }
        public virtual Boolean Dia19 { get; set; }
        public virtual Boolean Dia20 { get; set; }
        public virtual Boolean Dia21 { get; set; }
        public virtual Boolean Dia22 { get; set; }
        public virtual Boolean Dia23 { get; set; }
        public virtual Boolean Dia24 { get; set; }
        public virtual Boolean Dia25 { get; set; }
        public virtual Boolean Dia26 { get; set; }
        public virtual Boolean Dia27 { get; set; }
        public virtual Boolean Dia28 { get; set; }
        public virtual Boolean Dia29 { get; set; }
        public virtual Boolean Dia30 { get; set; }
        public virtual Boolean Dia31 { get; set; }
        public virtual Boolean ProgramacaoExpira { get; set; }
        public virtual String DataExpira { get; set; }


    }

    public class ProgramacaoRoteirizacaoSupervisorViewModel
    {
        public virtual long Id { get; set; }
        public virtual string Numfre { get; set; }
    }

    public class ProgramacaoRoteirizacaoTecnicoViewModel
    {
        public virtual long Id { get; set; }
        public virtual string Equipe { get; set; }
    }

    public class ProgramacaoRoteirizacaoCategoriaClienteViewModel
    {
        public virtual long Id { get; set; }
        public virtual long? IdCategoria { get; set; }
    }

    public class ProgramacaoRoteirizacaoNaturezaProducaoViewModel
    {
        public virtual long Id { get; set; }
        public virtual long? IdNaturezaProducao { get; set; }
    }

    public class ProgramacaoRoteirizacaoProjetoViewModel
    {
        public virtual long Id { get; set; }
        public virtual string CodCt { get; set; }
        public virtual string DescricaoCodCt { get; set; }
        public virtual string Projeto { get; set; }
    }

    public class ProgramacaoRoteirizacaoFilialViewModel
    {
        public virtual long Id { get; set; }
        public virtual short? CodEmpresa { get; set; }
        public virtual short? CodFilial { get; set; }
        public virtual string DescricaoEmpresa { get; set; }
        public virtual string DescricaoFilial { get; set; }
    }

    public class ProgramacaoRoteirizacaoLocalidadeViewModel
    {
        public virtual long Id { get; set; }
        public virtual string Localidade { get; set; }
        public virtual string CodEscritorio { get; set; }
        public virtual string CodArea { get; set; }
        public virtual string DescricaoEscritorio { get; set; }
        public virtual string DescricaoArea { get; set; }
    }

    public class ProgramacaoRoteirizacaoAutomatica_DataTableRequest : DataTableRequest
    {

    }
}