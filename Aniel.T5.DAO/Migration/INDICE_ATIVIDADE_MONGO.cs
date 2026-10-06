using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Sinapse.DAO;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.Migration
{
    public class INDICE_ATIVIDADE_MONGO : MongoMigration
    {
        public INDICE_ATIVIDADE_MONGO()
        {
            Codigo = "20240930_1136";
            Descricao = "Criar indices para collection AtividadeMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<AtividadeMongo>(this.Conexao))
            {
                IMongoCollection<AtividadeMongo> _collection = repository.GetCollection();

                //Index UuidNotificacao
                IndexKeysDefinition<AtividadeMongo> indexDefinitionUuidNotificacao = Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.UuidNotificacao);

                CreateIndexModel<AtividadeMongo> indexModelUuidNotificacao = new CreateIndexModel<AtividadeMongo>(indexDefinitionUuidNotificacao);

                //Index Equipe
                IndexKeysDefinition<AtividadeMongo> indexDefinitionEquipe = Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.Equipe);

                CreateIndexModel<AtividadeMongo> indexModelEquipe = new CreateIndexModel<AtividadeMongo>(indexDefinitionEquipe);

                //Index Equipes Técnico, Supervisor e Monitor
                IndexKeysDefinition<AtividadeMongo> indexDefinitionComposeEquipes = Builders<AtividadeMongo>.IndexKeys.Combine( Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.Equipe),
                                                                                                                        Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.EquipeSupervisor),
                                                                                                                        Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.EquipeMonitor));

                CreateIndexModel<AtividadeMongo> indexModelComposeEquipes = new CreateIndexModel<AtividadeMongo>(indexDefinitionComposeEquipes);

                //Index DataHoraCriacao
                IndexKeysDefinition<AtividadeMongo> indexDefinitionDataHoraCriacao = Builders<AtividadeMongo>.IndexKeys.Ascending(f => f.DataHoraCriacao);

                CreateIndexModel<AtividadeMongo> indexModelDataHoraCriacao = new CreateIndexModel<AtividadeMongo>(indexDefinitionDataHoraCriacao);

                List<CreateIndexModel<AtividadeMongo>> listaIndexes = new List<CreateIndexModel<AtividadeMongo>>();

                listaIndexes.Add(indexModelUuidNotificacao);
                listaIndexes.Add(indexModelEquipe);
                listaIndexes.Add(indexModelComposeEquipes);
                listaIndexes.Add(indexModelDataHoraCriacao);

                _collection.Indexes.CreateManyAsync(listaIndexes);
            }
        }
    }
}
