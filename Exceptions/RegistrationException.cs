namespace LibraryManagement.Exceptions;

public class RegistrationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public RegistrationException(IEnumerable<string> errors)
        : base("Registration failed.")
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors.ToList().AsReadOnly();
    }
}