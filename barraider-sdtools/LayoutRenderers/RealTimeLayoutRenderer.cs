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
	/// Renderer to get the real time added to the log. Uses the real time (DateTime.Now) instead of the logEvent.TimeStamp since that TimeStamp seems to be some clustered timestamp used for multiple log entries.
	/// </summary>
	[LayoutRenderer("real-time")]
	public class RealTimeLayoutRenderer : LayoutRenderer
	{
		/// <summary>
		/// Append DateTime.Now including milliseconds to the log
		/// </summary>
		/// <param name="builder">StringBuilder containing log parts, the result is appended to this StringBuilder</param>
		/// <param name="logEvent">not used</param>
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{		
			builder.Append($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");		
		}
	}
}
