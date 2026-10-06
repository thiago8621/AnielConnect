using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Tb_Pt_Jornada_FuncionarioMap : ClassMap<Tb_Pt_Jornada_Funcionario>
    {
        public Tb_Pt_Jornada_FuncionarioMap()
        {
            Table("TB_PT_JORNADA_FUNCIONARIO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_JORNADA_FUNC");

            Map(x => x.Sigla, "SIGLA");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Padrao, "PADRAO");
            Map(x => x.Tipo_Jornada, "TIPO_JORNADA").CustomType<Tipo_Jornada>();
            Map(x => x.Intevalo_Almoco, "INTERVALO");
            Map(x => x.Tipo_Intervalo, "TIPO_INTERVALO").CustomType<Tipo_Intervalo_Jornada>();

            Map(x => x.Opcao_Trabalha_Feriado, "OPCAO_TRABALHA_FERIADO");
            Map(x => x.Inicio_Feriado, "INICIO_FERIADO");
            Map(x => x.Termino_Feriado, "TERMINO_FERIADO");
            Map(x => x.Duracao_Feriado, "DURACAO_FERIADO");
            Map(x => x.Inicio_Intervalo_Feriado, "INICIO_INTERVALO_FERIADO");
            Map(x => x.Termino_Intervalo_Feriado, "TERMINO_INTERVALO_FERIADO");
            Map(x => x.Duracao_Intervalo_Feriado, "DURACAO_INTERVALO_FERIADO");

            Map(x => x.Opcao_Trabalha_Domingo, "OPCAO_TRABALHA_DOMINGO");
            Map(x => x.Inicio_Domingo, "INICIO_DOMINGO");
            Map(x => x.Termino_Domingo, "TERMINO_DOMINGO");
            Map(x => x.Duracao_Domingo, "DURACAO_DOMINGO");
            Map(x => x.Inicio_Intervalo_Domingo, "INICIO_INTERVALO_DOMINGO");
            Map(x => x.Termino_Intervalo_Domingo, "TERMINO_INTERVALO_DOMINGO");
            Map(x => x.Duracao_Intervalo_Domingo, "DURACAO_INTERVALO_DOMINGO");

            Map(x => x.Opcao_Trabalha_Segunda, "OPCAO_TRABALHA_SEGUNDA");
            Map(x => x.Inicio_Segunda, "INICIO_SEGUNDA");
            Map(x => x.Termino_Segunda, "TERMINO_SEGUNDA");
            Map(x => x.Duracao_Segunda, "DURACAO_SEGUNDA");
            Map(x => x.Inicio_Intervalo_Segunda, "INICIO_INTERVALO_SEGUNDA");
            Map(x => x.Termino_Intervalo_Segunda, "TERMINO_INTERVALO_SEGUNDA");
            Map(x => x.Duracao_Intervalo_Segunda, "DURACAO_INTERVALO_SEGUNDA");

            Map(x => x.Opcao_Trabalha_Terca, "OPCAO_TRABALHA_TERCA");
            Map(x => x.Inicio_Terca, "INICIO_TERCA");
            Map(x => x.Termino_Terca, "TERMINO_TERCA");
            Map(x => x.Duracao_Terca, "DURACAO_TERCA");
            Map(x => x.Inicio_Intervalo_Terca, "INICIO_INTERVALO_TERCA");
            Map(x => x.Termino_Intervalo_Terca, "TERMINO_INTERVALO_TERCA");
            Map(x => x.Duracao_Intervalo_Terca, "DURACAO_INTERVALO_TERCA");

            Map(x => x.Opcao_Trabalha_Quarta, "OPCAO_TRABALHA_QUARTA");
            Map(x => x.Inicio_Quarta, "INICIO_QUARTA");
            Map(x => x.Termino_Quarta, "TERMINO_QUARTA");
            Map(x => x.Duracao_Quarta, "DURACAO_QUARTA");
            Map(x => x.Inicio_Intervalo_Quarta, "INICIO_INTERVALO_QUARTA");
            Map(x => x.Termino_Intervalo_Quarta, "TERMINO_INTERVALO_QUARTA");
            Map(x => x.Duracao_Intervalo_Quarta, "DURACAO_INTERVALO_QUARTA");

            Map(x => x.Opcao_Trabalha_Quinta, "OPCAO_TRABALHA_QUINTA");
            Map(x => x.Inicio_Quinta, "INICIO_QUINTA");
            Map(x => x.Termino_Quinta, "TERMINO_QUINTA");
            Map(x => x.Duracao_Quinta, "DURACAO_QUINTA");
            Map(x => x.Inicio_Intervalo_Quinta, "INICIO_INTERVALO_QUINTA");
            Map(x => x.Termino_Intervalo_Quinta, "TERMINO_INTERVALO_QUINTA");
            Map(x => x.Duracao_Intervalo_Quinta, "DURACAO_INTERVALO_QUINTA");

            Map(x => x.Opcao_Trabalha_Sexta, "OPCAO_TRABALHA_SEXTA");
            Map(x => x.Inicio_Sexta, "INICIO_SEXTA");
            Map(x => x.Termino_Sexta, "TERMINO_SEXTA");
            Map(x => x.Duracao_Sexta, "DURACAO_SEXTA");
            Map(x => x.Inicio_Intervalo_Sexta, "INICIO_INTERVALO_SEXTA");
            Map(x => x.Termino_Intervalo_Sexta, "TERMINO_INTERVALO_SEXTA");
            Map(x => x.Duracao_Intervalo_Sexta, "DURACAO_INTERVALO_SEXTA");

            Map(x => x.Opcao_Trabalha_Sabado, "OPCAO_TRABALHA_SABADO");
            Map(x => x.Inicio_Sabado, "INICIO_SABADO");
            Map(x => x.Termino_Sabado, "TERMINO_SABADO");
            Map(x => x.Duracao_Sabado, "DURACAO_SABADO");
            Map(x => x.Inicio_Intervalo_Sabado, "INICIO_INTERVALO_SABADO");
            Map(x => x.Termino_Intervalo_Sabado, "TERMINO_INTERVALO_SABADO");
            Map(x => x.Duracao_Intervalo_Sabado, "DURACAO_INTERVALO_SABADO");

            Map(x => x.Notif_Inicio_Jornada, "NOTIF_INICIO_JORNADA");
            Map(x => x.Notif_Inicio_Jornada_Min, "NOTIF_INICIO_JORNADA_MIN");
            Map(x => x.Notif_Termino_Jornada, "NOTIF_TERMINO_JORNADA");
            Map(x => x.Notif_Termino_Jornada_Min, "NOTIF_TERMINO_JORNADA_MIN");

            Map(x => x.Notif_Inicio_Almoco, "NOTIF_INICIO_ALMOCO");
            Map(x => x.Notif_Inicio_Almoco_Min, "NOTIF_INICIO_ALMOCO_MIN");
            Map(x => x.Notif_Termino_Almoco, "NOTIF_TERMINO_ALMOCO");
            Map(x => x.Notif_Termino_Almoco_Min, "NOTIF_TERMINO_ALMOCO_MIN");

            Map(x => x.Notif_Pausa_Almoco, "NOTIF_PAUSA_ALMOCO");
            Map(x => x.Notif_Pausa_Almoco_Min, "NOTIF_PAUSA_ALMOCO_MIN");
            Map(x => x.Notif_Termino_Jornada_Flex, "NOTIF_TERMINO_JORNADA_FLEX");
            Map(x => x.Notif_Termino_Jornada_Flex_Min, "NOTIF_TERMINO_JORNADA_FLEX_MIN");

            Map(x => x.Notif_Trab_Feriado, "NOTIF_TRAB_FERIADO");
            Map(x => x.Notif_Trab_Feriado_Dias, "NOTIF_TRAB_FERIADO_DIAS");

            Map(x => x.Notif_Trab_Ferias, "NOTIF_TRAB_FERIAS");
            Map(x => x.Notif_Trab_Ferias_Dias, "NOTIF_TRAB_FERIAS_DIAS");
            Map(x => x.Notif_Descanso_Minimo, "NOTIF_DESCANSO_MINIMO");

            Map(x => x.Notif_Sobreaviso, "NOTIF_SOBREAVISO");
            Map(x => x.Notif_Solicitacao_Ajuste, "NOTIF_SOLICITACAO_AJUSTE");
            Map(x => x.Notif_Ajuste_Solicitado_Colaborador, "NOTIF_AJUSTE_SOLICITADO_COLABOR");

            Map(x => x.Regra_Descanso_Minimo, "REGRA_DESCANSO_MINIMO");
            Map(x => x.Regra_Adicional_Noturno_Ini, "REGRA_ADICIONAL_NOTURNO_INI");
            Map(x => x.Regra_Adicional_Noturno_Fin, "REGRA_ADICIONAL_NOTURNO_FIN");
            Map(x => x.Regra_Perc_Noturno, "REGRA_PERC_NOTURNO");

            Map(x => x.Regra_Marca_OffLine, "REGRA_MARCA_OFFLINE");
            Map(x => x.Regra_Habilitar_Cerca, "REGRA_HABILITAR_CERCA");
            Map(x => x.Regra_Solicitar_Foto, "REGRA_SOLICITAR_FOTO");
            Map(x => x.Regra_Solicitar_QrCode, "REFRA_SOLICITAR_QRCODE");
            Map(x => x.Regra_Solicitar_Foto_Assinatura, "REGRA_SOLIC_FOTO_ASSINATURA");

            Map(x => x.Regra_Tolerancia_Check_Entrada, "TOLERANCIA_CHECK_ENTRADA");
            Map(x => x.Regra_Tolerancia_Check_Saida, "TOLERANCIA_CHECK_SAIDA");
            Map(x => x.Regra_Tolerancia_Check_Limite_Max_Diario, "TOLERANCIA_LIM_MAX_DIARIO");
            Map(x => x.Regra_Tolerancia_Input_Entrada, "TOLERANCIA_MINUTOS_ENTRADA");
            Map(x => x.Regra_Tolerancia_Input_Saida, "TOLERANCIA_MINUTOS_SAIDA");
            Map(x => x.Regra_Tolerancia_Input_Carga_Diaria, "TOLERANCIA_CARGA_DIARIA");

            Map(x => x.Teto_Diario_HoraExtra, "TETO_DIARIO_HORAEXTRA");
            Map(x => x.Horas_Diarias, "HORAS_DIARIAS");
            Map(x => x.Utiliza_Banco_Horas, "UTILIZA_BANCO_HORAS");
            Map(x => x.Horas_Extras_Periodo_Vigente, "HORAS_EXTRAS_PERIODO_VIGENTE");
            Map(x => x.Trabalha_Compensacao, "TRABALHA_COMPENSACAO");
            Map(x => x.Qtd_Horas_Sobreaviso_Extras, "QDT_HORAS_SOBREAVISO_EXTRAS");
            Map(x => x.Considera_Intervalo_HoraExtra_Atraso, "CONSIDERA_INTERVALO_HE_ATRASO");
            Map(x => x.Tolerancia_Atraso_Hora_Extra, "TOLERANCIA_ATRASO_HORA_EXTRA");
            Map(x => x.Enviar_Banco_Horas, "ENVIAR_BANCO_HORAS");
            Map(x => x.Base_Conversao_Hora_Extra, "BASE_CONVERSAO_HORAS");
            Map(x => x.Forma_Compensacao_Horas, "FORMA_COMPENSACAO").CustomType<Enum_Forma_Compensacao>();
            Map(x => x.Ordem_Compensacao, "ORDEM_COMPENSACAO").CustomType<Ordem_Compensacao>();
            Map(x => x.Utiliza_tolerancia_Horas, "UTILIZA_TOLERANCIA");

            Map(x => x.Escala_Personalizada, "ESCALA_PERSONALIZADA");
            Map(x => x.Dias_Trabalho, "DIAS_TRABALHO");
            Map(x => x.Dias_Folga, "DIAS_FOLGA");
            Map(x => x.Id_Horario_Connect, "ID_HORARIO_CONNECT");
        }
    }
}
