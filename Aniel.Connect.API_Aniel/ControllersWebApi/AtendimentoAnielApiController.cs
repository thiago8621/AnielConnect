using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Sinapse.Model.Identity;
using Sinapse.Model;



namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class AtendimentoAnielApiController : BaseApiController
    {
        /// <summary>
        ///Cadastrar novos Registro sde Atendimento via Api 
        ///</summary>
        ///<param name = "request"> </param> 
        /// <returns></returns>
        [HttpPost]
        [Route("api/AtendimentoAnielApiController/GravarAtendimento")]
        public async Task<ApiResponse<Atendimento_Aniel>> GravarAtendimento(Atendimento_AnielApiRequest request)
        {
            List<Atendimento_Aniel> retorno = new List<Atendimento_Aniel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                #region Condicao
                var cpf = request.Cpf.Replace(".", "").Replace("-", "").Replace("/", "");
                if (String.IsNullOrEmpty(cpf) || cpf.Length > 20)
                { throw new Exception("Houve um problema no campo de CPF."); }

                if (String.IsNullOrEmpty(request.Plano) || request.Plano.Length > 100)
                { throw new Exception("Houve um problema no campo de Nome."); }

                if (String.IsNullOrEmpty(request.TipoAtendimento.ToString()) || request.TipoAtendimento.ToString().Length > 9)
                { throw new Exception("Houve um problema no Campo de CEP."); }

                #endregion

                #region Usando


                using (var session = HibernateUtil.GetSession())
                {
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoCategoria = new Conexao<Model.nhibernate.Categoria_Atendimento>(session))
                    using (var repoTipo = new Conexao<Model.nhibernate.Tipo_Atendimento>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCliPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoPlano = new Conexao<Plano>(session))
                    using (var repoO = new Conexao<Origem_Atendimento>(session))
                    

                    {
                        #endregion

                        #region Atendimento

                        Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                        Atendimento atendimento = new Atendimento();

                        Cliente_PlanoProduto plan = new Cliente_PlanoProduto();

                        plan.Plano = await repoPlano.selectFirst(p => p.Codigo == request.Plano);
                        atendimento.Tipo_Pessoa_Contato = Tipo_Pessoa_Contato.Titular;
                        atendimento.Tipo_Atendimento = await repoTipo.selectFirst(p => p.Id == request.TipoAtendimento);
                        atendimento.Status_Atendimento = Status_Atendimento.A_Iniciar;
                        atendimento.Status = Status_Default.Ativo;

                        //atendimento.Setor = request.Setor;
                        //atendimento.Responsavel = request.Responsavel;                        
                        //atendimento.Problema_Relatado = request.Problema_Relatado;

                        atendimento.Prioridade = request.Prioridade;
                        atendimento.PlanoProduto = await repoCliPlanoProduto.selectFirst(p => p.Id_Venda == request.Venda);
                        atendimento.Origem_Atendimento = await repoO.selectFirst(p=>p.Id == request.Codigo_origem);

                        //atendimento.Pessoa_Contato = request.Pessoa_Contato;

                        if (config.Formato_Protocolo_Atendimento == Formato_Protocolo_Atendimento.Ano_Mes_Dia_Hora_Sequencial)
                            atendimento.Numero = String.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHH"), (await new Conexao<Atendimento>(session).GetSequence("GN_ATENDIMENTO")));
                        else
                            atendimento.Numero = $"{await new Conexao<Atendimento>(session).GetSequence("GN_ATENDIMENTO")}";

                        //aten dimento.Motivo_Status = request.Motivo_Status;
                        //atendimento.Id_Email_Abertura = request.Id_Email_Abertura;
                        //atendimento.Grau_Ligacao = request.Grau_Ligacao;
                        //atendimento.Endereco_Mudanca = request.Endereco_Mudanca;

                        atendimento.Data_Hora_Abertura = DateTime.Now;

                        //atendimento.DataHora_Indicador = ;

                        atendimento.Cliente = await repoCliente.selectFirst( p => p.CpfCnpj == request.Cpf);
                        atendimento.Categoria_Atendimento = await repoCategoria.selectFirst(p => p.Codigo == request.Codigo_Categoria);
                        atendimento.Assunto = request.Assunto;
                        #endregion



                        await repoAtendimento.Save(atendimento);

                        return new ApiResponse<Atendimento_Aniel>()
                        {
                            draw = request.settings.length,
                            data = retorno,
                            error = ""
                        };
                    }
                }
            }

            catch (Exception erro)
            {
                return new ApiResponse<Atendimento_Aniel>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = erro.Message
                };

            }
        }
    }
}