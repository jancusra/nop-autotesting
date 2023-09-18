namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Threading.Tasks;

    public interface IBaseTestingUrlProvider
    {
        Task<string> GetTestingUrlAsync(string parameters = null);
    }
}
