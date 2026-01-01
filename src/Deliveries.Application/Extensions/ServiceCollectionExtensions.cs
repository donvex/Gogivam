using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using Deliveries.Application.Interfaces;
using Deliveries.Application.Services;
using Deliveries.Application.Validators;
using Deliveries.Application.Commands.Deliveries;
using Deliveries.Application.Handlers.Commands;
using Deliveries.Application.Handlers.Queries;
using Deliveries.Application.Behaviors;
using Deliveries.Domain.Entities;
using Deliveries.Domain.Factories;

namespace Deliveries.Application.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services de l'application
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre tous les services de la couche application
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Enregistrer MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        });

        // Enregistrer les behaviors MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Enregistrer les validateurs FluentValidation
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Enregistrer les services applicatifs
        services.AddScoped<IDeliveryApplicationService, DeliveryApplicationService>();

        // Enregistrer les services du domaine
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<DeliveryFactory>();

        // Enregistrer les handlers explicitement si nécessaire
        services.AddScoped<IRequestHandler<CreateDeliveryCommand, Domain.DTOs.DeliveryResponseDto>, CreateDeliveryCommandHandler>();

        return services;
    }

    /// <summary>
    /// Configure les validateurs
    /// </summary>
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateDeliveryCommand>, CreateDeliveryCommandValidator>();
        
        return services;
    }
}