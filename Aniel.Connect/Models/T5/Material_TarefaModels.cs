using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    public class Material_TarefaListaModel
    {
        public String Codigo { get; set; }
        public String Descricao { get; set; }
        public String Equipe { get; set; }

        public String Codigo_Complementar { get; set; }
    }

    public class Material_AplicadoListaModel
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public Int32? Status_Auditoria { get; set; }


        public Boolean Permitir_TI_Automatica { get; set; }
        public String CodigoTmp { get; set; }

        public String Tarefa { get; set; }
        public String Codigo { get; set; }

        public String Descricao { get; set; }
        public String Codigo_Complementar { get; set; }
        public String Unidade { get; set; }
        public String Equipe { get; set; }
        public String Valor_Unitario { get; set; }
        public String Quantidade { get; set; }
        public String Valor_Total { get; set; }
        public Boolean Exibir_Material { get; set; }
        public String Data_Aplic { get; set; }
        public Boolean Retirada { get; set; }
    }

    public class Material_Tarefa_DataTableRequest : DataTableRequest
    {
        public Int32 Id_Cliente { get; set; }
        public Int32 Id_ClientePlanoProduto { get; set; }
    }

    public class Material_Aplicado_DataTableRequest : DataTableRequest
    {
        public Int32 Id_Cliente { get; set; }
        public String Projeto { get; set; }
        public String CodCt { get; set; }
        public String Num_Obra { get; set; }
        public String Num_Doc { get; set; }
        public Boolean Retirada { get; set; }
        public Boolean Exibir_Vinculado { get; set; }

    }



    public class RelatorioMateriaisAnaliticosModel
    {
        public string Contrato_aniel { get; set; }
        public string Projeto { get; set; }
        public string Contrato_cliente { get; set; }
        public string Cod_equipe { get; set; }
        public string Nome_equipe { get; set; }
        public string Cpf_cnpj_cliente { get; set; }
        public string Nome_cliente { get; set; }
        public string Id_cliente { get; set; }
        public string Os { get; set; }
        public string Status_os { get; set; }
        public string Cod_encerramento { get; set; }
        public string Descricao_encerramento { get; set; }
        public string Data_servico { get; set; }
        public string Cod_tipo_serv { get; set; }
        public string Desc_tipo_serv { get; set; }
        public string Cod_sub_tipo_serv { get; set; }
        public string desc_sub_tipo_serv { get; set; }
        public string Segmento { get; set; }
        public string Cod_material { get; set; }
        public string Nome_material { get; set; }
        public string Unidade { get; set; }
        public string Num_serie { get; set; }
        public string Qtde_aplicada { get; set; }
        public string Qtde_removida { get; set; }
        public string Valor_unitario { get; set; }
        public string Total_aplicado { get; set; }
        public string Total_removido { get; set; }
        public string Hora { get; set; }
        public string Data { get; set; }
        public string Usuario { get; set; }
    }

    public class Relatorio_Materiais_Analiticos_DataTableRequest : DataTableRequest
    {
        public String Equipe { get; set; }
        public String Contrato { get; set; }
        public String Projeto { get; set; }
        public Int64? Id_Categoria_Cliente { get; set; }
        public Int32? Cod_Tipo_Serv { get; set; }
        public Int64? Id_NaturezaProducao { get; set; }
        public Int64? Cod_Cliente { get; set; }
        public String Supervisor { get; set; }
        public String Data_Inicio { get; set; }
        public String Data_Fim { get; set; }
        public String Tipo_Data { get; set; }
        public String CodMat { get; set; }
        public ClassificacaoMaterial? ClassificacaoMaterial { get; set; }
        public String FamiliaMaterial { get; set; }
        public String GrupoMaterial { get; set; }
        public String Agrupamento { get; set; }
    }
    public class Relatorio_Saldo_Cliente_DataTableRequest : DataTableRequest
    {
        public virtual String Plano { get; set; }
        public virtual String Tipo_Pessoa { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual String Cod_Tipo_Serv { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Area { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Natureza_Servico { get; set; }
        public virtual String Empresa { get; set; }
        public virtual String Filial { get; set; }

        public virtual Int64? Id_NaturezaProducao { get; set; }
        public virtual String CpfCliente { get; set; }
        public virtual String Contrato { get; set; }
   
        public virtual String Tipo_Data { get; set; }
        public virtual String CodMat { get; set; }
        public virtual ClassificacaoMaterial? ClassificacaoMaterial { get; set; }
        public virtual String FamiliaMaterial { get; set; }
        public virtual String GrupoMaterial { get; set; }
        public virtual String Agrupamento { get; set; }
        public virtual Boolean ExibirApenasSerializados { get; set; }
    }

    public class Relatorio_Saldo_POP_DataTableRequest : DataTableRequest
    {
        public virtual String Pop { get; set; }
        public virtual String Empresa { get; set; }
        public virtual String Filial { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }       
        public virtual String CodMat { get; set; }
        public virtual String FamiliaMaterial { get; set; }
        public virtual String GrupoMaterial { get; set; }
    }



    public class Relatorio_Materiais_SinteticoModel
    {
        public String cod_material { get; set; }
        public String nome_material { get; set; }
        public String unidade { get; set; }
        public String qtde_aplicada { get; set; }
        public String qtde_removida { get; set; }
        public String valor_unitario { get; set; }
        public String total_aplicado { get; set; }
        public String total_removido { get; set; }
        public String mes_ano { get; set; }
        public String cidade { get; set; }
        public String contrato_aniel { get; set; }
        public String projeto { get; set; }
        public String nome_equipe { get; set; }
        public String nome_cliente { get; set; }
        public String data_servico { get; set; }
        public String desc_tipo_serv { get; set; }
        public String media_aplicada_servico { get; set; }
        public String media_removida_servico { get; set; }
    }
     public class Relatorio_Saldo_ClienteModel
    {
        public virtual String cod_cliente { get; set; }
        public virtual String cpf { get; set; }
        public virtual String nome { get; set; }
        public virtual String codct { get; set; }
        public virtual String plano { get; set; }
        public virtual String codmat { get; set; }
        public virtual String material { get; set; }
        public virtual String codcpl { get; set; }
        public virtual String unid { get; set; }
        public virtual String saldo { get; set; }
        public virtual String cidade { get; set; }
        public virtual String uf { get; set; }
        public virtual String projeto { get; set; }
        public virtual String contrato { get; set; }
        public virtual String materialagrupamento { get; set; }
        public virtual String grupo_area { get; set; }
        public virtual String area { get; set; }
        public virtual String localidade { get; set; }
        public virtual String tiposervico { get; set; }
        public virtual String cliente { get; set; }
        public virtual String valTotal { get; set; }
        public virtual String valUnit { get; set; }



    }

    public class Relatorio_Saldo_POPModel
    {
        public virtual String pop { get; set; }
        public virtual String contrato { get; set; }
        public virtual String projeto { get; set; }
        public virtual String codmat { get; set; }
        public virtual String material { get; set; }
        public virtual String codcpl { get; set; }
        public virtual String unid { get; set; }
        public virtual String saldo { get; set; }
        public virtual String valUnit { get; set; }
        public virtual String valTotal { get; set; }

    }

    public class Material_TarefaChave
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public String Tarefa { get; set; }
        public String Equipe { get; set; }
        public String CodMat { get; set; }
        public String CodCpl { get; set; }
        public Boolean Retirada { get; set; }
    }

}