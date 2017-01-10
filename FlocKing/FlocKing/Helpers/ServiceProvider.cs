using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlocKing.Helpers
{

    public class ServiceProvider
    {
        private static Dictionary<Type, object> _services;

        static ServiceProvider()
        {
            _services = new Dictionary<Type, object>();

        }
        /// <summary>
        /// Add an object to the service provider.  You are only able to add one object of each type.
        /// Just pass in the object you want to add.
        /// </summary>
        /// <returns>True if the service was successfully added. False if the type is already taken.</returns>
        public static bool AddService<T>(T service)
        {
            bool retVal = false;
            if (!_services.ContainsKey(typeof(T)))
            {
                _services.Add(typeof(T), service);
                retVal = true;
            }
            return retVal;
        }
        /// <summary>
        /// Returns the instance of the object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class
        {
            if (_services.ContainsKey(typeof(T)))
            {
                return (T)_services[typeof(T)];
            }
            return null;
        }
    }
}
