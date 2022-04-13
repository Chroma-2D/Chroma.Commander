### What is Chroma.Commander?
Commander is a quick to set up, simple and powerful debugging console for games made using Chroma Framework.

### What are its features?
1. > Attribute-based convar registration for both fields and properties, no matter their access level. Supported property types are listed in a separate section.
   > ```C#
   > [ConsoleVariable("g_showfps", Description = "whether or not to show the FPS counter")]
   > private static bool _showFps;
   >
   > [ConsoleVariable("ai_thinkdebug", Description = "specify AI thinker logging level")]
   > internal static int AiThinkDebuggingLevel { get; set; }
   > ```

2. > Attribute-based command registration for methods matching the standard command signature. Supports default arguments.
   > ```C#
   > [ConsoleCommand("test_cmd",
   >  Description = "this is a test command, for testing!",
   >  DefaultArguments = new object[] { -1, "lorem ipsum", true })]
   > internal static void TestCommand(DebugConsole console, params ExpressionValue[] args)
   > {
   >     foreach (var arg in args)
   >     {
   >        console.Print(arg.ToConsoleStringRepresentation());
   >     }
   > }
   >```

3. > Easy initialization and wiring.
   > ```C#
   > using Chroma;
   > using Chroma.Input;
   > using Chroma.Graphics;
   > using Chroma.Commander;
   > 
   > internal class GameCore : Game
   > {
   >    private DebugConsole _console;
   >    
   >    protected override void LoadContent()
   >    {
   >        _console = new DebugConsole(this.Window);
   >        _console.RegisterStaticEntities();
   >      
   >        // If you have instance members you can pass the console object around
   >        // and do the following:
   >        //
   >        // _console.RegisterInstanceEntities(this);
   >        //
   >        // It is your responsibility to keep the object from being garbage collected. 
   >    }
   > 
   >    protected override void Update(float delta)
   >    {
   >        _console.Update(delta);
   >    }
   > 
   >    protected override void Draw(RenderContext context)
   >    {
   >        _console.Draw(context);
   >    }
   > 
   >    protected override void WheelMoved(MouseWheelEventArgs e)
   >    {
   >        _console.WheelMoved(e);
   >    }
   > 
   >    protected override void KeyPressed(KeyEventArgs e)
   >    {
   >        _console.KeyPressed(e);
   >    }
   > 
   >    protected override void TextInput(TextInputEventArgs e)
   >    {
   >        _console.TextInput(e);
   >    }
   > }
   > ```

4. > In-console expression parsing complete with variable reference support. Full syntax reference is present later down in the document.
   > ```
   > > $g_showfps
   > false
   > > !$g_showfps
   > > $g_showfps
   > true
   > > test_cmd $g_showfps
   > true
   > "lorem ipsum"
   > true
   > ```

5. > Input history with last-input caching and a scrollable window output buffer.

### Supported convar types
Commander has 3 basic expression value types: `boolean`, `string` and `number`. Depending on the type they are subject to different conversion rules as follows.  

**Numeric primitive types**  
`byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` are all supported and internally treated as a `double`.

**Strings**  
`string` is supported out-of-box, implicit conversion to numeric types is **not supported**.

**Booleans**  
`bool` is supported out-of-box and is not convertible to other types. Booleans are subject to a special toggle operator described in the syntax reference below.

**Enums**  
Enums of any underlying type are supported, it is possible to set values of enums using either a string representing an enum field name or a number. Enums are subject to special printing and inspection rules.

### Syntax reference
**Command invocation**  
Commands are invoked using direct identifiers, followed by an optional list of expressions which will be evaluated and passed to the command as a list of arguments:
```
> test_cmd 1 2 3 4 2*5 $cl_tickrate
```

**Variable referencing**  
Variables are read, referenced and assigned using `$` variable reference operator. It is used for both reads and writes.
```
> $cl_tickrate
666
> $cl_tickrate = 20
> test_cmd $cl_tickrate
> $cl_tickrate = $debug_variable * 3
```

**Boolean toggling**  
Boolean variables can be toggled using `!` toggle operator.
```
> $g_showfps
false
> !$g_showfps
> $g_showfps
true
```

**Variable inspection**  
All registered convars can be inspected using `?` inspection operator. The inspection operator provides console type, read-write permissions and - for enums - the CLR type the convar is wrapped around.
```
> ?$g_showfps
boolean | RW
> ?$gfx_vsync
number | RW | Chroma.Graphics.VerticalSyncMode
```

### Acknowledgments
#### INT10H.org
This project uses ToshibaSat Plus 8x14 TrueType font as its visual buffer representation medium. Visit https://int10h.org to learn more.
