using Aniel.Connect.Model.enumerado;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class ServicoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Numero { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }

        public virtual String Status { get; set; }

        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual String Assunto { get; set; }

        public virtual String Pessoa_Contato { get; set; }



        public virtual String Cliente { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Origem { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual String Tipo_Atendimento { get; set; }

        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }

        public virtual String DataHoraFechamento { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Status_Atendimento { get; set; }

        public virtual String Sla { get; set; }

        public virtual Int32? SlaOrdem { get; set; }

        public virtual String Login_Radius { get; set; }
        public virtual String Email_Cliente { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Celular { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Produto { get; set; }
        public virtual String Id_Motivo_Status { get; set; }

        public virtual Boolean Tecnico_Ativos { get; set; }

        public virtual Boolean Tecnico_Em_Execucao { get; set; }
        public virtual String Etapa_Atual { get; set; }
        public virtual String Prazo_Atual { get; set; }
        public virtual String Tempo_Gasto { get; set; }
        public virtual String Data_Sla_Etapa { get; set; }
    }


    public class ServicoChat
    {
        public string num_os { get; set; }

        public string backOfficeId { get; set; }
        public string dataAbertura { get; set; }
        public string dataEncerramento { get; set; }
        public DateTime? dataAgendamento { get; set; }
        public ClienteChat_Mobile cliente { get; set; }
        public List<Mensagem_ChatBackOfficeModels> mensagens { get; set; }
        private List<ServicoUsuario> _usuarios;
        public List<ServicoUsuario> usuarios { get; set; }
        public string codct { get; set; }
        public string projeto { get; set;}
        public int num_doc { get; set; }

        
    }

    public class listaUsuarioOS
    {
        public virtual string num_fre { get; set; }
        public virtual bool ativo { get; set; }
        public virtual string dataDesvinculacao { get; set; }
        public virtual string tipo_usuario { get; set; }
    }

    public class ServicoUsuario
    {
        public virtual string num_fre { get; set; }
        public virtual string nome { get; set; }
        public virtual string foto_usuario { get; set; }
        public virtual bool ativo { get; set; }
        public virtual DateTime? dataDesvinculacao { get; set; }
        public virtual TipoUsuarioChatEnum tipo_usuario { get; set; }
        public virtual string connectionid { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ServicoChatMongo : ServicoChat
    {
        [BsonId()]
        public ObjectId _id { get; set; }

    }

    public class ServicoFiltroModel
    {
        public virtual Int64 Id_Tipo_Atend { get; set; }
        public virtual String Tipo_Atendimento { get; set; }
        public virtual String Etapa_Atual { get; set; }

        public virtual String Responsavel_Atual { get; set; }


        public virtual Int64 Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual Int64 Id_Categoria_Atendimento { get; set; }
        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Setor { get; set; }
        public virtual String Setor { get; set; }


        public virtual Status_Atendimento? Status_Atendimento { get; set; }

        public virtual Int64 Id_Motivo_Status { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String Origem { get; set; }

        public virtual String Plano_Produto { get; set; }

        public virtual String ProximaEtapa { get; set; }
        public virtual Int64? Num_Os { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }


        //Período
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }


        //Prioridade de Atendimento
        public virtual Boolean Baixa { get; set; }
        public virtual Boolean Media { get; set; }
        public virtual Boolean Alta { get; set; }


        //SLA de Atendimento
        public virtual Boolean Dentro_Prazo { get; set; }
        public virtual Boolean Atencao { get; set; }
        public virtual Boolean Vencimento_Proximo { get; set; }
        public virtual Boolean Risco_Perda_Prazo { get; set; }
        public virtual Boolean Prazo_Vencido { get; set; }
    }

    public class ServicoHistoricoCadastroModel
    {
        public virtual Int64 id { get; set; }

        public virtual String numero { get; set; }
        public virtual String assunto { get; set; }

        public virtual String EtapaAtual { get; set; }
        public virtual String ProximaEtapa { get; set; }
        public virtual String tipo_Atendimento { get; set; }


        public virtual String data_Limite { get; set; }

        public virtual String setor { get; set; }

        public virtual String extensao { get; set; }

        public virtual String responsavel { get; set; }

        public virtual Status_Atendimento status { get; set; }

        private IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_atendimento;

        public virtual IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_Atendimento

        {
            get
            {
                if (historico_atendimento == null)
                    historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                return historico_atendimento;
            }
            set
            {
                historico_atendimento = value;
            }
        }
    }

}