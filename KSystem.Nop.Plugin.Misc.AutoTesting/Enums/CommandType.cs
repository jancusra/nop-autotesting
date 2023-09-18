namespace KSystem.Nop.Plugin.Misc.AutoTesting.Enums
{
    public enum CommandType
    {
        ElementsExists = 10,
        ElementsExistsInCount = 11,
        ElementHasClass = 16,
        ElementProperty = 18,

        Click = 20,

        FillInput = 30,
        ValidateInput = 35,

        ChangeDropDownValue = 40,
        ChangeDropDownToSomeValue = 41,

        SwitchToNextPage = 400,
        SendReportToServer = 401,

        ClearShoppingCart = 410,
        DeleteLastProfileAddress = 420,

        DOMNodeInserted = 450,
        DOMNodeInsertedEnd = 451,

        AjaxComplete = 500,
        AjaxCompleteEnd = 501
    }
}
