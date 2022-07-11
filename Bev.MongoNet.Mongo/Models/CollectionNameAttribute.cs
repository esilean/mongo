using System;

namespace Bev.MongoNet.Mongo.Models
{
    public class CollectionNameAttribute : Attribute
    {
        public string Name { get; }

        public CollectionNameAttribute(string name)
        {
            Name = name;
        }
    }
}
