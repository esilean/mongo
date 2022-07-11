namespace Bev.MongoNet.Mongo.Abstractions
{
    public interface IDocumentDatabase
    {
        string Name { get; set; }
        object NativeDatabase { get; set; }
    }

    public class DocumentDatabase : IDocumentDatabase
    {
        public string Name { get; set; }
        public object NativeDatabase { get; set; }
    }
}
