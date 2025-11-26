// Assets/Scripts/ConversationSystem/Core/Variables/BuiltInVariableProvider.cs
using System;
using UnityEngine;

namespace GimGim.ConversationSystem {
    /// <summary>
    /// Provides built-in variables like current time, platform, etc.
    /// Low priority so game-specific providers can override.
    /// </summary>
    public class BuiltInVariableProvider : IConversationVariableProvider {
        /// <inheritdoc />
        public int Priority => -1000; // Very low priority

        /// <inheritdoc />
        public bool TryGetBool(string variableName, out bool value) {
            value = default;
            
            switch (variableName.ToLower()) {
                case "iseditor":
                    value = Application.isEditor;
                    return true;
                case "isdebugbuild":
                    value = Debug.isDebugBuild;
                    return true;
                case "ismobile":
                    value = Application.isMobilePlatform;
                    return true;
                case "isconsole":
                    value = Application.isConsolePlatform;
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool TryGetInt(string variableName, out int value) {
            value = 0;
            
            switch (variableName.ToLower()) {
                case "currenthour":
                    value = DateTime.Now.Hour;
                    return true;
                case "currentminute":
                    value = DateTime.Now.Minute;
                    return true;
                case "currentday":
                    value = DateTime.Now.Day;
                    return true;
                case "currentmonth":
                    value = DateTime.Now.Month;
                    return true;
                case "currentyear":
                    value = DateTime.Now.Year;
                    return true;
                case "dayofweek":
                    value = (int)DateTime.Now.DayOfWeek;
                    return true;
                case "screenwidth":
                    value = Screen.width;
                    return true;
                case "screenheight":
                    value = Screen.height;
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool TryGetFloat(string variableName, out float value) {
            value = 0;
            
            switch (variableName.ToLower()) {
                case "gametime":
                    value = Time.time;
                    return true;
                case "realtime":
                    value = Time.realtimeSinceStartup;
                    return true;
                case "timescale":
                    value = Time.timeScale;
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool TryGetString(string variableName, out string value) {
            value = null;
            
            switch (variableName.ToLower()) {
                case "platform":
                    value = Application.platform.ToString();
                    return true;
                case "systemlanguage":
                    value = Application.systemLanguage.ToString();
                    return true;
                case "appversion":
                    value = Application.version;
                    return true;
                case "unityversion":
                    value = Application.unityVersion;
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public bool HasVariable(string variableName) {
            string lower = variableName.ToLower();
            return lower switch {
                "iseditor" or "isdebugbuild" or "ismobile" or "isconsole" => true,
                "currenthour" or "currentminute" or "currentday" or "currentmonth" or "currentyear" or "dayofweek" or "screenwidth" or "screenheight" => true,
                "gametime" or "realtime" or "timescale" => true,
                "platform" or "systemlanguage" or "appversion" or "unityversion" => true,
                _ => false
            };
        }
    }
}