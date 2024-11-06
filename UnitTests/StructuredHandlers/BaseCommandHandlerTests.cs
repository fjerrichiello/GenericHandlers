using Common.Structured.Authorization;
using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.StructuredHandlers;

public abstract class BaseCommandHandlerTests<TMessage, TMetadata, TData, TCommandHandler>
    where TMessage : Message
    where TMetadata : MessageMetadata
{
    protected readonly IDataFactory<TMessage, TMetadata, TData> _dataFactory =
        Mock.Of<IDataFactory<TMessage, TMetadata, TData>>();

    protected readonly IAuthorizer<TData> _authorizer = Mock.Of<IAuthorizer<TData>>();

    protected readonly IValidator<TData> _validator = Mock.Of<IValidator<TData>>();

    protected readonly ILogger<TCommandHandler> _logger = Mock.Of<ILogger<TCommandHandler>>();
}