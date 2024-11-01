﻿using CommonWithEventFactories.DataFactory;
using CommonWithEventFactories.Messaging;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.SubtractCommandHandler;

public class
    SubtractCommandDataFactory : IDataFactory<SubtractCommand, CommandMetadata, SubtractCommandUnverifiedData, SubtractCommandVerifiedData>
{
    public async Task<SubtractCommandUnverifiedData> GetDataAsync(MessageContainer<SubtractCommand, CommandMetadata> container)
    {
        await Task.Delay(250);
        return new SubtractCommandUnverifiedData(Random.Shared.Next(100));
    }

    public SubtractCommandVerifiedData GetVerifiedData(SubtractCommandUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new SubtractCommandVerifiedData(unverifiedData.Value1.Value);
    }
}