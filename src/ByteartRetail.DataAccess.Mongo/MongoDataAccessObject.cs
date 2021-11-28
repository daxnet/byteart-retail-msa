using ByteartRetail.Common;
using ByteartRetail.Common.DataAccess;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.DataAccess.Mongo
{
    public sealed class MongoDataAccessObject : IDataAccessObject
    {

        #region Private Fields

        private readonly IMongoClient _client;
        private readonly ConventionPack _conventionPack;
        private readonly string _databaseName;

        #endregion Private Fields

        #region Public Constructors

        public MongoDataAccessObject(MongoUrl url, string databaseName)
        {
            _client = new MongoClient(url);
            _databaseName = databaseName;
            _conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", _conventionPack, _ => true);
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            var collection = GetCollection<TEntity>();
            var options = new InsertOneOptions { BypassDocumentValidation = true };
            await collection.InsertOneAsync(entity, options, cancellationToken);
        }

        public async Task DeleteByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            var filterDefinition = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            await GetCollection<TEntity>().DeleteOneAsync(filterDefinition, cancellationToken);
        }

        public void Dispose() { }

        public async Task<PagedResult<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> expr,
            Expression<Func<TEntity, object>> sortExpression,
            SortOrder sortOrder = SortOrder.Ascending,
            int pageSize = 25,
            int pageNumber = 1,
            string? projectionFields = null,
            CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            var collection = GetCollection<TEntity>();
            var sortDefinition = sortOrder switch
            {
                SortOrder.Ascending => Builders<TEntity>.Sort.Ascending(sortExpression),
                _ => Builders<TEntity>.Sort.Descending(sortExpression)
            };

            ProjectionDefinition<TEntity>? projectionDefintion = null;
            if (projectionFields != null)
            {
                var projectionDefinitionBuilder = Builders<TEntity>.Projection;
                var fields = projectionFields.Split(',');
                if (fields.Any())
                {
                    var projectionFieldDefinitions = fields.Select(f => projectionDefinitionBuilder.Include(f));
                    projectionDefintion = projectionDefinitionBuilder.Combine(projectionFieldDefinitions);
                }
            }

            var (totalPages, totalCount, data) = await collection.AggregateByPage(
                Builders<TEntity>.Filter.Where(expr),
                sortDefinition,
                pageNumber,
                pageSize,
                projectionDefintion,
                cancellationToken);

            return new PagedResult<TEntity>(data, pageNumber, pageSize, totalCount, totalPages);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            var filterDefinition = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            return await GetCollection<TEntity>().Find(filterDefinition).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateByIdAsync<TEntity>(Guid id, TEntity entity, CancellationToken cancellationToken = default)
             where TEntity : IEntity
        {
            var filterDefinition = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            await GetCollection<TEntity>().ReplaceOneAsync(filterDefinition, entity, cancellationToken: cancellationToken);
        }

        public async Task UpdateMultipleAsync<TEntity>(
            Expression<Func<TEntity, bool>> filterExpr,
            Expression<Func<TEntity, object>> updateExpr,
            object updatedValue,
            CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            var filter = Builders<TEntity>.Filter.Where(filterExpr);
            var update = Builders<TEntity>.Update.Set(updateExpr, updatedValue);
            await GetCollection<TEntity>().UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private static string NormalizedCollectionName<TEntity>() where TEntity : IEntity => $"{typeof(TEntity).Name[0].ToString().ToLower()}{typeof(TEntity).Name[1..]}";

        private IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : IEntity
        {
            var database = this._client.GetDatabase(_databaseName);
            return database.GetCollection<TEntity>(NormalizedCollectionName<TEntity>());
        }

        #endregion Private Methods

    }
}
