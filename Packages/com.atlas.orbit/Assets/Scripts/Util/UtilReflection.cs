using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Atlas.Orbit.Util {
    public static class UtilReflection {
        /// <summary>
        /// Creates a list of instances of every subclass of a certain type
        /// </summary>
        /// <typeparam name="T">Parent type</typeparam>
        /// <param name="constructorArgs">Arguments to pass to the constructors of each instance</param>
        /// <returns>List of instances</returns>
        public static List<T> GetAllSubclasses<T>(params object[] constructorArgs) {
            return GetAllSubclasses<T>(Assembly.GetAssembly(typeof(T)));
        }
        /// <summary>
        /// Creates a list of instances of every subclass of a certain type
        /// </summary>
        /// <typeparam name="T">Parent type</typeparam>
        /// <param name="assembly">Assembly to search for types in</param>
        /// <param name="constructorArgs">Arguments to pass to the constructors of each instance</param>
        /// <returns>List of instances</returns>
        public static List<T> GetAllSubclasses<T>(Assembly assembly, params object[] constructorArgs) {
            List<T> objects = new List<T>();
            foreach(Type type in assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));

            return objects;
        }
    }
}
