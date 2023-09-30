namespace KSystem.Nop.Plugin.Misc.AutoTesting.Infrastructure
{
    using AutoMapper;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Enums;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TaskReports;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingPages;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models.TestingTasks;

    using global::Nop.Core.Infrastructure.Mapper;

    /// <summary>
    /// Represents mapping configuration for domain models to DTO models
    /// </summary>
    public class AutoTestingMapper : Profile, IOrderedMapperProfile
    {
        public int Order => 10;

        public AutoTestingMapper()
        {
            AutoTestingModelMaps();
        }

        protected virtual void AutoTestingModelMaps()
        {
            CreateMap<AutoTestingSettings, AutoTestingConfigureModel>().ReverseMap();

            CreateMap<TestingCommand, TestingCommandModel>().ReverseMap();
            CreateMap<TestingPage, TestingPageModel>().ReverseMap();

            CreateMap<TestingCommand, TestingCommandWidgetModel>()
                .AfterMap((src, dest) => dest.CommandType = (CommandType)src.CommandTypeId);

            CreateMap<TestingTask, TestingTaskModel>().ReverseMap();
            CreateMap<TestingTaskPageMap, TestingTaskPageModel>().ReverseMap();

            CreateMap<ExecutedTask, ExecutedTaskModel>().ReverseMap();
            CreateMap<ReportedMessage, ReportedMessageModel>().ReverseMap();
        }
    }
}
