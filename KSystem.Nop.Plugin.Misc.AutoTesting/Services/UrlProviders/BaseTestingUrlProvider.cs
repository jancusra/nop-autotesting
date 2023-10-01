namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Services.Seo;

    /// <summary>
    /// Provides base method for URL provider
    /// </summary>
    public class BaseTestingUrlProvider : IBaseTestingUrlProvider
    {
        private readonly IUrlRecordService _urlRecordService;

        private readonly IWebHelper _webHelper;

        private readonly IWorkContext _workContext;

        public BaseTestingUrlProvider(
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        /// <summary>
        /// Base method to get testing URL
        /// </summary>
        /// <param name="parameters">optional URL parameters</param>
        /// <returns>testing URL</returns>
        public virtual async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            await Task.CompletedTask;

            return string.Empty;
        }

        /// <summary>
        /// Select one random URL slug by entity identifiers and type
        /// </summary>
        /// <param name="entityIds">list of entity identifiers</param>
        /// <param name="entityType">name of entity type</param>
        /// <returns>one random URL slug</returns>
        public async Task<string> SelectOneRandomSeNameByIdsAndTypeAsync(List<int> entityIds, string entityType)
        {
            if (entityIds.Count > 0)
            {
                Random random = new Random();
                var randomIndex = random.Next(entityIds.Count);

                return await _urlRecordService.GetSeNameAsync(entityIds[randomIndex], entityType, (await _workContext.GetWorkingLanguageAsync()).Id);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get final complete URL by slug
        /// </summary>
        /// <param name="seName">URL slug name</param>
        /// <returns>complete URL</returns>
        public string GetFinalUrlBySeName(string seName)
        {
            return $"{_webHelper.GetStoreHost(_webHelper.IsCurrentConnectionSecured())}{seName}";
        }

        /// <summary>
        /// Get query URL parameter value by name
        /// </summary>
        /// <param name="parameterName">URL parameter name</param>
        /// <param name="parameters">string of all parameters</param>
        /// <returns>URL parameter value</returns>
        public string GetQueryParameterByName(string parameterName, string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var queryParams = parameters.Split(AutoTestingDefaults.ParameterQuerySeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var queryParam in queryParams)
                {
                    var paramNameValue = queryParam.Split(AutoTestingDefaults.ParameterQueryNameValueSeparator, 
                        StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (paramNameValue.Count == 2 && paramNameValue[0] == parameterName)
                    {
                        return paramNameValue[1];
                    }
                }
            }

            return string.Empty;
        }
    }
}
