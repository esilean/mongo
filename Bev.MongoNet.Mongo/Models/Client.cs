using Bev.MongoNet.Mongo.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bev.MongoNet.Mongo.Models
{
    [CollectionName("Clients")]
    public class Client : IDocument
    {
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Salary { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }
    }
}
