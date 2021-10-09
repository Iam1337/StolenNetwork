/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StolenNetwork
{
    internal static class PlatformApi
    {
        #region Private Vars

		private const string kUnityEngine_AssemblyName = "UnityEngine";

		private const string kUnityEngine_Application_ClassName = "UnityEngine.Application";

		const string kUnity_IPhone_Player = "IPhonePlayer";

        const string kXamarin_Android_Object_ClassName = "Java.Lang.Object, Mono.Android";

		const string kXamarin_IOS_Object_ClassName = "Foundation.NSObject, Xamarin.iOS";

        private static readonly bool _isWindows;

		private static readonly bool _isLinux;

		private static readonly bool _isOSX;

		private static readonly bool _isNetCore;

		private static readonly bool _isNet5OrHigher;

		private static readonly string _clrVersion;

		private static readonly bool _isMono;

		private static readonly string _unityApplicationPlatform;

        private static readonly bool _isXamarin;

		private static readonly bool _isXamarinIOS;

		private static readonly bool _isXamarinAndroid;

		private static readonly Architecture _architecture;

        #endregion

        #region Public Vars

        public static bool IsLinux => _isLinux;

        public static bool IsMacOSX => _isOSX;

        public static bool IsWindows => _isWindows;

        public static bool IsMono => _isMono;

        /// <summary>
        /// true if running on Unity platform.
        /// </summary>
        public static bool IsUnity => _unityApplicationPlatform != null;

        /// <summary>
        /// true if running on Unity iOS, false otherwise.
        /// </summary>
        public static bool IsUnityIOS => _unityApplicationPlatform == kUnity_IPhone_Player;

        /// <summary>
        /// true if running on a Xamarin platform (either Xamarin.Android or Xamarin.iOS),
        /// false otherwise.
        /// </summary>
        public static bool IsXamarin => _isXamarin;

        /// <summary>
        /// true if running on Xamarin.iOS, false otherwise.
        /// </summary>
        public static bool IsXamarinIOS => _isXamarinIOS;

        /// <summary>
        /// true if running on Xamarin.Android, false otherwise.
        /// </summary>
        public static bool IsXamarinAndroid => _isXamarinAndroid;

        /// <summary>
        /// true if running on .NET 5+, false otherwise.
        /// </summary>
        public static bool IsNet5OrHigher => _isNet5OrHigher;


        /// <summary>
        /// Contains the version of common language runtime obtained from <c>Environment.Version</c>
        /// if the property is available on current TFM. <c>null</c> otherwise.
        /// </summary>
        public static string ClrVersion => _clrVersion;

        /// <summary>
        /// true if running on .NET Core (CoreCLR) or NET 5+, false otherwise.
        /// </summary>
        public static bool IsNetCore => _isNetCore;

        public static bool Is64Bit => IntPtr.Size == 8;

		public static Architecture Architecture => _architecture;

        #endregion

        #region Constructor

        static PlatformApi()
		{
			_isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			_isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			_isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

			_isNet5OrHigher = Environment.Version.Major >= 5;
			_isNetCore = _isNet5OrHigher || RuntimeInformation.FrameworkDescription.StartsWith(".NET Core");
			_clrVersion = Environment.Version.ToString();

			_isMono = Type.GetType("Mono.Runtime") != null;
			_unityApplicationPlatform = TryGetUnityApplicationPlatform();

			_isXamarinIOS = Type.GetType(kXamarin_IOS_Object_ClassName) != null;
			_isXamarinAndroid = Type.GetType(kXamarin_Android_Object_ClassName) != null;
			_isXamarin = _isXamarinIOS || _isXamarinAndroid;

			_architecture = RuntimeInformation.ProcessArchitecture;
		}

        #endregion

        #region Private Methods

        static string TryGetUnityApplicationPlatform()
        {

			Assembly unityAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == kUnityEngine_AssemblyName);
			var applicationClass = unityAssembly?.GetType(kUnityEngine_Application_ClassName);
            var platformProperty = applicationClass?.GetTypeInfo().GetProperty("platform", BindingFlags.Static | BindingFlags.Public);
           
			try
            {
                // Consult value of Application.platform via reflection
                // https://docs.unity3d.com/ScriptReference/Application-platform.html
                return platformProperty?.GetValue(null)?.ToString();
            }
            catch (TargetInvocationException)
            {
                // The getter for Application.platform is defined as "extern", so if UnityEngine assembly is loaded outside of a Unity application,
                // the definition for the getter will be missing - note that this is a sneaky trick that helps us tell a real Unity application from a non-unity
                // application which just happens to have loaded the UnityEngine.dll assembly.
                // https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Runtime/Export/Application/Application.bindings.cs#L375
                // See https://github.com/grpc/grpc/issues/23334

                // If TargetInvocationException was thrown, it most likely means that the method definition for the extern method is missing,
                // and we are going to interpret this as "not running on Unity".
                return null;
            }
        }

        #endregion
    }
}
