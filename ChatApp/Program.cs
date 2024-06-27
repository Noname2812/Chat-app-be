using ChatApp.Hubs;
using ChatApp.Installers;
using ChatApp.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.InstallerServiceInAssembly(builder.Configuration);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials());
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// handle middleware
app.UseMiddleware<BlacklistMiddleware>();
app.MapControllers();
app.MapHub<ChatHub>("/hub");
app.Run();
