
## LibrettoXUI

Microsoft introduced WPF in 2006. I dabbled with it briefly then but never wrote anything beyond "Hello, world." Other shiny balls got my attention and I never returned to WPF, until the other day. 

This WPF app puts a user interface over a Python command-line code generator I wrote. The generator and its command line interface work just fine. However, a point-and-click way to use it would be sure be nice. And, while I'm at it, a way to store a code generation "set" to return to later would sure be nice. That's what this little WPF app does. 

![](https://rogerpence.dev/wp-content/uploads/2022/03/LibrettoUI-2_cpsO6sRCjY.png)

I strongly suspect that despite my best efforts try to write this little app in a non-naive way, my WPF techniques remain naive and crude. But the app does do the job. 

### Interesting things 

#### INotifyPropertyChanged

Use WPF's `Binding` object to bind a value in a window's model to a given property. Use `UpdateSourceTrigger=PropertyChanged` to cause the bound value to referesh when the bound property value changes. This implicitly refreshes whenever a value is changed from the UI. However, if the value is changed programmatically there is a little more work to do. 

For example, in the button below, its Content property is bound to a the `SchemaLinkButtonLabel` property in the WPF window's model. The `Content` property should be automatically refreshed whenever the `SchemaLinkButtonLabel` property changes.

```
<Button  Content="{Binding SchemaLinkButtonLabel,
                           UpdateSourceTrigger=PropertyChanged}" 
...
</Button>                           
```

You need to implement the `INotifyPropertyChanged` interface for every property for which you want changes observed. You could do this by hand for each class that needs it, but it's easier and better to use a base class like this one. 

Add this class to your WPF project:

```
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibrettoUI_2.Model
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        protected bool SetField<T>(ref T field, T value, 
                                   [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```
and have your WPF model class derive from the ObservableObject: 

```
public class LibrettoSet : ObservableObject 
{
```

In a bound property's setter, be sure set the field value with the `SetField` method. This ensures that the `INotifyPropertyChanged's` changed event is raised when the property changes. Every time the property changes the WPF data binding engine knows the property changes. 

```
private string? _SchemaLinkButtonLabel;
public string? SchemaLinkButtonLabel
{
    get { return _SchemaLinkButtonLabel; }
    set
    {
        SetField(ref _SchemaLinkButtonLabel, value);
    }
}
```

With that wired up, when the `SchemaLinkButtonLabel` changes, the UI reflects that change. In this example, when the schema selection combobox's `SelectionEvent` fires, `SchemaaLinkButtonLabel` is changed to the proper text:

```
librettoSet.SchemaLinkButtonLabel = 
     (librettoSet.Schema.ToLower().EndsWith("*.json")) ? 
         "Open schema path" : "Open schema file";
```

It's a damn shame that observable properties aren't a little more baked into .NET. Not only does this technique require the roll-your-own `ObservableObject` class to implement `INotifyPropertyChanged`, but it also requires a full property declaration with an explicit getter and setter--no automatic properties here. 

The [PostSharp Essentials](https://www.postsharp.net/essentials) library solves the problem very gracefully in the free version of its library, but only for automatic properties, not for explicit properties or child objects. 

```
[NotifyPropertyChanged]
public string SchemaLinkButtonlabel { get; set; }
```

#### Setting a reference to `System.Windows.Forms`

Some Windows features in WPF (file open/save dialogs for example) require the project have a reference to `System.Windows.Forms`. For some silly reason, at least in .NET 6, this must be done in the `.csproj` file. Use the `UseWindowsForms` element as shown below to add a reference in your WPF project file to `System.Windows.Forms`.

```
<PropertyGroup>
...
  <UseWindowsForms>true</UseWindowsForms>
...  
</PropertyGroup>
```

#### Add images to the Projects resource folder

When you add images to your project, be sure to set their "Build Action" property to `Resource`.

![](https://rogerpence.dev/wp-content/uploads/2022/03/BmTGIdPz3U.png)

And, while you're at it, whine a little about WPF not supporting SVG files! (The upcoming .NET Maui does!)

#### A link label control

Curiously, WPF doesn't have a LinkLabel control (like WinForms does). However, [this blog post](https://akashsoni7.blogspot.com/2012/11/wpf-hyperlink-button-using-style-and.html) provides the best way I've found to implement a link label in WPF. 

Add the style below in your WPF project's `App.xaml` file (you may want to tweak some of the properties for your application):

```
<Style  x:Key="HyperLinkButtonStyle" TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <TextBlock TextDecorations="Underline">
                    <ContentPresenter TextBlock.FontFamily="Segoe UI"
                                      TextBlock.FontSize="13"/>
                </TextBlock>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
    <Setter Property="Foreground" Value="WhiteSmoke" />
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="true">
            <Setter Property="Foreground" Value="CornflowerBlue" />
            <Setter Property="Cursor" Value="Hand" />
        </Trigger>
    </Style.Triggers>
</Style>
```

And apply that style to a `<Button>` as shown below: 

```
<Button Style="{StaticResource HyperLinkButtonStyle}" 
    Margin="5,0,0,0"
    Content="Open template path" 
    HorizontalAlignment="Center" 
    VerticalAlignment="Center" 
    Click="button_openTemplatePath"/>
```    
Boom. Problem solved. 