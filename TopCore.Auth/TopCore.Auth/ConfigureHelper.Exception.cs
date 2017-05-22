using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class Exception
        {
            public static void Middleware(IApplicationBuilder app)
            {
                if (Environment.IsDevelopment())
                    app.UseDeveloperExceptionPage();
                else
                    app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}