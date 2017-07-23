![Logo](favicon.ico)
# Puppy.Web
> Project Created by [**Top Nguyen**](http://topnguyen.net)

# Setup Tag Helper

Go to _ViewImports.cshtml then add `@addTagHelper "*, Puppy.Web"`

## Markdown Tag Helper
Write text in html with tag `<markdown>` or `<p markdown>`

```markup
<markdown>

```csharp
	public void Main()
	{
		Console.WriteLine("Markdown by Top Nguyen");
	}
```.

</markdown>

@{
    ViewData["MarkdownContent"] = "*Italic*, **bold**, and `monospace`";
}

<markdown asp-for="@ViewData["MarkdownContent"]" />
```

It will transform to
```markup
<pre><code class='language-cs'>
	public void Main()
	{
	  Console.WriteLine("Markdown by Top Nguyen");
	}
</code></pre>

<p><em>Italic</em>, <strong>bold</strong>, and <code>monospace</code></p>
```

You can use a library like [HighlightJS](http://highlightjs.org) to sytnax highlight that code block!

```markup
@section Styles
{
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/styles/default.min.css">
}
@section Scripts{
    <script src="//cdnjs.cloudflare.com/ajax/libs/highlight.js/9.12.0/highlight.min.js"></script>
    <script>hljs.initHighlightingOnLoad();</script>
}
```