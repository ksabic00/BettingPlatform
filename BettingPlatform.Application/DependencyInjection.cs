using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BettingPlatform.Application.Common.Behaviors;
using BettingPlatform.Application.Tickets.Commands.PlaceTicket; 

namespace BettingPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<PlaceTicketHandler>(); 

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<PlaceTicketCommand>();

        return services;
    }
}
