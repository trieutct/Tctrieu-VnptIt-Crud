using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Tctrieu.VnptIt.Crud.Api.Common.ModelBinding
{
    // Cho phép bind query string dạng snake_case (?page_size=10) vào property PascalCase (PageSize)
    public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var query = context.ActionContext.HttpContext.Request.Query;
            if (query.Count == 0) return Task.CompletedTask;

            // Bỏ dấu "_" khỏi key, binder so khớp không phân biệt hoa thường nên "pagesize" khớp "PageSize"
            var normalized_query = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            foreach (var (key, value) in query)
            {
                normalized_query[key.Replace("_", string.Empty)] = value;
            }

            context.ValueProviders.Add(new QueryStringValueProvider(
                BindingSource.Query,
                new QueryCollection(normalized_query),
                CultureInfo.InvariantCulture));

            return Task.CompletedTask;
        }
    }
}
