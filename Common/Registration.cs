using Common.DataFactory;
using Common.DefaultHandlers;
using Common.Helpers;
using Common.Messaging;
using Common.Operations;
using Common.Verifiers;
using Dumpify;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class Registration
{
    private static readonly Type GenericAuthorizedCommandHandlerType =
        typeof(AuthorizedCommandHandler<,,,,,>);

    private static readonly Type GenericCommandHandlerType =
        typeof(CommandHandler<,,,,>);

    private static readonly Type GenericPublishingEventHandlerType =
        typeof(PublishingEventHandler<,,>);

    private static readonly Type GenericEventHandlerType =
        typeof(ConventionalEventHandler<,,>);

    private static readonly Type GenericMessageOrchestratorType = typeof(MessageContainerOrchestrator<,>);

    private static readonly Type MessageContainerHandlerType = typeof(IMessageContainerHandler<,>);
    private static readonly Type DataFactoryType = typeof(IDataFactory<,,,>);

    private static readonly Type AuthorizedCommandVerifierType = typeof(IAuthorizedCommandVerifier<,,,>);
    private static readonly Type MessageVerifierType = typeof(IMessageVerifier<,,,>);
    private static readonly Type EventVerifierType = typeof(IEventVerifier<,>);

    private static readonly IEnumerable<Type> VerifierTypes =
    [
        AuthorizedCommandVerifierType, MessageVerifierType, EventVerifierType,
    ];

    private static readonly Type OperationType = typeof(IOperation<,,,>);
    private static readonly Type PublishingOperationType = typeof(IPublishingOperation<,,,>);
    private static readonly Type EventOperationType = typeof(IEventOperation<,,>);
    private static readonly Type EventPublishingOperationType = typeof(IEventPublishingOperation<,,>);

    private static readonly IEnumerable<Type> OperationTypes =
    [
        OperationType, PublishingOperationType, EventOperationType,
        EventPublishingOperationType
    ];

    public static IServiceCollection AddEventHandlersAndNecessaryWork(this IServiceCollection services,
        params Type[] sourceTypes)
    {
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) => member?.Name.ToSnakeCase();
        
        services.AddSingleton(typeof(MessageContainerMapper<,>));

        var registrations = sourceTypes.Select(sourceType => sourceType.Assembly).Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsAllowedType)
            .SelectMany(usageType => usageType.GetInterfaces().Where(IsAllowedInterfaceType).Select(x => new
            {
                MessageType = x.GetGenericArguments().First(),
                UsageType = usageType,
                UsageInterfaceType = x
            }))
            .GroupBy(x => x.MessageType)
            .ToDictionary(group => group.Key, group => group.ToList());

        registrations.Dump();

        foreach (var message in registrations.Keys)
        {
            var handlerDependencies = registrations[message];

            var genericParameters =
                new HandlerParameters(handlerDependencies.Select(x => x.UsageInterfaceType).ToList());

            foreach (var handlerDependency in handlerDependencies)
            {
                services.AddScoped(handlerDependency.UsageInterfaceType, handlerDependency.UsageType);

                genericParameters.SetType(handlerDependency.UsageInterfaceType);
            }

            services.AddScoped(genericParameters.GetGenericInterfaceType(),
                genericParameters.GetGenericHandlerType());


            services.AddKeyedScoped(typeof(IMessageOrchestrator), genericParameters.GetMessageName(),
                genericParameters.GetGenericOrchestratorType());
        }

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }

    private record HandlerParameters(List<Type> Interfaces)
    {
        public string GetMessageName() => MessageType.Name;

        public Type GetGenericOrchestratorType()
            => GenericMessageOrchestratorType.MakeGenericType(MessageType,
                MessageMetadataType);

        public Type GetGenericInterfaceType()
            => MessageContainerHandlerType.MakeGenericType(MessageType,
                MessageMetadataType);

        public Type GetGenericHandlerType()
        {
            var genericInterfaces = Interfaces.Select(i => i.GetGenericTypeDefinition()).ToList();
            if (IsGenericAuthorizedCommandHandlerType(genericInterfaces))
            {
                return GenericAuthorizedCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType,
                    VerifiedDataType,
                    AuthorizationFailedEventType, ValidationFailedEventType, FailedEventType);
            }

            if (IsGenericCommandHandlerType(genericInterfaces))
            {
                return GenericCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType,
                    ValidationFailedEventType, FailedEventType);
            }

            if (IsGenericPublishingEventHandler(genericInterfaces))
            {
                return GenericPublishingEventHandlerType.MakeGenericType(MessageType, UnverifiedDataType,
                    VerifiedDataType);
            }

            if (IsGenericEventHandlerType(genericInterfaces))
            {
                return GenericEventHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
            }

            throw new Exception();
        }


        private Type MessageType { get; set; }

        private Type MessageMetadataType { get; set; }

        private Type UnverifiedDataType { get; set; }

        private Type VerifiedDataType { get; set; }

        private Type AuthorizationFailedEventType { get; set; }

        private Type ValidationFailedEventType { get; set; }

        private Type FailedEventType { get; set; }

        public void SetType(Type type)
        {
            var genericArguments = type.GetGenericArguments();

            if (IsDataFactoryInterface(type))
            {
                UnverifiedDataType = genericArguments[^2];
                VerifiedDataType = genericArguments.Last();
            }

            if (IsAuthorizedCommandVerifierInterface(type))
            {
                AuthorizationFailedEventType = genericArguments[^2];
                ValidationFailedEventType = genericArguments.Last();
            }

            if (IsMessageVerifierInterface(type))
            {
                ValidationFailedEventType = genericArguments.Last();
            }

            if (IsAnyOperationInterface(type))
            {
                MessageType = genericArguments.First();
                MessageMetadataType = genericArguments.Skip(1).First();
                if (IsPublishingOperationInterface(type) || IsOperationInterface(type))
                    FailedEventType = genericArguments[^1];
            }
        }
    }

    private static bool IsGenericAuthorizedCommandHandlerType(List<Type> interfaces) =>
        interfaces.Contains(AuthorizedCommandVerifierType) &&
        interfaces.Contains(PublishingOperationType);

    private static bool IsGenericCommandHandlerType(List<Type> interfaces) =>
        interfaces.Contains(MessageVerifierType) && interfaces.Contains(PublishingOperationType);

    private static bool IsGenericPublishingEventHandler(List<Type> interfaces) =>
        interfaces.Contains(EventVerifierType) && interfaces.Contains(EventPublishingOperationType);

    private static bool IsGenericEventHandlerType(List<Type> interfaces) =>
        interfaces.Contains(EventVerifierType) && interfaces.Contains(EventOperationType);

    private static bool IsAllowedType(Type type) =>
        !type.IsAbstract && (IsDataFactory(type) || IsVerifier(type) || IsAnyOperation(type));

    private static bool IsAllowedInterfaceType(Type type) =>
        IsDataFactoryInterface(type) || IsVerifierInterface(type) || IsAnyOperationInterface(type);

    private static bool IsDataFactory(Type type)
    {
        return type.GetInterfaces().Any(IsDataFactoryInterface);
    }

    private static bool IsDataFactoryInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == DataFactoryType;
    }

    private static bool IsVerifier(Type type)
    {
        return type.GetInterfaces().Any(IsVerifierInterface);
    }

    private static bool IsVerifierInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               VerifierTypes.Contains(interfaceType.GetGenericTypeDefinition());
    }

    private static bool IsAuthorizedCommandVerifierInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == AuthorizedCommandVerifierType;
    }

    private static bool IsMessageVerifierInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == MessageVerifierType;
    }

    private static bool IsAnyOperation(Type type)
    {
        return type.GetInterfaces().Any(IsAnyOperationInterface);
    }

    private static bool IsAnyOperationInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               OperationTypes.Contains(interfaceType.GetGenericTypeDefinition());
    }

    private static bool IsPublishingOperationInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == PublishingOperationType;
    }

    private static bool IsOperationInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == OperationType;
    }
}