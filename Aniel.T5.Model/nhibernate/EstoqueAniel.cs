using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class EstoqueAniel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String CodCpl { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual String CodOs { get; set; }
        public virtual String CodLib { get; set; }
        public virtual String SeqLivre { get; set; }
        public virtual String SeqRmdm { get; set; }
        public virtual String SeqTransf { get; set; }
        public virtual String Pedido { get; set; }
        public virtual String ProjOrigem { get; set; }
        public virtual String ProjDestino { get; set; }
        public virtual Int64 Entrada { get; set; }
        public virtual Int64 Saida { get; set; }
        public virtual String SeqDevolucao { get; set; }
        public virtual String Rir { get; set; }
        public virtual String CtOrigem { get; set; }
        public virtual String Empresa_Origem { get; set; }
        public virtual String CtDestino { get; set; }
        public virtual String Empresa_Destino { get; set; }
        public virtual String Equipe { get; set; }
        public virtual Int64? Cod_CCusto { get; set; }
        public virtual String Cod_Fornecedor { get; set; }
        public virtual Char? Transferencia { get; set; }
        public virtual String NotaFiscal { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Observacao { get; set; }
        public virtual Double? Valor_Compra { get; set; }
        public virtual Double? Limite_Saida { get; set; }
        public virtual Int64? Qtde_Tarefa { get; set; }
        public virtual Int64? Saldo_Dia { get; set; }
        public virtual Char? Tipo_Atividade { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual String Integracao { get; set; }
        public virtual Char? Status { get; set; }
        public virtual Int64? Cod_Local_Entrega { get; set; }
        public virtual Int64? Sequencial { get; set; }
        public virtual Int64? Cod_Tipo_Movto { get; set; }
        public virtual String Seq_Contabil { get; set; }
        public virtual Char? OP { get; set; }
        public virtual Int64? CodEmpresa { get; set; }
        public virtual Char? Locado { get; set; }
        public virtual Char? Lancado_Contabil { get; set; }
        public virtual Char? Valor_Manual { get; set; }
        public virtual String Cod_Receb_Fisico { get; set; }
        public virtual DateTime? Hora_Atend { get; set; }
        public virtual Char? Consolida_NF { get; set; }
        public virtual Double? Valor_Total { get; set; }
        public virtual Int64? Tipo_Transferencia { get; set; }
        public virtual Char? Selecionar { get; set; }
        public virtual Int64? Num_Doc { get; set; }
        public virtual Int64? Id_Receb_Item { get; set; }
        public virtual Int64? Id_Fechamento_Fornecedor { get; set; }
        public virtual Char? Calcula_Precisao { get; set; }
        public virtual Int64? Id_Inv_Fisico { get; set; }
        public virtual Int64? Id_Inv_Equipe { get; set; }
        public virtual Double? Diferenca { get; set; }
        public virtual Int64? Processado_Portal { get; set; }
        public virtual Char? INS { get; set; }
        public virtual Char? AC_IE { get; set; }
        public virtual Int64? Id_Lei_Icms { get; set; }
        public virtual Int64? Id_Lei_Ipi { get; set; }
        public virtual Int64? Id_Lei_Confins { get; set; }
        public virtual Int64? Id_Lei_Pis { get; set; }
        public virtual Int64? Id_Nf_Type { get; set; }
        public virtual String Retem_Icms_Iss { get; set; }
        public virtual DateTime? Data_Emissao_Nf { get; set; }
        public virtual Int64? Qtde_Volumes_Recebidos { get; set; }
        public virtual Int64? Peso_Bruto_Volumes { get; set; }
        public virtual Int64? Peso_Liquido_Volumes { get; set; }
        public virtual String CFOP { get; set; }
        public virtual String Tipo_Item { get; set; }
        public virtual String Serie_Nf { get; set; }
        public virtual Int64? Origem_Material { get; set; }
        public virtual Double? Valor_Descontar_Impostos { get; set; }
        public virtual Char? Saldo_Revisado { get; set; }
        public virtual Char? Audita_Sucata { get; set; }
        public virtual Char? Recuperado { get; set; }
        public virtual Int64? Seq_Insert { get; set; }
        public virtual String Observacao_Bkp { get; set; }
        public virtual Int64? Id_Atendimento_Estoque { get; set; }
        public virtual Int64? Id_Saldo_Auto { get; set; }
        public virtual Int64? Num_Contrato_Cliente { get; set; }
        public virtual String Chave_Acesso { get; set; }
        public virtual Int64? Id_Conta_Financeira { get; set; }
        public virtual String Conta_Financeira { get; set; }
        public virtual Int64? Id_Contas_Pagar { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EstoqueAniel other = obj as EstoqueAniel;
            if (other == null)
                return false;
            if (CodCpl == other.CodCpl && CodCt == other.CodCt && CodMat == other.CodMat && Data == other.Data && Entrada == other.Entrada && 
                NotaFiscal == other.NotaFiscal && Pedido == other.Pedido && ProjDestino == other.ProjDestino && Projeto == other.Projeto && ProjOrigem == other.ProjOrigem && 
                Saida == other.Saida && SeqLivre == other.SeqLivre && SeqRmdm == other.SeqRmdm && SeqTransf == other.SeqTransf)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodCpl + "|" + CodCt + "|" + CodMat + "|" + Data + "|" + Entrada + "|" + NotaFiscal + "|" + Pedido + "|" + ProjDestino + "|" + Projeto + "|" + ProjOrigem + "|" + Saida + "|" + SeqLivre + "|" + SeqRmdm + "|" + SeqTransf).GetHashCode();
        }
    }
}
