namespace Orbit.Util {
    public class ArrayParameters<T> {
        private static T[] singleArray = new T[1];
        /// <summary>
        /// Use this when passing a single property to a method with reflection to reuse the array of parameters and create less garbage
        /// </summary>
        public static T[] Single(T parameter) {
            singleArray[0] = parameter;
            return singleArray;
        }
    }
}