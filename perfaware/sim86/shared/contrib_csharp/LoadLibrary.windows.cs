#if Windows
using System.Runtime.InteropServices;

internal static class LoadLibrary
{
     	[DllImport("kernel32", EntryPoint="LoadLibrary")]
		public static extern nint LoadLibrary(string path);

		[DllImport("kernel32", EntryPoint="GetProcAddress")]
		public static extern nint GetProcAddress(nint libraryHandle, string symbolName);

		[DllImport("kernel32", EntryPoint="FreeLibrary")]
		public static extern bool FreeLibrary(nint libraryHandle);

		public static nint OpenLibrary(string path)
		{
			var handle = LoadLibrary(path);
			if (handle == nint.Zero)
			{
				throw new Exception("Couldn't open native library: " + path);
			}

			return handle;
		}

		public static void CloseLibrary(nint libraryHandle)
		{
			FreeLibrary(libraryHandle);
		}

	    private static ConcurrentDictionary<nint, ConcurrentDictionary<string, WeakReference>> _functionCache = new ConcurrentDictionary<nint, ConcurrentDictionary<string, WeakReference>>();
		
		public static T GetDelegate<T>(nint libraryHandle, string functionName) where T : class
		{
			var functions = _functionCache.GetOrAdd(libraryHandle, new ConcurrentDictionary<string, WeakReference>());

			if (functions.TryGetValue(functionName, out var funcRef) && funcRef.Target is T func)
			{
				return func as T;
			}

			var symbol = GetProcAddress(libraryHandle, functionName);
			if (symbol == nint.Zero)
			{
				throw new Exception("Couldn't get function: " + functionName);
			}

			func = (Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T)!;
			functions[functionName] = new WeakReference(func);

			return func;
		}
	}
}
#endif
