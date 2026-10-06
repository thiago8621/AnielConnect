using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq;
using System.Text;
using Aniel.Connect.Repository.Model;
using Sinapse.Util;

namespace Aniel.Connect.Repository.T5.Localidade
{
    public class LocalidadeAnielRepository : ILocalidadeAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public LocalidadeAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<LocalidadeAniel> GetAll()
        {
            using (var repoLocalidade = new Conexao<LocalidadeAniel>(this._sessionT5Nh))
            {
                return repoLocalidade.QueryableSelect();
            }
        }

        public LocalidadeAniel GetByCodigo(string codEscritorio, string codArea, int codLocalidade)
        {
            return this.GetAll()
                       .Where(l => l.Cod_Escritorio == codEscritorio &&
                                   l.Cod_Area == codArea &&
                                   l.Cod_Localidade == codLocalidade)
                       .FirstOrDefault();
        }

        public LocalidadeResponse GetByLocalidade(String Cidade, String Bairro, Config_Aniel_Integrator Config = null)
        {
            StringBuilder sql = new StringBuilder();

            Aniel.Connect.Repository.Model.LocalidadeResponse retorno = null;
            List<LocalidadeResponse> localidades = new List<LocalidadeResponse>();
            using (var com = this._sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT A.COD_CLIENTE, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE, A.LOCALIDADE, A.CODCT, A.PROJETO, B.DESCRICAO AS DESCRICAO_AREA, C.DESCRICAO AS DESCRICAO_ESCRITORIO ")
               .AppendLine("FROM TB_LOCALIDADE A ")
               .AppendLine("INNER JOIN TB_AREA B ")
               .AppendLine("ON (A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA) ")
               .AppendLine("INNER JOIN TB_ESCRITORIO C ")
               .AppendLine("ON (A.COD_ESCRITORIO = C.COD_ESCRITORIO) ")

               .AppendLine(" WHERE A.STATUS = 1 AND B.STATUS = 1 AND C.STATUS = 1 ")
               .AppendLine($" AND CAST(UPPER(B.DESCRICAO) AS VARCHAR(35) CHARACTER SET ISO8859_1) COLLATE PT_BR = UPPER(@CIDADE) ");

                if (!string.IsNullOrEmpty(Bairro))
                    sql.AppendLine($" AND CAST(UPPER(A.LOCALIDADE) AS VARCHAR(50) CHARACTER SET ISO8859_1) COLLATE PT_BR = UPPER(@LOCALIDADE) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                
                parametros.Add("@CIDADE", StringService.Truncate(Cidade.ToUpper(), 35));
                
                if (!string.IsNullOrEmpty(Bairro))
                    parametros.Add("@LOCALIDADE", StringService.Truncate(Bairro.ToUpper(), 50));

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Nh.Connection, null).Result)
                {
                    while (_reader.Read())
                    {
                        localidades.Add(new LocalidadeResponse()
                        {
                            codEscritorio = Convert.ToString(_reader["COD_ESCRITORIO"]),
                            codArea = Convert.ToString(_reader["COD_AREA"]),
                            descricaoLocalidade = Convert.ToString(_reader["LOCALIDADE"]),
                            codLocalidade = _reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(_reader["COD_LOCALIDADE"]) : (int?)null,
                            CodCt = Convert.ToString(_reader["CODCT"]),
                            Projeto = Convert.ToString(_reader["PROJETO"]),
                            DescricaoArea = Convert.ToString(_reader["DESCRICAO_AREA"]) ?? "",
                            DescricaoEscritorio = Convert.ToString(_reader["DESCRICAO_ESCRITORIO"]) ?? "",
                            LocalidadeStr = Convert.ToString(_reader["LOCALIDADE"]) ?? "",
                        });
                    }
                }

                retorno = localidades.FirstOrDefault();
                if (Config != null && retorno != null)
                {
                    if (localidades?.Count > 1 || retorno?.Projeto == null)
                    {
                        retorno.CodCt = null;
                        retorno.Projeto = null;
                    }
                }


                return retorno;
            }
        }


        public LocalidadeResponse GetByCidadeBairro(String Cidade, String Bairro)
        {
            StringBuilder sql = new StringBuilder();

            Aniel.Connect.Repository.Model.LocalidadeResponse retorno = null;
            using (var com = this._sessionT5Nh.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT A.COD_CLIENTE, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE, A.LOCALIDADE, A.CODCT, A.PROJETO, B.DESCRICAO AS DESCRICAO_AREA, C.DESCRICAO AS DESCRICAO_ESCRITORIO ")
               .AppendLine("FROM TB_LOCALIDADE A ")
               .AppendLine("INNER JOIN TB_AREA B ")
               .AppendLine("ON (A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA) ")
               .AppendLine("INNER JOIN TB_ESCRITORIO C ")
               .AppendLine("ON (A.COD_ESCRITORIO = C.COD_ESCRITORIO) ")
               .AppendLine(" WHERE UPPER(B.DESCRICAO) = @CIDADE AND UPPER(A.LOCALIDADE) = @LOCALIDADE ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                parametros.Add("@CIDADE", StringService.Truncate(Cidade.ToUpper(), 35));
                parametros.Add("@LOCALIDADE", StringService.Truncate(Bairro.ToUpper(), 50));

                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Nh.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        retorno = new LocalidadeResponse()
                        {
                            codEscritorio = Convert.ToString(_reader["COD_ESCRITORIO"]),
                            codArea = Convert.ToString(_reader["COD_AREA"]),
                            descricaoLocalidade = Convert.ToString(_reader["LOCALIDADE"]),
                            codLocalidade = _reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(_reader["COD_LOCALIDADE"]) : (int?)null,
                            CodCt = Convert.ToString(_reader["CODCT"]),
                            Projeto = Convert.ToString(_reader["PROJETO"]),
                            DescricaoArea = Convert.ToString(_reader["DESCRICAO_AREA"]) ?? "",
                            DescricaoEscritorio = Convert.ToString(_reader["DESCRICAO_ESCRITORIO"]) ?? "",
                            LocalidadeStr = Convert.ToString(_reader["LOCALIDADE"]) ?? "",
                        };
                    }
                }

                return retorno;
            }
        }
    }
}
