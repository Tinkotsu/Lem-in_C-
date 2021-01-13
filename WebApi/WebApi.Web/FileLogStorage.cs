using System;
using Microsoft.Extensions.Configuration;
using WebApi.Web.Models;

namespace WebApi.Web
{
    public class FileLogStorage : ILogStorage
    {
        private readonly string _path;
        public FileLogStorage(IConfiguration configuration)
        {
            _path = configuration["LogFile"];
        }

        public void Store(LogModel log)
        {
            string[] lines =  
            {
                $"Path: {log.Path}",
                $"QueryString: {log.QueryString}",
                $"Method: {log.Method}",
                $"RequestBody: {log.RequestBody}",
                $"Requested at: {log.RequestedOn}",
                $"Response: {log.Response}",
                $"ResponseCode: {log.ResponseCode}",
                $"Responded at: {log.RespondedOn}",
                Environment.NewLine
            };

            System.IO.File.AppendAllLines(_path, lines);
        }
    }
}
