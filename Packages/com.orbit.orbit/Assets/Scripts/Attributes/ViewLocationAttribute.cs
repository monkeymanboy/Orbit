using System;

namespace Orbit.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ViewLocationAttribute : Attribute {
        public string Location { get; }
        public string FullPath { get; set; }

        /// <summary>
        /// When applied to a UIView, indicates that it uses the resource at
        /// <paramref name="location"/> instead of the default name.
        /// </summary>
        /// <param name="location">the name of the resource to use</param>
        public ViewLocationAttribute(string location) {
            Location = location;
        }
        
        public ViewLocationAttribute() {
            Location = null;
        }
    }
}
