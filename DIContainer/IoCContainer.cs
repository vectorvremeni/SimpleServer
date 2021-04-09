using System;
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

        public void Register<TInterface, TImplementation>()
        {
            types[typeof(TInterface)] = typeof(TImplementation);
        }

        public TInterface Create<TInterface>()
        {
            return (TInterface)Create(typeof(TInterface));
        }

        public object Create(Type type)
        {
            var concreteType = types[type];
            var defaultConstructor = concreteType.GetConstructors()[0];
            var defaultParams = defaultConstructor.GetParameters();
            var parameters = defaultParams.Select(param => Create(param.ParameterType)).ToArray();

            return defaultConstructor.Invoke(parameters);
        }

        public void Register(Type type, Type instance)
        {
            types[type] = instance;
        }
    }
}
