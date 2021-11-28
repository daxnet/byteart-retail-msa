using MongoDB.Driver;

namespace ByteartRetail.DataAccess.Mongo
{
    internal static class MongoCollectionQueryExtensions
    {
        public static async Task<(int totalPages, long totalCount, IReadOnlyList<TDocument> data)> AggregateByPage<TDocument>(
            this IMongoCollection<TDocument> collection,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition,
            int page,
            int pageSize,
            ProjectionDefinition<TDocument>? projectionDefinition = null,
            CancellationToken cancellationToken = default)
        {
            var countPipelineDefinition = PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                });

            var dataPipelineDefinition = PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
                });

            var countFacet = AggregateFacet.Create("count", countPipelineDefinition);

            AggregateFacet<TDocument> dataFacet = projectionDefinition != null
                ? AggregateFacet.Create("data", dataPipelineDefinition.Project<TDocument, TDocument, TDocument>(projectionDefinition))
                : AggregateFacet.Create("data", dataPipelineDefinition);

            var aggregation = await collection.Aggregate()
                              .Match(filterDefinition)
                              .Facet(countFacet, dataFacet)
                              .ToListAsync(cancellationToken);

            var count = aggregation.First()
                             .Facets.First(x => x.Name == "count")
                             .Output<AggregateCountResult>()
                             .FirstOrDefault()
                             ?.Count ?? 0;
            var totalPages = (int)Math.Ceiling((double)count / pageSize);

            var data = aggregation.First()
               .Facets.First(x => x.Name == "data")
               .Output<TDocument>();

            return (totalPages, count, data);
        }
    }
}