function EXPath(%addonName)
{
	%path = findFirstFile("add-ons/*" @ %addonName @ "/server.cs");
	if(%path $= "")
	{
		echo("Easy Execute: No add-on found with the name" SPC %addonName);
	}
	else
	{
		$EX:Path = filePath(%path);
		echo("Easy Execute: Add-on found with path" SPC $EX:Path);
	}
	return "";
}

function EX(%name)
{
	%file = findFirstFile($EX:Path @ "/*" @ %name @ ".cs");	
	if(isFile(%file) && fileExt(%file) $= ".cs")
		exec(%file);
	else
		echo("Easy Execute: No file found name" SPC %name);
		
	return "";
}