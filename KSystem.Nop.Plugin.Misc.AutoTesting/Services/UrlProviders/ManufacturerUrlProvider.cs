namespace KSystem.Nop.Plugin.Misc.AutoTesting.Services.UrlProviders
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Nop.Core;
    using global::Nop.Core.Domain.Catalog;
    using global::Nop.Services.Catalog;
    using global::Nop.Services.Seo;

    public class ManufacturerUrlProvider : BaseTestingUrlProvider, IManufacturerUrlProvider
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerUrlProvider(
            IManufacturerService manufacturerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext)
            : base(urlRecordService,
                  webHelper,
                  workContext)
        {
            _manufacturerService = manufacturerService;
        }

        public override async Task<string> GetTestingUrlAsync(string parameters = null)
        {
            var manufacturerIds = (await _manufacturerService.GetAllManufacturersAsync()).Select(x => x.Id).ToList();
            var randomManufacturerSeName = await base.SelectOneRandomSeNameByIdsAndTypeAsync(manufacturerIds, nameof(Manufacturer));

            if (!string.IsNullOrEmpty(randomManufacturerSeName))
            {
                return base.GetFinalUrlBySeName(randomManufacturerSeName);
            }

            return string.Empty;
        }
    }
}
