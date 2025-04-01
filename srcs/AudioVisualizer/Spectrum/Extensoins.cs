namespace Visualizer.UI.Spectrum
{
    public static class Extensoins
    {
        /// <summary>
        /// Checks the length of an array.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="inst">The array to check. This parameter can be null.</param>
        /// <param name="size">The target length of the array.</param>
        /// <param name="exactSize">A value which indicates whether the length of the array has to fit exactly the specified <paramref name="size"/>.</param>
        /// <returns>Array which fits the specified requirements. Note that if a new array got created, the content of the old array won't get copied to the return value.</returns>
        public static T[] CheckBuffer<T>(this T[] inst, long size, bool exactSize = false)
        {
            if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
                return new T[size];
            return inst;
        }
    }
}
