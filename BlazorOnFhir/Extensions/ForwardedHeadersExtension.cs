using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace BlazorOnFhir.Extensions
{
    /// <summary>
    /// Class containing extention method to add Forwarded Headers middleware base on environment variable of the app.
    /// </summary>
    public static class UseForwardedHeadersExtension
    {
        /// <summary><para>
        /// Add forwarded headers services necessary when using the app behind a reverse proxy.
        /// To enable this middleware, you need to set these environment variables :</para><para>
        /// Forwarded_headers: "true"</para><para>
        /// To add know network (not neccessary) :</para><para>
        /// KNOW_NETWORKS: 127.0.0.1/32;192.168.0.0/24</para><para>
        /// To use the schema send be the reverse proxy. Usefull when the app is
        /// in an isolted environment and the reverse proxy secure the connection.
        /// this will override the request schema according to this pseudocode: 
        /// context.Request.Scheme = headers["X-Forwarded-Proto"]</para><para>
        /// USE_SCHEMA_FROM_PROXY: "true"</para>
        /// </summary>
        /// <remarks>
        /// You also need to <see cref="UseForwardedHeadersRules"/> in your Startup.Configure method.
        /// </remarks>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
        {
            if (string.Equals(configuration.GetValue<string>("Forwarded_headers"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();

                    foreach (var network in configuration.GetValue<string>("KNOW_NETWORKS")?.Split(';') ?? Array.Empty<string>())
                    {
                        var ipInfo = network.Split("/");

                        options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(ipInfo[0]), int.Parse(ipInfo[1])));
                    }
                });
            }

            return services;
        }

        /// <summary>
        /// Add forwarded headers middleware necessary when using the app behind a reverse proxy.
        /// <para>
        /// Use this middleware at the begining of the pipeline, juste after the your exception handler
        /// and before the Htst and HttpsRedirect middleware.
        /// </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseForwardedHeadersRules(this IApplicationBuilder app, ILogger<Startup> logger, IConfiguration configuration)
        {
            if (string.Equals(configuration.GetValue<string>("Forwarded_headers"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                app.UseForwardedHeaders();

                DebugHeaders(app, logger, configuration);

                if (string.Equals(configuration.GetValue<string>("USE_SCHEMA_FROM_PROXY"), bool.TrueString, StringComparison.CurrentCultureIgnoreCase))
                {
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var forwardedProto))
                        {
                            context.Request.Scheme = forwardedProto;
                        }

                        await next();
                    });
                }
            }

            return app;
        }

        private static void DebugHeaders(IApplicationBuilder app, ILogger<Startup> logger, IConfiguration configuration)
        {
            if (string.Equals(configuration.GetValue<string>("Debug_headers"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                app.Use(async (context, next) =>
                {
                    // Request method, scheme, and path
                    logger.LogDebug("Request Method: {Method}", context.Request.Method);
                    logger.LogDebug("Request Scheme: {Scheme}", context.Request.Scheme);
                    logger.LogDebug("Request Path: {Path}", context.Request.Path);

                    // Headers
                    foreach (var header in context.Request.Headers)
                    {
                        logger.LogDebug("Header: {Key}: {Value}", header.Key, header.Value);
                    }

                    // Connection: RemoteIp
                    logger.LogDebug("Request RemoteIp: {RemoteIpAddress}",
                        context.Connection.RemoteIpAddress);

                    await next();
                });
            }
        }
    }
}
