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
                    IList<DetectedFace> faces = await _faceclient.Face.DetectWithUrlAsync(attach.ContentUrl, true, true, Consts.faceAttributes);
                    IList<IdentifyResult> ids = await _faceclient.Face.IdentifyAsync("AI_Weekend", faces.Select(x => x.FaceId).ToList() as IList<Guid>);
                    IList<Person> names = await _faceclient.PersonGroupPerson.ListAsync("AI_Weekend");


                    

                    foreach(var face in faces)
                    {
                        List<ReceiptItem> items = new List<ReceiptItem>();
                        List<Fact> facts = new List<Fact>();

                        items.Add(new ReceiptItem
                        {
                            Title = (ids.Where(x => x.FaceId == face.FaceId).ToList().Count > 0) ? names.Where(x => x.PersonId == ids.Where(y => y.FaceId == face.FaceId).Select(y => y.Candidates.Where(z => z.Confidence > 0.8)).Select(y => y.FirstOrDefault().PersonId).FirstOrDefault()).FirstOrDefault().Name : "Неизвестно",
                            Subtitle = $"{face.FaceAttributes.Gender}",
                            Price = $"{face.FaceAttributes.Age}",
                            
                        });
                        facts.Add(new Fact
                        {
                            Key = "Эмоция",
                            Value = $"{}"
                        });
                    }

                    IMessageActivity response = context.MakeMessage();

                    ReceiptCard results = new ReceiptCard
                    {
                        Title = "Результат распознавания изображения",
                        
                        Total = $"Количество человек: {faces.Count}",

                    };

                    response.Attachments.Add(results.ToAttachment());

                    await context.PostAsync(response);
                }
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}