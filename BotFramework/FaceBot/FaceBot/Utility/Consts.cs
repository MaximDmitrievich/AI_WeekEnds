using FaceBot.Dialogs;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FaceBot.Utility
{
    public static class Consts
    {
        public static string Root => nameof(RootDialog);

        public static string FaceAPIKey => "df5f5279d3214db59837e73e6afba01b";
        public static string FaceAPIUri => "https://westus.api.cognitive.microsoft.com/face/v1.0";

        public static IList<FaceAttributeType> faceAttributes =
               new FaceAttributeType[]
               {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair
               };

    }
}