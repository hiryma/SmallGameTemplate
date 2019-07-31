using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kayac
{
	public static class DataUtil
	{
		public static int? GetInt(string s)
		{
			if (s == null)
			{
				return null;
			}
			else
			{
				int ret;
				if (!int.TryParse(s, out ret))
				{
					Debug.Assert(false, "DataUtil.GetInt failed. input=" + s);
					return null;
				}
				else
				{
					return ret;
				}
			}
		}

		public static float? GetFloat(string s)
		{
			if (s == null)
			{
				return null;
			}
			else
			{
				float ret;
				if (!float.TryParse(s, out ret))
				{
					Debug.Assert(false, "DataUtil.GetFloat failed. input=" + s);
					return null;
				}
				else
				{
					return ret;
				}
			}
		}

		public static bool? GetBool(string s)
		{
			if (s == null)
			{
				return null;
			}
			else
			{
				if (s == "true")
				{
					return true;
				}
				else if (s == "false")
				{
					return false;
				}
				else
				{
					Debug.Assert(false, "DataUtil.GetBool failed. input=" + s);
					return null;
				}
			}
		}
	}
}
