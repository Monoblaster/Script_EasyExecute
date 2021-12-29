function UploadExec(%file) {
	if (%file $= "")
		%file = "Add-ons/Client_MultilineUpload/upload.cs";
	if(!isFile(%file))
		return;
	if(!compile(%file))
	{
		//exec(%file);	
		return;
	}

	%fileObject = new fileObject();
	%fileObject.openForRead(%file);

	commandToServer('messageSent', "\\Executing_" @ %file);

	while(!%fileObject.isEoF()) {
		 %line = %fileObject.readLine();

		 %line = executionFix(filePath(%file),%line);

		 if(%line $= "")
			  continue;
		
		 commandToServer('messageSent', "\\\\" @ %line);
	}
	commandToServer('messageSent', "\\\\");
	%fileObject.close();
	%fileObject.delete();
}

function executionFix(%filePath,%line)
{
	if((%start = strPos(%line,"exec(")) != -1)
	{	
		//4 for the offset
		%open = %start + 5;
		//do we have a closed parathesis?
		if((%closed = strPos(%line,"\"",%open + 1)) == -1)
		{
			return %line;
		}

		%checkFile = getSubStr(%line,%open + 1,%closed - %open - 1);

		//so we doin't catch the . in .cs
		%checkFile = strReplace(%checkFile,"./",%filePath @ "/");

		if((%endLine = strPos(%line,";",%closed)) == -1)
		{
			return %line;
		}

		%line = getSubStr(%line,0,%start) @ executionFix(%filePath,getSubStr(%line,%endLine + 1,strLen(%line) - %closed));

		schedule(33,0,"UploadExec",%checkFile);

		return %line;
	}

	return %line;
}

function UploadEXPath(%addonName)
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
		Warn("Upload Easy Execute: No add-on found with the name" SPC %addonName);
	}
	else
	{
		$UploadEX::Path = filePath(%path);
		$UploadEX::Server = isFile($UploadEX::Path @ "/server.cs");
		$UploadEX::Client = isFile($UploadEX::Path @ "/client.cs");
	}
	return "";
}

function UploadEX(%name)
{
	if($UploadEX::Path $= "")
	{
		Warn("Upload Easy Execute: No path set");
		return "";
	}

	if(%name $= "")
	{
		if($UploadEX::Server)
		{
			%name = "server";
		}
		else if($UploadEX::Client)
		{
			%name = "client";
		}
	}

	%upper = strupr(%name);
	%lower = strlwr(%name);
	%file = findFirstFile($UploadEX::Path @ "/*" @ %name @ ".cs");

	if(%file $= "")
	{
		%file = findFirstFile($UploadEX::Path @ "/*" @ %upper @ ".cs");
	}

	if(%file $= "")
	{
		%file = findFirstFile($UploadEX::Path @ "/*" @ %lower @ ".cs");
	}

	if(isFile(%file) && fileExt(%file) $= ".cs")
		UploadExec(%file);
	else
		Warn("Upload Easy Execute: No file found name" SPC %name);
		
	return "";
}