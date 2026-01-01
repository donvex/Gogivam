using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Deliveries.Application.Behaviors;

/// <summary>
/// Comportement MediatR pour la validation automatique des commandes
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var errors = string.Join(", ", failures.Select(f => f.ErrorMessage));
                _logger.LogWarning("Validation failed for {RequestType}: {Errors}", 
                    typeof(TRequest).Name, errors);
                
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}

/// <summary>
/// Comportement pour le logging des performances
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
                
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error handling {RequestName} after {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}