using Common.Structured.Authorization;
using Common.Structured.DataFactory;
using Common.Structured.Helpers;
using Common.Structured.Messaging;
using Dumpify;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Structured;

public static class Registration
{
    private static readonly Type GenericMessageOrchestratorType = typeof(MessageContainerOrchestrator<,>);
    private static readonly Type GenericMessageContainerHandlerType = typeof(IMessageContainerHandler<,>);
    private static readonly Type GenericDataFactoryType = typeof(IDataFactory<,,>);
    private static readonly Type GenericAuthorizerType = typeof(IAuthorizer<>);
    private static readonly Type GenericValidatorType = typeof(IValidator<>);

    public static IServiceCollection AddStructuredEventHandlersAndNecessaryWork(this IServiceCollection services,
        params Type[] sourceTypes)
    {
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) => member?.Name.ToSnakeCase();

        services.AddSingleton(typeof(MessageContainerMapper<,>));

        var registrations = sourceTypes.Select(sourceType => sourceType.Assembly).Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(IsAllowedType)
            .SelectMany(usageType => usageType.GetInterfaces().Where(IsAllowedInterfaceType).Select(x => new
            {
                UsageType = usageType,
                UsageInterfaceType = x
            })).ToList();

        registrations.Dump();

        foreach (var registration in registrations)
        {
            if (IsMessageContainerHandlerInterface(registration.UsageInterfaceType))
            {
                var genericParameters = registration.UsageInterfaceType.GetGenericArguments();

                var closedOrchestratorType =
                    GenericMessageOrchestratorType.MakeGenericType(genericParameters.First(),
                        genericParameters.Skip(1).First());

                services.AddKeyedScoped(typeof(IMessageOrchestrator), genericParameters.First().Name,
                    closedOrchestratorType);
            }

            services.AddScoped(registration.UsageInterfaceType, registration.UsageType);
        }

        return services;
    }


    private static bool IsAllowedType(Type type)
        => IsMessageContainerHandler(type) ||
           IsDataFactory(type) ||
           IsAuthorizer(type) ||
           IsValidator(type);

    private static bool IsAllowedInterfaceType(Type type)
        => IsMessageContainerHandlerInterface(type) ||
           IsDataFactoryInterface(type) ||
           IsAuthorizerInterface(type) ||
           IsValidatorInterface(type);

    private static bool IsMessageContainerHandler(Type type)
    {
        return !type.IsAbstract && type.GetInterfaces().Any(IsMessageContainerHandlerInterface);
    }

    private static bool IsMessageContainerHandlerInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == GenericMessageContainerHandlerType;
    }


    private static bool IsDataFactory(Type type)
    {
        return !type.IsAbstract && type.GetInterfaces().Any(IsDataFactoryInterface);
    }

    private static bool IsDataFactoryInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == GenericDataFactoryType;
    }


    private static bool IsAuthorizer(Type type)
    {
        return !type.IsAbstract && type.GetInterfaces().Any(IsAuthorizerInterface);
    }

    private static bool IsAuthorizerInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == GenericAuthorizerType;
    }


    private static bool IsValidator(Type type)
    {
        return !type.IsAbstract && type.GetInterfaces().Any(IsValidatorInterface);
    }

    private static bool IsValidatorInterface(Type interfaceType)
    {
        return interfaceType.IsGenericType &&
               interfaceType.GetGenericTypeDefinition() == GenericValidatorType;
    }
}