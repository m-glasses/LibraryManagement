using System;

namespace LibraryManagement.Extensions;

public class RegistrationException  : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public RegistrationException(IEnumerable<string> errors)
        : base("Registration failed.")
    {
        Errors = errors.ToList();
    }
}
