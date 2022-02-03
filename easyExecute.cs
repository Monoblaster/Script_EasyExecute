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

function EXClip(%name)
{
	eval(getClipboard());
				
	return "";
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