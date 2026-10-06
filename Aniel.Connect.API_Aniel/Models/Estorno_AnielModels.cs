using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    //19- API para disponibilizar uma consulta de todos os estornos realizados no sistema Aniel.
    //Na request dessa API deverá ser enviada o filtro período(obrigatório, não superior a 90 dias),
    //contrato destino(opicional) e projeto destino(opicional). Serão disponibilizadas as informações
    //de Contrato Origem, Projeto Origem, Contrato Destino, Projeto Destino, CNPJ Fornecedor,
    //Tipo de Documento, Número do Documento, Data/Hora/Usuário Estorno, Equipe,
    //Número da Obra, Código de Material, Código Complementar,
    //Cód Complementar Auxiliar, Quantidade Estornada.
    #region MyRegion
    public class EstornoL_AnielListaModel
    {
        public virtual String Contrato_Origem { get; set; }
        public virtual String Projeto_Origem { get; set; }
        public virtual String Contrato_Destino { get; set; }
        public virtual String Projeto_Destino { get; set; }
        public virtual String CNPJ_Fornecedor { get; set; }
        public virtual String Documento { get; set; }
        public virtual String Numero_Documento { get; set; }
        public virtual String Data_Hora_Estorno { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Numero_Obra { get; set; }
        public virtual String Cod_Material { get; set; }
        public virtual String Codigo_Complementar { get; set; }
        public virtual String Codigo_Complementar_Auxiliar { get; set; }
        public virtual String Quantidade_Estorno { get; set; }
    } 
    #endregion
    public class Estorno_AnielApiRequest: ApiRequest 
    {
        public virtual String periodo { get; set; } 
        public virtual String Contrato_Destino { get; set; } 
        public virtual String Projeto_Destino { get; set; } 
    }

}