namespace Titan_Fitness.application_layer;

using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // تسجيل MediatR لجميع الـ Handlers في هذا المشروع
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // تسجيل جميع الـ Validators المكتوبة بـ FluentValidation تلقائياً
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}