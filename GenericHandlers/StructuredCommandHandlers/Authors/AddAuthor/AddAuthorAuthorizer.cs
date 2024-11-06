﻿using Common.Structured.Authorization;
using FluentValidation;

namespace GenericHandlers.StructuredCommandHandlers.Authors.AddAuthor;

public class AddAuthorAuthorizer
    : Authorizer<AddAuthorData>
{
    public AddAuthorAuthorizer()
    {
        RuleFor(data => data.FirstName)
            .NotEmpty();
    }
}