using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc.Resources;

namespace System.Web.Mvc.Html
{
    public static class SelectExtensionFix
    {
        // DropDownList

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name) {
            return FixedDropDownList(htmlHelper, name, null /* selectList */, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, string optionLabel) {
            return FixedDropDownList(htmlHelper, name, null /* selectList */, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList) {
            return FixedDropDownList(htmlHelper, name, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes) {
            return FixedDropDownList(htmlHelper, name, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes) {
            return FixedDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel) {
            return FixedDropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes) {
            return FixedDropDownList(htmlHelper, name, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FixedDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes) {
            return FixedDropDownListHelper(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList) {
            return FixedDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes) {
            return FixedDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes) {
            return FixedDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel) {
            return FixedDropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes) {
            return FixedDropDownListFor(htmlHelper, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString FixedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }

            return FixedDropDownListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
        }

        private static MvcHtmlString FixedDropDownListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes) {
            return SelectInternal(htmlHelper, optionLabel, expression, selectList, false /* allowMultiple */, htmlAttributes);
        }

        // Helper methods

        private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name) {
            object o = null;
            if (htmlHelper.ViewData != null) {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "There is no ViewData item of type '{1}' that has the key '{0}'.",
                        name,
                        "IEnumerable<SelectListItem>"));
            }
            IEnumerable<SelectListItem> selectList = o as IEnumerable<SelectListItem>;
            if (selectList == null) {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        @"The ViewData item that has the key ""{0}"" is of type ""{1}"" but must be of type ""{2}"".",
                        name,
                        o.GetType().FullName,
                        "IEnumerable<SelectListItem>"));
            }
            return selectList;
        }

        internal static string ListItemToOption(SelectListItem item) {
            TagBuilder builder = new TagBuilder("option") {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null) {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected) {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        public static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType) {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState)) {
                if (modelState.Value != null) {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<SelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes) {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName)) {
                throw new ArgumentException("Value cannot be null or empty.", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null) {
                selectList = htmlHelper.GetSelectData(fullName);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(fullName, typeof(string[])) : htmlHelper.GetModelStateValue(fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData) {
                if (defaultValue == null) {
                    defaultValue = htmlHelper.ViewData.Eval(fullName);
                }
                if (defaultValue == null) {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
            }

            if (defaultValue != null) {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<SelectListItem> newSelectList = new List<SelectListItem>();

                foreach (SelectListItem item in selectList) {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null) {
                listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (SelectListItem item in selectList) {
                listItemBuilder.AppendLine(ListItemToOption(item));
            }

            TagBuilder tagBuilder = new TagBuilder("select") {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple) {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState)) {
                if (modelState.Errors.Count > 0) {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}
