using Microsoft.Xrm.Sdk;

namespace McGlobalAzureFunctions.Gate
{
    public static class Protect
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> when the value is null.
        /// </summary>
        /// <typeparam name="T">Type parameter.</typeparam>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        public static void ForNull<T>(T paramValue, string paramName)
            where T : class
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName, $"{typeof(T)} object value must not be null.");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> when the value is null or white space.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="message">The message for <see cref="ArgumentException"/>.</param>
        public static void ForNullOrWhiteSpace(string? paramValue, string paramName, string message = "String value must not be null or whitespace.")
        {
            if (string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> when the value is null or white space.
        /// </summary>
        /// <param name="entity">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="message">The message for <see cref="ArgumentException"/>.</param>
        public static void ForNullLogicalName(Entity entity, string message = "Entity is null or logical name is missing")
        {
            if (string.IsNullOrWhiteSpace(entity.LogicalName))
            {
                throw new ArgumentException(message, "Entity Logical name");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> when the value is less than zero.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="message">The message for <see cref="ArgumentException"/>.</param>
        public static void ForNegative(int paramValue, string paramName, string message = "Int value must not be less than zero.")
        {
            if (paramValue < 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> when the value is zero.
        /// </summary>
        /// <param name="paramValue">The parameter value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="message">The message for <see cref="ArgumentException"/>.</param>
        public static void ForZero(int paramValue, string paramName, string message = "Int value must not be zero.")
        {
            if (paramValue == 0)
            {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}
