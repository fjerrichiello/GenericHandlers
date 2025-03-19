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
        ValidatorOptions.Global.PropertyNameResolver = (type, member, expression) => member?.Name.ToSnakeCase();
        
        services.AddSingleton(typeof(MessageContainerMapper<,>));

        var handlers = GetHandlerDetails(sourceTypes);

        handlers.Dump();
        foreach (var handler in handlers)
        {
            RegisterHandlerServices(services, handler);
        }

        services.AddScoped<IEventPublisher, EventPublisher>();
        return services;
    }

    private static List<HandlerDetails> GetHandlerDetails(Type[] sourceTypes) =>
        sourceTypes.SelectMany(st => st.Assembly.GetTypes())
            .Where(IsAllowedType)
            .SelectMany(GetHandlerDetailsFromType)
            .GroupBy(h => (h.MessageType, h.MessageMetadataType))
            .Select(g => new HandlerDetails(g.Key.MessageType, g.Key.MessageMetadataType,
                g.Select(x => (x.UsageInterfaceType, x.UsageType)).ToList()))
            .ToList();

    private static IEnumerable<(Type MessageType, Type MessageMetadataType, Type UsageType, Type UsageInterfaceType)>
        GetHandlerDetailsFromType(Type usageType) =>
        usageType.GetInterfaces()
            .Where(IsAllowedInterfaceType)
            .Select(i => (
                MessageType: i.GetGenericArguments().First(),
                MessageMetadataType: i.GetGenericArguments().Skip(1).First(),
                UsageType: usageType,
                UsageInterfaceType: i
            ));

    private static void RegisterHandlerServices(IServiceCollection services, HandlerDetails handler)
    {
        foreach (var service in handler.Services)
        {
            services.AddScoped(service.Item1, service.Item2);
        }

        services.AddScoped(handler.ClosedInterfaceType, handler.ClosedType);
        services.AddKeyedScoped(typeof(IMessageOrchestrator), handler.MessageType.Name,
            handler.ClosedMessageOrchestratorType);
    }

    private record HandlerDetails
    {
        public Type MessageType { get; }
        public Type MessageMetadataType { get; }
        public Type ClosedMessageOrchestratorType { get; }
        public Type ClosedInterfaceType { get; }
        public Type ClosedType { get; }
        public Type UnverifiedDataType { get; }
        public Type VerifiedDataType { get; }
        public List<(Type, Type)> Services { get; }

        public HandlerDetails(Type messageType, Type messageMetadataType, List<(Type, Type)> services)
        {
            MessageType = messageType;
            MessageMetadataType = messageMetadataType;
            Services = services;

            var genericTypes = services.SelectMany(s => s.Item1.GetGenericArguments()).Distinct().ToList();
            UnverifiedDataType = GetGenericType(genericTypes, "UnverifiedData");
            VerifiedDataType = GetGenericType(genericTypes, "VerifiedData");

            ClosedMessageOrchestratorType =
                GenericMessageOrchestratorType.MakeGenericType(MessageType, MessageMetadataType);
            ClosedInterfaceType = MessageContainerHandlerType.MakeGenericType(MessageType, MessageMetadataType);
            ClosedType = DetermineClosedType(services.Select(s => s.Item1.GetGenericTypeDefinition()).ToList());
        }

        private static Type GetGenericType(IEnumerable<Type> types, string suffix) =>
            types.FirstOrDefault(t => t.Name.EndsWith(suffix)) ?? throw new Exception($"{suffix} Type does not exist.");

        private Type DetermineClosedType(List<Type> interfaces)
        {
            if (interfaces.Contains(AuthorizedCommandVerifierType))
                return GenericAuthorizedCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType,
                    VerifiedDataType);
            return MessageMetadataType == typeof(CommandMetadata)
                ? GenericCommandHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType)
                : GenericEventHandlerType.MakeGenericType(MessageType, UnverifiedDataType, VerifiedDataType);
        }
    }

    private static bool IsAllowedType(Type type) =>
        !type.IsAbstract && (IsDataFactory(type) || IsVerifier(type) || IsOperation(type));

    private static bool IsAllowedInterfaceType(Type type) =>
        type.IsGenericType && (IsDataFactoryInterface(type) || IsVerifierInterface(type) || IsOperationInterface(type));

    private static bool IsDataFactory(Type type) => type.GetInterfaces().Any(IsDataFactoryInterface);

    private static bool IsDataFactoryInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == DataFactoryType;

    private static bool IsVerifier(Type type) => type.GetInterfaces().Any(IsVerifierInterface);

    private static bool IsVerifierInterface(Type type) =>
        type.IsGenericType && VerifierTypes.Contains(type.GetGenericTypeDefinition());

    private static bool IsOperation(Type type) => type.GetInterfaces().Any(IsOperationInterface);

    private static bool IsOperationInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == OperationType;
}