using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Reflection.Emit;

namespace Foo {
    static public class TypeFactory {
        static object _lock = new object();
        static AssemblyName assemblyName;
        static AssemblyBuilder assemblyBuilder;
        static ModuleBuilder module;

        static TypeFactory() {
            lock (_lock) {
                assemblyName = new AssemblyName();
                assemblyName.Name = "FooBarAssembly";
                assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run); //Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                module = assemblyBuilder.DefineDynamicModule("FooBarModule");
            }
        }

        static public object CreateType(string typeName, List<Calculator.DataItem> elements, bool isInterface = false) {
            TypeBuilder typeBuilder = module.DefineType(typeName, TypeAttributes.Public | (isInterface ? TypeAttributes.Class : TypeAttributes.Interface));

            foreach(var element in elements) {
                string propertyName = element.Name;
                Type dataType = element.DataType;
                
                FieldBuilder field = typeBuilder.DefineField("_" + propertyName, dataType, FieldAttributes.Private);
                PropertyBuilder property =
                    typeBuilder.DefineProperty(propertyName,
                                        PropertyAttributes.None,
                                        dataType,
                                        new Type[] { dataType });

                MethodAttributes GetSetAttr =
                        MethodAttributes.Public |
                        MethodAttributes.HideBySig;

                MethodBuilder currGetPropMthdBldr =
                    typeBuilder.DefineMethod("get_value",
                                                GetSetAttr,
                                                dataType,
                                                Type.EmptyTypes);

                ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                MethodBuilder currSetPropMthdBldr =
                    typeBuilder.DefineMethod("set_value",
                                                GetSetAttr,
                                                null,
                                                new Type[] { dataType });

                ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
            }

            Type generetedType = typeBuilder.CreateType();
            return Activator.CreateInstance(generetedType);
        }
    }
}