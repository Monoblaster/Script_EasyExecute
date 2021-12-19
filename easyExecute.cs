function EXPath(%addonName)
{
	%upper = strupr(%addonName);
	%lower = strlwr(%addonName);
	%path = findFirstFile("add-ons/*" @ %addonName @ "/server.cs");

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/*" @ %upper @ "/server.cs");
	}

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/*" @ %lower @ "/server.cs");
	}

	if(%path $= "")
	{
		Warn("Easy Execute: No add-on found with the name" SPC %addonName);
	}
	else
	{
		$EX::Path = filePath(%path);
	}
	return "";
}

function EX(%name)
{
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