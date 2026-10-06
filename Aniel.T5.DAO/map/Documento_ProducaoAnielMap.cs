using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Documento_ProducaoAnielMap : ClassMap<Documento_ProducaoAniel>
    {
        public Documento_ProducaoAnielMap() : base()
        {
            Table("TB_DOCUMENTO_PRODUCAO");

            CompositeId()
            .KeyProperty(x => x.CodCt, "CODCT")
            .KeyProperty(x => x.Projeto, "PROJETO")
            .KeyProperty(x => x.Num_Obra, "NUM_OBRA")
            .KeyProperty(x => x.Num_Doc, "NUM_DOC");

            Map(x => x.Unidade_Custo, "UNIDADE_CUSTO");
            Map(x => x.Numero_Obra_Original, "NUM_OBRA_ORIGINAL");
            Map(x => x.Seq, "SEQ");
            Map(x => x.OXBDPEE, "OXBDPEE");
            Map(x => x.Devolvida, "DEVOLVIDA");
            Map(x => x.Status, "STATUS_EXECUCAO").CustomType<StatusExecucaoAniel>().Index("IDX_DOCUMENTO_PRODUCAO_DTEQ");
            Map(x => x.Status_Cobranca, "STATUS_COBRANCA").CustomType<Status_CobrancaAniel>().Index("IDX_DOCUMENTO_PRODUCAO_SSC");
            Map(x => x.Status_Auditoria, "STATUS_AUDITORIA").CustomType<Status_AuditoriaAniel>();
            Map(x => x.UsuarioAuditoria, "USUARIO_AUDITORIA").Length(10);
            Map(x => x.Tipo_Prioridade, "TIPO_PRIORIDADE").CustomType<Tipo_PrioridadeT5>();
            //Map(x => x.Prioridade_Natureza, "PRIORIDADE_NATUREZA").CustomType<Tipo_PrioridadeT5>();
            Map(x => x.Equipe, "EQUIPE").Index("IDX_DOC_PRODUCAO_EQUIPE").Index("IDX_DOCUMENTO_PRODUCAO_DTEQ");
            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO");
            Map(x => x.Cod_Area, "COD_AREA");
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");
            Map(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");
            Map(x => x.Cod_Sub_Tipo_Serv, "COD_SUB_TIPO_SERV");
            Map(x => x.Segmento, "SEGMENTO");
            Map(x => x.Observacao, "OBSERVACAO");
            Map(x => x.ObservacaoPausa, "OBSERVACAO_PAUSA");
            Map(x => x.InformacoesComplementares, "OBSERVACAO_OITV");
            Map(x => x.Cod_Tipo_Imovel, "PARPRIM_PARA");
            Map(x => x.Tipo_Producao, "TIPO_PRODUCAO").Length(2);
            Map(x => x.Nome_Cliente, "CIENTE");
            Map(x => x.Id_PlanoProduto, "ID_PLANOPRODUTO");
            Map(x => x.CodigoPlanoIntegrator, "CODIGO_PLANO_INTEGRATOR").Length(50);
            Map(x => x.Id_Cliente_PlanoProdutoConnect, "ID_CLIENTE_PLANOPRODUTOCONNECT");
            Map(x => x.Id_Cliente_ContratoConnect, "ID_CLIENTE_CONTRATOCONNECT");
            Map(x => x.Id_PlanoProduto_Ant, "ID_PLANOPRODUTO_ANT");
            Map(x => x.Id_Cliente_PlanoConnect_Ant, "ID_CLIENTE_PLANOCONNECT_ANT");
            Map(x => x.DescPlanoAnterior, "DESC_PLANO_ANT");
            Map(x => x.CategoriaPlanoAnterior, "CATEGORIA_PLANO_ANT");
            Map(x => x.Logradouro, "ENDERECO");
            Map(x => x.Cidade, "ENDERECO_TIPO");
            Map(x => x.Bairro, "ENDERECO_BAIRRO");
            Map(x => x.UF, "ENDERECO_UF");
            Map(x => x.Cep, "ENDERECO_CPL");
            Map(x => x.Numero, "ENDERECO_NUMERO");
            Map(x => x.Complemento, "ENDERECO_COMPL");
            Map(x => x.Pais, "ENDERECO_PAIS");
            Map(x => x.Id_Endereco_Cliente_Antigo, "ID_ENDERECO_CLIENTE_ANTIGO");
            Map(x => x.Endereco_Antigo, "ENDERECO_ANTIGO").Length(100);
            Map(x => x.Endereco_Complemento_Antigo, "ENDERECO_COMPL_ANTIGO").Length(100);
            Map(x => x.Endereco_Numero_Antigo, "ENDERECO_NUMERO_ANTIGO").Length(10);
            Map(x => x.Endereco_Bairro_Antigo, "ENDERECO_BAIRRO_ANTIGO").Length(100);
            Map(x => x.Endereco_Cidade_Antigo, "ENDERECO_CIDADE_ANTIGO").Length(100);
            Map(x => x.Endereco_UF_Antigo, "ENDERECO_UF_ANTIGO").Length(2);
            Map(x => x.Endereco_Cep_Antigo, "ENDERECO_CEP_ANTIGO").Length(100);
            Map(x => x.TelefonePrincipal, "TEL_TITULAR");
            Map(x => x.TelefoneComercial, "TEL_COMERCIAL");
            Map(x => x.CelularCliente, "TEL_CIENTE");
            Map(x => x.CPF_CLIENTE, "CPF_CLIENTE").Length(14);
            Map(x => x.Titular, "TITULAR").Length(50);
            Map(x => x.RG_Assinante, "RG_ASSINANTE").Length(20);
            Map(x => x.Email_Assinante, "EMAIL_ASSINANTE").Length(50);

            Map(x => x.DataAbertura, "DATA_ABERTURA");
            Map(x => x.HoraAbertura, "HORA_ABERTURA");


            Map(x => x.DataHora_Deslocamento, "DATAHORA_DESLOCAMENTO");
            Map(x => x.DataHora_Interromper_Deslocamento, "DATAHORA_INTERROMPER_DESLOC");
            Map(x => x.DataHora_Ini_Atend, "DATAHORA_INI_ATEND").Index("IDX_DOCUMENTO_PRODUCAO_INI_ATEN");
            Map(x => x.DataHora_Fim_Atend, "DATAHORA_FIM_ATEND");
            Map(x => x.DataHora_Pausou, "DATAHORA_PAUSOU");
            Map(x => x.DataHora_Reinicio, "DATAHORA_REINICIO");
            Map(x => x.Data_Maxima, "DATA_MAXIMA").Index("IDX_DOC_MAXIMA_EQUIPE");
            Map(x => x.Hora_Maxima, "HORA_MAXIMA").CustomType("Time");
            Map(x => x.Data_Prod, "DATA_PROD").Index("IDX_DOC_PRODUCAO_EQUIPE").Index("IDX_DOCUMENTO_PRODUCAO_DTEQ");
            Map(x => x.Hora_Prod, "HORA_PROD").CustomType("Time");
            Map(x => x.Data_Indicador, "DATA_INDICADOR").Index("IDX_DOCUMENTO_PRODUCAO_DTIND");
            Map(x => x.Hora_Indicador, "HORA_INDICADOR").CustomType("Time");
            Map(x => x.Data_Encerramento, "DATA_ENCERRAMENTO");
            Map(x => x.Hora_Encerramento, "HORA_ENCERRAMENTO").CustomType("Time");
            Map(x => x.Cod_Encerramento, "COD_ENCERRAMENTO");
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Latitude_Str, "LATITUDE_STR");
            Map(x => x.Longitude_Str, "LONGITUDE_STR");
            Map(x => x.Num_RVT, "NUM_RVT");
            Map(x => x.Tarefa, "TAREFA");
            Map(x => x.Observacao_Aux, "OBSERVACAO_AUX");
            Map(x => x.Observacao_Teste, "OBSERVACAO_TESTE");
            Map(x => x.Usuario_Baixa, "USUARIO_BAIXA");
            Map(x => x.Modulo_Baixa, "MODULO_BAIXA");
            Map(x => x.Usuario, "USUARIO");
            Map(x => x.Dia, "DIA").Index("IDX_DOCPROD_DIA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.AlterarIndicador, "ALTERAR_INDICADOR");
            Map(x => x.Usuario_Monitor, "USUARIO_MONITOR");
            Map(x => x.Cod_Superv, "COD_SUPERV");
            Map(x => x.Usuario_Agendamento, "USUARIO_AGENDAMENTO");
            Map(x => x.DiaHora_Agendamento, "DATAHORA_AGENDAMENTO");
            Map(x => x.Origem_Documento, "ORIGEM_DOCUMENTO").CustomType<Origem_Documento_Producao>();
            Map(x => x.Id_Atendimento, "ID_ATENDIMENTO_CONNECT").Index("IDX_DOC_PRODUCAO_ATENDIMENTO");
            Map(x => x.Sigla_Modulo, "SIGLA_MODULO");
            Map(x => x.Responsabilidade, "RESPONSABILIDADE").CustomType<ResponsabilidadeOS>();
            Map(x => x.Roterizar, "ROTERIZAR");
            Map(x => x.SenhaRadius, "SENHA_BAIXA");
            Map(x => x.LoginRadius, "PPPOE");
            Map(x => x.CodTurnoServ, "COD_TURNO_SERV");
            Map(x => x.PortaFTTH, "PORTA_FTTH");
            Map(x => x.CaixaFTTH, "CAIXA_FTTH");
            Map(x => x.IdLogin, "ID_LOGIN");
            Map(x => x.Adicional_1, "ADICIONAL_1").Length(100);
            Map(x => x.Adicional_2, "ADICIONAL_2").Length(100);
            Map(x => x.Adicional_3, "ADICIONAL_3").Length(100);
            Map(x => x.Adicional_4, "ADICIONAL_4").Length(100);
            Map(x => x.Adicional_5, "ADICIONAL_5").Length(100);
            Map(x => x.Adicional_6, "ADICIONAL_6").Length(100);
            Map(x => x.Adicional_7, "ADICIONAL_7").Length(100);
            Map(x => x.Adicional_8, "ADICIONAL_8").Length(100);
            Map(x => x.Adicional_9, "ADICIONAL_9").Length(100);
            Map(x => x.Adicional_10, "ADICIONAL_10").Length(100);
            Map(x => x.Id_Venda, "ID_VENDA");
            Map(x => x.Id_Cobranca, "ID_COBRANCA");
            Map(x => x.NomeInstalacao, "NOME_INSTALACAO");
            Map(x => x.CpfInstalacao, "CPF_INSTALACAO").Length(50);
            Map(x => x.TelefoneInstalacao, "TELEFONE_INSTALACAO").Length(50);
            Map(x => x.WO);
            Map(x => x.TemOsDigital, "TEM_OSDIGITAL");
            //Map(x => x.UltimoSeq, "ULTIMO_SEQ");
            Map(x => x.CodDepto, "COD_DEPTO");
            Map(x => x.Tipo, "TIPO").Length(4);
            Map(x => x.Id_Olt, "ID_OLT");
            Map(x => x.Id_Pop, "ID_POP");
            Map(x => x.Id_Elemento_Rede, "ID_ELEMENTO_REDE");
            Map(x => x.Integracao, "INTEGRACAO").Length(1);
            Map(x => x.Ssid_Wifi, "WIFI_SSID").Length(50);
            Map(x => x.Senha_Wifi, "WIFI_PASSWORD").Length(50);
            Map(x => x.Bloco, "BLOCO").Length(50);
            Map(x => x.Apartamento, "APARTAMENTO").Length(50);
            Map(x => x.NumDoc_Manut, "NUMDOC_MANUT");
            Map(x => x.CodCt_Manut, "CODCT_MANUT");
            Map(x => x.Projeto_Manut, "PROJETO_MANUT");
            Map(x => x.NumObra_Manut, "NUMOBRA_MANUT");
            Map(x => x.CodMotivoReagendamento, "COD_MOTIVO_REAGENDAMENTO");
            Map(x => x.ObsMotivoReagendamento, "OBSERVACAO_MOTIVO_REAGENDAMENTO").Length(110);
            Map(x => x.Repetida, "REPETIDA");
            Map(x => x.UtilizaEndCond, "UTILIZA_END_COND");
            Map(x => x.IdCondominioPredioEndereco, "END_CONDPRED_SELECIONADO");
            Map(x => x.BuscarEnderecoPorId, "BUSCAR_ENDPOR_ID");
            Map(x => x.RoteirizadoAuto, "ROTEIRIZADO_AUTO");
            Map(x => x.Cod_Motivo_Cancel, "COD_MOTIVO_CANCEL");
            Map(x => x.MotivoCancelamento, "MOTIVOCANCELAMENTO");
            Map(x => x.Senha_Autorizacao_Baixa, "SENHA_AUTORIZACAO_BAIXA").Length(6);
            Map(x => x.Data_DashBoard, "DATA_DASHBOARD").Index("IDX_DATA_DASHBOARD");
            Map(x => x.TipoManutencaoOs, "TIPO_MANUTENCAO").CustomType<TipoManutencaoOs>();
            Map(x => x.DescricaoPlanoAssinante, "DESCRICAO_PLANO_ASSINANTE").Length(150);
            Map(x => x.Tecnologia, "TECNOLOGIA").Length(100);

            References(x => x.EquipeAniel, "EQUIPE").LazyLoad().ReadOnly();
            References(x => x.Agendamento, "ID_AGENDAMENTO");
            References(x => x.TipoServico, "COD_TIPO_SERV").LazyLoad().ReadOnly();
            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_DOC_PROD_IDCLIENTE");
            //References(x => x.PlanoProduto, "ID_PLANOPRODUTO");
            References(x => x.Monitor, "USUARIO_MONITOR").ReadOnly();
            References(x => x.TurnoServico, "COD_TURNO_SERV").ReadOnly();

        }

    }
}
