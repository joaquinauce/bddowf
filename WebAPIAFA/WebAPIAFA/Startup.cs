using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebAPIAFA.Entity;
using WebAPIAFA.Helpers;
using WebAPIAFA.Helpers.Config;
using WebAPIAFA.Helpers.Crypto;
using WebAPIAFA.Helpers.File;
using WebAPIAFA.Helpers.Mail;
using WebAPIAFA.Helpers.Menu;
using WebAPIAFA.Helpers.StampRequest;
using WebAPIAFA.Helpers.ValidatorSignature;
using WebAPIAFA.Helpers.Verification;
using WebAPIAFA.Middleware;
using WebAPIAFA.Repository;
using WebAPIAFA.Repository.IRepository;
using WebAPIAFA.Services;
using WebAPIAFA.Services.IServices;

namespace WebAPIAFA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddHttpContextAccessor();

            services.AddControllers()
                .AddJsonOptions(x => 
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

            //Encriptacion de cadena de conexion.
            Crypto cripto = new Crypto();
            var connection = cripto.DecryptText(Configuration.GetConnectionString("DefaultConnection"));
            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(connection));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(Configuration["Jwt"])),
                    ClockSkew = TimeSpan.Zero
                });


            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAFA", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));

            // Utilidades
            services.AddSingleton<IConfig, Config>();
            services.AddSingleton<ICrypto, Crypto>();
            services.AddScoped<IDbOperation, DbOperation>();
            services.AddScoped<IHelperFile, HelperFile>();
            services.AddScoped<IHelperMail, HelperMail>();
            services.AddScoped<IMenu,Menu>();
            services.AddScoped<IStampRequestHelper, StampRequestHelper>();
            services.AddScoped<IValidator,Validator>();
            services.AddScoped<IHelperVerificationCode, HelperVerificationCode>();

            // Repositorios
            services.AddScoped<IActionTypeRepository, ActionTypeRepository>();
            services.AddScoped<IBulletinRepository, BulletinRepository>();
            services.AddScoped<IBulletinSubscriberRepository, BulletinSubscriberRepository>();
            services.AddScoped<IBulletinTypeRepository, BulletinTypeRepository>();
            services.AddScoped<IClubAuthorityRepository, ClubAuthorityRepository>();
            services.AddScoped<IClubFileRepository, ClubFileRepository>();
            services.AddScoped<IClubFileTypeRepository, ClubFileTypeRepository>();
            services.AddScoped<IClubInformationRepository, ClubInformationRepository>();
            services.AddScoped<IClubMandateRepository, ClubMandateRepository>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IClubStaffRepository, ClubStaffRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<ILeagueRepository, LeagueRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IPassRepository, PassRepository>();
            services.AddScoped<IPassStepRepository, PassStepRepository>();
            services.AddScoped<IPassTypeRepository, PassTypeRepository>();            
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IReasonRepository, ReasonRepository>();
            services.AddScoped<IResponsibleXClubRepository, ResponsibleXClubRepository>();
            services.AddScoped<IResponsibleXLeagueRepository, ResponsibleXLeagueRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleUserRepository, RoleUserRepository>();
            services.AddScoped<ISanctionRepository, SanctionRepository>();
            services.AddScoped<ISponsorRepository, SponsorRepository>();
            services.AddScoped<IStampRequestRepository, StampRequestRepository>();
            services.AddScoped<IStepRepository, StepRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITournamentDivisionRepository, TournamentDivisionRepository>();
            services.AddScoped<IUserFolderRepository, UserFolderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTypeRepository, UserTypeRepository>();

            // Servicios
            services.AddScoped<IAdministrationService, AdministrationService>();
            services.AddScoped<IBulletinService, BulletinService>();
            services.AddScoped<IClubAuthorityService, ClubAuthorityService>();
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IFolderService,FolderService>();
            services.AddScoped<ILeagueService, LeagueService>();
            services.AddScoped<ISanctionService, SanctionService>();
            services.AddScoped<ISponsorService, SponsorService>();
            services.AddScoped<IPassService, PassService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "wwwroot", "Images")),
                RequestPath = "/Images"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
