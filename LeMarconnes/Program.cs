using MVC.HttpServices;

var builder = WebApplication.CreateBuilder(args);

var baseAdress = new Uri("https://localhost:7290");

// dependency injection voor service api connecties
builder.Services.AddHttpClient<LoginHttpService>(client =>
{
    client.BaseAddress = baseAdress;
});

builder.Services.AddHttpClient<ReservationHttpService>(client =>
{
    client.BaseAddress = baseAdress;
});

builder.Services.AddHttpClient<RegistrationHttpService>(client =>
{
    client.BaseAddress = baseAdress;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Homepage}/{id?}");

app.UseStaticFiles();

app.Run();








