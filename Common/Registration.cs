using System.Reflection;
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

        var handlers = sourceTypes.Select(sourceType => sourceType.Assembly).Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsAllowedType)
            .SelectMany(usageType => usageType.GetInterfaces().Where(IsAllowedInterfaceType).Select(x => new
            {
                MessageType = x.GetGenericArguments().First(),
                MessageMetadataType = x.GetGenericArguments().Skip(1).First(),
                UsageType = usageType,
                UsageInterfaceType = x,
            }))
            .GroupBy(x => (x.MessageType, x.MessageMetadataType))
            .Select(x => new HandlerDetails(x.Key.MessageType, x.Key.MessageMetadataType,
                x.Select(y => (y.UsageInterfaceType, y.UsageType)).ToList())).ToList();

        handlers.Dump();

        foreach (var handler in handlers)
        {
            foreach (var service in handler.Services)
            {
                services.AddScoped(service.Item1, service.Item2);
            }

            services.AddScoped(handler.ClosedInterfaceType, handler.ClosedType);


            services.AddKeyedScoped(typeof(IMessageOrchestrator), handler.MessageType.Name,
                handler.ClosedMessageOrchestratorType);
        }

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }

    private record HandlerDetails
    {
        public HandlerDetails(Type messageType, Type messageMetadataType, List<(Type, Type)> services)
        {
            Services = services;
            MessageType = messageType;
            MessageMetadataType = messageMetadataType;


            var genericParameters = services.SelectMany(x => x.Item1.GetGenericArguments()).Distinct().ToList();
            UnverifiedDataType = genericParameters.FirstOrDefault(p => p.Name.EndsWith("UnverifiedData")) ??
                                 throw new Exception("Unverified Data Type does not exist.");
            VerifiedDataType = genericParameters.FirstOrDefault(p => p.Name.EndsWith("VerifiedData")) ??
                               throw new Exception("Verified Data Type does not exist.");

            ClosedMessageOrchestratorType =
                GenericMessageOrchestratorType.MakeGenericType(MessageType, MessageMetadataType);
            ClosedInterfaceType =
                MessageContainerHandlerType.MakeGenericType(MessageType, MessageMetadataType);
            ClosedType = GetGenericHandlerType(services.Select(x => x.Item1.GetGenericTypeDefinition()).ToList());
        }

        private Type GetGenericHandlerType(List<Type> interfaces)
        {
            var genericInterfaces = interfaces.Select(u => u.GetGenericTypeDefinition()).ToList();
            if (genericInterfaces.Contains(AuthorizedCommandVerifierType))
            {
                return GenericAuthorizedCommandHandlerType.MakeGenericType(MessageType,
                    UnverifiedDataType,
                    VerifiedDataType);
            }

            if (MessageMetadataType == typeof(CommandMetadata))
            {
                return
                    GenericCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
            }
            else
            {
                return
                    GenericEventHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
            }
        }

        public Type ClosedMessageOrchestratorType { get; set; }
        public Type ClosedInterfaceType { get; set; }
        public Type ClosedType { get; set; }
        public Type UnverifiedDataType { get; set; }
        public Type VerifiedDataType { get; set; }
        public Type MessageType { get; }
        public Type MessageMetadataType { get; }
        public List<(Type, Type)> Services { get; }
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