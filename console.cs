function ConsoleLoggerTick(%fileObject)
{
    if(!isObject($ConsoleLoggerObject) && !isObject(ConsoleLoggerObject))
    {
        $consoleLoggerCount++;
        $ConsoleLoggerObject = new ConsoleLogger(ConsoleLoggerObject, "config/client/logConsole2.out", false);
        ConsoleLoggerObject.level = "normal";
    }
    cancel($ConsoleLoggerTick);
    $ConsoleLoggerObject.detach();

    if(!isObject(%fileObject))
        %fileObject = new FileObject(ConsoleLoggerFile);

    if(!isObject(%g = consoleGroup))
        %g = new SimSet(consoleGroup);

    %cg = %g.getCount();
    %gg = %fileObject.openForRead("config/client/logConsole2.out");
    while(!%fileObject.isEOF())
    {
        for(%I = 0; %I < $ConsoleLoggerCount; %I++)
            if(!%fileObject.isEOF())
                %a = %fileObject.readLine();

        for(%I = 0; %I < %cg; %I++)
            commandToClient(%g.getObject(%I),'LoggerPushLine',getSubStr(%a,0,255),getSubStr(%a,255,255),getSubStr(%a,510,255));
    }
    %fileObject.close();

    $ConsoleLoggerObject.attach();
    $ConsoleLoggerTick = schedule(1000,0,ConsoleLoggerTick,%fileObject);
}
if(!isEventPending($ConsoleLoggerTick))
    ConsoleLoggerTick();

function serverCmdConsoleAddClient(%client)
{
	consoleGroup.add(%client);
}