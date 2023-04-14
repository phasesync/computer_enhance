#if OSX || Linux
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
internal static class LoadLibrary
{
	[DllImport("libc", EntryPoint="dlopen")]
	private static extern nint dlopen(string path, int flag);

	[DllImport("libc", EntryPoint="dlsym")]
	private static extern nint dlsym(nint handle, string symbolName);

	[DllImport("libc", EntryPoint="dlclose")]
	private static extern int dlclose(nint handle);

	public static nint OpenLibrary(string path)
	{
		var handle = dlopen(path, 0);
		if (handle == nint.Zero)
		{
			throw new Exception("Couldn't open native library: " + path);
		}

		return handle;
	}

	public static void CloseLibrary(nint libraryHandle)
	{
		dlclose(libraryHandle);
	}

	private static ConcurrentDictionary<nint, ConcurrentDictionary<string, WeakReference>> _functionCache = new ConcurrentDictionary<nint, ConcurrentDictionary<string, WeakReference>>();

	public static T GetDelegate<T>(nint libraryHandle, string functionName) where T : class
	{
		var functions = _functionCache.GetOrAdd(libraryHandle, new ConcurrentDictionary<string, WeakReference>());

		if (functions.TryGetValue(functionName, out var funcRef) && funcRef.Target is T func)
		{
			return func as T;
		}

		var symbol = dlsym(libraryHandle, functionName);

		if (symbol == nint.Zero)
		{
			throw new Exception("Couldn't get function: " + functionName);
		}

		func = (Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T)!;
		functions[functionName] = new WeakReference(func);

		return func;
	}
}
#endif
