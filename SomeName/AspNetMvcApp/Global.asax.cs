using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AspNetMvcApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TracerProvider _tracerProvider;
        private MeterProvider _meterProvider;

        protected void Application_Start()
        {
            var serverName = "legacymvcapp";
            var version = "1.19.0.0";
            InitializeOpenTelemetry(serverName, version);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        
        protected void Application_End(object sender, EventArgs e)
        {
            _tracerProvider?.Dispose();
            _meterProvider?.Dispose();
        }

        private void InitializeOpenTelemetry(string serverName, string version)
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            var tracerBuilder = Sdk.CreateTracerProviderBuilder();

            if (useOtlpExporter)
                tracerBuilder.AddOtlpExporter();

            _tracerProvider = tracerBuilder
                .AddSqlClientInstrumentation()
                .AddAspNetInstrumentation()
                .AddSource(serverName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: $"web_{Environment.MachineName}", serviceVersion: version))

                .Build();

            var meterBuilder = Sdk.CreateMeterProviderBuilder();
            if (useOtlpExporter)
                meterBuilder.AddOtlpExporter();

            _meterProvider = meterBuilder
                .AddRuntimeInstrumentation()
                .Build();
        }

    }
}
