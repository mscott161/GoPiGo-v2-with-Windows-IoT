using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Dexter.Controller.App
{
    //public static class HttpServerResponse
    //{
    //    public static void WriteResponseError(string text,
    //                                          IOutputStream outputStream)
    //    {
    //        var textBytes = Encoding.UTF8.GetBytes(text);
    //        WriteResponse(HttpContentType.Text, textBytes, HttpStatusCode.HttpCode500, outputStream);
    //    }

    //    public static void WriteResponseOk(IOutputStream outputStream)
    //    {
    //        WriteResponse(null, null, HttpStatusCode.HttpCode204, outputStream);
    //    }

    //    public static void WriteResponseText(string text,
    //                                         IOutputStream outputStream)
    //    {
    //        var textBytes = Encoding.UTF8.GetBytes(text);
    //        WriteResponse(HttpContentType.Text, textBytes, HttpStatusCode.HttpCode200, outputStream);
    //    }

    //    public static void WriteResponseJson(string json,
    //                                         IOutputStream outputStream)
    //    {
    //        var jsonBytes = Encoding.UTF8.GetBytes(json);
    //        WriteResponse(HttpContentType.Json, jsonBytes, HttpStatusCode.HttpCode200, outputStream);
    //    }

    //    public static async Task WriteResponseFile(string pathFileName,
    //                                          HttpContentType mimeType,
    //                                          IOutputStream outputStream)
    //    {
    //        System.Diagnostics.Debug.WriteLine("WriteResponseFile " + pathFileName);

    //        var file = await Package.Current.InstalledLocation.GetFileAsync(@"" + pathFileName);

    //        byte[] fileBytes = null;
    //        switch (mimeType)
    //        {
    //            case HttpContentType.Html:
    //            case HttpContentType.Text:
    //            case HttpContentType.Json:
    //            case HttpContentType.JavaScript:
    //            case HttpContentType.Css:
    //                var fileString = await FileIO.ReadTextAsync(file);
    //                var textBytes = Encoding.UTF8.GetBytes(fileString);
    //                fileBytes = textBytes;
    //                break;
    //            case HttpContentType.Jpeg:
    //            case HttpContentType.Png:
    //                var fileImage = await FileIO.ReadBufferAsync(file);
    //                fileBytes = new byte[fileImage.Length];
    //                fileImage.CopyTo(fileBytes);
    //                break;
    //        }

    //        WriteResponse(mimeType, fileBytes, HttpStatusCode.HttpCode200, outputStream);
    //    }

    //    public static void WriteResponseFile(byte[] content,
    //                                         HttpContentType mimeType,
    //                                         IOutputStream outputStream)
    //    {
    //        WriteResponse(mimeType, content, HttpStatusCode.HttpCode200, outputStream);
    //    }

    //    private static async void WriteResponse(HttpContentType? mimeType,
    //                                      byte[] content,
    //                                      HttpStatusCode httpStatusCode,
    //                                      IOutputStream outputStream)
    //    {
    //        //var stream = outputStream.AsStreamForWrite();
    //        var stream = outputStream;

    //        var responseHeader = new StringBuilder();
    //        responseHeader.Append($"{HttpStatusCodeHelper.GetHttpStatusCodeForHttpHeader(httpStatusCode)}\r\n");

    //        if (httpStatusCode != HttpStatusCode.HttpCode204)
    //        {
    //            responseHeader.Append($"Content-Type: {MimeTypeHelper.GetHttpContentType(mimeType.Value)}\r\n");
    //            responseHeader.Append($"Content-Length: {content.Length}\r\n");
    //        }

    //        responseHeader.Append("Connection: Close\r\n\r\n");

    //        var responsHeaderBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
    //        await stream.WriteAsync(responsHeaderBytes.AsBuffer()); //, 0, responsHeaderBytes.Length);

    //        if (content != null)
    //        {
    //            await stream.WriteAsync(content.AsBuffer()); //, 0, content.Length);
    //        }

    //        await stream.FlushAsync();
    //        stream.Dispose();
    //    }
    //}
}
