using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using libMultiTerm.Plugin.Models;

namespace libMultiTerm.Plugin;

public class PluginLoader {

    public List<UserDefinedPayload>? Payloads { get; private set; }

    public Exception? LoadPlugin(string path) {
        string full = Path.GetFullPath(path);
        AssemblyLoadContext ctx = new AssemblyLoadContext("MutiTermPlugin", isCollectible: false);
        //Assembly assy = ctx.LoadFromAssemblyPath(full);
        Assembly assy;
        try {
            assy = ctx.LoadFromAssemblyPath(full);
        } catch (Exception ex) {
            return ex;
        }

        Payloads = new List<UserDefinedPayload>();

        Type[] assembly_types = assy.GetTypes();

        if (assembly_types.Count() is 0) {
            return null;
        }

        foreach (Type t in assembly_types) {
            if (t.IsAbstract) {
                continue;
            }

            MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (MethodInfo m in methods) {
                bool hasAttribute = m.IsDefined(attributeType: typeof(DeviceCommandAttribute), inherit: false);
                if (!hasAttribute) {
                    continue;
                }

                if (m.ReturnType != typeof(byte[])) {
                    continue;
                }

                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length != 0) {
                    continue;
                }

                Func<byte[]> the_delegate;

                if (m.IsStatic) {
                    Delegate d = Delegate.CreateDelegate(typeof(Func<byte[]>), m, throwOnBindFailure: false);
                    if (d == null) {
                        continue;
                    }
                    the_delegate = (Func<byte[]>)d;
                } else {
                    continue;
                    // ConstructorInfo? ctor = t.GetConstructor(Type.EmptyTypes);
                    // if (ctor == null) {
                    //     continue;
                    // }
                    // object instance = Activator.CreateInstance(t);
                    // Delegate d = Delegate.CreateDelegate(typeof(Func<byte[]>), instance, m, throwOnBindFailure: false);
                    // if (d == null) {
                    //     continue;
                    // }
                    // the_delegate = (Func<byte[]>)d;
                }

                DeviceCommandAttribute? attr = m.GetCustomAttribute<DeviceCommandAttribute>(false);
                string attributeName = attr!.Name;

                UserDefinedPayload udp = new() {
                    Name = attributeName,
                    Delegate = the_delegate,
                };
                Payloads.Add(udp);

                //map[key] = the_delegate;
            }
        }

        //UserDefinedPayloads = map;

        return null;
    }
}