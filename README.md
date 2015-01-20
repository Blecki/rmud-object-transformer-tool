# rmud-object-transformer-tool

This tool transforms RMUD mud object source files from terse form to valid C#.

An example of the transformations it can make:
	"continue;"" becomes "return PerformResult.Continue;"

	Check can take? when item == this do { SendMessage(actor, "It's way too heavy!"); disallow; };
		becomes
	Check<MudObject, MudObject>("can take?")
		.When((actor, item) => item == this)
		.Do((actor, item) => {
				SendMessage(actor, "It's way too heavy!");
				return CheckResult.Disallow;
			});

This tool can be used as a library (As by the github database in RMUD) or as a custom tool in visual studio.

Visual Studio must be run as an administrator to build this tool. Upon building, the tool is automatically installed in visual studio. After building for the first time, you will need to run devenv /setup to complete installation. Devenv is (usually) at C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe, if you are running Visual Studio 12.0 (2013, Community Edition 2013).

After installation, Visual Studio will automatically attach this tool to any file given the extension '.mud'. You can also attach it manually in the file's properties, the correct name of the tool is '.mud'.

This project is also a good example of how to get a custom tool working with Visual Studio Community Edition. It's tough. Go ahead and google for 'visual studio compile time file transformation'. Did you find any results that weren't about t4 text templates? I didn't either! t4 templates are fantastic, but they can't do what I wanted to accomplish with this. 

Lots of googling later eventually lead me to www.codeproject.com/Articles/31257/Custom-Tools-Explained. Most of the integration with visual studio is taken directly from that article. There are, however, a few noteworthy issues I had to deal with to get it to actually work, and my solutions might be helpful for someone else.
	
A) To get the Interop assemblies, so you can reference them, you need to install the visual studio SDK.

B) To register your custom tool for COM, which Visual Studio will do for you, you first need to register the Interop dll for com, which Visual Studio will not do for you. Google for the text of the error message Visual Studio gives you the first time you try to build. Lots of other people have had this problem.

C) The article mentions a simple way to get visual studio to automatically apply the tool to files with a specific extension. The article makes one simple mistake. In the line 'key.SetValue("", ....);' the article places the description of the tool. Instead, duplicate the extension you want the tool associated with.

	using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
              GetKeyName(CSharpCategoryGuid, ".mud")))
            {
                key.SetValue("", ".mud");

Visual Studio seems to use the key name (the first .mud) to tie the extension to the tool, and then populates the 'custom tool' property of the file with the unnamed value. Which it will then use to try and find the tool later.

D) You need to add the dll to the global assembly cache after it's built. To do this use gacutil, which comes with .net. Chances are, you've got a version from .net 1.0. This version won't work. You need the version that came with .net 4.0. It's only available as part of one of the windows SDKs. You need the exact right version of the SDK. Unfortunately, you probably can't install it. Installation will fail if you've got too new of a version of Visual Studio installed, or you're running a 64bit processor, or any number of reasons. There are lots of workarounds out there on the google, but none of them worked for me. I recommend you don't waste your time, because gacutil is supposed to only be for development work. Everything else should use an installer program. This installer program works by calling System.EnterpriseServices.Internal.Publish.GacInstall. Yes, there is a function in the framework to do it. Seriously. You need to add a reference to System.EnterpriseServices to get at it. I've included a simple tool called GacFucker to get the job done for us. Here is it's entire source.

    namespace gacfucker
	{
    	class Program
    	{
        	static void Main(string[] args)
        	{
            	var publish = new System.EnterpriseServices.Internal.Publish();
            	publish.GacInstall(args[0]);
        	}
    	}
	}

This is run as a post-build step.

E) After you install it once, you can't build it again with any other copy of visual studio running. That's because visual studio loads the dll from the GAC at startup, so any attempt to stick it in the GAC after that will fail. It will still build and install if you only have a single (admin) instance of visual studio running.

F) If you just build it, you shouldn't need to run devenv /setup again. You only need to run this if you change what's being put in the registry.

G) Moving the project, or deleting it, or the dll, or the bin directory, will break it. Even though it's been stuffed into the GAC, it's been registered for COM in it's original location. Building it again should fix it.

H) I have no idea how to make it show up in the assembly list in visual studio's add reference dialog. Presumably it should be there, because it's in the GAC, but it's not. It might need to be a 'strongly named' assembly, but I haven't bothered since my use case involves packing the dll along, not requiring users to jump through all these hoops.
