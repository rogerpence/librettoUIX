
## LibrettoXUI

Microsoft introduced WPF in 2006. I dabbled with it briefly then but never wrote anything beyond "Hello, world." Other shiny balls got my attention and I never returned to WPF, until the other day. 

This WPF app puts a user interface over a Python command-line code generator I wrote. The generator and its command line interface work just fine. However, a point-and-click way to use it would be sure be nice. And, while I'm at it, a way to store a code generation "set" to return to later would sure be nice. That's what this little WPF app does. 

![](https://rogerpence.dev/wp-content/uploads/2022/03/LibrettoUI-2_cpsO6sRCjY.png)

I strongly suspect that despite my best efforts to write this little naively, my WPF techniques remain naive and crude. But the app goes the job. 

### Interesting things 

#### A link label control

Curiously, WPF doesn't have a LinkLabel control. However, [this blog post](https://akashsoni7.blogspot.com/2012/11/wpf-hyperlink-button-using-style-and.html) provides the best way I found to implement a link lable in WPF. 

Add this style in the `App.xaml` and the problem is easily solved. 

```
<Style  x:Key="HyperLinkButtonStyle" TargetType="Button">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <TextBlock TextDecorations="Underline">
                        <ContentPresenter TextBlock.FontFamily="Segoe UI" TextBlock.FontSize="13"/>
                </TextBlock>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
    <Setter Property="Foreground" Value="WhiteSmoke" />
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="true">
            <Setter   Property="Foreground" Value="CornflowerBlue" />
            <Setter Property="Cursor" Value="Hand" />
        </Trigger>
    </Style.Triggers>
</Style>
```

Apply the style as shown below: 

```
<Button Style="{StaticResource HyperLinkButtonStyle}" 
    Margin="5,0,0,0"
    Content="Open template path" 
    HorizontalAlignment="Center" 
    VerticalAlignment="Center" 
    Visibility="{Binding Path=SchemasPopulated, 
                 Converter={StaticResource Converter}}"
    Click="button_openTemplatePath"/>
```    