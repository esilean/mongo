using Bev.MongoNet.Mongo.Configs;
using Bev.MongoNet.Mongo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bev.MongoNet.Mongo
{
    public interface IMongoAdapter
    {
        public string DatabaseName { get; set; }

        Task<T> Save<T>(T data, CancellationToken cancellationToken) where T : class;

        Task<T> GetById<T>(string id, CancellationToken cancellationToken) where T : class;

        Task<IEnumerable<T>> Get<T>(string sqlQuery, CancellationToken cancellationToken) where T : class;
    }

    public class MongoAdapter : IMongoAdapter
    {
        private readonly IMongoContext _mongoContext;

        public string DatabaseName { get; set; }

        public MongoAdapter(MongoOptions mongoOptions)
        {
            _mongoContext = new MongoContext(mongoOptions.ConnectionString);
            DatabaseName = mongoOptions.DatabaseName;
        }

        public async Task<T> Save<T>(T data, CancellationToken cancellationToken) where T : class
        {
            var nativeDatabase = _mongoContext.GetDatabase(DatabaseName);
            if (nativeDatabase is null)
            {
                return null;
            }

            var collectionName = GetCollectionName<T>();
            var nativeCollection = _mongoContext.GetCollection(nativeDatabase, collectionName);
            if (nativeCollection is null)
            {
                return null;
            }

            var document = await _mongoContext.Add<T>(nativeCollection, data, cancellationToken);

            return document;
        }

        public async Task<T> GetById<T>(string id, CancellationToken cancellationToken) where T : class
        {
            var nativeDatabase = _mongoContext.GetDatabase(DatabaseName);
            if (nativeDatabase is null)
            {
                return null;
            }

            var collectionName = GetCollectionName<T>();
            var nativeCollection = _mongoContext.GetCollection(nativeDatabase, collectionName);
            if (nativeCollection is null)
            {
                return null;
            }

            var document = await _mongoContext.GetById<T>(nativeCollection, id, cancellationToken);

            return document;
        }

        public async Task<IEnumerable<T>> Get<T>(string sqlQuery, CancellationToken cancellationToken) where T : class
        {
            var nativeDatabase = _mongoContext.GetDatabase(DatabaseName);
            if (nativeDatabase is null)
            {
                return null;
            }

            var collectionName = GetCollectionName<T>();
            var nativeCollection = _mongoContext.GetCollection(nativeDatabase, collectionName);
            if (nativeCollection is null)
            {
                return null;
            }

            var documents = await _mongoContext.Get<T>(nativeCollection, sqlQuery, cancellationToken);

            return documents;
        }

        private string GetCollectionName<T>()
        {
            string collectionName = null;
            var attrs = typeof(T).GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                var collectionNameAttribute = attr as CollectionNameAttribute;
                collectionName = collectionNameAttribute.Name;
            }

            return collectionName ?? typeof(T).Name;
        }
    }
}
