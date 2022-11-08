function EXPath(%addonName)
{
	%upper = strupr(%addonName);
	%lower = strlwr(%addonName);
	%path = findFirstFile("add-ons/*" @ %addonName @ "/description.txt");

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/*" @ %upper @ "/description.txt");
	}

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/*" @ %lower  @ "/description.txt");
	}

	if(%path $= "")
	{
		Warn("Easy Execute: No add-on found with the name" SPC %addonName);
	}
	else
	{
		$EX::Path = filePath(%path);
		$EX::Server = isFile($EX::Path @ "/server.cs");
		$EX::Client = isFile($EX::Path @ "/client.cs");
	}
	return "";
}

function EXComp(%name)
{
	if($EX::Path $= "")
	{
		Warn("Easy Execute: No path set");
		return "";
	}

	if(%name $= "")
	{
		if($EX::Server)
		{
			%name = "server";
		}
		else if($EX::Client)
		{
			%name = "client";
		}
	}

	%upper = strupr(%name);
	%lower = strlwr(%name);
	%file = findFirstFile($EX::Path @ "/*" @ %name @ ".cs");

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %upper @ ".cs");
	}

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %lower @ ".cs");
	}

	if(isFile(%file) && fileExt(%file) $= ".cs")
		%success = compile(%file);

		if(%success)
		{
			echo("Easy Execute: Compile successful" SPC %file);
		}
	else
		Warn("Easy Execute: No file found name" SPC %name);
		
	return "";
}

function EXCComp(%name)
{
	%clipboard = getClipboard();
	//write it to a file so we can use our functions
	%out = new FileObject();
	%success = %out.openForWrite("config/client/EasyExecute/uploadTemp.cs");
	if(%success)
	{	
		%out.writeLine(%clipboard);
	}
	
	%out.close();
	%out.delete();

	%success = compile("config/client/EasyExecute/uploadTemp.cs");

	if(%success)
	{
		echo("Easy Execute: Compile successful" SPC %file);
	}
		
	return "";
}

function EXC()
{
	eval(getClipboard());
}

function EX(%name)
{
	if($EX::Path $= "")
	{
		Warn("Easy Execute: No path set");
		return "";
	}

	if(%name $= "")
	{
		if($EX::Server)
		{
			%name = "server";
		}
		else if($EX::Client)
		{
			%name = "client";
		}
	}

	%upper = strupr(%name);
	%lower = strlwr(%name);
	%file = findFirstFile($EX::Path @ "/*" @ %name @ ".cs");

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %upper @ ".cs");
	}

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %lower @ ".cs");
	}

	if(isFile(%file) && fileExt(%file) $= ".cs")
		exec(%file);
	else
		Warn("Easy Execute: No file found name" SPC %name);
		
	return "";
}

function EXL(%name)
{
	if($EX::Path $= "")
	{
		Warn("Easy Execute: No path set");
		return "";
	}

	if(%name $= "")
	{
		if($EX::Server)
		{
			%name = "server";
		}
		else if($EX::Client)
		{
			%name = "client";
		}
	}

	%upper = strupr(%name);
	%lower = strlwr(%name);
	%file = findFirstFile($EX::Path @ "/*" @ %name @ ".lua");

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %upper @ ".lua");
	}

	if(%file $= "")
	{
		%file = findFirstFile($EX::Path @ "/*" @ %lower @ ".lua");
	}

	if(isFile(%file) && fileExt(%file) $= ".lua")
		luaexec(%file);
	else
		Warn("Easy Execute: No file found name" SPC %name);
		
	return "";
}