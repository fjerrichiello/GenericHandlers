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
        typeof(AuthorizedCommandHandler<,,>);

    private static readonly Type GenericCommandHandlerType =
        typeof(CommandHandler<,,>);

    private static readonly Type GenericEventHandlerType =
        typeof(EventHandler<,,>);

    private static readonly Type GenericMessageOrchestratorType = typeof(MessageContainerOrchestrator<,>);

    private static readonly Type MessageContainerHandlerType = typeof(IMessageContainerHandler<,>);
    private static readonly Type DataFactoryType = typeof(IDataFactory<,,,>);

    private static readonly Type AuthorizedCommandVerifierType = typeof(IAuthorizedMessageVerifier<,,>);
    private static readonly Type MessageVerifierType = typeof(IMessageVerifier<,,>);

    private static readonly IEnumerable<Type> VerifierTypes =
    [
        AuthorizedCommandVerifierType, MessageVerifierType
    ];

    private static readonly Type OperationType = typeof(IOperation<,,>);

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
            if (genericInterfaces.Contains(AuthorizedCommandVerifierType))
            {
                return GenericAuthorizedCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType,
                    VerifiedDataType);
            }

            if (MessageMetadataType == typeof(CommandMetadata))
            {
                return GenericCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
            }


            return GenericEventHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
        }

        private Type MessageType { get; set; }

        private Type MessageMetadataType { get; set; }

        private Type UnverifiedDataType { get; set; }

        private Type VerifiedDataType { get; set; }

        public void SetType(Type type)
        {
            var genericArguments = type.GetGenericArguments();

            if (IsDataFactoryInterface(type))
            {
                UnverifiedDataType = genericArguments[^2];
                VerifiedDataType = genericArguments.Last();
            }

            if (IsOperationInterface(type))
            {
                MessageType = genericArguments.First();
                MessageMetadataType = genericArguments.Skip(1).First();
            }
        }
    }


    private static bool IsAllowedType(Type type) =>
        !type.IsAbstract && (IsDataFactory(type) || IsVerifier(type) || IsOperation(type));

    private static bool IsAllowedInterfaceType(Type type) =>
        IsDataFactoryInterface(type) || IsVerifierInterface(type) || IsOperationInterface(type);

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

    private static bool IsAuthorizedMessageVerifierInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == AuthorizedCommandVerifierType;
    }

    private static bool IsMessageVerifierInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == MessageVerifierType;
    }

    private static bool IsOperation(Type type)
    {
        return type.GetInterfaces().Any(IsOperationInterface);
    }

    private static bool IsOperationInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == OperationType;
    }
}