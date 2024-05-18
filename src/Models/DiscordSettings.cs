using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Models
{
	public class DiscordSettings
	{
		private string urlFormat = "https://streamkit.discord.com/overlay/voice/{0}/{1}?icon=true&online=true&logo=white&text_color=%23{2}&text_size={3}&text_outline_color=%23{4}&" +
			"text_outline_size={5}&text_shadow_color=%23{6}&text_shadow_size={7}&bg_color=%23{8}&bg_opacity={9}&bg_shadow_color=%23{10}&bg_shadow_size={11}&invite_code=&" +
			"limit_speaking={12}&small_avatars={13}&hide_names={14}&fade_chat={15}&streamer_avatar_first={16}";

		private string _discordUrl { get; set; }
		public string DiscordUrl
		{
			get
			{
				return _discordUrl;
			}
			set => _discordUrl = string.Format(urlFormat, DiscordServer, DiscordChannel, TextColorHex, TextSize, TextOutlineColorHex, TextOutlineSize, TextShadowColorHex,
				TextShadowSize, BgColorHex, BgOpacity, BgShadowColorHex, BgShadowSize, LimitSpeaking, SmallAvatars, HideNames, FadeChat, StreamerAvatarFirst);
		}

		private string _discordServer { get; set; } = string.Empty;
		public string DiscordServer
		{
			get
			{
				return _discordServer;
			}
			set => _discordServer = value;
		}
		private string _discordChannel { get; set; } = string.Empty;
		public string DiscordChannel
		{
			get
			{
				return _discordChannel;
			}
			set => _discordChannel = value;
		}
		private string _textColorHex { get; set; } = "ffffff";
		public string TextColorHex
		{
			get
			{
				return _textColorHex;
			}
			set => _textColorHex = value;
		}
		private string _textSize { get; set; } = "14";
		public string TextSize
		{
			get
			{
				return _textSize;
			}
			set => _textSize = value;
		}
		private string _textOutlineColorHex { get; set; } = "000000";
		public string TextOutlineColorHex
		{
			get
			{
				return _textOutlineColorHex;
			}
			set => _textOutlineColorHex = value;
		}
		private string _textOutlineSize { get; set; } = "0";
		public string TextOutlineSize
		{
			get
			{
				return _textOutlineSize;
			}
			set => _textOutlineSize = value;
		}
		private string _textShadowColorHex { get; set; } = "000000";
		public string TextShadowColorHex
		{
			get
			{
				return _textShadowColorHex;
			}
			set => _textShadowColorHex = value;
		}
		private string _textShadowSize { get; set; } = "0";
		public string TextShadowSize
		{
			get
			{
				return _textShadowSize;
			}
			set => _textShadowSize = value;
		}
		private string _bgColorHex { get; set; } = "1e2124";
		public string BgColorHex
		{
			get
			{
				return _bgColorHex;
			}
			set => _bgColorHex = value;
		}
		private string _bgOpacity { get; set; } = "0";
		public string BgOpacity
		{
			get
			{
				return _bgOpacity;
			}
			set => _bgOpacity = value;
		}
		private string _bgShadowColorHex { get; set; } = "000000";
		public string BgShadowColorHex
		{
			get
			{
				return _bgShadowColorHex;
			}
			set => _bgShadowColorHex = value;
		}
		private string _bgShadowSize { get; set; } = "0";
		public string BgShadowSize
		{
			get
			{
				return _bgShadowSize;
			}
			set => _bgShadowSize = value;
		}
		private string _inviteCode { get; set; } = string.Empty;
		public string InviteCode
		{
			get
			{
				return _inviteCode;
			}
			set => _inviteCode = value;
		}
		private string _limitSpeaking { get; set; } = "false";
		public string LimitSpeaking
		{
			get
			{
				return _limitSpeaking;
			}
			set => _limitSpeaking = value;
		}
		private string _smallAvatars { get; set; } = "true";
		public string SmallAvatars
		{
			get
			{
				return _smallAvatars;
			}
			set => _smallAvatars = value;
		}
		private string _hideNames { get; set; } = "false";
		public string HideNames
		{
			get
			{
				return _hideNames;
			}
			set => _hideNames = value;
		}
		private string _fadeChat { get; set; } = "0";
		public string FadeChat
		{
			get
			{
				return _fadeChat;
			}
			set => _fadeChat = value;
		}
		private string _streamerAvatarFirst { get; set; } = "false";
		public string StreamerAvatarFirst
		{
			get
			{
				return _streamerAvatarFirst;
			}
			set => _streamerAvatarFirst = value;
		}



	}
}
