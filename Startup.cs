using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.Handlers;
using Backend.Services;

namespace Backend
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddSingleton<CipherService>();
      services.AddSingleton<WebSocketHandler>();  
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebSocketHandler webSocketHandler)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseWebSockets();
      app.Use(async (context, next) =>
      {
        if (context.Request.Path == "/ws")
        {
          if (context.WebSockets.IsWebSocketRequest)
          {
            await webSocketHandler.HandleAsync(context);
          }
          else
          {
            context.Response.StatusCode = 400;
          }
        }
        else
        {
          await next();
        }
      });

      app.UseFileServer();  
    }
  }
}
