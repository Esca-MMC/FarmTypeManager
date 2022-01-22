using StardewModdingAPI;
using System;
using System.Reflection;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Returns every <see cref="Type"/> from an <see cref="Assembly"/>, or an empty array if <see cref="Assembly.GetTypes"/> encounters an error.</summary>
            /// <param name="assembly">The <see cref="Assembly"/> to check.</param>
            /// <returns>An array of Types from the given assembly. Empty if <see cref="Assembly.GetTypes"/> encounters an error.</returns>
            public static Type[] TryGetTypes(Assembly assembly)
            {
                try
                {
                    return assembly.GetTypes(); //attempt to return this assembly's types
                }
                catch (Exception) //if an error happens
                {
                    Monitor.VerboseLog($"TryGetTypes skipped an unreadable assembly. Assembly name: {assembly?.GetName()?.Name ?? "(null)"}");
                    return Array.Empty<Type>(); //return an empty type array
                }
            }
        }
    }
}