# Decimal numbers validation in ASP.NET MVC 5

It is known that by default ASP.NET applications accept only dot as the separator for decimal numbers as it is common for the English localization Culture. If you want to use a comma as a separator, the official documentation says it is necessary to include an extra jQuery script for globalization [ASP.NET tutorial](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/examining-the-edit-methods-and-edit-view#jquery-validation-for-non-english-locales)

**Let's say you want your app to accept BOTH comma and dot as a decimal separator in input fields.**

Here is a simple way to do this:

1. overwrite jQuery validation number method
2. add custom ModelBinder class

### Why are both steps necessary? 
Step 1. enables jQuery to validate the TextBox for decimal numbers with dot or decimal separators. With step one the values with dot or comma separator are allowed, but the controller gets null as the value from TextBox. That is why in step 3 custom model binder is necessary. Now both numbers 1.98 and 1,98 are accepted, as well as its negatives (-1.98 and -1,98).

### Steps:

#### 1. overwrite jQuery validation number method (based on [ReBuildAll Blog](http://blog.rebuildall.net/2011/03/02/jQuery_validate_and_the_comma_decimal_separator) )

```javascript
$.validator.methods.number = function (value, element) {
return this.optional(element) || /^$?-?\d+((.(\d+))|(,(\d+)))?$/.test(value);
} //add the method in the script tag in _Layout.cshtml before line (if you have it) //$(document).ready(){}
```
the regex `/^$?-?\d+((.(\d+))|(,(\d+)))?$/` accepts positive or negative decimal numbers with dot or comma as separator

#### 2. custom ModelBinder class (add it to your project), this is for type Decimal (thanks to [stackoverflow topic](https://stackoverflow.com/questions/25849160/decimal-numbers-in-asp-net-mvc-5-app#25862916) and [haacked.com](https://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/) for the code)

```C#
public class ModelBinder
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                                         ModelBindingContext bindingContext)
        {
            object result = null;

            // Don't do this here!
            // It might do bindingContext.ModelState.AddModelError
            // and there is no RemoveModelError!
            // 
            // result = base.BindModel(controllerContext, bindingContext);

            string modelName = bindingContext.ModelName;
            string attemptedValue = bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

            // Depending on CultureInfo, the NumberDecimalSeparator can be "," or "."
            // Both "." and "," should be accepted, but aren't.
            string wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            string alternateSeperator = (wantedSeperator == "," ? "." : ",");

            if (attemptedValue.IndexOf(wantedSeperator) == -1
                && attemptedValue.IndexOf(alternateSeperator) != -1)
            {
                attemptedValue =  attemptedValue.Replace(alternateSeperator, wantedSeperator);
            }

            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType
                    && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }

                result = decimal.Parse(attemptedValue, NumberStyles.Any);
            }
            catch (FormatException e)
            {
                bindingContext.ModelState.AddModelError(modelName, e);
            }

            return result;
        }
    }
}
```

**Add this to Application_Start() method in Global.asax**
```c#
ModelBinders.Binders.Add(typeof(decimal), new ModelBinder.DecimalModelBinder());
ModelBinders.Binders.Add(typeof(decimal?), new ModelBinder.DecimalModelBinder());
```

If you also want the model to accept decimal numbers with maximum 2 decimals, you can add
the [RegularExpression] data annotation to your model variable:
```c#
[RegularExpression(@"^$?-?\d+((.(\d{1,2}))|(,(\d{1,2})))?$", ErrorMessage = "Max two numbers after decimal separator accepted.")]
public decimal Price { get; set; }
```
 
Hope this helps someone looking to use both comma and dot as separators. In this case adding the jquery globalisation script might not work since usually only one or the other is allowed, based on your country specific localization culture, but not both.
For the same reason simply changing the culture and cultureInfo in Web.config won't work.

