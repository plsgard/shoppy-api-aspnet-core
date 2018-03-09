using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Shoppy.Api.Models
{
    /// <summary>
    /// All API informations.
    /// </summary>
    public class ApiVersionDto
    {
        /// <summary>
        /// Current version of the API.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Last release date of the API.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        public static ApiVersionDto GetApiVersionDto(ApiVersion apiVersion)
        {
            return new ApiVersionDto
            {
                Version = new Version(apiVersion.MajorVersion.GetValueOrDefault(0), apiVersion.MinorVersion.GetValueOrDefault(0)),
                ReleaseDate = new FileInfo(typeof(Startup).Assembly.Location).LastWriteTime
            };
        }

    }
}
