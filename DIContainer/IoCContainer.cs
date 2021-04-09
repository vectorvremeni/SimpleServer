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
        private readonly Dictionary<Type, Service> types = new Dictionary<Type, Service>();

        public void Register<TInterface, TImplementation>()
        {
            Service s = new Service();
            s.service = typeof(TImplementation);
            s.ServiceType = Service.Transient;
            types[typeof(TInterface)] = s;
        }

        public void RegisterSingleton<TInterface, TImplementation>()
        {
            Service s = new Service();
            s.service = typeof(TImplementation);
            s.ServiceType = Service.Singleton;
            types[typeof(TInterface)] = s;
        }

        public TInterface Create<TInterface>()
        {
            return (TInterface)Create(typeof(TInterface));
        }

        public object Create(Type type)
        {
            var concreteType = types[type];

            if (concreteType.Initialized == false || concreteType.ServiceType == Service.Transient)
            {
                var defaultConstructor = concreteType.service.GetConstructors()[0];
                var defaultParams = defaultConstructor.GetParameters();
                var parameters = defaultParams.Select(param => Create(param.ParameterType)).ToArray();
                concreteType.Initialized = true;
                concreteType.Instance = defaultConstructor.Invoke(parameters);
                return concreteType.Instance;
            }
            else
            {
                return concreteType.Instance;
            }
        }

        public void Register(Type type, Type instance)
        {
            Service s = new Service();
            s.ServiceType = Service.Singleton;
            s.service = instance;
            types[type] = s;
        }
    }

    public class Service
    {
        public Type service { get; set; }
        public object Instance { get; set; }
        public String ServiceType { get; set; }
        public bool Initialized = false;

        public static String Singleton="Singleton";
        public static String Transient = "Transient";
    }
}
