package testertest // test functions must be inside of a package named the same as the file
{
    //create test functions with a incrmemnting number
    //they will be run in this order
    function Test1() // can suceed
    {
        return true; //when a test suceeds return true
    }

    function Test2() // can fail
    {
        return false; //when a test fails ruturn false
    }

    function Test3() // created objects will be destroyed when the function ends
    {
        $test = new ScriptObject();
        // %result = true;  
        // %test.delete(); UNECESSARY because any objects created will be destroyed when the function ends
        // return %result;
        return true;
    }

    function Test4() // does our test object still exist?
    {
        return !IsObject($test);
    }
};
// you can define anything else you want in this file and it will be run when the test runs