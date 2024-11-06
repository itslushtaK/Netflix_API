using Application;
using CodeInvention.ScalingUp.Web.API;
using Infrastructure;
using Presistence;
using Stripe;
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



StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = "swagger";
    options.DocExpansion(DocExpansion.None);
    options.DefaultModelsExpandDepth(-1);
});

app.UseCors("AllowAngularApp");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UsePresentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
