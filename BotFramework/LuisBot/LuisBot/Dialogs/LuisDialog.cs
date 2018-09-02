using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Dialogs
{
    [Serializable]
    [LuisModel("", "")]
    public class LuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task ProcessNone(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I do not understand");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Hello")]
        public async Task ProcessHello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello, user!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Doing")]
        public async Task ProcessHow_are_you(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Yes, I can do this thing");
            context.Wait(MessageReceived);
        }

    }
}