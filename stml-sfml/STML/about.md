# Simple Text Markup Language
```
# tmsl
Text: Это Текст
hr:
Image: CuteDog.png
Button: doges.tmsl: Смотрите страницу о собаках
```
Замены
```
<gh> - ":"
<a> - "<"
<b> - ">"
```
## Браузер
### ACF
Automatic Control Forms
надо управлять формами
### Sockets
Сокеты
### Syntax Analyzer & Parser
``` c#
For line in code {
	if line.length == 0 {
		continue;
	} 
	let sp = line.split(": ");
	if sp.length == 0 {
		continue;
	}
	if sp.length == 1 {
		if sp[0] == "hr" {
			Commands.Add(new Hr())
		}
	}
	if sp.length == 2 {
		if sp[0] == "Text" {
			Commands.Add(new Text(sp[1]))
		}
		if sp[0] == "Image" {
			Commands.Add(new Image(sp[1]))
		}
	}
	if sp.length == 3 {
		if sp[0] == "Button" {
			Commands.Add(new Button(sp[1], sp[2]))
		}
	}
}
```