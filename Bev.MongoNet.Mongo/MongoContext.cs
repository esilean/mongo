using Bev.MongoNet.Mongo.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bev.MongoNet.Mongo
{
    internal interface IMongoContext
    {
        IDocumentDatabase GetDatabase(string databaseName);

        IDocumentCollection GetCollection(IDocumentDatabase database, string collectionName);

        Task<T> GetById<T>(IDocumentCollection collection, string id, CancellationToken cancellationToken);

        Task<IEnumerable<T>> Get<T>(IDocumentCollection collection, string sqlQuery, CancellationToken cancellationToken);

        Task<T> Add<T>(IDocumentCollection collection, T data, CancellationToken cancellationToken) where T : class;

        Task<T> Update<T>(IDocumentCollection collection, T data, CancellationToken cancellationToken) where T : class, IDocument;
    }

    internal class MongoContext : IMongoContext
    {
        public IMongoClient Client { get; }

        public MongoContext(string connectionString)
        {
            Client = new MongoClient(connectionString);
        }

        public IDocumentDatabase GetDatabase(string databaseName)
        {
            var nativeDatabase = Client.GetDatabase(databaseName, new MongoDatabaseSettings());

            return new DocumentDatabase
            {
                Name = nativeDatabase.DatabaseNamespace.DatabaseName,
                NativeDatabase = nativeDatabase
            };
        }

        public IDocumentCollection GetCollection(IDocumentDatabase database, string collectionName)
        {
            if (database.NativeDatabase is not IMongoDatabase nativeDatabase)
            {
                return null;
            }

            var nativeCollection = nativeDatabase.GetCollection<BsonDocument>(collectionName);

            return new DocumentCollection
            {
                Name = nativeCollection.CollectionNamespace.CollectionName,
                NativeCollection = nativeCollection
            };
        }

        public async Task<T> GetById<T>(IDocumentCollection collection, string id, CancellationToken cancellationToken)
        {
            if (collection?.NativeCollection is not IMongoCollection<BsonDocument> nativeColletion)
            {
                return default;
            }

            var filter = GenerateIdFilter(id);
            var bsonDocument = await nativeColletion.FindAsync<T>(filter: filter,
                                                               cancellationToken: cancellationToken);

            var document = await bsonDocument.FirstOrDefaultAsync();

            return document;
        }

        public async Task<IEnumerable<T>> Get<T>(IDocumentCollection collection, string sqlQuery, CancellationToken cancellationToken)
        {
            if (collection?.NativeCollection is not IMongoCollection<BsonDocument> nativeColletion)
            {
                return null;
            }

            var bsonDocuments = await nativeColletion.FindAsync<T>(filter: sqlQuery,
                                                                   cancellationToken: cancellationToken);

            var documents = await bsonDocuments.ToListAsync();

            return documents;
        }

        public async Task<T> Add<T>(IDocumentCollection collection, T data, CancellationToken cancellationToken) where T : class
        {
            if (collection?.NativeCollection is not IMongoCollection<BsonDocument> nativeColletion)
            {
                return null;
            }

            var bsonData = data.ToBsonDocument();
            await nativeColletion.InsertOneAsync(bsonData, options: null, cancellationToken);

            return data;
        }

        public async Task<T> Update<T>(IDocumentCollection collection, T data, CancellationToken cancellationToken) where T : class, IDocument
        {
            if (collection?.NativeCollection is not IMongoCollection<BsonDocument> nativeColletion)
            {
                return null;
            }

            var filter = GenerateIdFilter(data.Id);
            var bsonData = data.ToBsonDocument();
            await nativeColletion.FindOneAndUpdateAsync(filter, update: bsonData, options: null, cancellationToken);

            return data;
        }

        private FilterDefinition<BsonDocument> GenerateIdFilter(string id)
        {
            FilterDefinition<BsonDocument> filter = $"{{ _id : '{id}' }}";
            return filter;
        }
    }
}
