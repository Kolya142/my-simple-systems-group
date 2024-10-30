using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Mouse;

namespace Browser
{
	static class Parser
	{
		public static List<PreDrawable> Parse(string code, Font font, SttpClient sttpClient)
		{
			List<PreDrawable> Commands = new List<PreDrawable>();
			foreach (var line in code.Split("\n"))
			{
				if (line.Length == 0)
				{
					continue;
				}

				var sp = line.Split(new[] { ": " }, StringSplitOptions.None);
				if (sp.Length == 0)
				{
					continue;
				}

				if (sp.Length == 2)
				{
					if (sp[0] == "Hr")
					{
						Commands.Add(new CHr());
					}
				}

				if (sp.Length == 2)
				{
					if (sp[0] == "Text")
					{
						Commands.Add(new CText { text = sp[1].Replace("<gh>", ":").Replace("<a>", "<").Replace("<b>", ">"), font=font }) ;
					}
					else if (sp[0] == "Image")
					{
						sttpClient.GenerateHeader(sp[1], "BetterFly");
						Commands.Add(new CImage {bytes=sttpClient.Get()});
					}
				}

				if (sp.Length == 3)
				{
					if (sp[0] == "Button")
					{
						Commands.Add(new CButton { text = sp[1].Replace("<gh>", ":").Replace("<a>", "<").Replace("<b>", ">"), font = font, page = sp[2].Trim().Replace("\n", "").Replace("\r", "") });
					}
				}

				if (sp.Length == 5)
				{
					if (sp[0] == "GButton")
					{
						Commands.Add(
							new CGButton { 
								text = sp[1].Replace("<gh>", ":").Replace("<a>", "<").Replace("<b>", ">"), 
								font = font, 
								ip = sp[2].Trim().Replace("\n", "").Replace("\r", ""),
								port = sp[3].Trim().Replace("\n", "").Replace("\r", ""),
								page = sp[4].Trim().Replace("\n", "").Replace("\r", "") });
					}
				}
			}
			return Commands;
		}
	}
}
