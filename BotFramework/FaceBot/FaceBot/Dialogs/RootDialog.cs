using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Linq;
using FaceBot.Utility;

namespace FaceBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IFaceClient _faceclient;

        public RootDialog(IFaceClient faceclient)
        {
            _faceclient = faceclient;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;
            if (message.Attachments?.Count > 0)
            {
                foreach (Attachment attach in message.Attachments)
                {
                    IList<DetectedFace> faces = await _faceclient.Face.DetectWithUrlAsync(attach.ContentUrl, true, false, Consts.faceAttributes);
                    IList<IdentifyResult> ids = await _faceclient.Face.IdentifyAsync("aiweekend", faces.Select(x => x.FaceId.Value).ToList(), 5, 0.5);
                    IList<Person> names = await _faceclient.PersonGroupPerson.ListAsync("aiweekend");

                    List<HeroCard> items = new List<HeroCard>();

                    foreach (var face in faces)
                    {
                        var lst = ids.Where(x => x.FaceId == face.FaceId.Value).ToList();
                        var person = (lst?.Count > 0) && (lst[0].Candidates?.Count > 0) ? names.Where(x => x.PersonId == ids.Where(y => y.FaceId == face.FaceId.Value).Select(y => y.Candidates.Aggregate((l, r) => l.Confidence > r.Confidence ? l : r).PersonId).FirstOrDefault()) : null;
                        string name = person != null ? person.FirstOrDefault().Name : "Неизвестно";
                        string gender = face.FaceAttributes.Gender.Value == Gender.Male ? "Мужской" : "Женский";
                        double age = face.FaceAttributes.Age.Value;
                        string emos = Utilities.GetEmotions(face);

                        items.Add(new HeroCard
                        {
                            Title = $"{name}",
                            Subtitle = $"Пол: {gender}",
                            Text = $"Возраст: {age}" + "\n\n\u200c" + emos,
                        });
                    }

                    IMessageActivity response = context.MakeMessage();

                    HeroCard results = new HeroCard
                    {
                        Title = "Результат распознавания изображения",
                        Subtitle = $"Количество распознанных лиц: {faces.Count}",
                        Images = new List<CardImage> { new CardImage { Url = attach.ContentUrl } }
                    };

                    response.Attachments.Add(results.ToAttachment());

                    await context.PostAsync(response);

                    foreach (var item in items)
                    {
                        IMessageActivity faceOut = context.MakeMessage();
                        faceOut.Attachments.Add(item.ToAttachment());
                        await context.PostAsync(faceOut);
                    }

                    
                }
            } else if (message.Text == "/start")
            {
                IMessageActivity response = context.MakeMessage();
                HeroCard results = new HeroCard
                {
                    Title = "Бот-детектор",
                    Subtitle = $"Распознаю твою личность, эмоцию, пол и возраст",
                    Text = "Просто отправь мне картинку с лицами"
                };

                response.Attachments.Add(results.ToAttachment());

                await context.PostAsync(response);
            }
            

            context.Wait(MessageReceivedAsync);
        }
    }
}