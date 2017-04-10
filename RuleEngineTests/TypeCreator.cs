using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using System.ComponentModel;

namespace RuleDLL
{
    public class MetricDashboardType
    {
        public int Key { get; set; }        

        public class TypeCreator
        {
            public static object CreateObject(Dictionary<string, Type> propDict, Dictionary<string, string> valueDict)
            {
                Type dynamicType = CreateNewType("MetricTypeName", propDict, typeof(MetricDashboardType));
                MetricDashboardType newClassBase = (MetricDashboardType)Activator.CreateInstance(dynamicType);
                dynamic newClass = newClassBase;
                foreach (var o in valueDict)
                {
                    Type propType = propDict[o.Key];
                    var property = TypeDescriptor.GetProperties(newClass)[o.Key];
                    var convertedValue = property.Converter.ConvertFrom(o.Value);
                    property.SetValue(newClass, convertedValue);
                }
                return newClass;
            }

            public static Type CreateNewType(string newTypeName, Dictionary<string, Type> dict, Type baseClassType)
            {
                bool noNewProperties = true;
                // create a dynamic assembly and module
                AssemblyBuilder assemblyBldr = Thread.GetDomain().DefineDynamicAssembly (new AssemblyName("tmpAssembly"), AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBldr = assemblyBldr.DefineDynamicModule("tmpModule");

                // create a new type builder
                TypeBuilder typeBldr = moduleBldr.DefineType(newTypeName, TypeAttributes.Public | TypeAttributes.Class, baseClassType);

                // Loop over the attributes that will be used as the
                // properties names in my new type
                string propertyName = null;
                Type propertyType = null;
                var baseClassObj = Activator.CreateInstance(baseClassType);
                foreach (var word in dict)
                {
                    propertyName = word.Key;
                    propertyType = word.Value;

                    //is it already in the base class?
                    var src_pi = baseClassObj.GetType().GetProperty(propertyName);
                    if (src_pi != null)
                    {
                        continue;
                    }

                    // Generate a private field for the property
                    FieldBuilder fldBldr = typeBldr.DefineField ("_" + propertyName, propertyType, FieldAttributes.Private);
                    // Generate a public property
                    PropertyBuilder prptyBldr = typeBldr.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new Type[] { propertyType });
                    // The property set and property get methods need the
                    // following attributes:
                    MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;
                    // Define the “get” accessor method for newly created private field.
                    MethodBuilder currGetPropMthdBldr = typeBldr.DefineMethod ("get_value", GetSetAttr, propertyType, null);

                    // Intermediate Language stuff… as per Microsoft
                    ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
                    currGetIL.Emit(OpCodes.Ldarg_0);
                    currGetIL.Emit(OpCodes.Ldfld, fldBldr);
                    currGetIL.Emit(OpCodes.Ret);

                    // Define the “set” accessor method for the newly created private field.
                    MethodBuilder currSetPropMthdBldr = typeBldr.DefineMethod("set_value", GetSetAttr, null, new Type[] { propertyType });

                    // More Intermediate Language stuff…
                    ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
                    currSetIL.Emit(OpCodes.Ldarg_0);
                    currSetIL.Emit(OpCodes.Ldarg_1);
                    currSetIL.Emit(OpCodes.Stfld, fldBldr);
                    currSetIL.Emit(OpCodes.Ret);

                    // Assign the two methods created above to the
                    // PropertyBuilder’s Set and Get
                    prptyBldr.SetGetMethod(currGetPropMthdBldr);
                    prptyBldr.SetSetMethod(currSetPropMthdBldr);
                    noNewProperties = false; //I added at least one property
                }
                if (noNewProperties)
                {
                    return baseClassType; //deliver the base class
                }
                // Generate (and deliver) my type
                return typeBldr.CreateType();
            }
        }
    }
}
