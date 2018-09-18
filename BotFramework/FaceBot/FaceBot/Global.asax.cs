using Autofac;
using FaceBot.Dialogs;
using FaceBot.Utility;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace FaceBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterType<BotData>();

                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                var store = new TableBotDataStore(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

                builder.Register(x => new FaceClient(new ApiKeyServiceClientCredentials(Consts.FaceAPIKey))
                {
                    BaseUri = new Uri(Consts.FaceAPIUri)
                })
                    .Keyed<IFaceClient>(FiberModule.Key_DoNotSerialize)
                    .AsImplementedInterfaces()
                    .SingleInstance();

                builder.RegisterType<RootDialog>()
                    .Named<IDialog<object>>(nameof(RootDialog))
                    .SingleInstance();
            }
            );
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
