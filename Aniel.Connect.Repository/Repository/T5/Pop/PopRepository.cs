using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
namespace Aniel.Connect.Repository.T5.Equipe
{
    public class PopRepository : IPopRepository
    {
        private readonly ISession _sessionConnNh;

        public PopRepository(ISession sessionConnNh)
        {
            _sessionConnNh = sessionConnNh;
        }
        /// <summary>
        /// Listar Pop
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ID_CIDADE Valor deve ser obtido pelo endpoint CIdade </returns>
        public System.Collections.Generic.List<PopModelsResponse> GetAll()
        {
            List<PopModelsResponse> retorno = new List<PopModelsResponse>();
            try
            {
                Dictionary<String, Object> parametro = new Dictionary<string, object>();
                using (ISession session = HibernateUtil.GetSession())
                {
                    var com = session.Connection.CreateCommand();
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME, A.TOKEN, ");                        
                        sql.AppendLine("       A.BAIRRO, A.ID_CIDADE, A.UF, ");
                        sql.AppendLine("       A.ENDERECO, A.CODCT, A.PROJETO, ");                                                    
                        sql.AppendLine("       A.ID_EMPRESA, A.ID_FILIAL, A.STATUS, ");
                        sql.AppendLine("       A.COMPLEMENTO, A.LATITUDE, A.LONGITUDE, ");
                        sql.AppendLine("       A.CEP, TC.DESCRICAO AS CIDADE, A.NUMERO ");
                        //sql.AppendLine("    A.DESCRICAO,");
                        //sql.AppendLine("    A.CODIGO");
                        sql.AppendLine("FROM TB_POP A INNER JOIN TB_CIDADE TC  ON A.ID_CIDADE = TC.ID  ");
                        using (var reader = CommandHelper.ExecuteReader(com, sql, parametro, session.Connection, null).Result)
                        {
                            while (reader.Read())
                            {
                                retorno.Add(new PopModelsResponse
                                {
                                    ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    TOKEN = reader["TOKEN"] != DBNull.Value ? Convert.ToString(reader["TOKEN"]) : string.Empty,
                                    NOME = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : string.Empty,
                                    CODCT = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : string.Empty,
                                    PROJETO = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : string.Empty,
                                    CEP = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : string.Empty,
                                    ENDERECO = reader["ENDERECO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO"]) : string.Empty,
                                    NUMERO = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : string.Empty,
                                    BAIRRO = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : string.Empty,
                                    ID_CIDADE = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt32(reader["ID_CIDADE"]) : (int?)null,
                                    UF = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : string.Empty,
                                    COMPLEMENTO = reader["COMPLEMENTO"] != DBNull.Value ? Convert.ToString(reader["COMPLEMENTO"]) : string.Empty,
                                    LATITUDE = reader["LATITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LATITUDE"]) : 0,
                                    LONGITUDE = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDecimal(reader["LONGITUDE"]) : 0,
                                    ID_EMPRESA = reader["ID_EMPRESA"] != DBNull.Value ? Convert.ToInt32(reader["ID_EMPRESA"]) : (int?)null,
                                    ID_FILIAL = reader["ID_FILIAL"] != DBNull.Value ? Convert.ToInt32(reader["ID_FILIAL"]) : (int?)null,
                                    STATUS = reader["STATUS"] != DBNull.Value ? Convert.ToInt32(reader["STATUS"]) : (int?)null,
                                    //DESCRICAO = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : string.Empty,
                                    CIDADE = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : string.Empty,
                                    //CODIGO = reader["CODIGO"] != DBNull.Value ? Convert.ToInt32(reader["CODIGO"]) : (int?)null
                                });
                            }
                        }

                    }

                }
            }
            catch (Exception erro)
            {

            }
            return retorno;

        }


        public PostApiRetorno AtualizarPop(PopModels dados)
        {


            PostApiRetorno retorno = new PostApiRetorno();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            using (DbCommand com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                // Monta a query de UPDATE
                sql.Append("UPDATE TB_POP SET ");
                sql.Append("TOKEN = @TOKEN, ");
                sql.Append("NOME = @NOME, ");
                sql.Append("CODCT = @CODCT, ");
                sql.Append("PROJETO = @PROJETO, ");
                sql.Append("CEP = @CEP, ");
                sql.Append("ENDERECO = @ENDERECO, ");
                sql.Append("NUMERO = @NUMERO, ");
                sql.Append("CIDADE = @CIDADE, ");

                sql.Append("BAIRRO = @BAIRRO, ");
                sql.Append("ID_CIDADE = @ID_CIDADE, ");
                sql.Append("UF = @UF, ");
                sql.Append("COMPLEMENTO = @COMPLEMENTO, ");
                sql.Append("LATITUDE = @LATITUDE, ");
                sql.Append("LONGITUDE = @LONGITUDE, ");
                sql.Append("ID_EMPRESA = @ID_EMPRESA, ");
                sql.Append("ID_FILIAL = @ID_FILIAL, ");
                sql.Append("STATUS = @STATUS, ");
                sql.Append("DESCRICAO = @DESCRICAO, ");
                sql.Append("CODIGO = @CODIGO ");
                sql.Append("WHERE ID = @ID"); // Condição para atualizar o registro correto

                // Adiciona os parâmetros
           
                parametros.Add("@CIDADE", dados.CIDADE);

                parametros.Add("@TOKEN", dados.TOKEN);
                parametros.Add("@NOME", dados.NOME);
                parametros.Add("@CODCT", dados.CODCT);
                parametros.Add("@PROJETO", dados.PROJETO);
                parametros.Add("@CEP", dados.CEP);
                parametros.Add("@ENDERECO", dados.ENDERECO);
                parametros.Add("@NUMERO", dados.NUMERO);
                parametros.Add("@BAIRRO", dados.BAIRRO);
                parametros.Add("@ID_CIDADE", dados.ID_CIDADE);
                parametros.Add("@UF", dados.UF);
                parametros.Add("@COMPLEMENTO", dados.COMPLEMENTO);
                parametros.Add("@LATITUDE", dados.LATITUDE);
                parametros.Add("@LONGITUDE", dados.LONGITUDE);
                parametros.Add("@ID_EMPRESA", dados.ID_EMPRESA);
                parametros.Add("@ID_FILIAL", dados.ID_FILIAL);
                parametros.Add("@STATUS", dados.STATUS);
                parametros.Add("@DESCRICAO", dados.DESCRICAO);
                parametros.Add("@CODIGO", dados.CODIGO);
                parametros.Add("@ID", dados.ID); // Condição para atualizar o registro correto

                // Executa o comando de UPDATE
                CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null).Wait();

                retorno.Mensagem= "OK";
                retorno.Ok = true;
            }
            return retorno;

        }




        /// <summary>
        /// Inserir Pop - Campo ID_CIDADE pode ser visto no endPOint /api/CidadeApi/GetAll 
        /// </summary>
        public PostApiRetorno InsertPop(PopModels dados)
        {
            Dictionary<String, Object> parametro = new Dictionary<string, object>();
            var retorno = new PostApiRetorno();
            StringBuilder sql = new StringBuilder();
            string codCt = "";
            var id = 0;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        sql = new StringBuilder();
                        sql.Append("Select MAX(ID) as CO From TB_POP ");
                        {
                            using (var reader = CommandHelper.ExecuteReader(com, sql, parametro, session.Connection, null).Result)
                            {
                                    if (reader.Read())
                                    {
                                        id = Convert.ToInt32(reader["CO"]) + 1;
                                    }
                                
                            }
                        }
                    }
                }
                using (var comIns = _sessionConnNh.Connection.CreateCommand())
                {


                    sql = new StringBuilder();
                    sql.Append("INSERT INTO TB_POP (ID,USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, NOME, CODCT, PROJETO, CEP, ENDERECO, NUMERO,  BAIRRO,CIDADE, ID_CIDADE,UF, COMPLEMENTO, LATITUDE, LONGITUDE, ID_EMPRESA, ID_FILIAL, STATUS, DESCRICAO, CODIGO) ");
                    sql.Append("VALUES ('" + id + "',  'Integracao', current_date, null, current_date, '" + dados.TOKEN + "', '" + dados.NOME + "', '" + dados.CODCT + "', '" + dados.PROJETO + "', '" + dados.CEP + "', '" + dados.ENDERECO + "', '" + dados.NUMERO + "',  '" + dados.BAIRRO + "', '" + dados.CIDADE + "'," + dados.ID_CIDADE + ",  '" + dados.UF + "', '" + dados.COMPLEMENTO + "', " + dados.LATITUDE + ", " + dados.LONGITUDE + ", '" + dados.ID_EMPRESA + "', '" + dados.ID_FILIAL + "', '" + dados.STATUS + "', '" + dados.DESCRICAO + "', '" + dados.CODIGO + "')");
                    CommandHelper.ExecuteNonQuery(comIns, sql, parametro, _sessionConnNh.Connection, null).Wait();

                }

                retorno.Mensagem = "OK";
                retorno.Ok = true;
                return retorno;
            }
            catch (Exception erro)
            {
                retorno.InnerException = erro.Message;
                retorno.Mensagem = "erro";
                retorno.Ok = false;
                return retorno;

            }
        }



        //public bool RemoverTecnicoAuxiliar(string Equipe, Documento_ProducaoAniel documento)
        //{
        //    Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //    using (var comDelete = _sessionT5Nh.Connection.CreateCommand())
        //    {
        //        parametros.Add("@NUM_OBRA", documento.Num_Obra);
        //        parametros.Add("@CODCT", documento.CodCt);
        //        parametros.Add("@PROJETO", documento.Projeto);
        //        parametros.Add("@NUMDOC", documento.Num_Doc);
        //        parametros.Add("@EQUIPE", Equipe);


        //        StringBuilder sqlDelete = new StringBuilder($@"DELETE FROM TB_MOBILE_PARTICIPANTES_RDO 
        //                                                       WHERE EQUIPE = @EQUIPE AND 
        //                                                       CODCT = @CODCT AND 
        //                                                       PROJETO = @PROJETO AND
        //                                                       NUM_DOC = @NUMDOC AND 
        //                                                       NUM_OBRA = @NUM_OBRA");
        //        CommandHelper.ExecuteNonQuery(comDelete, sqlDelete, parametros, _sessionT5Nh.Connection, null).Wait();

        //        return true;
        //    }
        //}



    }
}
