using wey.Console;
using wey.Style;

Console.WriteLine(
        Style.Colorize(
                text: "Hello",
                color: Color.Dark_Green
            )
    );

Choice c = new(new String[] { "Hi", "hello", "1", "2", "3", "4" });
var b = c.Start();

Console.WriteLine(b.Name);

Console.Clear();

new Choice(new String[] { "1", "b", "c"}).Start();