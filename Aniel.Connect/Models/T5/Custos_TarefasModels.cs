using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class Prod_LancadaListaModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual Int32? Status_Auditoria { get; set; }

        public virtual String Unidade_Custo { get; set; }

        public virtual String Tarefa { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String NomeEquipe { get; set; }

        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Cod_Sub_Tipo_Serv { get; set; }

        public virtual String Tipo_Producao { get; set; }

        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }

        public virtual String Cod_Preco { get; set; }

        public virtual String Data_Prod { get; set; }       

        public virtual String Valor { get; set; }
        public virtual String Quant_Prod { get; set; }
        public virtual String Quant_Catal { get; set; }

        public virtual String Usuario { get; set; }
        public virtual String Dia { get; set; }
        public virtual String Hora { get; set; }
    }

    /*p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc &&
                                                               p.Tarefa == value.Objeto.Tarefa && p.Unidade_Custo == value.Objeto.Unidade_Custo && p.Codigo == value.Objeto.Codigo && p.Tipo_Producao == value.Objeto.Tipo_Producao &&
                                                               p.Equipe == value.Objeto.Equipe && p.Cod_Tipo_Serv == value.Objeto.Cod_Tipo_Serv && p.Cod_Sub_Tipo_Serv == value.Objeto.Cod_Sub_Tipo_Serv*/

    public class Prod_LancadaChave
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Unidade_Custo { get; set; }
        public virtual String Tarefa { get; set; }
        public virtual String Equipe { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Cod_Sub_Tipo_Serv { get; set; }
        public virtual String Tipo_Producao { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Data_Prod { get; set; }
    }
}