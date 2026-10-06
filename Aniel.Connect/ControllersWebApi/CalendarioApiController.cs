using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class CalendarioApiController : BaseApiController
    {      

        [HttpGet()]
        [Route("api/CalendarioApi/FindById")]
        public async Task<GetApiRetorno<CalendarioCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<CalendarioCadastroModel> retorno = new GetApiRetorno<CalendarioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Calendario>(session))
            {
                try
                {
                    var CalendarioAtend = await repository.selectFirst(p => p.Id == 1);

                    var CalendarioExec = await repository.selectFirst(p => p.Id == 2);

                    retorno.Objeto = new CalendarioCadastroModel
                    {
                        Id = 1,
                        Atend_Domingo_Entrada = CalendarioAtend?.Domingo_Entrada?.ToString("HH:mm"),
                        Atend_Domingo_Saida = CalendarioAtend?.Domingo_Saida?.ToString("HH:mm"),
                        Atend_Segunda_Entrada = CalendarioAtend?.Segunda_Entrada?.ToString("HH:mm"),
                        Atend_Segunda_Saida = CalendarioAtend?.Segunda_Saida?.ToString("HH:mm"),
                        Atend_Terca_Entrada = CalendarioAtend?.Terca_Entrada?.ToString("HH:mm"),
                        Atend_Terca_Saida = CalendarioAtend?.Terca_Saida?.ToString("HH:mm"),
                        Atend_Quarta_Entrada = CalendarioAtend?.Quarta_Entrada?.ToString("HH:mm"),
                        Atend_Quarta_Saida = CalendarioAtend?.Quarta_Saida?.ToString("HH:mm"),
                        Atend_Quinta_Entrada = CalendarioAtend?.Quinta_Entrada?.ToString("HH:mm"),
                        Atend_Quinta_Saida = CalendarioAtend?.Quinta_Saida?.ToString("HH:mm"),
                        Atend_Sexta_Entrada = CalendarioAtend?.Sexta_Entrada?.ToString("HH:mm"),
                        Atend_Sexta_Saida = CalendarioAtend?.Sexta_Saida?.ToString("HH:mm"),
                        Atend_Sabado_Entrada = CalendarioAtend?.Sabado_Entrada?.ToString("HH:mm"),
                        Atend_Sabado_Saida = CalendarioAtend?.Sabado_Saida?.ToString("HH:mm"),

                        Exec_Domingo_Entrada = CalendarioExec?.Domingo_Entrada?.ToString("HH:mm"),
                        Exec_Domingo_Saida = CalendarioExec?.Domingo_Saida?.ToString("HH:mm"),
                        Exec_Segunda_Entrada = CalendarioExec?.Segunda_Entrada?.ToString("HH:mm"),
                        Exec_Segunda_Saida = CalendarioExec?.Segunda_Saida?.ToString("HH:mm"),
                        Exec_Terca_Entrada = CalendarioExec?.Terca_Entrada?.ToString("HH:mm"),
                        Exec_Terca_Saida = CalendarioExec?.Terca_Saida?.ToString("HH:mm"),
                        Exec_Quarta_Entrada = CalendarioExec?.Quarta_Entrada?.ToString("HH:mm"),
                        Exec_Quarta_Saida = CalendarioExec?.Quarta_Saida?.ToString("HH:mm"),
                        Exec_Quinta_Entrada = CalendarioExec?.Quinta_Entrada?.ToString("HH:mm"),
                        Exec_Quinta_Saida = CalendarioExec?.Quinta_Saida?.ToString("HH:mm"),
                        Exec_Sexta_Entrada = CalendarioExec?.Sexta_Entrada?.ToString("HH:mm"),
                        Exec_Sexta_Saida = CalendarioExec?.Sexta_Saida?.ToString("HH:mm"),
                        Exec_Sabado_Entrada = CalendarioExec?.Sabado_Entrada?.ToString("HH:mm"),
                        Exec_Sabado_Saida = CalendarioExec?.Sabado_Saida?.ToString("HH:mm"),
                    };
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/CalendarioApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<CalendarioCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Calendario modeloAtend = null;
                Calendario modeloExec = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Calendario>(session))
                    {
                        modeloAtend = (await repository.select(p => p.Id == 1)).FirstOrDefault();

                        if (modeloAtend == null)
                        {
                            modeloAtend = new Calendario
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modeloAtend.UsuarioAlteracao = value.Usuario;
                        }

                        modeloAtend.Domingo_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Domingo_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Domingo_Entrada) : (DateTime?)null;
                        modeloAtend.Domingo_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Domingo_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Domingo_Saida) : (DateTime?)null;
                        modeloAtend.Segunda_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Segunda_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Segunda_Entrada) : (DateTime?)null; 
                        modeloAtend.Segunda_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Segunda_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Segunda_Saida) : (DateTime?)null; 
                        modeloAtend.Terca_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Terca_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Terca_Entrada) : (DateTime?)null; 
                        modeloAtend.Terca_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Terca_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Terca_Saida) : (DateTime?)null; 
                        modeloAtend.Quarta_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Quarta_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Quarta_Entrada) : (DateTime?)null; 
                        modeloAtend.Quarta_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Quarta_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Quarta_Saida) : (DateTime?)null; 
                        modeloAtend.Quinta_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Quinta_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Quinta_Entrada) : (DateTime?)null; 
                        modeloAtend.Quinta_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Quinta_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Quinta_Saida) : (DateTime?)null; 
                        modeloAtend.Sexta_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Sexta_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Sexta_Entrada) : (DateTime?)null; 
                        modeloAtend.Sexta_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Sexta_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Sexta_Saida) : (DateTime?)null; 
                        modeloAtend.Sabado_Entrada = !String.IsNullOrEmpty(value.Objeto.Atend_Sabado_Entrada) ? Convert.ToDateTime(value.Objeto.Atend_Sabado_Entrada) : (DateTime?)null; 
                        modeloAtend.Sabado_Saida = !String.IsNullOrEmpty(value.Objeto.Atend_Sabado_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Sabado_Saida) : (DateTime?)null; 
                        modeloAtend.Tipo_Calendario = Tipo_Calendario.SLA_Atendimento;
                        await repository.SaveOrUpdate(modeloAtend);


                        modeloExec = (await repository.select(p => p.Id == 2)).FirstOrDefault();

                        if (modeloExec == null)
                        {
                            modeloExec = new Calendario
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modeloExec.UsuarioAlteracao = value.Usuario;
                        }

                        modeloExec.Domingo_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Domingo_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Domingo_Entrada) : (DateTime?)null;
                        modeloExec.Domingo_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Domingo_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Domingo_Saida) : (DateTime?)null; 
                        modeloExec.Segunda_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Segunda_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Segunda_Entrada) : (DateTime?)null; 
                        modeloExec.Segunda_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Segunda_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Segunda_Saida) : (DateTime?)null; 
                        modeloExec.Terca_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Terca_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Terca_Entrada) : (DateTime?)null; 
                        modeloExec.Terca_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Terca_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Terca_Saida) : (DateTime?)null; 
                        modeloExec.Quarta_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Quarta_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Quarta_Entrada) : (DateTime?)null; 
                        modeloExec.Quarta_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Quarta_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Quarta_Saida) : (DateTime?)null; 
                        modeloExec.Quinta_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Quinta_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Quinta_Entrada) : (DateTime?)null; 
                        modeloExec.Quinta_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Quinta_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Quinta_Saida) : (DateTime?)null; 
                        modeloExec.Sexta_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Sexta_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Sexta_Entrada) : (DateTime?)null;
                        modeloExec.Sexta_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Sexta_Saida) ? Convert.ToDateTime(value.Objeto.Exec_Sexta_Saida) : (DateTime?)null; 
                        modeloExec.Sabado_Entrada = !String.IsNullOrEmpty(value.Objeto.Exec_Sabado_Entrada) ? Convert.ToDateTime(value.Objeto.Exec_Sabado_Entrada) : (DateTime?)null; 
                        modeloExec.Sabado_Saida = !String.IsNullOrEmpty(value.Objeto.Exec_Sabado_Saida) ? Convert.ToDateTime(value.Objeto.Atend_Domingo_Saida) : (DateTime?)null; 
                        modeloAtend.Tipo_Calendario = Tipo_Calendario.SLA_Execucao;
                        await repository.SaveOrUpdate(modeloExec);

                    }
                    retorno.IdObjeto = modeloAtend.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

    }
}
