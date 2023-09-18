namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Services.Seo;

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

        public virtual async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            await Task.CompletedTask;

            return string.Empty;
        }

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

        public string GetFinalUrlBySeName(string seName)
        {
            return $"{_webHelper.GetStoreHost(_webHelper.IsCurrentConnectionSecured())}{seName}";
        }

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
