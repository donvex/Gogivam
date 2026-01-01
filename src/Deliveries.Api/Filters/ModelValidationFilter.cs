using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Deliveries.Api.Filters;

/// <summary>
/// Filtre d'action pour valider automatiquement les modèles
/// </summary>
public class ModelValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Title = "Erreur de validation",
                Status = 400,
                Detail = "Une ou plusieurs erreurs de validation se sont produites"
            };

            context.Result = new BadRequestObjectResult(problemDetails);
        }

        base.OnActionExecuting(context);
    }
}