function EasyExecute_CodeFile(%func,%base,%name,%ext)
{
	if(%name $= "clip")
	{
		%clipboard = getClipboard();
		//write it to a file so we can use our functions
		%out = new FileObject();
		%success = %out.openForWrite("config/client/EasyExecute/temp.cs");
		if(%success && %clipboard !$= "")
		{	
			%out.writeLine(%clipboard);
			%file = "config/client/EasyExecute/temp.cs";
		}
		%out.close();
		%out.delete();
	}
	else if(%base $= "")
	{
		Warn("Easy Execute: No path set");
		return "";
	}

	if(%name $= "")
	{
		if($EX::Server)
		{
			%name = "/server";
		}
		else if($EX::Client)
		{
			%name = "/client";
		}
	}

	if(%file $= "")
	{	
		%upper = strupr(%name);
		%lower = strlwr(%name);
		%file = findFirstFile(%base @ "*" @ %name @ "." @ %ext);
	}

	if(%file $= "")
	{
		%file = findFirstFile(%base @ "*" @ %upper @ "." @ %ext);
	}

	if(%file $= "")
	{
		%file = findFirstFile(%base @ "*" @ %lower @ "." @ %ext);
	}

	if(!isFile(%file))
	{
		Warn("Easy Execute: No file found with name" SPC %name);
		return false;
	}

	return call(%func,%file) TAB %file;
}

function EXPath(%addonName)
{
	%upper = strupr(%addonName);
	%lower = strlwr(%addonName);
	%path = findFirstFile("add-ons/" @ %addonName @ "*/description.txt");

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/" @ %upper @ "*/description.txt");
	}

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/" @ %lower  @ "*/description.txt");
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

function EX(%name)
{
	EasyExecute_CodeFile("exec",$EX::Path,%name,"cs");
	return "";
}

function EXLua(%name)
{
	EasyExecute_CodeFile("luaexec",$EX::Path,%name,"lua");
	return "";
}

function EXComp(%name)
{
	%result = EasyExecute_CodeFile("compile",$EX::Path,%name,"cs");
	if(getField(%result,0))
	{
		echo("Easy Execute: Compile successful" SPC getField(%result,1));
	}
	return "";
}

function UploadExec(%file) {
	if(!isFile(%file))
		return;
	if(!compile(%file))
	{
		//exec(%file);	
		return;
	}

	%fileObject = new fileObject();
	%fileObject.openForRead(%file);

	while(!%fileObject.isEoF()) {
		//replace fixes issues involving incorrect escape character unpacking
		 %line = %fileObject.readLine();
		 %line = executionFix(filePath(%file),%line);

		 if(%line $= "")
			  continue;
		 commandToServer('messageSent', "\\\\" @ %line);
	}
	commandToServer('messageSent', "\\\\echo(\"Upload Easy Execute: Executed " @ %file @ "\");");
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

		%endLine = strPos(%line,";",%closed);
		%endFunc = strPos(%line,")",%closed);
		if(%endLine < %endFunc || %endFunc == -1 || !isFile(%checkFile))
		{
			return %line;
		}

		%line = getSubStr(%line,0,%start) @ executionFix(%filePath,getSubStr(%line,%endLine + 1,strLen(%line) - %closed));

		schedule(33,0,"UploadExec",%checkFile);

		return %line;
	}

	return %line;
}

function EXUp(%name)
{
	EasyExecute_CodeFile("UploadExec",$EX::Path,%name,"cs");
	return "";
}

function EXConsole()
{
	UploadExec("Add-ons/script_easyExecute/console.cs");
	schedule(100,0,"CommandToServer",'ConsoleAddClient');
}

function clientcmdLoggerPushLine(%Line1,%line2,%line3,%line4,%line5)
{
    for(%I=1;%I<=5;%I++)
        if(%line[%I] !$= "")
            echo("LOG: "@ %line[%I]);
}