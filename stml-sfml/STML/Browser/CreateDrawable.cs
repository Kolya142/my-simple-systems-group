using Browser;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

static class CreateDrawable
{
	public static (Drawable?, float, string?) CreateFromPreDrawable(PreDrawable preDrawable)
	{
		if (preDrawable.GetType() == typeof(CImage))
		{
			return CreateImage((CImage)preDrawable);
		}
		if (preDrawable.GetType() == typeof(CText))
		{
			return CreateText((CText)preDrawable);
		}
		if (preDrawable.GetType() == typeof(CButton))
		{
			return CreateButton((CButton)preDrawable);
		}
		if (preDrawable.GetType() == typeof(CGButton))
		{
			return CreateGButton((CGButton)preDrawable);
		}
		if (preDrawable.GetType() == typeof(CHr))
		{
			return CreateHr((CHr)preDrawable);
		}
		return (null, 0f, null); // Return 0 for the float if Drawable is null
	}

	public static (Drawable?, float, string?) CreateImage(CImage pd)
	{
		using (var stream = new System.IO.MemoryStream(pd.bytes))
		{
			Texture texture = new Texture(stream);
			Sprite sp = new Sprite(texture);
			return (sp, sp.GetLocalBounds().Height, null);
		}
	}

	public static (Drawable?, float, string?) CreateText(CText pd)
	{
		Text text = new Text(pd.text, pd.font);
		return (text, text.GetLocalBounds().Height, null);
	}

	public static (Drawable?, float, string?) CreateButton(CButton pd)
	{
		Text text = new Text(pd.text, pd.font);
		return (text, text.GetLocalBounds().Height, pd.page);
	}

	public static (Drawable?, float, string?) CreateGButton(CGButton pd)
	{
		Text text = new Text(pd.text, pd.font);
		return (text, text.GetLocalBounds().Height, $"{pd.ip}: {pd.port}: {pd.page}");
	}

	public static (Drawable?, float, string?) CreateHr(CHr _)
	{
		RectangleShape rect = new RectangleShape(new Vector2f(3000, 3));
		return (rect, 3, null);
	}
}