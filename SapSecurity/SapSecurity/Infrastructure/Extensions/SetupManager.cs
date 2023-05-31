using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Services;
using SapSecurity.Services.Connection;
using SapSecurity.Services.Db;
using SapSecurity.Services.Mapper;
using SapSecurity.Services.Notification;
using SapSecurity.Services.Security;
using SapSecurity.Services.Sms;
using SapSecurity.Services.SocketManager;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Extensions;

public static class SetupManager
{
    /// <summary>
    /// setup services instance
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    public static void SetupServices(this IServiceCollection services, string connectionString)
    {

        //db sql
        SetupSql(ref services, connectionString);

        //repositories
        services.AddTransient<ISensorDetailRepository, SensorDetailRepository>();
        services.AddTransient<ISensorGroupRepository, SensorGroupRepository>();
        services.AddTransient<ISensorLogRepository, SensorLogRepository>();
        services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddTransient<IZoneRepository, ZoneRepository>();
        services.AddTransient<ICameraImageRepository, CameraImageRepository>();


        //db services
        services.AddTransient<ISensorDetailService, SensorDetailService>();
        services.AddTransient<ISensorGroupService, SensorGroupService>();
        services.AddTransient<ISensorLogService, SensorLogService>();
        services.AddTransient<IApplicationUserService, ApplicationUserService>();
        services.AddTransient<IZoneService, ZoneService>();
        services.AddTransient<ICameraImageService, CameraImageService>();

        //http
        //services.AddTransient<IHttpServer, HttpServer>();

        //mapper
        services.AddTransient<IMapper, Mapper>();

        //notification manager
        services.AddTransient<HomeSocketNotificationManager>();
        services.AddTransient<HttpPushNotificationManager>();
        services.AddTransient<NotificationManager>();
        services.AddTransient<UserSmsNotificationManager>();
        services.AddTransient<UserSocketNotificationManager>();
        services.AddTransient<INotificationManager, NotificationManager>();

        //security
        services.AddTransient<ISecurityManager, SecurityManager>();

        //connection
        services.AddTransient<IHomeUdpSocketManager, HomeUdpSocketManager>();
        services.AddTransient<IUserSmsManager, UserSmsManager>();
        services.AddTransient<IUserSocketManager, UserSocketManager>();
        services.AddTransient<IUserWebSocketManager, UserWebSocketManager>();
        services.AddTransient<IMapManager, MapManager>();


        //connection hub
        services.AddTransient<IConnectionHub, ConnectionHub>();

        


        //socket
        services.AddTransient<ISocketManager, SocketManager>();
        services.AddTransient<ISocketUdpManager, SocketUdpManager>();
        //sms
        services.AddTransient<ISmsManager, SmsManager>();

        //logging
        services.AddLogging(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);


        });

        services.AddTransient<ApplicationContext>();

    }
    /// <summary>
    /// setup sql connection
    /// </summary>
    /// <returns></returns>
    private static void SetupSql(ref IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseSqlServer(connectionString);

        }, ServiceLifetime.Transient);
    }
}