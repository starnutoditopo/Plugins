using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Plugins.Extensibility.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Plugins.Helpers
{
    public static class PluginHelper
    {
        private static IEnumerable<Type> GetAllAvailableTypes(string[] pluginPaths)
        {
            var pluginAssemblies = GetPluginAssemblies(pluginPaths)
                .ToList();

            Debug.WriteLine("Loaded assemblies:");
            foreach (var assembly in pluginAssemblies.OrderBy(a=>a.FullName))
            {
                Debug.WriteLine($"   {assembly.FullName}");
            }

            var allAvailableTypes = GetAllTypesInAssemblies(pluginAssemblies);
            return allAvailableTypes;
        }

        private static IEnumerable<Assembly> GetPluginAssemblies(string[] pluginPaths)
        {
            HashSet<Type> sharedTypes = new HashSet<Type>();
            var mainAssembly = typeof(PluginHelper).Assembly;
            var mainAssemblyLocation = mainAssembly.Location;

            //First of all, return the executing assembly
            yield return mainAssembly;


            foreach (var path in pluginPaths)
            {
                if (Directory.Exists(path))
                {
                    var fileNames = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)

                        //Avoid to load another instance of executing assembly!
                        .Where(fullPath => !StringComparer.InvariantCultureIgnoreCase.Equals(mainAssemblyLocation, fullPath))
                        ;

                    foreach (var fileName in fileNames)
                    {
                        var pluginFile = fileName;
                        using var loader = PluginLoader.CreateFromAssemblyFile(
                            pluginFile,

                            configure =>
                            {
                                configure.PreferSharedTypes = true;
                            }
                            );
                        var assembly = loader.LoadDefaultAssembly();
                        yield return assembly;
                    }
                }
            }
        }

        private static IEnumerable<Type> GetAllTypesInAssemblies(IEnumerable<Assembly> availableAssemblies)
        {
            HashSet<Type> typesToInspect = new HashSet<Type>();
            foreach (var assembly in availableAssemblies)
            {
                Debug.WriteLine($"   Scanning assembly '{assembly.FullName}'...");
                try
                {
                    var types = assembly.GetTypes();
                    typesToInspect.AddRange(types);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Warning: an exception occurred while scanning types to inject. Details: {e}");
                    throw;
                }
            }
            Debug.WriteLine($"   Found {typesToInspect.Count} public types...");

            return typesToInspect;
        }

        private static IEnumerable<ExportAttribute> GetExportedTypeAttributes(this Type type)
        {
            if (type.IsAbstract || type.IsInterface) yield break;
            var exportAttributes = type
                .GetCustomAttributes(typeof(ExportAttribute), true)
                .Cast<ExportAttribute>();

            foreach (var exportAttribute in exportAttributes)
            {
                var contract = exportAttribute.Contracts;
                if (!type.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException(
                        string.Format("Type {0} does not implement exported type {1}.", type, contract));
                }
                yield return exportAttribute;
            }
        }

        private static void Register(this IServiceCollection services, Type[] contracts, Type type, ExportMode exportMode)
        {
            if (exportMode == ExportMode.Singleton)
            {
                foreach (Type contract in contracts)
                {
                    services.AddSingleton(contract, type);
                }
            }
            else if (exportMode == ExportMode.Transient)
            {
                foreach (Type contract in contracts)
                {
                    services.AddTransient(contract, type);
                }
            }
            else if (exportMode == ExportMode.PerThread)
            {
                foreach (Type contract in contracts)
                {
                    services.AddScoped(contract, type);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unrecognized export mode '{exportMode}'");
            }
        }

        public static void RegisterPlugins(IServiceCollection services)
        {
            Debug.WriteLine($"Registering plugins...");
            var directoryName = AppDomain.CurrentDomain.BaseDirectory;
            var pluginPaths = new string[] {
                directoryName
                //Path.Combine(directoryName, "Plugins")
            };

            var allAvailableTypes = GetAllAvailableTypes(pluginPaths);

            // register exported types
            foreach (var type in allAvailableTypes)
            {
                //Debug.WriteLine($"Scanning type '{type.Name}' ('{type.FullName}')...");

                var exportedTypeAttributes = type.GetExportedTypeAttributes();
                foreach (var exportedTypeAttribute in exportedTypeAttributes)
                {
                    services.Register(exportedTypeAttribute.Contracts, type, exportedTypeAttribute.Mode);

                    #region just "logging"
                    {
                        StringBuilder message = new StringBuilder($"   Type '{type}' registered for the following contracts:")
                                .AppendLine();
                        foreach (var contract in exportedTypeAttribute.Contracts)
                        {
                            message.AppendLine($"      {contract}");
                        }
                        Debug.WriteLine(message.ToString());
                    }
                    #endregion
                }
            }
            Debug.WriteLine($"... plugins registered.");
        }
    }
}

