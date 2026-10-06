using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NetTools;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Twilio.TwiML.Voice;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Pool_Ip_EnderecoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Pool_Ip_Enderecoapi/GetAllByPoolId")]
        public async Task<DataTableResponse<Pool_Ip_EnderecoModels>> GetAllByPoolId(Pool_Ip_Endereco_DataTableRequest request)
        {
            List<Pool_Ip_EnderecoModels> retorno = new List<Pool_Ip_EnderecoModels>();
            IOrderedEnumerable<Pool_Ip_EnderecoModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                bool poolIPFixo = false;
                string ipInicioRange = string.Empty;
                string ipFimRange = string.Empty;
                string cidr = string.Empty;

                //Coleta dados da pool como pool de IP fixo, IP início, IP final da range e Máscara de IP
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT IP_INICIO_RANGE, IP_FIM_RANGE, POOL_IP_FIXO, MASCARA_IP_CIDR FROM TB_POOL_IP");
                    sql.AppendLine(" WHERE ID = @ID_POOL_IP ");

                    parametros.Add("@ID_POOL_IP", request.Id_Pool_Ip);


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            
                            poolIPFixo = reader["POOL_IP_FIXO"] != DBNull.Value && Convert.ToBoolean(reader["POOL_IP_FIXO"]);
                            ipInicioRange = reader["IP_INICIO_RANGE"] != DBNull.Value ? reader["IP_INICIO_RANGE"].ToString() : string.Empty;
                            ipFimRange = reader["IP_FIM_RANGE"] != DBNull.Value ? reader["IP_FIM_RANGE"].ToString() : string.Empty;
                            cidr = reader["MASCARA_IP_CIDR"] != DBNull.Value ? reader["MASCARA_IP_CIDR"].ToString() : string.Empty;
                        }
                    }

                }
                
                //Se não for pool de IP fixo irá gerar os IPs via código
                //ao invés de consultar na tabela
                if(poolIPFixo == false)
                {
                    long idCounter = 1;
                    if (String.IsNullOrEmpty(cidr)) //Se range for definida pelo IP inicial e final da range, cairá nesta condição
                    {
                        IPAddressRange ipRange = new IPAddressRange(IPAddress.Parse(ipInicioRange), IPAddress.Parse(ipFimRange));

                        int start = request.start;
                        int length = request.length;

                        //Lógica para paginação
                        var ipList = ipRange.AsEnumerable()
                                            .Skip(start)
                                            .Take(length)
                                            .ToList();


                        foreach (IPAddress ip in ipList)
                        {
                            retorno.Add(new Pool_Ip_EnderecoModels
                            {
                                Id = idCounter++,
                                Ip = Convert.ToString(ip),
                                Usuario_Login_Radius = string.Empty,
                                Id_Login_Radius = (long?)null,
                                Desc_Status_Ip = "Disponivel"
                            });
                        }

                        recordsTotal = ipRange.AsEnumerable().Count();
                    }
                    else //Se range for definida pela máscara de IP, cairá nesta condição
                    {
                        string ipComMascara = $"{ipInicioRange}/{cidr}";
                        IPAddressRange ipRange = IPAddressRange.Parse(ipComMascara);

             
                        int start = request.start; 
                        int length = request.length; 

                       //Lógica para paginação
                        var ipList = ipRange.AsEnumerable()
                                            .Skip(start) 
                                            .Take(length) 
                                            .ToList();

                        foreach (IPAddress ip in ipList)
                        {
                            retorno.Add(new Pool_Ip_EnderecoModels
                            {
                                Id = idCounter++,
                                Ip = ip.ToString(),
                                Usuario_Login_Radius = string.Empty,
                                Id_Login_Radius = (long?)null,
                                Desc_Status_Ip = "Disponivel"
                            });
                        }

                        recordsTotal = ipRange.AsEnumerable().Count();
                    }

                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // Ip
                                {
                                    if (sortDirection == "asc")
                                        listaOrdenada = retorno.OrderBy(p => p.Id);
                                    else
                                        listaOrdenada = retorno.OrderByDescending(p => p.Id);
                                    break;
                                }
                        }
                    }


                }
                else
                {
                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_POOL_IP_ENDERECO A ")
                    .AppendLine(" LEFT JOIN TB_LOGIN_RADIUS B ON (A.ID_LOGIN_RADIUS = B.ID) ")
                    .AppendLine(" WHERE A.ID_POOL_IP = @ID_POOL_IP ");

                    //parametros.Add("@ID_POOL_IP", request.Id_Pool_Ip);

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

                        sql.AppendLine(" SELECT A.ID, A.IP, A.STATUS_IP, A.ID_LOGIN_RADIUS, COALESCE(B.LOGIN, '-') AS LOGIN_RADIUS ");
                        sql.AppendLine(sqlFrom.ToString());
                        //sql.AppendLine(" FROM TB_POOL_IP_ENDERECO A ");
                        //sql.AppendLine(" LEFT JOIN TB_LOGIN_RADIUS B ON (A.ID_LOGIN_RADIUS = B.ID) ");
                        //sql.AppendLine($" WHERE A.ID_POOL_IP = {request.Id_Pool_Ip} ");

                        //Filtrando
                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            sql.AppendLine("AND ((UPPER(A.IP) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        }


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Pool_Ip_EnderecoModels
                                {
                                    Ip = Convert.ToString(reader["IP"]),
                                    Usuario_Login_Radius = Convert.ToString(reader["LOGIN_RADIUS"]),
                                    Id_Login_Radius = reader["ID_LOGIN_RADIUS"] != DBNull.Value ? Convert.ToInt64(reader["ID_LOGIN_RADIUS"]) : (long?)null,
                                    Desc_Status_Ip = HtmlEnumExtensions.GetEnumDescription<Pool_Ip_Range_StatusEnum>(Convert.ToString(reader["STATUS_IP"]))
                                });
                            }
                        }

                    }

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // Ip
                                {
                                    if (sortDirection == "asc")
                                        listaOrdenada = retorno.OrderBy(p => p.Id);
                                    else
                                        listaOrdenada = retorno.OrderByDescending(p => p.Id);
                                    break;
                                }
                        }
                    }
                }
            }

                  
            //Page e retornando DataTable
            return new DataTableResponse<Pool_Ip_EnderecoModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Pool_Ip_Enderecoapi/GetAll")]
        public async Task<DataTableResponse<Pool_Ip_EnderecoModels>> GetAll(Pool_Ip_Endereco_DataTableRequest request)
        {
            List<Pool_Ip_EnderecoModels> retorno = new List<Pool_Ip_EnderecoModels>();
            IOrderedEnumerable<Pool_Ip_EnderecoModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_POOL_IP_ENDERECO A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_LOGIN_RADIUS B ON (A.ID_LOGIN_RADIUS = B.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_POOL_IP C ON (A.ID_POOL_IP = C.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CONCENTRADORA D ON (B.ID_CONCENTRADORA = D.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_PON E ON (B.ID_PON = E.ID) ");
                sqlFrom.AppendLine(" WHERE C.STATUS = 1 ");

                if (request.Id_Pool_Ip.HasValue)
                    sqlFrom.AppendLine($" AND A.ID_POOL_IP = {request.Id_Pool_Ip} ");

                if (request.Id_Concentradora.HasValue)
                    sqlFrom.AppendLine($" AND D.ID = {request.Id_Concentradora} ");

                if (request.Status_Ip.HasValue)
                    sqlFrom.AppendLine($" AND A.STATUS_IP = {request.Status_Ip} ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "ip")
                    {
                        sqlFrom.AppendLine(" AND (A.IP LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "login")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(B.LOGIN) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "pool_ip")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(C.DESCRICAO) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "mac")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(B.MAC) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
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

                    sql.AppendLine(" SELECT A.ID, A.IP, A.STATUS_IP, A.ID_LOGIN_RADIUS, B.LOGIN, COALESCE(B.MAC, '-') AS MAC, ");
                    sql.AppendLine(" COALESCE(E.INTERFACE,'-') AS CONEXAO, C.DESCRICAO AS DESC_POOL_IP, D.IP AS IP_CONCENTRADORA ");
                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Pool_Ip_EnderecoModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Ip = Convert.ToString(reader["IP"]),
                                Usuario_Login_Radius = Convert.ToString(reader["LOGIN"]),
                                Id_Login_Radius = reader["ID_LOGIN_RADIUS"] != DBNull.Value ? Convert.ToInt64(reader["ID_LOGIN_RADIUS"]) : (long?)null,
                                Desc_Status_Ip = HtmlEnumExtensions.GetEnumDescription<Pool_Ip_Range_StatusEnum>(Convert.ToString(reader["STATUS_IP"])),
                                Ip_Concentradora = Convert.ToString(reader["IP_CONCENTRADORA"]),
                                Desc_Pool_Ip = Convert.ToString(reader["DESC_POOL_IP"]),
                                Mac = Convert.ToString(reader["MAC"]),
                                Conexao = Convert.ToString(reader["CONEXAO"])
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Ip
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Pool_Ip_EnderecoModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/pool_Ip_Enderecoapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_POOL_IP_ENDERECO A ");

                    List<String> condicoes = new List<String>();
                    String condicaoId = String.Empty;                   

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id} ");
                    }
                    else
                    {
                        //Filtrar Pools que possuem IPs disponíveis
                        //Não pode filtrar junto com o request.id pois o IP dele já estará ocupado.
                        condicoes.Add($"A.STATUS_IP = 1 ");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_POOL_IP = {request.idpai}");                      
                    }

                    

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(A.IP LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {                        
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.IP  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.ID ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["IP"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["IP"])
                                    });

                                }
                            }
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

        public async Task<PostApiRetorno<Int64>> Gravar(Pool_Ip_EnderecoModels value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<long>();

            try
            {

            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        /// <summary>
        /// Grava o Range de IPs na tabela. Em caso de edição, apenas insere os novos IPs do Range.
        /// </summary>
        /// <param name="rangeIps">Range dos IPs.</param>
        /// <param name="Id_Pool_Ip">Id do Pool ao qual pertence o range de IPs.</param>
        /// <param name="isEdicao">Se false, insere o range completo. Se true, insere apenas os novos IPs do range.</param>
        /// <returns></returns>
        public async Task<PostApiRetorno<Int64>> GravarRangeIps(IPAddressRange rangeIps, Int64 Id_Pool_Ip, bool isEdicao, bool poolIpFixo)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<long>();

            try
            {   if(poolIpFixo == true)
                {
                    List<Pool_Ip_Endereco> listaIps = new List<Pool_Ip_Endereco>();

                    //O IP só será gravado no banco caso seja de pool de IP fixo
                    //pois no momento há uma lógica específica que gera os IPs
                    //dinâmicos, por código, para serem exibidos na tela de 
                    //cadastro de Pool de IP
                    if (isEdicao)
                    {

                        using (ISession session = HibernateUtil.GetSession())
                        using (var repoPoolIpEndereco = new Conexao<Pool_Ip_Endereco>(session))
                        {
                            //var ip_inicio_antigo = (await repoPoolIpEndereco.selectSql($"SELECT FIRST 1 ID, IP FROM TB_POOL_IP_ENDERECO WHERE ID_POOL_IP = {Id_Pool_Ip} ORDER BY DATAHORACRIACAO ")).FirstOrDefault();
                            //var ip_fim_antigo = (await repoPoolIpEndereco.selectSql($"SELECT FIRST 1 ID, IP FROM TB_POOL_IP_ENDERECO WHERE ID_POOL_IP = {Id_Pool_Ip} ORDER BY DATAHORACRIACAO DESC ")).FirstOrDefault();

                            var ip_inicio_antigo = session.Query<Pool_Ip_Endereco>()
                            .Where(x => x.Pool_Ip.Id == Id_Pool_Ip)
                            .FirstOrDefault();

                            var ip_fim_antigo = session.Query<Pool_Ip_Endereco>()
                                .Where(x => x.Pool_Ip.Id == Id_Pool_Ip)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();



                            if (ip_inicio_antigo != null && ip_fim_antigo != null)
                            {
                                var ipAddressIni = System.Net.IPAddress.Parse(ip_inicio_antigo.Ip);
                                var ipAddressFim = System.Net.IPAddress.Parse(ip_fim_antigo.Ip);

                                foreach (var ip in rangeIps)
                                {
                                    // Converte o IP para um valor númerico, de byte array para um inteiro 32 bits
                                    var converterIPParaComparar = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);

                                    // Converte o IP para um valor númerico, de byte array para um inteiro 32 bits
                                    var ipIniValue = BitConverter.ToUInt32(ipAddressIni.GetAddressBytes().Reverse().ToArray(), 0);
                                    var ipFimValue = BitConverter.ToUInt32(ipAddressFim.GetAddressBytes().Reverse().ToArray(), 0);

                                    // Realiza a comparação para inserir somentes os novos IPs da range
                                    if (converterIPParaComparar < ipIniValue || converterIPParaComparar > ipFimValue)
                                    {
                                        // Adiciona IP na lista de IPs que serão inseridos no banco
                                        listaIps.Add(new Pool_Ip_Endereco
                                        {
                                            Ip = ip.ToString()
                                        });
                                    }
                                }

                                if (listaIps.Count == 0)
                                {
                                    retorno.Ok = true;
                                    return retorno;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var ip in rangeIps)
                        {
                            listaIps.Add(new Pool_Ip_Endereco
                            {
                                Ip = ip.ToString()
                            });
                        }
                    }

                    var index = 0;
                    var pular = 0;
                    var novoLoteinsert = true;

                    while (novoLoteinsert)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        using (var com = session.Connection.CreateCommand())
                        {
                            var registrosRestantes = listaIps.Count - index;
                            StringBuilder sql = new StringBuilder();
                            Dictionary<string, object> parametros = new Dictionary<string, object>();

                            sql.AppendLine("EXECUTE BLOCK AS BEGIN ");
                            while (index < listaIps.Count)
                            {
                                if (index % 100 == 0 && index > 0)
                                {
                                    index++;
                                    break;
                                }
                                sql.AppendLine("INSERT INTO TB_POOL_IP_ENDERECO (ID, DATAHORACRIACAO, IP, STATUS_IP, ID_POOL_IP) VALUES ");
                                sql.AppendLine($"((SELECT GEN_ID( GN_POOL_IP_ENDERECO, 1 ) FROM RDB$DATABASE), '{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}', '{listaIps[index].Ip}', 1, {Id_Pool_Ip}); ");

                                index++;
                            }
                            sql.AppendLine("END");

                            registrosRestantes = listaIps.Count - index;
                            if (registrosRestantes > 0)
                                novoLoteinsert = true;
                            else
                                novoLoteinsert = false;

                            if (!sql.ToString().Contains("INSERT"))
                                continue;
                            //parametros.Add("@ID_POOL_IP", Id_Pool_Ip);

                            await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        }
                    }
                }
                                            
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/pool_Ip_Enderecoapi/VincularLoginRadius")]
        public async Task<PostApiRetorno<Int64>> VincularLoginRadius(Pool_Ip_Endereco_VincularLoginModels value)
        {
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                using (var repoPoolIpEndereco = new Conexao<Pool_Ip_Endereco>(session))                
                using (var repoPoolIp = new Conexao<Pool_Ip>(session))                
                {
                    if (!value.Id_Pool_Ip.HasValue && !value.Id_Ip.HasValue)
                        throw new EAnielException("Não foi informado Endereço IP ou Pool de IPs.");

                    var clientePlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == value.Id_ClientePlanoProduto);
                    var login = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == clientePlanoProduto.Id);

                    if (login != null)
                    {
                        var pool_ip_endereco_antigo = await repoPoolIpEndereco.selectFirst(p => p.Login.Id == login.Id);

                        if (pool_ip_endereco_antigo != null)
                        {
                            pool_ip_endereco_antigo.Login = null;
                            pool_ip_endereco_antigo.DataHoraAlteracao = DateTime.Now;
                            pool_ip_endereco_antigo.Status_Ip = Pool_Ip_Range_StatusEnum.Disponivel;
                        }

                        if(value.Id_Ip.HasValue) //Novo Endereço IP
                        {
                            var pool_ip_endereco = await repoPoolIpEndereco.selectFirst(p => p.Id == value.Id_Ip);

                            login.Pool_Ip_Endereco = pool_ip_endereco;

                            if (pool_ip_endereco.Pool_Ip.Padrao_Ip == Sinapse.Model.enumerados.Tipo_Ip.IPv4)
                                login.EnderecoIPV4 = pool_ip_endereco.Ip;
                            else
                                login.EnderecoIPV6 = pool_ip_endereco.Ip;

                            await repoLoginRadius.SaveOrUpdate(login);

                            pool_ip_endereco.Login = login;
                            pool_ip_endereco.DataHoraAlteracao = DateTime.Now;
                            pool_ip_endereco.Status_Ip = Pool_Ip_Range_StatusEnum.Ocupado;

                            await repoPoolIpEndereco.SaveOrUpdate(pool_ip_endereco);
                        }
                        else //Novo Pool IP sem definir Endereço IP específico
                        {
                            login.EnderecoIPV4 = null;
                            login.EnderecoIPV6 = null;
                            login.Pool_Ip_Endereco = null;
                            clientePlanoProduto.Pool_Ip = await repoPoolIp.selectFirst(p => p.Id == value.Id_Pool_Ip);
                            await repoClientePlanoProduto.SaveOrUpdate(clientePlanoProduto);
                        }                        

                        var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(login.Concentradora.Radius);
                        if (pool_ip_endereco_antigo != null)
                        { 
                            await repoPoolIpEndereco.SaveOrUpdate(pool_ip_endereco_antigo);                            
                        }


                        await RadiusRepository.AtualizarPoolIpUsuario(anielRadius, sessionRadius, new IntegracaoRadiusApiController().BindObjToLoginRadiusModel(login));
                    }
                    else
                    {
                        throw new Exception("Plano/Produto informado não possui login vinculado");
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/pool_Ip_Enderecoapi/DesvincularLoginRadius")]
        public async Task<PostApiRetorno<Int64>> DesvincularLoginRadius(Pool_Ip_Endereco_DesvincularLoginModel value)
        {
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_POOL_IP_ENDERECO SET ID_LOGIN_RADIUS = NULL, STATUS_IP = 1 WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_LOGIN_RADIUS SET ID_POOL_IP_ENDERECO = NULL WHERE ID_POOL_IP_ENDERECO IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);


                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/pool_Ip_Enderecoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_POOL_IP_ENDERECO SET STATUS_IP = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
    }
}