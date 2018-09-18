using Autofac;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FaceBot.Utility
{
    public static class Utilities
    {
        public static IDialog<object> ResolveDialog(string name)
        {
            return Conversation.Container.ResolveNamed<IDialog<object>>(name);
        }

        public static string GetEmotions(DetectedFace faces)
        {
            string result = @"";
            Emotion emotion = faces.FaceAttributes.Emotion;
            Dictionary<string, double> emos = new Dictionary<string, double>()
            {
                ["Злость"] = emotion.Anger,
                ["Презрение"] = emotion.Contempt,
                ["Отвращение"] = emotion.Disgust,
                ["Страх"] = emotion.Fear,
                ["Счастье"] = emotion.Happiness,
                ["Нейтральная"] = emotion.Neutral,
                ["Грусть"] = emotion.Sadness,
                ["Удивление"] = emotion.Surprise

            };
            var max = emos.Aggregate((l, r) => l.Value > r.Value ? l : r);
            result = $"Эмоция: {max.Key}";
            return result;
        }
    }
}