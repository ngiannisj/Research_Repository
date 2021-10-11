using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Research_Repository.Data;
using Research_Repository_DataAccess.Initialiser;
using Research_Repository_DataAccess.Repository;
using Research_Repository_DataAccess.Repository.IRepository;
using Research_Repository_DataAccess.Repository.Solr;
using Research_Repository_Models.Solr;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using System;

namespace Research_Repository
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
            //Pass database context using dependency injection
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

            //Pass identity context using dependency injection
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //Used for accessing current user properties
            services.AddHttpContextAccessor();

            //Used for session configuration
            services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });

            //Pass repository contexts using dependency injection
            services.AddScoped<IThemeRepository, ThemeRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IItemRequestRepository, ItemRequestRepository>();
            services.AddScoped<IDbInitialiser, DbInitialiser>();

            services.AddControllersWithViews();

            //Pass solr context using dependency injection
            //services.AddSolrNet<ItemSolr>("http://research-repo-solr-server.azurewebsites.net/solr/research_repository_items");
            services.AddSolrNet<ItemSolr>("http://localhost:8983/solr/research_repository_items"); //For local dev
            services.AddScoped<ISolrIndexService<ItemSolr>, SolrIndexService<ItemSolr, ISolrOperations<ItemSolr>>>();

            //Set up and pass solr admin context using dependency injection
            //const string solrUrl = "http://research-repo-solr-server.azurewebsites.net/solr";
            const string solrUrl = "http://localhost:8983/solr/research_repository_items"; //For local dev
            ISolrCoreAdmin solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), new HeaderResponseParser<string>(), new SolrStatusResponseParser());
            services.AddSingleton(solrCoreAdmin);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitialiser dbInitialiser)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            dbInitialiser.Initialise();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
