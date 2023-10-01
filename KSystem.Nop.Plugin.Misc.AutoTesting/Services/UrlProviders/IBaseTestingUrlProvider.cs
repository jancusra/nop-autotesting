namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides base method for URL provider
    /// </summary>
    public interface IBaseTestingUrlProvider
    {
        /// <summary>
        /// Base method to get testing URL
        /// </summary>
        /// <param name="parameters">optional URL parameters</param>
        /// <returns>testing URL</returns>
        Task<string> GetTestingUrlAsync(string parameters = null);
    }
}
