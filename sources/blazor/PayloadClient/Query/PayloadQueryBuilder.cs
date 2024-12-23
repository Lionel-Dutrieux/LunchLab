namespace PayloadClient.Query;

public class PayloadQueryBuilder
{
    private readonly Dictionary<string, string> _queryParams = new();

    public PayloadQueryBuilder WithDepth(int depth)
    {
        _queryParams["depth"] = depth.ToString();
        return this;
    }

    public PayloadQueryBuilder WithLocale(string locale)
    {
        _queryParams["locale"] = locale;
        return this;
    }

    public PayloadQueryBuilder WithPage(int page)
    {
        _queryParams["page"] = page.ToString();
        return this;
    }

    public PayloadQueryBuilder WithLimit(int limit)
    {
        _queryParams["limit"] = limit.ToString();
        return this;
    }

    public PayloadQueryBuilder Where(string field, string operation, object value)
    {
        _queryParams[$"where[{field}][{operation}]"] = Uri.EscapeDataString(value.ToString() ?? string.Empty);
        return this;
    }

    public PayloadQueryBuilder Sort(string field, bool ascending = true)
    {
        _queryParams["sort"] = $"{(ascending ? "" : "-")}{field}";
        return this;
    }

    public PayloadQueryBuilder Select(params string[] fields)
    {
        if (fields.Length > 0)
        {
            _queryParams["select"] = string.Join(",", fields);
        }
        return this;
    }

    public PayloadQueryBuilder Populate(params string[] fields)
    {
        if (fields.Length > 0)
        {
            _queryParams["populate"] = string.Join(",", fields);
        }
        return this;
    }

    public PayloadQueryBuilder Or(params Action<PayloadQueryBuilder>[] conditions)
    {
        var orConditions = conditions.Select(condition =>
        {
            var builder = new PayloadQueryBuilder();
            condition(builder);
            return builder.Build();
        });

        _queryParams.Add("or", string.Join(",", orConditions));
        return this;
    }

    public string Build()
    {
        if (!_queryParams.Any())
            return string.Empty;

        var queryString = string.Join("&", _queryParams.Select(p => $"{p.Key}={p.Value}"));
        return $"?{queryString}";
    }
} 