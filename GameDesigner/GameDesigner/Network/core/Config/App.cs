using Net.Share;
using System;
using System.Reflection;

namespace Net.Config
{
    public static class App
    {
        public static void Setup() => Init();

        public static void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblie in assemblies)
            {
                foreach (var type in assemblie.GetTypes())
                {
                    var members = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    foreach (var member in members)
                    {
                        var runtimeInitialize = member.GetCustomAttribute<RuntimeInitializeOnLoadMethod>();
                        if (runtimeInitialize == null)
                            continue;
                        member.Invoke(null, null);
                    }
                }
            }
        }
    }
}
