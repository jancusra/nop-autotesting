namespace KSystem.Nop.Plugin.Misc.AutoTesting.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class EnumExtension
    {
        public static IList<SelectListItem> ToSelectList<TEnum>(this TEnum enumObj) where TEnum : struct
        {
            IList<SelectListItem> listItems = new List<SelectListItem>();

            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("An Enumeration type is required.", nameof(enumObj));
            }

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         select enumValue;

            foreach (var enumValue in values)
            {
                listItems.Add(new SelectListItem
                {
                    Value = Convert.ToInt32(enumValue).ToString(),
                    Text = enumObj.ToString()
                });
            }

            return listItems;
        }
    }
}
