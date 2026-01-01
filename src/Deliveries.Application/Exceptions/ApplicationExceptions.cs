namespace Deliveries.Application.Exceptions;

/// <summary>
/// Exception de base pour la couche application
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message) { }
    protected ApplicationException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception levée lors d'une erreur de validation
/// </summary>
public class ValidationApplicationException : ApplicationException
{
    public IList<string> ValidationErrors { get; }

    public ValidationApplicationException(IList<string> validationErrors) 
        : base($"Validation failed: {string.Join(", ", validationErrors)}")
    {
        ValidationErrors = validationErrors;
    }

    public ValidationApplicationException(string validationError) 
        : this(new List<string> { validationError })
    {
    }
}

/// <summary>
/// Exception levée quand une ressource n'est pas trouvée
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key) 
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

/// <summary>
/// Exception levée lors d'un conflit métier
/// </summary>
public class BusinessConflictException : ApplicationException
{
    public BusinessConflictException(string message) : base(message) { }
    public BusinessConflictException(string message, Exception innerException) : base(message, innerException) { }
}