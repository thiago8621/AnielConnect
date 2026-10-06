using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Chart.Mvc.ComplexChart;
using Chart.Mvc.SimpleChart;
using Grafana.Models;
using Grafana.Services.Impl;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Aniel.Connect.Models
{
    public class Pesquisa_SatisfacaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }
        public virtual String Nome { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Email { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Obs { get; set; }

        public virtual String Usuario_Criacao { get; set; }

        public virtual String Titulo_Da_Pesquisa { get; set; }
        public virtual String Quem_Indicou { get; set; }
        public virtual String DisparoAuto { get; set; }
        public virtual String Tipo_Disparo { get; set; }
        public virtual String Status { get; set; }
        public virtual String Quem_Preenche { get; set; }
        public virtual String Data_Hora_Criacao { get; set; }
        public virtual String Data_Hora_Respondida { get; set; }
        public virtual String Qtde_Envios { get; set; }
        public virtual String Qtde_Respostas { get; set; }
        public virtual String Percentual { get; set; }
    }

    public class OpiniaoListaModel
    {
        public virtual Int64 Id_Cliente { get; set; }
        public virtual String Data_Resposta { get; set; }
        public virtual String Categoria { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Comentario { get; set; }
        public virtual String Comentario_Truncate { get; set; }
        public virtual String Nome_Arquivo_Foto_Perfil { get; set; }
    }


        public class Pesquisa_SatisfacaoCadastroModel :  IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? IdAtendimento { get; set; }

        [Display(Name = "TITULO_DA_PESQUISA", ResourceType = typeof(Resources.RsCadastros))]
        //[MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
        //        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Titulo_Da_Pesquisa { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Descricao_Pesquisa { get; set; }

        public virtual Boolean Status { get; set; }
        public virtual Boolean SomentePesquisa { get; set; }
        public virtual Boolean SomentePesquisaSem{ get; set; }

        [Display(Name = "DISPARO_AUTOMATICO_", ResourceType = typeof(Resources.RsCadastros))]
        public virtual PesquisaSatisfacaoDisparoAuto DisparoAuto { get; set; }

        [Display(Name = "QUEM_PREENCHE_", ResourceType = typeof(Resources.RsCadastros))]
        public virtual PesquisaSatisfacaoQuemPreenche QuemPreenche { get; set; }

        public virtual String Nome { get; set; }
        public virtual Status_PesquisaSatisfacao StatusPesquisa { get; set; }

        private List<Pesquisa_PerguntasItemModel> _perguntas;
        public virtual List<Pesquisa_PerguntasItemModel> Perguntas
        {
            get
            {
                if (_perguntas == null)
                    _perguntas = new List<Pesquisa_PerguntasItemModel>();
                return _perguntas;
            }
            set
            {
                _perguntas = value;
            }
        }
        

        #region Temporarios de Itens
        public virtual String Descricao { get; set; }

        public virtual Boolean Obrigatorio { get; set; }

        public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

        public virtual String Escala_Linear_Ini { get; set; }

        public virtual String Escala_Linear_Fin { get; set; }
        public virtual Padrao? Escala_Emoticos { get; set; }

        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }

        public virtual String Respostas_Possiveis { get; set; }

        public virtual Pesquisa_Tipo_Resposta Tipo_Resposta { get; set; }

        public virtual Boolean Tem_Caixa_Texto { get; set; }
        public virtual Boolean Envia_Msg_Emai_Sms { get; set; }
        public virtual Configuracao_Envio_Mensagens? Assunto { get; set; }
        public virtual Configuracao_Envio_Mensagens? Corpo { get; set; }
        public virtual String Modelo_Envio { get; set; }
        public virtual String Desc_Modelo_Envio { get; set; }

        public virtual PesquisaSatisfacaoTipoCaixaTexto? TipoCaixaTexto { get; set; }

        public virtual Boolean PesquisaJaEnviada { get; set; }

        public virtual String URLLogo { get; set; }
        public virtual String AssuntoPesquisa { get; set; }
        public virtual String CorpoPesquisa { get; set; }

        public virtual String NomeEmpresa { get; set; }
        #endregion

    }

    public class Pesquisa_PerguntasItemModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Descricao { get; set; }

        public virtual Boolean Obrigatorio { get; set; }

        public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

        public virtual String Escala_Linear_Ini { get; set; }

        public virtual String Escala_Linear_Fin { get; set; }

        public virtual Int32 Ordem { get; set; }

        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }

        public virtual String Respostas_Possiveis { get; set; }
        public virtual Padrao? Escala_Emoticos { get; set; }

        //Utilizada no Envio da Pesquisa para Preencher as Respostas Multiplas
        public virtual List<Boolean> Respostas { get; set; }

        //Utilizada no Envio da Pesquisa para Preencher as Respostas Unicas
        public virtual String Resposta { get; set; }
        public virtual string RespostaTexto { get; set; }
        public virtual String RespostaTextoEmoticon { get; set; }
        public virtual Pesquisa_Tipo_Resposta Tipo_Resposta { get; set; }
    }

    public class PesquisaSatisfacaoFiltroModel {


        public virtual Status_PesquisaSatisfacao? StatusPesquisa { get; set; }
        public virtual PesquisaSatisfacaoQuemPreenche? QuemPreenche { get; set; }
        public virtual PesquisaSatisfacaoDisparoAuto? DisparoAuto { get; set; }
        public virtual Boolean SomentePesquisa { get; set; }
         public virtual Boolean SomentePesquisaSem{ get; set; }
    }
    
    
   
    public class IndicadoresModel
    {
        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }

        public virtual String Titulo_Pesquisa { get; set; }
        public virtual String Url { get; set; }
        public virtual String Url_Local { get; set; }
        public virtual String KeyPainel { get; set; }
        public virtual String VariavelIdpesquisa { get; set; }
        public virtual String VariavelDescricao { get; set; }
        public virtual String VariavelGrafico1 { get; set; }
        public virtual String VariavelGrafico2 { get; set; }
        public virtual String VariavelGrafico3 { get; set; }
        public virtual String PainelGrafana1 { get; set; }
        public virtual String PainelGrafana2 { get; set; }
        public virtual String PainelGrafana3 { get; set; }
         public virtual String Pergunta { get; set; }

        public virtual String Descricao_Pesquisa { get; set; }

        public virtual Boolean Tem_Comentario { get; set; }

        public virtual List<Data_Pergunta> Perguntas { get; set; }

        // Objeto com o número de Envios e Respostas da pesquisa
        public virtual Qtde_Envios_Respostas_Complex_Data Qtde_Envios_Respostas { get; set; }
    }

    public class Pergunta_Resposta
    {
        public virtual Int64 Id_pergunta { get; set; }
        public virtual String Resposta { get; set; }
    }

    public class Value_Label
    {
        public virtual String Label { get; set; }
        public virtual Double Value { get; set; }
    }

    public class Data_Pergunta
    {
        public virtual Int64 Id_Pergunta { get; set; }
        public virtual String Descricao_Pergunta { get; set; }
        public virtual String Tipo_Pergunta { get; set; }

        public virtual List<Value_Label> Obj_Lista_Valores { get; set; }
        public virtual List<SimpleData> Obj_Grafico { get; set; }
    }
        public class Garfana
        {
            public virtual Int64 Id_Pergunta { get; set; }
            public virtual String Descricao_Pergunta { get; set; }
            public virtual String Tipo_Pergunta { get; set; }           
            public virtual String Resposta { get; set; }
            public virtual List<Value_Label> Obj_Lista_Valores { get; set; }
            public virtual List<SimpleData> Obj_Grafico { get; set; }


    }      
    

    public class Qtde_Envios_Respostas_Complex_Data
    {
        public virtual List<String> Labels { get; set; }
        public virtual List<ComplexDataset> Datasets { get; set; }
    }

    public class Pesquisa_Satisfacao_Filtro_DataTableRequest : DataTableRequest {

    public virtual Status_PesquisaSatisfacao StatusPesquisa { get; set; }
    public virtual PesquisaSatisfacaoQuemPreenche QuemPreenche { get; set; }
    public virtual PesquisaSatisfacaoDisparoAuto DisparoAuto { get; set; }
    public virtual Boolean SomentePesquisa { get; set; }
    public virtual Boolean SomentePesquisaSem { get; set; }
}
    public class Pesquisa_Satisfacao_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Cliente { get; set; }

    public virtual String StatusPesquisa { get; set; }
    public virtual String QuemPreenche { get; set; }
    public virtual String DisparoAuto { get; set; }
    public virtual Boolean SomentePesquisa { get; set; }
    public virtual Boolean SomentePesquisaSem { get; set; }
}

    public class Opinioes_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }
        public virtual PesquisaSatisfacaoCategoriaCaixaTexto? Categoria_Opiniao { get; set; }
    }

    public class Pesquisa_SatisfacaoConsulta_DataTableRequest : DataTableRequest
    {
        public virtual String Id_Pesquisa_Satisfacao { get; set; }
        public virtual String dataIni { get; set; }
        public virtual String DataCriacao { get; set; }   
        public virtual String dataFin { get; set; }
 
        public virtual String status_Pesquisa { get; set; }
    }

    public class Pesquisa_Satisfacao_FormularioModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Cliente { get; set; }

        public virtual PesquisaSatisfacaoCategoriaCaixaTexto Categoria { get; set; }

        public virtual String Comentario { get; set; }

        public virtual Pesquisa_SatisfacaoCadastroModel Pesquisa { get; set; }
        public virtual string Tipo_Resposta { get; set; }
        

        public virtual Boolean JaRespondido { get; set; }
    }

    public class VisualizarRespostasModel
    {
        public virtual Int64 Id_Pesquisa_Satisfacao { get; set; }
        public virtual String Titulo_Pesquisa { get; set; }

        public virtual String Comentario_Cliente { get; set; }
        public virtual PesquisaSatisfacaoCategoriaCaixaTexto Categoria_Comentario { get; set; }
       

        public virtual List<Pesquisa_PerguntasItemModel> Perguntas { get; set; }
    }

    public class Pesquisa_Satisfacao_FormularioConcluidoModel
    {
        public virtual String Titulo_Da_Pesquisa { get; set; }
        public virtual String URLLogo { get; set; }
        public virtual String NomeEmpresa { get; set; }
    }

    public class Pesquisa_Satisfacao_PesquisaEmMassaModel
    {
        public List<Int64> ids { get; set; }
        public List<Int64> ids_pesquisa { get; set; }

        public String usuario { get; set; }
        public long idAtendimento { get; set; }
    }

    public class Preenchimento_Atendedor_Model
    {
        public virtual Int64 Id_Pesquisa { get; set; }
        public virtual Int64 Id_Cliente { get; set; }
    }

    public class Pesquisa_Satisfacao_Atendedor
    {
        public virtual String Caminho_Pesquisa { get; set; }
    }

    public class HistoricoEnvio_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
    }
    public class GrafanaRequestModel
    {
        public CreateMessageResponse servicoAnotacoes { get; set; }
        public DashboardResponse DashboardAnotacoes { get; set; }
        
    }

    public class GarfanaRequestModel
    {
        public virtual Int64 Id_Pergunta { get; set; }
        public virtual String Tipo_Resposta { get; set; }
        public virtual String Tipo_Pergunta { get; set; }
        public virtual Int64 Total { get; set; }
        public virtual Int64 Id_Resposta { get; set; }
        


    }
      public class GarfanaEnviadaRequestModel
    {      
        public virtual String Respondida { get; set; }
        public virtual String MesAno { get; set; }
        public virtual String Enviada { get; set; }
        


    }


    public class Garfana_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Pergunta { get; set; }
        public virtual String Tipo_Resposta { get; set; }
        public virtual String Tipo_Pergunta { get; set; }
        public virtual String Id_Resposta { get; set; }

    }
}
