using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MSLA.Server.Utilities
{
    /// <summary>This class can be used to call the most common reflection methods</summary>
    //[System.Diagnostics.DebuggerStepThrough()]
    public class ReflectionHelper
    {
        /// <summary>This creates an instance of the object from the assembly</summary>
        /// <param name="DocAssembly">The name of the assembly</param>s
        /// <param name="DocNamespace">The Namespace of the class</param>
        /// <param name="DocObject">The name of the Class</param>
        /// <param name="Args">The arguments required by the new constructor</param>
        /// <returns>Returns an instance of the class</returns>
        public static object CreateObject(string DocAssembly, string DocNamespace, string DocObject, object[] Args)
        {
            Assembly asm = null;
            foreach (Assembly asmLoaded in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asmLoaded.FullName == DocAssembly)
                {
                    asm = asmLoaded;
                    break;
                }
            }
            if (asm == null)
            { asm = Assembly.Load(DocAssembly); }

            // create an instance of the business object
            Object Instance=null;
            System.Type aType;
            try
            {
                BindingFlags BdFlags = BindingFlags.CreateInstance | BindingFlags.NonPublic |
                                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                                BindingFlags.Instance;

                aType = asm.GetType(DocNamespace + "." + DocObject);
                Instance = aType.InvokeMember(null, BdFlags, null, null, Args, null, null, null);
            }
            catch (Exception Ex)
            {
                string Desc = "Failed to create an instance of '" + DocNamespace + "." + DocObject + "'\r\n from the Assembly '" + DocAssembly + "'.";
                Exceptions.ServiceExceptionHandler.HandleException(new Security.SimpleUserInfo(), Ex, Desc);
                throw new ReflectionHelperException("Failed to create an instance of '" + DocNamespace + "." + DocObject + "'\r\n from the Assembly '" + DocAssembly + "'.", Ex);
            }
                return Instance;
        }

        /// <summary>This creates an instance of the object of the Type Defined</summary>
        /// <param name="aType">A System.Type of the Object to be created.</param>
        /// <param name="Args">The arguments required by the new constructor</param>
        /// <returns>Returns an instance of the class</returns>
        public static object CreateObject(System.Type aType, object[] Args)
        {
            object Instance = null;
            try
            {
                BindingFlags BdFlags = BindingFlags.CreateInstance | BindingFlags.NonPublic |
                                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                                BindingFlags.Instance;

                Instance = aType.InvokeMember(null, BdFlags, null, null, Args, null, null, null);
            }
            catch (Exception Ex)
            {
                string Desc = "Failed to create an instance of '" + aType.ToString() + "'.";
                Exceptions.ServiceExceptionHandler.HandleException(new Security.SimpleUserInfo(), Ex, Desc);

                //throw new ReflectionHelperException("Failed to create an instance of '" + aType.ToString() + "'.", Ex);
            }
            return Instance;
        }


        /// <summary>This creates an instance of the object from the assembly</summary>
        /// <param name="DocAssembly">The name of the assembly</param>
        /// <param name="DocNamespace">The Namespace of the class</param>
        /// <param name="DocObject">The name of the Class</param>
        /// <param name="MethodName">The Static/Shared method name</param>
        /// <param name="Args">The arguments required by the new constructor</param>
        /// <returns>Returns an instance of the class</returns>
        public static object CallStaticMethod(string DocAssembly, string DocNamespace, string DocObject, string MethodName, params object[] Args)
        {
            Assembly asm = null;
            foreach (Assembly asmLoaded in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asmLoaded.FullName == DocAssembly)
                {
                    asm = asmLoaded;
                    break;
                }
            }
            if (asm == null)
            { asm = Assembly.Load(DocAssembly); }

            System.Type aType = null;

            aType = asm.GetType(DocNamespace + "." + DocObject);
            if (aType == null)
            { throw new ReflectionHelperException("'" + DocNamespace + "." + DocObject + "' not found in assembly '" + DocAssembly + "'.", null); }
            if (!HasSharedMethod(aType, MethodName, Args))
            { throw new ReflectionHelperException(MethodName + " (Static) not found in'" + DocNamespace + "." + DocObject + "'\r\n Assembly '" + DocAssembly + "'.", null); }
            MethodInfo info = GetMethod(aType, MethodName, Args);
            Object Result = info.Invoke(new Object(), Args);
            return Result;

        }

        /// <summary>Calls the particular method on the class</summary>
        /// <param name="obj">Instance of a Class/Object</param>
        /// <param name="method">Name of the method</param>
        /// <param name="Args">The Parameters required by the method</param>
        /// <returns>Returns the result of the method or returns void/null/nothing</returns>
        public static object CallMethod(object obj, string method, params object[] Args)
        {
            // call a private method on the object
            MethodInfo info = GetMethod(obj.GetType(), method, Args);
            if (info == null)
            {
                throw new ReflectionHelperException(method + " not found in object " + obj.GetType().ToString() + ".", null);
            }
            Object result = info.Invoke(obj, Args);
            return result;
        }

        private static MethodInfo GetMethod(Type ObjectType, String method, params object[] Args)
        {
            if (Args != null)
            {
                Type[] ParamTypes = new Type[Args.Length];
                Type ParamType;
                int i = 0;
                foreach (object arg in Args)
                {
                    ParamTypes[i] = arg.GetType();
                    i += 1;
                }
                return ObjectType.GetMethod(method, BindingFlags.FlattenHierarchy |
                                                  BindingFlags.Static |
                                                  BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic, null, ParamTypes, null);

            }
            else
            {
                return ObjectType.GetMethod(method, BindingFlags.FlattenHierarchy |
                                                  BindingFlags.Static |
                                                  BindingFlags.Instance |
                                                  BindingFlags.Public |
                                                  BindingFlags.NonPublic);
            }
        }


        /// <summary>Calls the handler method on the class event</summary>
        /// <param name="ctrlObj">Instance of a Class/Object/Control of which event is to be handled</param>
        /// <param name="eventName">Name of the event to be handled</param>
        /// <param name="objSO">Instance of a Class/Object from which handler method is to be called</param>
        /// <param name="handlerMethodName">Name of the Handler Method which will get fired on specified event</param>
        /// <param name="Args">The Parameters required by the handler method</param>
        public static void AddHandlerMethodForEvent(object ctrlObj, string eventName, object objSO, string handlerMethodName, params object[] Args)
        {
            Type ctrlObjType = ctrlObj.GetType();
            Type ObjSOType = objSO.GetType();

            EventInfo evntInfo = ctrlObjType.GetEvent(eventName);
            Type typDelegate = evntInfo.EventHandlerType;

            Type[] ParameterTypeArray = new Type[Args.Length];
            Int64 ArgCnt = 0;
            foreach (Object argument in Args)
            {
                ParameterTypeArray[ArgCnt] = argument.GetType();
                ArgCnt += 1;
            }

            MethodInfo miHandler = ObjSOType.GetMethod(handlerMethodName, ParameterTypeArray);
            if (miHandler == null)
            {
                throw new ReflectionHelperException(handlerMethodName + " not found in object " + ObjSOType.ToString() + ".", null);
            }
            Delegate delgt = Delegate.CreateDelegate(typDelegate, objSO, miHandler);

            MethodInfo miAddHandler = evntInfo.GetAddMethod();
            Object[] AddHandlerArgs = { delgt };
            miAddHandler.Invoke(ctrlObj, AddHandlerArgs);

        }

        /// <summary>This can be used to verify whether the class has the requested static/shared method</summary>
        /// <param name="ObjectType">Instance of a class/object</param>
        /// <param name="Method">Name of the method</param>
        /// <returns>Returns True if the method exists, else false</returns>
        public static bool HasSharedMethod(Type ObjectType, String Method, params Object[] Args)
        {
            if (Args != null)
            {
                Type[] ParamTypes = new Type[Args.Length];
                Type ParamType;
                int i = 0;
                foreach (object arg in Args)
                {
                    ParamTypes[i] = arg.GetType();
                    i += 1;
                }

                MethodInfo MInfo = ObjectType.GetMethod(Method, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public, null, ParamTypes, null);
                if (MInfo == null)
                { return false; }
                else
                { return true; }
            }
            else
            {
                MethodInfo MInfo = ObjectType.GetMethod(Method, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
                if (MInfo == null)
                { return false; }
                else
                { return true; }
            }
        }


        /// <summary>This can be used to verify whether the calss has the requested method</summary>
        /// <param name="ObjectType">Instance of a class/object</param>
        /// <param name="Method">Name of the method</param>
        /// <param name="Args">Parameters/Arguments required by the method</param>
        /// <returns>Returns True if the method exists, else false</returns>
        public static bool HasMethod(Type ObjectType, String Method, Type[] Args)
        {
            MethodInfo MInfo = ObjectType.GetMethod(Method, BindingFlags.Public |
                                                            BindingFlags.Static |
                                                            BindingFlags.Instance, null, Args, null);
            if (MInfo == null)
            { return false; }
            else
            { return true; }
        }

        /// <summary>This method can be used to fetch the value of a property. Does not raise an exception if property is not found.</summary>
        /// <param name="Obj">The instance of the class/object</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns>Returns the property value. If Property is not found, returns Nothing</returns>
        public static Object GetPropertyValueSafe(Object Obj, String PropertyName)
        {
            PropertyInfo myProperty = Obj.GetType().GetProperty(PropertyName);
            if (myProperty == null)
            {
                return null;

            }
            return myProperty.GetValue(Obj, null);
        }
        /// <summary>This method can be used to fetch the value of a property</summary>
        /// <param name="Obj">The instance of the class/object</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns>Returns the property value.</returns>
        public static Object GetPropertyValue(Object Obj, String PropertyName)
        {
            PropertyInfo myProperty = Obj.GetType().GetProperty(PropertyName);
            if (myProperty == null)
            {
                throw new ReflectionHelperException("Property '" + PropertyName + "' not found in object " + Obj.GetType().ToString() + ".", null);
            }
            return myProperty.GetValue(Obj, null);
        }  
        
        /// <summary>This method can be used to fetch the type of a property. Does not raise an exception if property is not found.</summary>
        /// <param name="Obj">The instance of the class/object</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns>Returns the property type. If Property is not found, returns Nothing</returns>
        public static object[] GetPropertyAttributesSafe(Object Obj, String PropertyName)
        {
            PropertyInfo myProperty = Obj.GetType().GetProperty(PropertyName);
            if (myProperty == null)
            {
                return null;
            }
            return myProperty.GetCustomAttributes(true);
        }

        /// <summary>This method can be used to set a value to a property</summary>
        /// <param name="Obj">The instance of the Class/Object</param>
        /// <param name="PropertyName">Name of the Property</param>
        /// <param name="PropertyValue">The Value to be set into the property</param>
        public static bool SetPropertyValueSafe(Object Obj, String PropertyName, Object PropertyValue)
        {
            BindingFlags Bindings = BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo myProperty = Obj.GetType().GetProperty(PropertyName);
            if (myProperty == null)
            {
                return false;
            }
            else if (!myProperty.CanWrite)
            {
                return false;
            }
            TypeCode myTypeCode = Type.GetTypeCode(myProperty.PropertyType);
            if (myTypeCode != TypeCode.Object && PropertyValue != null)
            { myProperty.SetValue(Obj, Convert.ChangeType(PropertyValue, myTypeCode), Bindings, null, null, null); }
            else
            { myProperty.SetValue(Obj, PropertyValue, Bindings, null, null, null); }
            return true;
        }

        /// <summary>This method can be used to set a value to a property</summary>
        /// <param name="Obj">The instance of the Class/Object</param>
        /// <param name="PropertyName">Name of the Property</param>
        /// <param name="PropertyValue">The Value to be set into the property</param>
        public static void SetPropertyValue(Object Obj, String PropertyName, Object PropertyValue)
        {
            BindingFlags Bindings = BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo myProperty = Obj.GetType().GetProperty(PropertyName);

            if (myProperty == null)
            {
                throw new ReflectionHelperException("Property '" + PropertyName + "' not found in object " + Obj.GetType().ToString() + ".", null);
            }
            else if (!myProperty.CanWrite)
            {
                throw new ReflectionHelperException("Property '" + PropertyName + "' is readonly in object " + Obj.GetType().ToString() + ". Set the Databinding ReadOnly option to True to avoid this error.", null);
            }
            TypeCode myTypeCode = Type.GetTypeCode(myProperty.PropertyType);
            if (myTypeCode != TypeCode.Object && PropertyValue != null)
            { myProperty.SetValue(Obj, Convert.ChangeType(PropertyValue, myTypeCode), Bindings, null, null, null); }
            else
            { myProperty.SetValue(Obj, PropertyValue, Bindings, null, null, null); }
        }

        /// <summary>Returns whether the proper version of .Net Framework is installed along with required service packs.</summary>
        /// <returns></returns>
        public static bool IsDotNetFrameworkVersionInvalid(out string MismatchAssembly)
        {
            //OS Version


            // This is the only line of code that is to be changed when .Net Service Pack is released.
            string mscorlib_ReqVer = "2.0.50727.3053";
            string System_Windows_Forms_ReqVer = "2.0.50727.3053";
            string System_Data_ReqVer = "2.0.50727.3053";

            // This is the logic to validate Product Version
            string ProdVersion;
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                ProdVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion;
                switch (asm.GetName().Name)
                {
                    case "mscorlib":
                        if (!matchVersion(ProdVersion, mscorlib_ReqVer))
                        {
                            MismatchAssembly = "Incorrect Assembly Loaded: " + asm.GetName().Name + "\r\nCurrent Version: " + ProdVersion
                                                + "\r\nRequired Version: " + mscorlib_ReqVer;
                            return true;
                        }
                        break;
                    case "System.Windows.Forms":
                        if (!matchVersion(ProdVersion, System_Windows_Forms_ReqVer))
                        {
                            MismatchAssembly = "Incorrect Assembly Loaded: " + asm.GetName().Name + "\r\nCurrent Version: " + ProdVersion
                                                + "\r\nRequired Version: " + System_Windows_Forms_ReqVer;
                            return true;
                        }
                        break;
                    case "System.Data":
                        if (!matchVersion(ProdVersion, System_Data_ReqVer))
                        {
                            MismatchAssembly = "Incorrect Assembly Loaded: " + asm.GetName().Name + "\r\nCurrent Version: " + ProdVersion
                                                + "\r\nRequired Version: " + System_Data_ReqVer;
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }
            MismatchAssembly = string.Empty;
            return false;
        }

        private static Boolean matchVersion(String ProdVersion, String ReqVersion)
        {
            string[] ProdVerSplit = ProdVersion.Split('.');
            string[] ReqVerSplit = ReqVersion.Split('.');

            if (ProdVerSplit.Length != 4 || ReqVerSplit.Length != 4)
                return false;

            if (Convert.ToInt64(ProdVerSplit[0]) == Convert.ToInt64(ReqVerSplit[0]))
            {
                if (Convert.ToInt64(ProdVerSplit[1]) == Convert.ToInt64(ReqVerSplit[1]))
                {
                    if (Convert.ToInt64(ProdVerSplit[2]) == Convert.ToInt64(ReqVerSplit[2]))
                    {
                        if (Convert.ToInt64(ProdVerSplit[3]) >= Convert.ToInt64(ReqVerSplit[3]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
