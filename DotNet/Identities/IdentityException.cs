﻿namespace AndrejKrizan.DotNet.Identities;

public class IdentityException : Exception
{
    // Constructors

    public IdentityException() { }

    public IdentityException(string message)
        : base(message) { }

    public IdentityException(string message, Exception innerException)
        : base(message, innerException) { }
}
