using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aniel.T5.Model.enumerado;

namespace Aniel.Connect.Models.T5
{
    public class Mapa_Gestao_RedeListaModel
    {
        #region Mapa_Gestao_RedeListaModel
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Descricao_Tipo { get; set; }
        public virtual String Descricao_Modelo { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Certificado { get; set; }
        public virtual bool Area_Cobertura { get; set; }
        public virtual bool Caixa { get; set; }
        public virtual bool Cliente { get; set; }
        public virtual bool Lead { get; set; }
        public virtual bool Prospect { get; set; }
        public Status_Cliente Status { get; set; }
        public virtual Double? Latitude_Elemento { get; set; }
        public virtual Double? Longitude_Elemento { get; set; }
        #endregion
    }
    public class Gestao_Rede_Plano
    {
        #region Gestao_Rede_Plano
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Distancia { get; set; }
        public virtual String Portas_Disponiveis { get; set; }
        #endregion
    }
    public class Gestao_Rede_Caixa
    {
        #region Gestao_Rede_Caixa
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Carencia { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Periodo_Teste { get; set; }
        #endregion
    }
    public class Gestao_RedeModel
    {
        public virtual List<Int64?> Ids_Elemento { get; set; }
        public virtual List<Int64?> Ids_Area { get; set; }

        #region Funil
        public virtual String Ref_Id_Elemento { get; set; }
        public virtual String Ref_Tipo_Elemento { get; set; }
        public virtual String Ref_Tecnologia { get; set; }
        public virtual String Ref_Porta_Login { get; set; }
        public virtual String Ref_Plano { get; set; }
        public virtual String Ref_Cliente { get; set; }
        public virtual String Ref_Status_Cliente { get; set; }
        public virtual String Ref_Potencia_ONU { get; set; }
        public virtual String Ref_Fibra { get; set; }
        public virtual String Ref_Caixa_Ftth { get; set; }

        //public virtual String X { get; set; }
        #endregion
        #region comentado
        //public virtual Int64? Id_Area_Cobertura{get;set;}
        //public virtual String Contrato{get;set;}
        //public virtual String Projeto{get;set;}
        ////public virtual String Id_Elemento{get;set;}
        //public virtual Int64? Id_Tipo_Elemento_Rede{get;set;}
        //public virtual Int64? Id_Elemento_Rede{get;set;}
        //public virtual Boolean Sem_Projeto{get;set;} 
        #endregion
    }
    public class Posicao_Elemento
    {
        public virtual double latitude { get; set; }
        public virtual double longitude { get; set; }
    }
    public class Posicao_ElementoString
    {
        public virtual string latitude { get; set; }
        public virtual string longitude { get; set; }
    }
    public class Gestao_ObjRetorno
    {
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Cor { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String CorLinhaAtiva { get; set; }
        public virtual String CorLinhaInativa { get; set; }
        public virtual String Login { get; set; }
        public virtual String Cliente_Plano_Produto { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String ArquivoXML_str { get; set; }
        public virtual String Path { get; set; }
        public virtual String Id_Icone { get; set; }
        public virtual String Id_Elemento { get; set; }
        public virtual String Descricao_Tipo { get; set; }
        //Area_Cobertura
        public virtual String Descricao_Area { get; set; }
        public virtual String Cor_Area { get; set; }
        public virtual String Estilo_Linha_Ativa { get; set; }
        public virtual String Estilo_Linha_Inativa { get; set; }
        public virtual int Espessura { get; set; }
        public virtual long? Id_Area { get; set; }
        public virtual List<List<Double?>> Coordenadas_Area { get; set; }
        public virtual List<Double?> Coordenadas_Elemento { get; set; }
        public virtual Boolean Poste { get; set; }

        #region Connect
        public virtual String a { get; set; }
        #endregion
    }


}