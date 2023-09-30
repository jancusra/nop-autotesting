namespace KSystem.Nop.Plugin.Misc.AutoTesting.Enums
{
    /// <summary>
    /// Represents a command type enumeration
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Check if specific elements exists
        /// </summary>
        ElementsExists = 10,

        /// <summary>
        /// Check if specific elements exists in defined count
        /// </summary>
        ElementsExistsInCount = 11,

        /// <summary>
        /// Check if specific element has class
        /// </summary>
        ElementHasClass = 16,

        /// <summary>
        /// Check if element has specific property
        /// </summary>
        ElementProperty = 18,

        /// <summary>
        /// Click action to defined target
        /// </summary>
        Click = 20,

        /// <summary>
        /// Fill input with specific value
        /// </summary>
        FillInput = 30,

        /// <summary>
        /// Validate input for specific value
        /// </summary>
        ValidateInput = 35,

        /// <summary>
        /// Change dropdown to specific value
        /// </summary>
        ChangeDropDownValue = 40,

        /// <summary>
        /// Change dropdown to some value
        /// </summary>
        ChangeDropDownToSomeValue = 41,

        /// <summary>
        /// Switch to next page in testing scenario
        /// </summary>
        SwitchToNextPage = 400,

        /// <summary>
        /// Send report message to server
        /// </summary>
        SendReportToServer = 401,

        /// <summary>
        /// Remove all items in shopping cart for current user
        /// </summary>
        ClearShoppingCart = 410,

        /// <summary>
        /// Delete the last profile address created by testing task
        /// </summary>
        DeleteLastProfileAddress = 420,

        /// <summary>
        /// DOM node inserted event
        /// </summary>
        DOMNodeInserted = 450,

        /// <summary>
        /// DOM node inserted - end of the event
        /// </summary>
        DOMNodeInsertedEnd = 451,

        /// <summary>
        /// Ajax complete event
        /// </summary>
        AjaxComplete = 500,

        /// <summary>
        /// Ajax complete - end of the event
        /// </summary>
        AjaxCompleteEnd = 501
    }
}
