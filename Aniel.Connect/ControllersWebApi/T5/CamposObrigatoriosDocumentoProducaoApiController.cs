using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class CamposObrigatoriosDocumentoProducaoApiController : BaseApiController
    {
        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/CamposObrigatoriosDocumentoProducaoApi/GetAll")]
        public async Task<DataTableResponse<CamposObrigatoriosDocumento_ProducaoModels>> GetAll(CamposObrigatoriosDataTableRequest request)
        {

            List<CamposObrigatoriosDocumento_ProducaoModels> retorno = new List<CamposObrigatoriosDocumento_ProducaoModels>();
            IOrderedEnumerable<CamposObrigatoriosDocumento_ProducaoModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CAMPO, A.VISIVEL, ")
                    .AppendLine("A.OBRIGATORIO, A.TITULO ")
                    .AppendLine(" FROM TB_CAMPOS_OBRIG_DOC_PROD A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {

                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new CamposObrigatoriosDocumento_ProducaoModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Campo = (CamposObrigatoriosDocumentoProducaoEnum)Enum.Parse(typeof(CamposObrigatoriosDocumentoProducaoEnum), Convert.ToString(reader["CAMPO"])),
                                Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1,
                                Titulo = Convert.ToString(reader["TITULO"])
                            };

                            obj.CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosDocumentoProducaoEnum>(obj.Campo.ToString());

                            retorno.Add(obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.Where(p => !String.IsNullOrEmpty(p.CampoString)).OrderBy(p => p.CampoString);
            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<CamposObrigatoriosDocumento_ProducaoModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/CamposObrigatoriosDocumentoProducao/busca")]
        public async Task<List<CamposObrigatoriosDocumento_ProducaoModels>> GetCampos()
        {
            List<CamposObrigatoriosDocumento_ProducaoModels> retorno = new List<CamposObrigatoriosDocumento_ProducaoModels>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CAMPO, A.VISIVEL, ")
                           .AppendLine("A.OBRIGATORIO, A.TITULO ")
                           .AppendLine("FROM TB_CAMPOS_OBRIG_DOC_PROD A ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new CamposObrigatoriosDocumento_ProducaoModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Campo = (CamposObrigatoriosDocumentoProducaoEnum)Enum.Parse(typeof(CamposObrigatoriosDocumentoProducaoEnum), Convert.ToString(reader["CAMPO"])),
                                    CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosDocumentoProducaoEnum>(Convert.ToString(reader["CAMPO"])),
                                    Obrigatorio = Convert.ToInt32(reader["OBRIGATORIO"]) == 1,
                                    Bloqueado = Convert.ToInt32(reader["VISIVEL"]) == 1,
                                    Titulo = Convert.ToString(reader["TITULO"])
                                });
                            }
                        }
                    }


                    //Inserir Enums Novos
                    foreach (String enumerado in Enum.GetNames(typeof(CamposObrigatoriosDocumentoProducaoEnum)))
                    {
                        var tmp = (CamposObrigatoriosDocumentoProducaoEnum)Enum.Parse(typeof(CamposObrigatoriosDocumentoProducaoEnum), enumerado);
                        if (retorno.Find(p => p.Campo == tmp) == null)
                        {
                            retorno.Add(new CamposObrigatoriosDocumento_ProducaoModels
                            {
                                Campo = tmp,
                                Obrigatorio = false,
                                Bloqueado = false,
                                CampoString = HtmlEnumExtensions.GetEnumDescription<CamposObrigatoriosDocumentoProducaoEnum>(tmp)
                            });
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        public static Boolean CampoBloqueado(List<CamposObrigatoriosDocumento_ProducaoModels> campos, CamposObrigatoriosDocumentoProducaoEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo == null)
                return false;
            else
                return _campo.Bloqueado;
        }

        public static String CampoTitulo(List<CamposObrigatoriosDocumento_ProducaoModels> campos, CamposObrigatoriosDocumentoProducaoEnum campo)
        {
            var _campo = campos.Find(p => p.Campo == campo);
            if (_campo == null)
                return null;
            else
                return _campo.Titulo;
        }


        [HttpPost()]
        [Route("api/CamposObrigatoriosDocumentoProducao/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(CamposObrigatoriosListModel value, ISession sessionT5)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (var repositoryT5 = new Conexao<CamposObrigatoriosDocumentoProducao>(sessionT5))
                {
                    //var listaDocumentoProducao = (await repository.select()).ToList();
                    var listaDocumentoProducao = (await repositoryT5.select()).ToList();

                    foreach (var objView in value.Campos_Documento_Producao)
                    {
                        var obj = listaDocumentoProducao.Find(p => p.Campo == objView.Campo);
                        if (obj != null)
                        {
                            obj.UsuarioAlteracao = User.Identity.GetUserId();
                            obj.DataHoraAlteracao = DateTime.Now;

                            obj.Obrigatorio = objView.Obrigatorio;
                            obj.Bloqueado = objView.Bloqueado;
                            obj.Titulo = objView.Titulo;
                        }
                        else
                        {
                            obj = new CamposObrigatoriosDocumentoProducao
                            {
                                Campo = objView.Campo,
                                Bloqueado = objView.Bloqueado,
                                Obrigatorio = objView.Obrigatorio,
                                Titulo = objView.Titulo,

                                UsuarioCriacao = User.Identity.GetUserId(),
                                DataHoraCriacao = DateTime.Now
                            };
                        }

                        await repositoryT5.SaveOrUpdate(obj);
                    }
                }

                retorno.IdObjeto = 1;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        [HttpPost()]
        [Route("api/CamposObrigatoriosDocumentoProducao/ValidacaoCamposObrigatoriosOS")]
        public async Task<PostApiRetorno> ValidacaoCamposObrigatoriosOS(long atendimento)
        {
            var retorno = new PostApiRetorno();            
            try
            {
                using (var atendApi = new AtendimentoApiController())
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoPP = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(session))
                {
                    var record = (await atendApi.FindById(atendimento)).Objeto;
                    var recordCliente = (await atendApi.FindById(atendimento)).Objeto;
                    var recordPP = await repoPP.selectFirst(p => p.Id == record.Id_Cliente_Plano_Produto);
                    var elementoRede = (await repoPorta.selectFirst(p => p.Id == recordPP.Id_ElementoRede_Porta))?.Caixa;
                    var camposResultValidacao = new Dictionary<CamposObrigatoriosDocumento_ProducaoModels, bool>();

                    var camposObrigatorios =  this.GetCampos().Result.Where(p => p.Obrigatorio).ToList();

                    if (camposObrigatorios.Any(p => p.Obrigatorio) && recordPP != null && record != null)
                    {
                        var listagem = camposObrigatorios;
                        #region camposObrigatoriosEnum
                        /* SUBTIPO_SERVICO
                           SEGMENTO
                           OBSERVACAO
                           ADCIONAL_1
                           ADCIONAL_2
                           ADCIONAL_3
                           ADCIONAL_4
                           ADCIONAL_5
                           ADCIONAL_6
                           ADCIONAL_7
                           ADCIONAL_8
                           ADCIONAL_9
                           ADCIONAL_10
                           POP
                           OLT
                           ELEMENTO_REDE
                           DEPARTAMENTO
                           PRIORIDADE
                           CEP_MANUTENCAO
                           ENDERECO_MANUTENCAO
                           NUMERO_MANUTENCAO
                           UF_MANUTENCAO
                           CIDADE_MANUTENCAO
                           BAIRRO_MANUTENCAO
                           PAIS
                           TIPO_IMOVEL
                           COMPLEMENTO_MANUTENCAO
                           PONTO_REFERENCIA_MANUTENCAO
                           LATITUDE
                           LONGITUDE
                           AREA
                           LOCALIDADE
                           ORIGEM
                           NOME_CONTATO_INSTALACAO
                           CPF_CONTATO_INSTALACAO
                           TELEFONE_CONTATO_INSTALACAO
                           PRIORIDADE_CLIENTE
                        */
                        #endregion
                        foreach (CamposObrigatoriosDocumento_ProducaoModels campoObrigatorio in camposObrigatorios) 
                        {
                            bool comparacao = false;

                            if (!campoObrigatorio.Obrigatorio)
                                continue;

                            switch (campoObrigatorio.Campo)
                            {
                                case CamposObrigatoriosDocumentoProducaoEnum.PONTO_REFERENCIA_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Ponto_Referencia); 
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.PRIORIDADE_CLIENTE:
                                    comparacao = recordPP.Cliente.Categoria_Cliente.Tipo_Prioridade.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.LATITUDE:
                                    comparacao = recordPP.Endereco_Instalacao.Latitude.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.LONGITUDE:
                                    comparacao = recordPP.Endereco_Instalacao.Longitude.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.GRUPO_AREA:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Cod_Escritorio);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.AREA:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Cod_Area);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.COMPLEMENTO_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Complemento);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.LOCALIDADE:
                                    comparacao = recordPP.Cod_Localidade.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.PRIORIDADE:
                                    comparacao = record.Prioridade.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.DEPARTAMENTO:
                                    comparacao = recordPP.Cliente.Departamento != null;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.ELEMENTO_REDE:
                                    comparacao = elementoRede.Id != 0;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.PAIS:
                                    comparacao = recordPP.Endereco_Instalacao.Pais != null;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.CIDADE_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Cidade);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.BAIRRO_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Bairro);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.CEP_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Cep);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;

                                case CamposObrigatoriosDocumentoProducaoEnum.NUMERO_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.Numero);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;

                                case CamposObrigatoriosDocumentoProducaoEnum.UF_MANUTENCAO:
                                    comparacao = !string.IsNullOrEmpty(recordPP.Endereco_Instalacao.UF);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.TELEFONE_CONTATO_INSTALACAO:                                     
                                    comparacao = !string.IsNullOrEmpty(record.Telefone_Celular);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.NOME_CONTATO_INSTALACAO:
                                    comparacao = !string.IsNullOrEmpty(record.Pessoa_Contato);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.CPF_CONTATO_INSTALACAO:
                                    comparacao = !string.IsNullOrEmpty(record.CPF_CNPJ_Cliente);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.ORIGEM:
                                    comparacao = record.Id_Origem_Atendimento.HasValue;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.POP:
                                    comparacao = !string.IsNullOrEmpty(record.POP);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                case CamposObrigatoriosDocumentoProducaoEnum.OLT:
                                    comparacao = !string.IsNullOrEmpty(record.OLT);
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                                default: 
                                    comparacao = false;
                                    if (!comparacao)
                                        camposResultValidacao.Add(campoObrigatorio, comparacao);
                                    break;
                            }                            
                        }

                        retorno.Ok = false;
                        if (camposResultValidacao.Values.All(p => p == true))
                        {
                            retorno.Ok = true;
                        }
                        else 
                        {                          
                            foreach (var item in camposResultValidacao.Where(x => x.Value == false))
                                retorno.Mensagens.Add(new PostApiMensagem { Mensagem = $"{item.Key.CampoString}", Tipo = PostApiMensagemType.Erro });

                            retorno.Ok = false;
                            return retorno;
                        }
                    }
                    else
                    {
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex?.InnerException != null ? $"{ex.Message} - {ex.InnerException.Message}" : ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }
}