namespace CsharpToHtml;

//todo: should be customizable, read from app settings.
public class ClassTable
{
    public static string Header => @"<pre class=""source"" title="""">
<code>";

    public static string Footer => @"</code></pre>
";

    public static string? ClassToColor(string? @class) => @class switch
    {
        "keyword" => "0000FF",
        "control" => "8F08C4",
        "method" => "74531F",
        "type" => "2B91AF",
        "string" => "A31515",
        "variable" => "1F377F",
        "comment" => "008000",
        "excluded" => "686868",
        "preprocess" => "686868",
        _ => null,
    };

    public static string? TypeToClass(string classificationType) => classificationType switch
    {
        "keyword" => "reserved",

        "keyword - control" => "control",

        "method name"
        or "extension method name"
        => "method",

        "class name"
        or "enum name"
        or "interface name"
        or "record class name"
        or "delegate name"
        => "type",

        "struct name"
        or "record struct name"
        => "type struct",

        "type parameter name" => "type param",

        "string" or "string - verbatim" => "string",

        "local name" => "variable",
        "parameter name" => "variable local",

        "comment"
        or "xml doc comment - attribute name"
        or "xml doc comment - attribute quotes"
        or "xml doc comment - delimiter"
        or "xml doc comment - name"
        or "xml doc comment - text"
        => "comment",

        "excluded code" => "excluded",
        "preprocessor keyword" => "preprocess",

        "number" => "number",
        "operator" => "operator",
        "constant name" => "constant",
        "field name" => "field",
        "property name" => "property",

        "identifier" => null,
        "namespace name" => null,
        "punctuation" => null,

        _ => null,
    };
}
