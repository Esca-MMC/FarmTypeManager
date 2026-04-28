using System;
using System.Collections.Generic;
using System.Reflection;

namespace FarmTypeManager.Utilities
{
    public static class Reflection
    {
        /******************/
        /* Public methods */
        /******************/

        /// <summary>Searches all loaded assemblies for subclasses of the provided class type and returns them in a list.</summary>
        /// <param name="baseClass">The returned type must be derived from this class.</param>
        /// <returns>A list of types derived from baseClass.</returns>
        public static List<Type> GetAllSubclassTypes(Type baseClass)
        {
            List<Type> types = [];

            if (baseClass == null)
                return types;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    //if this assembly is not excluded (e.g. due to causing errors if checked here)
                    if (!assembly.IsDynamic && assembly.ManifestModule.Name != "<In Memory Module>" && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("Microsoft"))
                        foreach (Type type in TryGetTypes(assembly))
                            if (type?.IsSubclassOf(baseClass) == true)
                                types.Add(type);
                }
                catch (Exception ex)
                {
                    if (Properties.Monitor.IsVerbose)
                        Properties.Monitor.VerboseLog($"{nameof(GetAllSubclassTypes)} skipped an unreadable assembly. Assembly name: {assembly?.GetName()?.Name ?? "(null)"}. Error: \n{ex.ToString()}");
                }
            }
            return types;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Returns every type from an assembly, or an empty array if <see cref="Assembly.GetTypes"/> encounters an error.</summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>An array of Types from the given assembly. Empty if <see cref="Assembly.GetTypes"/> encounters an error.</returns>
        private static Type[] TryGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes(); //attempt to return this assembly's types
            }
            catch (Exception ex) //if an error happens
            {
                if (Properties.Monitor.IsVerbose)
                    Properties.Monitor.VerboseLog($"{nameof(TryGetTypes)} skipped an unreadable assembly. Assembly name: {assembly?.GetName()?.Name ?? "(null)"}. Error: \n{ex.ToString()}");
                return []; //return an empty array
            }
        }
    }
}