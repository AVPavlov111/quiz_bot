using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrueQuizBot.Infrastructure;
using TrueQuizBot.Infrastructure.EntityFramework;
using TrueQuizBot.WebApi.Bots;
using TrueQuizBot.WebApi.Dialogs;

namespace TrueQuizBot.WebApi
{
    public class Startup
    {
        private const string CosmosServiceEndpoint = "https://truequizbot-db.documents.azure.com:443/";  
        private const string CosmosDbKey = "DtwTNQsFwKLhbq1HUc8gKZfJi0h5gp2XFogu06Bs3dOSTgplMuA6NHPiVjWHUyi3FSHfxynpbCarjp708gcwKw==";  
        private const string CosmosDbDatabaseName = "truequizbot-db";  
        private const string CosmosDbCollectionName = "truequizbot-storage";

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IQuestionsProvider, QuestionProvider>();

            services.AddSingleton<IDataProvider, SqlServerDataProvider>();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<IStorage>(isp => new CosmosDbStorage(new CosmosDbStorageOptions  
            {  
                AuthKey = CosmosDbKey,  
                CollectionId = CosmosDbCollectionName,  
                CosmosDBEndpoint = new Uri(CosmosServiceEndpoint),  
                DatabaseId = CosmosDbDatabaseName,  
            }));

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();
            
            services.AddSingleton<QuizDialog>();
            services.AddSingleton<TrueEmotionsDialog>();
            services.AddSingleton<TrueLuckyDialog>();
            
            services.AddSingleton<RegistrationDialog>();

            services.AddSingleton<MainDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();

            services.AddTransient(isp => new TrueQuizBotDbContext(TrueQuizBotDbContextFactory.GetSqlServerOptions(Configuration.GetConnectionString("DefaultConnection"))));
            services.AddSingleton<DbContextFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
