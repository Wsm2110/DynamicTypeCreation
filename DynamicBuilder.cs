using DynamicTypeCreation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeCreation
{
    public class DynamicBuilder : IBuilder
    {
        #region Fields

        private AssemblyBuilder _assembly;
        private ModuleBuilder _module;
        private TypeBuilder _builder;

        private List<Type> _types;

        #endregion

        public DynamicBuilder(string assemblyName, string moduleName, string typeName)
        {
            _assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            _module = _assembly.DefineDynamicModule(moduleName);
            _builder = _module.DefineType(typeName, TypeAttributes.Public);
            _builder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            _types = new List<Type>();
        }

        public IBuilder Extend<T>() where T : class
        {
            _types.Add(typeof(T));
            _builder.AddInterfaceImplementation(typeof(T));

            foreach (var v in typeof(T).GetProperties())
            {
                var field = _builder.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
                var property = _builder.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                var getter = _builder.DefineMethod("get_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType, new Type[0]);
                var setter = _builder.DefineMethod("set_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[] { v.PropertyType });

                var getGenerator = getter.GetILGenerator();
                var setGenerator = setter.GetILGenerator();

                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, field);
                getGenerator.Emit(OpCodes.Ret);

                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Stfld, field);
                setGenerator.Emit(OpCodes.Ret);

                property.SetGetMethod(getter);
                property.SetSetMethod(setter);

                _builder.DefineMethodOverride(getter, v.GetGetMethod());
                _builder.DefineMethodOverride(setter, v.GetSetMethod());
            }

            return this;
        }
        
                
        public object Build()
        {
            var instance = Activator.CreateInstance(_builder.CreateType());

            foreach (var v in _types.SelectMany(type => type.GetProperties().Where(x => x.PropertyType.GetConstructor(new Type[0]) != null)))
            {
                instance.GetType()
                        .GetProperty(v.Name)
                        .SetValue(instance, Activator.CreateInstance(v.PropertyType), null);
            }

            return instance;
        }

    }
}
