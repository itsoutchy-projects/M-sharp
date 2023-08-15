# M# source code information
The interpreter runs code by iterating over all the lines and performing checks to see what is being done there. This means:  
- You are **not** able to call a function and have the definition be afterwards
- Variables will **not** be able to be used before definition

This comes as shown in the `runCode(string filePath)` function inside the `Program.cs` file:
```cs
// We iterate over it like so:
string code = File.ReadAllText(filePath);
string[] lines = code.Split("\n"); // We split the code by a newline character, making iteration possible
int index = 0; // We use this so we can get a custom function's code
foreach (string l in lines)
{
    // "functions" is a Dictionary<string, string>, where the key is the name and the value is the code as it's written
    if (functions.ContainsKey(l))
    {
        foreach (string s in functions[l].Split("\n"))
        {
            runLine(s, index); // We expect this to run the function's code, however there is a bug preventing it from running successfully
        }
        break;
    } else
    {
        runLine(l, index); // This line simply runs the code as normal if no custom functions were found
    }
    index++;
}
```
You *could* have it scan the code first for custom functions and variables, however that would mean reworking pretty much the whole interpreter.

## Heading over to the `Function.cs` file
You can see that there are 2 overloads to the `Function` class, this is for functions which would require parameters, however:  
Since these functions may expect types which are not `object`, we face the variables dictionary being misstyped, as in the value would be an object but the function may expect a string, causing exceptions.

These functions still work as expected (when outside of a function for now) though.

## Questions
Q: How could I make this language be compiled?  
A: You'd need to write a compiler to convert the code from M# to assembly which the OS would then run.

Q: Why haven't *you* reworked the interpreter?  
A: I don't have time to do so, this project is being updated frequently, meaning I'd probably have to pause this project and start re-writing the **whole** thing.