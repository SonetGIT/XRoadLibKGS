using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Calculator
{
    // Declare a generic delegate that can be used to execute the 
    // finished method.
    //
    public delegate TOut D<TIn, TOut>(TIn[] input);

    public class GenericMethodBuilder
    {
        // This method shows how to declare, in Visual Basic, the generic
        // method this program emits. The method has two type parameters,
        // TInput and TOutput, the second of which must be a reference type
        // (class), must have a parameterless constructor (new()), and must
        // implement ICollection<TInput>. This interface constraint
        // ensures that ICollection<TInput>.Add can be used to add
        // elements to the TOutput object the method creates. The method 
        // has one formal parameter, input, which is an array of TInput. 
        // The elements of this array are copied to the new TOutput.
        //
        public static TOutput Factory<TInput, TOutput>(TInput[] tarray)
            where TOutput : class, ICollection<TInput>, new()
        {
            TOutput ret = new TOutput();
            ICollection<TInput> ic = ret;

            foreach (TInput t in tarray)
            {
                ic.Add(t);
            }
            return ret;
        }

        public static void Execute()
        {
            // The following shows the usage syntax of the C#
            // version of the generic method emitted by this program.
            // Note that the generic parameters must be specified 
            // explicitly, because the compiler does not have enough 
            // context to infer the type of TOutput. In this case, TOutput
            // is a generic List containing strings.
            // 
            int[] arr = { 1, 2, 3, 4, 5 };
            List<int> list1 = Factory<int, List<int>>(arr);
            Console.WriteLine("The first element is: {0}", list1[0]);


            // Creating a dynamic assembly requires an AssemblyName
            // object, and the current application domain.
            //
            AssemblyName asmName = new AssemblyName("MethodBuilder1");
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyBuilder demoAssembly = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);

            // Define the module that contains the code. For an 
            // assembly with one module, the module name is the 
            // assembly name plus a file extension.
            ModuleBuilder demoModule = demoAssembly.DefineDynamicModule(asmName.Name);

            // Define a type to contain the method.
            TypeBuilder demoType = demoModule.DefineType("MyClass", TypeAttributes.Public);

            // Define a public static method with standard calling
            // conventions. Do not specify the parameter types or the
            // return type, because type parameters will be used for 
            // those types, and the type parameters have not been
            // defined yet.
            //
            MethodBuilder methodBuilder = demoType.DefineMethod("Factory", MethodAttributes.Public | MethodAttributes.Static);

            // Defining generic type parameters for the method makes it a
            // generic method. To make the code easier to read, each
            // type parameter is copied to a variable of the same name.
            //
            string[] typeParameterNames = { "TInput", "TOutput" };
            GenericTypeParameterBuilder[] typeParameters = methodBuilder.DefineGenericParameters(typeParameterNames);

            GenericTypeParameterBuilder TInput = typeParameters[0];
            GenericTypeParameterBuilder TOutput = typeParameters[1];

            // Add special constraints.
            // The type parameter TOutput is constrained to be a reference
            // type, and to have a parameterless constructor. This ensures
            // that the Factory method can create the collection type.
            // 
            TOutput.SetGenericParameterAttributes( GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint);

            // Add interface and base type constraints.
            // The type parameter TOutput is constrained to types that
            // implement the ICollection<T> interface, to ensure that
            // they have an Add method that can be used to add elements.
            //
            // To create the constraint, first use MakeGenericType to bind 
            // the type parameter TInput to the ICollection<T> interface,
            // returning the type ICollection<TInput>, then pass
            // the newly created type to the SetInterfaceConstraints
            // method. The constraints must be passed as an array, even if
            // there is only one interface.
            //
            Type icoll = typeof(ICollection<>);
            Type icollOfTInput = icoll.MakeGenericType(TInput);
            Type[] constraints = { icollOfTInput };
            TOutput.SetInterfaceConstraints(constraints);

            // Set parameter types for the method. The method takes
            // one parameter, an array of type TInput.
            Type[] parms = { TInput.MakeArrayType() };
            methodBuilder.SetParameters(parms);

            // Set the return type for the method. The return type is
            // the generic type parameter TOutput.
            methodBuilder.SetReturnType(TOutput);

            // Generate a code body for the method. 
            // -----------------------------------
            // Get a code generator and declare local variables and
            // labels. Save the input array to a local variable.
            //
            ILGenerator ilgen = methodBuilder.GetILGenerator();

            LocalBuilder retVal = ilgen.DeclareLocal(TOutput);
            LocalBuilder ic = ilgen.DeclareLocal(icollOfTInput);
            LocalBuilder input = ilgen.DeclareLocal(TInput.MakeArrayType());
            LocalBuilder index = ilgen.DeclareLocal(typeof(int));

            Label enterLoop = ilgen.DefineLabel();
            Label loopAgain = ilgen.DefineLabel();

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Stloc_S, input);

            // Create an instance of TOutput, using the generic method 
            // overload of the Activator.CreateInstance method. 
            // Using this overload requires the specified type to have
            // a parameterless constructor, which is the reason for adding 
            // that constraint to TOutput. Create the constructed generic
            // method by passing TOutput to MakeGenericMethod. After
            // emitting code to call the method, emit code to store the
            // new TOutput in a local variable. 
            MethodInfo createInst = typeof(Activator).GetMethod("CreateInstance", Type.EmptyTypes);
            MethodInfo createInstOfTOutput = createInst.MakeGenericMethod(TOutput);

            ilgen.Emit(OpCodes.Call, createInstOfTOutput);
            ilgen.Emit(OpCodes.Stloc_S, retVal);

            // Load the reference to the TOutput object, cast it to
            // ICollection<TInput>, and save it.
            //
            ilgen.Emit(OpCodes.Ldloc_S, retVal);
            ilgen.Emit(OpCodes.Box, TOutput);
            ilgen.Emit(OpCodes.Castclass, icollOfTInput);
            ilgen.Emit(OpCodes.Stloc_S, ic);

            // Loop through the array, adding each element to the new
            // instance of TOutput. Note that in order to get a MethodInfo
            // for ICollection<TInput>.Add, it is necessary to first 
            // get the Add method for the generic type defintion,
            // ICollection<T>.Add. This is because it is not possible
            // to call GetMethod on icollOfTInput. The static overload of
            // TypeBuilder.GetMethod produces the correct MethodInfo for
            // the constructed type.
            //
            MethodInfo mAddPrep = icoll.GetMethod("Add");
            MethodInfo mAdd = TypeBuilder.GetMethod(icollOfTInput, mAddPrep);

            // Initialize the count and enter the loop.
            ilgen.Emit(OpCodes.Ldc_I4_0);
            ilgen.Emit(OpCodes.Stloc_S, index);
            ilgen.Emit(OpCodes.Br_S, enterLoop);

            // Mark the beginning of the loop. Push the ICollection
            // reference on the stack, so it will be in position for the
            // call to Add. Then push the array and the index on the 
            // stack, get the array element, and call Add (represented
            // by the MethodInfo mAdd) to add it to the collection.
            //
            // The other ten instructions just increment the index
            // and test for the end of the loop. Note the MarkLabel
            // method, which sets the point in the code where the 
            // loop is entered. (See the earlier Br_S to enterLoop.)
            //
            ilgen.MarkLabel(loopAgain);

            ilgen.Emit(OpCodes.Ldloc_S, ic);
            ilgen.Emit(OpCodes.Ldloc_S, input);
            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldelem, TInput);
            ilgen.Emit(OpCodes.Callvirt, mAdd);

            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldc_I4_1);
            ilgen.Emit(OpCodes.Add);
            ilgen.Emit(OpCodes.Stloc_S, index);

            ilgen.MarkLabel(enterLoop);
            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldloc_S, input);
            ilgen.Emit(OpCodes.Ldlen);
            ilgen.Emit(OpCodes.Conv_I4);
            ilgen.Emit(OpCodes.Clt);
            ilgen.Emit(OpCodes.Brtrue_S, loopAgain);

            ilgen.Emit(OpCodes.Ldloc_S, retVal);
            ilgen.Emit(OpCodes.Ret);

            // Complete the type.
            Type dt = demoType.CreateType();
            // Save the assembly, so it can be examined with Ildasm.exe.
            //demoAssembly.Save(asmName.Name + ".dll");

            // To create a constructed generic method that can be
            // executed, first call the GetMethod method on the completed 
            // type to get the generic method definition. Call MakeGenericType
            // on the generic method definition to obtain the constructed
            // method, passing in the type arguments. In this case, the
            // constructed method has string for TInput and List<string>
            // for TOutput. 
            //
            Console.WriteLine("ClassName: {0}", dt);
            MethodInfo m = dt.GetMethod("Factory");
            MethodInfo bound = m.MakeGenericMethod(typeof(int), typeof(List<int>));

            // Display a string representing the bound method.
            Console.WriteLine("MethodName: {0}", bound);


            // Once the generic method is constructed, 
            // you can invoke it and pass in an array of objects 
            // representing the arguments. In this case, there is only
            // one element in that array, the argument 'arr'.
            //
            object o = bound.Invoke(null, new object[] { arr });
            var list2 = (List<int>)o;

            Console.WriteLine("The first element is: {0}", list2[0]);


            // You can get better performance from multiple calls if
            // you bind the constructed method to a delegate. The 
            // following code uses the generic delegate D defined 
            // earlier.
            //
            Type dType = typeof(D<int, List<int>>);
            D<int, List<int>> test;
            test = (D<int, List<int>>) Delegate.CreateDelegate(dType, bound);

            List<int> list3 = test(arr);
            Console.WriteLine("The first element is: {0}", list3[0]);
        }
    }

    /* This code example produces the following output:

    The first element is: a
    System.Collections.Generic.List`1[System.String] Factory[String,List`1](System.String[])
    The first element is: a
    The first element is: a
     */
}