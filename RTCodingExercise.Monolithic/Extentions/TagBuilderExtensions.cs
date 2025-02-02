using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RTCodingExercise.Monolithic.Extensions
{
    public static class TagBuilderExtensions
    {
        public static string ToHtmlString(this TagBuilder tagBuilder)
        {
            using var writer = new StringWriter();
            
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
