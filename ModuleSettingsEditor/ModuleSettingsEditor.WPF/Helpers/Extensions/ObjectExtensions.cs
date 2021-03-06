﻿using Newtonsoft.Json;

namespace ModuleSettingsEditor.WPF.Helpers.Extensions
{
    public static class ObjectExtensions
    {
        public static string Serialize(this object This)
            => JsonConvert.SerializeObject(This);

        public static T CloneBySerialization<T>(this T This)
            => This.Serialize().Deserialize<T>();
    }
}