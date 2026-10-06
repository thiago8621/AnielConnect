using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.CodigoEncerramento;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq.Visitors;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.enumerado;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class TarefaAnielApiController : BaseApiController
    {
        public async Task Gravar_Obra(Area_Manutencao area, ISession sessionT5/*, System.Data.Common.DbTransaction trans*/)
        {

            try
            {
                using (ConexaoMongo mongoDB = new ConexaoMongo())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoPais = new Conexao<Pais>(sessionConnect))

                using (var repoDocProdMongo = new RepositoryMongo<DocumentoProducaoMongo>(mongoDB))
                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
                using (var repoElements = new Conexao<Area_Manutencao_Elementos>(sessionT5))

                using (var repoImovel = new Conexao<Tipo_ImovelAniel>(sessionT5))
                using (var repoUnidadeCusto = new Conexao<Custos_TarefaAniel>(sessionT5))
                using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var repoTarefa = new Conexao<TarefaAniel>(sessionT5))


                using (var repoEmpresa = new Conexao<EmpresaAniel>(sessionT5))
                using (var repoVenda = new Conexao<Venda>(sessionConnect))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                using (var docProducaoApi = new Documento_ProducaoApiController())
                {

                    Dictionary<String, Object> parametros;
                    List<Documento_ProducaoAniel> Docs = new List<Documento_ProducaoAniel>();
                    List<TarefaAniel> Tarefas = new List<TarefaAniel>();

                    //Ordem_ServicoAniel OS = new Ordem_ServicoAniel
                    //{
                    //    CodOs = await docProducaoApi.GetNum_Obra(sessionConnect, repoDocProd),
                    //    CodCt = area.CodCt,
                    //    Projeto = area.Projeto,
                    //    InicPrev = area.Data_HoraInicial,
                    //    TermPrev = area.Data_HoraFinal,
                    //    Equipe = null,
                    //    InicReal = null,
                    //    TermReal = null,
                    //};

                    var empresa = await repoEmpresa.selectFirst(p => p.CodCt == area.CodCt);

                    Custos_TarefaAniel Unid_Custo = (await repoUnidadeCusto.selectFirst(p => p.CodCt == area.CodCt));
                    (string tarefa, Int64 sequencia) Gen_tarefa = (await Get_StringTarefa(sessionT5));
                    this.BindDictionaryTarefa(out parametros, area.CodCt,
                                                              area.Projeto,
                                                              empresa?.CodEmpresa,
                                                              Unid_Custo.Unidade_Custo,
                                                              area.Data_HoraInicial.Value,
                                                              area.Data_HoraFinal.Value,
                                                              Gen_tarefa.tarefa);

                    await this.Gravar_ManutencaoOT_OR(sessionT5, parametros);

                    List<Area_Manutencao_Elementos> elementosAfetados = (await repoElements.select(p => p.AreaManutencao == area)).ToList();

                    var fromManutencao = elementosAfetados.Select(async (afetado, index) =>
                    {
                        var cliplanProd = await repoClientePlanoProduto.selectFirst(p => p.Caixa_Ftth_Reservado == Convert.ToString(afetado.Elemento_de_Rede.Id));


                        //using (var trans = sessionAniel.Connection.BeginTransaction())                       


                        Documento_ProducaoAniel docModel = new Documento_ProducaoAniel
                        {
                            CPF_CLIENTE = cliplanProd?.Cliente?.CpfCnpj,
                            // Campos Adicionados Pós comparação
                            Latitude_Str = Convert.ToString(cliplanProd?.Endereco_Instalacao?.Latitude),
                            Longitude_Str = Convert.ToString(cliplanProd?.Endereco_Instalacao?.Longitude),
                            Devolvida = GetStringBoolean(false),
                            Pais = (await repoPais.selectFirst()).Nome,
                            UF = afetado?.Elemento_de_Rede.UF,
                            Cidade = afetado?.Elemento_de_Rede?.Cidade,
                            Bairro = afetado?.Elemento_de_Rede?.Bairro,
                            Cep = afetado?.Elemento_de_Rede?.CEP,
                            Complemento = afetado?.Elemento_de_Rede?.Complemento,
                            Data_Prod = DateTime.Now,
                            Hora_Prod = DateTime.Now,
                            Cod_Sub_Tipo_Serv = "1",
                            OXBDPEE = cliplanProd?.ClienteContrato?.NumeroContrato,
                            Usuario = StringService.Truncate(User.Identity.GetUserName(), 10),
                            //Equipe=,

                            //Logradouro=afetado?.Elemento_de_Rede?.,

                            // End campos Adicionados Pós comparação
                            Num_Doc = index,
                            Projeto = area.Projeto,
                            CodCt = area.CodCt,
                            TelefonePrincipal = cliplanProd?.Cliente?.Telefone_Celular,
                            Nome_Cliente = cliplanProd?.Cliente?.NomeFantasia,
                            Observacao = "",
                            Origem_Documento = Origem_Documento_Producao.Connect,
                            Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                            Cod_Escritorio = cliplanProd?.Cod_Escritorio,
                            Cod_Area = cliplanProd?.Cod_Area,
                            Cod_Localidade = cliplanProd?.Cod_Localidade,
                            Id_Venda = cliplanProd?.Id_Venda,
                            Num_Obra = Gen_tarefa.tarefa,
                            Unidade_Custo = Unid_Custo.Unidade_Custo,
                            Dia = DateTime.Now,
                            Hora = DateTime.Now,
                            //Cod_Tipo_Serv =,
                            Sigla_Modulo = "MCO",
                            //Tarefa = Gen_tarefa.tarefa,
                            //CaixaFTTH = cliplanProd?.Caixa_Ftth_Reservado,
                            //PortaFTTH = cliplanProd?.Porta_Ftth_Reservado,
                            Numero_Obra_Original = Gen_tarefa.tarefa,
                            Id_PlanoProduto = cliplanProd?.Plano?.Id,
                            Id_Cliente_PlanoProdutoConnect = cliplanProd?.Id,
                            Logradouro = cliplanProd?.Endereco_Instalacao?.Logradouro,
                            CpfInstalacao = cliplanProd?.Cliente?.CpfCnpj,
                        };

                        TarefaAniel tarefa = new TarefaAniel
                        {
                            Tarefa = StringService.Truncate(Gen_tarefa.tarefa, 15),
                            Projeto = StringService.Truncate(docModel.Projeto, 20),
                            CodCt = StringService.Truncate(docModel.CodCt, 4),
                            Mobile = GetStringBoolean(true),
                            CodOs = StringService.Truncate(docModel.Numero_Obra_Original, 13),
                            Num_Obra = StringService.Truncate(docModel.Num_Obra, 50),
                            Num_Doc = docModel.Num_Doc,
                            Usuario = StringService.Truncate(User.Identity.GetUserName(), 10),
                            Dia = DateTime.Now,
                            Hora = DateTime.Now,
                            
                        };
                        return (docModel, tarefa);

                    }).Select(p => p.Result).ToList();

                    await repoDocProd.SaveOrUpdate(fromManutencao.Select(p => p.docModel));

                    foreach (var item in fromManutencao.Select(p => p.tarefa))
                    {
                        await repoTarefa.SaveOrUpdate(item);
                    }

                }
            }
            catch (Exception)
            {

            }
        }

        private async Task<(String tarefa, long sequencia)> Get_StringTarefa(ISession session)
        {
            (string tarefa, long seq) retorno = (String.Empty, 0);
            StringBuilder sb = new StringBuilder();

            using (var com = session.Connection.CreateCommand())
            using (var repoTarefaAniel = new Conexao<TarefaAniel>(session))
            {
                sb.AppendLine("SELECT (SELECT RETORNO FROM EXTRACT_INTEGER(MAX(A.TAREFA))) AS NUM_TAREFA FROM TB_TAREFA A ");
                using (var reader = await CommandHelper.ExecuteReader(com, sb, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        //var RDO = Convert.ToString(reader["TAREFA"]);

                        retorno.seq = Convert.ToInt64(reader["TAREFA"]) + 1;
                        retorno.tarefa = $"RDO-{String.Format("{0:D11}", retorno.seq)}";
                    }
                }
            }

            return retorno;
        }

        private void BindDictionaryTarefa(out Dictionary<String, Object> parametros, params Object[] values)
        {
            parametros = new Dictionary<String, Object>();
            try
            {
                parametros.Add("@CODCT", values[0]);
                parametros.Add("@PROJETO", values[1]);

                parametros.Add("@UNIDADE_CUSTO", values[2]);

                parametros.Add("@INICPREV", values[3]);
                parametros.Add("@TERMPREV", values[4]);

                parametros.Add("@NUM_OT_OR", values[5]);

            }
            catch (Exception)
            {
            }
        }

        private async Task Gravar_ManutencaoOT_OR(ISession sessionT5, Dictionary<string, object> parametros)
        {
            try
            {
                var sql = new StringBuilder();
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    sql.AppendLine(" UPDATE OR INSERT INTO TB_MANUTENCAO_OT_OR (")

                       .AppendLine(" CODCT, PROJETO, UNIDADE_CUSTO, NUM_EMPRESA, INICPREV, TERMPREV, NUM_OT_OR, DIA, HORA)")
                       .AppendLine(" VALUES(@CODCT, @PROJETO, @UNIDADE_CUSTO, @CODEMPRESA, @INICPREV, @TERMPREV, @NUM_OT_OR, CURRENT_DATA, CURRENT TIME)");


                    //Criação do registro na tb_manutencao_ot_or
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);

                }
            }
            catch (Exception)
            {
            }

        }

        private String GetStringBoolean(bool factor)
        {
            if (factor)
            {
                return "S";
            }
            else
            {
                return "N";
            }
        }
    


    }
}