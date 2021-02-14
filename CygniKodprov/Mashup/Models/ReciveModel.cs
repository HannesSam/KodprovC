using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CygniKodprov.Mashup.Models
{
    public class ReciveModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("relations")]
        public Relation[] Relations { get; set; }
        [JsonPropertyName("release-groups")]
        public ReleaseGroups[] Releasegroups { get; set; }
    }

    public class Relation
    {
        public object[] attributes { get; set; }
        public string type { get; set; }
        public object end { get; set; }
        public string targettype { get; set; }
        public string typeid { get; set; }
        public string targetcredit { get; set; }
        public string direction { get; set; }
        public object begin { get; set; }
        public Url url { get; set; }
        public bool ended { get; set; }
        public string sourcecredit { get; set; }
    }

    public class Url
    {
        public string resource { get; set; }
        public string id { get; set; }
    }

    public class ReleaseGroups
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }


}