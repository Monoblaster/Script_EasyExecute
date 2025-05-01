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
	%path = findFirstFile("add-ons/" @ %addonName @ "/description.txt");

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/" @ %upper @ "/description.txt");
	}

	if(%path $= "")
	{
		%path = findFirstFile("add-ons/" @ %lower  @ "/description.txt");
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
	return getField(EasyExecute_CodeFile("exec",$EX::Path,%name,"cs"),0);
}

$EasyExecute::Number = "0123456789";
$EasyExecute::Dash = "-";
$EasyExecute::WhiteSpace = " \n\t";
function EasyExecute_TestString(%teststr,%testcount)
{
	if(%teststr $= "")
	{
		%teststr = "-";
	}

	%teststrlen = trim(strLen(%teststr)); //tokenizer
	%tokenType = "Whitespace";
	%lastTokenType = "Whitespace";
	for(%i = 0; %i < %teststrlen; %i++)
	{
		%char = getSubStr(%teststr,%i,1);
		if(strPos($EasyExecute::WhiteSpace,%char) != -1)//is whitespace
		{
			%charType = "Whitespace";
		}
		else if(strPos($EasyExecute::Number,%char) != -1)//is a number
		{
			%charType = "Number";
		}
		else if(strPos($EasyExecute::Dash,%char) != -1)//is a dash
		{
			%charType = "Dash";
		}
		else // invalid char error
		{
			warn();
			error("Character cannot be used" NL getSubStr(%teststr,0,%i-1)@"##"@getSubStr(%teststr,%i,1)@"##"@getSubStr(%teststr,%i+1,%teststrlen-%i));
			return;
		}

		if(%tokenType !$= "Whitespace" && (// do we push the current token to the stack?
			%charType $= "Whitespace"||
			%tokenType !$= "Number" && %charType $= "Number"||
			%charType $= "Dash"
		))
		{
			%lastTokenType = %tokenType;
			%tokenTypeStack = %tokenTypeStack SPC %tokenType;
			%tokenStack = %tokenStack SPC %token;
			%token = "";
			%tokenType = "";
		}

		%tokenType = %charType;
		if(
			%charType $= "Whitespace"
		)
		{
			
			continue;
		}
		%token = %token @ %char;
	}

	if(%tokenType !$= "Whitespace")
	{
		%lastTokenType = %tokenType;
		%tokenTypeStack = %tokenTypeStack SPC %tokenType;
		%tokenStack = %tokenStack SPC %token;
		%token = "";
		%tokenType = ""; //push any remaining tokens
	}
	%tokenStack = 1 SPC lTrim(%tokenStack) SPC %testcount;
	%tokenTypeStack = "Number" SPC lTrim(%tokenTypeStack) SPC "Number";
	
	%tokenCount = getWordCount(%tokenStack);
	for(%i = 1; %i < %tokenCount-1; %i++) //lazy and i don't feel like doing a propper algorithm for this
	{
		%currToken = getWord(%tokenStack,%i);
		%currTokenType = getWord(%tokenTypeStack,%i);
		
		if(%currTokenType $= "Dash")
		{

			if(getWord(%tokenTypeStack,%i-1) !$= "Number" || getWord(%tokenTypeStack,%i+1) !$= "Number")
			{
				Warn("Non number next to dash");
				return "";
			}

			%a = getWord(%tokenStack,%i-1);
			%b = getWord(%tokenStack,%i+1);
			if(%a > %b)
			{
				Warn("The number left of dash must be smaller than the right");
				return "";
			}

			if(%i == 1)
			{
				%a--;
			}

			if(%i == %tokenCount-2)
			{
				%b++;
			}

			for(%j = %a+1; %j < %b; %j++)
			{
				%testsToDo = %testsToDo SPC %j;
			}
		}

		if(%currTokenType $= "Number")
		{
			%testsToDo = %testsToDo SPC getWord(%tokenStack,%i);
		}
	}

	return lTrim(%testsToDo);
}

function EasyExecute_AddTestOutput(%string)
{
	$EasyExecute::OutputLog = $EasyExecute::OutputLog NL %string;
}

function EasyExecute_EmptyTestOutput()
{
	%output = $EasyExecute::OutputLog;
	%count = getRecordCount(%output);
	for(%i = 0; %i < %count; %i++)
	{
		echo(getRecord(%output,%i));
	}
	$EasyExecute::OutputLog = "";
}

$EasyExecute::PackageStack = "";
$EasyExecute::OutputLog = "";
function EXTest(%name,%teststr)
{
    %result = EasyExecute_CodeFile("compile",$EX::Path,%name,"cs");
	if(!getField(%result,0))
	{
		return false;
	}
	
	EasyExecute_CodeFile("exec",$EX::Path,%name,"cs");
	
	%file = getField(%result,1);

    %packagename = fileBase(%file);
	if(!isPackage(%packagename))
	{
		Warn("Easy Execute: No package with the same name as the file. Should be named" SPC %packagename);
		return false;
	}

	EasyExecute_AddTestOutput("Easy Execute: Starting test \"" @ %packagename @"\"");
	%packages = $EasyExecute::PackageStack;
	%count = getWordCount(%packages);
	for(%i = 0; %i < %count; %i++)
	{
		deactivatePackage(getWord(%packages,%i));
	}	

	activatePackage(%packagename);

	%testCount = 0;
    while(isFunction("Test"@(%testCount+1)))
    {
		%testCount++;
	}

	deactivatePackage(%packagename);
	$EasyExecute::PackageStack = lTrim($EasyExecute::PackageStack SPC %packagename);

	%packages = $EasyExecute::PackageStack;
	%count = getWordCount(%packages);
	for(%i = 0; %i < %count; %i++)
	{
		activatePackage(getWord(%packages,%i));
	}

	%testsToDo = EasyExecute_TestString(%teststr,%testcount);
	%testCount = getWordCount(%testsToDo);
	for(%i = 0; %i < %testCount; %i++)
	{
		%testFunc = "Test"@getWord(%testsToDo,%i);
		%startId = new ScriptObject().getId(); //where to start to delete objects
        %result = call(%testfunc);
		%endId = new ScriptObject().getId(); //where to finish deleting objects
		if(%startId > %endID) //sanity
		{
			EasyExecute_AddTestOutput("Easy Execute: Object ID tracking failed. Restarting game suggested");
			return false;
		}

		for(%j = %startId; %j <= %endId; %j++)
		{
			if(!isObject(%j))
			{
				continue;
			}
			%j.delete();
		}

        if(!%result)
        {
            EasyExecute_AddTestOutput(%packagename SPC %testfunc@": failed");
            %failures = %failures SPC %testfunc;
        }
        else
        {
            EasyExecute_AddTestOutput(%packagename SPC %testfunc@": success");
        }
	}
    %failures = ltrim(%failures);
	deactivatePackage(%packagename);

	$EasyExecute::PackageStack = removeWord($EasyExecute::PackageStack,getWordCount($EasyExecute::PackageStack)-1);

    if(%failures !$= "")
    {
        EasyExecute_AddTestOutput("Easy Execute: \"" @%packagename @ "\" failed  on "@ %failures @ "\n");
		if($EasyExecute::PackageStack $= "")
		{
			EasyExecute_EmptyTestOutput();
		}
		return false;
    }

    EasyExecute_AddTestOutput("Easy Execute: \"" @ %packagename @ "\" was successful!\n");
	if($EasyExecute::PackageStack $= "")
	{
		EasyExecute_EmptyTestOutput();
	}
	return true;
}

function EXLua(%name)
{
	return getField(EasyExecute_CodeFile("luaexec",$EX::Path,%name,"lua"),0);
}

function EXComp(%name)
{
	%result = EasyExecute_CodeFile("compile",$EX::Path,%name,"cs");
	if(getField(%result,0))
	{
		echo("Easy Execute: Compile successful" SPC getField(%result,1));
	}
	return getField(%result,0);
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
	return getField(EasyExecute_CodeFile("UploadExec",$EX::Path,%name,"cs"),0);
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