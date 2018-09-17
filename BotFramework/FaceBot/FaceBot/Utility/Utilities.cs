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

        public static string GetAverageEmotion(DetectedFace faces)
        {
            string result = "";
            Emotion average = new Emotion();

            
            
            return result;
        }
    }
}