using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Requisicao_SequenciaRmdmAniel
    {
        public virtual String Seqrmdm { get; set; }
        public virtual String Codct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual EquipeAniel Equipe { get; set; }
        public virtual String Seqtransf { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual DateTime? Data { get; set; }
        public virtual Int64? Cod_CCusto { get; set; }
        public virtual Char? Tipo_Movimento { get; set; }
        public virtual String Observacao { get; set; }
        public virtual Char? Baixada { get; set; }
        public virtual Char? Filtro { get; set; }
        public virtual Byte?[] Historico { get; set; }
        public virtual DateTime? Data_Impressao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual String Usuario_Autoriz { get; set; }
        public virtual DateTime? Dia_Autoriz { get; set; }
        public virtual DateTime? Hora_Autoriz { get; set; }
        public virtual String Seqrmdm_Vinc { get; set; }
        public virtual Int64? Cod_Regiao { get; set; }
        public virtual Int64? Id_Delegacia { get; set; }
        public virtual String BO { get; set; }
        public virtual Char? Liberar_Impressao { get; set; }
        public virtual Int64? Cod_Motivo_N_Atend { get; set; }
        public virtual Int64? Alteracao_Capa { get; set; }
        public virtual Int64? Alteracao_Item { get; set; }
        public virtual Int64? Recurso_Origem { get; set; }
        public virtual Int64? Id_Solicitacao_Material { get; set; }
        public virtual String Cod_Superv { get; set; }
        public virtual DateTime Data_Excluiu_Mat { get; set; }
        public virtual Int64? Id_Atendimento_Estoque { get; set; }
        public virtual Int64? Id_Apropriacao_Baixa { get; set; }
        public virtual Int64? Id_Moviment_Type { get; set; }
        public virtual Char? Cancelado { get; set; }
        public virtual DateTime? DataHora_Reportagem { get; set; }
        public virtual String Usuario_Reportagem { get; set; }
        public virtual String Motivo_Reportagem { get; set; }
        public virtual Int64? Id_Motivo_Cancelamento { get; set; }
        public virtual String Seqrmdm_Dm_Vinc { get; set; }
        public virtual Int64? Cod_Local_Entrega { get; set; }
        public virtual String Codct_Operacional { get; set; }
        public virtual String Projeto_Operacional { get; set; }
        public virtual Char? Assinado { get; set; }
        public virtual Int64? Status_Fedex_So { get; set; }
        public virtual String Num_Lanc_Fedex_So { get; set; }
        public virtual Int64? Status_Fedex_Po { get; set; }
        public virtual String Num_Lanc_Fedex_Po { get; set; }
        public virtual Int64? Id_Planejamento_Rm { get; set; }
        public virtual Int64? Id_Nf_Comodato { get; set; }
        public virtual Char? Confirm_Comparece { get; set; }
        public virtual String QrCode { get; set; }
        public virtual String Nome_Locker { get; set; }
        public virtual Int64? Id_Locker { get; set; }
        public virtual String Box { get; set; }
        public virtual Int64? Status_Box_Locker { get; set; }
        public virtual Int64? Seq_Locker { get; set; }
        public virtual String QrCode_Locker_Retirada { get; set; }
        public virtual String QrCode_Locker { get; set; }
        public virtual DateTime? Data_Locker_Deposito { get; set; }
        public virtual DateTime? Hora_Locker_Deposito { get; set; }
        public virtual DateTime? Data_Locker_Retirada { get; set; }
        public virtual DateTime? Hora_Locker_Retirada { get; set; }
        public virtual DateTime? Data_Locker_Reservou { get; set; }
        public virtual DateTime? Hora_Locker_Reservou { get; set; }
        public virtual String Usuario_Locker_Reservou { get; set; }
        public virtual String Usuario_Locker_Deposito { get; set; }
        public virtual String Usuario_Locker_Retirada { get; set; }
        public virtual Int64? Retorno_Retirada { get; set; }
        public virtual String URL_Aceite { get; set; }
        public virtual String URL_Recusa { get; set; }
        [NotMapped]
        public virtual String ObservacaoGenerecia { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Requisicao_SequenciaRmdmAniel other = obj as Requisicao_SequenciaRmdmAniel;
            if (other == null)
                return false;
            if (Codct == other.Codct && Projeto == other.Projeto)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


}
