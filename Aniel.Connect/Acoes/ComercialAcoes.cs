using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Util;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Acoes
{
    public class ComercialAcoes : BaseApiController
    {
        public async Task<DataTableResponse<ClienteListViewModel>> ConsultaProspect(AcaoConsultaCliente request)
        {
            List<ClienteListViewModel> retorno = new List<ClienteListViewModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE A ")
                                                .AppendLine("LEFT OUTER JOIN TB_ENDERECO_CLIENTE B ")
                                                .AppendLine("ON(A.ID_ENDERECO_PRINCIPAL = B.ID) ")
                                                .AppendLine("LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C ")
                                                .AppendLine("ON(A.ID_CATEGORIA = C.ID) ")
                                                .AppendLine("LEFT OUTER JOIN AspNetUsers D ")
                                                .AppendLine("ON(A.ID_IDENTITY = D.ID) ")
                                                .AppendLine("LEFT JOIN TB_TIPO_IMOVEL E ")
                                                .AppendLine("ON (B.ID_TIPO_IMOVEL = E.COD_TIPO_IMOVEL) ")
                                                .AppendLine("WHERE A.ID > 0 AND TIPO_CLIENTE = 1");

                //Filtrando
                if (!String.IsNullOrEmpty(request.Identificacao))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.NUMERO_CONTRATO) = @NUMERO_CONTRATO)");
                    parametros.Add("@NUMERO_CONTRATO", request.Identificacao);
                }

                if (!String.IsNullOrEmpty(request.Cpf_Cnpj))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.CPF) = @CPF)");
                    parametros.Add("@CPF", request.Cpf_Cnpj);
                }

                if (!String.IsNullOrEmpty(request.Rg))
                {
                    sqlFrom.AppendLine("AND (UPPER(A.RG) = @RG)");
                    parametros.Add("@RG", request.Rg);
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, C.DESCRICAO AS CATEGORIA_CLIENTE, A.CPF, A.TIPOPESSOA, A.TELEFONE_FIXO, ");
                    sql.AppendLine("A.SEXO, A.GRAU_INSTRUCAO, A.ESTADO_CIVIL,");
                    sql.AppendLine("A.NOMEFANTASIA, B.ENDERECO_BAIRRO,B.ENDERECO_CIDADE, B.ENDERECO_UF, A.STATUS, D.USERNAME, E.DESCRICAO AS TIPO_IMOVEL, C.DESCRICAO AS CATEGORIA ");

                    sql.AppendLine(sqlFrom.ToString());

                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 2:     // Estado
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.CPF)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.CPF) DESC");
                                    break;
                                }
                            case 3:     // Estado
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY UPPER(A.RG)");
                                    else
                                        sql.AppendLine(" ORDER BY UPPER(A.RG) DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader/*First*/(com, sql, parametros, session.Connection, null/*, request.length, request.start*/))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ClienteListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Pendencia = false,
                                Categoria = (Convert.ToString(reader["CATEGORIA"])),
                                CpfCnpj = Convert.ToString(reader["CPF"]),
                                TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Estado_Civil = HtmlEnumExtensions.GetEnumDescription<Estado_Civil>(Convert.ToString(reader["ESTADO_CIVIL"])),
                                Grau_Instrucao = HtmlEnumExtensions.GetEnumDescription<Grau_Instrucao>(Convert.ToString(reader["GRAU_INSTRUCAO"])),
                                Sexo = HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(reader["SEXO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                Login = Convert.ToString(reader["USERNAME"]),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ClienteListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }
    }
}