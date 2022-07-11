namespace Bev.MongoNet.Mongo.Abstractions
{
    public interface IDocumentCollection
    {
        string Name { get; set; }
        object NativeCollection { get; set; }
    }

    public class DocumentCollection : IDocumentCollection
    {
        public string Name { get; set; }
        public object NativeCollection { get; set; }
    }
}
