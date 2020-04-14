using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using NLog.LayoutRenderers;

namespace BarRaider.SdTools.LayoutRenderers
{
	/// <summary>
	/// Renderer to get the time elapsed since the last logged event.
	/// </summary>
	[LayoutRenderer("elapsed-time")]
	public class ElapsedTimeLayoutRenderer : LayoutRenderer
	{
		private DateTime? _lastTimeStamp;

		/// <summary>
		/// Append elapsed time since the last log entry in milliseconds to the log
		/// </summary>
		/// <param name="builder">StringBuilder containing log parts, the result is appended to this StringBuilder</param>
		/// <param name="logEvent">not used</param>
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			DateTime now = DateTime.Now;
			var lastTimeStamp = _lastTimeStamp ?? now;
			var elapsedTime = now - lastTimeStamp;
			var elapsedTimeString = $"{elapsedTime.TotalMilliseconds:f4}".PadLeft(10);
			builder.Append($"{elapsedTimeString}ms");
			_lastTimeStamp = now;
		}
	}

}
