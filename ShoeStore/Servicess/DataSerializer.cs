using ShoeStore.Servicess.Impl;
using System.Text.Json;

namespace ShoeStore.Servicess
{
    public class DataSerializer : IDataSerializer
    {
        public void Serializer<T>(T dataSource, string componentName)
        {
            string json = JsonSerializer.Serialize(dataSource, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.WriteLine($"{componentName}: {json}");
        }
    }
}
