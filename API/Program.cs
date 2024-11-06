using API;
using Application;
using Infrastructure;
using Presistence;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPresistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPresentation(builder.Configuration);
builder.Services.AddInfrastructure();
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = "swagger";
    options.DocExpansion(DocExpansion.None);
    options.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();  // Ensure HTTPS is used before setting CORS
app.UseCors("AllowAngularApp");  // Moved higher in the pipeline
app.UseStaticFiles();  // Serve static files (can be after CORS as well)

app.UsePresentation();  // Custom middleware
app.UseAuthentication();  // Authentication middleware
app.UseAuthorization();  // Authorization middleware

app.MapControllers();  // Map API controllers
app.MapRazorPages();  // Map Razor pages

app.Run();
