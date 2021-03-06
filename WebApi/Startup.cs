using CqrsFramework;
using DataAccess.MsSql;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UseCases.Order;
using UseCases.Order.CheckOrder;
using UseCases.Order.GetOrder;
using UseCases.Order.UpdateOrder;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddDbContext<IDbContext, AppDbContext>(builder =>
                builder.UseSqlServer(Configuration.GetConnectionString("Database")));

            // NuGet: AutoMapper.Extensions.Microsoft.Dependency
            services.AddAutoMapper(typeof(OrderMappingProfile));

            services.AddScoped<IRequestHandler<GetOrderQuery, OrderDto>, GetOrderQueryHandler>();
            services.AddScoped<IRequestHandler<UpdateOrderCommand, Unit>, UpdateOrderCommandHandler>();
            services.AddScoped<IHandlerDispatcher, HandlerDispatcher>();

            // The Autofac will call them in a reverse order. The TestMiddleware
            // will be called in the beginning, but we can change the order in
            // the HandlerDispatcher.
            services.AddScoped(typeof(IMiddleware<,>), typeof(CheckOrderMiddleware<,>));
            services.AddScoped(typeof(IMiddleware<,>), typeof(TestMiddleware<,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}