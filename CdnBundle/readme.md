#CdnBundle

### History (One tag to rule them all?)
How do you bundle multiple JavaScript or CSS CDN into a single file? ASP.NET doesn't let you do it because it's _counter-intuitive_. Is it? Google's Web Performance Tests show that the fewer script references you have, the better for your page. So how about having just one script reference, huh?

![Beautiful things come in little packages](http://static1.squarespace.com/static/51c1d4f5e4b053b1a67a3219/t/51c31a0fe4b0fd9e08f9687c/1469413897881/?format=1000w)

### Prerequisites
You'd need a an IDE to edit .NET projects, such as Visual Studio 2010 or a more revent version.

### Installation
Add the CdnBundle.dll Reference to your .NET project, and enter import the namespace as follows:

```cs
using CdnBundle; //csharp
```

```vbnet
Import CdnBundle `vb.net
```

### How it works
Each Bundle contains a Cdn URL, Local Path URL, a type _(CSS / JavaScript)_, and a boolean indicating whether to minify the resultant script or not. 

When the Cdn URL is specified for a bundle, it gets the resource from the CDN server and if a local path is specified, it saves to the local file. This cycle is only repeated after 24 hours, or when the web application is restarted.

```cs
List<Bundle> jsBundles = new List<Bundle>();
jsBundles.Add(new Bundle("https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.0/jquery.min.js", @"~/jquery.min.js", Bundle.BundleType.JavaScript, false));
jsBundles.Add(new Bundle("https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.0/jquery-ui.min.js", @"~/jquery-ui.min.js", Bundle.BundleType.JavaScript, false));
jsBundles.Add(new Bundle(@"~/my-local-script.js", Bundle.BundleType.JavaScript, true));
```

The above code creates a list of bundles, which you can add to your page using the following code:

```cs
@jsBundles.Load();
```

Using the load function without any parameters will render a script tag with the entire scripts inlined in order. E.g.

```html
<script>
---jquery body here---
---jquery-ui body here---
---my-local-script body here---
</script>
```
You can specify a local file url for the bundled resulting script to be saved. If you do, a script tag with a reference to the local file resource will be rendered instead. E.g.

```cs
@jsBundles.Load("~/scripts/all-my-scripts.js");
```

will give:

```html
<script src="[relative path]/all-my-scripts.js"></script>
```

#### For CSS Bundling

```cs
List<Bundle> cssBundles = new List<Bundle>();
cssBundles.Add(new Bundle("https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.0/jquery-ui.min.css", @"~/jquery.ui.css", Bundle.BundleType.CSS, false));
cssBundles.Add(new Bundle(@"~/css/my-local-style.css", Bundle.BundleType.CSS, true));
```

```cs
@cssBundles.Load();
//or
@cssBundles.Load("@/css/all-my-styles.css");
```

### Dependencies

- Microsoft Ajax Minifier - _For Minification_

### Acknowledgments

A huge hat tip to Google Nigeria and the Google Developer Community for organising the Progressive Web Apps conference in Lagos, Nigeria, during which we were made to understand that simplicity is the new bae.

Thanks, Deji for not throwing this idea out the window too quickly.

### Contributions

You're absolutely free to fork this repo and make pull requests. I'd love to see this get better. Also, star this repo if it helps you, or you think it'll help someone, or you think it doesn't suck, or just star it. **shameless solicitation ends**

Follow me on twitter [@mykeels](https://twitter.com/mykeels) and [instagram](https://instagram.com/mykeels). I'd enjoy hearing what you think, really. Try me. (okay, i really need to get a grip, and more coffee too).
