﻿using System.Reflection;

namespace BotsDotNet.Handling
{
    public class ReflectedPlugin : IExportedPlugin
    {
        public object Instance { get; set; }
        public MethodInfo Method { get; set; }
        public ICommand Command { get; set; }
    }
}
