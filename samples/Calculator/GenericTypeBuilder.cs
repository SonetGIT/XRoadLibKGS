﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    // Define a trivial base class and two trivial interfaces 
    // to use when demonstrating constraints.
    //
    public class ExampleBase { }

    public interface IExampleA { }

    public interface IExampleB { }

    // Define a trivial type that can substitute for type parameter 
    // TSecond.
    //
    public class ExampleDerived : ExampleBase, IExampleA, IExampleB { }


    public class GenericTypeBuilder
    {
        public static void Execute()
        {
            // Define a dynamic assembly to contain the sample type. The
            // assembly will not be run, but only saved to disk, so
            // AssemblyBuilderAccess.Save is specified.
            //
            AppDomain currentDomain = AppDomain.CurrentDomain;
            AssemblyName assemblyName = new AssemblyName("GenericEmitExample1");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            // An assembly is made up of executable modules. For a single-
            // module assembly, the module name and file name are the same 
            // as the assembly name. 
            //
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            // Get type objects for the base class trivial interfaces to
            // be used as constraints.
            //
            Type baseType = typeof(ExampleBase);
            Type interfaceA = typeof(IExampleA);
            Type interfaceB = typeof(IExampleB);

            // Define the sample type.
            //
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Sample", TypeAttributes.Public);

            Console.WriteLine("Type 'Sample' is generic: {0}", typeBuilder.IsGenericType);

            // Define type parameters for the type. Until you do this, 
            // the type is not generic, as the preceding and following 
            // WriteLine statements show. The type parameter names are
            // specified as an array of strings. To make the code
            // easier to read, each GenericTypeParameterBuilder is placed
            // in a variable with the same name as the type parameter.
            // 
            string[] typeParamNames = { "TFirst", "TSecond" };
            GenericTypeParameterBuilder[] typeParams = typeBuilder.DefineGenericParameters(typeParamNames);

            GenericTypeParameterBuilder TFirst = typeParams[0];
            GenericTypeParameterBuilder TSecond = typeParams[1];

            Console.WriteLine("Type 'Sample' is generic: {0}", typeBuilder.IsGenericType);

            // Apply constraints to the type parameters.
            //
            // A type that is substituted for the first parameter, TFirst,
            // must be a reference type and must have a parameterless
            // constructor.
            TFirst.SetGenericParameterAttributes(GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint);

            // A type that is substituted for the second type
            // parameter must implement IExampleA and IExampleB, and
            // inherit from the trivial test class ExampleBase. The
            // interface constraints are specified as an array 
            // containing the interface types.
            TSecond.SetBaseTypeConstraint(baseType);
            Type[] interfaceTypes = { interfaceA, interfaceB };
            TSecond.SetInterfaceConstraints(interfaceTypes);

            // The following code adds a private field named ExampleField,
            // of type TFirst.
            FieldBuilder exField = typeBuilder.DefineField("ExampleField", TFirst, FieldAttributes.Private);

            // Define a static method that takes an array of TFirst and 
            // returns a List<TFirst> containing all the elements of 
            // the array. To define this method it is necessary to create
            // the type List<TFirst> by calling MakeGenericType on the
            // generic type definition, List<T>. (The T is omitted with
            // the typeof operator when you get the generic type 
            // definition.) The parameter type is created by using the
            // MakeArrayType method. 
            //
            Type listOf = typeof(List<>);
            Type listOfTFirst = listOf.MakeGenericType(TFirst);
            Type[] mParamTypes = { TFirst.MakeArrayType() };

            MethodBuilder exMethod = typeBuilder.DefineMethod("ExampleMethod", MethodAttributes.Public | MethodAttributes.Static, listOfTFirst, mParamTypes);

            // Emit the method body. 
            // The method body consists of just three opcodes, to load 
            // the input array onto the execution stack, to call the 
            // List<TFirst> constructor that takes IEnumerable<TFirst>,
            // which does all the work of putting the input elements into
            // the list, and to return, leaving the list on the stack. The
            // hard work is getting the constructor.
            // 
            // The GetConstructor method is not supported on a 
            // GenericTypeParameterBuilder, so it is not possible to get 
            // the constructor of List<TFirst> directly. There are two
            // steps, first getting the constructor of List<T> and then
            // calling a method that converts it to the corresponding 
            // constructor of List<TFirst>.
            //
            // The constructor needed here is the one that takes an
            // IEnumerable<T>. Note, however, that this is not the 
            // generic type definition of IEnumerable<T>; instead, the
            // T from List<T> must be substituted for the T of 
            // IEnumerable<T>. (This seems confusing only because both
            // types have type parameters named T. That is why this example
            // uses the somewhat silly names TFirst and TSecond.) To get
            // the type of the constructor argument, take the generic
            // type definition IEnumerable<T> (expressed as 
            // IEnumerable<> when you use the typeof operator) and 
            // call MakeGenericType with the first generic type parameter
            // of List<T>. The constructor argument list must be passed
            // as an array, with just one argument in this case.
            // 
            // Now it is possible to get the constructor of List<T>,
            // using GetConstructor on the generic type definition. To get
            // the constructor of List<TFirst>, pass List<TFirst> and
            // the constructor from List<T> to the static
            // TypeBuilder.GetConstructor method.
            //
            ILGenerator ilgen = exMethod.GetILGenerator();

            Type ienumOf = typeof(IEnumerable<>);
            Type TfromListOf = listOf.GetGenericArguments()[0];
            Type ienumOfT = ienumOf.MakeGenericType(TfromListOf);
            Type[] ctorArgs = { ienumOfT };

            ConstructorInfo ctorPrep = listOf.GetConstructor(ctorArgs);
            ConstructorInfo ctor = TypeBuilder.GetConstructor(listOfTFirst, ctorPrep);

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, ctor);
            ilgen.Emit(OpCodes.Ret);

            // Create the type and save the assembly. 
            Type finished = typeBuilder.CreateType();
            //myAssembly.Save(myAsmName.Name + ".dll");

            // Invoke the method.
            // ExampleMethod is not generic, but the type it belongs to is
            // generic, so in order to get a MethodInfo that can be invoked
            // it is necessary to create a constructed type. The Example 
            // class satisfies the constraints on TFirst, because it is a 
            // reference type and has a default constructor. In order to
            // have a class that satisfies the constraints on TSecond, 
            // this code example defines the ExampleDerived type. These
            // two types are passed to MakeGenericMethod to create the
            // constructed type.
            //
            Type[] typeArgs = { typeof(GenericTypeBuilder), typeof(ExampleDerived) };
            Type constructed = finished.MakeGenericType(typeArgs);
            MethodInfo mi = constructed.GetMethod("ExampleMethod");

            // Create an array of Example objects, as input to the generic
            // method. This array must be passed as the only element of an 
            // array of arguments. The first argument of Invoke is 
            // null, because ExampleMethod is static. Display the count
            // on the resulting List<Example>.
            // 
            GenericTypeBuilder[] input = { new GenericTypeBuilder(), new GenericTypeBuilder() };
            object[] arguments = { input };

            List<GenericTypeBuilder> listX = (List<GenericTypeBuilder>)mi.Invoke(null, arguments);

            Console.WriteLine("\nThere are {0} elements in the List<Example>.", listX.Count);

            DisplayGenericParameters(finished);
        }

        private static void DisplayGenericParameters(Type t)
        {
            if (!t.IsGenericType)
            {
                Console.WriteLine("Type '{0}' is not generic.");
                return;
            }
            if (!t.IsGenericTypeDefinition)
            {
                t = t.GetGenericTypeDefinition();
            }

            Type[] typeParameters = t.GetGenericArguments();
            Console.WriteLine("\nListing {0} type parameters for type '{1}'.", typeParameters.Length, t);

            foreach (Type tParam in typeParameters)
            {
                Console.WriteLine("\r\nType parameter {0}:", tParam.ToString());

                foreach (Type c in tParam.GetGenericParameterConstraints())
                {
                    if (c.IsInterface)
                    {
                        Console.WriteLine("    Interface constraint: {0}", c);
                    }
                    else
                    {
                        Console.WriteLine("    Base type constraint: {0}", c);
                    }
                }

                ListConstraintAttributes(tParam);
            }
        }

        // List the constraint flags. The GenericParameterAttributes
        // enumeration contains two sets of attributes, variance and
        // constraints. For this example, only constraints are used.
        //
        private static void ListConstraintAttributes(Type t)
        {
            // Mask off the constraint flags. 
            GenericParameterAttributes constraints = t.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

            if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != GenericParameterAttributes.None)
            {
                Console.WriteLine("    ReferenceTypeConstraint");
            }

            if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None)
            {
                Console.WriteLine("    NotNullableValueTypeConstraint");
            }

            if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != GenericParameterAttributes.None)
            {
                Console.WriteLine("    DefaultConstructorConstraint");
            }
        }
    }

    /* This code example produces the following output:

    Type 'Sample' is generic: False
    Type 'Sample' is generic: True

    There are 2 elements in the List<Example>.

    Listing 2 type parameters for type 'Sample[TFirst,TSecond]'.

    Type parameter TFirst:
        ReferenceTypeConstraint
        DefaultConstructorConstraint

    Type parameter TSecond:
        Interface constraint: IExampleA
        Interface constraint: IExampleB
        Base type constraint: ExampleBase
     */
}