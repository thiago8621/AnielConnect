using Aniel.T5.Model.enumerado;
using NHibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    [Serializable]
    public class Tb_Pt_Jornada : BaseModel
    {
        public virtual Status_Default Status { get; set; }
        public virtual String Sigla { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Descricao_Completa
        {
            get
            {
                return String.Format("{0} - {1}", Sigla, Descricao);
            }
        }

        public virtual Boolean Padrao { get; set; }

        public virtual Tipo_Jornada Tipo_Jornada { get; set; }
        public virtual Boolean Intevalo_Almoco { get; set; }

        public virtual Tipo_Intervalo_Jornada? Tipo_Intervalo { get; set; }

        #region Horarios Jornada

        public virtual Boolean Opcao_Trabalha_Feriado { get; set; }
        public virtual DateTime? Inicio_Feriado { get; set; }
        public virtual DateTime? Termino_Feriado { get; set; }
        public virtual Double? Duracao_Feriado { get; set; }
        public virtual DateTime? Inicio_Intervalo_Feriado { get; set; }
        public virtual DateTime? Termino_Intervalo_Feriado { get; set; }
        public virtual Double? Duracao_Intervalo_Feriado { get; set; }

        public virtual Boolean Opcao_Trabalha_Domingo { get; set; }
        public virtual DateTime? Inicio_Domingo { get; set; }
        public virtual DateTime? Termino_Domingo { get; set; }
        public virtual Double? Duracao_Domingo { get; set; }
        public virtual DateTime? Inicio_Intervalo_Domingo { get; set; }
        public virtual DateTime? Termino_Intervalo_Domingo { get; set; }
        public virtual Double? Duracao_Intervalo_Domingo { get; set; }

        public virtual Boolean Opcao_Trabalha_Segunda { get; set; }
        public virtual DateTime? Inicio_Segunda { get; set; }
        public virtual DateTime? Termino_Segunda { get; set; }
        public virtual Double? Duracao_Segunda { get; set; }
        public virtual DateTime? Inicio_Intervalo_Segunda { get; set; }
        public virtual DateTime? Termino_Intervalo_Segunda { get; set; }
        public virtual Double? Duracao_Intervalo_Segunda { get; set; }

        public virtual Boolean Opcao_Trabalha_Terca { get; set; }
        public virtual DateTime? Inicio_Terca { get; set; }
        public virtual DateTime? Termino_Terca { get; set; }
        public virtual Double? Duracao_Terca { get; set; }
        public virtual DateTime? Inicio_Intervalo_Terca { get; set; }
        public virtual DateTime? Termino_Intervalo_Terca { get; set; }
        public virtual Double? Duracao_Intervalo_Terca { get; set; }

        public virtual Boolean Opcao_Trabalha_Quarta { get; set; }
        public virtual DateTime? Inicio_Quarta { get; set; }
        public virtual DateTime? Termino_Quarta { get; set; }
        public virtual Double? Duracao_Quarta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Quarta { get; set; }
        public virtual DateTime? Termino_Intervalo_Quarta { get; set; }
        public virtual Double? Duracao_Intervalo_Quarta { get; set; }

        public virtual Boolean Opcao_Trabalha_Quinta { get; set; }
        public virtual DateTime? Inicio_Quinta { get; set; }
        public virtual DateTime? Termino_Quinta { get; set; }
        public virtual Double? Duracao_Quinta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Quinta { get; set; }
        public virtual DateTime? Termino_Intervalo_Quinta { get; set; }
        public virtual Double? Duracao_Intervalo_Quinta { get; set; }

        public virtual Boolean Opcao_Trabalha_Sexta { get; set; }
        public virtual DateTime? Inicio_Sexta { get; set; }
        public virtual DateTime? Termino_Sexta { get; set; }
        public virtual Double? Duracao_Sexta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Sexta { get; set; }
        public virtual DateTime? Termino_Intervalo_Sexta { get; set; }
        public virtual Double? Duracao_Intervalo_Sexta { get; set; }

        public virtual Boolean Opcao_Trabalha_Sabado { get; set; }
        public virtual DateTime? Inicio_Sabado { get; set; }
        public virtual DateTime? Termino_Sabado { get; set; }
        public virtual Double? Duracao_Sabado { get; set; }
        public virtual DateTime? Inicio_Intervalo_Sabado { get; set; }
        public virtual DateTime? Termino_Intervalo_Sabado { get; set; }
        public virtual Double? Duracao_Intervalo_Sabado { get; set; }

        #endregion

        public virtual Boolean Notif_Inicio_Jornada { get; set; }
        public virtual Double? Notif_Inicio_Jornada_Min { get; set; }
        public virtual Boolean Notif_Termino_Jornada { get; set; }
        public virtual Double? Notif_Termino_Jornada_Min { get; set; }

        public virtual Boolean Notif_Inicio_Almoco { get; set; }
        public virtual Double? Notif_Inicio_Almoco_Min { get; set; }
        public virtual Boolean Notif_Termino_Almoco { get; set; }
        public virtual Double? Notif_Termino_Almoco_Min { get; set; }

        public virtual Boolean Notif_Pausa_Almoco { get; set; }
        public virtual Double? Notif_Pausa_Almoco_Min { get; set; }
        public virtual Boolean Notif_Termino_Jornada_Flex { get; set; }
        public virtual Double? Notif_Termino_Jornada_Flex_Min { get; set; }

        public virtual Boolean Notif_Trab_Feriado { get; set; }
        public virtual Double? Notif_Trab_Feriado_Dias { get; set; }

        public virtual Boolean Notif_Trab_Ferias { get; set; }
        public virtual Double? Notif_Trab_Ferias_Dias { get; set; }
        public virtual Boolean Notif_Descanso_Minimo { get; set; }
        public virtual Boolean Notif_Sobreaviso { get; set; }
        public virtual Boolean Notif_Solicitacao_Ajuste { get; set; }
        public virtual Boolean Notif_Ajuste_Solicitado_Colaborador { get; set; }


        public virtual DateTime Regra_Descanso_Minimo { get; set; }
        public virtual DateTime Regra_Adicional_Noturno_Ini { get; set; }
        public virtual DateTime Regra_Adicional_Noturno_Fin { get; set; }
        public virtual Double Regra_Perc_Noturno { get; set; }

        public virtual Boolean Regra_Marca_OffLine { get; set; }
        public virtual Boolean Regra_Habilitar_Cerca { get; set; }
        public virtual Boolean Regra_Solicitar_Foto { get; set; }
        public virtual Boolean Regra_Solicitar_QrCode { get; set; }
        public virtual Boolean Regra_Solicitar_Foto_Assinatura { get; set; }

        public virtual Boolean Regra_Tolerancia_Check_Entrada { get; set; }
        public virtual Boolean Regra_Tolerancia_Check_Saida { get; set; }
        public virtual Boolean Regra_Tolerancia_Check_Limite_Max_Diario { get; set; }
        public virtual Double? Regra_Tolerancia_Input_Entrada { get; set; }
        public virtual Double? Regra_Tolerancia_Input_Saida { get; set; }
        public virtual Double? Regra_Tolerancia_Input_Carga_Diaria { get; set; }
        public virtual Boolean? Escala_Personalizada { get; set; }
        public virtual Int32? Dias_Trabalho { get; set; }
        public virtual Int32? Dias_Folga { get; set; }
        public virtual Int64? Id_Horario_Connect { get; set; }


        #region Hora Extra e Banco de Horas

        public virtual Boolean Teto_Diario_HoraExtra { get; set; }
        public virtual Double? Horas_Diarias { get; set; }
        public virtual Boolean Utiliza_Banco_Horas { get; set; }
        public virtual Double? Horas_Extras_Periodo_Vigente { get; set; }
        public virtual Boolean Trabalha_Compensacao { get; set; }
        public virtual Boolean Qtd_Horas_Sobreaviso_Extras { get; set; }
        public virtual Boolean Considera_Intervalo_HoraExtra_Atraso { get; set; }
        public virtual Tipo_Tolerancia? Tolerancia_Atraso_Hora_Extra { get; set; }
        public virtual Boolean Enviar_Banco_Horas { get; set; }
        public virtual Int32 Base_Conversao_Hora_Extra { get; set; }
        public virtual Enum_Forma_Compensacao? Forma_Compensacao_Horas { get; set; }
        public virtual Ordem_Compensacao? Ordem_Compensacao { get; set; }
        public virtual Boolean Utiliza_tolerancia_Horas { get; set; }

        #endregion

        public virtual Boolean TrabalhaNoDia(DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Sunday:
                    return Opcao_Trabalha_Domingo;
                case DayOfWeek.Monday:
                    return Opcao_Trabalha_Segunda;
                case DayOfWeek.Tuesday:
                    return Opcao_Trabalha_Terca;
                case DayOfWeek.Wednesday:
                    return Opcao_Trabalha_Quarta;
                case DayOfWeek.Thursday:
                    return Opcao_Trabalha_Quinta;
                case DayOfWeek.Friday:
                    return Opcao_Trabalha_Sexta;
                case DayOfWeek.Saturday:
                    return Opcao_Trabalha_Sabado;
                default:
                    return Opcao_Trabalha_Feriado;
            }
        }

        public virtual async Task<List<Tb_Pt_Faixa_He_Jornada>> GetFaixasHoraExtraOrdenadas()
        {
            List<Tb_Pt_Faixa_He_Jornada> listFaixaHe = new List<Tb_Pt_Faixa_He_Jornada>();

            using (ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            using (var repoFaixaHe = new Sinapse.DAO.repository.Conexao<Tb_Pt_Faixa_He_Jornada>(session))
            {
                listFaixaHe = (await repoFaixaHe.select(p => p.Jornada.Id == this.Id)).ToList();
            }

            for (int i = 0; i < listFaixaHe.Count; i++)
            {
                if (i == 0)
                {
                    listFaixaHe[i] = listFaixaHe.First(p => p.Percentual == listFaixaHe.Min(u => u.Percentual));
                }
                else
                {
                    listFaixaHe[i] = listFaixaHe.First(p => p.Percentual == listFaixaHe[i - 1].Percentual_Apos);
                }
            }

            return listFaixaHe;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Sigla, Descricao);
        }
    }
}