namespace ShoeStore.Servicess.Impl
{
    public interface IAuthHeaderService
    {
        Task AddAuthHeaderAsync(HttpClient httpClient);
    }
}
