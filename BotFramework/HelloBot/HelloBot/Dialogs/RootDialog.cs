using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HelloBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;
            string text = message.Text;

            if (text.ToUpper().Contains("ПРИВЕТ"))
            {
                IMessageActivity response = context.MakeMessage();
                response.Text = $"Привет, @{context.Activity.From.Name}";
                await context.PostAsync(response);
            } else
            {
                IMessageActivity response = context.MakeMessage();
                response.Text = $"Я тебя не понял, @{context.Activity.Recipient.Name}";
                await context.PostAsync(response);
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}