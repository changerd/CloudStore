using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IBAstore.Infastructure
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;

            try
            {
                actualValue = !string.IsNullOrEmpty(valueResult.AttemptedValue) ?
                    decimal.Parse(valueResult.AttemptedValue, new NumberFormatInfo { NumberDecimalSeparator = "." }) :
                    (decimal?)null;
            }
            catch (Exception e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}