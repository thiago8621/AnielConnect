using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class CircuitoTelefoniaMap : ClassMapBase<CircuitoTelefonia>
    {
        public CircuitoTelefoniaMap() : base()
        {
            Table("TB_CIRCUITO_TELEFONIA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CIRCUITO_TELEFONIA");
            Map(u => u.Id_Venda, "ID_VENDA");
            
            References(m => m.RotaTelefonia, "ID_ROTA_TELEFONIA").ForeignKey("FK_CIRCUITO_ROTA_TELEFONIA");
            References(m => m.Clientes_PlanosProdutos, "ID_CLIENTE_PLANO_PRODUTO").ForeignKey("FK_CIRCUITO_TELEFONIA_CPP");

            HasMany(m => m.Did).KeyColumn("ID_CIRCUITO_TELEFONIA").Cascade.All().Inverse().Not.LazyLoad();

            Map(m => m.Status, "STATUS").CustomType<Status_Default>();
            Map(m => m.Status_Circuito, "STATUS_CIRUCUITO").CustomType<Status_PlanoTelefonia>();
            Map(m => m.TipoDiscagem, "TIPO_DISCAGEM").CustomType<TipoDiscagem>();
            Map(m => m.Senha, "SENHA");
            Map(m => m.Numero_Sip, "Numero_SIP").Length(25);
            Map(m => m.HabilitarVc1, "HABILITAR_VC1");
            Map(m => m.HabilitarVc2, "HABILITAR_VC2");
            Map(m => m.HabilitarVc3, "HABILITAR_VC3");
            Map(m => m.HabilitarBina, "HABILITAR_BINA");
            Map(m => m.HabilitarConsumoMinimo, "HABILITAR_CONSUMO_MINIMO");
            Map(m => m.HabilitarLc, "HABILITAR_LC");
            Map(m => m.HabilitarLdn, "HABILITAR_LDN");
            Map(m => m.BloqueioChamadaCobrar, "BLOQUEIO_CHAMADA_COBRAR");
            Map(m => m.CodigoAuxiliar, "CODIGO_AUXILIAR");
            Map(m => m.QuantidadeLigacoesSimultaneas, "QTD_LIGACOES_SIMULTANEAS");
        }
    }
}
