﻿using Common.Messaging;
using FluentValidation.Results;

namespace Common.Verifiers;

public interface IEventVerifier<TMessage, TUnverified>
    where TMessage : Message
{
    ValidationResult Validate(MessageVerificationParameters<TMessage, EventMetadata, TUnverified> parameters);
}