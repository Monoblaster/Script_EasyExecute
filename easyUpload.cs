package EventGames
{
	function collectEventGameBrickParameters(%game,%name)
    {
        %strippedName = stripEventGameParameters(%name);
		%strippedParameters = strReplace(stripEventGameName(%name),"APOS","\t");
        %count = getFieldCount(%strippedParameters);
        for(%i = 0; %i < %count; %i++)
        {
		talk(getField(%strippedParameters,%i));
            %game.PT[%strippedName,%i] = getField(%stripEventGameParameters,%i);
        }
        
    }
	function stripEventGameParameters(%name)
    {
		%end = strPos(%name,"APOS") - 1;
		
		if(%end == -1)
		{
			%end = strLen(%name);
		}

        return getSubStr(%name,1, %end);
    }
	function stripEventGameName(%name)
    {
		%start = strPos(%name,"APOS") + 4;
		
		if(%start == 3)
		{
			%start = strLen(%name);
		}

        return getSubStr(%name,%start,strLen(%name) - %start);
    }
};
deactivatePackage("EventGames");
activatePackage("EventGames");