using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions.Authorization;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace SpotifyTransferVkMusic
{
    public class VkAgent
    {
        public VkApi VkApi { get; private set; }

        public VkAgent()
        {
            VkApi =  new VkApi(InitDi());
        }

        public async Task Auth()
        {
        }

        public IEnumerable<Audio> GetMusicList(string userUrl)
        {
            var a = new AudioGetParams() { };
            return VkApi.Audio.Get(a);
        }

        public string GetUserUrl()
        {
            return "vk.com/id" + VkApi.UserId;
        }

        private static ServiceCollection InitDi()
        {
            var di = new ServiceCollection();

            di.AddAudioBypass();

            return di;
        }
    }
}
