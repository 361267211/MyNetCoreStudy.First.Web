using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace NetCoreStudy.First.Utility
{
    public class EmitDemo
    {
        public void CreateEmitClass()
        {
            AssemblyName aName = new AssemblyName("ChefDynamicAssembly");
            AssemblyBuilder ab =
                AssemblyBuilder.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.Run);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name + ".dll");
            TypeBuilder tb = mb.DefineType("Commander");
            var attrs = MethodAttributes.Public;
            // 使用类型构建器创建一个方法构建器
            MethodBuilder methodBuilder = tb.DefineMethod("Do", attrs, typeof(string), Type.EmptyTypes);
            // 通过方法构建器获取一个MSIL生成器
            var IL = methodBuilder.GetILGenerator();
            // 开始编写方法的执行逻辑
            // var store = new string[3];
            var store = IL.DeclareLocal(typeof(string[]));
            IL.Emit(OpCodes.Ldc_I4, 3);
            IL.Emit(OpCodes.Newarr, typeof(string));
            IL.Emit(OpCodes.Stloc, store);
            //store[0] = "C"
            IL.Emit(OpCodes.Ldloc, store);
            IL.Emit(OpCodes.Ldc_I4, 0);
            IL.Emit(OpCodes.Ldstr, "C");
            IL.Emit(OpCodes.Stelem, typeof(string));
            //store[1] = "JAVA"
            IL.Emit(OpCodes.Ldloc, store);
            IL.Emit(OpCodes.Ldc_I4, 1);
            IL.Emit(OpCodes.Ldstr, "JAVA");
            IL.Emit(OpCodes.Stelem, typeof(string));
            //store[2] = "Python"
            IL.Emit(OpCodes.Ldloc, store);
            IL.Emit(OpCodes.Ldc_I4, 2);
            IL.Emit(OpCodes.Ldstr, "Python");
            IL.Emit(OpCodes.Stelem, typeof(string));
            // IChef chef = new GoodChef();
            var chef = IL.DeclareLocal(typeof(IChef));
            IL.Emit(OpCodes.Newobj, typeof(StoreChef).GetConstructor(Type.EmptyTypes));
            IL.Emit(OpCodes.Stloc, chef);
            //var dish = chef.Cook(vegetables);
            var dish = IL.DeclareLocal(typeof(string));
            IL.Emit(OpCodes.Ldloc, chef);
            IL.Emit(OpCodes.Ldloc, store);
            IL.Emit(OpCodes.Callvirt, typeof(IChef).GetMethod("Cook"));
            IL.Emit(OpCodes.Stloc, dish);
            // return dish;
            IL.Emit(OpCodes.Ldloc, dish);
            IL.Emit(OpCodes.Ret);
            //方法结束
            // 从类型构建器中创建出类型
            var dynamicType = tb.CreateType();
            // 通过反射创建出动态类型的实例
            var commander = Activator.CreateInstance(dynamicType);
            Console.WriteLine(dynamicType.GetMethod("Do").Invoke(commander, null).ToString());
       
        }


    }

    public interface IChef
    {
        string Cook(string[] store);
    }
    public class StoreChef : IChef
    {
        public string Cook(string[] store)
        {
            return "Value:" + string.Join("+", store);
        }
    }
}
