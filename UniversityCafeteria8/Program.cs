using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniversityCafeteria8.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// صفحات الحساب الحالية ما فيها خطوة لتأكيد الإيميل.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // التسجيل بحفظ الاسم الكامل، والدخول بستخدم إيميل فريد.
        options.User.AllowedUserNameCharacters = "";
        options.User.RequireUniqueEmail = true;
    })
    // نفعّل الأدوار وتسجيل الدخول بالإيميل.
    .AddRoles<IdentityRole>()
    .AddSignInManager<EmailSignInManager>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// إذا الزائر فتح صفحة محمية، بنحوّله لصفحة الدخول.
builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");
// نحدّث صلاحيات الحساب حتى لو كان مسجّل دخوله وقت تغيير دوره.
builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ننشئ الأدوار بعد تطبيق المايغريشن على قاعدة البيانات.
await using (var scope = app.Services.CreateAsyncScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var roleName in new[] { "User", "Admin", "SuperAdmin" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded && !await roleManager.RoleExistsAsync(roleName))
            {
                throw new InvalidOperationException($"Could not create the {roleName} role: {string.Join(", ", result.Errors.Select(error => error.Description))}");
            }
        }
    }

    // نعطي دور SuperAdmin لأول حساب محدد بالإعدادات إذا ما في SuperAdmin حاليًا.
    var initialSuperAdminEmail = builder.Configuration["InitialSuperAdminEmail"];
    if (!string.IsNullOrWhiteSpace(initialSuperAdminEmail))
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var superAdmins = await userManager.GetUsersInRoleAsync("SuperAdmin");
        if (superAdmins.Count == 0)
        {
            var user = await userManager.FindByEmailAsync(initialSuperAdminEmail);
            if (user != null)
            {
                var result = await userManager.AddToRoleAsync(user, "SuperAdmin");
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Could not assign the first SuperAdmin role.");
                }
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// نقرأ حساب المستخدم قبل ما نفحص صلاحياته.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

// نبحث بالإيميل عند تسجيل الدخول ونخلي الاسم الكامل محفوظ كاسم المستخدم.
sealed class EmailSignInManager(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
    IOptions<IdentityOptions> options,
    ILogger<SignInManager<IdentityUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<IdentityUser> confirmation)
    : SignInManager<IdentityUser>(userManager, contextAccessor, claimsFactory, options, logger, schemes, confirmation)
{
    public override async Task<SignInResult> PasswordSignInAsync(
        string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByEmailAsync(userName);
        return user == null
            ? SignInResult.Failed
            : await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }
}
