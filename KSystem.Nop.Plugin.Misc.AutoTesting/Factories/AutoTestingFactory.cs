namespace KSystem.Nop.Plugin.Misc.AutoTesting.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using KSystem.Nop.Plugin.Misc.AutoTesting.Domain;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Enums;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Models;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Services;
    using KSystem.Nop.Plugin.Misc.AutoTesting.Shared;

    using global::Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

    public class AutoTestingFactory : IAutoTestingFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITestingPageService _testingPageService;

        private readonly ITestingTaskService _testingTaskService;

        public AutoTestingFactory(
            IHttpContextAccessor httpContextAccessor,
            ITestingPageService testingPageService,
            ITestingTaskService testingTaskService)
        {
            _httpContextAccessor = httpContextAccessor;
            _testingPageService = testingPageService;
            _testingTaskService = testingTaskService;
        }

        public virtual async Task<TestingWidgetModel> PrepareTestingWidgetModelAsync(int testingTaskPageMapId)
        {
            var testingTaskPageMap = await _testingTaskService.GetTestingTaskPageMapByIdAsync(testingTaskPageMapId);
            TestingWidgetModel testingModel = null;

            if (testingTaskPageMap != null)
            {
                var testingCommands = await (await _testingPageService.GetAllTestingCommandsByPageIdAsync(testingTaskPageMap.PageId)).ToListAsync();
                var nextTaskPageMap = await _testingTaskService.GetNextTestingTaskPageMapByMapAsync(testingTaskPageMap);
                testingModel = new TestingWidgetModel();

                if (nextTaskPageMap != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString(AutoTestingDefaults.NextTaskPageSessionKey, nextTaskPageMap.Id.ToString());
                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.SetString(AutoTestingDefaults.NextTaskPageSessionKey, string.Empty);
                }

                await PrepareBaseTestingCommandsWidgetModelAsync(testingModel, testingTaskPageMap, testingCommands);

                await PrepareSectionTestingWidgetModelAsync(CommandType.AjaxComplete, CommandType.AjaxCompleteEnd,
                    testingTaskPageMap, testingCommands, testingModel.AjaxTestingCommands);

                await PrepareSectionTestingWidgetModelAsync(CommandType.DOMNodeInserted, CommandType.DOMNodeInsertedEnd,
                    testingTaskPageMap, testingCommands, testingModel.DOMNodeInsertedTestingCommands);
            }

            return testingModel;
        }

        public virtual async Task<TestingWidgetModel> ValidateTaskPageAndPrepareTestingWidgetModelAsync(int testingTaskPageMapId, string actualPath)
        {
            var testingTaskPageMap = await _testingTaskService.GetTestingTaskPageMapByIdAsync(testingTaskPageMapId);
            TestingWidgetModel testingModel = null;

            if (testingTaskPageMap != null)
            {
                var testingPage = await _testingPageService.GetTestingPageByIdAsync(testingTaskPageMap.PageId);

                if (testingPage != null && actualPath == testingPage.TestingUrl)
                {
                    return await PrepareTestingWidgetModelAsync(testingTaskPageMapId);
                }
            }

            return testingModel;
        }

        private async Task PrepareSectionTestingWidgetModelAsync(
            CommandType startCommandType,
            CommandType endCommandType,
            TestingTaskPageMap testingTaskPageMap,
            List<TestingCommand> testingCommands,
            IList<TestingSectionModel> testingSectionModel)
        {
            var sectionCommandsCount = testingCommands.Where(x => x.CommandTypeId == (int)startCommandType).Count();

            if (sectionCommandsCount > default(int))
            {
                var commandIndex = testingCommands.FindIndex(x => x.CommandTypeId == (int)startCommandType);

                for (int sectionIndex = default(int); sectionIndex < sectionCommandsCount; sectionIndex++)
                {
                    var sectionModel = new TestingSectionModel();

                    for (int sectionCommandIndex = commandIndex; sectionCommandIndex < testingCommands.Count; sectionCommandIndex++)
                    {
                        if (testingCommands[sectionCommandIndex].CommandTypeId == (int)startCommandType)
                        {
                            sectionModel.BaseCommand = testingCommands[sectionCommandIndex].ToModel<TestingCommandWidgetModel>();
                        }
                        else if (testingCommands[sectionCommandIndex].CommandTypeId != (int)endCommandType)
                        {
                            if (testingCommands[sectionCommandIndex].CommandTypeId != (int)CommandType.SwitchToNextPage)
                            {
                                sectionModel.TestingCommands.Add(testingCommands[sectionCommandIndex].ToModel<TestingCommandWidgetModel>());
                            }
                            else
                            {
                                sectionModel.TestingCommands.Add(await PrepareNextPageCommandModel(testingTaskPageMap));
                            }
                        }
                        else
                        {
                            commandIndex++;
                            break;
                        }

                        commandIndex++;
                    }

                    testingSectionModel.Add(sectionModel);
                }
            }
        }

        private async Task PrepareBaseTestingCommandsWidgetModelAsync(
            TestingWidgetModel testingModel,
            TestingTaskPageMap testingTaskPageMap,
            List<TestingCommand> testingCommands)
        {
            foreach (var testingCommand in testingCommands)
            {
                if (testingCommand.CommandTypeId != (int)CommandType.AjaxComplete && testingCommand.CommandTypeId != (int)CommandType.DOMNodeInserted)
                {
                    if (testingCommand.CommandTypeId != (int)CommandType.SwitchToNextPage)
                    {
                        testingModel.BaseTestingCommands.Add(testingCommand.ToModel<TestingCommandWidgetModel>());
                    }
                    else
                    {
                        testingModel.BaseTestingCommands.Add(await PrepareNextPageCommandModel(testingTaskPageMap));
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private async Task<TestingCommandWidgetModel> PrepareNextPageCommandModel(TestingTaskPageMap taskPageMap)
        {
            var nextTaskPageMap = await _testingTaskService.GetNextTestingTaskPageMapByMapAsync(taskPageMap);
            var redirectToUrl = string.Empty;

            if (nextTaskPageMap != null)
            {
                var testingUrl = await _testingPageService.GetTestingUrlByPageIdAsync(nextTaskPageMap.PageId);
                var parameterDelimeter = _testingPageService.GetTestingUrlParameterDelimeter(testingUrl);

                redirectToUrl = $"{testingUrl}{parameterDelimeter}{AutoTestingDefaults.TestingTaskPageUrlParameterName}={nextTaskPageMap.Id}";
            }

            return new TestingCommandWidgetModel
            {
                PageId = taskPageMap.PageId,
                CommandType = CommandType.SwitchToNextPage,
                Selector = redirectToUrl
            };
        }
    }
}
