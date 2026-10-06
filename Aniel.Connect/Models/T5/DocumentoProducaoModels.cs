using Aniel.Connect.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models.T5
{
    public class StatusDocumentoMongoModel
    {
        public virtual int? statusExecucao { get; set; }
        public virtual String equipe { get; set; }
        public virtual String numFre { get; set; }
        public virtual String nome { get; set; }
        public virtual Double latitude { get; set; }
        public virtual Double longitude { get; set; }
        public virtual DateTime data { get; set; }
        public virtual DateTime hora { get; set; }
        public virtual DateTime? dataHoraServidor { get; set; }
        public virtual String codCt { get; set; }
        public virtual String projeto { get; set; }
        public virtual int contrato { get; set; }
        public virtual String numObra { get; set; }
        public virtual String justificativaPausaMongo { get; set; }
        public virtual String indexStatus { get; set; }

        public virtual String justificativaAprova { get; set; }
        public virtual String justificativaReprova { get; set; }

    }

    public class DocumentoProducaoModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Numero_OS { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int64 Num_Doc { get; set; }

        //Codigo do cliente gerado no cadastro do Connect
        public virtual String Codigo_Cliente { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual String Endereco_Cliente { get; set; }
        public virtual String Endereco_Logradouro { get; set; }
        public virtual String Endereco_CEP { get; set; }
        public virtual String Endereco_PontoReferencia { get; set; }
        public virtual String Endereco_Latitude { get; set; }
        public virtual String Endereco_Longitude { get; set; }
        public virtual String Endereco_Numero { get; set; }
        public virtual String Endereco_Cidade { get; set; }
        public virtual String Endereco_Bairro { get; set; }
        public virtual String Endereco_UF { get; set; }
        public virtual String Endereco_Pais { get; set; }
        public virtual String Endereco_Complemento { get; set; }

        public virtual String Telefone_Principal_Cliente { get; set; }
        public virtual String Celular_Cliente { get; set; }


        //Aniel
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String SubTipo_Servico { get; set; }

        //Status_OS é o mesmo do MSG
        public virtual String Status_OS { get; set; }

        public virtual String Status_Cobranca { get; set; }



        public virtual Int32? Id_Tipo_Imovel { get; set; }
        //Tipo de Imovel (Referente ao Cliente)
        public virtual String Tipo_Imovel { get; set; }

        //Aniel - Grupo_Area (Campo escritorio Aniel)
        public virtual String Cod_Grupo_Area { get; set; }
        public virtual String Grupo_Area { get; set; }

        public virtual String Cod_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }
        public virtual Int32? Cod_Natureza { get; set; }
        public virtual String Exige_Pagamento_Instalacao { get; set; }


        /// <summary>
        /// Campo Utilizado no Agendamento
        /// </summary>
        public virtual DateTime? TempoExecucao { get; set; }

        public virtual String GetEndereco()
        {
            if (String.IsNullOrEmpty(Endereco_Cliente))
                return String.Empty;

            return $"{Endereco_Cliente} {Endereco_Numero} {Endereco_Bairro} {Endereco_Cidade}";
        }

    }
    public class Tipo_Servico
    {
        public Int64 codTipoServ { get; set; }
        public String descricaoTipoServ { get; set; }
    }

    public class Sub_Tipo_Servico
    {
        public string codSubTipoServ { get; set; }
        public Int64 codTipoServ { get; set; }
        public String descricaoSubTipoServ { get; set; }
    }

    public class EscritorioImp
    {
        public String codEscritorio { get; set; }
        public Int64 codCliente { get; set; }
        public String descricaoEscritorio { get; set; }
    }


    public class DocumentoProducaoLog
    {
        public string codCt { get; set; }
        public string projeto { get; set; }
        public string numObra { get; set; }
        public string numObraOriginal { get; set; }
        public string numDoc { get; set; }
        public string dataMaxima { get; set; }
        public string usuarioCriacao { get; set; }
        public string diaCriacao { get; set; }
        public string horaCriacao { get; set; }
        public string usuarioExclusao { get; set; }
        public string ipMaquina { get; set; }
        public string nomeMaquina { get; set; }
        public string obs { get; set; }
    }


    public class DocumentoProducao_GetDocumento
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public String Num_Obra_Original { get; set; }
        public Int32? Status { get; set; }
        public Int32? Status_Cobranca { get; set; }
        public String Cod_Encerramento { get; set; }
    }



    public class DocumentoProducao_VinculoONUComboResult : ObjComboResult<String>
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String CodCpl { get; set; }
        public virtual String Equipe { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_PlanoProduto { get; set; }
        public virtual Int64? Id_Cliente_PlanoProduto { get; set; }
    }

    public class OjCombCodigoEncerramento : IObjComboResult<String>
    {
        public virtual String Id { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Text { get; set; }

    }

    public class DocumentoProducao_ImportacaoOS
    {
        [Display(Description = "Número da OS Original")]
        public virtual String NUM_OBRA_ORIGINAL { get; set; }
        [Display(Description = "Número da OS")]
        public virtual String NUM_OBRA { get; set; }
        [Display(Description = "Contrato")]
        public virtual String NUM_DOC { get; set; }
        [Display(Description = "Período")]
        public virtual String COD_TURNO_SERV { get; set; }
        [Display(Description = "Segmento")]
        public virtual String SEGMENTO { get; set; }
        [Display(Description = "Logradouro")]
        public virtual String ENDERECO { get; set; }
        [Display(Description = "CEP")]
        public virtual String ENDERECO_CPL { get; set; }
        [Display(Description = "Area")]
        public virtual String COD_AREA { get; set; }
        [Display(Description = "Localidade")]
        public virtual String COD_LOCALIDADE { get; set; }
        [Display(Description = "Produto Para")]
        public virtual String END_ARM { get; set; }
        [Display(Description = "Cod.Tipo de Serviço")]
        public virtual String COD_TIPO_SERV_PRINC { get; set; }
        [Display(Description = "Tipo de Serviço")]
        public virtual String COD_TIPO_SERV { get; set; }
        [Display(Description = "Cliente")]
        public virtual String TITULAR { get; set; }
        [Display(Description = "Produto")]
        public virtual String OUTROS_GASTOS { get; set; }
        [Display(Description = "Telefone Celular")]
        public virtual String TEL_CIENTE { get; set; }
        [Display(Description = "Telefone")]
        public virtual String TAREFA { get; set; }
        [Display(Description = "Observação")]
        public virtual String OBSERVACAO_AUX { get; set; }
        [Display(Description = "Data Agendamento")]
        public virtual String DATA_MAXIMA { get; set; }
        [Display(Description = "Sub.Tipo de Serviço")]
        public virtual String COD_SUB_TIPO_SERV { get; set; }
        [Display(Description = "Numero do Cliente")]
        public virtual String DESC_TIPO_SERVICO { get; set; }
        [Display(Description = "Descricao tipo servico")]
        public virtual String OXBDPEE { get; set; }
        [Display(Description = "Numero do Imovel")]
        public virtual String ENDERECO_NUMERO { get; set; }
        [Display(Description = "Complemento do Endereco")]
        public virtual String ENDERECO_COMPL { get; set; }
        [Display(Description = "E-Mail do Cliente")]
        public virtual String EMAIL_ASSINANTE { get; set; }
        [Display(Description = "Escritório")]
        public virtual String COD_ESCRITORIO { get; set; }
        [Display(Description = "Data Status Cliente")]
        public virtual String DATA_STATUS_CLIENTE { get; set; }
        [Display(Description = "Hora Agendamento")]
        public virtual String HORA_MAXIMA { get; set; }
        [Display(Description = "UF Cliente")]
        public virtual String ENDERECO_UF { get; set; }
        [Display(Description = "Latitude")]
        public virtual String LATITUDE_STR { get; set; }
        [Display(Description = "Longitude")]
        public virtual String LONGITUDE_STR { get; set; }
        [Display(Description = "Equipe")]
        public virtual String EQUIPE { get; set; }
        [Display(Description = "Tipo de Imovel")]
        public virtual Int32? PARPRIM_PARA { get; set; }
        [Display(Description = "Seq")]
        public virtual int SEQ { get; set; }
        [Display(Description = "Cidade")]
        public virtual String ENDERECO_TIPO { get; set; }
        [Display(Description = "Bairro")]
        public virtual String ENDERECO_BAIRRO { get; set; }
        [Display(Description = "CPF do Cliente")]
        public virtual String CPF_CLIENTE { get; set; }
        [Display(Description = "Senha Baixa")]
        public virtual String SENHA_BAIXA { get; set; }
        [Display(Description = "Pppoe")]
        public virtual String PPPOE { get; set; }
        [Display(Description = "Data Criacao")]
        public virtual String DATA_CRIACAO { get; set; }
        [Display(Description = "Hora Criacao")]
        public virtual String HORA_CRIACAO { get; set; }   
        [Display(Description = "Código POP")]
        public virtual String ID_POP { get; set; }
        [Display(Description = "Código OLT")]
        public virtual String ID_OLT { get; set; }
        [Display(Description = "Código Elemento de Rede")]
        public virtual String ID_ELEMENTO_REDE { get; set; }


    }

}