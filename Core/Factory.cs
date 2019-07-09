using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
using datagrid_mvc5.App_Start;


namespace Core
{

    public static class F
    {
        #region current
        private static Factory _current;

        static Factory Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Factory();
                }
                return _current;
            }
            set { _current = value; }
        }

        #endregion
        /// <summary>
        /// получение объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() 
        {
            return Current.Get<T>();
        }
        /// <summary>
        /// Получение объекта по идентификатору
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Get<T>(int id) 
        {
             return Current.Get<T>(id);
        }

    }

     class Factory
    {
        private IContainer _container;
        internal Factory()
        {
            var builder = AutoFacConfig.Builder;
             _container = builder.Build();
        }

        public T Get<T>() 
        {
            return _container.Resolve<T>();
        }
        public T Get<T>(int id) 
        {
            return _container.Resolve<T>(new NamedParameter("Id",id));
        }

    }


}
