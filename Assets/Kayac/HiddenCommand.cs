using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kayac
{
	/*
	画面をx,y分割して0-8を定まった順で通ってドラッグを終えれば発火する
	番号は左上から、右へ行って、端まで行ったら左端に戻して下に一段ずれる、という順。例えば
	012
	345
	678
	9ab
	*/
	public class HiddenCommand
	{
		char[] key;
		char[] state;
		int nextStatePos;
		int xDiv;
		int yDiv;

		public HiddenCommand(string key, int xDiv, int yDiv)
		{
			Debug.Assert(xDiv * yDiv <= 16);
			this.xDiv = xDiv;
			this.yDiv = yDiv;
			this.key = key.ToCharArray();
			state = new char[key.Length];
		}

		public void OnDrag(PointerEventData data)
		{
			int x = (int)((int)xDiv * data.position.x / (float)Screen.width);
			int y = yDiv - 1 - (int)((int)yDiv * data.position.y / (float)Screen.height);
			int i = (y * xDiv) + x;
			char c = ToHex(i);
			if ((nextStatePos == 0)
				|| ((nextStatePos < state.Length) && (state[nextStatePos - 1] != c)))
			{
				state[nextStatePos] = c;
				nextStatePos++;
				Debug.Log(c);
			}
		}

		public bool OnEndDrag(PointerEventData data)
		{
			var ret = false;
			if (nextStatePos == key.Length)
			{
				ret = true;
				for (int i = 0; i < key.Length; i++)
				{
					if (state[i] != key[i])
					{
						ret = false;
						break;
					}
				}
			}
			nextStatePos = 0;
			return ret;
		}

		char ToHex(int a)
		{
			if ((a >= 0) && (a <= 9))
			{
				return (char)('0' + a);
			}
			else if ((a >= 10) && (a <= 15))
			{
				return (char)('a' + a - 10);
			}
			else
			{
				return '_';
			}
		}
	}
}