﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace IoC
{
    public class IoCContainer
    {
        private readonly Dictionary<Type, Type> types = new Dictionary<Type, Type>();

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            types[typeof(TInterface)] = typeof(TImplementation);
        }

        public TInterface Create<TInterface>()
        {
            return (TInterface)Create(typeof(TInterface));
        }

        private object Create(Type type)
        {
            //Find a default constructor using reflection
            var concreteType = types[type];
            var defaultConstructor = concreteType.GetConstructors()[0];
            //Verify if the default constructor requires params
            var defaultParams = defaultConstructor.GetParameters();
            //Instantiate all constructor parameters using recursion
            var parameters = defaultParams.Select(param => Create(param.ParameterType)).ToArray();

            return defaultConstructor.Invoke(parameters);
        }
    }
}
