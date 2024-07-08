using System;

namespace NAudio.Utils
{
    /// <summary>
    /// Allows us to add descriptions to interop members
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldDescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Field description
        /// </summary>
        public FieldDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public override string ToString()
        {
            return Description;
        }
    }
}
