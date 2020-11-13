using Refit;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIClient
{
    public interface HitokotoApi
    {
        [Get("/")]
        Task<HitokotoContentResponse> FetchContent(Category? c = null);
    }

    public enum Category
    {
        [EnumMember(Value = "a")]
        Anime,
        [EnumMember(Value = "b")]
        Comic,
        [EnumMember(Value = "c")]
        Game,
        [EnumMember(Value = "d")]
        Novel,
        [EnumMember(Value = "e")]
        Myself,
        [EnumMember(Value = "f")]
        Internet,
        [EnumMember(Value = "g")]
        Other,
        [EnumMember(Value = "h")]
        Movies,
        [EnumMember(Value = "i")]
        Poetry,
        [EnumMember(Value = "j")]
        NeteaseCloud, //wang yi yun : )
        [EnumMember(Value = "k")]
        Philosophy,
        [EnumMember(Value = "l")]
        PettyTrick
    }

    public class HitokotoContentResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hitokoto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 网易云
        /// </summary>
        public string from { get; set; }
        /// <summary>
        /// 不自爱里迷失
        /// </summary>
        public string from_who { get; set; }
        /// <summary>
        /// 小忧忧
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int creator_uid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int reviewer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commit_from { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int length { get; set; }
    }
}
