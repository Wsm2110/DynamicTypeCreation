using DynamicTypeCreation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;


namespace DynamicTypeCreation
{

    class Program
    {
        public static void Main(string[] args)
        {

            // No intellisense .... visual studio 2019??? :)
            dynamic extended = new DynamicBuilder("WSM.Dynamic.Type", "Test", "Person")
                .Extend<IPerson>()
                .Build();

            extended.Name = "Peter";

            dynamic extended2 = new DynamicBuilder("WSM.Dynamic.Type", "Test", "Person2")
            .Extend<IPerson>()
            .Extend<IPersonB>()
            .Build();

            //Type test = LoadSomething("Awesomeness, WSM.Dynamic, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            //cant cast type variable to static type -_-   
            // Console.Read();
        }

        public static Type LoadSomething(string assemblyQualifiedName)
        {
            // This will return null
            // Just here to test that the simple GetType overload can't return the actual type
            var t0 = Type.GetType(assemblyQualifiedName);

            // Throws exception is type was not found
            return Type.GetType(
                assemblyQualifiedName,
                (name) =>
                {
                    // Returns the assembly of the type by enumerating loaded assemblies
                    // in the app domain            
                    return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
                },
                null,
                true);
        }

    }
}
