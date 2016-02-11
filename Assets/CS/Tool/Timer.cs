using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game {
    public class TimerArgs {
        DateTime date;
        int startSecond;
        public string Id;
        public int Second;
        public bool UseDateTime;
        float _removeDelayDate = 0.0f; //延迟删除计时器时间戳
        float _removeDelayMs; //延迟删除计时器时间
        public TimerArgs(string id, int second) {
            date = DateTime.Now;
            Id = id;
            startSecond = second < 0 ? 0 : second;
            Second = startSecond;
            _removeDelayMs = 0;
        }

		public TimerArgs(string id, int second, float removeDelayMs, bool useDateTime) {
            date = DateTime.Now;
            Id = id;
            startSecond = second < 0 ? 0 : second;
            Second = startSecond;
            _removeDelayMs = removeDelayMs;
            UseDateTime = useDateTime;
        }

        //超时判定
        public bool IsTimeout() {
            if (Second <= 0) {
                Second = 0;
                if (_removeDelayDate == 0.0f) {
                    _removeDelayDate = Time.fixedTime;
                }
                if (Time.fixedTime - _removeDelayDate >= _removeDelayMs) {
                    _removeDelayDate = 0.0f;
                    return true;
                } 
                else {
                    return false;
                }

            }
            return false;
        }

        //计时打点
        public bool IsDot() {
            DateTime dt = DateTime.Now;
            TimeSpan ts = dt - date;
            if (ts.TotalSeconds >= 1.0f) {
                if (Second > 0) {
					if (UseDateTime) {
						//考虑切换到后台时的时间损耗
						Second -= (int)(ts.TotalSeconds);
					}
					else {
						//切换到后台的时间损耗不考虑,这里有1秒的误差
						Second--;
					}
                }
                date = dt;
                IsTimeout();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改计时器时间
        /// </summary>
        /// <param name="second"></param>
        public void SetSecond(int second) {
            Second = second;
        }
    }
    
    public delegate void TimerCallBack(TimerArgs args);

    class Bridge {
        public string Id;
        public TimerArgs Args;
        public TimerCallBack CallBack;
        public TimerCallBack EndCallBack;
        public Bridge(string id, TimerArgs args, TimerCallBack callBack, TimerCallBack endCallBack) {
            Id = id;
            Args = args;
            CallBack = callBack;
            EndCallBack = endCallBack;
        }
    }

    public class Timer {
        static List<Bridge> list = new List<Bridge>();

        //添加计时器
		public static void AddTimer(string id, int second, TimerCallBack callBack, TimerCallBack endCallBack, bool useDateTime = true) {
			AddTimer(id, second, callBack, endCallBack, 0, useDateTime);
        }

        //添加计时器
		public static void AddTimer(string id, int second, TimerCallBack callBack, TimerCallBack endCallBack, float removeDelayMs, bool useDateTime = true) {
            if (list.Where(s => s.Id == id).ToList().Count == 0) {
				list.Add(new Bridge(id, new TimerArgs(id, second, removeDelayMs, useDateTime), callBack, endCallBack));
            }
        }

        //移除计时器
        public static void RemoveTimer(string id) {
            Bridge item = list.Where(s => s.Id == id).FirstOrDefault<Bridge>();
            if (item != null) {
                list.Remove(item);
            }
        }

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public static void Clear() {
			list.Clear();
		}

        /// <summary>
        /// 修改计时器时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="second"></param>
        public static void SetTimerSecond(string id, int second) {
            Bridge item = list.Where(s => s.Id == id).FirstOrDefault<Bridge>();
            if (item != null) {
                item.Args.SetSecond(second);
            }
        }

        public static void Action() {
            Bridge bridge;
            for (int i = list.Count - 1; i >= 0; i--) {
                bridge = list[i];
                if (bridge == null) {
                    continue;
                }
                if (bridge.Args.IsTimeout()) {
                    if (bridge.EndCallBack != null) { 
                        bridge.EndCallBack(bridge.Args);
                        list.Remove(bridge);
                        continue;
                    }
                    //if (list.Count > i) { 
                    //    list.RemoveAt(i);
                    //    continue;
                    //}
                }
                if (bridge.Args.IsDot()) {
                    if (bridge.CallBack != null) { 
                        bridge.CallBack(bridge.Args);
                    }
                }
            }
        }
    }
}
