using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wey.Tool;

namespace wey.Provider
{
    class PaperMC
    {
        private static readonly Rest Client = new("https://api.papermc.io/v2/");

        // projects

        public class ProjectName
        {
            [JsonPropertyName("projects")]
            public string[] Names { get; set; }
        }

        public static ProjectName GetProjects()
        {
            return Client.Get<ProjectName>($"projects");
        }

        // project

        public class Project
        {
            [JsonPropertyName("versions")]
            public string[] Versions { get; set; }
        }

        public static Project GetProject(string project)
        {
            return Client.Get<Project>($"projects/{project}");
        }

        // build

        public class BuildDownloadApplicationData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }

        public class BuildDownloadData
        {
            [JsonPropertyName("application")]
            public BuildDownloadApplicationData Application { get; set; }
        }

        public class BuildData
        {
            [JsonPropertyName("build")]
            public int ID { get; set; }

            [JsonPropertyName("time")]
            public DateTime ReleaseDate { get; set; }

            [JsonPropertyName("channel")]
            public string Type { get; set; }

            [JsonPropertyName("downloads")]
            public BuildDownloadData Download { get; set; }
        }

        public class Build
        {
            [JsonPropertyName("builds")]
            public BuildData[] Builds { get; set; }
        }

        public static Build GetBuilds(string project, string version)
        {
            return Client.Get<Build>($"projects/{project}/versions/{version}/builds");
        }

        // download

        public static byte[] Download(string project, string version, string build, string download)
        {
            return Client.Download($"projects/{project}/versions/{version}/builds/{build}/downloads/{download}");
        }
    }
}
