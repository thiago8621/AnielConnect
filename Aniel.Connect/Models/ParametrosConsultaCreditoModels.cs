using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AprovacaoCriterioParametroCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito;

namespace Aniel.Connect.Models
{
    public class ParametrosConsultaCreditoModel
    {
        public virtual Int64 Id { get; set; }
  
    }


    public class ParametrosConsultaCreditoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 PontosPrimeiraFaixaCriterio { get; set; }
        public virtual Int64 PontosSegundaFaixaCriterio { get; set; }
        public virtual Int64 PontosTerceiraFaixaCriterio { get; set; }
        public virtual Int64 Valor { get; set; }
        public virtual Int64 Pontos { get; set; }
        public virtual Boolean Eliminatorio { get; set; }
        public virtual Boolean UtilizaFaixaCriterio1 { get; set; }
        public virtual Boolean UtilizaFaixaCriterio2 { get; set; }
        public virtual Boolean UtilizaFaixaCriterio3 { get; set; }
        public virtual CriteriosParametrosCredito? Criterios { get; set; }
        public virtual EnumOperadores Operador { get; set; }
        public virtual AprovacaoCriterioParametroCredito TipoAprovacaoFaixa1 { get; set; }
        public virtual AprovacaoCriterioParametroCredito TipoAprovacaoFaixa2 { get; set; }
        public virtual AprovacaoCriterioParametroCredito TipoAprovacaoFaixa3 { get; set; }

        private List<CriteriosListaModel> criterios_lista;
        public virtual List<CriteriosListaModel> CriteriosLista
        {
            get
            {
                if (criterios_lista == null)
                    criterios_lista = new List<CriteriosListaModel>();
                return criterios_lista;
            }
            set
            {
                criterios_lista = value;
            }
        }

    }

    public class CriteriosListaModel 
    {
        public Int64 Id { get; set; }
        public String Criterio { get; set; }
        public int TipoCriterio { get; set; }
        public Int64 Valor { get; set; }
        public String Operador { get; set; }
        public int TipoOperador { get; set; }
        public Int64 Pontos { get; set; }
        public Boolean Eliminatorio { get; set; }

    }
    public class CriteriosParamDataTableRequest : DataTableRequest
    {
        public virtual Int32 Id { get; set; }
        public virtual String Criterio { get; set; }
        public virtual Int64 Valor { get; set; }
        public virtual String Operador { get; set; }
        public virtual Int64 Pontos { get; set; }
        public virtual Boolean Eliminatorio { get; set; }

    }
    public class CriteriosParametrosListaModel
    {
        public virtual Int64 Id { get; set; }
        //public virtual CriteriosParametrosCredito Criterio { get; set; }
        public virtual String Criterio { get; set; }
        public virtual Int64 Valor { get; set; }
        public virtual String Operadores { get; set; }
        public virtual Int64 Pontos { get; set; }
        public virtual Boolean Eliminatorio { get; set; }

    }

    public class ParametrosConsultaCreditoList
    {
        public virtual String cpf { get; set; }
        public virtual String cnpj { get; set; }

    }

    public class BlocosRequestApiSerasaModel
    {
        public virtual String cpf { get; set; }
        public virtual String consulta { get; set; }
    }

    public class ConsultaBlocosSerasaPFListaModel
    {
        public string cpf { get; set; }
        public string BlocoN200 { get; set; }
        public string BlocoN210 { get; set; }
        public string BlocoN220 { get; set; }
        public string BlocoN230 { get; set; }
        public string BlocoN240 { get; set; }
        public string BlocoN250 { get; set; }
        public string BlocoN270 { get; set; }
        public string BlocoN410 { get; set; }
        public string BlocoN420 { get; set; }
        public string BlocoN430 { get; set; }
        public string BlocoN440 { get; set; }
        public string BlocoN460 { get; set; }
        public string BlocoN500 { get; set; }
        public string BlocoN690 { get; set; }
    }

    public class ConsultaBlocosSerasaPFDataTableRequest : DataTableRequest
    {
        public string cpf { get; set; }
    }

    public class DadosConsultaSerasaList
    {
        public virtual String cpf { get; set; }
        public virtual String BlocoN200 { get; set; }
        public virtual String BlocoN210 { get; set; }
        public virtual String BlocoN220 { get; set; }
        public virtual String BlocoN230 { get; set; }
        public virtual String BlocoN240 { get; set; }
        public virtual String BlocoN250 { get; set; }
        public virtual String BlocoN270 { get; set; }
        public virtual String BlocoN410 { get; set; }
        public virtual String BlocoN420 { get; set; }
        public virtual String BlocoN430 { get; set; }
        public virtual String BlocoN440 { get; set; }
        public virtual String BlocoN460 { get; set; }
        public virtual String BlocoN500 { get; set; }
        public virtual String BlocoN690 { get; set; }
    }
        public class ConsultaBlocosSerasaPJListaModel
    {
        public virtual String cpf { get; set; }
        public virtual String BlocoN200 { get; set; }
        public virtual String BlocoN210 { get; set; }
        public virtual String BlocoN220 { get; set; }
        public virtual String BlocoN230 { get; set; }
        public virtual String BlocoN240 { get; set; }
        public virtual String BlocoN250 { get; set; }
        public virtual String BlocoN270 { get; set; }
        public virtual String BlocoN410 { get; set; }
        public virtual String BlocoN420 { get; set; }
        public virtual String BlocoN430 { get; set; }
        public virtual String BlocoN440 { get; set; }
        public virtual String BlocoN460 { get; set; }
        public virtual String BlocoN500 { get; set; }
        public virtual String BlocoN690 { get; set; }
    }

    public class ConsultaStringBlocosLista
    {
        public virtual String N200subtipo { get; set; }
        public virtual String N200razao_social { get; set; }
        public virtual String N200data_nascimento { get; set; }
        public virtual String N200situacao_documento { get; set; }
        public virtual String N200data_situacao_documento { get; set; }
        public virtual String N200existe_CCF { get; set; }     
        public virtual String N210subtipo { get; set; }
        public virtual String N210num_mensagem { get; set; }
        public virtual String N210total_mensagem { get; set; }
        public virtual String N210tipo_documento { get; set; }
        public virtual String N210NumDocumentoRoubado { get; set; }
        public virtual String N210motivo_ocorrencia { get; set; }
        public virtual String N210data_ocorrencia { get; set; }
        public virtual String N210reservado { get; set; }     
        public virtual String N220subtipo { get; set; }
        public virtual String N220num_mensagem { get; set; }
        public virtual String N220total_mensagem { get; set; }
        public virtual String N220tipo_documento { get; set; }
        public virtual String N220motivo_ocorrencia { get; set; }
        public virtual String N220data_ocorrencia { get; set; }
        public virtual String N220reservado { get; set; }
    
        //Pendencias internas - pode repetir ate 10x    
        public virtual String N230subtipo { get; set; }
        public virtual String N230dataOcorrencia { get; set; }
        public virtual String N230modalidade { get; set; }
        public virtual String N230avalista { get; set; }
        public virtual String N230tipoMoeda { get; set; }
        public virtual String N230valor { get; set; }
        public virtual String N230contrato { get; set; }
        public virtual String N230origem { get; set; }
        public virtual String N230totalOcorrencias { get; set; }
        public virtual String N230dataPendenciaMaisAntiga { get; set; }
        public virtual String N230dataPendenciaMaisRecente { get; set; }
        public virtual String N230valorTotalPendenciasInternas { get; set; }    

            //Detalhe das Pendências Financeiras separadas em Pefin, Refin e Dívidas vencidas.    
        public virtual String N240subtipo { get; set; }
        public virtual String N240dataOcorrencia { get; set; }
        public virtual String N240modalidade { get; set; }
        public virtual String N240avalista { get; set; }
        public virtual String N240tipoMoeda { get; set; }
        public virtual String N240valor { get; set; }
        public virtual String N240contrato { get; set; }
        public virtual String N240origem { get; set; }
        public virtual String N240dataPendenciaMaisAntiga { get; set; }
        public virtual String N240dataPendenciaMaisRecente { get; set; }
        public virtual String N240valorTotalPendenciasInternas { get; set; }    
                //Retorna Protesto Nacional nas consultas.    
        public virtual String N250subtipo { get; set; }
        public virtual String N250dataOcorrencia { get; set; }
        public virtual String N250modalidade { get; set; }
        public virtual String N250avalista { get; set; }
        public virtual String N250tipoMoeda { get; set; }
        public virtual String N250valor { get; set; }
        public virtual String N250contrato { get; set; }
        public virtual String N250origem { get; set; }    
                        // Detalhe de Cheques sem fundos Bacen.    
        public virtual String N270subtipo { get; set; }
        public virtual String N270dataOcorrencia { get; set; }
        public virtual String N270numCheque { get; set; }
        public virtual String N270alinea { get; set; }
        public virtual String N270quantidade { get; set; }
        public virtual String N270valor { get; set; }
        public virtual String N270num_banco { get; set; }
        public virtual String N270num_agencia { get; set; }
        public virtual String N270cidade { get; set; }
        public virtual String N270uf { get; set; }    
             // Informações retornadas apenas quando o CEP for informado para consulta.
        public virtual String N410subtipo { get; set; }
        public virtual String N410endereco_cep { get; set; }
        public virtual String N410bairro { get; set; }
        public virtual String N410cidade { get; set; }
        public virtual String N410uf { get; set; } 
         // Endereco do telefone    
        public virtual String N420subtipo { get; set; }
        public virtual String N420doc_assinante { get; set; }
        public virtual String N420tipDocumentoAssinante { get; set; }
        public virtual String N420classeAssinante { get; set; }
        public virtual String N420logradouro { get; set; }
        public virtual String N420bairro { get; set; }
                        //Últimos telefones consultados
        public virtual String N430subtipo { get; set; }
        public virtual String N430primeiroTelefoneConsultado { get; set; }
        public virtual String N430segundoTelefoneConsultado { get; set; }
        public virtual String N430terceiroTelefoneConsultado { get; set; }
        public virtual String N430quartoTelefoneConsultado { get; set; }
        public virtual String N430quintoTelefoneConsultado { get; set; }
            //Consultas de cheques - Interno
        public virtual String subtipo { get; set; }
        public virtual String N440dataEmissaoPrimeiroChequeAVista { get; set; }
        public virtual String N440dataEmissaoUltimoChequeAVista { get; set; }
        public virtual String N440totalChequesEmitidos15diasAPrazo { get; set; }
        public virtual String N440totalChequesEmitidos30diasAPrazo { get; set; }
        public virtual String N440totalChequesEmitidos60diasAPrazo { get; set; }
        public virtual String N440totalChequesEmitidos90diasAPrazo { get; set; }
        public virtual String N440totalChequesAPrazoEmitidosAPrazo { get; set; }
         //Detalhe das Ações Judiciais
        public virtual String N460subtipo { get; set; }
        public virtual String N460dataOcorencia { get; set; }
        public virtual String N460numVaraCivel { get; set; }
        public virtual String N460numDistrinbuidor { get; set; }
        public virtual String N460CodNatureza { get; set; }
        public virtual String N460CodPracaEmbratel { get; set; } // 2decimais
        public virtual String N460UF { get; set; }
        public virtual String N460numFilialCNPPJConsultado { get; set; }
        public virtual String N460digitoVerificadorConsultado { get; set; }
        public virtual String N460principal { get; set; }           
        public virtual String N460dataInclusao { get; set; }           
        public virtual String N460ValorAcaoJudicial { get; set; }           
          
        // participacao societaria
        public virtual String N690subtipo { get; set; }
        public virtual String N690nomeEmpresaParticipante { get; set; }
        public virtual String N690cnpjEmpresaParticipante { get; set; }
        public virtual String N690percentualParticipacao { get; set; } //2 casas decimais retorno
        public virtual String N690ufEmpresaParticipante { get; set; }
        public virtual String N690dataInicioParticipacao { get; set; }
        public virtual String N690dataAtualizacao { get; set; }
        public virtual String N690participadaComRestricao{ get; set; }
        public virtual String N690reservado { get; set; }
        // Score Positivo
        public virtual String F900score { get; set; }
        public virtual String F900faixaIntevaloClass { get; set; }
        public virtual String F900probabilidadeInadimplencia { get; set; }
        public virtual String F900mensagem { get; set; }
    }


}
  