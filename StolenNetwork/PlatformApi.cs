/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StolenNetwork
{
	// Based on https://github.com/grpc/grpc/blob/master/src/csharp/Grpc.Core/

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

		public static bool IsWindows => _isWindows;

        public static bool IsLinux => _isLinux;

        public static bool IsMacOSX => _isOSX;

		public static bool IsMono => _isMono;

        public static bool IsUnity => _unityApplicationPlatform != null;

        public static bool IsUnityIOS => _unityApplicationPlatform == kUnity_IPhone_Player;

        public static bool IsXamarin => _isXamarin;

        public static bool IsXamarinIOS => _isXamarinIOS;

        public static bool IsXamarinAndroid => _isXamarinAndroid;

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

			var unityAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == kUnityEngine_AssemblyName);
			var applicationClass = unityAssembly?.GetType(kUnityEngine_Application_ClassName);
            var platformProperty = applicationClass?.GetTypeInfo().GetProperty("platform", BindingFlags.Static | BindingFlags.Public);
           
			try
            {

                return platformProperty?.GetValue(null)?.ToString();
            }
            catch (TargetInvocationException)
            {
				return null;
            }
        }

        #endregion
    }
}
