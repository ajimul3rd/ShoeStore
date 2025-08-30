namespace ShoeStore.Servicess.Impl
{
    public interface IDataSerializer
    {
        void Serializer<T>(T dataSource, string componentName);
    }
}
