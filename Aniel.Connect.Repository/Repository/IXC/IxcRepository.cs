using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Exceptions;
using Aniel.Connect.Repository.Exceptions.Voalle;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using NHibernate.Util;
using Npgsql;
using RestSharp;
using RestSharp.Validation;
using Sinapse.DAO.repository;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace Aniel.Connect.Repository.Voalle
{
    /// <summary>
    ///     Classe para realizar requisições para a API da Voalle
    /// </summary>
    public class IxcRepository : IIxcRepository
    {
        private readonly IDocumentoProducaoAnielRepository _repoDocProd;
        private readonly Configuracoes_Gerais _config;
        private readonly string _url;
        private readonly string _token;

        public IxcRepository(string url, string token, Configuracoes_Gerais config, IDocumentoProducaoAnielRepository repoDocProd)
        {
            this._url = url;
            this._token = token;
            this._config = config;
            this._repoDocProd = repoDocProd;
        }

        public Equipe GetByMatAux(String MatAux)
        {
            StringBuilder sql = new StringBuilder();

            Equipe retorno = null;
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT FIRST 1 A.EQUIPE, A.EQUIPEAUX, A.NOME , C.CODCT_PADRAO, C.PROJETO_PADRAO, A.CODCT_INTEGRATOR, A.PROJETO_INTEGRATOR, C.LATITUDE, C.LONGITUDE ");
                sql.AppendLine("FROM TB_EQUIPE A ");
                sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ");
                sql.AppendLine("ON (A.NUM_FRE = B.NUM_FRE)");
                sql.AppendLine("LEFT JOIN TB_FILIAL C ");
                sql.AppendLine("ON (C.CODEMPRESA = B.CODEMPRESA AND C.CODFILIAL = B.CODFILIAL) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.EQUIPEAUX = @EQUIPEAUX) ");
                parametros.Add("@EQUIPEAUX", MatAux);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new Equipe()
                        {
                            Matricula = Convert.ToString(_reader["EQUIPE"]),
                            Nome = Convert.ToString(_reader["NOME"]),
                            CodCtPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["CODCT_INTEGRATOR"])) ? Convert.ToString(_reader["CODCT_PADRAO"]) : Convert.ToString(_reader["CODCT_INTEGRATOR"]),
                            ProjetoPadrao = String.IsNullOrEmpty(Convert.ToString(_reader["PROJETO_INTEGRATOR"])) ? Convert.ToString(_reader["PROJETO_PADRAO"]) : Convert.ToString(_reader["PROJETO_INTEGRATOR"]),
                            CodAux = Convert.ToString(_reader["EQUIPEAUX"]),
                            Latitude = _reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LATITUDE"]) : (double?)null,
                            Longitude = _reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LONGITUDE"]) : (double?)null,
                        };
                    }
                }
            }

            return retorno;
        }

        public Ordem_Servico GetOrdem_Servico(String Codct, String Projeto, String Num_Obra_Original, Int32 Num_Doc, DateTime DataMaxima)
        {
            StringBuilder sql = new StringBuilder();
            List<Ordem_Servico> retorno = new List<Ordem_Servico>();
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {


                sql.AppendLine("SELECT FIRST 1 A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.UNIDADE_CUSTO, A.TAREFA, A.EQUIPE, A.DATAHORA_INI_ATEND, ");
                sql.AppendLine("A.COD_ENCERRAMENTO, A.DATA_PROD, A.HORA_ENCERRAMENTO, A.COD_TIPO_SERV,A.COD_TURNO_SERV, ");
                sql.AppendLine("A.DATA_MAXIMA, A.HORA_MAXIMA,A.ENDERECO_UF, A.ENDERECO, A.ENDERECO_TIPO, A.ENDERECO_BAIRRO,A.ENDERECO_CPL,A.ENDERECO_NUMERO,A.ENDERECO_COMPL, A.RESPONSABILIDADE,");
                sql.AppendLine("A.TIPO_PRODUCAO, E.DESCRICAO AS DESC_TIPO_SERV, E.CODIGO_INTEGRATOR AS COD_TIPO_SERV_INTEGRATOR, ");
                sql.AppendLine("COALESCE(F.ENCERRA_OS_APOS_BAIXA,'S') ENCERRA_OS_APOS_BAIXA ,F.DESCRICAO AS DESC_CODIGO_ENCERRAMENTO, F.STATUS_EXECUCAO,A.STATUS_EXECUCAO AS STATUS_EXECUCAO_SERVICO, A.OBSERVACAO_AUX, A.OBSERVACAO_TESTE, A.SEQ, A.PARAMETROS, G.EQUIPEAUX,F.CODAUX,F.CODAUX2,A.PARPRIM_PARA,");
                sql.AppendLine("I.USUARIO, H.NUM_FRE ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE E ");
                sql.AppendLine("  ON(A.COD_TIPO_SERV = E.COD_TIPO_SERV) ");
                sql.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO F ");
                sql.AppendLine("  ON(A.COD_ENCERRAMENTO = F.COD_ENCERRAMENTO AND A.CODCT = F.CODCT)");
                sql.AppendLine("LEFT JOIN TB_EQUIPE G ");
                sql.AppendLine("  ON(A.EQUIPE = G.EQUIPE) ");
                sql.AppendLine("LEFT JOIN TB_FUNCIONARIO H ");
                sql.AppendLine("  ON(H.NUM_FRE = G.NUM_FRE) ");
                sql.AppendLine("LEFT JOIN TB_SENHA I ");
                sql.AppendLine("  ON(I.NUM_FRE = I.NUM_FRE) ");
                sql.AppendLine("LEFT JOIN TB_CONTRATO J ");
                sql.AppendLine("  ON(A.CODCT = J.CODCT) ");



                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.Append(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL AND A.NUM_DOC = @NUM_DOC");
                //AND DATA_MAXIMA = @DATA_MAXIMA
                parametros.Add("@CODCT", Codct);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA_ORIGINAL", Num_Obra_Original);
                parametros.Add("@NUM_DOC", Num_Doc);
                sql.AppendLine(" ORDER BY SEQ DESC");
                //parametros.Add("@DATA_MAXIMA", DataMaxima.ToString("dd.MM.yyyy"));

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    while (_reader.Read())
                    {
                        retorno.Add(new Ordem_Servico()
                        {
                            CodCt = GetString(_reader, "CODCT"),
                            Projeto = GetString(_reader, "PROJETO"),
                            OS = GetString(_reader, "NUM_OBRA"),
                            OSIntegrator = GetString(_reader, "PARAMETROS"),
                            Contrato = GetInt32(_reader, "NUM_DOC"),
                            Unidade_Custo = GetString(_reader, "UNIDADE_CUSTO"),
                            Tarefa = GetString(_reader, "TAREFA"),
                            Equipe = GetString(_reader, "EQUIPE"),
                            EquipeAux = GetString(_reader, "EQUIPEAUX"),
                            Cod_Encerramento = GetString(_reader, "COD_ENCERRAMENTO"),
                            Cod_Encerramento_Auxiliar = GetString(_reader, "CODAUX"),
                            Cod_Encerramento_Auxiliar2 = GetString(_reader, "CODAUX2"),
                            Desc_Cod_Encerramento = GetString(_reader, "DESC_CODIGO_ENCERRAMENTO"),
                            Status_Execucao = GetInt16(_reader, "STATUS_EXECUCAO"),
                            Status_Execucao_Servico = GetInt16(_reader, "STATUS_EXECUCAO_SERVICO"),
                            DataHoraInicioAtendimento = GetDateTime(_reader, "DATAHORA_INI_ATEND"),
                            DataEncerramento = GetDateTime(_reader, "DATA_PROD"),
                            HoraEncerramento = GetDateTime(_reader, "HORA_ENCERRAMENTO"),
                            Cod_Tipo_Serv = GetInt16(_reader, "COD_TIPO_SERV"),
                            DataAgendamento = GetDateTime(_reader, "DATA_MAXIMA"),
                            HoraAgendamento = GetDateTime(_reader, "HORA_MAXIMA"),
                            Endereco = GetString(_reader, "ENDERECO"),
                            Cidade = GetString(_reader, "ENDERECO_TIPO"),
                            Bairro = GetString(_reader, "ENDERECO_BAIRRO"),
                            Tipo_Producao = GetString(_reader, "TIPO_PRODUCAO") == "AT" ? Enum_Tipo_Producao.AT : Enum_Tipo_Producao.AC,
                            Cliente = GetString(_reader, "CIENTE"),
                            Desc_Tipo_Serv = GetString(_reader, "DESC_TIPO_SERV"),
                            Observacao = GetString(_reader, "OBSERVACAO_AUX"),
                            ObservacaoTeste = GetString(_reader, "OBSERVACAO_TESTE"),
                            Cod_TipoServ_Integrator = GetString(_reader, "COD_TIPO_SERV_INTEGRATOR"),
                            Seq = GetInt32(_reader, "SEQ") ?? 1,
                            Usuario = GetString(_reader, "USUARIO"),
                            Num_Fre = GetString(_reader, "NUM_FRE"),
                            Cod_Tipo_Imovel = GetInt64(_reader, "PARPRIM_PARA"),
                            Cod_Turno = GetInt16(_reader, "COD_TURNO_SERV"),
                            Encerra_OS_Apos_Baixa = GetString(_reader, "ENCERRA_OS_APOS_BAIXA"),
                            Uf = GetString(_reader, "ENDERECO_UF"),
                            Complemento = GetString(_reader, "ENDERECO_COMPL"),
                            Numero = GetString(_reader, "ENDERECO_NUMERO"),
                            Cep = GetString(_reader, "ENDERECO_CPL")
                        });
                    }
                }
            }
            if (retorno.Count > 0)
                return retorno.OrderByDescending(p => p.Seq).FirstOrDefault();
            else
                return null;
        }
        public String GetUnidadeCusto(String CODCT)
        {
            StringBuilder sql = new StringBuilder();
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendFormat("SELECT FIRST 1 A.UNIDADE_CUSTO FROM TB_CONTRATO A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.Append(" WHERE A.CODCT = @CODCT");
                parametros.Add("@CODCT", CODCT);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return GetString(_reader, "UNIDADE_CUSTO");
                    }
                }
            }

            return null;
        }

        public Tipo_Servico GetTipoServ(String DESCRICAO, String CODCT)
        {
            StringBuilder sql = new StringBuilder();

            Tipo_Servico retorno = null;

            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendFormat("SELECT FIRST 1 COD_TIPO_SERV, DESCRICAO, CODIGO_INTEGRATOR FROM TB_TIPO_SERVICO_EQUIPE A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.Append(" WHERE UPPER(A.DESCRICAO) = @DESCRICAO AND A.CODCT = @CODCT");
                parametros.Add("@DESCRICAO", DESCRICAO.ToUpper());
                parametros.Add("@CODCT", CODCT);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new Tipo_Servico()
                        {
                            Codigo = GetInt16(_reader, "COD_TIPO_SERV").Value,
                            Descricao = GetString(_reader, "DESCRICAO"),
                            Codigo_Integrator = GetString(_reader, "CODIGO_INTEGRATOR")
                        };
                    }
                }
            }

            return retorno;
        }

        private Int16 GetMaxTipoServ()
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendFormat("SELECT COALESCE(MAX(COD_TIPO_SERV),0) AS CO FROM TB_TIPO_SERVICO_EQUIPE A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        Int16 _tmp = GetInt16(_reader, "CO") ?? 0;
                        return _tmp++;
                    }
                    else return 0;
                }
            }
        }

        public Tipo_Servico InsertTipoServ(String Descricao, String Codigo_Integrator, String CodCt)
        {
            StringBuilder sql = new StringBuilder();

            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.Append("insert into TB_TIPO_SERVICO_EQUIPE (COD_TIPO_SERV, DESCRICAO, CODIGO_INTEGRATOR, CODCT) ");
                sql.Append("values(@COD_TIPO_SERV, @DESCRICAO, @CODIGO_INTEGRATOR, @CODCT) ");

                Int32 _max = GetMaxTipoServ() + 1;

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@COD_TIPO_SERV", _max },
                    { "@DESCRICAO", Descricao.ToUpper() },
                    { "@CODIGO_INTEGRATOR", Codigo_Integrator },
                    { "@CODCT", CodCt }
                };

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();

                return new Tipo_Servico
                {
                    Codigo = _max,
                    Descricao = Descricao,
                    Codigo_Integrator = Codigo_Integrator
                };
            }
        }

        public SubTipo_Servico GetSubTipoServ(String DESCRICAO, Int32? COD_TIPO_SERV)
        {
            StringBuilder sql = new StringBuilder();
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                SubTipo_Servico retorno = null;

                sql.AppendFormat("SELECT FIRST 1 COD_TIPO_SERV, DESCRICAO, COD_SUB_TIPO_SERV FROM TB_SUB_TIPO_SERVICO A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.Append(" WHERE UPPER(A.DESCRICAO) = @DESCRICAO AND A.COD_TIPO_SERV = @COD_TIPO_SERV");
                parametros.Add("@DESCRICAO", DESCRICAO.ToUpper());
                parametros.Add("@COD_TIPO_SERV", COD_TIPO_SERV);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new SubTipo_Servico()
                        {
                            CodigoTipoServ = GetInt16(_reader, "COD_TIPO_SERV").Value,
                            CodigoSubTipo = GetString(_reader, "COD_SUB_TIPO_SERV"),
                            Descricao = GetString(_reader, "DESCRICAO")
                        };
                    }
                }

                return retorno;
            }
        }

        private Int32 GetMaxSubTipoServ()
        {
            StringBuilder sql = new StringBuilder();
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendFormat("SELECT COALESCE(MAX(CAST(COD_SUB_TIPO_SERV AS INTEGER)),0) AS CO FROM TB_SUB_TIPO_SERVICO A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        Int32 _tmp = GetInt16(_reader, "CO") ?? 0;
                        return _tmp++;
                    }
                    else return 0;
                }
            }
        }



        public SubTipo_Servico InsertSubTipoServ(String Descricao, Int32? Cod_Tipo_Serv)
        {
            StringBuilder sql = new StringBuilder();

            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                sql.Append("insert into TB_SUB_TIPO_SERVICO (COD_TIPO_SERV, COD_SUB_TIPO_SERV, DESCRICAO, ENVIA_MOBILE) ");
                sql.Append("values(@COD_TIPO_SERV, @COD_SUB_TIPO_SERV, @DESCRICAO,@ENVIA_MOBILE) ");

                Int32 _max = GetMaxSubTipoServ() + 1;

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@COD_TIPO_SERV", Cod_Tipo_Serv },
                    { "@DESCRICAO", Descricao.ToUpper() },
                    { "@COD_SUB_TIPO_SERV", _max },
                    { "@ENVIA_MOBILE", "S" }
                };

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();

                return new SubTipo_Servico
                {
                    CodigoSubTipo = _max.ToString(),
                    CodigoTipoServ = Cod_Tipo_Serv,
                    Descricao = Descricao
                };
            }
        }
        public ProjetoIxc GetByContratoProjeto(string Contrato, string Projeto)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                ProjetoIxc retorno = null;

                sql.AppendLine("SELECT FIRST 1 A.PROJETO,A.COD_ESCRITORIO ");
                sql.AppendLine("FROM TB_PROJETO A ");


                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("WHERE (A.CODCT = @CODCT) AND (A.PROJETO = @PROJETO) ");
                parametros.Add("@CODCT", Contrato);
                parametros.Add("@PROJETO", Projeto);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new ProjetoIxc()
                        {
                            Codigo = GetString(_reader, "PROJETO"),
                            CodEscritorio = GetString(_reader, "COD_ESCRITORIO"),
                        };
                    }
                }

                return retorno;
            }
        }
        public LocalidadeIxc GetByLocalidade(String Cidade, String Localidade, string codEscritorio = null)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                var cidadeBusca = StringService.RemoverAcentos(Cidade).ToUpperInvariant();
                var localidadeBusca = StringService.RemoverAcentos(Localidade).ToUpperInvariant();
                List<LocalidadeIxc> localidades = new List<LocalidadeIxc>();

                sql.AppendLine("SELECT A.COD_CLIENTE, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE, A.LOCALIDADE, A.CODCT, A.PROJETO, B.DESCRICAO AS DESCRICAO_AREA ")
                   .AppendLine("FROM TB_LOCALIDADE A ")
                   .AppendLine("INNER JOIN TB_AREA B ")
                   .AppendLine("ON (A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA) ")
                   .AppendLine("INNER JOIN TB_ESCRITORIO C ")
                   .AppendLine("ON (A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");

                sql.AppendLine(" WHERE A.STATUS = 1");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    while (_reader.Read())
                    {
                        localidades.Add(new LocalidadeIxc()
                        {
                            Cod_Cliente = GetInt32(_reader, "COD_CLIENTE"),
                            Cod_Escritorio = GetString(_reader, "COD_ESCRITORIO"),
                            Cod_Area = GetString(_reader, "COD_AREA"),
                            LocalidadeStr = StringService.RemoverAcentos(GetString(_reader, "LOCALIDADE")) ?? "",
                            DescricaoArea = StringService.RemoverAcentos(GetString(_reader, "DESCRICAO_AREA")) ?? "",
                            Cod_Localidade = GetInt32(_reader, "COD_LOCALIDADE"),
                            CodCt = GetString(_reader, "CODCT"),
                            Projeto = GetString(_reader, "PROJETO"),
                        });
                    }
                }

                localidades = localidades.FindAll(p => p.DescricaoArea.ToUpperInvariant().Trim() == cidadeBusca.Trim() &&
                                                       p.LocalidadeStr.ToUpperInvariant().Trim() == localidadeBusca.Trim());

                if (string.IsNullOrEmpty(codEscritorio) == false)
                {
                    localidades = localidades.FindAll(p => p.Cod_Escritorio == codEscritorio);
                }

                var retorno = localidades.FirstOrDefault();

                return retorno;
            }
        }
        public EnderecoClienteIxc GetLocalidade(String numeroContrato, String cpf)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendFormat(@"SELECT FIRST 1 B.ID,B.COD_AREA,B.COD_EESCRITORIO,B.LOCALIDADE,B.COD_LOCALIDADE,B.ENDERECO_LATITUDE,B.ENDERECO_LONGITUDE,B.ENDERECO_COMPLEMENTO,B.ENDERECO_CEP,
                               B.ENDERECO_NUMERO,B.ENDERECO_LOGRADOURO,B.ENDERECO_UF,B.ENDERECO_BAIRRO,B.ENDERECO_CIDADE,B.ENDERECO_COMPLEMENTO, B.ENDERECO_PONTO_REFERENCIA FROM TB_CLIENTE_CONTRATO A " +
                                   " INNER JOIN TB_ENDERECO_CLIENTE B ON (A.ID_ENDERECO = B.ID)" +
                                   " INNER JOIN TB_CLIENTE C ON (A.ID_CLIENTE = C.ID)" +
                                   " WHERE A.NUMERO_CONTRATO = @NUMERO_CONTRATO AND C.CPF = @CPF");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@NUMERO_CONTRATO", numeroContrato);
                parametros.Add("@CPF", cpf);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return new EnderecoClienteIxc
                        {
                            Id = Convert.ToInt64(_reader["ID"]),
                            Bairro = Convert.ToString(_reader["ENDERECO_BAIRRO"]),
                            Cidade = Convert.ToString(_reader["ENDERECO_CIDADE"]),
                            Numero = Convert.ToString(_reader["ENDERECO_NUMERO"]),
                            Uf = Convert.ToString(_reader["ENDERECO_UF"]),
                            Cep = Convert.ToString(_reader["ENDERECO_CEP"]),
                            Logradouro = Convert.ToString(_reader["ENDERECO_LOGRADOURO"]),
                            Complemento = Convert.ToString(_reader["ENDERECO_COMPLEMENTO"]),
                            Referencia = Convert.ToString(_reader["ENDERECO_PONTO_REFERENCIA"]),
                            CodEscritorio = Convert.ToString(_reader["COD_EESCRITORIO"]),
                            CodArea = Convert.ToString(_reader["COD_AREA"]),
                            CodLocalidade = _reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(_reader["COD_LOCALIDADE"]) : (int?)null,
                            Latitude = _reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["ENDERECO_LATITUDE"]) : (double?)null,
                            Longitude = _reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["ENDERECO_LONGITUDE"]) : (double?)null,
                            Localidade = Convert.ToString(_reader["LOCALIDADE"])
                        };

                    }
                }

                return null;
            }
        }
        public AreaIxc GetByCidade(String Desc_Area, string codEscritorio = null)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                var descAreaBusca = StringService.RemoverAcentos(Desc_Area).ToUpperInvariant();
                List<AreaIxc> areas = new List<AreaIxc>();

                sql.AppendLine("SELECT COD_CLIENTE, COD_ESCRITORIO, COD_AREA, DESCRICAO, CODCT, PROJETO ")
                    .AppendLine("FROM TB_AREA A");

                sql.AppendLine(" WHERE A.STATUS = 1");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    while (_reader.Read())
                    {
                        areas.Add(new AreaIxc()
                        {
                            Cod_Cliente = GetInt32(_reader, "COD_CLIENTE"),
                            Cod_Escritorio = GetString(_reader, "COD_ESCRITORIO"),
                            Cod_Area = GetString(_reader, "COD_AREA"),
                            Descricao = StringService.RemoverAcentos(GetString(_reader, "DESCRICAO")),
                            CodCt = GetString(_reader, "CODCT"),
                            Projeto = GetString(_reader, "PROJETO"),
                        });
                    }
                }

                areas = areas.FindAll(p => p.Descricao.ToUpperInvariant() == descAreaBusca);

                if (string.IsNullOrEmpty(codEscritorio) == false)
                {
                    areas = areas.FindAll(p => p.Cod_Escritorio == codEscritorio);
                }

                var retorno = areas.FirstOrDefault();

                return retorno;
            }
        }
        public Boolean InsertOrdemServico(Ordem_Servico value)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();


                sql.Append("insert into TB_DOCUMENTO_PRODUCAO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, UNIDADE_CUSTO, TAREFA, EQUIPE," +
                            "COD_TIPO_SERV,COD_SUB_TIPO_SERV, DATA_MAXIMA, HORA_MAXIMA,DATA_ABERTURA, HORA_ABERTURA, ENDERECO, ENDERECO_UF,ENDERECO_TIPO, ENDERECO_BAIRRO,ENDERECO_CPL,ENDERECO_NUMERO,ENDERECO_COMPL, " +
                            "TIPO_PRODUCAO, TITULAR, OBSERVACAO_AUX, SEQ, OXBDPEE, RESPONSABILIDADE, STATUS_EXECUCAO, NUM_OBRA_ORIGINAL, ID_BDOS, " +
                            "DESCRICAO_PLANO_ASSINANTE, CPF_CLIENTE, RG_ASSINANTE, EMAIL_ASSINANTE, TEL_COMERCIAL, TEL_CIENTE, USUARIO, DIA, HORA, SENHA_BAIXA, PPPOE," +
                            "COD_ESCRITORIO, COD_AREA, COD_LOCALIDADE, LATITUDE_STR, LONGITUDE_STR, PARPRIM_PARA, CODIGO_PLANO_INTEGRATOR, OBSERVACAO_TESTE, ID_CLIENTE,GEOSEGMENTO,COD_TURNO_SERV,ID_LOGIN,CAIXA_FTTH,PORTA_FTTH,ID_CLIENTE_CONTRATOCONNECT) ");
                sql.AppendFormat("values(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @UNIDADE_CUSTO, @TAREFA, @EQUIPE, " +
                            "@COD_TIPO_SERV,@COD_SUB_TIPO_SERV, @DATA_MAXIMA, @HORA_MAXIMA,@DATA_ABERTURA, @HORA_ABERTURA, @ENDERECO, @ENDERECO_UF,@ENDERECO_TIPO, @ENDERECO_BAIRRO,@ENDERECO_CEP,@ENDERECO_NUMERO,@ENDERECO_COMPLEMENTO, " +
                            "@TIPO_PRODUCAO, @TITULAR, @OBSERVACAO_AUX, @SEQ, @OXBDPEE, @RESPONSABILIDADE, @STATUS_EXECUCAO, @NUM_OBRA_ORIGINAL, @ID_BDOS, " +
                            "@DESCRICAO_PLANO_ASSINANTE, @CPF_CLIENTE, @RG_ASSINANTE, @EMAIL_ASSINANTE, @TEL_COMERCIAL, @TEL_CIENTE, @USUARIO, CURRENT_DATE, CURRENT_TIME, @SENHA_BAIXA, @PPPOE, " +
                            "@COD_ESCRITORIO, @COD_AREA, @COD_LOCALIDADE, @LATITUDE_STR, @LONGITUDE_STR, @PARPRIM_PARA, @CODIGO_PLANO_INTEGRATOR, @OBSERVACAO_TESTE, @ID_CLIENTE,@GEOSEGMENTO,@COD_TURNO_SERV,@ID_LOGIN,@CAIXA_FTTH,@PORTA_FTTH,@ID_CLIENTE_CONTRATOCONNECT) ");

                Int32 _seq = GerarSeq(value.CodCt, value.Projeto, value.OS, value.Contrato.GetValueOrDefault(0));
                value.Seq = _seq;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", value.CodCt);
                parametros.Add("@PROJETO", value.Projeto);
                parametros.Add("@NUM_OBRA", String.Format("{0}/{1}", value.OS, _seq));
                parametros.Add("@ID_BDOS", String.Format("{0}/{1}", value.OS, _seq));
                parametros.Add("@NUM_OBRA_ORIGINAL", value.OS);
                parametros.Add("@SEQ", _seq);
                parametros.Add("@NUM_DOC", value.Contrato.GetValueOrDefault(0));
                parametros.Add("@UNIDADE_CUSTO", value.Unidade_Custo);
                parametros.Add("@TAREFA", StringService.Truncate(value.Tarefa, 15));
                parametros.Add("@EQUIPE", value.Equipe != null ? value.Equipe : String.Empty);
                parametros.Add("@COD_TIPO_SERV", value.Cod_Tipo_Serv);
                parametros.Add("@COD_SUB_TIPO_SERV", value.Cod_SubTipo_Serv);
                parametros.Add("@DATA_MAXIMA", ConvertToDataValida(value.DataAgendamento?.Date.ToString()));
                parametros.Add("@HORA_MAXIMA", ConvertToHoraValida(value.DataAgendamento?.TimeOfDay.ToString()));
                parametros.Add("@COD_TURNO_SERV", value.Cod_Turno);
                parametros.Add("@DATA_ABERTURA", ConvertToDataValida(value.DataAbertura?.Date.ToString()));
                parametros.Add("@HORA_ABERTURA", ConvertToHoraValida(value.HoraAbertura?.TimeOfDay.ToString()));
                parametros.Add("@ENDERECO", StringService.Truncate(value.Endereco, 100));
                parametros.Add("@ENDERECO_UF", StringService.Truncate(value.Uf, 20));
                parametros.Add("@ENDERECO_TIPO", StringService.Truncate(value.Cidade, 35));
                parametros.Add("@ENDERECO_BAIRRO", StringService.Truncate(value.Bairro, 30));
                parametros.Add("@ENDERECO_CEP", StringService.Truncate(value.Cep, 30));
                parametros.Add("@ENDERECO_NUMERO", StringService.Truncate(value.Numero, 10));
                parametros.Add("@ENDERECO_COMPLEMENTO", StringService.Truncate(value.Complemento, 100));
                parametros.Add("@TIPO_PRODUCAO", "AC");
                parametros.Add("@TITULAR", StringService.Truncate(StringService.RemoverAcentos(value.Titular), 50));
                parametros.Add("@USUARIO", value.Usuario);
                parametros.Add("@OBSERVACAO_AUX", StringService.Truncate($"Id: {value.IdOriginalIxc} / Plano: {value.CodPlano} / {value.DescPlano} | Referencia: {value.Referencia}", 900));
                parametros.Add("@OXBDPEE", StringService.Truncate(value.Oxbdpee, 50));
                parametros.Add("@RESPONSABILIDADE", 0);
                parametros.Add("@ID_CLIENTE_CONTRATOCONNECT", value.Id_Cliente_Contrato);

                if (value.Equipe != null && ConvertToDataValida(value.DataAgendamento?.Date.ToString()) != null)
                {
                    parametros.Add("@STATUS_EXECUCAO", 0); // aberta aguardando atendimento
                }
                else if (value.Equipe == null && ConvertToDataValida(value.DataAgendamento?.Date.ToString()) != null)
                {
                    parametros.Add("@STATUS_EXECUCAO", 13); // aberta aguardando responsável
                }
                else
                {
                    parametros.Add("@STATUS_EXECUCAO", 4); // aberta aguardando agendamento
                }


                parametros.Add("@DESCRICAO_PLANO_ASSINANTE", StringService.Truncate(value.DescPlano, 150));
                parametros.Add("@CPF_CLIENTE", StringService.Truncate(value.CpfCliente, 14));
                parametros.Add("@RG_ASSINANTE", StringService.Truncate(value.Rg, 20));
                parametros.Add("@EMAIL_ASSINANTE", StringService.Truncate(value.Email_Assinante, 50));
                parametros.Add("@TEL_COMERCIAL", StringService.Truncate(value.Telefone_Comercial, 15));
                parametros.Add("@TEL_CIENTE", StringService.Truncate(value.Telefone, 15));
                parametros.Add("@SENHA_BAIXA", StringService.Truncate(value.SenhaRadius, 100));
                parametros.Add("@PPPOE", StringService.Truncate(value.LoginRadius, 100));
                parametros.Add("@COD_ESCRITORIO", StringService.Truncate(value.Cod_Escritorio, 4));
                parametros.Add("@COD_AREA", StringService.Truncate(value.Cod_Area, 4));
                parametros.Add("@COD_LOCALIDADE", value.Cod_Localidade);
                parametros.Add("@LATITUDE_STR", value.Latitude);
                parametros.Add("@LONGITUDE_STR", value.Longitude);
                parametros.Add("@PARPRIM_PARA", 1); /*CONSTANTES.Cod_TipoImovelPadrao*/
                parametros.Add("@CODIGO_PLANO_INTEGRATOR", StringService.Truncate(value.CodPlano, 50));
                parametros.Add("@OBSERVACAO_TESTE", StringService.Truncate($"Login: {StringService.Truncate(value.LoginRadius, 100)} / Senha: {StringService.Truncate(value.SenhaRadius, 100)} / {value.Observacao}", 4096));
                parametros.Add("@ID_CLIENTE", value.Id_Cliente);
                parametros.Add("@GEOSEGMENTO", value.Contrato_Kit);
                parametros.Add("@ID_LOGIN", value.Id_Login);
                parametros.Add("@CAIXA_FTTH", value.Id_Caixa);
                parametros.Add("@PORTA_FTTH", value.Id_Porta);


                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();

                return true;
            }
        }

        protected Int32 GerarSeq(String Codct, String Projeto, String OS, Int32 Num_Doc)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT COALESCE(MAX(SEQ),0)AS CO FROM TB_DOCUMENTO_PRODUCAO ");
                sql.AppendLine(" WHERE (CODCT = @CODCT) AND (PROJETO = @PROJETO) AND (NUM_OBRA_ORIGINAL = @NUM_OBRA) ");
                sql.AppendLine(" AND (NUM_DOC = @NUM_DOC) ");


                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", Codct);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", OS);
                parametros.Add("@NUM_DOC", Num_Doc);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return GetInt32(_reader, "CO") == null ? 1 : GetInt32(_reader, "CO").Value + 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }

        public Boolean InsertAgendamento(Ordem_Servico servico)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();


                sql.Append("INSERT INTO TB_AGENDAMENTO_EQUIPEANIEL (ID,CODCT, PROJETO, NUM_OBRA, NUM_DOC, EQUIPE," +
                            "USUARIOCRIACAO, DATAHORACRIACAO,DATAHORA_INICIO, DATAHORA_FIM, DURACAO, COD_ESCRITORIO, COD_AREA,COD_LOCALIDADE, " +
                            "ID_TIPO_SERV,ORIGEM, LATITUDE, LONGITUDE)");


                sql.AppendFormat("values(GEN_ID(GN_AGENDAMENTO_EQUIPEANIEL,1),@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @EQUIPE, " +
                            "@USUARIOCRIACAO,@DATAHORACRIACAO,@DATAHORA_INICIO,@DATAHORA_FIM,@DURACAO,@COD_ESCRITORIO, @COD_AREA, @COD_LOCALIDADE," +
                            "@COD_TIPO_SERV,@ORIGEM, @LATITUDE_STR, @LONGITUDE_STR)");



                DateTime? dataHoraInicio = servico.DataAgendamento.HasValue ? servico.DataAgendamento.Value.Date : (DateTime?)null;
                if (dataHoraInicio.HasValue)
                {
                    if (servico.HoraAgendamento.HasValue)
                    {
                        dataHoraInicio = dataHoraInicio.Value.Add(servico.HoraAgendamento.Value.TimeOfDay);
                    }

                    TimeSpan? tempoExecucao = _repoDocProd.GetTempoExecucao(_config, dataHoraInicio.Value, servico.CodCt, servico.Projeto, StringService.Truncate(String.Format("{0}/{1}", servico.OS, servico.Seq), 50), servico.Contrato.GetValueOrDefault(0));
                    DateTime? dataHoraFim = dataHoraInicio.Value.Add(tempoExecucao.Value);



                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    parametros.Add("@CODCT", servico.CodCt);
                    parametros.Add("@PROJETO", servico.Projeto);
                    parametros.Add("@NUM_OBRA", StringService.Truncate(String.Format("{0}/{1}", servico.OS, servico.Seq), 50));
                    parametros.Add("@NUM_DOC", servico.Contrato.GetValueOrDefault(0));
                    parametros.Add("@COD_ESCRITORIO", servico.Cod_Escritorio);
                    parametros.Add("@COD_AREA", servico.Cod_Area);
                    parametros.Add("@COD_LOCALIDADE", servico.Cod_Localidade);
                    parametros.Add("@USUARIOCRIACAO", "INTEGRACAO");
                    parametros.Add("@DATAHORACRIACAO", DateTime.Now);
                    parametros.Add("@DATAHORA_INICIO", dataHoraInicio);
                    parametros.Add("@DATAHORA_FIM", dataHoraFim);
                    parametros.Add("@DURACAO", tempoExecucao);
                    parametros.Add("@ORIGEM", 4);
                    parametros.Add("@LATITUDE", servico.Latitude == String.Empty ? null : servico.Latitude);
                    parametros.Add("@LONGITUDE", servico.Longitude == String.Empty ? null : servico.Longitude);
                    parametros.Add("@EQUIPE", servico.Equipe);
                    parametros.Add("@COD_TIPO_SERV", servico.Cod_Tipo_Serv);


                    double latitudeDouble;
                    double longitudeDouble;

                    if (Double.TryParse(servico.Latitude, out latitudeDouble))//&& CONSTANTES.ImportarLatitudeLongitude == "S"
                    {
                        parametros.Add("@LATITUDE_STR", latitudeDouble);
                    }
                    else
                    {
                        parametros.Add("@LATITUDE_STR", null);
                    }

                    if (Double.TryParse(servico.Longitude, out longitudeDouble)) //&& CONSTANTES.ImportarLatitudeLongitude == "S"
                    {
                        parametros.Add("@LONGITUDE_STR", longitudeDouble);
                    }
                    else
                    {
                        parametros.Add("@LONGITUDE_STR", null);
                    }




                    CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();
                }

                return true;
            }

        }

        public Boolean InsertCliente(ClienteIxc value)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("insert into TB_CLIENTE (");
                sql.Append("    COD_CLIENTE, RAZAO_SOCIAL, CNPJ, EMAIL, TIPO, SITUACAO, USUARIO_CRIACAO, DATA_CADASTRO, HORA_CRIACAO, ");
                sql.Append("    ENDERECO, ENDERECO_NUMERO, BAIRRO, MUNICIPIO, CEP, UF, TELEFONE,TELEFONE2, REF_COMERCIAL_TEL, COD_MUNICIPIO) ");
                sql.Append("values(");
                sql.Append("    @COD_CLIENTE, @RAZAO_SOCIAL, @CNPJ, @EMAIL, @TIPO, @SITUACAO, 'INTEGRATOR', CURRENT_DATE, CURRENT_TIME, ");
                sql.Append("    @ENDERECO, @ENDERECO_NUMERO, @BAIRRO, @MUNICIPIO, @CEP, @UF, @TELEFONE, @TELEFONE2,@REF_COMERCIAL_TEL, @COD_MUNICIPIO) ");

                if (value.Codigo == null)
                    value.Codigo = GetMaxIdCliente() + 1;

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@COD_CLIENTE", value.Codigo },
                    { "@RAZAO_SOCIAL", StringService.Truncate(value.Razao_Social, 100) },
                    { "@CNPJ", StringService.Truncate(value.CNPJ, 20) },
                    { "@EMAIL", StringService.Truncate(value.Email, 50) },
                    { "@TIPO", value.TipoCliente },
                    { "@SITUACAO", value.Situacao },
                    { "@ENDERECO", StringService.Truncate(value.Endereco, 50) },
                    { "@ENDERECO_NUMERO", StringService.Truncate(value.Numero, 10) },
                    { "@BAIRRO", StringService.Truncate(value.Bairro, 20) },
                    { "@MUNICIPIO", StringService.Truncate(value.Municipio, 20) },
                    { "@CEP", StringService.Truncate(value.CEP, 10) },
                    { "@UF", StringService.Truncate(value.UF, 2) },
                    { "@TELEFONE", StringService.Truncate(value.Telefone_Residencial, 15) },
                    { "@TELEFONE2", StringService.Truncate(value.Telefone, 15) },
                    { "@REF_COMERCIAL_TEL", StringService.Truncate(value.Telefone_Comercial, 15) },
                    { "@COD_MUNICIPIO", value.CodMunicipio }
                };

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();

                return true;
            }
        }

        public Boolean UpdateDadosCliente(ClienteIxc value)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update TB_CLIENTE SET ");
                sql.Append("ENDERECO = @ENDERECO, " +
                    "BAIRRO = @BAIRRO, " +
                    "RAZAO_SOCIAL = @RAZAO_SOCIAL, " +
                    "EMAIL = @EMAIL, " +
                    "MUNICIPIO = @MUNICIPIO, " +
                    "CEP = @CEP, " +
                    "UF = @UF, " +
                    "TELEFONE = @TELEFONE, " +
                    "TELEFONE2 = @TELEFONE2, " +
                    "REF_COMERCIAL_TEL = @REF_COMERCIAL_TEL, " +
                    "USUARIO_ALTERACAO = 'INTEGRATOR', " +
                    "DATA_ATUALIZACAO = CURRENT_DATE, " +
                    "HORA_ALTERACAO = CURRENT_TIME ");
                sql.Append("where (CNPJ = @CNPJ) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@CNPJ", StringService.Truncate(value.CNPJ, 20) },
                    { "@ENDERECO", StringService.Truncate(StringService.RemoverAcentos(value.Endereco), 50) },
                    { "@BAIRRO", StringService.Truncate(StringService.RemoverAcentos(value.Bairro), 20) },
                    { "@MUNICIPIO", StringService.Truncate(StringService.RemoverAcentos(value.Municipio), 20) },
                    { "@CEP", StringService.Truncate(StringService.RemoverAcentos(value.CEP), 10) },
                    { "@UF", StringService.Truncate(StringService.RemoverAcentos(value.UF), 2) },
                    { "@RAZAO_SOCIAL", StringService.Truncate(value.Razao_Social, 100) },
                    { "@TELEFONE", StringService.Truncate(value.Telefone_Residencial, 15) },
                    { "@EMAIL", StringService.Truncate(value.Email, 50) },
                    { "@TELEFONE2", StringService.Truncate(value.Telefone, 15) },
                    { "@REF_COMERCIAL_TEL", StringService.Truncate(value.Telefone_Comercial, 15) }
                };

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();

                return true;
            }
        }

        private Int32 GetMaxIdCliente()
        {
            using (var _sessionT5Nh = HibernateUtil.GetSessionT5())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                Int32 retorno = 0;

                StringBuilder sql = new StringBuilder();

                sql.Append("Select MAX(COALESCE(COD_CLIENTE, 0)) as CO From TB_CLIENTE ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = GetInt32(_reader, "CO").GetValueOrDefault(1);
                    }
                }

                return retorno == 0 ? 1 : retorno;
            }
        }

        public EnderecoClienteIxc GetLocalidadeEnderecoCliente(String numeroContrato, String cpf)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendFormat(@"SELECT FIRST 1 B.ID,B.COD_AREA,B.COD_EESCRITORIO,B.LOCALIDADE,B.COD_LOCALIDADE,B.ENDERECO_LATITUDE,B.ENDERECO_LONGITUDE,B.ENDERECO_COMPLEMENTO,B.ENDERECO_CEP,
                               B.ENDERECO_NUMERO,B.ENDERECO_LOGRADOURO,B.ENDERECO_UF,B.ENDERECO_BAIRRO,B.ENDERECO_CIDADE,B.ENDERECO_COMPLEMENTO, B.ENDERECO_PONTO_REFERENCIA FROM TB_CLIENTE_CONTRATO A " +
                                   " INNER JOIN TB_ENDERECO_CLIENTE B ON (A.ID_ENDERECO = B.ID)" +
                                   " INNER JOIN TB_CLIENTE C ON (A.ID_CLIENTE = C.ID)" +
                                   " WHERE A.NUMERO_CONTRATO = @NUMERO_CONTRATO AND C.CPF = @CPF");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@NUMERO_CONTRATO", numeroContrato);
                parametros.Add("@CPF", cpf);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return new EnderecoClienteIxc
                        {
                            Id = Convert.ToInt64(_reader["ID"]),
                            Bairro = Convert.ToString(_reader["ENDERECO_BAIRRO"]),
                            Cidade = Convert.ToString(_reader["ENDERECO_CIDADE"]),
                            Numero = Convert.ToString(_reader["ENDERECO_NUMERO"]),
                            Uf = Convert.ToString(_reader["ENDERECO_UF"]),
                            Cep = Convert.ToString(_reader["ENDERECO_CEP"]),
                            Logradouro = Convert.ToString(_reader["ENDERECO_LOGRADOURO"]),
                            Complemento = Convert.ToString(_reader["ENDERECO_COMPLEMENTO"]),
                            Referencia = Convert.ToString(_reader["ENDERECO_PONTO_REFERENCIA"]),
                            CodEscritorio = Convert.ToString(_reader["COD_EESCRITORIO"]),
                            CodArea = Convert.ToString(_reader["COD_AREA"]),
                            CodLocalidade = _reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(_reader["COD_LOCALIDADE"]) : (int?)null,
                            Latitude = _reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["ENDERECO_LATITUDE"]) : (double?)null,
                            Longitude = _reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["ENDERECO_LONGITUDE"]) : (double?)null,
                            Localidade = Convert.ToString(_reader["LOCALIDADE"])
                        };

                    }
                }
            }

            return null;
        }
        public long? GetIdContrato(String numeroContrato, String cpf)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendFormat(@"SELECT FIRST 1 A.ID FROM TB_CLIENTE_CONTRATO A " +
                                   " INNER JOIN TB_CLIENTE C ON (A.ID_CLIENTE = C.ID)" +
                                   " WHERE A.NUMERO_CONTRATO = @NUMERO_CONTRATO AND C.CPF = @CPF");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@NUMERO_CONTRATO", numeroContrato);
                parametros.Add("@CPF", cpf);

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        return Convert.ToInt64(_reader["ID"]);
                    }
                }
            }
            return null;
        }

        public Boolean UpdateOrInsertClienteConnect(ClienteIxc value)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"UPDATE OR INSERT INTO TB_CLIENTE(
                                                        ID, NOMEFANTASIA, CPF, EMAIL_PRINCIPAL, TIPO_CLIENTE, TIPOPESSOA, STATUS, USUARIOCRIACAO, DATAHORACRIACAO, 
                                                        TELEFONE_FIXO,TELEFONE_CELULAR,TELEFONE_RECADO) 
                                                    values(
                                                        @ID, @NOMEFANTASIA, @CPF, @EMAIL_PRINCIPAL, @TIPO_CLIENTE, @TIPOPESSOA, @STATUS, 'INTEGRATOR', CURRENT_TIMESTAMP,
                                                        @TELEFONE_FIXO,@TELEFONE_CELULAR,@TELEFONE_RECADO) 
                                                    MATCHING(ID)");

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@ID", value.Codigo },
                    { "@NOMEFANTASIA", StringService.Truncate(StringService.RemoverAcentos(value.Razao_Social), 255) },
                    { "@CPF", StringService.Truncate(value.CNPJ, 20) },
                    { "@EMAIL_PRINCIPAL", StringService.Truncate(value.Email, 50) },
                    { "@TIPO_CLIENTE", 0 },
                    { "@TIPOPESSOA", value.TipoCliente == "F" ? 0 : 1 },
                    { "@STATUS", 1 }, //Ativo                
                    { "@TELEFONE_FIXO", StringService.Truncate(value.Telefone_Residencial, 18) },
                    { "@TELEFONE_CELULAR", StringService.Truncate(value.Telefone, 18) },
                    { "@TELEFONE_RECADO", StringService.Truncate(value.Telefone_Comercial, 18) }
                };

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();
            }

            return true;
        }
        public void AtualizaGeneratorClienteConnect()
        {
            Int64 id = GetMaxIdClienteConnect();
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder($"set generator GN_CLIENTE to {id}");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();
            }
        }

        private Int64 GetMaxIdClienteConnect()
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                Int64 retorno = 0;

                StringBuilder sql = new StringBuilder();

                sql.Append("Select MAX(COALESCE(ID, 0)) as CO From TB_CLIENTE ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = GetInt64(_reader, "CO").GetValueOrDefault(1);
                    }
                }

                return retorno == 0 ? 1 : retorno;
            }
        }

        public bool UpdateOrInsertContrato(InfoCliente value, Cliente_ContratoResponse cliente_ContratoResponse, long? IdEndereco)
        {
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {

                var idContrato = GetIdContrato(cliente_ContratoResponse.id, value.cliente.CNPJ);

                if (idContrato == null)
                {
                    StringBuilder sql = new StringBuilder(@"INSERT INTO TB_CLIENTE_CONTRATO(
                                                        ID,DATAHORACRIACAO,USUARIOCRIACAO,DESCRICAO,NUMERO_CONTRATO,ID_ENDERECO,ID_CLIENTE) 
                                                    values(
                                                        GEN_ID(GN_CLIENTE_CONTRATO,1),@DATAHORACRIACAO, @USUARIOCRIACAO, @DESCRICAO, @NUMERO_CONTRATO, @ID_ENDERECO, @ID_CLIENTE)");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@DATAHORACRIACAO", DateTime.Now },
                        { "@USUARIOCRIACAO", "INTEGRACAO" },
                        { "@DESCRICAO", StringService.Truncate(cliente_ContratoResponse.contrato,100) },
                        { "@NUMERO_CONTRATO", cliente_ContratoResponse.id },
                        { "@ID_ENDERECO", IdEndereco },
                        { "@ID_CLIENTE", value.cliente.Codigo }
                    };

                    CommandHelper.ExecuteNonQuery(com, sql, parametros, _sessionT5Nh.Connection, null).Wait();
                }
                else
                {
                    StringBuilder sql = new StringBuilder(@"UPDATE OR INSERT INTO TB_CLIENTE_CONTRATO(
                                                        ID,DATAHORACRIACAO,USUARIOCRIACAO,DESCRICAO,NUMERO_CONTRATO,ID_ENDERECO,ID_CLIENTE) 
                                                    values(
                                                        @ID,@DATAHORACRIACAO, @USUARIOCRIACAO, @DESCRICAO, @NUMERO_CONTRATO, @ID_ENDERECO, @ID_CLIENTE)
                                                        MATCHING (ID)");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@ID", idContrato },
                        { "@DATAHORACRIACAO", DateTime.Now },
                        { "@USUARIOCRIACAO", "INTEGRACAO" },
                        { "@DESCRICAO", StringService.Truncate(cliente_ContratoResponse.contrato,100) },
                        { "@NUMERO_CONTRATO", cliente_ContratoResponse.id },
                        { "@ID_ENDERECO", IdEndereco },
                        { "@ID_CLIENTE", value.cliente.Codigo }
                    };
                }

                return true;
            }
        }

        public long? UpdateOrInsertEndereco(InfoCliente value, Cliente_ContratoResponse cliente_ContratoResponse, List<CidadeResponse> responseCidade)
        {
            var localidadeContrato = GetLocalidade(cliente_ContratoResponse.id, value.cliente.CNPJ);
            long idEndereco = (localidadeContrato?.Id != null ? localidadeContrato.Id : GetIdEndereco());
            string nomeCidade = null;
            string uf = null;

            if (cliente_ContratoResponse.endereco_padrao_cliente == "S")
            {
                nomeCidade = value.cliente.Municipio;
                uf = value.cliente.UF;
            }
            else if (cliente_ContratoResponse.cidade != String.Empty)
            {
                if (responseCidade != null)
                {
                    var registroCidade = responseCidade.FirstOrDefault();
                    if (registroCidade != null)
                    {
                        nomeCidade = registroCidade.nome;
                        uf = registroCidade.uf;
                    }
                }
                //clienteAniel.CodMunicipio = Convert.ToInt16(responseCidade != null ? responseCidade.First().cod_ibge : "0");
            }

            if (cliente_ContratoResponse.endereco == String.Empty)
                cliente_ContratoResponse.endereco = null;
            if (cliente_ContratoResponse.numero == String.Empty)
                cliente_ContratoResponse.numero = null;
            if (cliente_ContratoResponse.bairro == String.Empty)
                cliente_ContratoResponse.bairro = null;
            if (cliente_ContratoResponse.cep == String.Empty)
                cliente_ContratoResponse.cep = null;


            if (localidadeContrato == null ||
                nomeCidade != localidadeContrato?.Cidade ||
                uf != localidadeContrato?.Uf ||
                localidadeContrato?.Logradouro != cliente_ContratoResponse.endereco ||
                localidadeContrato?.Numero != cliente_ContratoResponse.numero ||
                localidadeContrato?.Bairro != cliente_ContratoResponse.bairro ||
                localidadeContrato?.Cep != cliente_ContratoResponse.cep)
            {
                using (var _sessionT5Nh = HibernateUtil.GetSession())
                using (var com = _sessionT5Nh.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"update or insert into TB_ENDERECO_CLIENTE (ID, ENDERECO_CEP, ENDERECO_LOGRADOURO, ENDERECO_NUMERO,
                                           ENDERECO_BAIRRO, ENDERECO_CIDADE, ENDERECO_UF, ENDERECO_COMPLEMENTO,
                                           ENDERECO_PONTO_REFERENCIA, ENDERECO_LATITUDE, ENDERECO_LONGITUDE, COD_AREA,
                                           COD_EESCRITORIO, COD_LOCALIDADE, LOCALIDADE, USUARIOCRIACAO)
values (@ID, @ENDERECO_CEP, @ENDERECO_LOGRADOURO, @ENDERECO_NUMERO, @ENDERECO_BAIRRO,
        @ENDERECO_CIDADE, @ENDERECO_UF, @ENDERECO_COMPLEMENTO, @ENDERECO_PONTO_REFERENCIA, @ENDERECO_LATITUDE,
        @ENDERECO_LONGITUDE, @COD_AREA, @COD_EESCRITORIO, @COD_LOCALIDADE, @LOCALIDADE, @USUARIOCRIACAO)
matching (ID)");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    double latitudeDouble;
                    double longitudeDouble;

                    parametros.Add("@ID", idEndereco);
                    parametros.Add("@ENDERECO_CEP", cliente_ContratoResponse.cep == String.Empty ? null : cliente_ContratoResponse.cep);
                    parametros.Add("@ENDERECO_LOGRADOURO", cliente_ContratoResponse.endereco == String.Empty ? null : StringService.Truncate(cliente_ContratoResponse.endereco, 50));
                    parametros.Add("@ENDERECO_NUMERO", cliente_ContratoResponse.numero == String.Empty ? null : StringService.Truncate(cliente_ContratoResponse.numero, 10));
                    parametros.Add("@ENDERECO_BAIRRO", cliente_ContratoResponse.bairro == String.Empty ? null : StringService.Truncate(cliente_ContratoResponse.bairro, 50));
                    parametros.Add("@ENDERECO_CIDADE", nomeCidade == String.Empty ? null : StringService.Truncate(nomeCidade, 50));
                    parametros.Add("@ENDERECO_UF", uf == String.Empty ? null : uf);
                    parametros.Add("@ENDERECO_COMPLEMENTO", cliente_ContratoResponse.complemento == String.Empty ? null : StringService.Truncate(cliente_ContratoResponse.complemento, 100));
                    parametros.Add("@ENDERECO_PONTO_REFERENCIA", cliente_ContratoResponse.referencia == String.Empty ? null : cliente_ContratoResponse.referencia);



                    if (Double.TryParse(cliente_ContratoResponse.latitude, out latitudeDouble)) //&& CONSTANTES.ImportarLatitudeLongitude == "S"
                    {
                        parametros.Add("@ENDERECO_LATITUDE", cliente_ContratoResponse.latitude);
                    }
                    else
                    {
                        parametros.Add("@ENDERECO_LATITUDE", null);
                    }

                    if (Double.TryParse(cliente_ContratoResponse.longitude, out longitudeDouble))//&& CONSTANTES.ImportarLatitudeLongitude == "S"
                    {
                        parametros.Add("@ENDERECO_LONGITUDE", cliente_ContratoResponse.longitude);
                    }
                    else
                    {
                        parametros.Add("@ENDERECO_LONGITUDE", null);
                    }

                    parametros.Add("@COD_AREA", localidadeContrato?.CodArea == String.Empty ? null : localidadeContrato?.CodArea);
                    parametros.Add("@COD_EESCRITORIO", localidadeContrato?.CodEscritorio == String.Empty ? null : localidadeContrato?.CodEscritorio);
                    parametros.Add("@COD_LOCALIDADE", localidadeContrato?.CodLocalidade);
                    parametros.Add("@LOCALIDADE", localidadeContrato?.Localidade == String.Empty ? null : localidadeContrato?.Localidade);
                    parametros.Add("@USUARIOCRIACAO", "INTEGRACAO");


                    if (cliente_ContratoResponse.endereco_padrao_cliente == "S")
                    {


                        parametros = new Dictionary<string, object>();

                        parametros.Add("@ID", idEndereco);
                        parametros.Add("@ENDERECO_CEP", value.cliente.CEP == String.Empty ? null : value.cliente.CEP);
                        parametros.Add("@ENDERECO_LOGRADOURO", value.cliente.Endereco == String.Empty ? null : StringService.Truncate(value.cliente.Endereco, 50));
                        parametros.Add("@ENDERECO_NUMERO", value.cliente.Numero == String.Empty ? null : StringService.Truncate(value.cliente.Numero, 10));
                        parametros.Add("@ENDERECO_BAIRRO", value.cliente.Bairro == String.Empty ? null : StringService.Truncate(value.cliente.Bairro, 50));
                        parametros.Add("@ENDERECO_CIDADE", nomeCidade == String.Empty ? null : StringService.Truncate(nomeCidade, 50));
                        parametros.Add("@ENDERECO_UF", uf == String.Empty ? null : uf);
                        parametros.Add("@ENDERECO_COMPLEMENTO", value.cliente.Complemento == String.Empty ? null : StringService.Truncate(value.cliente.Complemento, 100));
                        parametros.Add("@ENDERECO_PONTO_REFERENCIA", value.cliente.Referencia == String.Empty ? null : value.cliente.Referencia);



                        if (Double.TryParse(value.cliente.Latitude, out latitudeDouble)) /*&& CONSTANTES.ImportarLatitudeLongitude == "S"*/
                        {
                            parametros.Add("@ENDERECO_LATITUDE", value.cliente.Latitude);
                        }
                        else
                        {
                            parametros.Add("@ENDERECO_LATITUDE", null);
                        }

                        if (Double.TryParse(value.cliente.Longitude, out longitudeDouble)) /*&& CONSTANTES.ImportarLatitudeLongitude == "S"*/
                        {
                            parametros.Add("@ENDERECO_LONGITUDE", value.cliente.Longitude);
                        }
                        else
                        {
                            parametros.Add("@ENDERECO_LONGITUDE", null);
                        }

                        parametros.Add("@COD_AREA", localidadeContrato?.CodArea == String.Empty ? null : localidadeContrato?.CodArea);
                        parametros.Add("@COD_EESCRITORIO", localidadeContrato?.CodEscritorio == String.Empty ? null : localidadeContrato?.CodEscritorio);
                        parametros.Add("@COD_LOCALIDADE", localidadeContrato?.CodLocalidade);
                        parametros.Add("@LOCALIDADE", localidadeContrato?.Localidade == String.Empty ? null : localidadeContrato?.Localidade);
                        parametros.Add("@USUARIOCRIACAO", "INTEGRACAO");

                    }


                    CommandHelper.ExecuteNonQuery(com,sql, parametros, _sessionT5Nh.Connection, null).Wait();
                }
            }
            return idEndereco;
        }

        private Int64 GetIdEndereco()
        {
            Int64 retorno = 0;
            using (var _sessionT5Nh = HibernateUtil.GetSession())
            using (var com = _sessionT5Nh.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("SELECT GEN_ID(GN_ENDERECO_CLIENTE, 1) AS ID FROM RDB$DATABASE ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com,sql, parametros, _sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = GetInt64(_reader, "ID").GetValueOrDefault(1);
                    }
                }
            }

            return retorno == 0 ? 1 : retorno;
        }

        protected String GetString(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? String.Empty : reader[FieldName].ToString();
            }
            catch
            {
                return null;
            }
        }

        protected Int64? GetInt64(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? Convert.ToInt64(null) : Int64.Parse(reader[FieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        protected Int32? GetInt32(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? Convert.ToInt32(null) : Int32.Parse(reader[FieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        protected Int16? GetInt16(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? Convert.ToInt16(null) : Int16.Parse(reader[FieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        protected Double? GetDouble(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? Convert.ToDouble(null) : Double.Parse(reader[FieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        protected DateTime? GetDateTime(DbDataReader reader, String FieldName)
        {
            try
            {
                return reader[FieldName] == null ? Convert.ToDateTime(null) : DateTime.Parse(reader[FieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        protected DateTime? ConvertDataHora(DateTime? Data, DateTime? Hora)
        {
            try
            {
                if (!Data.HasValue) return null;
                else
                {
                    DateTime _datahora = Data.Value.Date;
                    if (Hora.HasValue) _datahora.AddHours(Hora.Value.Hour).AddMinutes(Hora.Value.Minute).AddSeconds(Hora.Value.Second);

                    return _datahora;
                }
            }
            catch
            {
                return null;
            }
        }

        #region IDisposable Support
        private Boolean disposedValue;// To detect redundant calls

        // IDisposable
        protected virtual void Dispose(Boolean disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {

                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public DateTime? ConvertToDataValida(String value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            try
            {
                return Convert.ToDateTime(value.Substring(0, 10));
            }
            catch
            {
                return null;
            }
        }

        public DateTime? ConvertToHoraValida(String value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return null;
            }
        }

    }
}
