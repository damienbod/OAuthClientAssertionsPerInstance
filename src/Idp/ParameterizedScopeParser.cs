﻿using Duende.IdentityServer.Validation;

namespace Idp;

public class ParameterizedScopeParser : DefaultScopeParser
{
    public ParameterizedScopeParser(ILogger<DefaultScopeParser> logger) : base(logger)
    { }

    public override void ParseScopeValue(ParseScopeContext scopeContext)
    {
        const string transactionScopeName = "auth_session";
        const string separator = ":";
        const string transactionScopePrefix = transactionScopeName + separator;

        var scopeValue = scopeContext.RawValue;

        if (scopeValue.StartsWith(transactionScopePrefix))
        {
            // we get in here with a scope like "transaction:something"
            var parts = scopeValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                scopeContext.SetParsedValues(transactionScopeName, parts[1]);
            }
            else
            {
                scopeContext.SetError("auth_session scope missing transaction parameter value");
            }
        }
        else if (scopeValue != transactionScopeName)
        {
            // we get in here with a scope not like "transaction"
            base.ParseScopeValue(scopeContext);
        }
        else
        {
            // we get in here with a scope exactly "transaction", which is to say we're ignoring it 
            // and not including it in the results
            scopeContext.SetIgnore();
        }
    }
}