using Sim86;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine($"USAGE: sim86 [8086 machine code file]");
            return;
        }

        var Table = InstructionDecoder.Get8086InstructionTable();
        Console.WriteLine($"; 8086 Instruction Instruction Encoding Count: {Table.MaxInstructionByteCount}");

        var stdout = new StreamWriter(Console.OpenStandardOutput());
        stdout.AutoFlush = true;
        Console.SetOut(stdout);

        InstructionWriter.PrintInstructions(args[0], stdout);
    }

    // Declare the C function using DllImport
    //[DllImport("libinterop", CallingConvention = CallingConvention.Cdecl)]
    //public static extern int add(int a, int b);
    //public delegate int Add(int a, int b);
}

//#if OSX || Linux
//    [DllImport("libc")]
//    public static extern IntPtr dlopen(string path, int flag);
//
//    [DllImport("libc")]
//    public static extern IntPtr dlsym(IntPtr handle, string symbolName);
//
//    [DllImport("libc")]
//    public static extern int dlclose(IntPtr handle);
//
//    public static IntPtr OpenLibrary(string path)
//    {
//        IntPtr handle = dlopen(path, 0);
//        if (handle == IntPtr.Zero)
//        {
//            throw new Exception("Couldn't open native library: " + path);
//        }
//        return handle;
//    }
//
//    public static void CloseLibrary(IntPtr libraryHandle)
//    {
//        dlclose(libraryHandle);
//    }
//
//    public static T GetDelegate<T>(IntPtr libraryHandle, string functionName) where T : class
//    {
//        IntPtr symbol = dlsym(libraryHandle, functionName);
//
//        if (symbol == IntPtr.Zero)
//        {
//            throw new Exception("Couldn't get function: " + functionName);
//        }
//
//        return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
//    }
//#endif
//
//#if Windows
//    [DllImport("kernel32")]
//	public static extern IntPtr LoadLibrary(string path);
// 
//	[DllImport("kernel32")]
//	public static extern IntPtr GetProcAddress(IntPtr libraryHandle, string symbolName);
// 
//
//	[DllImport("kernel32")]
//	public static extern bool FreeLibrary(IntPtr libraryHandle);
//
//	public static IntPtr OpenLibrary(string path)
//	{
//		IntPtr handle = LoadLibrary(path);
//		if (handle == IntPtr.Zero)
//		{
//			throw new Exception("Couldn't open native library: " + path);
//		}
//		return handle;
//	}
//
//	public static void CloseLibrary(IntPtr libraryHandle)
//	{
//		FreeLibrary(libraryHandle);
//	}
//
//	public static T GetDelegate<T>(IntPtr libraryHandle, string functionName) where T : class
//	{
//		IntPtr symbol = GetProcAddress(libraryHandle, functionName);
//		if (symbol == IntPtr.Zero)
//		{
//			throw new Exception("Couldn't get function: " + functionName);
//		}
//		return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
//	}
//#endif
//
//
//
//#if OSX || Linux
//	const string LIB_PATH = "./libinterop.dylib";
//#elif Windows
//	const string LIB_PATH = "./libinterop.dll";
//#endif
//
//    static void Main(string[] args)
//    {
//
//#if Linux
//        Console.WriteLine("Built on Linux!"); 
//#elif OSX
//        Console.WriteLine("Built on macOS!");
//#elif Windows
//        Console.WriteLine("Built in Windows!"); 
//#else
//        Console.WriteLine("Unknown");
//#endif
//
//        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//        {
//            Console.WriteLine("Running on Windows");
//        }
//        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
//        {
//            Console.WriteLine("Running on Linux");
//        }
//        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))// || RuntimeInformation.IsOSPlatform(OSPlatform.Create("DARWIN")))
//        {
//            Console.WriteLine("Running on macOS");
//        }
//        else
//        {
//            Console.WriteLine("Unknown operating system");
//        }
//
//
//        Console.WriteLine("OSX: " + RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
//
//
//        var libHandle = OpenLibrary(LIB_PATH);
//        Console.WriteLine($"{libHandle:x}");
//
//        if (libHandle == IntPtr.Zero)
//        {
//            Console.WriteLine($"Failed to load {LIB_PATH}");
//        }
//        // var add = GetDelegate<Add>(libHandle, "add");
//        // Console.WriteLine($"{add:x}");
//
//        //int a = 10;
//        //int b = 20;
//        //int result = add(a, b);
//        //Console.WriteLine("{0} + {1} = {2}", a, b, result);
//
//        if (libHandle != IntPtr.Zero)
//        {
//            CloseLibrary(libHandle);
//        }
//
//        //Console.ReadLine();
//    }
//}
