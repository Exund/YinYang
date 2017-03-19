﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YinYang
{
	public sealed class StaticFileHandler : RequestHandler
	{
		private const string DirectoryPathChars = @"/\";
		private const string ExtraPathChars = DirectoryPathChars + @"-_=";

		public string RootDirectory { get; set; } = "";

		public Task HandleRequest(HttpListenerContext context)
		{
			string path = context.Request.Url.AbsolutePath;
			path = path.Substring("/test".Length);
			if (path.Length > 0)
			{
				path = path.Substring(1);
			}
			IsSafePath(path);
			path = Path.Combine(RootDirectory, path);
			var info = new FileInfo(path);

			return info.OpenRead().CopyToAsync(context.Response.OutputStream);
		}

		private bool IsSafePath(string path)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (path.Length <= 0) throw new ArgumentException("path is empty", nameof(path));
#warning Ensure first char is slash, ensure path starts with only one slash
			for (int i = 1; i < path.Length; i++)
			{
				char current = path[i++];
				if (!IsSafePathChar(current))
				{
					if (current == '.' && path[i - 1] == '.' && DirectoryPathChars.Contains(path[i - 2]))
					{
						return false;
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool IsSafePathChar(char c)
		{
			return char.IsLetterOrDigit(c) || ExtraPathChars.Contains(c);
		}
	}
}